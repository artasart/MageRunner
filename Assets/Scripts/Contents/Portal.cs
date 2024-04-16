using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : LevelElement
{
	protected override void Awake()
	{
		base.Awake();

		type = PropsType.Portal;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(Define.PLAYER))
		{
			FindObjectOfType<PlayerActor>().Stop();
		}
	}
}
