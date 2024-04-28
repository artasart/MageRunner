using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using EnhancedScrollerDemos.GridSimulation;
using System.Linq;
using static Enums;
using System.Text.RegularExpressions;
using System;
using TMPro;

public class Scene_Main : SceneLogic
{
	public CinemachineVirtualCamera virtualCamera { get; private set; }
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

		LocalData.gameData = JsonManager<GameData>.LoadData(Define.JSON_GAMEDATA);
		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);
		LocalData.invenData = JsonManager<InvenData>.LoadData(Define.JSON_INVENDATA);

		if(LocalData.gameData == null)
		{
			LocalData.gameData = new GameData();
			LocalData.gameData.ride = new Ride();
			LocalData.gameData.equipment = new SerializableDictionary<EquipmentType, Equipment>();
			LocalData.gameData.bags = new SerializableDictionary<string, int>();
			LocalData.gameData.passiveSkills = new SerializableDictionary<Skills, PlayerPassiveSkill>();
			LocalData.gameData.activeSkills = new List<ActiveSkill>();
			LocalData.gameData.gold += 1000000;
		}

		if(LocalData.invenData == null)
		{
			LocalData.invenData = new InvenData();
			LocalData.invenData.invenItemData = new List<InvenItemData>();
			LocalData.invenData.amount = 0;
			LocalData.invenData.totalAmount = 10;
			LocalData.invenData.stashLevel = 1;
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

		Scene.main = this;
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

		GameManager.UI.FetchPanel<Panel_Main>().SetGold(LocalData.gameData.gold);

		GameObject.Find("PlayerActor").transform.Search("hp").GetComponent<TMP_Text>().text = "Lv." + LocalData.gameData.level;
		GameObject.Find("PlayerHorseActor").transform.Search("hp").GetComponent<TMP_Text>().text = "Lv." + LocalData.gameData.level;

		GameManager.Sound.PlayBGM("Dawn");
	}

	public void GetFarmedItem()
	{
		if (LocalData.gameData != null)
		{
			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(LocalData.gameData.bags.Count > 0);

			foreach (var element in LocalData.gameData.bags)
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

			LocalData.gameData.bags.Clear();
			LocalData.gameData.bags = null;

			JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
		}

		else
		{
			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(false);
		}

		if (LocalData.gameData.passiveSkills.Count == 0)
		{
			DebugManager.Log("No Game Data", DebugColor.Data);

			int index = 0;

			Debug.Log("Count : " + LocalData.masterData.skillData.Count);

			foreach (var item in LocalData.masterData.skillData)
			{
				var passiveSkills = new PlayerPassiveSkill(item.name, "active", item.level1, item.thumbnailPath, 1);

				LocalData.gameData.passiveSkills.Add(Util.ConvertIntToEnum<Skills>(index), passiveSkills);

				index++;
			}
		}

		if (LocalData.gameData.activeSkills.Count == 0)
		{
			LocalData.gameData.activeSkills.Add(new ActiveSkill("Thunder", "Active", "Hit and damage with thunder.", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Skull", Skills.Execution));
			// LocalData.gameData.activeSkills.Add(new ActiveSkill("Explosion", "Active", "This is Explosion Skill.", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Horner", Skills.Electricute));
			// LocalData.gameData.activeSkills.Add(new ActiveSkill("PowerOverWhelming", "This is PowerOverWhelming Skill.", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Shield_A", Skills.PowerOverWhelming));

			LocalData.gameData.activeSkills.Add(new ActiveSkill("Gold", "Passive", "Gold x ", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Gold", Skills.Gold));
			LocalData.gameData.activeSkills.Add(new ActiveSkill("Exp", "Passive", "Exp x ", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Soulgem", Skills.Exp));
			LocalData.gameData.activeSkills.Add(new ActiveSkill("Damage", "Passive", "Damage + ", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Sword_A", Skills.Damage));
			LocalData.gameData.activeSkills.Add(new ActiveSkill("Mana", "Passive", "Mana + ", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Potion_Blue", Skills.Mana));
			// LocalData.gameData.activeSkills.Add(new ActiveSkill("CoolTime", "Passive", "Cool Time - ", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Sandglass", Skills.CoolTime));
			// LocalData.gameData.activeSkills.Add(new ActiveSkill("Critical", "This is Critical Skill.", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Target", Skills.Critical));
			LocalData.gameData.activeSkills.Add(new ActiveSkill("Speed", "Passive", "Speed + ", Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Talaria", Skills.Speed));
		}

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	public static string ExtractSubstring(string originalString)
	{
		// "_1"과 같은 형식의 패턴을 정규표현식으로 정의합니다.
		string pattern = @"_\d+$";

		// 정규표현식을 사용하여 매칭되는 부분을 제거합니다.
		string extractedString = Regex.Replace(originalString, pattern, "");

		return extractedString;
	}

	public static string RemoveAssetPath(string filePath)
	{
		// "Assets/Resources/" 부분을 제거합니다.
		string modifiedPath = filePath.Replace("Assets/Resources/", "");
		return modifiedPath;
	}

	public static int ExtractNumber(string input)
	{
		// 정규표현식을 사용하여 숫자만 추출합니다.
		Match match = Regex.Match(input, @"\d+");
		if (match.Success)
		{
			// 추출한 숫자를 int로 변환하여 반환합니다.
			return int.Parse(match.Value);
		}
		else
		{
			// 숫자가 없을 경우 0을 반환하거나 에러 처리를 수행할 수 있습니다.
			throw new InvalidOperationException("숫자를 찾을 수 없습니다.");
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
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameManager.UI.StackPopup<Popup_SkillUpgrade>(true);

			GameManager.UI.FetchPopup<Popup_SkillUpgrade>().Test();
		}
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