using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : LevelElement
{
	protected override void Awake()
	{
		base.Awake();

		type = PropsType.Obstacle;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(Define.PLAYER))
		{
			other.GetComponent<PlayerActor>().Execute();
		}
	}
}