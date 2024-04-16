using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : LevelElement
{
	GameObject[] blocks;

	[Range(0f, 1f)] public float probability = .3f;

	public bool isDefault;

	protected override void Awake()
	{
		base.Awake();
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
}