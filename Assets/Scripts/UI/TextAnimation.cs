using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using static EasingFunction;

public class TextAnimation : MonoBehaviour
{
	TMP_Text txtmp;
	public CanvasGroup canvasGroup;

	public float start = 1f; // ���� ���İ�
	public float end = .4f; // �� ���İ�

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_PingPong) + this.GetHashCode());
	}

	private void Awake()
	{
		txtmp = GetComponent<TMP_Text>();
		canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
	}

	public void SetAlphaRange(int start, int end)
	{
		this.start = start;
		this.end = end;
	}

	public void StartPingPong(float pingpongSpeed = 1f)
	{
		Debug.Log("Start Pingpong");

		Util.RunCoroutine(Co_PingPong(pingpongSpeed), nameof(Co_PingPong) + this.GetHashCode());
	}

	private IEnumerator<float> Co_PingPong(float pingpongSpeed = 1f)
	{
		while (true)
		{
			float alpha = Mathf.PingPong(Time.time * pingpongSpeed, 1f);

			alpha = Mathf.Lerp(start, end, alpha);

			canvasGroup.alpha = alpha;

			yield return Timing.WaitForOneFrame;
		}
	}

	public void StopPingPong()
	{
		Debug.Log("Stop Pingpong");

		Util.KillCoroutine(nameof(Co_PingPong) + this.GetHashCode());

		Util.FadeCanvasGroup(canvasGroup, 1f);
	}
}