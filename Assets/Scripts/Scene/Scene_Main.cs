using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine;
using Cinemachine;
using TMPro;
using static Enums;
using MEC;

public class Scene_Main : SceneLogic
{
	#region Members

	List<InvenItemData> invenData = new List<InvenItemData>();

	public CinemachineVirtualCamera virtualCamera { get; private set; }
	public EquipmentManager equipmentManager { get; private set; }
	public RideManager rideManager { get; private set; }
	public GameObject playerActor { get; private set; }
	public GameObject playerHorseActor { get; private set; }
	public Transform particle_RingShield { get; private set; }
	public Transform renderTextureCam { get; private set; }
	public GameObject navigator { get; private set; }

	[HideInInspector] public int payAmount = 10000;
	[HideInInspector] public float adWaitTime = 120f;

	#endregion



	#region Initialize

	protected override void OnDestroy()
	{
		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
		JsonManager<InvenData>.SaveData(LocalData.invenData, Define.JSON_INVENDATA);
	}

	private void OnDisable()
	{
		LocalData.gameData.equipment = equipmentManager.equipments;
	}

	public void SaveData()
	{
		LocalData.gameData.equipment = equipmentManager.equipments;

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
		JsonManager<InvenData>.SaveData(LocalData.invenData, Define.JSON_INVENDATA);
	}

	protected override void Awake()
	{
		base.Awake();

		LocalData.LoadMasterData();
		LocalData.LoadGameData();
		LocalData.LoadInvenData();

		equipmentManager = FindObjectOfType<EquipmentManager>();
		rideManager = FindObjectOfType<RideManager>();
		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		renderTextureCam = GameObject.Find(Define.RENDER_TEXTURE_CAMERA).transform;

		playerActor = GameObject.Find("PlayerActor");
		playerActor.transform.Search("hp").GetComponent<TMP_Text>().text = "Lv." + LocalData.gameData.level;

		playerHorseActor = GameObject.Find("PlayerHorseActor");
		playerHorseActor.transform.Search("hp").GetComponent<TMP_Text>().text = "Lv." + LocalData.gameData.level;

		particle_RingShield = playerActor.transform.Search(nameof(particle_RingShield));

		PoolManager.InitPool();
		PoolManager.SetPoolData(Define.VFX_PUFF, 10, Define.PATH_VFX);

		GameScene.main = this;
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .5f);

		GameManager.UI.Restart();
		GameManager.UI.StackLastPopup<Popup_Basic>();
		GameManager.UI.StartPanel<Panel_Main>(true);
		GameManager.UI.StackPanel<Panel_Shop>();
		GameManager.UI.PopPanel();
		GameManager.UI.FetchPanel<Panel_Main>().SetGoldUI(LocalData.gameData.gold);

		navigator = GameManager.UI.FetchPanel<Panel_Main>().group_TopMenu.gameObject;

		Util.RunCoroutine(Co_MainStart(), nameof(Co_MainStart));
	}

	private IEnumerator<float> Co_MainStart()
	{
		if (LocalData.gameData.nickname == string.Empty)
		{
			GameManager.UI.FetchPanel<Panel_Main>().SetUserInfo("nickname-empty", "0");
		}
		else
		{
			GameManager.UI.FetchPanel<Panel_Main>().SetUserInfo(LocalData.gameData.nickname, LocalData.gameData.runnerTag.ToString());
		}

		yield return Timing.WaitUntilTrue(() => GameManager.Scene.IsFaded());

#if UNITY_EDITOR

		if (string.IsNullOrEmpty(LocalData.gameData.nickname))
		{
			GameManager.UI.FetchPanel<Panel_Main>().GetComponent<CanvasGroup>().blocksRaycasts = false;

			yield return Timing.WaitForSeconds(.5f);

			GameManager.UI.StackSplash<Splash_Notice>().SetTimer();
		}

		var tag = UnityEngine.Random.Range(100000, 999999);

		LocalData.gameData.nickname = "test nickname for editor";
		LocalData.gameData.runnerTag = tag;

		GameManager.UI.FetchPanel<Panel_Main>().SetUserInfo("test nickname for editor", tag.ToString());

#elif UNITY_IOS

		if (string.IsNullOrEmpty(LocalData.gameData.nickname))
		{
			GameManager.UI.FetchPanel<Panel_Main>().GetComponent<CanvasGroup>().blocksRaycasts = false;

			yield return Timing.WaitForSeconds(.5f);

			GameManager.UI.StackSplash<Splash_Notice>();
			GameManager.UI.FetchSplash<Splash_Notice>().SetTimer();

			GameManager.UI.FetchSplash<Splash_Notice>().SetEndAction(() =>
			{
				GameManager.UI.StackPopup<Popup_InputField>(true);

				LocalData.gameData.runnerTag = UnityEngine.Random.Range(100000, 999999);

				GameManager.UI.FetchPanel<Panel_Main>().SetUserInfo(string.Empty, LocalData.gameData.runnerTag.ToString());

				GameManager.UI.FetchPanel<Panel_Main>().GetComponent<CanvasGroup>().blocksRaycasts = true;
			});
		}

		else
		{
			GameManager.UI.FetchPanel<Panel_Main>().SetUserInfo(LocalData.gameData.nickname, LocalData.gameData.runnerTag.ToString());
		}

#endif

		GetFarmedItem();
		CheckLogin();

		yield return Timing.WaitForOneFrame;

		if (!string.IsNullOrEmpty(LocalData.gameData.ride.name))
		{
			rideManager.Ride();
			rideManager.ChangeRide(LocalData.gameData.ride.name, 4);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameManager.UI.StackSplash<Splash_Gold>();
			GameManager.UI.FetchSplash<Splash_Gold>().OpenBox();
		}
	}

	private void CheckLogin()
	{
		if (LocalData.gameData.isAdWatched)
		{
			var ads = FindObjectsOfType<BlockAds>();

			foreach (var item in ads)
			{
				item.WatchAd(init: false);
			}
		}

		else
		{
			LocalData.gameData.isAdWatched = false;
		}

		LocalData.gameData.lastLogin = DateTime.Now;

		DateTime nextMidnight = DateTime.Today.AddDays(1);
		TimeSpan timeUntilMidnight = nextMidnight - DateTime.Now;

		Invoke(nameof(CheckDayPast), (float)timeUntilMidnight.TotalSeconds);
	}

	#endregion



	#region Util

	public void GetFarmedItem()
	{
		GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(false);

		GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(LocalData.gameData.bags.Count > 0);

		foreach (var element in LocalData.gameData.bags)
		{
			string thumnailPath = Util.RemoveAssetPath(element.Key).Replace(".png", "");
			string filename = Util.GetFileNameFromPath(element.Key).Replace(".png", "");
			int quantity = element.Value;

			var invenItem = LocalData.masterData.itemData.FirstOrDefault(item => item.filename == filename);

			var itemData = new InvenItemData(invenItem.name);
			itemData.type = Util.String2Enum<EquipmentType>(invenItem.type);
			itemData.nameIndex = Util.ExtractSubstring(invenItem.filename);
			itemData.index = Util.ExtractNumber(invenItem.filename);
			itemData.price = invenItem.price;
			itemData.quantity = quantity;
			itemData.thumbnail = thumnailPath;

			invenData.Add(itemData);
		}

		foreach (var item in invenData)
		{
			LocalData.invenData.invenItemData.Add(item);
		}

		LocalData.gameData.bags.Clear();
		LocalData.gameData.bags = null;
	}


	public void CameraUp()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .5f, -10f));

		SetCameraPositionY(.5f);
		SetCameraSize(1.2f);

		particle_RingShield = playerActor.transform.Search(nameof(particle_RingShield));
	}

	public void CameraDown()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .4f, -10f));

		SetCameraPositionY(.4f);
		SetCameraSize(1f);

		particle_RingShield = playerHorseActor.transform.Search(nameof(particle_RingShield));
	}

	private void SetCameraPositionY(float y)
	{
		var position = renderTextureCam.position;

		renderTextureCam.position = new Vector3(position.x, y, position.z);
	}

	private void SetCameraSize(float size)
	{
		renderTextureCam.GetComponent<Camera>().orthographicSize = size;
	}


	private void CheckDayPast()
	{
		if (LocalData.gameData.energy < LocalData.gameData.energyTotal)
		{
			LocalData.gameData.energy = LocalData.gameData.energyTotal;
		}
	}

	public void AddGold(int amount)
	{
		LocalData.gameData.gold += amount;

		GameManager.UI.FetchPanel<Panel_Main>().SetGoldUI(LocalData.gameData.gold);
	}

	#endregion
}