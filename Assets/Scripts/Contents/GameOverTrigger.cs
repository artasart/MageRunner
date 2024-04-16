using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverTrigger : LevelElement
{
	protected override void Awake()
	{
		base.Awake();

		type = PropsType.Trigger;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag(Define.PLAYER))
		{
			foreach (var item in FindObjectsOfType<Ground>())
			{
				item.Stop();
			}

			other.GetComponent<PlayerActor>().Die();
		}
	}
}
