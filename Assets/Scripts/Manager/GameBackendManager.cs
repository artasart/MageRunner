using UnityEngine;
using BackEnd;
using System;
using System.Collections.Generic;

public class GameBackendManager : SingletonManager<GameBackendManager>
{
	private void Awake()
	{
		InitializeBackend();
	}

	private void InitializeBackend()
	{
		var backendResult = Backend.Initialize(true);

		if (backendResult.IsSuccess())
		{
			Debug.Log("Initialized successfully: " + backendResult);
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
			Debug.Log($"Nickname changed to {nickname}");
			success?.Invoke();
		}
		else if (backendResult.GetStatusCode() == "409")
		{
			Debug.Log("Nickname change failed: Nickname already exists.");
			fail?.Invoke();
		}
		else
		{
			Debug.Log("Nickname change failed.");
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


	public void GameDataInsert(Action success = null)
	{
		Debug.Log("GameData Insert");

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
		};

		Backend.GameData.Insert(Define.USER_DATA, param, callback =>
		{
			if (callback.IsSuccess())
			{
				Debug.Log("Game Data insert Completed.");

				success?.Invoke();
			}

			else
			{
				Debug.Log("Game Data insert Failed.");
			}
		});
	}

	public void RankDataInsert()
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
				Debug.Log("Success");
			}

			else
			{
				Debug.Log("Failed");
			}
		}

		else
		{
			Debug.Log("Error");
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

				Debug.Log(int.Parse(rankDataJson[i]["rank"].ToString()));
				Debug.Log(int.Parse(rankDataJson[i]["score"].ToString()));
				Debug.Log(int.Parse(rankDataJson[i]["level"].ToString()));
				Debug.Log(rankDataJson[i]["nickname"?.ToString()]);

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

			if (rankDataJson.Count <= 0)
			{
				Debug.Log("I'm not in a rank");
			}

			else
			{
				if (rankDataJson[0].ContainsKey("nickname") == true)
				{
					Debug.Log(rankDataJson[0]["nickname"?.ToString()]);
				}
			}
		}
	}



	#region Login

	public void SignUp(string id, string password)
	{
		Debug.Log("회원가입을 요청합니다.");

		var bro = Backend.BMember.CustomSignUp(id, password);

		if (bro.IsSuccess())
		{
			Debug.Log("회원가입에 성공했습니다. : " + bro);

			Login(id, password);
		}
		else
		{
			Debug.LogError("회원가입에 실패했습니다. : " + bro);
		}
	}

	public void Login(string id, string password)
	{
		var bro = Backend.BMember.CustomLogin(id, password);

		if (bro.IsSuccess())
		{
			Debug.Log("로그인에 성공했습니다. : " + bro);
		}
		else
		{
			Debug.LogError("로그인에 실패했습니다. : " + bro);
		}
	}

	#endregion
}
