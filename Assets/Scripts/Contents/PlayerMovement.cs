using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f; // �÷��̾� �̵� �ӵ�
	public float jumpForce = 5f; // �÷��̾� ���� ��
	public float slideDuration = 1f; // �����̵� ���� �ð�
	public float slideSpeedMultiplier = 2f; // �����̵� �ӵ� ����
	public float doubleJumpForceRatio = 0.8f; // ���� ���� �� �� ���� ����
	public int maxJumps = 2; // �ִ� ���� Ƚ��

	float jumpValue = 10f; // �÷��̾� ���� ��
	bool isGrounded = true; // �÷��̾ ���� �ִ��� ����
	bool isSliding = false; // �÷��̾ �����̵� ������ ����
	float slideTimer = 0f; // �����̵� Ÿ�̸�
	int remainingJumps; // ���� ���� Ƚ��

	Rigidbody2D rb;
	Animator animator;

	Action die;

	bool isDead = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		remainingJumps = maxJumps;

		die += FindObjectOfType<Scene_Game>().SaveScore;
		die += GameManager.UI.FetchPanel<Panel_HUD>().RefreshUI;
	}

	void Update()
	{
		if (isDead) return;

		float moveInput = Input.GetAxis("Horizontal");

		UpdateAnimator(moveInput);

		if (moveInput > 0f)
		{
			transform.localScale = new Vector3(-1, 1, 1); // �������� ������ ����
		}
		else if (moveInput < 0f)
		{
			transform.localScale = new Vector3(1, 1, 1); // ������ ������ ����
		}

		if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isSliding)
		{
			isSliding = true;
			slideTimer = slideDuration; // �����̵� ���� �ð� ����
			moveSpeed *= slideSpeedMultiplier;
		}

		if (isSliding)
		{
			slideTimer -= Time.deltaTime;
			if (slideTimer <= 0)
			{
				isSliding = false;
				moveSpeed /= slideSpeedMultiplier;
			}
		}

		if (isSliding && Mathf.Abs(rb.velocity.x) < 0.1f)
		{
			isSliding = false;
			moveSpeed /= slideSpeedMultiplier;
		}

		rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (isGrounded || remainingJumps > 0)
			{
				if (isSliding)
				{
					isSliding = false;
					moveSpeed /= slideSpeedMultiplier;
				}
				if (!isGrounded)
				{
					jumpValue *= doubleJumpForceRatio; // ���� ���� �� ����
					remainingJumps--;
				}
				rb.velocity = new Vector2(rb.velocity.x, jumpValue);
				isGrounded = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			Die();
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
			remainingJumps = maxJumps;
			jumpValue = jumpForce; // �⺻ ���� ������ �缳��
		}
	}

	void UpdateAnimator(float moveInput)
	{
		animator.SetBool("Run", moveInput != 0);
		animator.SetFloat("RunState", Mathf.Abs(moveInput * .5f));
		animator.SetBool("Slide", isSliding);
	}

	public void Die()
	{
		Util.RunCoroutine(Co_Die(), nameof(Co_Die));
	}

	private IEnumerator<float> Co_Die()
	{
		isDead = true;

		animator.SetBool("Die", true);

		yield return Timing.WaitForSeconds(1f);

		GameManager.Scene.Fade(true);

		yield return Timing.WaitUntilTrue(GameManager.Scene.IsFaded);

		this.transform.position = Vector3.up * -0.2f;

		animator.SetBool("Die", false);
		animator.SetBool("Run", false);
		animator.SetBool("Editchk", false);

		die?.Invoke();

		GameManager.Scene.Fade(false);

		yield return Timing.WaitUntilFalse(GameManager.Scene.IsFaded);

		isDead = false;
	}
}
