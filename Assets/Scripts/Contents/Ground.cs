using MEC;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Ground : LevelElement
{
	public GameObject[] blocks;

	private Vector3 startPosition;
	public int index = 0;

	public bool isDefault;

	Scene_Game game;
	LevelController levelController;

	public List<GameObject> generatedMonsters = new List<GameObject>();
	public List<GameObject> generatedCoins = new List<GameObject>();
	public List<GameObject> generatedSkillCards = new List<GameObject>();

	public float moveSpeed { get; set; }

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_Move) + this.GetHashCode());
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

	IEnumerator<float> Co_Move()
	{
		while (this.transform.position.x >= -10f)
		{
			yield return Timing.WaitUntilTrue(() => game.gameState == GameState.Playing);

			transform.Translate(Vector3.left * Scene.game.levelController.moveSpeed * Scene.game.levelController.moveSpeedMultiplier * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		CorrectPosition();

		RandomizeGround();

		Util.RunCoroutine(Co_Move(), nameof(Co_Move) + this.GetHashCode());
	}

	public void Stop()
	{
		Util.KillCoroutine(nameof(Co_Move) + this.GetHashCode());
	}

	public float heightProbability = .1f;

	private void CorrectPosition()
	{
		int childCount = this.transform.parent.childCount;

		int randomX = 0;
		int randomY = 0;

		if (heightProbability > UnityEngine.Random.Range(0, 1))
		{
			randomY = UnityEngine.Random.Range(-1, 2);
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
		generatedCoins.Clear();
		generatedSkillCards.Clear();
	}

	private void RandomizeGround()
	{
		for (int i = 0; i < blocks.Length - 1; i++)
		{
			var random = UnityEngine.Random.Range(0, 1f);

			blocks[i].SetActive(levelController.groundProbability <= random);

			if (blocks[i].activeSelf)
			{
				if (UnityEngine.Random.Range(0f, 1f) <= levelController.monsterProbability)
				{
					var monster = PoolManager.Spawn(Define.MONSTER_ACTOR, Vector3.right * UnityEngine.Random.Range(-.4f, .4f) + Vector3.up * 2.48f, Quaternion.identity, blocks[i].transform);
					monster.GetComponent<MonsterActor>().damage = Random.Range(1, 3) * 10;

					generatedMonsters.Add(monster);
				}

				else
				{
					if (UnityEngine.Random.Range(0f, 1f) <= levelController.coinProbability)
					{
						for (int amount = -1; amount < 2; amount++)
						{
							generatedCoins.Add(PoolManager.Spawn("Coin", Vector3.right * (amount * 0.675f) + Vector3.up * 2.9f, Quaternion.identity, blocks[i].transform));
						}
					}

					else
					{
						if (!isSkillCardGenerated && UnityEngine.Random.Range(0f, 1f) <= levelController.skillCardProbability)
						{
							generatedSkillCards.Add(PoolManager.Spawn("SkillCard", Vector3.up * 3f, Quaternion.identity, blocks[i].transform));

							Util.RunCoroutine(Cehck_SkillCard(), nameof(Cehck_SkillCard));
						}
					}
				}
			}
		}

		for (int amount = -1; amount < 2; amount++)
		{
			generatedCoins.Add(PoolManager.Spawn("Coin", Vector3.right * (amount * 0.675f) + Vector3.up * 2.9f, Quaternion.identity, blocks[blocks.Length - 1].transform));
		}
	}

	public bool isSkillCardGenerated;
	public IEnumerator<float> Cehck_SkillCard()
	{
		isSkillCardGenerated = true;

		yield return Timing.WaitForSeconds(5f);

		isSkillCardGenerated = false;
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

		var monsters = FindObjectsOfType<MonsterActor>();
		var coins = FindObjectsOfType<Coin>();
		var skillcards = FindObjectsOfType<SkillCard>();

		foreach (var item in monsters)
		{
			item.GetComponent<MonsterActor>().Refresh();
		}

		foreach (var item in coins)
		{
			item.GetComponent<RePoolObject>().RePool();
		}

		foreach (var item in skillcards)
		{
			item.GetComponent<RePoolObject>().RePool();
		}

		generatedMonsters.Clear();
		generatedCoins.Clear();
		generatedSkillCards.Clear();
	}

	public void ResetItems()
	{
		foreach (var item in generatedCoins)
		{
			item.GetComponent<RePoolObject>().RePool();
		}

		foreach (var item in generatedSkillCards)
		{
			item.GetComponent<RePoolObject>().RePool();
		}
	}
}