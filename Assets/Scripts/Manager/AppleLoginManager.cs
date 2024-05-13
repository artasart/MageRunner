using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Text;
using UnityEngine;
using static Enums;

public class AppleLoginManager : MonoBehaviour
{
	private IAppleAuthManager appleAuthManager;

	private void Awake()
	{
		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			var deserializer = new PayloadDeserializer();

			this.appleAuthManager = new AppleAuthManager(deserializer);
		}
	}

	public void Init()
	{
		if (PlayerPrefs.HasKey(Define.APPLEUSERID))
		{
			GameManager.UI.FetchPanel<Panel_Logo>().SetMessage(PlayerPrefs.GetString(Define.APPLEUSERID));

			var storedAppleUserId = PlayerPrefs.GetString(Define.APPLEUSERID);

			CheckCredentialStatusForUserId(storedAppleUserId);
		}

		this.appleAuthManager?.SetCredentialsRevokedCallback(result =>
		{
			DebugManager.Log("Received revoked callback " + result);

			PlayerPrefs.DeleteKey(Define.APPLEUSERID);

			GameManager.Scene.LoadScene(SceneName.Logo);
		});
	}

	public void CheckCredentialStatusForUserId(string appleUserId)
	{
		this.appleAuthManager.GetCredentialState(
			appleUserId,
			state =>
			{
				switch (state)
				{
					case CredentialState.Authorized:
						GameManager.Backend.QuickLogin(FindObjectOfType<Scene_Logo>().StartLogin);
						return;

					case CredentialState.Revoked:
					case CredentialState.NotFound:
						GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(ModalType.Confrim, 
						$"This account is currently being withdrawn.\nPlease try latter.\n\n" +
						$"<size=25><color=#323232>processed ususally takes within an hour</size></color>", "Notice");

						return;
				}
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				DebugManager.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	private void Update()
	{
		if (this.appleAuthManager != null)
		{
			this.appleAuthManager.Update();
		}
	}

	public void SignInWithApple()
	{
		var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

		this.appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{
				GameManager.Scene.Dim(true);

				PlayerPrefs.SetString(Define.APPLEUSERID, credential.User);
				PlayerPrefs.SetString(Define.LOGINTYPE, LoginType.Apple.ToString());

				SetupAppleData(credential.User, credential);

				var appleIdCredential = credential as IAppleIDCredential;
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

				GameManager.Backend.Login(identityToken, FindObjectOfType<Scene_Logo>().StartLogin);
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				DebugManager.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	public void SetupAppleData(string appleUserId, ICredential receivedCredential)
	{
		var appleIdCredential = receivedCredential as IAppleIDCredential;

		if (appleIdCredential != null)
		{
			PlayerPrefs.SetString("Username", appleIdCredential.User);

			if (appleIdCredential.IdentityToken != null)
			{
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

				PlayerPrefs.SetString("IdentityToken", identityToken);
			}

			if (appleIdCredential.AuthorizationCode != null)
			{
				var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);

				PlayerPrefs.SetString("authorizationCode", authorizationCode);
			}

			if (appleIdCredential.Email != null)
			{
				PlayerPrefs.SetString("authorizationCode", appleIdCredential.Email);
			}

			if (appleIdCredential.FullName != null)
			{
				var fullName = appleIdCredential.FullName;

				PlayerPrefs.SetString("fullName", appleIdCredential.FullName.ToLocalizedString());

				if (appleIdCredential.FullName.PhoneticRepresentation != null)
				{
					var phoneticName = appleIdCredential.FullName.PhoneticRepresentation;

					PlayerPrefs.SetString("phoneticName", phoneticName.ToLocalizedString());
				}
			}
		}
	}
}
