using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		FindObjectOfType<Scene_Logo>().StartLogin();

		PlayerPrefs.SetString("GoogleLogin", "Login");
	}

	public void SignOutGoogleLogin()
	{
		TheBackend.ToolKit.GoogleLogin.iOS.GoogleSignOut(GoogleSignOutCallback);
	}

	private void GoogleSignOutCallback(bool isSuccess, string error)
	{
		if (isSuccess == false)
		{
			DebugManager.Log("Google Login error.", DebugColor.Login);

			GameManager.Scene.ShowToastAndDisappear("Google sign out failed.");
		}

		else
		{
			DebugManager.Log("Google SignOut success.", DebugColor.Login);

			GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(ModalType.Confrim, "You have signed out successfully!\nmoving to login...", "Notice",
				() => {
					GameManager.Scene.LoadScene(SceneName.Logo);

					PlayerPrefs.SetString("GoogleLogin", "Logout");
				});
		}
	}

}