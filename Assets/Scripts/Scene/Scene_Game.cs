using Cinemachine;
using MEC;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
	Playing,
	Paused
}

public class Scene_Game : SceneLogic
{
	public int coin;
	public int score;
	public int exp;
	public int monsterKilled;

	LevelLoadManager levelLoadManager;
	CinemachineVirtualCamera virtualCamera;
	PlayerActor player;

	public int retryCount = 0;
	public int randomCount = 0;

	public bool isInfinteMode = false;

	public GameState gameState = GameState.Playing;

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_AddScorePerFrame));
	}

	protected override void Awake()
	{
		base.Awake();

		LocalData.gameData = JsonManager<GameData>.LoadData(Define.JSON_GAMEDATA);

		if(LocalData.gameData == null)
		{
			LocalData.gameData = new GameData();
		}

		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);

		if (LocalData.masterData == null)
		{
			DebugManager.Log("WARNING!! NO MASTER DATA.", DebugColor.Data);

			GameManager.Scene.LoadScene(SceneName.Logo);

			PlayerPrefs.SetString("Version", string.Empty);

			return;
		}

		levelLoadManager = FindObjectOfType<LevelLoadManager>();
		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		player = FindObjectOfType<PlayerActor>();
	}

	public void SaveGameData()
	{
		if (score > LocalData.gameData.highScore)
		{
			LocalData.gameData.highScore = score;
		}

		LocalData.gameData.coin += coin;
		LocalData.gameData.exp += exp;

		DebugManager.Log($"Player Gained {coin} coin.");
		DebugManager.Log($"Player Gained {exp} exp.");

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	private void Start()
	{
		// LoadLevel();

		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StackLastPopup<Popup_Pause>();

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

		AddScorePerFrame();
	}


	private void AddScorePerFrame()
	{
		Util.RunCoroutine(Co_AddScorePerFrame(), nameof(Co_AddScorePerFrame));
	}

	IEnumerator<float> Co_AddScorePerFrame()
	{
		while (!player.isDead)
		{
			yield return Timing.WaitUntilTrue(() => gameState == GameState.Playing);
			score += 1 * scoreMultiplier;

			GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);

			yield return Timing.WaitForSeconds(.5f);
		}
	}

	public int scoreMultiplier = 1;



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

	public void SetCameraTarget(Transform target)
	{
		virtualCamera.Follow = target;
		virtualCamera.LookAt = target;
	}



	public void Refresh()
	{
		score = 0;
		coin = 0;

		GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);
		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(coin);
	}

	public void Retry()
	{
		SaveGameData();

		Refresh();

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

		AddScorePerFrame();
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

		levelLoadManager.LoadLevel(Define.JSON_LEVELDATA);
	}


	public void ShowTutorial()
	{
		GameManager.UI.FetchPanel<Panel_HUD>().HidePanel();

		GameManager.UI.StackPanel<Panel_Tutorial>(true);
	}
}