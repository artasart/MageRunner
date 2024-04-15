using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_HUD : Panel_Base
{
	Button btn_Left;
	Button btn_Right;
	Button btn_Down;
	Button btn_Up;

	TMP_Text txtmp_Coin;
	TMP_Text txtmp_Score;

	protected override void Awake()
	{
		base.Awake();

		txtmp_Coin = GetUI_TMPText(nameof(txtmp_Coin), "0");
		txtmp_Score = GetUI_TMPText(nameof(txtmp_Score), "0");

		btn_Left = GetUI_Button(nameof(btn_Left), OnClick_Left);
		btn_Right = GetUI_Button(nameof(btn_Right), OnClick_Right);
		btn_Down = GetUI_Button(nameof(btn_Down), OnClick_Down);
		btn_Up = GetUI_Button(nameof(btn_Up), OnClick_Up);

		btn_Left.UseAnimation();
		btn_Right.UseAnimation();
		btn_Down.UseAnimation();
		btn_Up.UseAnimation();
	}

	private void OnClick_Left()
	{
		Debug.Log("OnClick_Left");
	}

	private void OnClick_Right()
	{
		Debug.Log("OnClick_Right");
	}

	private void OnClick_Down()
	{
		Debug.Log("OnClick_Down");
	}

	private void OnClick_Up()
	{
		Debug.Log("OnClick_Up");
	}


	public void SaveScore()
	{
		RefreshUI();
	}

	public void RefreshUI()
	{
		DebugManager.Log("UI initialized.");

		txtmp_Score.text = 0.ToString("N0");
		txtmp_Coin.text = 0.ToString("N0");
	}

	public void SetScoreUI(int amount)
	{
		txtmp_Score.text = amount.ToString("N0");
	}

	public void SetCoinUI(int amount)
	{
		txtmp_Coin.text = amount.ToString("N0");
	}
}
