using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Logo : SceneLogic
{
	bool isFirst = true;

	protected override void Awake()
	{
		base.Awake();

		isFirst = PlayerPrefs.GetInt(nameof(isFirst)) == 1 ? true : false;
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_Logo>(true);

		Util.RunCoroutine(Co_LogoStart(), nameof(Co_LogoStart));
	}

	private IEnumerator<float> Co_LogoStart()
	{
		yield return Timing.WaitForSeconds(.75f);

		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);

		if (LocalData.masterData == null)
		{
			GameManager.Data.GetSheet(EnterGame);
		}

		else
		{
			GameManager.Data.GetVersion(EnterGame);
		}
	}

	private void EnterGame()
	{
		if (isFirst)
		{
			GameManager.Scene.LoadScene(SceneName.Game);

			PlayerPrefs.SetInt(nameof(isFirst), Convert.ToInt32(true));
		}

		else GameManager.Scene.LoadScene(SceneName.Main);
	}
}