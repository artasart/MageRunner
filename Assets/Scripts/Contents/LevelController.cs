using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public bool generateMonster = false;

	public float groundProbability = .1f;
	float groundProbabilityOrigin;

	public float monsterProbability = .1f;
	float monsterProbabilityOrigin;

    List<Ground> grounds = new List<Ground>();

    private void Awake()
	{
		grounds = FindObjectsOfType<Ground>().ToList();

		monsterProbabilityOrigin = monsterProbability;
		groundProbabilityOrigin = groundProbability;
	}

	public float GetMoveSpeed()
	{
		return grounds[0].moveSpeed;
	}

	public float GetProbability()
	{
		return groundProbability;
	}

	public void SetMoveMultiplier(float multiplier)
	{
		foreach (var item in grounds)
		{
			item.moveSpeedMultiplier = multiplier;
		}
	}

	public void MoveGround(float moveSpeed = 5f)
    {
		foreach (var item in grounds)
		{
			item.SetMoveSpeed(moveSpeed);
			item.Move();
		}
	}

	public void StopGround()
	{
		foreach (var item in grounds)
		{
			item.SetMoveSpeed(0f);
			item.Stop();
		}
	}

	public void AddProbability(float amount)
	{
		foreach (var item in grounds)
		{
			item.SetProbability(amount);
		}
	}

	public void Refresh()
	{ 
		monsterProbability = monsterProbabilityOrigin;
		groundProbability = groundProbabilityOrigin;
	}
}
