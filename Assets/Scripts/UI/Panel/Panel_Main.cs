using DG.Tweening;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Main : Panel_Base
{
	Button btn_PlayGame;
	Button btn_Settings;
	Button btn_Inventory;
	Button btn_Shop;

	Image img_New;
	TMP_Text txtmp_Gold;

	TMP_Text txtmp_RunnerTag;
	TMP_Text txtmp_UserName;

	TMP_Text txtmp_Energy;
	TMP_Text txtmp_Message;

	Button btn_BuyGold;
	Button btn_BuyEnergy;

	Button btn_ChangeNickname;
	Button btn_Stat;

	public Transform group_TopMenu { get; private set; }

	Button btn_Energy;
	Button btn_Coin;


	protected override void Awake()
	{
		base.Awake();

		img_New = GetUI_Image(nameof(img_New));

		txtmp_Gold = GetUI_TMPText(nameof(txtmp_Gold), string.Empty);
		txtmp_Energy = GetUI_TMPText(nameof(txtmp_Energy), string.Empty);

		btn_PlayGame = GetUI_Button(nameof(btn_PlayGame), OnClick_PlayGame, useAnimation: true);
		btn_PlayGame.onClick.RemoveListener(OpenSound);

		btn_Settings = GetUI_Button(nameof(btn_Settings), OnClick_Settings, useAnimation: true);
		btn_Inventory = GetUI_Button(nameof(btn_Inventory), OnClick_Inventory, useAnimation: true);
		btn_Stat = GetUI_Button(nameof(btn_Stat), OnClick_Stat, useAnimation: true);

		btn_ChangeNickname = GetUI_Button(nameof(btn_ChangeNickname), OnClick_ChangeNickname, useAnimation: true);

#if UNITY_EDITOR
		btn_ChangeNickname.interactable = false;
#endif

		btn_Coin = GetUI_Button(nameof(btn_Coin), OnClick_BuyGold, useAnimation: true);
		btn_Coin.onClick.RemoveListener(OpenSound);
		btn_Energy = GetUI_Button(nameof(btn_Energy), OnClick_BuyEnergy, useAnimation: true);
		btn_Energy.onClick.RemoveListener(OpenSound);

		btn_BuyGold = GetUI_Button(nameof(btn_BuyGold), OnClick_BuyGold, useAnimation: true);
		btn_BuyGold.onClick.RemoveListener(OpenSound);
		btn_BuyEnergy = GetUI_Button(nameof(btn_BuyEnergy), OnClick_BuyEnergy, useAnimation: true);
		btn_BuyEnergy.onClick.RemoveListener(OpenSound);

		btn_Shop = GetUI_Button(nameof(btn_Shop), OnClick_Shop, useAnimation: true);

		txtmp_RunnerTag = GetUI_TMPText(nameof(txtmp_RunnerTag), string.Empty);
		txtmp_UserName = GetUI_TMPText(nameof(txtmp_UserName), string.Empty);


		group_TopMenu = this.transform.Search(nameof(group_TopMenu));

		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "mind controlling...");
		txtmp_Message.UsePingPong();
	}

	private void Start()
	{
		SetEnergy();

		txtmp_Gold.text = LocalData.gameData.gold.ToString("N0");
		txtmp_Message.StartPingPong(1f);
	}

	private void OnClick_ChangeNickname()
	{
		GameManager.UI.StackPopup<Popup_InputField>();
	}

	private void OnClick_BuyGold()
	{
		if (LocalData.gameData.isAdWatched)
		{
			GameManager.Sound.PlaySound(Define.SOUND_DENIED);

			btn_Coin.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			return;
		}

		GameManager.Sound.PlaySound(Define.SOUND_OPEN);

		GameManager.UI.StackPopup<Popup_Basic>(true);

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"Do you want to get <color=#FFC700>Gold Box</color> after wathching AD?", "Reward",
		() =>
		{
			GameManager.Scene.Dim(true);

			Util.RunCoroutine(GoldAd().Delay(.75f), nameof(GoldAd), CoroutineTag.Content);
			// GameManager.AdMob.ShowRewardedAd(() => Util.RunCoroutine(GoldAd().Delay(.75f), nameof(GoldAd), CoroutineTag.Content));
		},

		() =>
		{

		});
	}

	private void OnClick_BuyEnergy()
	{
		if (LocalData.gameData.isAdWatched)
		{
			GameManager.Sound.PlaySound(Define.SOUND_DENIED);

			btn_Energy.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			return;
		}

		GameManager.Sound.PlaySound(Define.SOUND_OPEN);

		GameManager.UI.StackPopup<Popup_Basic>(true);

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"Do you want to get <color=#FFC700>{5} Energy</color> after wathching AD?", "Reward",
		() =>
		{
			GameManager.Scene.Dim(true);

			Util.RunCoroutine(EnergyAd().Delay(.75f), nameof(EnergyAd), CoroutineTag.Content);
			// GameManager.AdMob.ShowRewardedAd(() => Util.RunCoroutine(EnergyAd().Delay(.75f), nameof(EnergyAd), CoroutineTag.Content));
		},
		() =>
		{

		});
	}

	public IEnumerator<float> GoldAd()
	{
		yield return Timing.WaitForOneFrame;

		GameManager.UI.StackSplash<Splash_Gold>();
		GameManager.UI.FetchSplash<Splash_Gold>().OpenBox();
	}

	private IEnumerator<float> EnergyAd()
	{
		yield return Timing.WaitForOneFrame;

		LocalData.gameData.energy += 5;

		SetEnergy();

		GameManager.Scene.callback_ShowToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(false);
		GameManager.Scene.callback_CloseToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
		GameManager.Scene.callback_ClickToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
		GameManager.Scene.ShowToastAndDisappear($"You gained {5} energy..!");

		GameManager.UI.PopPopup();

		GameManager.Scene.Dim(false);

		BlockUI();

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	public void SetUserInfo(string username, string runnerTag)
	{
		txtmp_UserName.text = username;
		txtmp_RunnerTag.text = $"<color=#FFC700>RUNNER #{runnerTag}</color>";
	}

	public void SetUserNickname(string nickname)
	{
		txtmp_UserName.text = nickname;

		LocalData.gameData.nickname = nickname;

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	private void OnClick_Shop()
	{
		GameManager.UI.StackPanel<Panel_Shop>(true);

		GameManager.UI.FetchPanel<Panel_Shop>().Init();
	}

	private void OnClick_PlayGame()
	{
		if (LocalData.gameData.energy <= 0)
		{
			btn_Energy.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);
			btn_PlayGame.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			GameManager.Sound.PlaySound(Define.SOUND_DENIED);

			return;
		}

		GameManager.Sound.PlaySound(Define.SOUND_OPEN);

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

	private void OnClick_Stat()
	{
		GameManager.UI.SwitchPanel<Panel_Stat>(true);
		GameManager.UI.FetchPanel<Panel_Stat>().Init();
	}

	private void OnClick_Mail()
	{
		GameManager.UI.StackPopup<Popup_Mail>();
	}

	private void OnClick_Rank()
	{
		GameManager.UI.StackPopup<Popup_Rank>();
	}

	private void OnClick_DayCheck()
	{
		GameManager.UI.StackPopup<Popup_DailyCheck>();
	}

	public void ShowTopMenu(bool isShow)
	{
		if (isShow)
		{
			GameManager.UI.FadeCanvasGroup(group_TopMenu.GetComponent<CanvasGroup>(), 1f, _end: () => group_TopMenu.GetComponent<CanvasGroup>().blocksRaycasts = true);
		}

		else GameManager.UI.FadeCanvasGroup(group_TopMenu.GetComponent<CanvasGroup>(), .25f, _start: () => group_TopMenu.GetComponent<CanvasGroup>().blocksRaycasts = false);
	}

	public void ShowNewIcon(bool enable)
	{
		img_New.gameObject.SetActive(enable);
	}

	public void SetGoldUI(int gold)
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

	public void AddEnergy(int amount)
	{
		LocalData.gameData.energy += amount;

		SetEnergy();
	}

	public void BlockUI()
	{
		btn_Energy.transform.Search("img_BlockAds").GetComponent<BlockAds>().WatchAd();
		btn_Coin.transform.Search("img_BlockAds").GetComponent<BlockAds>().WatchAd();
	}
}