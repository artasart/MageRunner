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
}

[Serializable]
public class GameData
{
	public int level = 1;
	public float exp;

	public int coin;
	public int highScore;
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