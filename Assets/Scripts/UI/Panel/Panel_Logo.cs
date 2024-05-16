using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Logo : Panel_Base
{
	TMP_Text txtmp_Download;
	TMP_Text txtmp_LoginMessage;
	TMP_Text txtmp_PressToStart;

	protected override void Awake()
	{
		base.Awake();

		txtmp_PressToStart = GetUI_TMPText(nameof(txtmp_PressToStart), "Touch screen to start..!");
		txtmp_PressToStart.UsePingPong();

		txtmp_Download = GetUI_TMPText(nameof(txtmp_Download), "loading...");
		txtmp_LoginMessage = GetUI_TMPText(nameof(txtmp_LoginMessage), "username");
		txtmp_Download.UsePingPong();
		txtmp_Download.StartPingPong(.25f);

		txtmp_Download.gameObject.SetActive(false);
		txtmp_PressToStart.gameObject.SetActive(false);

		img_Background.GetComponent<Button>().onClick.AddListener(OnClick_StackPopup);
		img_Background.GetComponent<Button>().interactable = false;
	}

	public void SetDownload(string message)
	{
		txtmp_Download.text = message;
	}
	public void SetMessage(string message)
	{
		txtmp_LoginMessage.text = message;
	}

	public void StartLogin(Action _action = null)
	{
		FindObjectOfType<Scene_Logo>().player.gameObject.SetActive(true);

		txtmp_Download.gameObject.SetActive(true);
	}

	public void HideDownload()
	{
		img_Background.GetComponent<Button>().interactable = true;

		txtmp_Download.gameObject.SetActive(false);

		txtmp_PressToStart.gameObject.SetActive(true);
		txtmp_PressToStart.StartPingPong(.25f);

		GameObject.Find(Define.PLAYER).SetActive(false);
	}

	private void OnClick_StackPopup()
	{
		GameManager.UI.StackPopup<Popup_Login>();
	}
}