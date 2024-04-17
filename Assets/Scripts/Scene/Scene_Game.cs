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

	public int retryCount = 0;
	public int randomCount = 0;

	public bool isInfinteMode = false;

	protected override void Awake()
	{
		base.Awake();

		levelLoadManager = FindObjectOfType<LevelLoadManager>();
		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		player = FindObjectOfType<PlayerActor>();
	}

	private void Start()
	{
		// LoadLevel();

		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameStart();
	}

	public void GameStart() => Util.RunCoroutine(Co_GameStart(), nameof(Co_GameStart));
	private IEnumerator<float> Co_GameStart()
	{
		virtualCamera.m_Lens.OrthographicSize = 5f;

		Util.Zoom(virtualCamera, 3f, .05f);

		yield return Timing.WaitUntilTrue(() => virtualCamera.m_Lens.OrthographicSize == 3f);

		FindObjectOfType<GroundController>().MoveGround();

		float value = 0f;

		while (value < 1f)
		{
			player.UpdateAnimator(value += Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		player.UpdateAnimator(1f);

		GameManager.UI.StartPanel<Panel_HUD>();
	}

	public void Restart() => Util.RunCoroutine(Co_Restart(), nameof(Co_Restart));
	
	IEnumerator<float> Co_Restart()
	{
		player.UpdateAnimator(1f);

		FindObjectOfType<GroundController>().MoveGround();

		virtualCamera.m_Lens.OrthographicSize = 5f;

		Util.Zoom(virtualCamera, 3f, .05f);

		GameManager.UI.StartPanel<Panel_HUD>();

		yield return Timing.WaitForOneFrame;
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

	public void Retry()
	{
		ShowAdWithRandomRetry();

		Util.RunCoroutine(Co_Retry(), nameof(Co_Retry));
	}

	private IEnumerator<float> Co_Retry()
	{
		GameManager.Scene.Fade(true);

		yield return Timing.WaitUntilTrue(GameManager.Scene.IsFaded);

		GameManager.UI.PopPopup(true);

		SetCameraTarget(player.transform);

		if (!isInfinteMode) LoadLevel();

		player.Refresh();

		foreach (var item in FindObjectsOfType<Ground>())
		{
			item.Refresh();
		}

		foreach(var item in FindObjectsOfType<MonsterActor>())
		{
			item.Refresh();
		}

		if (isInfinteMode) Restart();

		GameManager.Scene.Fade(false);

		yield return Timing.WaitUntilFalse(GameManager.Scene.IsFaded);

		player.isDead = false;
	}

	private void ShowAdWithRandomRetry()
	{
		if (retryCount == 0)
		{
			randomCount = UnityEngine.Random.Range(3, 5);
		}

		if (retryCount < randomCount)
		{
			retryCount++;
		}

		else
		{
			retryCount = 0;

			DebugManager.Log("Show Ad.", DebugColor.AD);
		}
	}

	public void LoadLevel()
	{
		levelLoadManager.DestroyLevel();

		levelLoadManager.LoadLevel("levelData.json");
	}


	public void ShowTutorial()
	{
		GameManager.UI.FetchPanel<Panel_HUD>().HidePanel();

		GameManager.UI.StackPanel<Panel_Tutorial>(true);
	}
}