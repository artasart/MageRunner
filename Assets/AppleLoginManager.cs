using System.Collections.Generic;
using System;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Unity.VisualScripting;
using UnityEngine;
using System.Security.Cryptography;
using Firebase.Auth;

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
		PerformLoginWithAppleIdAndFirebase((res) => { 
			Debug.Log(res.Email); 
			Debug.Log(res.IsValid());

			GameManager.UI.FetchPanel<Panel_Logo>().Message(res.Email);
		});

		//var loginArgs = new AppleAuthLoginArgs(LoginOptions.None);

		//this._appleAuthManager.LoginWithAppleId(
		//	loginArgs,
		//	credential =>
		//	{
		//		var appleIdCredential = credential as IAppleIDCredential;
		//		var passwordCredential = credential as IPasswordCredential;

		//		if (appleIdCredential.State != null) GameManager.UI.FetchPanel<Panel_Logo>().Message("Credential state is empty.");

		//		if (appleIdCredential.IdentityToken != null)
		//		{
		//			var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);

		//			GameManager.UI.FetchPanel<Panel_Logo>().Message("Token : " + identityToken + " FullName : " + appleIdCredential.FullName.ToString());
		//		}

		//		if (appleIdCredential.AuthorizationCode != null)
		//		{
		//			var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode, 0, appleIdCredential.AuthorizationCode.Length);

		//			GameManager.UI.FetchPanel<Panel_Logo>().Message(authorizationCode);
		//		}

		//	},
		//	error =>
		//	{
		//		var authorizationErrorCode = error.GetAuthorizationErrorCode();

		//		GameManager.UI.FetchPanel<Panel_Logo>().Message("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
		//	});
	}

	public void LogOut()
	{
		DebugManager.Log("Apple Logout.", DebugColor.Login);
	}

	private static string GenerateRandomString(int length)
	{
		if (length <= 0)
		{
			throw new Exception("Expected nonce to have positive length");
		}

		const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
		var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
		var result = string.Empty;
		var remainingLength = length;

		var randomNumberHolder = new byte[1];
		while (remainingLength > 0)
		{
			var randomNumbers = new List<int>(16);
			for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
			{
				cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
				randomNumbers.Add(randomNumberHolder[0]);
			}

			for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
			{
				if (remainingLength == 0)
				{
					break;
				}

				var randomNumber = randomNumbers[randomNumberIndex];
				if (randomNumber < charset.Length)
				{
					result += charset[randomNumber];
					remainingLength--;
				}
			}
		}

		return result;
	}

	private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
	{
		var sha = new SHA256Managed();
		var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
		var hash = sha.ComputeHash(utf8RawNonce);

		var result = string.Empty;
		for (var i = 0; i < hash.Length; i++)
		{
			result += hash[i].ToString("x2");
		}

		return result;
	}

	public void PerformLoginWithAppleIdAndFirebase(Action<FirebaseUser> firebaseAuthCallback)
	{
		var rawNonce = GenerateRandomString(32);
		var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

		var loginArgs = new AppleAuthLoginArgs(
			LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
			nonce);

		this._appleAuthManager.LoginWithAppleId(
			loginArgs,
			credential =>
			{
				var appleIdCredential = credential as IAppleIDCredential;
				if (appleIdCredential != null)
				{
					FindObjectOfType<FirebaseAppleLogin>().PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
				}
			},
			error =>
			{
				// Something went wrong
				GameManager.UI.FetchPanel<Panel_Logo>().Message("Something went wrong..");
			});
	}

	public void PerformQuickLoginWithFirebase(Action<FirebaseUser> firebaseAuthCallback)
	{
		var rawNonce = GenerateRandomString(32);
		var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

		var quickLoginArgs = new AppleAuthQuickLoginArgs(nonce);

		this._appleAuthManager.QuickLogin(
			quickLoginArgs,
			credential =>
			{
				var appleIdCredential = credential as IAppleIDCredential;
				if (appleIdCredential != null)
				{
					FindObjectOfType<FirebaseAppleLogin>().PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
				}
			},
			error =>
			{
				// Something went wrong
				GameManager.UI.FetchPanel<Panel_Logo>().Message("Something went wrong..");
			});
	}
}