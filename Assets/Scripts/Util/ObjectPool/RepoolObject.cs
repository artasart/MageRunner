using System.Collections;
using UnityEngine;
using MEC;
using System.Collections.Generic;

public class RePoolObject : MonoBehaviour
{
	public float repoolTime = 0f;
	public bool isRepoolAfterTime = false;

	private void OnEnable()
	{
		if (!isRepoolAfterTime) return;

		Timing.RunCoroutine(Co_RepoolObject(), $"{nameof(Co_RepoolObject)}_{GetHashCode()}");
	}

	private void OnDisable()
	{
		Timing.KillCoroutines($"{nameof(Co_RepoolObject)}_{GetHashCode()}");
	}

	private IEnumerator<float> Co_RepoolObject()
	{
		yield return Timing.WaitForSeconds(repoolTime);

		RePool();
	}

	public void SetRepoolTime(float _repoolTime)
	{
		repoolTime = _repoolTime;
	}

	public void RePool() => PoolManager.RePool(this.gameObject);
}