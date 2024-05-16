using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Stat : Panel_Base
{
	private void OnDisable()
	{
		if (GameManager.UI.FetchPanel<Panel_Main>() != null && GameScene.main != null && GameScene.main.navigator != null)
		{
			var parent = GameManager.UI.FetchPanel<Panel_Main>().transform;

			GameScene.main.navigator.transform.SetParent(parent.Search("MobileSafeArea"));
			GameScene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
			GameScene.main.navigator.transform.SetAsLastSibling();
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public void Init()
	{
		GameScene.main.navigator.transform.SetParent(this.transform.Search("MobileSafeArea"));
		GameScene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
		GameScene.main.navigator.transform.SetAsLastSibling();

		var items = FindObjectsOfType<Item_StatUpgrade>();

		foreach(var item in items)
		{
			item.Init();
		}
	}
}
