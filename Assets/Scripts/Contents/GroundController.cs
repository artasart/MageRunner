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

    public void MoveGround()
    {
		foreach (var item in grounds)
		{
			item.SetMoveSpeed(5f);
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
}
