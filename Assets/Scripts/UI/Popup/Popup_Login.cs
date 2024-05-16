using UnityEngine;
using UnityEngine.UI;

public class Popup_Login : Popup_Base
{
	Button btn_AppleLogin;
	Button btn_GoogleLogin;
	Button btn_GuestLogin;
	Button btn_Privacy;
	Button btn_Terms;

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();
	}

	private void Start()
	{
		btn_GuestLogin = GetUI_Button(nameof(btn_GuestLogin), OnClick_GuestLogin, useAnimation: true);
		btn_GuestLogin.onClick.RemoveListener(OpenSound);

		btn_AppleLogin = GetUI_Button(nameof(btn_AppleLogin), Onclick_AppleLogin, useAnimation: true);
		btn_AppleLogin.onClick.RemoveListener(OpenSound);

		btn_GoogleLogin = GetUI_Button(nameof(btn_GoogleLogin), OnClick_GoogleLogin, useAnimation: true);
		btn_GoogleLogin.onClick.RemoveListener(OpenSound);

		btn_Privacy = GetUI_Button(nameof(btn_Privacy), OnClick_Privacy, useAnimation: true);
		btn_Terms = GetUI_Button(nameof(btn_Terms), OnClick_Terms, useAnimation: true);
	}

	private void OnClick_GuestLogin()
	{
		DebugManager.Log("Guest Login", DebugColor.Login);

		GameManager.Backend.GuestLogin();
	}

	private void Onclick_AppleLogin()
	{
		DebugManager.Log("Apple Login", DebugColor.Login);

		FindObjectOfType<AppleLoginManager>().SignInWithApple();
	}

	private void OnClick_GoogleLogin()
	{
		DebugManager.Log("Google Login", DebugColor.Login);

		FindObjectOfType<GoogleLoginManager>().StartGoogleLogin();
	}

	private void OnClick_Privacy()
	{
		DebugManager.Log("Privacy Clicked.", DebugColor.Login);

		Application.OpenURL("https://sites.google.com/view/mindshelter/home/privacy-notice");
	}

	private void OnClick_Terms()
	{
		DebugManager.Log("Terms Clicked", DebugColor.Login);

		Application.OpenURL("https://sites.google.com/view/mindshelter/home/privacy-notice");
	}
}
