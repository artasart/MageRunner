using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class ParallexScrolling : MonoBehaviour
{
	[Header("Scroll Speed")]
	public float[] layerScrollSpeeds = { .7f, .5f, .35f };

	[Header("Reset Position")]
	public float[] layerResetPositions = { -7f, -7f, -12.5f };

	[Header("List")]
	public List<Transform>[] layers = new List<Transform>[3];
	private int[] currentIndices = new int[3];

	void Start()
	{
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i] = new List<Transform>();

			string layerName = "Layer_" + (i + 1);

			for (int j = 0; j < transform.Search(layerName).childCount; j++)
			{
				layers[i].Add(transform.Search(layerName).GetChild(j));
			}
		}

		StartScroll();
	}

	public void StartScroll()
	{
		Util.RunCoroutine(Co_MoveBackground(0, layerScrollSpeeds[0], layerResetPositions[0]), nameof(Co_MoveBackground) + "1", CoroutineTag.Content);
		Util.RunCoroutine(Co_MoveBackground(1, layerScrollSpeeds[1], layerResetPositions[1]), nameof(Co_MoveBackground) + "3", CoroutineTag.Content);
		Util.RunCoroutine(Co_MoveBackground(2, layerScrollSpeeds[2], layerResetPositions[2]), nameof(Co_MoveBackground) + "2", CoroutineTag.Content);
	}

	public void StopScroll()
	{
		Util.KillCoroutine(nameof(Co_MoveBackground) + "1");
		Util.KillCoroutine(nameof(Co_MoveBackground) + "2");
		Util.KillCoroutine(nameof(Co_MoveBackground) + "3");
	}

	IEnumerator<float> Co_MoveBackground(int layerIndex, float scrollSpeed, float resetPosition)
	{
		List<Transform> layer = layers[layerIndex];

		while (true)
		{
			foreach (Transform background in layer)
			{
				Vector3 newPos = background.position;
				newPos.x -= scrollSpeed * Time.deltaTime;
				background.position = newPos;

				if (background.position.x < resetPosition)
				{
					if (layerIndex == 0 || layerIndex == 1)
					{
						ResetPosition(background, -1.5f, 0f); // 1번과 2번 레이어에만 적용
					}
					RepositionBackground(background, layerIndex);
				}
			}
			yield return Timing.WaitForOneFrame;
		}
	}

	void RepositionBackground(Transform background, int layerIndex)
	{
		currentIndices[layerIndex] = (currentIndices[layerIndex] + 1) % layers[layerIndex].Count;

		Vector3 newPos = layers[layerIndex][currentIndices[layerIndex]].position;
		newPos.x = background.position.x + (background.GetComponent<SpriteRenderer>().bounds.size.x * 2);
		background.position = newPos;
	}

	void ResetPosition(Transform background, float rangeX, float rangeY)
	{
		Vector3 newPos = Vector3.right * 10 + Vector3.right * UnityEngine.Random.Range(5f, 10f) + Vector3.up * UnityEngine.Random.Range(rangeY, rangeX);

		background.position = newPos;
	}
}
