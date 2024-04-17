using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	Transform hp;

	public event Action<int, int> OnMonsterDie;

	bool isDead = false;

	public int healhtOrigin;

	Rigidbody2D rgbd2d;

	#endregion



	#region Initialize

	protected override void Awake()
	{
		capsuleCollider2D = GetComponent<CapsuleCollider2D>();
		capsuleCollider2D.isTrigger = true;
		capsuleCollider2D.offset = new Vector2(0, 0.325f);
		capsuleCollider2D.size = new Vector2(0.5f, 0.65f);

		hp = this.transform.Search(nameof(hp));
		hp.GetComponent<TMP_Text>().text = health.ToString();

		healhtOrigin = health;

		rgbd2d = GetComponent<Rigidbody2D>();
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
			attackRange = 1.25f;

			if (Vector3.Distance(this.gameObject.transform.position, player.position) < attackRange &&
				Mathf.Abs(this.gameObject.transform.position.y - player.position.y) <= .5f)
			{
				Attack();

				if (player.GetComponent<PlayerActor>().health <= attack)
				{
					yield return Timing.WaitForSeconds(.15f);

					if (isDead) yield break;

					player.GetComponent<PlayerActor>().Damage(attack);

					yield break;
				}
			}

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

	public void Damage(int amount = 0, bool execute = false)
	{
		if (execute) amount = health;

		if (health <= amount)
		{
			health = 0;

			animator.SetBool("Die", true);

			//flash method goes here...

			this.GetComponent<CapsuleCollider2D>().enabled = false;
			this.GetComponent<BoxCollider2D>().enabled = false;

			hp.GetComponent<TMP_Text>().text = 0.ToString();

			FindObjectOfType<Scene_Game>().GainResource(score, coint);

			isDead = true;
		}
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.M))
		{
			DieFar();
		}
	}

	public void DieFar()
	{
		var player = FindObjectOfType<PlayerActor>().gameObject;

		Vector2 pushDirection = (player.transform.position - transform.position).normalized;

		rgbd2d.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
	}

	public float pushForce = 10f;


	public void Refresh()
	{
		animator.Rebind();
		health = healhtOrigin;

		hp.GetComponent<TMP_Text>().text = health.ToString();

		this.GetComponent<CapsuleCollider2D>().enabled = true;
		this.GetComponent<BoxCollider2D>().enabled = true;
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