using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f; // 플레이어 이동 속도
	public float jumpForce = 5f; // 플레이어 점프 힘
	public float slideDuration = 1f; // 슬라이드 지속 시간
	public float slideSpeedMultiplier = 2f; // 슬라이드 속도 배율
	public float doubleJumpForceRatio = 0.8f; // 더블 점프 시 힘 적용 비율
	public int maxJumps = 2; // 최대 점프 횟수

	float jumpValue = 10f; // 플레이어 점프 힘
	bool isGrounded = true; // 플레이어가 땅에 있는지 여부
	bool isSliding = false; // 플레이어가 슬라이드 중인지 여부
	float slideTimer = 0f; // 슬라이드 타이머
	int remainingJumps; // 남은 점프 횟수

	Rigidbody2D rb;
	Animator animator;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
		remainingJumps = maxJumps;
	}

	void Update()
	{
		float moveInput = Input.GetAxis("Horizontal");

		UpdateAnimator(moveInput);

		if (moveInput > 0f)
		{
			transform.localScale = new Vector3(-1, 1, 1); // 오른쪽을 보도록 설정
		}
		else if (moveInput < 0f)
		{
			transform.localScale = new Vector3(1, 1, 1); // 왼쪽을 보도록 설정
		}

		if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isSliding)
		{
			isSliding = true;
			slideTimer = slideDuration; // 슬라이드 지속 시간 설정
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
					jumpValue *= doubleJumpForceRatio; // 더블 점프 힘 적용
					remainingJumps--;
				}
				rb.velocity = new Vector2(rb.velocity.x, jumpValue);
				isGrounded = false;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
			remainingJumps = maxJumps;
			jumpValue = jumpForce; // 기본 점프 힘으로 재설정
		}
	}

	void UpdateAnimator(float moveInput)
	{
		animator.SetBool("Run", moveInput != 0);
		animator.SetFloat("RunState", Mathf.Abs(moveInput * .5f));
		animator.SetBool("Slide", isSliding);
	}
}
