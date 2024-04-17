using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;
using static Unity.Collections.AllocatorManager;

public class Ground : LevelElement
{
	public GameObject[] blocks;

	private Vector3 startPosition;
	public int index = 0;

	[Range(0f, 1f)] public float probability = .3f;

	public bool isDefault;

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_Move) + this.GetHashCode());
		Util.KillCoroutine(nameof(Co_SetMoveSpeed) + this.GetHashCode());
	}

	protected override void Awake()
	{
		base.Awake();

		blocks = new GameObject[this.transform.childCount];

		for(int i =0; i < this.transform.childCount; i++)
		{
			blocks[i] = this.transform.GetChild(i).gameObject;
		}

		index = this.transform.GetSiblingIndex();
		startPosition = this.transform.position;
	}

	public void Generate()
	{
		type = PropsType.Ground;

		blocks = new GameObject[this.transform.childCount];

		for (int i = 0; i < this.transform.childCount; i++)
		{
			blocks[i] = this.transform.GetChild(i).gameObject;
		}

		for (int i = 1; i < blocks.Length - 1; i++)
		{
			var random = UnityEngine.Random.Range(0, 1f);

			blocks[i].gameObject.SetActive(random >= probability);
		}
	}

	public void Move()
	{
		Util.RunCoroutine(Co_Move(), nameof(Co_Move) + this.GetHashCode());
	}

	public void Stop()
	{
		Util.KillCoroutine(nameof(Co_Move) + this.GetHashCode());
	}

	public float moveSpeed = 0f;

	public void SetMoveSpeed(float target)
	{
		Util.RunCoroutine(Co_SetMoveSpeed(target), nameof(Co_SetMoveSpeed) + this.GetHashCode());
	}

	IEnumerator<float> Co_SetMoveSpeed(float target)
	{
		float value = 0f;

		while (Mathf.Abs(moveSpeed - target) > 0.01f)
		{
			moveSpeed = Mathf.Lerp(moveSpeed, target, value += Time.deltaTime * .15f);

			yield return Timing.WaitForOneFrame;
		}

		moveSpeed = target;
	}

	IEnumerator<float> Co_Move()
	{
		while (this.transform.position.x >= -10f)
		{
			transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		CorrectPosition();

		RandomizeGround();

		Util.RunCoroutine(Co_Move(), nameof(Co_Move) + this.GetHashCode());
	}

	private void CorrectPosition()
	{
		float gap = 10f;

		this.gameObject.transform.SetAsLastSibling();
		this.gameObject.transform.position = new Vector3(30f, 0f, 0f);

		for (int i = 0; i < 3; i++)
		{
			this.transform.parent.GetChild(i).transform.position = new Vector3(i * gap, 0f, 0f);
		}
	}

	private void RandomizeGround()
	{
		for (int i = 0; i < blocks.Length - 1; i++)
		{
			var random = UnityEngine.Random.Range(0, 1f);

			blocks[i].SetActive(random >= probability);
		}
	}


	public void Refresh()
	{
		for (int i = 0; i < blocks.Length; i++)
		{
			blocks[i].SetActive(true);
		}

		this.transform.SetSiblingIndex(index);

		this.transform.position = startPosition;

		moveSpeed = 0f;
	}
}