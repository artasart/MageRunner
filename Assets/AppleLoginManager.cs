using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Text;
using UnityEngine;

public class AppleLoginManager : MonoBehaviour
{
	private const string AppleUserIdKey = "AppleUserId";

	private IAppleAuthManager _appleAuthManager;

	private void Start()
	{
		if (AppleAuthManager.IsCurrentPlatformSupported)
		{

			var deserializer = new PayloadDeserializer();

			this._appleAuthManager = new AppleAuthManager(deserializer);
		}

		InitializeLoginMenu();
	}

	private void InitializeLoginMenu()
	{
		if (this._appleAuthManager == null) return;


		this._appleAuthManager.SetCredentialsRevokedCallback(result =>
		{
			Debug.Log("Received revoked callback " + result);

			PlayerPrefs.DeleteKey(AppleUserIdKey);
		});

		// If we have an Apple User Id available, get the credential status for it
		if (PlayerPrefs.HasKey(AppleUserIdKey))
		{
			var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);

			this.CheckCredentialStatusForUserId(storedAppleUserId);
		}

		// If we do not have an stored Apple User Id, attempt a quick login
		else
		{
			this.AttemptQuickLogin();
		}
	}

	public void SignInWithApple()
	{
		Debug.Log("SignInWithApple");

		var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

		this._appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{
				PlayerPrefs.SetString(AppleUserIdKey, credential.User);

				var appleIdCredential = credential as IAppleIDCredential;
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

				GameManager.Backend.LoginToBackEnd(identityToken);
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	private void CheckCredentialStatusForUserId(string appleUserId)
	{
		Debug.Log("CheckCredentialStatusForUserId");

		// If there is an apple ID available, we should check the credential state
		this._appleAuthManager.GetCredentialState(
			appleUserId,
			state =>
			{
				switch (state)
				{
					// If it's authorized, login with that user id
					case CredentialState.Authorized:
						return;

					// If it was revoked, or not found, we need a new sign in with apple attempt
					// Discard previous apple user id
					case CredentialState.Revoked:
					case CredentialState.NotFound:
						PlayerPrefs.DeleteKey(AppleUserIdKey);
						return;
				}
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();
				Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	private void AttemptQuickLogin()
	{
		Debug.Log("AttemptQuickLogin");

		var quickLoginArgs = new AppleAuthQuickLoginArgs();

		// Quick login should succeed if the credential was authorized before and not revoked
		this._appleAuthManager.QuickLogin(
			quickLoginArgs,
			credential =>
			{
				// If it's an Apple credential, save the user ID, for later logins
				var appleIdCredential = credential as IAppleIDCredential;
				if (appleIdCredential != null)
				{
					PlayerPrefs.SetString(AppleUserIdKey, credential.User);
				}

				var quickCredential = appleIdCredential as IAppleIDCredential;
				var identityToken = Encoding.UTF8.GetString(quickCredential.IdentityToken, 0, quickCredential.IdentityToken.Length);

				GameManager.Backend.LoginToBackEnd(identityToken);
			},
			error =>
			{
				// If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
				var authorizationErrorCode = error.GetAuthorizationErrorCode();
				Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}
}
