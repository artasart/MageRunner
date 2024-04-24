using DG.Tweening;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
	[SerializeField] bool isSliding = false;
	[SerializeField] float slideTimer = 0f;
	[SerializeField] int remainingJumps;

	[Header("Entity")]
	[HideInInspector] public int attackPoint = 10;
	[HideInInspector] public float moveSpeedOrigin = 5;

	public float dieVelocity = .75f;
	public float pushForce = -2f;

	Rigidbody2D rgbd2d;
	Transform hp;

	Scene_Game game;

	ParticleSystem particle_ElectricMode;

	#endregion



	#region Initialize

	protected override void Awake()
	{
		base.Awake();

		rgbd2d = GetComponent<Rigidbody2D>();

		hp = this.transform.Search(nameof(hp));
		hp.GetComponent<TMP_Text>().text = health.ToString();

		game = FindObjectOfType<Scene_Game>();
		remainingJumps = maxJumps;
	}

	private void Start()
	{
		moveSpeedOrigin = moveSpeed;

		particle_ElectricMode = this.transform.Search(nameof(particle_ElectricMode)).GetComponent<ParticleSystem>();
		particle_ElectricMode.Stop();
	}

	#endregion



	#region Movement

	void Update()
	{
		if (game.gameState == GameState.Paused) return;

		if (isDead) return;

		if (Input.GetKeyDown(KeyCode.Q))
		{
			PowerOverWelmingMode();
		}

		if (Input.GetKeyDown(KeyCode.W))
		{
			Thunder();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			Explosion();
		}

		HandleSlideInput();
		HandleJumpInput();

		if (rgbd2d.velocity.y < 0) isGrounded = false;
	}

	public void Thunder()
	{
		MonsterActor[] monsters = FindObjectsOfType<MonsterActor>();
		MonsterActor closestMonster = null;

		float closestDistance = Mathf.Infinity;

		// 각 MonsterActor 오브젝트와 플레이어 간의 거리를 비교합니다.
		foreach (MonsterActor monster in monsters)
		{
			float distance = Vector3.Distance(this.transform.position, monster.transform.position);

			// 현재까지 찾은 가장 가까운 MonsterActor를 업데이트합니다.
			if (distance < closestDistance && !monster.isDead)
			{
				closestDistance = distance;
				closestMonster = monster;
			}
		}

		// 가장 가까운 MonsterActor를 찾았습니다.
		if (closestMonster != null)
		{
			closestMonster.Die();

			PoolManager.Spawn("Thunder", Vector3.zero, Quaternion.identity, closestMonster.transform);

			GameManager.Sound.PlaySound("Spawn", .5f);
		}
	}

	public float killRadius = 10f;

	public void Explosion()
	{
		PoolManager.Spawn("Thunder_Explosion", Vector3.zero, Quaternion.identity, this.transform);

		GameManager.Sound.PlaySound("ElectricExplosion");

		MonsterActor[] monsters = FindObjectsOfType<MonsterActor>();

		foreach (MonsterActor monster in monsters)
		{
			float distance = Vector3.Distance(this.transform.position, monster.transform.position);

			if (distance <= killRadius)
			{
				monster.Die();
			}
		}
	}

	// 기즈모를 그리는 함수
	void OnDrawGizmosSelected()
	{
		// 기즈모 색상 설정
		Gizmos.color = Color.red;

		// 기즈모를 플레이어 주변에 그리기
		Gizmos.DrawWireSphere(this.transform.position, killRadius);
	}

	public bool isPowerMode = false;
	public void PowerOverWelmingMode()
	{
		isPowerMode = true;

		this.gameObject.transform.DOMove(new Vector3(this.transform.position.x, 2.3f, this.transform.position.z), .5f).OnComplete(() =>
		{
			GameManager.Sound.PlaySound("ElectricFlow");

			PoolManager.Spawn("Thunder_ExplosionSmall", this.transform.position, Quaternion.identity);

			this.transform.Search("Root").gameObject.SetActive(false);
			particle_ElectricMode.Play();

			FindObjectOfType<LevelController>().SetMoveMultiplier(game.moveMultiplier);
		});

		rgbd2d.gravityScale = 0;
		rgbd2d.velocity = Vector3.zero;

		game.SetVirtualCamBody(new Vector3(4.5f, -1.25f, -10f));
		game.ZoomCamera(4f);

		Invoke(nameof(EndPower), 4.5f);
	}

	public void EndPower()
	{
		GameManager.Sound.PlaySound("Zap");

		PoolManager.Spawn("Thunder_ExplosionSmall", this.transform.position, Quaternion.identity);

		FindObjectOfType<LevelController>().SetMoveMultiplier(1);

		this.transform.Search("Root").gameObject.SetActive(true);
		particle_ElectricMode.Stop();

		rgbd2d.gravityScale = 1.5f;

		game.SetVirtualCamBody(new Vector3(4f, 1.25f, -10f));
		game.ZoomCamera(3f);

		isPowerMode = false;
	}

	private void HandleFacingDirection(float moveInput)
	{
		if (moveInput > 0f)
			SetFacingDirection(-1);
		else if (moveInput < 0f)
			SetFacingDirection(1);
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

	private void HandleMovement(float moveInput)
	{
		if (!isSliding || Mathf.Abs(rgbd2d.velocity.x) < 0.1f)
			rgbd2d.velocity = new Vector2(moveInput * moveSpeed, rgbd2d.velocity.y);
	}

	public void HandleJumpInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (CanJump())
			{
				PerformJump();
				HandleJumpCount();
			}
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

					game.ZoomCamera(4f);

					isDoubleJump = true;
				}
			}
		}
	}

	bool isDoubleJump = false;

	private void SetFacingDirection(float direction)
	{
		transform.localScale = hp.localScale = new Vector3(direction, 1, 1);
	}

	private void StartSlide()
	{
		isSliding = true;
		slideTimer = slideDuration;
		moveSpeed *= slideSpeedMultiplier;
	}

	private void EndSlide()
	{
		isSliding = false;
		moveSpeed /= slideSpeedMultiplier;
	}

	public bool CanJump()
	{
		return (isGrounded || remainingJumps > 0) && !isPowerMode;
	}

	private void PerformJump()
	{
		rgbd2d.velocity = new Vector2(rgbd2d.velocity.x, jumpValue);
		isGrounded = false;

		CheckIsJumping();
	}

	private void CheckIsJumping()
	{
		Util.RunCoroutine(Co_CheckIsJumping(), nameof(Co_CheckIsJumping), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_CheckIsJumping()
	{
		isJumping = true;

		yield return Timing.WaitUntilTrue(() => rgbd2d.velocity.y < 0);

		if (isDoubleJump) game.ZoomCamera(3f);

		isJumping = false;
		isGrounded = false;
		Debug.Log(isGrounded);

		yield return Timing.WaitUntilTrue(() => isGrounded);

		isGrounded = true;
	}

	#endregion



	#region Entity

	public void Execute()
	{
		health = 0;

		hp.GetComponent<TMP_Text>().text = "Busted";

		Die();
	}

	public override void Damage(int amount, bool execute = false)
	{
		base.Damage(amount, execute);

		if (health < amount)
		{
			hp.GetComponent<TMP_Text>().text = "Busted";

			FindObjectOfType<CameraShake3D>().Shake();

			var multiplier = isGrounded ? 1.5f : 1f;

			rgbd2d.AddForce(Vector3.right * pushForce * multiplier, ForceMode2D.Impulse);

			Die();

			return;
		}

		health -= amount;

		hp.GetComponent<TMP_Text>().text = health.ToString();
	}

	public void UpdateAnimator(float moveInput)
	{
		animator.SetBool(Define.RUN, moveInput != 0);
		animator.SetFloat(Define.RUNSTATE, Mathf.Abs(moveInput * .5f));
		animator.SetBool(Define.SLIDE, isSliding);
	}

	public override void Die()
	{
		base.Die();

		if (isDead) return;

		isDead = true;

		GameManager.Sound.PlaySound("BodyFall_1");

		this.GetComponent<FootStepController>().StopWalk();
		FindObjectOfType<LevelController>().StopGround();
		var scroll = FindObjectsOfType<ParallexScrolling>();
		foreach (var item in scroll)
		{
			item.StopScroll();
		}

		animator.SetBool(Define.DIE, true);
		animator.SetBool(Define.EDITCHK, true);

		rgbd2d.velocity *= dieVelocity;

		game.SetCameraTarget(null);
		game.StopDifficult();

		this.transform.localEulerAngles = Vector3.forward * 13f;

		GameManager.UI.FetchPanel<Panel_HUD>().Hide();

		Invoke(nameof(ShowGameOverUI), .75f);
	}

	private void ShowGameOverUI()
	{
		GameManager.UI.FetchPopup<Popup_GameOver>().SetResult(game.score, game.gold, game.exp = Mathf.RoundToInt(game.score * .45f));

		GameManager.UI.StartPopup<Popup_GameOver>();
	}

	public override void Refresh()
	{
		base.Refresh();

		this.transform.position = Vector3.up * -0.2f;
		this.transform.rotation = Quaternion.identity;

		animator.Rebind();
		health = healthOrigin;
		moveSpeed = moveSpeedOrigin;

		this.GetComponent<BoxCollider2D>().enabled = true;
		hp.GetComponent<TMP_Text>().text = health.ToString();

		GameManager.UI.FetchPanel<Panel_HUD>().Show();
	}

	public void Stop()
	{
		isDead = true;
		rgbd2d.velocity *= dieVelocity;

		Util.RunCoroutine(Co_SmoothStop(), nameof(Co_SmoothStop));
	}

	IEnumerator<float> Co_SmoothStop()
	{
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

		ShowGameOverUI();
	}

	#endregion



	#region Callback

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag(Define.GROUND))
		{
			isGrounded = true;
			remainingJumps = maxJumps;
			jumpValue = jumpForce;

			if (isDoubleJump)
			{
				isDoubleJump = false;
				game.ZoomCamera(3f);
			}
		}

		else if (collision.gameObject.CompareTag(Define.OBSTACLE))
		{
			Die();

			FindObjectOfType<CameraShake3D>().Shake();

			Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

			rgbd2d.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);

			Debug.Log("Monster");
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
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

				PerformJump();
				HandleJumpCount(true);

				collision.gameObject.GetComponentInParent<MonsterActor>().Damage(attackPoint, true);

				DebugManager.Log("Execute Monster", DebugColor.Game);
			}
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

	#endregion
}