using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Popup_GameOver : Popup_Base
{
	Button btn_Reward;
	Button btn_Retry;
	Button btn_Home;

	TMP_Text txtmp_Score;
	TMP_Text txtmp_Energy;

	Transform group_Menu_Horizontal;

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		txtmp_Score = GetUI_TMPText(nameof(txtmp_Score), string.Empty);
		txtmp_Energy = GetUI_TMPText(nameof(txtmp_Energy), string.Empty);

		btn_Reward = GetUI_Button(nameof(btn_Reward), OnClick_Reward, useAnimation:true);
		btn_Retry = GetUI_Button(nameof(btn_Retry), OnClick_Retry, useAnimation: true);
		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home, useAnimation: true);

		group_Menu_Horizontal = this.transform.Search(nameof(group_Menu_Horizontal));
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
			btn_Retry.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

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

		if (score > LocalData.gameData.highScore)
		{
			group_Menu_Horizontal.GetComponent<CanvasGroup>().blocksRaycasts = false;

			this.highestScore = score;

			ShowHighScore();
		}

		Scene.game.AddExp(exp);
		Scene.game.SaveGameData();

		txtmp_Energy.text = $"{LocalData.gameData.energy}/{LocalData.gameData.energyTotal}";
	}

	int highestScore;

	private void ShowHighScore()
	{
		group_Menu_Horizontal.GetComponent<CanvasGroup>().blocksRaycasts = true;

		LocalData.gameData.highScore = highestScore;

		GameManager.UI.StackSplash<Splash_Congrates>();
	}
}
