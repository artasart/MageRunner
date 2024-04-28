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

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		txtmp_Score = GetUI_TMPText(nameof(txtmp_Score), string.Empty);

		btn_Reward = GetUI_Button(nameof(btn_Reward), OnClick_Reward, useAnimation:true);
		btn_Retry = GetUI_Button(nameof(btn_Retry), OnClick_Retry, useAnimation: true);
		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home, useAnimation: true);
	}

	private void OnClick_Reward()
	{
		btn_Reward.interactable = false;

		Scene.game.gold *= 2;
	}

	private void OnClick_Retry()
	{
		if (LocalData.gameData.energy <= 0)
		{
			Debug.Log("No Energy");

			return;
		}

		btn_Reward.interactable = true;

		Scene.game.Replay();
	}

	private void OnClick_Home()
	{
		Scene.game.SaveGameData();

		GameManager.Scene.LoadScene(SceneName.Main);
	}

	public void SetResult(int score, int gold, int exp)
	{
		txtmp_Score.text = $"You ran {score}m, gaiend {gold} Golds & {exp} exp!";

		Scene.game.AddExp(exp);
		Scene.game.SaveGameData();
	}
}
