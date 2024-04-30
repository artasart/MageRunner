using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
	[Header("Virtual")]
	public int health = 10;
	public int damage = 100;
	public int mana = 100;
	public bool isDead = false;
	public int manaTotal;

	protected int healthOrigin;

	protected Animator animator;
	protected BoxCollider2D boxCollider2D;

	protected virtual void Awake()
	{
		healthOrigin = health;

		animator = this.transform.Search("UnitRoot").GetComponent<Animator>();

		manaTotal = mana;
	}

	public virtual void Attack() { }

	public virtual void Die() { }

	public virtual void Damage(int amount, bool execute) { }

	public virtual void Refresh() { }
}
