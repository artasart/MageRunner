using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenManager
{
	private const string TokenKey = "AuthToken";
	public static string TokenHeader = "Authorization";

	public static void SaveToken(string token)
	{
		PlayerPrefs.SetString(TokenKey, token);
	}

	public static string LoadToken()
	{
		return PlayerPrefs.GetString(TokenKey, "");
	}

	public static void DeleteToken()
	{
		PlayerPrefs.DeleteKey(TokenKey);
	}
}
