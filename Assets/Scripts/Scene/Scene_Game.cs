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
		PoolManager.SetPoolData(Define.MONSTERACTOR, 20, Define.PATH_ACTOR);
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StackLastPopup<Popup_Pause>();

		playerModel = player.transform.GetChild(0).gameObject;
		playerModel.SetActive(false);

		GameStart();
	}

	public void SaveGameData()
	{
		if (score > LocalData.gameData.highScore)
		{
			LocalData.gameData.highScore = score;
		}

		LocalData.gameData.coin += coin;
		LocalData.gameData.gainedItems = gainedItems;

		DebugManager.Log($"Player Gained {coin} coin.");

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	public void GameStart() => Util.RunCoroutine(Co_GameStart(), nameof(Co_GameStart));

	private IEnumerator<float> Co_GameStart()
	{
		virtualCamera.m_Lens.OrthographicSize = 5f;

		Util.Zoom(virtualCamera, 3f, .05f);

		yield return Timing.WaitUntilTrue(() => virtualCamera.m_Lens.OrthographicSize == 3f);

		GameManager.Sound.PlaySound("Spawn", .25f);
		PoolManager.Spawn("Thunder", new Vector3(0f, -0.2f, 0f), Quaternion.identity);

		yield return Timing.WaitForSeconds(.1f);

		playerModel.SetActive(true);

		FindObjectOfType<LevelController>().MoveGround();
		FindObjectOfType<ParallexScrolling>().StartScroll();
		float value = 0f;
		bool isWalk = false;

		while (value < 1f)
		{
			player.UpdateAnimator(value += Time.deltaTime);

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
		yield return Timing.WaitForSeconds(5f);

		var groundController = FindObjectOfType<LevelController>();

		var currentSpeed = groundController.GetMoveSpeed();
		var addSpeed = currentSpeed * .1f;

		var probability = groundController.GetProbability();
		var addProbability = probability * .1f;

		while (true)
		{
			yield return Timing.WaitForSeconds(5f);

			currentSpeed = Mathf.Clamp(currentSpeed += addSpeed, 0f, 10f);
			probability += addProbability;

			groundController.MoveGround(currentSpeed);
			groundController.AddProbability(addProbability);
			groundController.monsterProbability += (groundController.monsterProbability * .1f);

			//DebugManager.Log("현재 속도 : " + currentSpeed);
			//DebugManager.Log("현재 확률 : " + probability);
		}
	}



	private void ScoreCount()
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

		FindObjectOfType<LevelController>().MoveGround();
		FindObjectOfType<ParallexScrolling>().StartScroll();

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

	public void StopDifficult()
	{
		Util.KillCoroutine(nameof(Co_AddDifficulty));
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
}