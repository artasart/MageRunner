using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
	[SerializeField] float jumpValue = 10f;
	[SerializeField] public bool isGrounded = true;
	[SerializeField] bool isSliding = false;
	[SerializeField] float slideTimer = 0f;
	[SerializeField] int remainingJumps;

	[Header("Entity")]
	[HideInInspector] public bool isDead = false;
	[HideInInspector] public int health = 10;
	[HideInInspector] public int attackPoint = 10;

	[HideInInspector] public int healthOrigin = 10;
	[HideInInspector] public float moveSpeedOrigin = 5;

	public float dieVelocity = .75f;

	Rigidbody2D rgbd2d;
	Animator animator;
	Transform hp;

	Scene_Game game;

	#endregion



	#region Initialize

	private void Awake()
	{
		game = FindObjectOfType<Scene_Game>();

		rgbd2d = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		remainingJumps = maxJumps;

		hp = this.transform.Search(nameof(hp));
		hp.GetComponent<TMP_Text>().text = health.ToString();
	}

	private void Start()
	{
		healthOrigin = health;
		moveSpeedOrigin = moveSpeed;
	}

	#endregion



	#region Movement

	void Update()
	{
		if (game.gameState == GameState.Paused) return;

		if (isDead) return;

		//float moveInput = Input.GetAxis("Horizontal");

		//HandleFacingDirection(1f);
		HandleSlideInput();
		//HandleMovement(moveInput);
		HandleJumpInput();
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
		if (!CanJump()) return;

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
				Debug.Log("Kill Jump");

				GameManager.Sound.PlaySound("Kill_1");
			}

			else
			{
				Debug.Log(remainingJumps);

				if(remainingJumps >= 1)
				{
					GameManager.Sound.PlaySound("Jump_1");
				}

				else
				{
					GameManager.Sound.PlaySound("Jump_2");
				}
			}
		}
	}

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
		return isGrounded || remainingJumps > 0;
	}

	private void PerformJump()
	{
		rgbd2d.velocity = new Vector2(rgbd2d.velocity.x, jumpValue);
		isGrounded = false;
	}

	#endregion



	#region Entity

	public void Execute()
	{
		health = 0;

		hp.GetComponent<TMP_Text>().text = "Busted";

		Die();
	}

	public void Damage(int amount)
	{
		if (health < amount)
		{
			hp.GetComponent<TMP_Text>().text = "Busted";

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

	public void Die()
	{
		if (isDead) return;

		GameManager.Sound.PlaySound("BodyFall_1");

		this.GetComponent<FootStepController>().StopWalk();
		FindObjectOfType<GroundController>().StopGround();

		isDead = true;
		animator.SetBool(Define.DIE, true);
		animator.SetBool(Define.EDITCHK, true);

		rgbd2d.velocity *= dieVelocity;

		game.SetCameraTarget(null);
		game.StopDifficult();

		this.transform.localEulerAngles = Vector3.forward * 13f;

		Invoke(nameof(ShowGameOverUI), .75f);
	}

	private void ShowGameOverUI()
	{
		GameManager.UI.FetchPanel<Panel_HUD>().HidePanel();

		GameManager.UI.FetchPopup<Popup_GameOver>().SetResult(game.score, game.coin, game.exp = Mathf.RoundToInt(game.score * .45f));

		GameManager.UI.StartPopup<Popup_GameOver>();
	}

	public void Refresh()
	{
		this.transform.position = Vector3.up * -0.2f;
		this.transform.rotation = Quaternion.identity;

		animator.Rebind();
		health = healthOrigin;
		moveSpeed = moveSpeedOrigin;

		this.GetComponent<BoxCollider2D>().enabled = true;
		hp.GetComponent<TMP_Text>().text = health.ToString();

		GameManager.UI.FetchPanel<Panel_HUD>().ShowPanel();
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
		}

		if (collision.gameObject.CompareTag(Define.MONSTER))
		{
			Vector2 contactPoint = collision.GetContact(0).point;
			Vector2 playerBottom = new Vector2(transform.position.x, transform.position.y - 0.5f);

			if (contactPoint.y > playerBottom.y)
			{
				isGrounded = true;
				remainingJumps = maxJumps;
				jumpValue = jumpForce;

				PerformJump();
				HandleJumpCount(true);

				collision.gameObject.GetComponent<MonsterActor>().Damage(attackPoint, true);

				DebugManager.ClearLog("Execute Monster", DebugColor.Game);
			}
		}

		if (collision.gameObject.CompareTag(Define.OBSTACLE))
		{
			Die();

			Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

			rgbd2d.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
		}
	}

	public float pushForce = -2f;

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