using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MEC;
using System.Security.Claims;
using System;
using System.Collections;

public class AppleRevoker : MonoBehaviour
{
	// Apple�� ��ū ��ȿȭ API ��������Ʈ URL
	private string revokeTokenUrl = "https://appleid.apple.com/auth/token";

	// Apple�� Ŭ���̾�Ʈ ���̵�� ��ũ��
	private string clientId = "com.kaleidoscopicmind.magerunner";
	private string clientSecret = "YOUR_CLIENT_SECRET";

	// ��ȿȭ�� ��ū
	private string tokenToRevoke = "TOKEN_TO_REVOKE";

	public void Revoke()
	{
		var token = PlayerPrefs.GetString(Define.APPLEUSERID);
		var authorizationCode = PlayerPrefs.GetString("authorizationCode");

		Debug.Log("token : " + token);
		Debug.Log("authorizationCode" + authorizationCode);

		GameManager.UI.FetchPanel<Panel_Logo>().SetMessage("token : " + token + "\n" + "authorizationCode" + authorizationCode);

		RevokeAppleToken(token, authorizationCode, null);
	}

	public void RevokeAppleToken(string clientSecret, string token, System.Action completionHandler)
	{
		string url = $"https://appleid.apple.com/auth/revoke?client_id={clientId}&client_secret=" + clientSecret + "&token=" + token + "&token_type_hint=refresh_token";
		Util.RunCoroutine(RevokeTokenCoroutine(url, completionHandler), nameof(RevokeTokenCoroutine));
	}

	IEnumerator<float> RevokeTokenCoroutine(string url, System.Action completionHandler)
	{
		// ��û ��� ����
		var header = new Dictionary<string, string>();
		header["Content-Type"] = "application/x-www-form-urlencoded";

		// UnityWebRequest ����
		using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
		{
			// ��û ��� �߰�
			foreach (var entry in header)
			{
				request.SetRequestHeader(entry.Key, entry.Value);
			}

			// ������ �ޱ� ���� ���
			yield return Timing.WaitUntilDone(request.SendWebRequest());

			// ��û�� �����ߴ��� Ȯ��
			if (request.result == UnityWebRequest.Result.Success)
			{
				Debug.Log("���� ��ū ���� ����!");
				completionHandler?.Invoke();
			}
			else
			{
				Debug.LogError("Error: " + request.error);
			}
		}
	}
}