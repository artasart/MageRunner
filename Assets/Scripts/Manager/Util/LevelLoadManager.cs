using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadManager : MonoBehaviour
{
	LevelElement[] elements;
	LevelData levelData;

	private void Awake()
	{

	}

	public void ClearLevel()
	{
		Transform child = this.transform.GetChild(0);

		if (child != null)
		{
			Destroy(child.gameObject);
		}

		DebugManager.ClearLog("Level Destroyed.");
	}

	public void LoadLevel(string leveFile)
	{
		levelData = JsonManager<LevelData>.LoadData(leveFile);

		if (levelData == null)
		{
			Debug.Log("NO Data");

			levelData = new LevelData();
		}

		else
		{
			Debug.Log("levelData is loaded.");

			var levelHolder = new GameObject();
			levelHolder.name = "LevelHolder";
			levelHolder.transform.SetParent(this.transform);

			for (int i = 0; i < levelData.elements.Length; i++)
			{
				var prefab = Resources.Load<GameObject>(Define.PATH_CONTENTS + levelData.elements[i].type);

				Instantiate(prefab, new Vector3(float.Parse(levelData.elements[i].x), float.Parse(levelData.elements[i].y), 0f), Quaternion.identity, levelHolder.transform);
			}
		}

		DebugManager.ClearLog("Levels Loaded!");
	}

	public void SaveLevel()
	{
		elements = FindObjectsOfType<LevelElement>();

		levelData = new LevelData();
		levelData.elements = new Element[elements.Length];

		for (int i = 0; i < levelData.elements.Length; i++)
		{
			levelData.elements[i] = new Element();
			levelData.elements[i].type = elements[i].type.ToString();
			levelData.elements[i].x = elements[i].transform.position.x.ToString("N2");
			levelData.elements[i].y = elements[i].transform.position.y.ToString("N2");
		}

		JsonManager<LevelData>.SaveData(levelData, "levelData.json");

		DebugManager.ClearLog("Levels Saved!");
	}
}

[Serializable]
public class LevelData
{
	public Element[] elements;
}

[Serializable]
public class Element
{
	public string type;
	public string x;
	public string y;
}