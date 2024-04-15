using Cinemachine;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Game : SceneLogic
{
	public int coin;
	public int score;
	public int monsterKilled;

	LevelLoadManager levelLoadManager;
	CinemachineVirtualCamera virtualCamera;
	PlayerActor player;

	protected override void Awake()
	{
		base.Awake();

		levelLoadManager = FindObjectOfType<LevelLoadManager>();
		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		player = FindObjectOfType<PlayerActor>();
	}

	private void Start()
	{
		RefreshLevel();

		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_HUD>();
	}

	public void GainResource(int scoreAmount, int coinAmount)
	{
		monsterKilled++;

		score += scoreAmount;
		coin += coinAmount;

		GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);
		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(coin);
	}

	public void AddCoin(int amount)
	{
		coin += amount;

		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(coin);
	}

	public void SaveScore()
	{
		DebugManager.Log("Score is saved.");

		coin = 0;
	}

	public void SetScore()
	{
		GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);
	}

	public void SetCameraTarget(Transform target)
	{
		virtualCamera.Follow = target;
		virtualCamera.LookAt = target;
	}

	public void Retry() => Util.RunCoroutine(Co_Retry(), nameof(Co_Retry));

	private IEnumerator<float> Co_Retry()
	{
		GameManager.Scene.Fade(true);

		yield return Timing.WaitUntilTrue(GameManager.Scene.IsFaded);

		GameManager.UI.PopPopup(true);

		SetCameraTarget(player.transform);

		RefreshLevel();

		player.Refresh();

		GameManager.Scene.Fade(false);

		yield return Timing.WaitUntilFalse(GameManager.Scene.IsFaded);

		player.isDead = false;
	}

	public void RefreshLevel()
	{
		foreach (Transform child in levelLoadManager.transform)
		{
			Destroy(child.gameObject);
		}

		levelLoadManager.LoadLevel("levelData.json");
	}
}