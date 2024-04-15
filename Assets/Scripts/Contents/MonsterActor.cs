using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class MonsterActor : LevelElement
{
	#region Members

	[SerializeField] int score = 100;
	[SerializeField] int coint = 10;
	[SerializeField] int health = 100;
	[SerializeField] int attack = 100;

	public float noticeRange = 2;
	public float attackRange = 1;

	Animator animator;
	CapsuleCollider2D capsuleCollider2D;

	public event Action<int, int> OnMonsterDie;

	bool isDead = false;

	#endregion



	#region Initialize

	private void Awake()
	{
		type = PropsType.Coin;

		capsuleCollider2D = GetComponent<CapsuleCollider2D>();
		capsuleCollider2D.isTrigger = true;
		capsuleCollider2D.offset = new Vector2(0, 0.325f);
		capsuleCollider2D.size = new Vector2(0.5f, 0.65f);
	}

	void Start()
	{
		animator = GetComponentInChildren<Animator>();

		OnMonsterDie += FindObjectOfType<Scene_Game>().GainResource;
	}

	#endregion



	#region Core methods

	public void WatchTarget(Transform player) => Util.RunCoroutine(Co_WatchTarget(player), nameof(Co_WatchTarget) + this.GetHashCode());

	private IEnumerator<float> Co_WatchTarget(Transform player)
	{
		while (true)
		{
			Debug.Log(player.GetComponent<Rigidbody2D>().velocity.x);

			if (player.GetComponent<Rigidbody2D>().velocity.x <= 3f)
			{
				attackRange = .75f;
			}

			else attackRange = 1f;

			if (Vector3.Distance(this.gameObject.transform.position, player.position) < attackRange)
			{
				Attack();

				if (player.GetComponent<PlayerActor>().health <= attack)
				{
					yield return Timing.WaitForSeconds(.15f);

					if (isDead) yield break;

					player.GetComponent<PlayerActor>().Damage(attack);

					//player.GetComponent<PlayerActor>().Die();

					yield break;
				}

				else
				{
					Debug.Log("Damge Player : " + attack);
				}
			}

			Debug.Log("I am watching Player");

			yield return Timing.WaitForOneFrame;
		}
	}

	public void StopTarget() => Util.KillCoroutine(nameof(Co_WatchTarget) + this.GetHashCode());

	#endregion



	#region Entity

	private void Attack()
	{
		animator.SetTrigger("Attack");
	}

	private void Die()
	{
		animator.SetBool("Die", true);

		OnMonsterDie?.Invoke(score, coint);
	}

	private void Damage(int amount)
	{
		if (health <= amount)
		{
			health = 0;

			//flash

			animator.SetBool("Die", true);

			FindObjectOfType<Scene_Game>().GainResource(score, coint);

			isDead = true;
		}
	}

	#endregion



	#region Callback

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(Define.PLAYER))
		{
			Damage(other.gameObject.GetComponent<PlayerActor>().health);

			other.gameObject.GetComponent<PlayerActor>().Damage(attack);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}

	#endregion
}