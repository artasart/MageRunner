using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Logo : Panel_Base
{
	Button btn_AppleLogin;
	Button btn_GoogleLogin;

	TMP_Text txtmp_Download;
	TMP_Text txtmp_LoginMessage;

	Transform group_Login;
	CanvasGroup loginCanvasGroup;

	protected override void Awake()
	{
		base.Awake();

		txtmp_Download = GetUI_TMPText(nameof(txtmp_Download), "loading...");
		txtmp_LoginMessage = GetUI_TMPText(nameof(txtmp_LoginMessage), "username");
		txtmp_Download.UsePingPong();
		txtmp_Download.StartPingPong(.25f);

		group_Login = this.transform.Search(nameof(group_Login));
		loginCanvasGroup = group_Login.GetComponent<CanvasGroup>();
		group_Login.gameObject.SetActive(true);

		txtmp_Download.gameObject.SetActive(false);
	}

	private void Start()
	{
		btn_AppleLogin = GetUI_Button(nameof(btn_AppleLogin), Onclick_AppleLogin, useAnimation: true);
		btn_AppleLogin.onClick.RemoveListener(OpenSound);
		btn_GoogleLogin = GetUI_Button(nameof(btn_GoogleLogin), OnClick_GoogleLogin, useAnimation: true);
	}

	public void SetDownload(string message)
	{
		txtmp_Download.text = message;
	}
	public void SetMessage(string message)
	{
		txtmp_LoginMessage.text = message;
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

	public void StartLogin(bool isInstant = false, Action _action = null)
	{
		if (isInstant)
		{
			GameManager.Scene.Dim(false);

			loginCanvasGroup.alpha = 0f;
			loginCanvasGroup.blocksRaycasts = false;

			FindObjectOfType<Scene_Logo>().player.gameObject.SetActive(true);
			txtmp_Download.gameObject.SetActive(true);

			_action?.Invoke();
		}

		else
		{
			GameManager.UI.FadeCanvasGroup(loginCanvasGroup, 0f, _end: () =>
			{
				FindObjectOfType<Scene_Logo>().player.gameObject.SetActive(true);
				txtmp_Download.gameObject.SetActive(true);

				_action?.Invoke();
			});
		}
	}
}