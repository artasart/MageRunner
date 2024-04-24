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

	public bool isDefault;

	Scene_Game game;
	LevelController levelController;

	List<GameObject> generatedMonsters = new List<GameObject>();


	public float moveSpeedMultiplier = 1f;

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_Move) + this.GetHashCode());
		Util.KillCoroutine(nameof(Co_SetMoveSpeed) + this.GetHashCode());
	}

	protected override void Awake()
	{
		base.Awake();

		blocks = new GameObject[this.transform.childCount];

		for (int i = 0; i < this.transform.childCount; i++)
		{
			blocks[i] = this.transform.GetChild(i).gameObject;
		}

		index = this.transform.GetSiblingIndex();
		startPosition = this.transform.position;

		game = FindObjectOfType<Scene_Game>();
		levelController = FindObjectOfType<LevelController>();
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

			blocks[i].gameObject.SetActive(random >= levelController.groundProbability);
		}
	}

	public void Move()
	{
		Util.RunCoroutine(Co_Move(), nameof(Co_Move) + this.GetHashCode(), CoroutineTag.Content);
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
			yield return Timing.WaitUntilTrue(() => game.gameState == GameState.Playing);

			transform.Translate(Vector3.left * moveSpeed * moveSpeedMultiplier * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		CorrectPosition();

		RandomizeGround();

		Util.RunCoroutine(Co_Move(), nameof(Co_Move) + this.GetHashCode());
	}

	public float heightProbability = .1f;

	private void CorrectPosition()
	{
		int childCount = this.transform.parent.childCount;

		int randomX = 0;
		int randomY = 0;
		
		if(heightProbability > UnityEngine.Random.Range(0, 1))
		{
			randomY = UnityEngine.Random.Range(-1, 1);
		}

		if (randomY != 0) randomX += 2;
		else if (this.transform.parent.GetChild(childCount - 1).position.y != 0) randomX += 2;

		Vector3 lastPosition = this.transform.parent.GetChild(childCount - 1).position + Vector3.right * 10f + new Vector3(randomX, 0, 0);
		lastPosition = new Vector3(lastPosition.x, randomY, lastPosition.z);

		this.transform.position = lastPosition;
		this.transform.SetAsLastSibling();

		foreach (var item in generatedMonsters)
		{
			item.GetComponent<MonsterActor>().Refresh();
		}

		generatedMonsters.Clear();
	}

	private void RandomizeGround()
	{
		for (int i = 0; i < blocks.Length - 1; i++)
		{
			var random = UnityEngine.Random.Range(0, 1f);

			blocks[i].SetActive(levelController.groundProbability <= random);

			if (!levelController.generateMonster) continue;

			if (blocks[i].activeSelf)
			{
				if (UnityEngine.Random.Range(0f, 1f) <= levelController.monsterProbability)
				{
					generatedMonsters.Add(PoolManager.Spawn(Define.MONSTERACTOR, Vector3.right * UnityEngine.Random.Range(-.4f, .4f) + Vector3.up * 2.48f, Quaternion.identity, blocks[i].transform));
				}
			}
		}
	}

	public void SetProbability(float amount)
	{
		levelController.groundProbability = Mathf.Clamp(levelController.groundProbability += amount, 0f, .85f);
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

		foreach (var item in generatedMonsters)
		{
			item.GetComponent<MonsterActor>().Refresh();
		}

		generatedMonsters.Clear();
	}
}