using DG.Tweening;
using TMPro;
using UnityEngine;
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

		btn_Reward = GetUI_Button(nameof(btn_Reward), OnClick_Reward, useAnimation: true);

		btn_Retry = GetUI_Button(nameof(btn_Retry), OnClick_Retry, useAnimation: true);
		btn_Retry.onClick.RemoveListener(OpenSound);

		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home, useAnimation: true);

		group_Menu_Horizontal = this.transform.Search(nameof(group_Menu_Horizontal));
	}

	private void OnClick_Reward()
	{
		btn_Reward.GetComponent<CanvasGroup>().alpha = .1f;
		btn_Reward.GetComponent<CanvasGroup>().blocksRaycasts = false;

		GameManager.Scene.Dim(true);

		Invoke(nameof(StackSplash), .5f);
	}

	private void StackSplash()
	{
		GameManager.Scene.Dim(true);

		GameManager.UI.StackSplash<Splash_Gold>();
		GameManager.UI.FetchSplash<Splash_Gold>().OpenBox(10);

		Invoke(nameof(PopSplash), 3f);
	}

	private void PopSplash()
	{
		GameManager.UI.FetchSplash<Splash_Gold>().ShowMessage();
		GameManager.UI.PopSplash();
	}

	private void OnClick_Retry()
	{
		if (LocalData.gameData.energy <= 0)
		{
			GameManager.Sound.PlaySound(Define.SOUND_DENIED);

			btn_Retry.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			GameManager.Scene.ShowToastAndDisappear("Go home and get energy..!");

			btn_Retry.interactable = false;

			return;
		}

		GameManager.Sound.PlaySound(Define.SOUND_OPEN);

		GameScene.game.Replay();
	}

	private void OnClick_Home()
	{
		GameScene.game.SaveGameData();

		GameManager.Scene.LoadScene(SceneName.Main);
	}

	public void SetResult(int score, int gold, int exp)
	{
		txtmp_Score.text = $"You ran {score}m, gaiend {gold} golds & {exp} exp!";

		GameScene.game.AddExp(exp);
		GameScene.game.SaveGameData();

		btn_Reward.GetComponent<CanvasGroup>().alpha = 1f;
		btn_Reward.GetComponent<CanvasGroup>().blocksRaycasts = true;
		btn_Reward.gameObject.SetActive(UnityEngine.Random.Range(0f, 1f) < .5f);

		string amount = string.Empty;

		if (LocalData.gameData.energy > LocalData.gameData.energyTotal)
		{
			amount = $"<color=#FFC700>{LocalData.gameData.energy}</color>";
		}
		else amount = LocalData.gameData.energy.ToString("N0");

		txtmp_Energy.text = $"{amount}/{LocalData.gameData.energyTotal}";
	}
}
