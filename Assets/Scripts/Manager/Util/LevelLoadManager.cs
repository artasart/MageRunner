using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadManager : MonoBehaviour
{
	LevelElement[] elements;
	LevelData levelData;
	Ground[] grounds;

	List<GameObject> blocks = new List<GameObject>();

	#region Core

	public void DestroyLevel()
	{
		blocks.Clear();

		var groundHolder = GameObject.Find("GroundHolder");
		if(groundHolder != null ) DestroyImmediate(groundHolder);

		var coinHolder = GameObject.Find("CoinHolder");
		if (coinHolder != null ) DestroyImmediate(coinHolder);

		var portalHoler = GameObject.Find("PortalHolder");
		if (portalHoler != null) DestroyImmediate(portalHoler);

		DebugManager.Log("Level Destroyed.");
	}

	public void LoadLevel(string leveFile)
	{
		levelData = JsonManager<LevelData>.LoadData(leveFile);

		if (levelData == null)
		{
			levelData = new LevelData();
		}

		else
		{
			for (int i = 0; i < levelData.elements.Length; i++)
			{
				var levelHolder = CreateHolder(levelData.elements[i].type + "Holder");

				var prefab = Resources.Load<GameObject>(Define.PATH_CONTENTS + levelData.elements[i].type);
				var gameObject = Instantiate(prefab, new Vector3(float.Parse(levelData.elements[i].x), float.Parse(levelData.elements[i].y), 0f), Quaternion.identity, levelHolder.transform);

				if (levelData.elements[i].type == PropsType.Ground.ToString())
				{
					for (int j = 0; j < levelData.elements[i].ground.Length; j++)
					{
						bool isActive = levelData.elements[i].ground[j];

						gameObject.transform.GetChild(j).gameObject.SetActive(isActive);
					}

					gameObject.GetComponent<Ground>().isDefault = levelData.elements[i].isDefault;
				}
			}
		}
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

			if (elements[i].type == PropsType.Ground)
			{
				levelData.elements[i].ground = new bool[elements[i].transform.childCount];

				for (int j = 0; j < 5; j++)
				{
					levelData.elements[i].ground[j] = elements[i].transform.GetChild(j).gameObject.activeSelf;
				}

				levelData.elements[i].isDefault = elements[i].GetComponent<Ground>().isDefault;
			}
		}

		JsonManager<LevelData>.SaveData(levelData, Define.JSON_LEVELDATA);
	}

	#endregion



	#region Generate Ground

	public void GenerateGround(int count)
	{
		DestroyGround();

		var prefab = Resources.Load<GameObject>(Define.PATH_CONTENTS + Define.GROUND);
		var portal = Resources.Load<GameObject>(Define.PATH_CONTENTS + Define.PORTAL);
		var levelHolder = CreateHolder("GroundHolder");
		int defaultCount = 2;
		float gap = 10;

		Vector3 endPosition = Vector3.zero;

		for (int i = 0; i < defaultCount; i++)
		{
			var position = Vector3.right * gap * i;

			var startGround = Instantiate(prefab, position, Quaternion.identity, levelHolder.transform);

			startGround.GetComponent<Ground>().isDefault = true;
		}

		for (int i = 0; i < count; i++)
		{
			var position = (Vector3.right * (gap * defaultCount)) + Vector3.right * gap * i;

			var ground = Instantiate(prefab, position, Quaternion.identity, levelHolder.transform);

			ground.GetComponent<Ground>().Generate();

			endPosition = position;
		}
		


		for(int i = 0; i < defaultCount; i++)
		{
			endPosition += Vector3.right * gap;

			GameObject endGround = Instantiate(prefab, endPosition, Quaternion.identity, levelHolder.transform);
			endGround.GetComponent<Ground>().isDefault = true;
		}

		Instantiate(portal, endPosition - (Vector3.right * gap), Quaternion.identity, levelHolder.transform);



		grounds = GetComponentsInChildren<Ground>();

		for (int i = 0; i < grounds.Length; i++)
		{
			if (grounds[i].isDefault) continue;

			for (int j = 0; j < grounds[i].transform.childCount; j++)
			{
				blocks.Add(grounds[i].transform.GetChild(j).gameObject);
			}
		}

		RandomizeGround();
	}

	public void RandomizeGround()
	{
		var grounds = FindObjectsOfType<Ground>();

		for (int i = 0; i < grounds.Length; i++)
		{
			if (grounds[i].isDefault) continue;

			grounds[i].Generate();
		}
	}

	public void DestroyGround()
	{
		blocks.Clear();

		var groundHolder = GameObject.Find("GroundHolder");

		DestroyImmediate(groundHolder);
	}

	#endregion



	#region Generate Coin

	public void GenerateCoin(int coinCount)
	{
		var prefab = Resources.Load<GameObject>(Define.PATH_CONTENTS + Define.COIN);
		var bigCoinPrefab = Resources.Load<GameObject>(Define.PATH_CONTENTS + Define.BIGCOIN);
		var levelHolder = CreateHolder("CoinHolder");


		for (int i = 0; i < blocks.Count; i++)
		{
			if (blocks[i].activeSelf)
			{
				for (int j = 0; j < 4; j++)
				{
					float x = -.75f + blocks[i].transform.position.x + (j * .5f);

					Instantiate(prefab, new Vector3(x, 0.175f, 0f), Quaternion.identity, levelHolder.transform);
				}
			}

			else
			{
				Instantiate(bigCoinPrefab, blocks[i].transform.position + Vector3.up * 2.5f, Quaternion.identity, levelHolder.transform);
			}
		}
	}

	public void DestroyCoin()
	{
		var coinHolder = GameObject.Find("CoinHolder");

		DestroyImmediate(coinHolder);
	}

	#endregion



	#region Util

	private GameObject CreateHolder(string name = "")
	{
		var levelHolder = GameObject.Find(name);

		if (levelHolder == null)
		{
			levelHolder = new GameObject(name);
			levelHolder.transform.SetParent(this.transform);
		}

		return levelHolder;
	}

	#endregion
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

	public bool[] ground;
	public bool isDefault;
}