using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	[Header("Ground Speed")]
	public float moveSpeed = 5f;
	public float moveSpeedOrigin = 5f;
	public int moveSpeedMultiplier = 1;
	public float heightProbability = .75f;

	[Header("Element Probability")]
	public float groundProbability = .1f;

	float groundProbabilityOrigin;

    List<Ground> grounds = new List<Ground>();

    private void Awake()
	{
		for (int i = 0; i < this.transform.childCount; i++)
		{
			grounds.Add(this.transform.GetChild(i).GetComponent<Ground>());
		}

		groundProbabilityOrigin = groundProbability;

		moveSpeedOrigin = moveSpeed;
	}

	public void MoveGround()
    {
		foreach (var item in grounds)
		{
			item.Move();
		}
	}

	public void StopGround()
	{
		foreach (var item in grounds)
		{
			item.Stop();
		}
	}

	public void Refresh()
	{ 
		groundProbability = groundProbabilityOrigin;

		foreach (var item in grounds)
		{
			item.Refresh();
		}

		moveSpeed = moveSpeedOrigin;
	}
}
