using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MasterDataRes
{
	public Auth[] auth;
}

[System.Serializable]
public class Auth
{
	public string key;
	public string kor;
	public string eng;
}

[System.Serializable]
public class LocalMasterData
{
	public string value;

	public LocalMasterData(string key)
	{
		value = MasterDataManager.GetLocalizedString(key, Util.GetLanguage());
	}
}

public static class MasterDataManager
{
	private static Dictionary<string, Dictionary<string, string>> authData = new Dictionary<string, Dictionary<string, string>>();

	public static void Initialize(MasterDataRes masterData)
	{
		authData.Clear();

		foreach (var item in masterData.auth)
		{
			Dictionary<string, string> localizedStrings = new Dictionary<string, string>();
			localizedStrings.Add("kor", item.kor);
			localizedStrings.Add("eng", item.eng);

			authData.Add(item.key, localizedStrings);
		}
	}

	public static string GetLocalizedString(string key, string language)
	{
		string localizedString = "";

		if (authData.ContainsKey(key))
		{
			Dictionary<string, string> localizedStrings = authData[key];

			if (localizedStrings.ContainsKey(language))
			{
				localizedString = localizedStrings[language];
			}
			else
			{
				Debug.LogError("Language not found for key: " + key);
			}
		}
		else
		{
			Debug.LogError("Key not found: " + key);
		}

		return localizedString;
	}
}