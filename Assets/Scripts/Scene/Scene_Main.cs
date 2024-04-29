using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;
using UnityEngine;
using Cinemachine;
using TMPro;
using static Enums;

public class Scene_Main : SceneLogic
{
	#region Members

	List<InvenItemData> invenData = new List<InvenItemData>();

	public CinemachineVirtualCamera virtualCamera { get; private set; }
	public EquipmentManager equipmentManager { get; private set; }
	public RideController rideController { get; private set; }
	public GameObject playerActor { get; private set; }
	public GameObject playerHorseActor { get; private set; }
	public Transform particle_RingShield { get; private set; }
	public Transform renderTextureCam { get; private set; }


	public int payAmount = 10000;

	#endregion



	#region Initialize

	private void OnDestroy()
	{
		Debug.Log("Current inventory : " + LocalData.invenData.invenItemData.Count);
	}

	private void OnDisable()
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
		rideController = FindObjectOfType<RideController>();
		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
		renderTextureCam = this.transform.Search(Define.RENDER_TEXTURE_CAMERA);

		playerActor = GameObject.Find("PlayerActor");
		playerActor.transform.Search("hp").GetComponent<TMP_Text>().text = "Lv." + LocalData.gameData.level;

		playerHorseActor = GameObject.Find("PlayerHorseActor");
		playerActor.transform.Search("hp").GetComponent<TMP_Text>().text = "Lv." + LocalData.gameData.level;

		particle_RingShield = playerActor.transform.Search(nameof(particle_RingShield));

		PoolManager.InitPool();
		PoolManager.SetPoolData(Define.VFX_PUFF, 10, Define.PATH_VFX);

		Scene.main = this;
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StackLastPopup<Popup_Basic>();

		GameManager.UI.StartPanel<Panel_Main>(true);

		GameManager.UI.FetchPanel<Panel_Main>().SetUserInfo("artasart", LocalData.gameData.runnerTag);

		GameManager.UI.FetchPanel<Panel_Main>().SetGold(LocalData.gameData.gold);

		GameManager.Sound.PlayBGM("Dawn");

		GetFarmedItem();

		CheckLogin();
	}

	private void CheckLogin()
	{
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
	}

	public void CameraDown()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .4f, -10f));
		SetCameraPositionY(.4f);
		SetCameraSize(1f);
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

	#endregion
}