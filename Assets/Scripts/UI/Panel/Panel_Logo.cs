using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Logo : Panel_Base
{
	TMP_Text txtmp_Download;
	TMP_Text txtmp_LoginMessage;

	protected override void Awake()
	{
		base.Awake();

		txtmp_Download = GetUI_TMPText(nameof(txtmp_Download), "loading...");
		txtmp_LoginMessage = GetUI_TMPText(nameof(txtmp_LoginMessage), "username");
		txtmp_Download.UsePingPong();
		txtmp_Download.StartPingPong(.25f);

		txtmp_Download.gameObject.SetActive(false);
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
}