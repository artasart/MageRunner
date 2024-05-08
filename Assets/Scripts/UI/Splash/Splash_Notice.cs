using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Splash_Notice : Splash_Base
{
	Slider slider_Timer;

	protected override void Awake()
	{
		base.Awake();

		slider_Timer = GetUI_Slider(nameof(slider_Timer));
	}

	public void SetTimer()
	{
		slider_Timer.value = 0;
		slider_Timer.maxValue = timeout;

		Util.RunCoroutine(Co_SetTimer(timeout), nameof(Co_SetTimer), CoroutineTag.Content);
	}

	IEnumerator<float> Co_SetTimer(float time)
	{
		float value = 0f;

		while (value < time)
		{
			value += Time.deltaTime;

			slider_Timer.value = value;

			yield return Timing.WaitForOneFrame;
		}

		slider_Timer.value = slider_Timer.maxValue;
	}
}
