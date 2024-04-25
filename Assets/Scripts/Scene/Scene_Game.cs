using Cinemachine;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState
{
	Playing,
	Paused
}

public class Scene_Game : SceneLogic
{
	public int gold;
	public int score;
	public int exp;
	public int monsterKilled;

	LevelLoadManager levelLoadManager;
	CinemachineVirtualCamera virtualCamera;
	public PlayerActor player { get; set; }
	GameObject playerModel;

	public int retryCount = 0;
	public int randomCount = 0;

	public bool isInfinteMode = false;

	public GameState gameState = GameState.Playing;

	public SerializableDictionary<string, int> gainedItems = new SerializableDictionary<string, int>();

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_AddScorePerFrame));
	}

	protected override void Awake()
	{
		base.Awake();

		LocalData.gameData = JsonManager<GameData>.LoadData(Define.JSON_GAMEDATA);

		if (LocalData.gameData == null)
		{
			LocalData.gameData = new GameData();
		}

		else
		{
			LocalData.gameData.gainedItems.Clear();
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

		PoolManager.InitPool();
		PoolManager.SetPoolData("Thunder", 10, Define.PATH_VFX);
		PoolManager.SetPoolData(Define.MONSTER_ACTOR, 20, Define.PATH_ACTOR);
		PoolManager.SetPoolData("Thunder_ExplosionSmall", 5, Define.PATH_VFX);
		PoolManager.SetPoolData("CoinSpawner", 5, Define.PATH_CONTENTS);
		PoolManager.SetPoolData("Coin", 40, Define.PATH_CONTENTS);
		PoolManager.SetPoolData("Thunder_Explosion", 1, Define.PATH_VFX);
	}

	private void Start()
	{
		GameManager.Sound.PlayBGM("Dawn", .5f);
		GameManager.Sound.bgmVolume = 1f;
		GameManager.Sound.sfxVolume = .75f;

		GameManager.Scene.Fade(false, .1f);
		GameManager.UI.Restart();
		GameManager.UI.StackLastPopup<Popup_Pause>();

		playerModel = player.transform.GetChild(0).gameObject;
		playerModel.SetActive(false);

		GameStart();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			GameManager.UI.FetchPanel<Panel_HUD>().Hide();

			FindObjectOfType<Scene_Game>().gameState = GameState.Paused;

			GameManager.UI.StartPopup<Popup_Skill>(true);
		}
	}

	public void SaveGameData()
	{
		if (score > LocalData.gameData.highScore)
		{
			LocalData.gameData.highScore = score;
		}

		LocalData.gameData.gold += gold;
		LocalData.gameData.gainedItems = gainedItems;

		DebugManager.Log($"Player Gained {gold} coin.");

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	public void GameStart() => Util.RunCoroutine(Co_GameStart(), nameof(Co_GameStart));

	private IEnumerator<float> Co_GameStart()
	{
		virtualCamera.m_Lens.OrthographicSize = 5f;

		Util.Zoom(virtualCamera, 3f, .05f);

		FindObjectOfType<LevelController>().MoveGround();
		var scroll = FindObjectsOfType<ParallexScrolling>();
		foreach (var item in scroll)
		{
			item.StartScroll();
		}

		yield return Timing.WaitUntilTrue(() => virtualCamera.m_Lens.OrthographicSize == 3f);

		FindObjectOfType<CameraShake3D>().Shake();
		GameManager.Sound.PlaySound("Spawn", .5f);
		PoolManager.Spawn("Thunder", new Vector3(0f, -0.2f, 0f), Quaternion.identity);

		yield return Timing.WaitForSeconds(.1f);

		playerModel.SetActive(true);

		float value = 0f;
		bool isWalk = false;

		while (value < 1f)
		{
			player.UpdateAnimator(value += 1.5f * Time.deltaTime);

			if (!isWalk && value > .25f)
			{
				player.GetComponent<FootStepController>().StartWalk();
				isWalk = true;
			}

			yield return Timing.WaitForOneFrame;
		}

		player.UpdateAnimator(1f);

		GameManager.UI.StartPanel<Panel_HUD>();

		ScoreCount();

		AddDifficulty();
	}

	public void AddDifficulty()
	{
		Util.RunCoroutine(Co_AddDifficulty(), nameof(Co_AddDifficulty), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_AddDifficulty()
	{
		var levelController = FindObjectOfType<LevelController>();

		int index = 0;
		while (true)
		{
			yield return Timing.WaitForSeconds(10f);

			levelController.AddSpeed();

			index++;

			if(index % 3 == 0)
			{
				levelController.groundProbability += .1f;
				levelController.monsterProbability += .1f;
			}

			Debug.Log("현재 속도 : " + levelController.moveSpeed);
		}
	}



	private void ScoreCount()
	{
		Util.RunCoroutine(Co_AddScorePerFrame(), nameof(Co_AddScorePerFrame));
	}

	IEnumerator<float> Co_AddScorePerFrame()
	{
		var levelController = FindObjectOfType<LevelController>();

		while (!player.isDead)
		{
			yield return Timing.WaitUntilTrue(() => gameState == GameState.Playing);

			score += (int)levelController.moveSpeed * levelController.currentMoveMultiplier;

			GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);

			yield return Timing.WaitForSeconds(.5f);
		}
	}

	public int scoreMultiplier = 1;
	public int moveMultiplier = 3;



	public void Restart() => Util.RunCoroutine(Co_Restart(), nameof(Co_Restart));

	IEnumerator<float> Co_Restart()
	{
		player.UpdateAnimator(1f);

		FindObjectOfType<LevelController>().MoveGround();
		var scroll = FindObjectsOfType<ParallexScrolling>();
		foreach (var item in scroll)
		{
			item.StartScroll();
		}

		virtualCamera.m_Lens.OrthographicSize = 5f;

		Util.Zoom(virtualCamera, 3f, .05f);

		GameManager.UI.StartPanel<Panel_HUD>();

		player.GetComponent<FootStepController>().StartWalk();

		AddDifficulty();

		yield return Timing.WaitForOneFrame;
	}


	public void ZoomCamera(float zoomValue)
	{
		Util.Zoom(virtualCamera, zoomValue, .05f);
	}



	public void GainResource(int scoreAmount, int coinAmount)
	{
		monsterKilled++;

		score += scoreAmount;
		gold += coinAmount;

		GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);
		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(gold);
	}

	public void AddGold(int amount)
	{
		gold += amount;

		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(gold);
	}

	public void SaveScore()
	{
		DebugManager.Log("Score is saved.");

		gold = 0;
	}

	public void SetCameraTarget(Transform target)
	{
		virtualCamera.Follow = target;
		virtualCamera.LookAt = target;
	}

	public void StopDifficult()
	{
		Util.KillCoroutine(nameof(Co_AddDifficulty));
	}

	public void Refresh()
	{
		score = 0;
		gold = 0;

		GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);
		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(gold);
		GameManager.UI.FetchPanel<Panel_HUD>().RefreshUI();
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

		FindObjectOfType<LevelController>().Refresh();

		foreach (var item in FindObjectsOfType<MonsterActor>())
		{
			item.Refresh();
		}

		if (isInfinteMode) Restart();

		GameManager.Scene.Fade(false);

		yield return Timing.WaitUntilFalse(GameManager.Scene.IsFaded);

		player.isDead = false;

		ScoreCount();
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
		GameManager.UI.FetchPanel<Panel_HUD>().Hide();

		GameManager.UI.StackPanel<Panel_Tutorial>(true);
	}




	public void AddExp(int exp)
	{
		if (LocalData.gameData.level == LocalData.masterData.levelData[LocalData.masterData.levelData.Count - 1].level)
		{
			Debug.Log("Max Level : " + LocalData.gameData.level);

			return;
		}

		var totalExp = LocalData.masterData.levelData[LocalData.gameData.level].exp;

		if (LocalData.gameData.exp + exp >= totalExp)
		{
			exp = (LocalData.gameData.exp + exp) - totalExp;

			LocalData.gameData.level++;
			LocalData.gameData.exp = 0;

			AddExp(exp);
		}

		else
		{
			Debug.Log("Player Level Up !!! : " + LocalData.gameData.level);

			LocalData.gameData.exp += exp;
		}
	}

	public void SetVirtualCamBody(Vector3 bodyPosition)
	{
		Util.RunCoroutine(Co_SetVirtualCamBody(bodyPosition), nameof(Co_SetVirtualCamBody), CoroutineTag.Content);
	}

	IEnumerator<float> Co_SetVirtualCamBody(Vector3 bodyPosition)
	{
		var lerpvalue = 0f;
		var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

		while (Vector3.Distance(transposer.m_FollowOffset, bodyPosition) > 0.01f)
		{
			transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, bodyPosition, 1.5f * lerpvalue * Time.deltaTime);

			lerpvalue += Time.deltaTime;

			yield return Timing.WaitForOneFrame;
		}

		transposer.m_FollowOffset = bodyPosition;
	}
}