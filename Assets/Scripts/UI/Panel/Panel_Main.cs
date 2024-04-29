using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Main : Panel_Base
{
	Button btn_PlayGame;
	Button btn_Settings;
	Button btn_Inventory;
	Button btn_Mail;
	Button btn_Rank;
	Button btn_DayCheck;

	Button btn_Shop;

	Image img_New;
	TMP_Text txtmp_Gold;

	TMP_Text txtmp_RunnerTag;
	TMP_Text txtmp_UserName;

	TMP_Text txtmp_Energy;
	TMP_Text txtmp_Message;


	Button btn_BuyGold;
	Button btn_BuyEnergy;

	Transform group_TopMenu;
	Transform btn_Energy;

	protected override void Awake()
	{
		base.Awake();

		img_New = GetUI_Image(nameof(img_New));

		txtmp_Gold = GetUI_TMPText(nameof(txtmp_Gold), string.Empty);
		txtmp_Energy = GetUI_TMPText(nameof(txtmp_Energy), string.Empty);

		btn_PlayGame = GetUI_Button(nameof(btn_PlayGame), OnClick_PlayGame, useAnimation: true);
		btn_Settings = GetUI_Button(nameof(btn_Settings), OnClick_Settings, useAnimation: true);
		btn_Inventory = GetUI_Button(nameof(btn_Inventory), OnClick_Inventory, useAnimation: true);
		btn_Mail = GetUI_Button(nameof(btn_Mail), OnClick_Mail, useAnimation: true);
		btn_Rank = GetUI_Button(nameof(btn_Rank), OnClick_Rank, useAnimation: true);
		btn_DayCheck = GetUI_Button(nameof(btn_DayCheck), OnClick_DayCheck, useAnimation: true);

		btn_BuyGold = GetUI_Button(nameof(btn_BuyGold), OnClick_BuyGold, useAnimation: true);
		btn_BuyEnergy = GetUI_Button(nameof(btn_BuyEnergy), OnClick_BuyEnergy, useAnimation: true);

		btn_Shop = GetUI_Button(nameof(btn_Shop), OnClick_Shop, useAnimation: true);

		txtmp_RunnerTag = GetUI_TMPText(nameof(txtmp_RunnerTag), string.Empty);
		txtmp_UserName = GetUI_TMPText(nameof(txtmp_UserName), string.Empty);

		btn_Energy = this.transform.Search(nameof(btn_Energy));

		group_TopMenu = this.transform.Search(nameof(group_TopMenu));

		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "mind controlling...");
		txtmp_Message.UsePingPong();
	}

	private void Start()
	{
		SetEnergy();

		txtmp_Message.StartPingPong(.25f);
	}

	private void OnClick_BuyGold()
	{
		// stack Popup
		// 광고 시청할래요?

		GameManager.UI.StackPopup<Popup_Basic>(true);

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"Do you want to get <color=#FFC700>{10000} gold</color> after wathching AD?", "Reward",
		() =>
		{
			Debug.Log("Watch AD!");

			Invoke(nameof(GoldAd), 1f);
		},
		() =>
		{
			Debug.Log("Canceled..");
		});
	}

	private void OnClick_BuyEnergy()
	{
		GameManager.UI.StackPopup<Popup_Basic>(true);

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"Do you want to get <color=#FFC700>{5} Energy</color> after wathching AD?", "Reward",
		() =>
		{
			Debug.Log("Watch AD!");

			Invoke(nameof(EnergyAd), 1f);
		},
		() =>
		{
			Debug.Log("Canceled..");
		});
	}

	public void GoldAd()
	{
		Debug.Log("Watched..!");

		LocalData.gameData.gold += 10000;

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);

		SetGold(LocalData.gameData.gold);

		GameManager.UI.PopPopup();
	}

	public void EnergyAd()
	{
		Debug.Log("Watched..!");

		LocalData.gameData.energy += 5;

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);

		SetEnergy();

		GameManager.UI.PopPopup();
	}

	public void SetUserInfo(string username, int runnerTag)
	{
		txtmp_UserName.text = username;
		txtmp_RunnerTag.text = $"<color=#FFC700>RUNNER #{runnerTag}</color>";
	}

	private void OnClick_Shop()
	{
		GameManager.UI.StackPanel<Panel_Shop>(true);

		GameManager.UI.FetchPanel<Panel_Shop>().Init();

		GameManager.UI.FetchPanel<Panel_Main>().Show();
	}

	private void OnClick_PlayGame()
	{
		if (LocalData.gameData.energy <= 0)
		{
			btn_Energy.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);
			btn_PlayGame.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			return;
		}

		btn_PlayGame.interactable = false;

		LocalData.gameData.energy -= 1;

		Scene.main.particle_RingShield.DOScale(Vector3.one * .75f, .5f);

		Util.Zoom(Scene.main.virtualCamera, .1f, .025f);

		GameManager.Scene.LoadScene(SceneName.Game, fadeSpeed: .25f);
	}

	private void OnClick_Settings()
	{
		GameManager.UI.StackPopup<Popup_Settings>(true);
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

	public void ShowTopMenu(bool isShow)
	{
		if (isShow)
		{
			GameManager.UI.FadeCanvasGroup(group_TopMenu.GetComponent<CanvasGroup>(), 1f);
		}

		else GameManager.UI.FadeCanvasGroup(group_TopMenu.GetComponent<CanvasGroup>(), .25f, _start: () => group_TopMenu.GetComponent<CanvasGroup>().blocksRaycasts = false);
	}

	public void ShowNewIcon(bool enable)
	{
		img_New.gameObject.SetActive(enable);
	}

	public void SetGold(int gold)
	{
		txtmp_Gold.text = Util.FormatNumber(gold);
	}

	public void SetEnergy()
	{
		string amount = string.Empty;

		if (LocalData.gameData.energy > LocalData.gameData.energyTotal)
		{
			amount = $"<color=#FFC700>{LocalData.gameData.energy}</color>";
		}

		else amount = LocalData.gameData.energy.ToString();

		txtmp_Energy.text = $"{amount}/{LocalData.gameData.energyTotal}";
	}
}
