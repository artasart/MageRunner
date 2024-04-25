using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataManager : SingletonManager<GameDataManager>
{
	public void GetVersion(Action callback)
	{
		DebugManager.ClearLog("Get Version", DebugColor.Data);

		string localVersion = PlayerPrefs.GetString("Version");
		bool isLatestVersion = false;

		GoogleSheets.GetData(Url.VERSION_SHEETID, (res) =>
		{
			var lines = res.Split('\n');

			for (int i = 1; i < lines.Length; i++)
			{
				string[] values = lines[i].Trim().Split('\t');

				var server = values[0];

				Debug.Log(server + ", local : "  + localVersion);

				if (Equals(server, localVersion))
				{
					DebugManager.Log("최신 버전 입니다.", DebugColor.Data);
					isLatestVersion = true;
					break;
				}

				PlayerPrefs.SetString("Version", values[0]);
				PlayerPrefs.SetString("Date", values[1]);
				PlayerPrefs.SetString("Status", values[2]);

				LocalData.masterData.version.version = values[0];
				LocalData.masterData.version.date = values[1];
				LocalData.masterData.version.status = values[2];

				DebugManager.Log("버전을 업데이트합니다.", DebugColor.Data);
			}

			if (!isLatestVersion)
			{
				GetSheet(callback);
			}

			else callback?.Invoke();
		});
	}

	public void GetSheet(Action callback = null)
	{
		DebugManager.Log("Sheet 데이터를 가져옵니다.", DebugColor.Data);

		LocalData.masterData ??= new MasterData();

		GoogleSheets.AddJob("Get Level Data", () => GoogleSheets.GetData(Url.LEVEL_SHEETID, SaveData<Level>));
		GoogleSheets.AddJob("Get Equipment Data", () => GoogleSheets.GetData(Url.EQUIPMENT_SHEETID, SaveData<Item>));
		GoogleSheets.AddJob("Get Skill Data", () => GoogleSheets.GetData(Url.SKILL_SHEETID, SaveData<Skill>));
		GoogleSheets.AddJob("Get SkillUpgrade Data", () => GoogleSheets.GetData(Url.SKILL_UPGRADE_SHEETID, SaveData<SkillUpgrade>));

		GoogleSheets.GetDataAll(callback);
	}

	private void SaveData<T>(string data) where T : new()
	{
		var dataList = new List<T>();
		var lines = data.Split('\n');

		for (int i = 1; i < lines.Length; i++)
		{
			string[] values = lines[i].Trim().Split('\t');

			if (values.Length >= GetColumnCount(data))
			{
				var obj = CreateObject<T>(values);

				dataList.Add(obj);
			}
		}

		SetMasterData(dataList);
	}

	private void SetMasterData<T>(List<T> dataList)
	{		
		if (typeof(T) == typeof(Level))
		{
			LocalData.masterData.levelData = dataList.Cast<Level>().ToList();
		}
		else if (typeof(T) == typeof(Item))
		{
			LocalData.masterData.itemData = dataList.Cast<Item>().ToList();
		}
		else if (typeof(T) == typeof(Skill))
		{
			LocalData.masterData.skillData = dataList.Cast<Skill>().ToList();
		}
		else if (typeof(T) == typeof(SkillUpgrade))
		{
			LocalData.masterData.skillUpgradeData = dataList.Cast<SkillUpgrade>().ToList();
		}
	}

	public T CreateObject<T>(string[] values) where T : new()
	{
		var obj = new T();
		var properties = typeof(T).GetFields();

		for (int i = 0; i < properties.Length; i++)
		{
			var property = properties[i];
			var value = i < values.Length ? values[i].Trim() : "";
			object convertedValue = GetConvertedValue(property.FieldType, value);
			property.SetValue(obj, convertedValue);
		}

		return obj;
	}

	private object GetConvertedValue(Type targetType, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
		}

		return Convert.ChangeType(value, targetType);
	}

	private int GetColumnCount(string data)
	{
		var lines = data.Split('\n');
		string[] headers = lines[0].Trim().Split('\t');

		return headers.Length;
	}
}
