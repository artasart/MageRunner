using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_GameOver : Popup_Base
{
	Button btn_Reward;
	Button btn_Retry;
	Button btn_Home;

	TMP_Text txtmp_Score;
	TMP_Text txtmp_Coin;
	TMP_Text txtmp_Exp;

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		txtmp_Score = GetUI_TMPText(nameof(txtmp_Score), string.Empty);
		txtmp_Coin = GetUI_TMPText(nameof(txtmp_Coin), string.Empty);
		txtmp_Exp = GetUI_TMPText(nameof(txtmp_Exp), string.Empty);

		btn_Reward = GetUI_Button(nameof(btn_Reward), OnClick_Reward);
		btn_Retry = GetUI_Button(nameof(btn_Retry), OnClick_Retry);
		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home);

		btn_Reward.UseAnimation();
		btn_Retry.UseAnimation();
		btn_Home.UseAnimation();
	}

	private void OnClick_Reward()
	{
		btn_Reward.interactable = false;

		FindObjectOfType<Scene_Game>().coin *= 2;
	}

	private void OnClick_Retry()
	{
		btn_Reward.interactable = true;

		FindObjectOfType<Scene_Game>().Retry();
	}

	private void OnClick_Home()
	{
		GameManager.Scene.LoadScene(SceneName.Main);
	}

	public void SetResult(int score, int coin, int exp)
	{
		txtmp_Score.text = score.ToString("N0");
		txtmp_Coin.text = coin.ToString("N0");
		txtmp_Exp.text = exp.ToString();

		FindObjectOfType<Scene_Game>().SaveGameData();
	}
}
