using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Pause : Popup_Base
{
	Button btn_Home;
	Button btn_BGM;
	Button btn_SFX;
	Button btn_NoAds;
	Button btn_Language;

	Scene_Game game;

	private void OnDisable()
	{
		if (!isInitialized) { isInitialized = true; return; }

		game.gameState = GameState.Playing;

		game.player.ToggleSimulation(true);

		GameManager.UI.FetchPanel<Panel_HUD>().ShowPanel();
	}

	private void OnEnable()
	{
		if (!isInitialized) { return; }

		game.gameState = GameState.Paused;

		game.player.ToggleSimulation(false);

		GameManager.UI.FetchPanel<Panel_HUD>().HidePanel();		
	}

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		UseDimClose();

		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home);
		btn_BGM = GetUI_Button(nameof(btn_BGM), OnClick_BGM);
		btn_SFX = GetUI_Button(nameof(btn_SFX), OnClick_SFX);
		btn_NoAds = GetUI_Button(nameof(btn_NoAds), OnClick_NoAds);
		btn_Language = GetUI_Button(nameof(btn_Language), OnClick_Language);

		game = FindObjectOfType<Scene_Game>();
	}

	private void OnClick_Home()
	{
		GameManager.Scene.LoadScene(SceneName.Main);
	}

	private void OnClick_BGM()
	{

	}

	private void OnClick_SFX()
	{

	}

	private void OnClick_NoAds()
	{

	}

	private void OnClick_Language()
	{

	}
}
