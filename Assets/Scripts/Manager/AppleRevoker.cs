using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MEC;
using System;

public class AppleRevoker : MonoBehaviour
{
	private string revokeTokenUrl = "https://appleid.apple.com/auth/token";
	private string clientId = "com.kaleidoscopicmind.magerunner";
	private string clientSecret = "YOUR_CLIENT_SECRET";
	private string tokenToRevoke = "TOKEN_TO_REVOKE";

	public void Revoke()
	{
		var token = PlayerPrefs.GetString("token");
		var authorizationCode = PlayerPrefs.GetString("authorizationCode");

		GameManager.UI.PopPopup(true);

		RevokeAppleToken(token, authorizationCode, ()=>
		{
			GameManager.UI.StackPopup<Popup_Basic>(true);
			GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.Confrim, $"Application need to be restarted.", "Notice",
			() =>
			{
				Application.Quit();
			},

			() =>
			{

			});

		},
		()=>
		{
			GameManager.Scene.ShowToastAndDisappear("Withdraw account failed.");
		});
	}

	public void RevokeAppleToken(string clientSecret, string token, System.Action completionHandler , Action error)
	{
		string url = $"https://appleid.apple.com/auth/revoke?client_id={clientId}&client_secret=" + clientSecret + "&token=" + token + "&token_type_hint=refresh_token";

		Util.RunCoroutine(RevokeTokenCoroutine(url, completionHandler, error), nameof(RevokeTokenCoroutine));
	}

	IEnumerator<float> RevokeTokenCoroutine(string url, System.Action completionHandler, Action error)
	{
		var header = new Dictionary<string, string>();

		header["Content-Type"] = "application/x-www-form-urlencoded";

		using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
		{
			foreach (var entry in header)
			{
				request.SetRequestHeader(entry.Key, entry.Value);
			}

			yield return Timing.WaitUntilDone(request.SendWebRequest());

			if (request.result == UnityWebRequest.Result.Success)
			{
				completionHandler?.Invoke();
			}
			else
			{
				error?.Invoke();
			}
		}
	}
}