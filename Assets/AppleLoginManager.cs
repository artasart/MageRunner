using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
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

		this.InitializeLogin();
	}

	private void Update()
	{
		if (this._appleAuthManager != null)
		{
			this._appleAuthManager.Update();
		}
	}

	private void InitializeLogin()
	{
		if (this._appleAuthManager == null)
		{
			return;
		}

		this._appleAuthManager.SetCredentialsRevokedCallback(result =>
		{
			PlayerPrefs.DeleteKey(AppleUserIdKey);
		});
	}

	public void Login()
	{
		var loginArgs = new AppleAuthLoginArgs(LoginOptions.None);

		this._appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{
				var appleIdCredential = credential as IAppleIDCredential;
				var passwordCredential = credential as IPasswordCredential;

				if (appleIdCredential.State != null) GameManager.UI.FetchPanel<Panel_Logo>().Message("Credential state is empty.");

				if (appleIdCredential.IdentityToken != null)
				{
					var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

					GameManager.UI.FetchPanel<Panel_Logo>().Message(identityToken);
				}

				if (appleIdCredential.AuthorizationCode != null)
				{
					var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);

					GameManager.UI.FetchPanel<Panel_Logo>().Message(authorizationCode);
				}

			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				GameManager.UI.FetchPanel<Panel_Logo>().Message("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	public void LogOut()
	{
		DebugManager.Log("Apple Logout.", DebugColor.Login);
	}
}