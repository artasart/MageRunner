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

	private void Start()
	{
		if (PlayerPrefs.HasKey(Define.APPLEUSERID))
		{
			GameManager.UI.FetchPanel<Panel_Logo>().SetMessage(PlayerPrefs.GetString(Define.APPLEUSERID));

			var storedAppleUserId = PlayerPrefs.GetString(Define.APPLEUSERID);

			CheckCredentialStatusForUserId(storedAppleUserId);
		}
	}

	public void CheckCredentialStatusForUserId(string appleUserId)
	{
		GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Has Key");

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

				GameManager.Backend.LoginToBackEnd(identityToken, FindObjectOfType<Scene_Logo>().StartLogin);
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}
}
