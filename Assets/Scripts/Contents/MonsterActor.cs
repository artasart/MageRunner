using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using static Enums;

[RequireComponent(typeof(BoxCollider2D))]
public class MonsterActor : Actor
{
	#region Members

	[Header("Override")]
	[SerializeField] public int score = 100;
	[SerializeField] public int gold = 10;
	[SerializeField] public int exp = 10;

	public float noticeRange = 2;
	public float attackRange = 1;

	Transform hp;

	ParticleSystem particle_SwordTrail;

	#endregion



	#region Initialize

	private void OnEnable()
	{
		hp.GetComponent<TMP_Text>().text = damage.ToString();
	}

	protected override void Awake()
	{
		base.Awake();

		hp = this.transform.Search(nameof(hp));

		particle_SwordTrail = this.transform.Search(nameof(particle_SwordTrail)).GetComponent<ParticleSystem>();

		boxCollider2D = GetComponent<BoxCollider2D>();

		healthOrigin = damage;
	}

	#endregion



	#region Core methods

	#endregion

	

	#region Entity

	public override void Attack()
	{
		animator.SetTrigger("Attack");

		particle_SwordTrail.Play();
	}

	public override void Die()
	{
		if (isDead) return;

		isDead = true;
		damage = 0;

		PoolManager.Spawn("Skull", Vector3.zero + Vector3.up * .5f, Quaternion.identity, this.transform);

		//var coin = PoolManager.Spawn("CoinSpawner", this.transform.position + Vector3.up *.5f, Quaternion.identity);
		//coin.transform.SetParent(this.transform.parent);
		//FindObjectOfType<CoinSpawner>().Spawn(gold);

		animator.SetBool("Die", true);

		this.GetComponent<BoxCollider2D>().enabled = false;
		this.transform.Search("ExecuteTrigger").GetComponent<BoxCollider2D>().enabled = false;

		hp.GetComponent<TMP_Text>().text = 0.ToString();

		Scene.game.AddGameExp();

		GetItem();

		CancelInvoke(nameof(ApplyDamage));

		Invoke(nameof(Refresh), 2f);
	}

	

	public override void Damage(int amount = 0, bool execute = false)
	{
		base.Damage(amount, execute);

		if (execute) amount = damage;

		if (damage <= amount)
		{
			Die();
		}

		else
		{
			damage -= amount;

			hp.GetComponent<TMP_Text>().text = damage.ToString();
		}
	}

	public override void Refresh()
	{
		base.Refresh();

		animator.Rebind();
		damage = healthOrigin;

		hp.GetComponent<TMP_Text>().text = health.ToString();
		boxCollider2D.enabled = true;
		this.transform.Search("ExecuteTrigger").GetComponent<BoxCollider2D>().enabled = true;

		isDead = false;

		CancelInvoke(nameof(ApplyDamage));

		this.GetComponent<EquipmentController>().RefreshEquipment();

		this.GetComponent<RePoolObject>().RePool();
	}

	#endregion



	#region Callback

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (Scene.game.playerActor.isDead) return;

		if (other.CompareTag(Define.PLAYER))
		{
			if (other.gameObject.GetComponent<PlayerActor>().isDead) return;

			if (other.GetComponent<PlayerActor>().health < damage)
			{
				Attack();

				Invoke(nameof(ApplyDamage), .2f);
			}

			else
			{
				Die();

				other.GetComponent<PlayerActor>().Distortion();

				Scene.game.cameraShake.Shake(new CameraNoise.Properties(90f, .05f, 10f, .5f, .125f, .089f, .028f));
			}
		}

		else if (other.CompareTag(Define.COLLECTOR))
		{
			Refresh();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag(Define.PLAYER))
		{
			CancelInvoke(nameof(ApplyDamage));
		}
	}


	private void ApplyDamage()
	{
		if (isDead) return;

		Scene.game.playerActor.Damage(damage);
	}

	#endregion



	#region Util

	public void GetItem()
	{
		SPUM_Prefabs spumPrefabs = GetComponent<SPUM_Prefabs>();

		//GainEquipment(EquipmentType.Hair, spumPrefabs._spriteOBj._hairListString);
		//GainEquipment(EquipmentType.Weapons, spumPrefabs._spriteOBj._weaponListString);
		
		if(UnityEngine.Random.Range(0f, 1f) < 0.01f)
		{
			GainEquipment(EquipmentType.Back, spumPrefabs._spriteOBj._backListString);
			GainEquipment(EquipmentType.Cloth, spumPrefabs._spriteOBj._clothListString);
			GainEquipment(EquipmentType.Armor, spumPrefabs._spriteOBj._armorListString);

			DebugManager.ClearLog("You gained item..!");
		}

		//GainEquipment(EquipmentType.Pant, spumPrefabs._spriteOBj._pantListString);
	}

	public void GainEquipment(EquipmentType type, List<string> test)
	{
		var game = FindObjectOfType<Scene_Game>();

		var itemName = (type == EquipmentType.Cloth || type == EquipmentType.Armor || type == EquipmentType.Pant) ? test[0] : "";

		foreach (var item in test)
		{
			if (string.IsNullOrEmpty(item)) continue;

			if (game.bags.ContainsKey(item))
			{
				game.bags[item]++;
			}

			else
			{
				game.bags.Add(item, 1);
			}
		}

		switch (type)
		{
			case EquipmentType.Cloth:
				game.bags[itemName] /= 3;
				break;
			case EquipmentType.Armor:
				game.bags[itemName] /= 2;
				break;
			case EquipmentType.Pant:
				game.bags[itemName] /= 3;
				break;
			default:
				break;
		}
	}

	public void SetDamageUI()
	{
		damage = LocalData.masterData.inGameLevel[Scene.game.level - 1].monsterDamage;

		hp.GetComponent<TMP_Text>().text = LocalData.masterData.inGameLevel[Scene.game.level - 1].monsterDamage.ToString();
	}

	#endregion
}