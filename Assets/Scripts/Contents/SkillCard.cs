using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class SkillCard : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			GameManager.UI.StartPopup<Popup_Skill>();

			GameManager.UI.FetchPopup<Popup_Skill>().SetCard();

			this.GetComponent<RePoolObject>().RePool();

			GameManager.Sound.PlaySound("Pick", .125f);
		}
	}
}
