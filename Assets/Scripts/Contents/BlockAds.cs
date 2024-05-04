using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockAds : MonoBehaviour
{
	TMP_Text txtmp_Time;

	CanvasGroup canvasGroup;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0f;
		canvasGroup.blocksRaycasts = false;

		txtmp_Time = Util.FindTMPText(this.gameObject, nameof(txtmp_Time), "00:00");
	}

	public void WatchAd(bool init = true)
	{
		Debug.Log("Watch AD");

		if(init)
		{
			LocalData.gameData.adWatchTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
		}

		Util.RunCoroutine(Co_WatchAD(), nameof(Co_WatchAD) + this.transform.parent.gameObject.GetHashCode(), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_WatchAD()
	{
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;

		LocalData.gameData.isAdWatched = true;

		DateTime adWatchTime = Util.StringToDateTime(LocalData.gameData.adWatchTime);
		DateTime endTime = adWatchTime.AddSeconds(Scene.main.adWaitTime);

		while (DateTime.Now < endTime)
		{
			TimeSpan remainingTime = endTime - DateTime.Now;
			string timeText = string.Format("{0:00}:{1:00}", (int)remainingTime.TotalMinutes, remainingTime.Seconds);
			txtmp_Time.text = timeText;

			yield return Timing.WaitForOneFrame;
		}

		canvasGroup.alpha = 0;
		canvasGroup.blocksRaycasts = false;

		LocalData.gameData.isAdWatched = false;
	}
}