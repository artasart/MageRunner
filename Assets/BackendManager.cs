using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;

public class BackendManager : SingletonManager<BackendManager>
{
	private void Awake()
	{
		var bro = Backend.Initialize(true);

		if(bro.IsSuccess())
		{
			Debug.Log("Initialized Success." + bro);
		}
		else
		{
			Debug.LogError("Initialized failed." + bro);
		}
	}

	public void LoginToBackEnd(string idToken, Action success)
	{
		BackendReturnObject bro = Backend.BMember.AuthorizeFederation(idToken, FederationType.Apple, "Apple");

		if (bro.IsSuccess())
		{
			DebugManager.Log("Apple Login Success.", DebugColor.Login);

			PlayerPrefs.SetString("token", idToken.ToString().Substring(0, 20));

			success?.Invoke();
		}
		else
		{
			DebugManager.Log("Apple Login Failed.", DebugColor.Login);
		}
	}

	public void UpdateNickname(string nickname)
	{
		var bro = Backend.BMember.UpdateNickname(nickname);

		if (bro.IsSuccess())
		{
			Debug.Log($"nickname changed to {nickname}");
		}

		else
		{
			Debug.Log("nickname change failed.");

			if (bro.GetStatusCode() == "409")
			{
				Debug.Log("nickname already exist.");
			}
		}
	}
}
