using UnityEngine;
using BackEnd;
using System;
using System.Collections.Generic;
using static Enums;

public class GameBackendManager : SingletonManager<GameBackendManager>
{
	public override void OnDestroy()
	{
		base.OnDestroy();

		SetGameData();
	}

	private void Awake()
	{
		InitializeBackend();
	}

	private void InitializeBackend()
	{
		var backendResult = Backend.Initialize(true);

		if (backendResult.IsSuccess())
		{
			DebugManager.Log("Initialized successfully: " + backendResult);
		}
		else
		{
			Debug.LogError("Initialization failed: " + backendResult);
		}
	}

	public void Login(string idToken, Action success)
	{
		var backendResult = Backend.BMember.AuthorizeFederation(idToken, FederationType.Apple, "Apple");

		if (backendResult.IsSuccess())
		{
			GameManager.Scene.Dim(false);
			PlayerPrefs.SetString("token", idToken);
			success?.Invoke();
		}
		else if (backendResult.GetStatusCode() == Define.STATUSCODE_WITHDRAW)
		{
			HandleWithdraw();
		}
	}

	public void QuickLogin(Action success)
	{
		var backendResult = Backend.BMember.AuthorizeFederation(PlayerPrefs.GetString("token"), FederationType.Apple, "Apple");

		if (backendResult.IsSuccess())
		{
			success?.Invoke();
		}
		else if (backendResult.GetStatusCode() == Define.STATUSCODE_WITHDRAW)
		{
			HandleWithdraw();
		}
	}

	public void SetNickname(string nickname, Action success, Action fail)
	{
		var backendResult = Backend.BMember.UpdateNickname(nickname);

		if (backendResult.IsSuccess())
		{
			DebugManager.Log($"Nickname changed to {nickname}");
			success?.Invoke();
		}
		else if (backendResult.GetStatusCode() == "409")
		{
			DebugManager.Log("Nickname change failed: Nickname already exists.");
			fail?.Invoke();
		}
		else
		{
			DebugManager.Log("Nickname change failed.");
		}
	}

	public string GetNickname()
	{
		var backendResult = Backend.BMember.GetUserInfo();
		var nickname = backendResult.GetReturnValuetoJSON()["row"]["nickname"].ToString();
		return nickname;
	}

	public void WithdrawAccount()
	{
		Backend.BMember.WithdrawAccount();
	}

	private void HandleWithdraw()
	{
		GameManager.Scene.Dim(false);
		GameManager.UI.StackPopup<Popup_Basic>(true);
		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.Confrim,
			$"This account is currently being withdrawn.\nplease try again later.\n\n" +
			$"<size=25><color=#323232>Processing usually takes within an hour</size></color>", "Notice");
	}


	public void SetGameData(Action success = null)
	{
		if (GameManager.UI.IsLogoScene()) return;

		Param param = new Param()
		{
			{ "nickname", LocalData.gameData.nickname},
			{ "level", LocalData.gameData.level},
			{ "exp", LocalData.gameData.exp},
			{ "gold", LocalData.gameData.gold},
			{ "highScore", LocalData.gameData.highScore},
			{ "energy", LocalData.gameData.energy},
			{ "energyTotal", LocalData.gameData.energyTotal},
			{ "isPremium", LocalData.gameData.isPremium},
			{ "runnerTag", LocalData.gameData.runnerTag},
			{ "damage", LocalData.gameData.damage},
			{ "mana", LocalData.gameData.mana},
			{ "speed", LocalData.gameData.speed},
			{ "damageLevel", LocalData.gameData.damageLevel},
			{ "manaLevel", LocalData.gameData.manaLevel},
			{ "speedLevel", LocalData.gameData.speedLevel},
			{ "equipment", LocalData.gameData.equipment},
			{ "bags", LocalData.gameData.bags},
			{ "actorSkills", LocalData.gameData.actorSkills},
		};

		Backend.GameData.Insert(Define.USER_DATA, param, callback =>
		{
			if (callback.IsSuccess())
			{
				DebugManager.Log("Game Data insert Completed.");

				success?.Invoke();
			}

			else
			{
				DebugManager.Log("Game Data insert Failed.");
			}
		});
	}

	public void GetGameData()
	{
		var bro = Backend.GameData.GetMyData("USER_DATA", new Where());

		if (bro.IsSuccess())
		{
			LitJson.JsonData gameDataJson = bro.FlattenRows();

			if (gameDataJson.Count <= 0) return;

			LocalData.gameData.nickname = gameDataJson[0]["nickname"].ToString();
			LocalData.gameData.level = int.Parse(gameDataJson[0]["level"].ToString());
			LocalData.gameData.exp = int.Parse(gameDataJson[0]["exp"].ToString());
			LocalData.gameData.gold = int.Parse(gameDataJson[0]["gold"].ToString());
			LocalData.gameData.highScore = int.Parse(gameDataJson[0]["highScore"].ToString());
			LocalData.gameData.energy = int.Parse(gameDataJson[0]["energy"].ToString());
			LocalData.gameData.energyTotal = int.Parse(gameDataJson[0]["energyTotal"].ToString());
			LocalData.gameData.isPremium = bool.Parse(gameDataJson[0]["isPremium"].ToString());
			LocalData.gameData.runnerTag = int.Parse(gameDataJson[0]["runnerTag"].ToString());
			LocalData.gameData.damage = int.Parse(gameDataJson[0]["damage"].ToString());
			LocalData.gameData.mana = int.Parse(gameDataJson[0]["mana"].ToString());
			LocalData.gameData.speed = float.Parse(gameDataJson[0]["speed"].ToString());
			LocalData.gameData.damageLevel = int.Parse(gameDataJson[0]["damageLevel"].ToString());
			LocalData.gameData.manaLevel = int.Parse(gameDataJson[0]["manaLevel"].ToString());
			LocalData.gameData.speedLevel = int.Parse(gameDataJson[0]["speedLevel"].ToString());
		}

		else
		{
			Debug.LogError("Failed. : " + bro);
		}
	}

	public void SetRankData()
	{
		DebugManager.ClearLog("Rank Data Insert");

		string rowInDate = string.Empty;

		var bro = Backend.GameData.GetMyData(Define.USER_DATA, new Where());

		if (bro.IsSuccess())
		{
			if (bro.FlattenRows().Count > 0)
			{
				rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
			}

			Param param = new Param()
			{
				{ "highScore", LocalData.gameData.highScore },
				{ "level", LocalData.gameData.level }
			};

			var score = Backend.URank.User.UpdateUserScore(Define.UUID_RANK, Define.USER_DATA, rowInDate, param);

			if (score.IsSuccess())
			{
				DebugManager.Log("Success");
			}

			else
			{
				DebugManager.Log("Failed");
			}
		}

		else
		{
			DebugManager.Log("Error");
		}
	}

	public void GetRankList()
	{
		var bro = Backend.URank.User.GetRankList(Define.UUID_RANK, 10);

		if (bro.IsSuccess())
		{
			LitJson.JsonData rankDataJson = bro.FlattenRows();

			GameManager.UI.FetchPopup<Popup_Rank>().SetEmpty(rankDataJson.Count <= 0);

			if (rankDataJson.Count <= 0) return;

			var rankDatas = new List<RankData>();
			for (int i = 0; i < rankDataJson.Count; i++)
			{
				var rankData = new RankData();

				rankData.nickname = rankDataJson[i]["nickname"].ToString();
				rankData.rank = int.Parse(rankDataJson[i]["rank"].ToString());
				rankData.score = int.Parse(rankDataJson[i]["score"].ToString());
				rankData.level = int.Parse(rankDataJson[i]["level"].ToString());
				rankData.isMine = rankData.nickname == GameManager.Backend.GetNickname();

				rankDatas.Add(rankData);
			}

			GameManager.UI.FetchPopup<Popup_Rank>().Refresh(rankDatas);
		}
	}

	public void GetMyRank()
	{
		var bro = Backend.URank.User.GetMyRank(Define.UUID_RANK);

		if (bro.IsSuccess())
		{
			LitJson.JsonData rankDataJson = bro.FlattenRows();

			if (rankDataJson.Count <= 0) return;
		}
	}



	#region Login

	public void SignUp(string id, string password)
	{
		DebugManager.Log("Requset Sign in.");

		var bro = Backend.BMember.CustomSignUp(id, password);

		if (bro.IsSuccess())
		{
			DebugManager.Log("Success " + bro);

			Login(id, password);
		}
		else
		{
			Debug.LogError("Failed : " + bro);
		}
	}

	public void Login(string id, string password)
	{
		var bro = Backend.BMember.CustomLogin(id, password);

		if (bro.IsSuccess())
		{
			DebugManager.Log("Success : " + bro);
		}
		else
		{
			Debug.LogError("Failed : " + bro);
		}
	}

	#endregion


	#region Guest

	public void GuestLogin()
	{
		GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(ModalType.ConfirmCancel, "The game data will be saved locally.\nDo you want to continue?", "Notice", () =>
		{
			BackendReturnObject bro = Backend.BMember.GuestLogin("Sign in with Guest");

			if (bro.IsSuccess())
			{
				DebugManager.Log("Guest Success.");

				PlayerPrefs.SetString(Define.LOGINTYPE, LoginType.Guest.ToString());

				GameManager.Scene.LoadScene(SceneName.Main);
			}

			else
			{
				if (bro.GetStatusCode() == Define.STATUSCODE_WITHDRAW)
				{
					GameManager.Scene.Dim(true);

					Invoke(nameof(Handle), .5f);
				}

				else
				{
					DebugManager.Log(bro.GetStatusCode());

					GameManager.Scene.ShowToastAndDisappear("Guest login failed.");
				}
			}
		});
	}

	private void Handle()
	{
		DebugManager.Log("Guest Login Failed");

		HandleWithdraw();

		GameManager.Scene.Dim(false);
	}

	public void DeleteGuestInfo()
	{
		Backend.BMember.DeleteGuestInfo();
	}

	#endregion
}
