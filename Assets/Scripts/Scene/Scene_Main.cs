using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;
using System.Linq;
using static Enums;
using System.Text.RegularExpressions;
using System;

public class Scene_Main : SceneLogic
{
	CinemachineVirtualCamera virtualCamera;
	GameObject renderTextureCamrea;

	EquipmentManager equipmentManager;

	private void OnDestroy()
	{
		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
		JsonManager<InvenData>.SaveData(LocalData.invenData, Define.JSON_INVENDATA);
	}

	private void OnDisable()
	{
		LocalData.gameData.equipment = equipmentManager.equipments;
	}

	protected override void Awake()
	{
		base.Awake();

		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);
		LocalData.invenData = JsonManager<InvenData>.LoadData(Define.JSON_INVENDATA);

		if(LocalData.invenData == null)
		{
			LocalData.invenData = new InvenData();
			LocalData.invenData.invenItemData = new List<InvenItemData>();
		}

		if (LocalData.masterData == null)
		{
			DebugManager.Log("WARNING!! NO MASTER DATA.", DebugColor.Data);

			GameManager.Scene.LoadScene(SceneName.Logo);

			PlayerPrefs.SetString("Version", string.Empty);

			return;
		}

		PoolManager.InitPool();

		equipmentManager = FindObjectOfType<EquipmentManager>();

		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

		renderTextureCamrea = GameObject.Find("RenderTextureCamera");
	}

	public List<InvenItemData> equipmentData = new List<InvenItemData>();

	private void Start()
	{ 
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_Main>(true);

		GameManager.UI.StackLastPopup<Popup_Basic>();

		PoolManager.SetPoolData("Puff", 10, Define.PATH_VFX);

		GetFarmedItem();
	}

	public void GetFarmedItem()
	{
		LocalData.gameData = JsonManager<GameData>.LoadData(Define.JSON_GAMEDATA);

		if (LocalData.gameData != null)
		{
			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(LocalData.gameData.gainedItems.Count > 0);

			foreach (var element in LocalData.gameData.gainedItems)
			{
				string thumnailPath = RemoveAssetPath(element.Key).Replace(".png", "");
				string filename = GetFileNameFromPath(element.Key).Replace(".png", "");
				int quantity = element.Value;

				var invenItem = LocalData.masterData.itemData.FirstOrDefault(item => item.filename == filename);

				var itemData = new InvenItemData();
				itemData.type = Util.String2Enum<EquipmentType>(invenItem.type);
				itemData.name = invenItem.name;
				itemData.nameIndex = ExtractSubstring(invenItem.filename);
				itemData.index = ExtractNumber(invenItem.filename);
				itemData.price = invenItem.price;
				itemData.quantity = quantity;
				itemData.thumbnail = thumnailPath;

				equipmentData.Add(itemData);
			}

			foreach (var item in equipmentData)
			{
				LocalData.invenData.invenItemData.Add(item);
			}

			JsonManager<InvenData>.SaveData(LocalData.invenData, Define.JSON_INVENDATA);

			LocalData.gameData.gainedItems.Clear();
			LocalData.gameData.gainedItems = null;

			JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
		}

		else
		{
			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(false);
		}
	}

	public static string ExtractSubstring(string originalString)
	{
		// "_1"�� ���� ������ ������ ����ǥ�������� �����մϴ�.
		string pattern = @"_\d+$";

		// ����ǥ������ ����Ͽ� ��Ī�Ǵ� �κ��� �����մϴ�.
		string extractedString = Regex.Replace(originalString, pattern, "");

		return extractedString;
	}

	public static string RemoveAssetPath(string filePath)
	{
		// "Assets/Resources/" �κ��� �����մϴ�.
		string modifiedPath = filePath.Replace("Assets/Resources/", "");
		return modifiedPath;
	}

	public static int ExtractNumber(string input)
	{
		// ����ǥ������ ����Ͽ� ���ڸ� �����մϴ�.
		Match match = Regex.Match(input, @"\d+");
		if (match.Success)
		{
			// ������ ���ڸ� int�� ��ȯ�Ͽ� ��ȯ�մϴ�.
			return int.Parse(match.Value);
		}
		else
		{
			// ���ڰ� ���� ��� 0�� ��ȯ�ϰų� ���� ó���� ������ �� �ֽ��ϴ�.
			throw new InvalidOperationException("���ڸ� ã�� �� �����ϴ�.");
		}
	}

	public string GetFileNameFromPath(string filePath)
	{	
		string[] pathParts = filePath.Split('/');

		string fileName = pathParts[pathParts.Length - 1];

		return fileName;
	}

	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	GameManager.UI.FetchPanel<Panel_Main>().ShowRewardAd();
		//}
	}

	public void UpCamera()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .5f, -10f));
		SetRenderCameraY(.5f);
		SetRenderCameraSize(1.2f);
	}

	public void DownCamera()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .4f, -10f));
		SetRenderCameraY(.4f);
		SetRenderCameraSize(1f);
	}

	public void ZoomInCamera()
	{
		Util.Zoom(virtualCamera, 1f, .05f);
	}

	public void ZoomOutCamera()
	{
		Util.Zoom(virtualCamera, 3f, .05f);
	}

	private void SetRenderCameraY(float y)
	{
		var position = renderTextureCamrea.transform.position;

		renderTextureCamrea.transform.position = new Vector3(position.x, y, position.z);
	}

	private void SetRenderCameraSize(float size)
	{
		renderTextureCamrea.GetComponent<Camera>().orthographicSize = size;
	}
}