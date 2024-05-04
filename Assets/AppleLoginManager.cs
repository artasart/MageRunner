using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class AppleLoginManager : MonoBehaviour
{
	private const string AppleUserIdKey = "AppleUserId";

	private IAppleAuthManager _appleAuthManager;

	private void Start()
	{
		// If the current platform is supported
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
			Debug.Log("#  ���� ����  #");
			return;
		}

		this._appleAuthManager.SetCredentialsRevokedCallback(result =>
		{
			Debug.Log("#  �α��� ���� ����  #");
			Debug.Log("Received revoked callback " + result);
			// this.SetupLoginMenuForSignInWithApple();
			PlayerPrefs.DeleteKey(AppleUserIdKey);
		});
	}

	public void login()
	{

		var loginArgs = new AppleAuthLoginArgs(LoginOptions.None);
		Debug.Log("#  �α��� ��ư Ŭ��  #");

		this._appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{

				Debug.Log("#  �α��� ����  #");
				Debug.Log("# userID: #");
				Debug.Log(credential.User);
				var appleIdCredential = credential as IAppleIDCredential;
				var passwordCredential = credential as IPasswordCredential;

				if (appleIdCredential.State != null)
					Debug.Log("# State: #");
				Debug.Log(appleIdCredential.State);

				if (appleIdCredential.IdentityToken != null)
				{
					var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
					Debug.Log("# identityToken:  #");
					Debug.Log(identityToken);
				}

				if (appleIdCredential.AuthorizationCode != null)
				{
					var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);
					Debug.Log("# authorizationCode:  #");
					Debug.Log(authorizationCode);
				}

			},
			error =>
			{
				Debug.Log("#  �α��� ����  #");
				var authorizationErrorCode = error.GetAuthorizationErrorCode();
				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	public void LogOut()
	{
		//�丸 ����
		Debug.Log("#  �α׾ƿ��Ǿ����ϴ�  #");
	}
}