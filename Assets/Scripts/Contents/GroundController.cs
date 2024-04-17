using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    List<Ground> grounds = new List<Ground>();

    private void Awake()
    {
        grounds = FindObjectsOfType<Ground>().ToList();
	}

	public float GetMoveSpeed()
	{
		return grounds[0].moveSpeed;
	}

	public float GetProbability()
	{
		return grounds[0].probability;
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
}
