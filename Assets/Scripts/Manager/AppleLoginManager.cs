using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Net;
using System.Text;
using UnityEngine;

public class AppleLoginManager : MonoBehaviour
{
	private const string AppleUserIdKey = "AppleUserId";

	private IAppleAuthManager _appleAuthManager;

	private void Awake()
	{
		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			var deserializer = new PayloadDeserializer();

			this._appleAuthManager = new AppleAuthManager(deserializer);
		}
	}

    private void Start()
    {
		GameManager.UI.FetchPanel<Panel_Logo>().SetMessage(PlayerPrefs.HasKey(AppleUserIdKey).ToString());

		if (PlayerPrefs.HasKey(AppleUserIdKey))
		{
			GameManager.UI.FetchPanel<Panel_Logo>().SetMessage(PlayerPrefs.GetString(AppleUserIdKey));

			var storedAppleUserId = PlayerPrefs.GetString(AppleUserIdKey);

			this.CheckCredentialStatusForUserId(storedAppleUserId);
		}
	}

    private void CheckCredentialStatusForUserId(string appleUserId)
	{
		GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Has Key");

		// If there is an apple ID available, we should check the credential state
		this._appleAuthManager.GetCredentialState(
			appleUserId,
			state =>
			{
				switch (state)
				{
					// If it's authorized, login with that user id
					case CredentialState.Authorized:
						GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Authorized");

						GameManager.Backend.LoginASAP(FindObjectOfType<Scene_Logo>().StartLogin);
						return;

					// If it was revoked, or not found, we need a new sign in with apple attempt
					// Discard previous apple user id
					case CredentialState.Revoked:
					case CredentialState.NotFound:
						GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("NotFound");
						PlayerPrefs.DeleteKey(AppleUserIdKey);
						return;
				}
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());

				Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());

			});
	}

	private void Update()
	{
		if (this._appleAuthManager != null)
		{
			this._appleAuthManager.Update();
		}
	}

	public void SignInWithApple()
	{
		var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

		this._appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{
				GameManager.Scene.Dim(true);

				PlayerPrefs.SetString(AppleUserIdKey, credential.User);

				var appleIdCredential = credential as IAppleIDCredential;
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
				
				GameManager.Backend.LoginToBackEnd(identityToken, FindObjectOfType<Scene_Logo>().StartLogin);
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}
}
