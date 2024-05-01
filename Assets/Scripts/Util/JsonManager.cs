using System.IO;
using UnityEngine;

public static class JsonManager<T>
{
	public static void SaveData(T data, string fileName)
	{
		var filePath = Path.Combine(Application.streamingAssetsPath, fileName);

		string json = JsonUtility.ToJson(data);

		File.WriteAllText(filePath, json);

		DebugManager.Log($"{fileName} saved safely!", DebugColor.Data);
	}

	public static T LoadData(string fileName)
	{
		var filePath = Path.Combine(Application.streamingAssetsPath, fileName);

		if (File.Exists(filePath))
		{
			string json = File.ReadAllText(filePath);

			return JsonUtility.FromJson<T>(json);
		}
		else
		{
			Debug.LogWarning("File does not exist: " + filePath);

			return default(T);
		}
	}
}