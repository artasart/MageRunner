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
			Debug.Log("#  지원 안함  #");
			return;
		}

		this._appleAuthManager.SetCredentialsRevokedCallback(result =>
		{
			Debug.Log("#  로그인 세션 삭제  #");
			Debug.Log("Received revoked callback " + result);
			// this.SetupLoginMenuForSignInWithApple();
			PlayerPrefs.DeleteKey(AppleUserIdKey);
		});
	}

	public void login()
	{

		var loginArgs = new AppleAuthLoginArgs(LoginOptions.None);
		Debug.Log("#  로그인 버튼 클릭  #");

		this._appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{

				Debug.Log("#  로그인 성공  #");
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
				Debug.Log("#  로그인 실패  #");
				var authorizationErrorCode = error.GetAuthorizationErrorCode();
				Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
			});
	}

	public void LogOut()
	{
		//뷰만 관리
		Debug.Log("#  로그아웃되었습니다  #");
	}
}