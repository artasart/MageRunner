using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalData
{
	public static GameData gameData;
	public static MasterData masterData;
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

	public int coin;
	public int highScore;

	public SerializableDictionary<EquipmentType, Equipment> equipment;
	//public SerializableDictionary<EquipmentType, Weapon> weapon;
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