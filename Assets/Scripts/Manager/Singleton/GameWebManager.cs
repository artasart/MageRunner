using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameWebManager : SingletonManager<GameWebManager>
{
	#region Core

	private IEnumerator<float> Co_PostRequest<T>(string url, string jsonData, Action<T> callback = null, Action<T> error = null, bool useDim = false)
	{
		if (useDim) GameManager.Scene.Dim(true);

		byte[] body = System.Text.Encoding.UTF8.GetBytes(jsonData);

		using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
		{
			request.uploadHandler = new UploadHandlerRaw(body);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			yield return Timing.WaitUntilDone(request.SendWebRequest());

			if (request.result != UnityWebRequest.Result.Success)
			{
				T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
				error?.Invoke(result);
			}
			else
			{
				T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
				callback?.Invoke(result);
			}
		}

		if (useDim) GameManager.Scene.Dim(false);
	}

	private IEnumerator<float> Co_GetRequest<T>(string url, Action<T> callback = null, bool useDim = false)
	{
		if (useDim) GameManager.Scene.Dim(true);

		using (UnityWebRequest request = UnityWebRequest.Get(url))
		{
			yield return Timing.WaitUntilDone(request.SendWebRequest());

			if (request.result != UnityWebRequest.Result.Success)
			{
				var result = JsonUtility.FromJson<DefaultRes>(request.downloadHandler.text);

				if (result != null) { DebugManager.Log(result.message); }
				else DebugManager.Log("Network Error");
			}
			else
			{
				T result = JsonUtility.FromJson<T>(request.downloadHandler.text);

				callback?.Invoke(result);
			}
		}

		if (useDim) GameManager.Scene.Dim(false);
	}

	#endregion
}