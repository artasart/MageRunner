using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Tutorial : Panel_Base
{
	Button btn_Background;
	TMP_Text txtmp_Continue;

	private void OnEnable()
	{
		txtmp_Continue.StartPingPong(1f);
	}

	protected override void Awake()
	{
		base.Awake();

		txtmp_Continue = GetUI_TMPText(nameof(txtmp_Continue), "tab to continue...");
		txtmp_Continue.UsePingPong();

		btn_Background = GetUI_Button(nameof(btn_Background), OnClick_Background);
	}

	private void OnClick_Background()
	{
		GameManager.UI.FetchPanel<Panel_HUD>().ShowPanel();

		GameManager.UI.PopPanel();

		txtmp_Continue.StopPingPong();
	}
}
