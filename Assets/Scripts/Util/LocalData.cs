using EnhancedScrollerDemos.GridSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public static class LocalData
{
	public static GameData gameData;
	public static MasterData masterData;
	public static InvenData invenData;
}

[Serializable]
public class MasterData
{
	public VersionData version;
	public List<Level> levelData;
	public List<Item> itemData;
}

[Serializable]
public class GameData
{
	public int level = 1;
	public int exp;

	public int gold;
	public int highScore;

	public SerializableDictionary<EquipmentType, Equipment> equipment;
	public Ride ride;

	public SerializableDictionary<string, int> gainedItems;
}

[Serializable]
public class InvenData
{
	public List<InvenItemData> invenItemData;
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

		path = $"{basePath}{category}{version}{typeName}{fileName}";
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