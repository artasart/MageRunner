using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Main : Panel_Base
{
	Button btn_PlayGame;

	protected override void Awake()
	{
		base.Awake();

		btn_PlayGame = GetUI_Button(nameof(btn_PlayGame), OnClick_PlayGame);
	}

	public void OnClick_PlayGame()
	{
		GameManager.Scene.LoadScene(SceneName.Game);
	}
}
