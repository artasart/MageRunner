using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Coin : LevelElement
{
	public int amount = 1;

	protected override void Awake()
	{
		base.Awake();

		type = PropsType.Coin;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(Define.PLAYER))
		{
			Scene_Game game = FindObjectOfType<Scene_Game>();

			game.AddGold(amount);

			this.GetComponent<RePoolObject>().RePool();
		}

		else if (other.CompareTag(Define.COLLECTOR))
		{
			this.GetComponent<RePoolObject>().RePool();
		}
	}
}