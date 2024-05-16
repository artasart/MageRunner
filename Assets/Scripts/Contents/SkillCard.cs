using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class SkillCard : MonoBehaviour
{
	Transform graphic;

	Vector3 targetScale = new Vector3(.75f, .75f, .75f);
	float duration = .75f;

	private void Awake()
	{
		graphic = this.transform.Search(nameof(graphic));
	}

	private void Start()
	{
		graphic.DOScale(targetScale, duration)
			.SetEase(Ease.OutQuad) // ????? ?? ??
			.SetLoops(-1, LoopType.Yoyo);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (GameScene.game.playerActor.isDead) return;

		if (other.CompareTag(Define.PLAYER))
		{
			GameManager.Sound.PlaySound("Pick", .125f);

			PoolManager.Spawn("Firework", this.transform.localPosition, Quaternion.identity, this.transform.parent);

			Invoke(nameof(ShowPopup), .25f);

			this.GetComponent<RePoolObject>().RePool();
		}

		else if (other.CompareTag(Define.COLLECTOR))
		{
			this.GetComponent<RePoolObject>().RePool();
		}
	}

	private void ShowPopup()
	{
		GameManager.UI.StartPopup<Popup_Skill>();

		GameManager.UI.FetchPopup<Popup_Skill>().SetCard();
	}
}