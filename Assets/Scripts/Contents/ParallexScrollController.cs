using System.Collections.Generic;
using UnityEngine;
using MEC;
using static Enums;

public class ParallexScrollController : MonoBehaviour
{
	public float scrollSpeed = 1f;
	public float resetPositonX = -10f;

	public Transform[] layers;
	public int childCount = 0;

	private void OnDestroy()
	{
		Stop();
	}

	void Start()
	{
		childCount = this.transform.childCount;

		layers = new Transform[childCount];

		for (int i = 0; i < childCount; i++)
		{
			layers[i] = this.transform.GetChild(i);
		}
	}

	public void Scroll()
	{
		Util.RunCoroutine(Co_MoveBackground(), nameof(Co_MoveBackground) + this.GetHashCode(), CoroutineTag.Content);
	}

	public void Stop()
	{
		Util.KillCoroutine(nameof(Co_MoveBackground) + this.GetHashCode());
	}

	IEnumerator<float> Co_MoveBackground()
	{
		while (true)
		{
			yield return Timing.WaitUntilTrue(() => FindObjectOfType<Scene_Game>().gameState == GameState.Playing);
			foreach(var item in layers)
			{
				Vector3 newPos = item.position;
				newPos.x -= scrollSpeed * Time.deltaTime;

				item.transform.position = newPos;

				if(item.position.x <= resetPositonX)
				{
					RepositionBackground(item);
				}
			}
					
			yield return Timing.WaitForOneFrame;
		}
	}

	void RepositionBackground(Transform background)
	{
		var lastChild = this.transform.GetChild(childCount - 1);
		var resetPositon = lastChild.position + Vector3.right * (lastChild.GetComponent<SpriteRenderer>().bounds.size.x - .1f);

		background.position = resetPositon;
		background.SetAsLastSibling();
	}
}
