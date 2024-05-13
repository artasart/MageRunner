using EnhancedScrollerDemos.GridSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static Enums;

public static class LocalData
{
	public static GameData gameData;
	public static MasterData masterData;
	public static InvenData invenData;

	public static void LoadMasterData()
	{
		masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);

		if (masterData == null)
		{
			DebugManager.Log("WARNING!! NO MASTER DATA.", DebugColor.Data);

			PlayerPrefs.SetString("Version", string.Empty);

			GameManager.Scene.LoadScene(SceneName.Logo);
		}
	}

	public static void LoadGameData()
	{
		gameData = JsonManager<GameData>.LoadData(Define.JSON_GAMEDATA);

		if (gameData == null)
		{
			InitGameData();

			JsonManager<GameData>.SaveData(gameData, Define.JSON_GAMEDATA);
		}
	}

	public static void InitGameData()
	{
		Debug.Log("InitGameData");

		gameData = new GameData();
		gameData.ride = new Ride("", 0);
		gameData.equipment = new SerializableDictionary<EquipmentType, Equipment>();
		gameData.bags = new SerializableDictionary<string, int>();
		gameData.gold = 1000;
		gameData.energy = 5;
		gameData.energyTotal = 5;
		gameData.isPremium = false;
		gameData.adWatchTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
		gameData.damage = 1;
		gameData.mana = 100;
		gameData.speed = 5;
		gameData.damageLevel = 1;
		gameData.manaLevel = 1;
		gameData.speedLevel = 1;

		if (masterData == null) return;

		gameData.actorSkills = new List<ActorSkill>();

		for (int i = 0; i < masterData.skillData.Count; i++)
		{
			gameData.actorSkills.Add(new ActorSkill(
				masterData.skillData[i].name,
				masterData.skillData[i].type,
				masterData.skillData[i].description,
				masterData.skillData[i].thumbnailPath,
				masterData.skillData[i].maxLevel,
				masterData.skillData[i].value,
				1));
		}

		foreach (var item in gameData.equipment)
		{
			item.Value.path = string.Empty;
		}
	}

	public static void SaveGameData(int score, int gold, SerializableDictionary<string, int> bags)
	{
		if (score > gameData.highScore)
		{
			gameData.highScore = score;
		}

		gameData.gold += gold;
		gameData.bags = bags;

		JsonManager<GameData>.SaveData(gameData, Define.JSON_GAMEDATA);
	}

	public static void InitSkill()
	{
		gameData.actorSkills.ToList().ForEach(item => item.level = 0);
	}

	public static void LoadInvenData()
	{
		invenData = JsonManager<InvenData>.LoadData(Define.JSON_INVENDATA);

		if (invenData == null)
		{
			invenData = new InvenData();
			invenData.invenItemData = new List<InvenItemData>();
			invenData.amount = 0;
			invenData.totalAmount = 10;
			invenData.stashLevel = 1;
		}
	}

	public static void InitInvenData()
	{
		invenData.invenItemData.Clear();
		invenData.amount = 0;
		invenData.totalAmount = 10;
		invenData.stashLevel = 1;
	}
}

public static class GameScene
{
	public static Scene_Game game;
	public static Scene_Main main;
}

[Serializable]
public class MasterData
{
	public VersionData version;

	public List<Level> levelData;
	public List<Item> itemData;
	public List<Skill> skillData;

	public List<SkillUpgrade> skillUpgradeData;
	public List<InGameLevel> inGameLevel;
	public List<SkillEntity> skillEntity;
	public List<ShopItem> shopItem;
	public List<Upgrade> upgradeData;
}

[Serializable]
public class GameData
{
	public string nickname;
	public int level = 1;
	public int exp;

	public int gold;
	public int highScore;

	public int energy;
	public int energyTotal;
	public bool isPremium;
	public int runnerTag = 1;

	public int damage = 1;
	public int mana = 100;
	public float speed = 5;

	public int damageLevel = 1;
	public int manaLevel = 1;
	public int speedLevel = 1;

	public DateTime lastLogin;
	public string adWatchTime;
	public bool isAdWatched;

	public SerializableDictionary<EquipmentType, Equipment> equipment;
	public SerializableDictionary<string, int> bags;
	public List<ActorSkill> actorSkills;

	public Ride ride;
}

[Serializable]
public class InvenData
{
	public List<InvenItemData> invenItemData;
	public int amount = 0;
	public int totalAmount = 0;
	public int stashLevel;
}


[Serializable]
public class Skill
{
	public string name;
	public string type;
	public string description;
	public string thumbnailPath;
	public int maxLevel;
	public string value;
}

[Serializable]
public class SkillUpgrade
{
	public string level;
	public string upgradeGold;
}

[Serializable]
public class SkillEntity
{
	public string name;
	public string mana;
	public float cooltime;
}








[Serializable]
public class VersionData
{
	public string version;
	public string date;
	public string status;
}

[Serializable]
public class Level
{
	public int level;
	public int exp;
}

[Serializable]
public class Ride
{
	public string name;
	public int index;

	public Ride(string name, int index)
	{
		this.name = name;
		this.index = index;
	}
}

[Serializable]
public class Item
{
	public string name;
	public string type;
	public string payment;
	public string rank;
	public string stat;
	public int price;
	public string description;
	public string filename;
}

[Serializable]
public class Equipment
{
	public EquipmentType type;
	public string name;
	public int index;
	public string path;

	protected readonly string basePath = "Assets/Resources/SPUM/SPUM_Sprites/";
	protected readonly string[] categories = { "Items/", "Packages/" };
	protected readonly string[] versions = { "", "Ver121/", "Ver300/", "F_SR/" };

	public Equipment(EquipmentType type, string filename)
	{
		this.type = type;
		this.name = ExtractTypeAndNumber(filename).name;
		this.index = ExtractTypeAndNumber(filename).index;

		string fileName = $"{name}_{index}.png";

		int categoryIndex = IsPackageItem(fileName) ? 1 : 0;
		int versionIndex = GetVersionIndex(fileName);

		string category = categories[categoryIndex];
		string version = versions[versionIndex];

		string typeName = $"{(int)type}_{type}/";

		path = $"{basePath}{category}{version}{typeName}{fileName}";
	}

	public Equipment(EquipmentType type, string name, int index)
	{
		this.type = type;
		this.name = name;
		this.index = index;

		string fileName = $"{name}_{index}.png";

		int categoryIndex = IsPackageItem(fileName) ? 1 : 0;
		int versionIndex = GetVersionIndex(fileName);

		string category = categories[categoryIndex];
		string version = versions[versionIndex];

		var typeName = $"{(int)type}_{type}/";

		if (name != string.Empty) path = $"{basePath}{category}{version}{typeName}{fileName}";
	}

	private bool IsPackageItem(string fileName)
	{
		return fileName.Contains("Normal") || fileName.Contains("New") || fileName.Contains("F_SR");
	}

	private int GetVersionIndex(string fileName)
	{
		if (fileName.Contains("Normal"))
			return 1;
		if (fileName.Contains("New"))
			return 2;
		if (fileName.Contains("F_SR"))
			return 3;
		return 0;
	}

	public string GetPath() => path;

	public (string name, int index) ExtractTypeAndNumber(string itemName)
	{
		string[] parts = itemName.Split('_');

		if (parts.Length >= 3 && int.TryParse(parts.Last(), out int number))
		{
			string type = string.Join("_", parts.Take(parts.Length - 1));
			return (type, number);
		}
		else
		{
			throw new ArgumentException("Invalid item name format.");
		}
	}
}

[Serializable]
public class InGameLevel
{
	public int level;
	public int exp;
	public float coinProbability;
	public float skillCardProbability;
	public float monsterProbability;
	public int monsterExp;
	public int monsterDamage;
}

[Serializable]
public class ShopItem
{
	public string name;
	public string type;
	public string category;
	public int amount;
	public int price;
	public string thumbnailPath;
}

[Serializable]
public class Upgrade
{
	public int gold;
	public int damage;
	public int mana;
	public int speed;
}