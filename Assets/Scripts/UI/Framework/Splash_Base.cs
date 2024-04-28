using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash_Base : UI_Base
{
	protected bool isInitialized = false;

	public float timeout = 1f;

	protected Action endAction;

	protected virtual void OnEnable()
	{
		if(isInitialized)
		{
			Util.RunCoroutine(Co_DisableAfterSeconds(timeout), nameof(Co_DisableAfterSeconds) + this.GetHashCode(), CoroutineTag.UI);
		}

		isInitialized = true;
	}

	private IEnumerator<float> Co_DisableAfterSeconds(float _timeout)
	{
		while (_timeout > 0)
		{
			yield return Timing.WaitForSeconds(1);

			_timeout--;
		}

		GameManager.UI.PopSplash();

		endAction?.Invoke();
	}

	public void SetEndAction(Action _action)
	{
		endAction = _action;
	}
}
