using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	[Header("Ground Speed")]
	public float moveSpeed = 5f;
	public int moveSpeedMultiplier = 1;

	[Header("Element Probability")]
	public float groundProbability = .1f;
	public float monsterProbability = .1f;
	public float coinProbability = .75f;
	public float skillCardProbability = .2f;

	float groundProbabilityOrigin;
	float monsterProbabilityOrigin;
	float coinProbabilityOrigin;
	float skillCardProbabilityOrigin;

    List<Ground> grounds = new List<Ground>();

    private void Awake()
	{
		for (int i = 0; i < this.transform.childCount; i++)
		{
			grounds.Add(this.transform.GetChild(i).GetComponent<Ground>());
		}

		monsterProbabilityOrigin = monsterProbability;
		groundProbabilityOrigin = groundProbability;
		coinProbabilityOrigin = coinProbability;
		skillCardProbabilityOrigin = skillCardProbability;
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
		monsterProbability = monsterProbabilityOrigin;
		groundProbability = groundProbabilityOrigin;
		coinProbability = coinProbabilityOrigin;
		skillCardProbability = skillCardProbabilityOrigin;

		foreach (var item in grounds)
		{
			item.Refresh();
		}
	}
}
