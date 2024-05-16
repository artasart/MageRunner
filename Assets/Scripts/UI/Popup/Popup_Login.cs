using BackEnd;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Login : Popup_Base
{
	Button btn_AppleLogin;
	Button btn_GoogleLogin;
	Button btn_GuestLogin;
	Button btn_Privacy;
	Button btn_Terms;

	private void OnDisable()
	{
		this.transform.Search("group_Modal").GetComponent<RectTransform>().localScale = Vector3.zero;
	}

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		UseDimClose();

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


	private void OnClick_ChangeFederationToGoogle()
	{
		OnClick_GoogleLogin();

		Util.RunCoroutine(Co_ChangeToGoogle(), nameof(Co_ChangeToGoogle));
	}

	private IEnumerator<float> Co_ChangeToGoogle()
	{
		yield return Timing.WaitUntilTrue(() => PlayerPrefs.GetString(Define.GOOGLETOKEN) != string.Empty);

		BackendReturnObject bro = Backend.BMember.ChangeCustomToFederation(PlayerPrefs.GetString(Define.GOOGLETOKEN), FederationType.Google);

		if (bro.IsSuccess())
		{
			GameManager.UI.StackPopup<Popup_Basic>().SetPopupInfo(ModalType.Confrim, "Scene will be restarted to initialize data.", "Notice", () =>
			{
				GameManager.Scene.LoadScene(SceneName.Main);
			});
		}
	}

	private void OnClick_ChangeFederationToApple()
	{
		Onclick_AppleLogin();

		Util.RunCoroutine(Co_ChangeToApple(), nameof(Co_ChangeToApple));
	}

	private IEnumerator<float> Co_ChangeToApple()
	{
		yield return Timing.WaitUntilTrue(() => PlayerPrefs.GetString(Define.APPLEUSERID) != string.Empty);

		BackendReturnObject bro = Backend.BMember.ChangeCustomToFederation(PlayerPrefs.GetString(Define.APPLEUSERID), FederationType.Apple);

		if (bro.IsSuccess())
		{
			GameManager.UI.StackPopup<Popup_Basic>().SetPopupInfo(ModalType.Confrim, "Scene will be restarted to initialize data.", "Notice", () =>
			{
				GameManager.Scene.LoadScene(SceneName.Main);
			});
		}
	}

	public void HideGuestLogin()
	{
		btn_GuestLogin.gameObject.SetActive(false);

		btn_AppleLogin.onClick.RemoveAllListeners();
		btn_AppleLogin = GetUI_Button(nameof(btn_AppleLogin), OnClick_ChangeFederationToApple, useAnimation: true);
		btn_AppleLogin.onClick.RemoveListener(OpenSound);

		btn_GoogleLogin.onClick.RemoveAllListeners();
		btn_GoogleLogin = GetUI_Button(nameof(btn_GoogleLogin), OnClick_ChangeFederationToGoogle, useAnimation: true);
		btn_GoogleLogin.onClick.RemoveListener(OpenSound);
	}
}
