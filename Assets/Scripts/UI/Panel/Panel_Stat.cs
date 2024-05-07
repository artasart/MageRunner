using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Stat : Panel_Base
{
	private void OnDisable()
	{
		if (GameManager.UI.FetchPanel<Panel_Main>() != null && Scene.main != null && Scene.main.navigator != null)
		{
			var parent = GameManager.UI.FetchPanel<Panel_Main>().transform;

			Scene.main.navigator.transform.SetParent(parent.Search("MobileSafeArea"));
			Scene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
			Scene.main.navigator.transform.SetAsLastSibling();
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public void Init()
	{
		Scene.main.navigator.transform.SetParent(this.transform.Search("MobileSafeArea"));
		Scene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
		Scene.main.navigator.transform.SetAsLastSibling();

		var items = FindObjectsOfType<Item_StatUpgrade>();

		foreach(var item in items)
		{
			item.Init();
		}
	}
}
