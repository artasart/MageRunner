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
}

[Serializable]
public class GameData
{

}

[Serializable]
public class VersionData
{
	public string version;
	public string date;
	public string status;
}