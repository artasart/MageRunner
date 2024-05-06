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

	private void Awake()
	{
		if (AppleAuthManager.IsCurrentPlatformSupported)
		{

			var deserializer = new PayloadDeserializer();

			this._appleAuthManager = new AppleAuthManager(deserializer);
		}
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
				PlayerPrefs.SetString(AppleUserIdKey, credential.User);

				var appleIdCredential = credential as IAppleIDCredential;
				var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

				GameManager.Scene.Dim(false);

				GameManager.Backend.LoginToBackEnd(identityToken, FindObjectOfType<Scene_Logo>().StartLogin);
			},
			error =>
			{
				var authorizationErrorCode = error.GetAuthorizationErrorCode();

				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}
}
