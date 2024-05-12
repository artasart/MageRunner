using UnityEngine;
using BackEnd;
using System;

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
			if(callback.IsSuccess())
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
		Debug.Log("Rank Data Insert");

		string rowInDate = string.Empty;

		Backend.GameData.GetMyData(Define.USER_DATA, new Where(), callback =>
		{
			if (!callback.IsSuccess())
			{
				Debug.LogError("Data search failed.");

				return;
			}

			Debug.Log("Data Searched..");

			if (callback.FlattenRows().Count > 0)
			{
				rowInDate = callback.FlattenRows()[0]["inDate"].ToString();
			}

			else
			{
				Debug.Log("No Data");
			}

			Param param = new Param()
			{
				{"highScore", LocalData.gameData.highScore }
			};

			Backend.URank.User.UpdateUserScore(Define.UUID_RANK, Define.USER_DATA, rowInDate, param, callack =>
			{
				if (callback.IsSuccess())
				{
					Debug.Log("Succes..");
				}
				else
				{
					Debug.Log("Failed to insert Game Rank");
				}
			});
		});
	}

	public void GetRankList()
	{
		Debug.Log("Get Rank Data");

		Backend.URank.User.GetRankList(Define.UUID_RANK, 10, callback =>
		{
			if(callback.IsSuccess())
			{
				LitJson.JsonData rankDataJson = callback.FlattenRows();

				if(rankDataJson.Count <= 0)
				{
					Debug.Log("NO Rank Data.");
				}

				else
				{
					Debug.Log("Rank data exist." + rankDataJson.Count);

					for (int i = 0; i < rankDataJson.Count; i++)
					{
						Debug.Log(int.Parse(rankDataJson[i]["rank"].ToString()));
						Debug.Log(int.Parse(rankDataJson[i]["highScore"].ToString()));

						if (rankDataJson[i].ContainsKey("nickname") == true)
						{
							Debug.Log(rankDataJson[i]["nickname"?.ToString()]);
						}
					}
				}
			}

			else
			{
				Debug.Log("No Data");
			}
		});
	}

	public void GetMyRank()
	{
		Backend.URank.User.GetMyRank(Define.UUID_RANK, callback =>
		{
			if (callback.IsSuccess())
			{
				LitJson.JsonData rankDataJson = callback.FlattenRows();

				if (rankDataJson.Count <= 0)
				{
					Debug.Log("I'm not in a rank");
				}

				else
				{
					Debug.Log(int.Parse(rankDataJson[0]["rank"].ToString()));
					Debug.Log(int.Parse(rankDataJson[0]["highScore"].ToString()));

					if (rankDataJson[0].ContainsKey("nickname") == true)
					{
						Debug.Log(rankDataJson[0]["nickname"?.ToString()]);
					}
				}
			}
		});
	}
}
