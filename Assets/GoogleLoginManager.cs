using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleLoginManager : MonoBehaviour
{
	public void StartGoogleLogin()
	{
		TheBackend.ToolKit.GoogleLogin.iOS.GoogleLogin(GoogleLoginCallback);
	}

	private void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
	{
		if (isSuccess == false)
		{
			Debug.LogError(errorMessage);
			return;
		}

		Debug.Log("���� ��ū : " + token);
		var bro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);
		Debug.Log("�䵥���̼� �α��� ��� : " + bro);
	}

	public void SignOutGoogleLogin()
	{
		TheBackend.ToolKit.GoogleLogin.iOS.GoogleSignOut(GoogleSignOutCallback);
	}

	private void GoogleSignOutCallback(bool isSuccess, string error)
	{
		if (isSuccess == false)
		{
			Debug.Log("���� �α׾ƿ� ���� ���� �߻� : " + error);
		}
		else
		{
			Debug.Log("�α׾ƿ� ����");
		}
	}

}