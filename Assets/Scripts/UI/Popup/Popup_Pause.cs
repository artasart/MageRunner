using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class Popup_Pause : Popup_Base
{
	Button btn_Home;
	Button btn_BGM;
	Button btn_SFX;
	Button btn_NoAds;
	Button btn_Language;

	bool isBgm = true;
	bool isSfx = true;

	Button btn_Confirm;

	private void OnDisable()
	{
		if (!isInitialized) { isInitialized = true; return; }

		Scene.game.gameState = GameState.Playing;

		Scene.game.playerActor.ToggleSimulation(true);

		GameManager.UI.FetchPanel<Panel_HUD>().Show();
	}

	private void OnEnable()
	{
		if (!isInitialized) { return; }

		Scene.game.gameState = GameState.Paused;

		Scene.game.playerActor.ToggleSimulation(false);

		GameManager.UI.FetchPanel<Panel_HUD>().Hide();		
	}

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		UseDimClose();

		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home, useAnimation: true);
		btn_BGM = GetUI_Button(nameof(btn_BGM), OnClick_BGM, useAnimation: true);
		btn_SFX = GetUI_Button(nameof(btn_SFX), OnClick_SFX, useAnimation: true);
		btn_NoAds = GetUI_Button(nameof(btn_NoAds), OnClick_NoAds, useAnimation: true);
		btn_Language = GetUI_Button(nameof(btn_Language), OnClick_Language, useAnimation: true);
		btn_Language.onClick.RemoveListener(OpenSound);

		btn_BGM.transform.Search("img_Icon_Block").gameObject.SetActive(false);
		btn_SFX.transform.Search("img_Icon_Block").gameObject.SetActive(false);

		btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnClick_Confirm);
	}

	private void OnClick_Home()
	{
		Scene.game.SaveGameData();

		GameManager.Scene.LoadScene(SceneName.Main);
	}

	public float bgmValue;
	public float sfxValue;

	private void OnClick_BGM()
	{
		isBgm = !isBgm;

		if(!isBgm)
		{
			bgmValue = GameManager.Sound.bgmVolume;

			GameManager.Sound.bgm.volume = 0f;
		}
		else
		{
			GameManager.Sound.bgm.volume = bgmValue;
		}

		btn_BGM.transform.Search("img_Icon_Block").gameObject.SetActive(!isBgm);
	}

	private void OnClick_SFX()
	{
		isSfx = !isSfx;

		if (!isSfx)
		{
			sfxValue = GameManager.Sound.sfxVolume;

			GameManager.Sound.soundEffect.volume = 0f;
		}
		else
		{
			GameManager.Sound.soundEffect.volume = sfxValue;
		}

		btn_SFX.transform.Search("img_Icon_Block").gameObject.SetActive(!isSfx);
	}

	private void OnClick_NoAds()
	{

	}

	private void OnClick_Language()
	{
		GameManager.Sound.PlaySound(Define.SOUND_DENIED);

		btn_Language.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);
	}
}
