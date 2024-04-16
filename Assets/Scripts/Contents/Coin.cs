using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Coin : LevelElement
{
	public int amount = 100;

	protected override void Awake()
	{
		base.Awake();

		type = PropsType.Coin;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Scene_Game game = FindObjectOfType<Scene_Game>();

			game.AddCoin(amount);

			Destroy(gameObject);

			// playsound
		}
	}
}