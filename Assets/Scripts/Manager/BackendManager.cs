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

			PlayerPrefs.SetString("token", idToken.ToString());

			success?.Invoke();
		}
		else
		{
			DebugManager.Log("Apple Login Failed.", DebugColor.Login);
		}
	}

	public void LoginASAP(Action success)
	{
		BackendReturnObject bro = Backend.BMember.AuthorizeFederation(PlayerPrefs.GetString("token"), FederationType.Apple, "Apple");

		if (bro.IsSuccess())
		{
			DebugManager.Log("Apple Login Success.", DebugColor.Login);

			GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Success.");
			success?.Invoke();
		}
		else
		{
			DebugManager.Log("Apple Login Failed.", DebugColor.Login);
			GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("Failed.");
		}
	}

	public void SetNickname(string nickname, Action success, Action fail)
	{
		var bro = Backend.BMember.UpdateNickname(nickname);

		if (bro.IsSuccess())
		{
			Debug.Log($"nickname changed to {nickname}");

			success?.Invoke();
		}

		else
		{
			Debug.Log("nickname change failed.");

			if (bro.GetStatusCode() == "409")
			{
				Debug.Log("nickname already exist.");

				fail.Invoke();
			}
		}
	}

	public string GetNickname()
	{
		BackendReturnObject bro = Backend.BMember.GetUserInfo();
		string nickname = bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();

		return nickname;
	}

	public void WithDrawAccount()
	{
		Backend.BMember.WithdrawAccount();
	}
}
