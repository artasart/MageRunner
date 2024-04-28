using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MEC;
using static Enums;
using System.Linq;

public class Scene_Game : SceneLogic
{
	#region Members

	[Header("Game State")]
	public GameState gameState = GameState.Playing;

	[Header("Game Score")]
	public int gold;
	public int score;
	public int level = 1;
	public int exp;

	public int playerExp;

	public int scoreMultiplier = 1;
	public int moveMultiplier = 3;

	[Header("Game Data")]
	public SerializableDictionary<string, int> bags = new SerializableDictionary<string, int>();
	public SerializableDictionary<Skills, ActiveSkill> skills = new SerializableDictionary<Skills, ActiveSkill>();

	private CinemachineVirtualCamera virtualCamera { get; set; }
	public CameraShake3D cameraShake { get; private set; }
	public PlayerActor playerActor { get; set; }
	public LevelController levelController { get; private set; }
	public ParallexScrollController[] parallexScrollController { get; private set; }
	public FootStepController footStepController { get; private set; }

	GameObject playerModel;

	int replayCount = 0;
	int replayRandomCount = 0;

	#endregion



	#region Initialize

	private void OnDestroy()
	{
		SaveGameData();
	}

	private void OnDisable()
	{
		for (int i = 0; i < LocalData.gameData.activeSkills.Count; i++)
		{
			LocalData.gameData.activeSkills[i].level = 0;
		}

		StopScoreCount();
		StopDifficulty();
	}

	protected override void Awake()
	{
		base.Awake();

		Scene.game = this;

		LoadGameData();
		MakePool();

		footStepController = FindObjectOfType<FootStepController>();
		levelController = FindObjectOfType<LevelController>();
		parallexScrollController = FindObjectsOfType<ParallexScrollController>();
		cameraShake = FindObjectOfType<CameraShake3D>();

		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		virtualCamera.m_Lens.OrthographicSize = 5f;

		playerActor = FindObjectOfType<PlayerActor>();
		playerModel = playerActor.transform.GetChild(0).gameObject;
		playerModel.SetActive(false);
	}

	private void Start()
	{
		GameManager.Sound.PlayBGM(Define.SOUND_DAWN, .5f);

		GameManager.Scene.Fade(false, .1f);
		GameManager.UI.Restart();
		GameManager.UI.StackLastPopup<Popup_Pause>();

		GameStart();
	}

	#endregion



	#region Core Methods

	public void GameStart() => Util.RunCoroutine(Co_GameStart(), nameof(Co_GameStart), CoroutineTag.Content);

	private IEnumerator<float> Co_GameStart()
	{
		DebugManager.Log("GameStart", DebugColor.Game);

		Util.Zoom(virtualCamera, 3f, .025f);

		MoveEnviroment();
		
		yield return Timing.WaitUntilTrue(() => virtualCamera.m_Lens.OrthographicSize == 3f);

		GameManager.Sound.PlaySound(Define.SOUND_THUNDER, .5f);
		PoolManager.Spawn(Define.VFX_THUNDER, new Vector3(0f, -0.2f, 0f), Quaternion.identity);
		cameraShake.Shake();

		yield return Timing.WaitForSeconds(.1f);

		playerModel.SetActive(true);

		footStepController.StartWalk();
		playerActor.UpdateAnimator(1f);

		StartScoreCount();
		// StartDifficulty();

		GameManager.UI.StartPanel<Panel_HUD>();
	}

	public void Replay()
	{
		DebugManager.Log("Retry", DebugColor.Game);

		ShowInterstitialAd();

		LocalData.InitSkill();

		score = gold = 0;
		level = 1;
		goldMultiplier = 1;
		expMultiplier = 1;
		coolTimePercentage = 0;
		totalMana = 100;
		skills = new SerializableDictionary<Skills, ActiveSkill>();

		Util.KillCoroutine(nameof(Co_UseThunder));

		Util.RunCoroutine(Co_Replay(), nameof(Co_Replay), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_Replay()
	{
		GameManager.Scene.Fade(true);

		yield return Timing.WaitUntilTrue(GameManager.Scene.IsFaded);

		GameManager.UI.PopPopup(true);

		SetCameraTarget(playerActor.transform);

		playerActor.Refresh();
		levelController.Refresh();

		virtualCamera.m_Lens.OrthographicSize = 5f;
		Util.Zoom(virtualCamera, 3f, .025f);

		MoveEnviroment();

		footStepController.StartWalk();
		playerActor.UpdateAnimator(1f);

		GameManager.Scene.Fade(false);
		GameManager.UI.StartPanel<Panel_HUD>();

		yield return Timing.WaitUntilFalse(GameManager.Scene.IsFaded);

		StartDifficulty();
		StartScoreCount();

		playerActor.isDead = false;

		GameManager.UI.FetchPanel<Panel_HUD>().Refresh();
	}

	public void GameOver()
	{
		DebugManager.Log("Game Over", DebugColor.Game);

		GameManager.UI.FetchPopup<Popup_GameOver>().SetResult(Scene.game.score, Scene.game.gold, Scene.game.playerExp = Mathf.RoundToInt(Scene.game.score * .45f));

		GameManager.UI.StartPopup<Popup_GameOver>();
	}


	private void MakePool()
	{
		PoolManager.InitPool();

		PoolManager.SetPoolData(Define.MONSTER_ACTOR, 20, Define.PATH_ACTOR);
		PoolManager.SetPoolData(Define.VFX_THUNDER, 10, Define.PATH_VFX);
		PoolManager.SetPoolData(Define.VFX_SMALL_THUNDER_EXPLOSION, 5, Define.PATH_VFX);
		PoolManager.SetPoolData(Define.VFX_BIG_THUNDER_EXPLOSION, 1, Define.PATH_VFX);
		PoolManager.SetPoolData(Define.VFX_SKULL, 5, Define.PATH_VFX);
		PoolManager.SetPoolData(Define.VFX_UI_ELECTRIC_MESH, 1, Define.PATH_VFX);
		PoolManager.SetPoolData("Firework", 5, Define.PATH_VFX);

		PoolManager.SetPoolData(Define.ITEM_COIN_SPAWNER, 5, Define.PATH_CONTENTS);
		PoolManager.SetPoolData(Define.COIN, 40, Define.PATH_ITEM);
		PoolManager.SetPoolData(Define.ITEM_SKILL_CARD, 5, Define.PATH_ITEM);
	}

	public void LoadGameData()
	{
		LocalData.LoadMasterData();
		LocalData.LoadGameData();
	}

	public void SaveGameData() => LocalData.SaveGameData(score, gold, bags);



	public void MoveEnviroment()
	{
		levelController.MoveGround();

		foreach (var item in parallexScrollController) item.Scroll();
	}

	public void StopEnviroment()
	{
		footStepController.StopWalk();
		levelController.StopGround();

		foreach (var item in parallexScrollController) item.Stop();
	}

	private void StartDifficulty() => Util.RunCoroutine(Co_StartDifficulty(), nameof(Co_StartDifficulty), CoroutineTag.Content);
	
	private IEnumerator<float> Co_StartDifficulty()
	{
		while (true)
		{
			yield return Timing.WaitUntilTrue(() => gameState == GameState.Playing);

			levelController.moveSpeed += (levelController.moveSpeed * .05f);
			levelController.groundProbability += (levelController.groundProbability * .05f);

			Debug.Log("Add dificullty");

			yield return Timing.WaitForSeconds(10);
		}
	}

	public void StopDifficulty() => Util.KillCoroutine(nameof(Co_StartDifficulty));

	private void StartScoreCount() => Util.RunCoroutine(Co_StartScoreCount(), nameof(Co_StartScoreCount), CoroutineTag.Content);
	
	private IEnumerator<float> Co_StartScoreCount()
	{
		while (true)
		{
			yield return Timing.WaitUntilTrue(() => gameState == GameState.Playing);

			score += (int)levelController.moveSpeed * levelController.moveSpeedMultiplier;

			GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);

			yield return Timing.WaitForSeconds(.5f);
		}
	}

	public void StopScoreCount() => Util.KillCoroutine(nameof(Co_StartScoreCount));

	#endregion



	#region Util

	public void SetCameraTarget(Transform target)
	{
		virtualCamera.Follow = target;
		virtualCamera.LookAt = target;
	}

	public void ZoomCamera(float zoomValue) => Util.Zoom(virtualCamera, zoomValue, .05f);

	public void SetVirtualCamBody(Vector3 body) => Util.RunCoroutine(Co_SetVirtualCamBody(body), nameof(Co_SetVirtualCamBody), CoroutineTag.Content);

	private IEnumerator<float> Co_SetVirtualCamBody(Vector3 bodyPosition)
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

	public void AddGold(int amount) =>	GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(gold += (amount * goldMultiplier));

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

	public void AddSkill(ActiveSkill skill)
	{
		var activeSKill = new ActiveSkill(skill.name, skill.skilltype, skill.description, skill.thumbnailPath, skill.type);

		if (!skills.ContainsKey(activeSKill.type))
		{
			skills.Add(activeSKill.type, activeSKill);
			skills[activeSKill.type].level = 1;

			for (int i = 0; i < LocalData.gameData.activeSkills.Count; i++)
			{
				if (LocalData.gameData.activeSkills[i].type == activeSKill.type)
				{
					LocalData.gameData.activeSkills[i] = skills[activeSKill.type];
				}
			}
		}

		else
		{
			if (activeSKill.skilltype != "Passive") skills[activeSKill.type].level++;
		}



		// Skill Methods

		if (activeSKill.type == Skills.Speed)
		{
			var increaseValue = levelController.moveSpeed * ((float)skills[activeSKill.type].level * .015f);

			Debug.Log($"Speed Upagreded : {levelController.moveSpeed} -> {levelController.moveSpeed + increaseValue}");

			levelController.moveSpeed += increaseValue;
		}

		if (activeSKill.type == Skills.Damage)
		{
			var increaseValue = 5;

			Debug.Log($"Daamge Upagreded : {playerActor.health} -> {playerActor.health + increaseValue}");

			playerActor.AddDamage((int)increaseValue);
		}

		if (activeSKill.type == Skills.Gold)
		{
			goldMultiplier = skills[activeSKill.type].level + 1;
		}

		if (activeSKill.type == Skills.Exp)
		{
			expMultiplier = skills[activeSKill.type].level + 1;
		}

		if (activeSKill.type == Skills.Mana)
		{
			totalMana = (skills[activeSKill.type].level + 1) * 10;
		}

		if (activeSKill.type == Skills.CoolTime)
		{
			coolTimePercentage = skills[activeSKill.type].level + 1 * 10;
		}

		if (activeSKill.type == Skills.Execution)
		{
			Debug.Log("SKillThunder");

			GameManager.UI.FetchPanel<Panel_HUD>().SetSlot(Skills.Execution);

			Util.RunCoroutine(Co_UseThunder().Delay(1f), nameof(Co_UseThunder), CoroutineTag.Content);
		}
	}


	private IEnumerator<float> Co_UseThunder()
	{
		while (true)
		{
			yield return Timing.WaitForOneFrame;

			MonsterActor[] monsters = FindObjectsOfType<MonsterActor>();
			MonsterActor closestMonster = null;
			float closestDistance = Mathf.Infinity;

			yield return Timing.WaitUntilTrue(() => Scene.game.gameState == GameState.Playing);

			foreach (MonsterActor monster in monsters)
			{
				float distance = Vector3.Distance(this.transform.position, monster.transform.position);

				if (distance < closestDistance && this.transform.position.x - 0.5f < monster.transform.position.x && this.transform.position.x + 8f >= monster.transform.position.x && !monster.isDead)
				{
					closestDistance = distance;
					closestMonster = monster;
				}
			}

			if (closestMonster != null)
			{
				if (thunderMana > Scene.game.totalMana)
				{
					Debug.Log("Not enough mana");
				}

				else
				{
					if(!closestMonster.isDead)
					{
						Scene.game.totalMana -= thunderMana;

						GameManager.UI.FetchPanel<Panel_HUD>().UseSkill(Skills.Execution, thunderCoolTime - (thunderCoolTime * (Scene.game.coolTimePercentage * 0.01f)));

						closestMonster.Die();

						PoolManager.Spawn("Thunder", Vector3.zero, Quaternion.identity, closestMonster.transform);
						
						GameManager.Sound.PlaySound("Spawn", .5f);

						yield return Timing.WaitForSeconds(thunderCoolTime - (thunderCoolTime * (Scene.game.coolTimePercentage * 0.01f)));
					}
				}

				yield return Timing.WaitUntilTrue(() => Scene.game.totalMana - thunderMana >= 0);
			}
		}
	}

	public int thunderCoolTime = 10;
	public int thunderMana = 10;



	public int totalMana = 100;
	public int coolTimePercentage = 0;
	public int goldMultiplier = 1;
	public int expMultiplier = 1;

	private void ShowInterstitialAd()
	{
		if (replayCount == 0)
		{
			replayRandomCount = UnityEngine.Random.Range(3, 5);
		}

		replayCount++;

		if (replayCount >= replayRandomCount)
		{
			replayCount = 0;
			DebugManager.Log("Showing Interstitial Ad.", DebugColor.AD);
		}
	}

	public void AddGameExp()
	{
		if (Scene.game.level == 30) return;

		var amount = LocalData.masterData.inGameLevel[Scene.game.level - 1].monsterExp * expMultiplier;

		GameManager.UI.FetchPanel<Panel_HUD>().SetExpUI(amount);
	}

	#endregion
}