using MEC;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerActor : MonoBehaviour
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
	[SerializeField] bool isGrounded = true;
	[SerializeField] bool isSliding = false;
	[SerializeField] float slideTimer = 0f;
	[SerializeField] int remainingJumps;

	[Header("Entity")]
	[HideInInspector] public bool isDead = false;
	[HideInInspector] public int health = 10;

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

	#endregion



	#region Movement

	void Update()
	{
		if (isDead) return;

		float moveInput = Input.GetAxis("Horizontal");

		UpdateAnimator(moveInput);
		HandleFacingDirection(moveInput);
		HandleSlideInput();
		HandleMovement(moveInput);
		HandleJumpInput();
	}

	private void HandleFacingDirection(float moveInput)
	{
		if (moveInput > 0f)
			SetFacingDirection(-1);
		else if (moveInput < 0f)
			SetFacingDirection(1);
	}

	private void HandleSlideInput()
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

	private void HandleJumpInput()
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

	private void HandleJumpCount()
	{
		if (!isGrounded)
		{
			jumpValue *= doubleJumpForceRatio;
			remainingJumps--;
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

	private bool CanJump()
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

	void UpdateAnimator(float moveInput)
	{
		animator.SetBool(Define.RUN, moveInput != 0);
		animator.SetFloat(Define.RUNSTATE, Mathf.Abs(moveInput * .5f));
		animator.SetBool(Define.SLIDE, isSliding);
	}

	public void Die()
	{
		isDead = true;
		animator.SetBool(Define.DIE, true);
		animator.SetBool(Define.EDITCHK, true);

		game.SetCameraTarget(null);

		Invoke(nameof(ShowGameOverUI), .75f);
	}

	private void ShowGameOverUI()
	{
		GameManager.UI.StackPopup<Popup_GameOver>();
	}

	public void Refresh()
	{
		this.transform.position = Vector3.up * -0.2f;
		animator.Rebind();
	}

	#endregion



	#region Callback

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
			remainingJumps = maxJumps;
			jumpValue = jumpForce; // 기본 점프 힘으로 재설정
		}
	}

	#endregion
}