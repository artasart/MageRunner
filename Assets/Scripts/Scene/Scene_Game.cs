using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MEC;
using System;
using System.Linq;
using static Enums;

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
	public int scoreMultiplier = 1;
	public int moveMultiplier = 3;

	[Header("Ride")]
	public bool isRide = false;

	public GameObject actor;
	public GameObject rideActor;

	[Header("Skill")]
	public SerializableDictionary<string, ActorSkill> actorSkills = new SerializableDictionary<string, ActorSkill>();

	[Header("Game Data")]
	public SerializableDictionary<string, int> bags = new SerializableDictionary<string, int>();

	private CinemachineVirtualCamera virtualCamera { get; set; }
	public CameraShake3D cameraShake { get; private set; }
	public PlayerActor playerActor { get; set; }
	public LevelController levelController { get; private set; }
	public ParallexScrollController[] parallexScrollController { get; private set; }
	public FootStepController footStepController { get; private set; }
	public EquipmentManager equipmentManager { get; private set; }

	GameObject playerModel;

	int replayCount = 0;
	int replayRandomCount = 0;

	#endregion



	#region Initialize

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SaveGameData();

		GameManager.Backend.SetGameData();

		Util.KillCoroutine(nameof(Co_CheckTimeForAd));
	}

	private void OnDisable()
	{
		for (int i = 0; i < LocalData.gameData.actorSkills.Count; i++)
		{
			LocalData.gameData.actorSkills[i].level = 0;
		}

		StopScoreCount();
		StopDifficulty();
	}

	protected override void Awake()
	{
		base.Awake();

		GameScene.game = this;

		LoadGameData();
		MakePool();

		levelController = FindObjectOfType<LevelController>();
		parallexScrollController = FindObjectsOfType<ParallexScrollController>();
		cameraShake = FindObjectOfType<CameraShake3D>();
		equipmentManager = FindObjectOfType<EquipmentManager>();
		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		virtualCamera.m_Lens.OrthographicSize = 5f;

		if (LocalData.gameData.ride.name != string.Empty)
		{
			actor.SetActive(false);
			playerActor = rideActor.GetComponent<PlayerActor>();
			playerModel = playerActor.transform.GetChild(0).gameObject;
			playerModel.SetActive(false);
			isRide = true;
			levelController.moveSpeed = 6f;

			footStepController = rideActor.GetComponent<FootStepController>();
		}

		else
		{
			rideActor.SetActive(false);
			playerActor = actor.GetComponent<PlayerActor>();
			playerModel = playerActor.transform.GetChild(0).gameObject;
			playerModel.SetActive(false);
			isRide = false;

			footStepController = actor.GetComponent<FootStepController>();
		}

		SetCameraTarget(playerActor.transform);
	}

	private void Start()
	{
		GameManager.AdMob.initialClosed += () => { initialClosed = true; Time.timeScale = 1f; };

		GameManager.Scene.Fade(false, .075f);
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

		var target = isRide ? 3.5f : 3.2f;

		Util.Zoom(virtualCamera, target, .025f);

		MoveEnviroment();

		yield return Timing.WaitUntilTrue(() => virtualCamera.m_Lens.OrthographicSize == target);

		equipmentManager.EquipAll(LocalData.gameData.equipment);

		GameManager.Sound.PlaySound(Define.SOUND_THUNDER, .5f);
		PoolManager.Spawn(Define.VFX_THUNDER, new Vector3(0f, -0.2f, 0f), Quaternion.identity);
		cameraShake.Shake();

		yield return Timing.WaitForSeconds(.1f);

		playerModel.SetActive(true);

		if (isRide)
		{
			playerActor.GetComponent<RideController>().Ride();
		}

		footStepController.StartWalk();
		playerActor.UpdateAnimator(1f);

		StartScoreCount();
		StartDifficulty();

		GameManager.UI.StartPanel<Panel_HUD>();

		playerActor.AddDamage(LocalData.gameData.damage);

		playerActor.mana = LocalData.gameData.mana;
		playerActor.manaTotal = LocalData.gameData.mana;

		levelController.moveSpeedOrigin = LocalData.gameData.speed;
		levelController.moveSpeed = LocalData.gameData.speed;

		GameManager.UI.FetchPanel<Panel_HUD>().SetManaUI();

		if (isRide) playerActor.AddDamage(50);

		Util.RunCoroutine(Co_CheckTimeForAd(), nameof(Co_CheckTimeForAd));
	}

	public void Replay()
	{
		DebugManager.Log("Retry", DebugColor.Game);

		LocalData.InitSkill();
		LocalData.gameData.energy -= 1;

		score = gold = 0;
		level = 1;
		goldMultiplier = 1;
		expMultiplier = 1;
		cooltimeMultiplier = 0;
		isThunderRunning = false;
		isSkillUsed = false;

		actorSkills = new SerializableDictionary<string, ActorSkill>();

		Util.KillCoroutine(nameof(Co_UseThunder));

		Util.RunCoroutine(Co_Replay(), nameof(Co_Replay), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_Replay()
	{
		GameScene.game.ShowInterstitialAd();

		if(initialClosed== false && isWatched)
		{ 
		yield return Timing.WaitUntilTrue(() => initialClosed);

					initialClosed = false;
				}

		GameManager.Scene.Fade(true);

		yield return Timing.WaitUntilTrue(GameManager.Scene.IsFaded);

		GameManager.UI.PopPopup(true);

		SetCameraTarget(playerActor.transform);

		levelController.Refresh();
		levelController.gameObject.SetActive(false);
		playerActor.Refresh();

		virtualCamera.m_Lens.OrthographicSize = 5f;

		var target = isRide ? 3.5f : 3.2f;
		Util.Zoom(virtualCamera, target, .025f);

		MoveEnviroment();

		footStepController.StartWalk();
		playerActor.UpdateAnimator(1f);
		levelController.gameObject.SetActive(true);

		GameManager.Scene.Fade(false);
		GameManager.UI.StartPanel<Panel_HUD>();

		yield return Timing.WaitUntilFalse(GameManager.Scene.IsFaded);

		StartDifficulty();
		StartScoreCount();

		playerActor.isDead = false;

		playerActor.AddDamage(LocalData.gameData.damage);

		playerActor.mana = LocalData.gameData.mana;
		playerActor.manaTotal = LocalData.gameData.mana;

		levelController.moveSpeedOrigin = LocalData.gameData.speed;
		levelController.moveSpeed = LocalData.gameData.speed;

		GameManager.UI.FetchPanel<Panel_HUD>().SetManaUI();

		if (isRide) playerActor.AddDamage(50);

		GameManager.UI.FetchPanel<Panel_HUD>().Refresh();
	}

	public void GameOver()
	{
		DebugManager.Log("Game Over", DebugColor.Game);

		Util.KillCoroutine(nameof(Co_UseThunder));

		Invoke(nameof(ShowGameResult), .5f);
	}

	private void ShowGameResult()
	{
		if (GameScene.game.score > LocalData.gameData.highScore)
		{
			LocalData.gameData.highScore = GameScene.game.score;

			GameManager.UI.StackSplash<Splash_Congrates>();

			GameManager.UI.FetchSplash<Splash_Congrates>().SetEndAction(() =>
			{
				GameManager.UI.FetchPopup<Popup_GameOver>().SetResult(
						GameScene.game.score,
						GameScene.game.gold,
						GameScene.game.exp = Mathf.RoundToInt(GameScene.game.score * .45f)
				);

				Invoke(nameof(ShowGameOverUI), .15f);
			});

			GameManager.UI.FetchSplash<Splash_Congrates>().SetScore(GameScene.game.score);
		}

		else
		{
			GameManager.UI.FetchPopup<Popup_GameOver>().SetResult(
			GameScene.game.score,
			GameScene.game.gold,
			GameScene.game.exp = Mathf.RoundToInt(GameScene.game.score * .45f)
			);

			GameManager.UI.PopPopupAll();
			GameManager.UI.StartPopup<Popup_GameOver>();
		}
	}

	private void ShowGameOverUI()
	{
		GameManager.UI.PopPopupAll();
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
		var seconds = 10f;

		while (true)
		{
			yield return Timing.WaitForSeconds(seconds);

			levelController.moveSpeed = Mathf.Clamp(levelController.moveSpeed += (levelController.moveSpeed * .01f), 0f, 8.5f);
			levelController.groundProbability += (levelController.groundProbability * .05f);

			if (score > 7000) seconds = 5f;

			yield return Timing.WaitUntilTrue(() => gameState == GameState.Playing);
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

	public void ZoomCamera(float zoomValue, float zoomSpeed = .05f) => Util.Zoom(virtualCamera, zoomValue, zoomSpeed);

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

	public void AddScore(int amount) => GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score += amount);
	public void AddGold(int amount) => GameManager.UI.FetchPanel<Panel_HUD>().SetGoldUI(gold += (amount * goldMultiplier));

	public void AddExp(int exp)
	{
		if (LocalData.gameData.level == LocalData.masterData.levelData[LocalData.masterData.levelData.Count - 1].level)
		{
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
			LocalData.gameData.exp += exp;
		}
	}

	public void AddSkill(ActorSkill actorSkill)
	{
		var skill = new ActorSkill(actorSkill.name, actorSkill.type, actorSkill.description, actorSkill.thumbnailPath, actorSkill.maxLevel, actorSkill.value, actorSkill.level);

		if (!actorSkills.ContainsKey(skill.name))
		{
			actorSkills.Add(skill.name, skill);

			actorSkills[skill.name].level = 1;
		}

		else
		{
			if (actorSkills[skill.name].maxLevel != 0)
			{
				actorSkills[skill.name].level = Mathf.Clamp(actorSkills[skill.name].level += 1, 0, actorSkills[skill.name].maxLevel);

				DebugManager.ClearLog($"{skill.name} curren current level  : " + actorSkills[skill.name].level, DebugColor.Game);
			}
		}

		LocalData.gameData.actorSkills.FirstOrDefault(element => element.name == actorSkill.name).level = actorSkills[skill.name].level;

		if (skill.name == Skills.Gold.ToString())
		{
			var multiplier = Util.ParseStringToIntArray(actorSkills[skill.name].value);

			goldMultiplier = multiplier[actorSkills[skill.name].level - 1];

			DebugManager.Log($"Gold Upgrade : {goldMultiplier}", DebugColor.Game);
		}

		if (skill.name == Skills.Exp.ToString())
		{
			var multiplier = Util.ParseStringToIntArray(actorSkills[skill.name].value);

			expMultiplier = multiplier[actorSkills[skill.name].level - 1];

			DebugManager.Log($"Exp Upgrade : {goldMultiplier}", DebugColor.Game);
		}

		if (skill.name == Skills.Damage.ToString())
		{
			DebugManager.Log($"Daamge Added : {playerActor.health} -> {playerActor.health + Convert.ToInt32(actorSkills[skill.name].value)}", DebugColor.Game);

			playerActor.AddDamage(Convert.ToInt32(actorSkills[skill.name].value));
		}

		if (skill.name == Skills.Mana.ToString())
		{
			playerActor.manaTotal += Convert.ToInt32(actorSkills[skill.name].value);

			playerActor.mana = playerActor.manaTotal;

			GameManager.UI.FetchPanel<Panel_HUD>().SetManaUI();

			DebugManager.Log($"Mana Added : {playerActor.manaTotal}", DebugColor.Game);
		}

		if (skill.name == Skills.Speed.ToString())
		{
			var value = levelController.moveSpeed * Convert.ToInt32(actorSkills[skill.name].value) / 100;

			DebugManager.Log($"Speed Upgrade : {levelController.moveSpeed} -> {levelController.moveSpeed + value}", DebugColor.Game);

			levelController.moveSpeed += value;
		}

		if (skill.name == Skills.CoolTime.ToString())
		{
			var cooltime = Util.ParseStringToIntArray(skill.value);

			cooltimeMultiplier = cooltime[actorSkills[skill.name].level - 1];
		}

		if (skill.name == Skills.Thunder.ToString())
		{
			GameManager.UI.FetchPanel<Panel_HUD>().SetSlot(Skills.Thunder);

			if (!isThunderRunning) Util.RunCoroutine(Co_UseThunder().Delay(1f), nameof(Co_UseThunder), CoroutineTag.Content);
		}
	}

	bool isThunderRunning = false;

	private IEnumerator<float> Co_UseThunder()
	{
		isThunderRunning = true;

		var element = LocalData.masterData.skillEntity
			.Where(skill => skill.name == Skills.Thunder.ToString())
			.FirstOrDefault();

		var thudnerMana = Util.ParseStringToIntArray(element.mana);

		while (true)
		{
			MonsterActor[] monsters = FindObjectsOfType<MonsterActor>();
			MonsterActor closestMonster = null;
			float closestDistance = Mathf.Infinity;

			yield return Timing.WaitUntilTrue(() => GameScene.game.gameState == GameState.Playing);

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
				if (thudnerMana[actorSkills[Skills.Thunder.ToString()].level - 1] > playerActor.mana)
				{
					DebugManager.Log("Not enough mana.", DebugColor.Game);

					yield return Timing.WaitUntilFalse(() => thudnerMana[actorSkills[Skills.Thunder.ToString()].level - 1] > playerActor.mana);
				}

				else
				{
					if (!closestMonster.isDead)
					{
						playerActor.mana -= thudnerMana[actorSkills[Skills.Thunder.ToString()].level - 1];

						GameManager.UI.FetchPanel<Panel_HUD>().SetManaUI();

						isSkillUsed = false;

						GameManager.UI.FetchPanel<Panel_HUD>().UseSkill(Skills.Thunder, (element.cooltime - (element.cooltime * (cooltimeMultiplier * 0.01f))), action: () => isSkillUsed = true);

						closestMonster.Die();

						PoolManager.Spawn("Thunder", Vector3.zero, Quaternion.identity, closestMonster.transform);

						GameManager.Sound.PlaySound("Spawn", .5f);

						yield return Timing.WaitUntilTrue(() => isSkillUsed);
					}
				}
			}

			yield return Timing.WaitForOneFrame;
		}
	}

	public bool isSkillUsed;
	public int cooltimeMultiplier = 0;
	public int goldMultiplier = 1;
	public int expMultiplier = 1;

	public void ShowInterstitialAd()
	{
		if (!isWatched) return;

		GameManager.AdMob.ShowInterstitialAd();

		Time.timeScale = 0f;

		Util.RunCoroutine(Co_CheckTimeForAd(), nameof(Co_CheckTimeForAd));
	}

	bool isWatched = false;
	bool initialClosed = false;

	IEnumerator<float> Co_CheckTimeForAd()
	{
		isWatched = false;

		yield return Timing.WaitForSeconds(90f);

		isWatched = true;
	}

	public void AddGameExp()
	{
		if (GameScene.game.level == 30) return;

		var amount = LocalData.masterData.inGameLevel[GameScene.game.level - 1].monsterExp * expMultiplier;

		GameManager.UI.FetchPanel<Panel_HUD>().SetExpUI(amount);
	}

	#endregion
}