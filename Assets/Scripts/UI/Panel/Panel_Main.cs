using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Main : Panel_Base
{
	Button btn_PlayGame;
	Button btn_RewardAd;
	Button btn_CloseReward;
	Button btn_Settings;
	Button btn_Inventory;
	Button btn_Mail;
	Button btn_Rank;
	Button btn_DayCheck;

	GameObject rewardAd;

	Image img_New;

	TMP_Text txtmp_Gold;

	protected override void Awake()
	{
		base.Awake();

		img_New = GetUI_Image(nameof(img_New));

		txtmp_Gold = GetUI_TMPText(nameof(txtmp_Gold), string.Empty);

		btn_PlayGame = GetUI_Button(nameof(btn_PlayGame), OnClick_PlayGame, useAnimation: true);
		btn_RewardAd = GetUI_Button(nameof(btn_RewardAd), OnClick_RewardedAd, useAnimation: true);
		btn_CloseReward = GetUI_Button(nameof(btn_CloseReward), OnClick_CloseRewardAd, useAnimation: true);
		btn_Settings = GetUI_Button(nameof(btn_Settings), OnClick_Settings, useAnimation: true);
		btn_Inventory = GetUI_Button(nameof(btn_Inventory), OnClick_Inventory, useAnimation: true);
		btn_Mail = GetUI_Button(nameof(btn_Mail), OnClick_Mail, useAnimation: true);
		btn_Rank = GetUI_Button(nameof(btn_Rank), OnClick_Rank, useAnimation: true);
		btn_DayCheck = GetUI_Button(nameof(btn_DayCheck), OnClick_DayCheck, useAnimation: true);

		btn_CloseReward.onClick.RemoveListener(OpenSound);
		btn_CloseReward.onClick.AddListener(CloseSound);
	}

	private void Start()
	{
		rewardAd = btn_RewardAd.gameObject;
		HideRewardAd(true);
	}

	private void OnClick_CloseRewardAd()
	{
		HideRewardAd();
	}

	private void OnClick_RewardedAd()
	{
		Debug.Log("OnClick_RewardedAd");
	}

	private void OnClick_PlayGame()
	{
		Util.Zoom(Scene.main.virtualCamera, .1f, .025f);

		GameManager.Scene.LoadScene(SceneName.Game, fadeSpeed: .25f);
	}

	private void OnClick_Settings()
	{
		Debug.Log("OnClick_Settings");

		GameManager.UI.StackPopup<Popup_Settings>();
	}

	private void OnClick_Inventory()
	{
		GameManager.UI.SwitchPanel<Panel_Inventory>(true);
		GameManager.UI.FetchPanel<Panel_Inventory>().ShowNewIcon(img_New.gameObject.activeSelf);

		ShowNewIcon(false);
	}

	private void OnClick_Mail()
	{
		Debug.Log("OnClick_Mail");

		GameManager.UI.StackPopup<Popup_Mail>();
	}

	private void OnClick_Rank()
	{
		Debug.Log("OnClick_Rank");
		GameManager.UI.StackPopup<Popup_Rank>();
	}

	private void OnClick_DayCheck()
	{
		Debug.Log("OnClick_DayCheck");
		GameManager.UI.StackPopup<Popup_DailyCheck>();
	}


	public void ShowRewardAd()
	{
		GameManager.UI.Move(rewardAd, new Vector3(0f, -110f, 0f));
	}

	public void HideRewardAd(bool isInstant = false)
	{
		if (isInstant)
		{
			rewardAd.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 80f, 0f);

			return;
		}

		GameManager.UI.Move(rewardAd, new Vector3(0f, 80f, 0f));
	}

	public void ShowNewIcon(bool enable)
	{
		img_New.gameObject.SetActive(enable);
	}

	public void SetGold(int gold)
	{
		txtmp_Gold.text = Util.FormatNumber(gold);
	}
}
