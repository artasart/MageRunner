using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Enums;

public class GoogleLoginManager : MonoBehaviour
{
	public void StartGoogleLogin()
	{
		TheBackend.ToolKit.GoogleLogin.iOS.GoogleLogin(GoogleLoginCallback);
	}

	private void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
	{
		if (isSuccess == false)
		{
			GameManager.Scene.ShowToastAndDisappear("Google Login failed.");

			return;
		}

		var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);

		Debug.Log(bro.GetStatusCode());


		FindObjectOfType<Scene_Logo>().StartLogin();

		PlayerPrefs.SetString(Define.LOGINTYPE, LoginType.Google.ToString());

		//			GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(ModalType.Confrim,
		//$"This account is currently being withdrawn.\nPlease try latter.\n\n" +
		//$"<size=25><color=#323232>processed ususally takes within an hour</size></color>", "Notice", () =>
		//{
		//	Application.Quit();
		//});		
	}

	public void SignOutGoogleLogin()
	{
		TheBackend.ToolKit.GoogleLogin.iOS.GoogleSignOut(GoogleSignOutCallback);
	}

	private void GoogleSignOutCallback(bool isSuccess, string error)
	{
		if (isSuccess == false)
		{
			GameManager.Scene.ShowToastAndDisappear("Google sign out failed. Try again.");
		}

		else
		{
			GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(
			ModalType.Confrim,
			"You have signed out successfully!\nmoving to login...", "Notice",
			() =>
			{
				GameManager.Scene.LoadScene(SceneName.Logo);
			});
		}
	}

	public void Revoke()
	{
		GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(ModalType.Confrim,
			$"Application need to be restarted.",
			"Notice",
			Application.Quit);
	}
}