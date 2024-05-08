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
	private IAppleAuthManager _appleAuthManager;

	private void Awake()
	{
		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			var deserializer = new PayloadDeserializer();

			this._appleAuthManager = new AppleAuthManager(deserializer);
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

		this._appleAuthManager?.SetCredentialsRevokedCallback(result =>
		{
			Debug.Log("Received revoked callback " + result);

			PlayerPrefs.DeleteKey(Define.APPLEUSERID);

			GameManager.Scene.LoadScene(SceneName.Logo);
		});
	}

	public void CheckCredentialStatusForUserId(string appleUserId)
	{
		this._appleAuthManager.GetCredentialState(
			appleUserId,
			state =>
			{
				switch (state)
				{
					case CredentialState.Authorized:
						GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Authorized");
						GameManager.Backend.LoginASAP(FindObjectOfType<Scene_Logo>().StartLogin);
						return;

					case CredentialState.Revoked:
					case CredentialState.NotFound:
						GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("NotFound");
						PlayerPrefs.DeleteKey(Define.APPLEUSERID);
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

				PlayerPrefs.SetString(Define.APPLEUSERID, credential.User);

				var appleIdCredential = credential as IAppleIDCredential;
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

				SetupAppleData(credential.User, credential);

				//appleIdCredential.IdentityToken
				GameManager.Backend.LoginToBackEnd(identityToken, FindObjectOfType<Scene_Logo>().StartLogin);
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	public void SetupAppleData(string appleUserId, ICredential receivedCredential)
	{
		var appleIdCredential = receivedCredential as IAppleIDCredential;

		if (appleIdCredential != null)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("RECEIVED APPLE ID CREDENTIAL.\nYOU CAN LOGIN/CREATE A USER WITH THIS");
			stringBuilder.AppendLine("<b>Username:</b> " + appleIdCredential.User);
			stringBuilder.AppendLine("<b>Real user status:</b> " + appleIdCredential.RealUserStatus.ToString());
			
			PlayerPrefs.SetString("Username", appleIdCredential.User);

			if (appleIdCredential.State != null)
				stringBuilder.AppendLine("<b>State:</b> " + appleIdCredential.State);

			if (appleIdCredential.IdentityToken != null)
			{
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

				stringBuilder.AppendLine("<b>Identity token (" + appleIdCredential.IdentityToken.Length + " bytes)</b>");
				stringBuilder.AppendLine(identityToken.Substring(0, 45) + "...");

				PlayerPrefs.SetString("IdentityToken", identityToken);
			}

			if (appleIdCredential.AuthorizationCode != null)
			{
				var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);
				stringBuilder.AppendLine("<b>Authorization Code (" + appleIdCredential.AuthorizationCode.Length + " bytes)</b>");
				stringBuilder.AppendLine(authorizationCode.Substring(0, 45) + "...");

				PlayerPrefs.SetString("authorizationCode", authorizationCode);
			}

			if (appleIdCredential.AuthorizedScopes != null)
				stringBuilder.AppendLine("<b>Authorized Scopes:</b> " + string.Join(", ", appleIdCredential.AuthorizedScopes));

			if (appleIdCredential.Email != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("<b>EMAIL RECEIVED: YOU WILL ONLY SEE THIS ONCE PER SIGN UP. SEND THIS INFORMATION TO YOUR BACKEND!</b>");
				stringBuilder.AppendLine("<b>You can test this again by revoking credentials in Settings</b>");
				stringBuilder.AppendLine("<b>Email:</b> " + appleIdCredential.Email);

				PlayerPrefs.SetString("authorizationCode", appleIdCredential.Email);
			}

			if (appleIdCredential.FullName != null)
			{
				var fullName = appleIdCredential.FullName;
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("<b>NAME RECEIVED: YOU WILL ONLY SEE THIS ONCE PER SIGN UP. SEND THIS INFORMATION TO YOUR BACKEND!</b>");
				stringBuilder.AppendLine("<b>You can test this again by revoking credentials in Settings</b>");
				stringBuilder.AppendLine("<b>Name:</b> " + fullName.ToLocalizedString());
				stringBuilder.AppendLine("<b>Name (Short):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Short));
				stringBuilder.AppendLine("<b>Name (Medium):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Medium));
				stringBuilder.AppendLine("<b>Name (Long):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Long));
				stringBuilder.AppendLine("<b>Name (Abbreviated):</b> " + fullName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));

				PlayerPrefs.SetString("fullName", appleIdCredential.FullName.ToLocalizedString());

				if (appleIdCredential.FullName.PhoneticRepresentation != null)
				{
					var phoneticName = appleIdCredential.FullName.PhoneticRepresentation;
					stringBuilder.AppendLine("<b>Phonetic name:</b> " + phoneticName.ToLocalizedString());
					stringBuilder.AppendLine("<b>Phonetic name (Short):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Short));
					stringBuilder.AppendLine("<b>Phonetic name (Medium):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Medium));
					stringBuilder.AppendLine("<b>Phonetic name (Long):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Long));
					stringBuilder.AppendLine("<b>Phonetic name (Abbreviated):</b> " + phoneticName.ToLocalizedString(PersonNameFormatterStyle.Abbreviated));

					PlayerPrefs.SetString("phoneticName", phoneticName.ToLocalizedString());
				}
			}
		}
	}
}
