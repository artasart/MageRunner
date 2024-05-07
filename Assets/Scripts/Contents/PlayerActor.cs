using DG.Tweening;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enums;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerActor : Actor
{
	#region Members

	[Header("Movement")]
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float jumpForce = 5f;
	[SerializeField] private float slideDuration = 1f;
	[SerializeField] private float slideSpeedMultiplier = 2f;
	[SerializeField] private float doubleJumpForceRatio = 0.8f;
	[SerializeField] private int maxJumps = 2;

	[Header("Jump and Slide")]
	[SerializeField] private bool isJumping = false;
	[SerializeField] float jumpValue = 10f;
	[SerializeField] public bool isGrounded = true;
	[SerializeField] public bool isSliding = false;
	[SerializeField] float slideTimer = 0f;
	[SerializeField] int remainingJumps;

	[Header("Entity")]
	[HideInInspector] public int attackPoint = 10;
	[HideInInspector] public float moveSpeedOrigin = 5;


	[Header("Die settings")]
	public float dieVelocity = .75f;
	public float pushForce = -2f;

	bool isDoubleJump = false;
	public bool isFlyMode = false;
	public float damageRadius = 10f;

	Transform hp;
	Rigidbody2D rgbd2d;

	ParticleSystem particle_ElectricMode;
	ParticleSystem particle_Distortion;

	#endregion



	#region Initialize

	protected override void Awake()
	{
		base.Awake();

		rgbd2d = GetComponent<Rigidbody2D>();

		hp = this.transform.Search(nameof(hp));
		hp.GetComponent<TMP_Text>().text = health.ToString();

		remainingJumps = maxJumps;

		boxCollider2D = this.GetComponent<BoxCollider2D>();
	}

	private void Start()
	{
		moveSpeedOrigin = moveSpeed;

		particle_Distortion = Util.GetParticle(this.gameObject, nameof(particle_Distortion));
		particle_ElectricMode = Util.GetParticle(this.gameObject, nameof(particle_ElectricMode));
	}

	#endregion



	#region Movement

	void Update()
	{
		if (Scene.game.gameState == GameState.Paused || isDead) return;

		HandleJumpInput();
		HandleSlideInput();
	}

	public void HandleJumpInput()
	{
		if (rgbd2d.velocity.y < 0) isGrounded = false;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (CanJump())
			{
				PerformJump();
				HandleJumpCount();
			}
		}
	}

	public void HandleSlideInput()
	{
		if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isSliding)
			StartSlide();

		if (isSliding && slideTimer > 0)
		{
			slideTimer -= Time.deltaTime;

			if (slideTimer <= 0)
				EndSlide();
		}
	}

	public void Slide()
	{
		if (isGrounded && !isSliding) StartSlide();

		if (isSliding && slideTimer > 0)
		{
			slideTimer -= Time.deltaTime;

			if (slideTimer <= 0)
				EndSlide();
		};
	}

	private void StartSlide()
	{
		isSliding = true;
		slideTimer = slideDuration;
		moveSpeed *= slideSpeedMultiplier;

		animator.SetBool("Slide", true);
	}

	private void EndSlide()
	{
		isSliding = false;
		moveSpeed /= slideSpeedMultiplier;

		animator.SetBool("Slide", false);
	}


	public void Jump()
	{
		if (!CanJump() || isDead) return;

		PerformJump();
		HandleJumpCount();
	}

	private void HandleJumpCount(bool isKillJump = false)
	{
		if (!isGrounded)
		{
			jumpValue *= doubleJumpForceRatio;
			remainingJumps--;

			if (isKillJump)
			{
				GameManager.Sound.PlaySound("Kill_1");
			}

			else
			{
				if (remainingJumps >= 1)
				{
					GameManager.Sound.PlaySound("Jump_1");
				}

				else
				{
					GameManager.Sound.PlaySound("Jump_2");

					Scene.game.ZoomCamera(4f);

					isDoubleJump = true;
				}
			}
		}
	}

	public bool CanJump()
	{
		return (isGrounded || remainingJumps > 0) && !isFlyMode;
	}

	private void PerformJump()
	{
		animator.SetBool("Jump", true);

		rgbd2d.velocity = new Vector2(rgbd2d.velocity.x, jumpValue);
		isGrounded = false;

		if(Scene.game.isRide)
		{
			Scene.game.ZoomCamera(4f, .2f);
		}

		CheckJump();
	}

	private void CheckJump()
	{
		Util.RunCoroutine(Co_CheckJump(), nameof(Co_CheckJump), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_CheckJump()
	{
		isJumping = true;

		yield return Timing.WaitUntilTrue(() => rgbd2d.velocity.y < 0);

		if (isDoubleJump) Scene.game.ZoomCamera(3f);

		isJumping = false;
		isGrounded = false;

		yield return Timing.WaitUntilTrue(() => isGrounded);

		isGrounded = true;
	}

	#endregion



	#region Skills

	public void Explosion()
	{
		PoolManager.Spawn("Thunder_Explosion", Vector3.zero, Quaternion.identity, this.transform);

		GameManager.Sound.PlaySound("ElectricExplosion");

		MonsterActor[] monsters = FindObjectsOfType<MonsterActor>();

		foreach (MonsterActor monster in monsters)
		{
			float distance = Vector3.Distance(this.transform.position, monster.transform.position);

			if (distance <= damageRadius)
			{
				monster.Die();
			}
		}
	}

	public void StartFly()
	{
		isFlyMode = true;

		this.gameObject.transform.DOMove(new Vector3(this.transform.position.x, 2.3f, this.transform.position.z), .5f).OnComplete(() =>
		{
			if (isDead)
			{
				CancelInvoke(nameof(EndFly));

				EndFly();

				return;
			}

			GameManager.Sound.PlaySound("ElectricFlow");

			PoolManager.Spawn("Thunder_ExplosionSmall", this.transform.position, Quaternion.identity);

			var target = Scene.game.isRide ? "Pivot_Root" : "Root";

			this.transform.Search(target).gameObject.SetActive(false);
			particle_ElectricMode.Play();

			Scene.game.levelController.moveSpeedMultiplier = Scene.game.moveMultiplier;

			mana -= 50;

			GameManager.UI.FetchPanel<Panel_HUD>().SetManaUI();
		});

		rgbd2d.gravityScale = 0;
		rgbd2d.velocity = Vector3.zero;

		Scene.game.SetVirtualCamBody(new Vector3(3.5f, -1.25f, -10f));
		Scene.game.ZoomCamera(4f);

		Invoke(nameof(EndFly), 4.5f);
	}

	public void EndFly()
	{
		GameManager.Sound.PlaySound("Zap");

		PoolManager.Spawn("Thunder_ExplosionSmall", this.transform.position, Quaternion.identity);

		Scene.game.levelController.moveSpeedMultiplier = 1;


		var target = Scene.game.isRide ? "Pivot_Root" : "Root";
		this.transform.Search(target).gameObject.SetActive(true);
		particle_ElectricMode.Stop();

		rgbd2d.gravityScale = 1.5f;

		Scene.game.SetVirtualCamBody(new Vector3(3.25f, 1.25f, -10f));
		Scene.game.ZoomCamera(3f);

		GameManager.UI.FetchPanel<Panel_HUD>().UseSkill(Skills.PowerOverWhelming, 10f);

		isFlyMode = false;
	}

	#endregion


	#region Entity

	public override void Damage(int amount, bool execute = false)
	{
		base.Damage(amount, execute);

		if (health < amount)
		{
			hp.GetComponent<TMP_Text>().text = "Busted";

			Scene.game.cameraShake.Shake();

			var multiplier = isGrounded ? 1.5f : 1f;

			rgbd2d.AddForce(Vector3.right * pushForce * multiplier, ForceMode2D.Impulse);

			Die();

			return;
		}

		health -= amount;

		hp.GetComponent<TMP_Text>().text = health.ToString();
	}

	public override void Die()
	{
		base.Die();

		if (isDead) return;

		GameManager.Sound.PlaySound(Define.SOUND_DIE);

		isDead = true;
		animator.SetBool(Define.DIE, true);
		animator.SetBool(Define.EDITCHK, true);
		rgbd2d.velocity *= dieVelocity;

		if (Scene.game.isRide)
		{
			boxCollider2D.size = new Vector2(1.33f, .75f);
		}

		this.transform.localEulerAngles = Vector3.forward * 13f;

		Scene.game.StopEnviroment();
		Scene.game.SetCameraTarget(null);
		Scene.game.StopDifficulty();
		Scene.game.StopScoreCount();
		Scene.game.GameOver();

		GameManager.UI.FetchPanel<Panel_HUD>().Hide();
	}

	public override void Refresh()
	{
		base.Refresh();

		this.GetComponent<Rigidbody2D>().simulated = false;

		if (Scene.game.isRide)
		{
			boxCollider2D.size = new Vector2(1.33f, 1f);
			boxCollider2D.enabled = false;
		}

		this.transform.position = Vector3.up;
		this.transform.rotation = Quaternion.identity;

		animator.Rebind();
		health = healthOrigin;
		moveSpeed = moveSpeedOrigin;

		hp.GetComponent<TMP_Text>().text = health.ToString();
		boxCollider2D.enabled = true;

		this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		this.GetComponent<Rigidbody2D>().simulated = true;

		GameManager.UI.FetchPanel<Panel_HUD>().Show();
	}

	public void Execute()
	{
		health = 0;

		hp.GetComponent<TMP_Text>().text = "Busted";

		Die();
	}

	public void UpdateAnimator(float moveInput)
	{
		animator.SetBool(Define.RUN, moveInput != 0);
		animator.SetFloat(Define.RUNSTATE, Mathf.Abs(moveInput * .5f));
		animator.SetBool(Define.SLIDE, isSliding);
	}


	public void Stop() => Util.RunCoroutine(Co_SmoothStop(), nameof(Co_SmoothStop), CoroutineTag.Content);

	private IEnumerator<float> Co_SmoothStop()
	{
		isDead = true;
		rgbd2d.velocity *= dieVelocity;

		var value = animator.GetFloat(Define.RUNSTATE);

		while (value > 0)
		{
			value -= Time.deltaTime * .75f;

			animator.SetFloat(Define.RUNSTATE, value);

			yield return Timing.WaitForOneFrame;
		}

		animator.SetBool(Define.RUN, false);
		animator.SetFloat(Define.RUNSTATE, 0f);

		yield return Timing.WaitForSeconds(.5f);

		Scene.game.GameOver();
	}

	#endregion



	#region Callback

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (isDead) return;

		if (collision.gameObject.CompareTag(Define.GROUND))
		{
			isGrounded = true;
			remainingJumps = maxJumps;
			jumpValue = jumpForce;
			animator.SetBool("Jump", false);

			if (isDoubleJump)
			{
				isDoubleJump = false;
				Scene.game.ZoomCamera(3f);
			}
		}

		else if (collision.gameObject.CompareTag(Define.OBSTACLE))
		{
			Die();

			Scene.game.cameraShake.Shake();

			Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

			rgbd2d.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isDead) return;

		if (collision.gameObject.CompareTag(Define.EXECUTE))
		{
			if (!isGrounded && !isJumping && this.transform.position.y > collision.gameObject.transform.position.y - 0.1f)
			{
				isGrounded = true;
				remainingJumps = maxJumps;
				jumpValue = jumpForce;

				if (isDoubleJump)
				{
					isDoubleJump = false;
				}

				animator.SetBool("Jump", false);

				PerformJump();
				HandleJumpCount(true);

				collision.gameObject.GetComponentInParent<MonsterActor>().Damage(attackPoint, true);

				DebugManager.Log("Execute Monster", DebugColor.Game);
			}
		}


		else if (collision.gameObject.CompareTag(Define.OBSTACLE))
		{
			Die();

			Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

			rgbd2d.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

			Scene.game.cameraShake.Shake();
		}
	}

	#endregion



	#region Util

	float pausedTime;

	public void ToggleSimulation(bool simulate)
	{
		if (animator == null) return;

		if (simulate)
		{
			animator.speed = 1f;
			animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, pausedTime);
		}
		else
		{
			AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
			pausedTime = currentState.normalizedTime;
			animator.speed = 0f;
		}

		rgbd2d.simulated = simulate;
	}


	public void Distortion()
	{
		particle_Distortion.Play();
	}

	public void AddDamage(int amount)
	{
		health += amount;

		hp.GetComponent<TMP_Text>().text = health.ToString();
	}

	#endregion
}