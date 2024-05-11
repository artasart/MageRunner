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
}
