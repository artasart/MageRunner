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

	public float coinProbability = .75f;
	float coinProbabilityOrigin;

	public float skillCardProbability = .2f;
	float skillCardProbabilityOrigin;

	public float moveSpeed = 5f;
	public int currentMoveMultiplier = 1;

    List<Ground> grounds = new List<Ground>();

    private void Awake()
	{
		grounds = FindObjectsOfType<Ground>().ToList();

		monsterProbabilityOrigin = monsterProbability;
		groundProbabilityOrigin = groundProbability;
		coinProbabilityOrigin = coinProbability;
		skillCardProbabilityOrigin = skillCardProbability;
	}

	public float GetMoveSpeed()
	{
		return moveSpeed;
	}

	public void SetMoveMultiplier(int multiplier)
	{
		currentMoveMultiplier = multiplier;

		foreach (var item in grounds)
		{
			item.moveSpeedMultiplier = multiplier;
		}
	}

	public void MoveGround(float moveSpeed = 4f)
    {
		this.moveSpeed = moveSpeed;

		foreach (var item in grounds)
		{
			item.moveSpeed = moveSpeed;
			item.Move();
		}
	}

	public void AddSpeed()
	{
		moveSpeed = Mathf.Clamp(moveSpeed += .1f, 0f, 10);

		foreach (var item in grounds)
		{
			item.moveSpeed = moveSpeed;
		}
	}

	public void StopGround()
	{
		foreach (var item in grounds)
		{
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
		coinProbability = coinProbabilityOrigin;
		skillCardProbability = skillCardProbabilityOrigin;
	}
}
