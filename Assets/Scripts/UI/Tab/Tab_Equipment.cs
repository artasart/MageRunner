using EnhancedScrollerDemos.GridSimulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab_Equipment : Tab_Base
{
	InfiniteInvenGridScroller infiniteGridScroller;
	List<InvenItemData> data = new List<InvenItemData>();

	private void OnEnable()
	{
		GenerateItem();
	}

	protected override void Awake()
	{
		base.Awake();

		btn_Back = GetUI_Button(nameof(btn_Back), () => GameManager.UI.FetchPanel<Panel_Inventory>().CloseTab<Tab_Equipment>());

		infiniteGridScroller = FindObjectOfType<InfiniteInvenGridScroller>();
	}

	private void GenerateItem()
	{
		data.Clear();

		for(int i = 0; i < 1; i++)
		{
			var invenItemData = new InvenItemData();
			invenItemData.isRide = true;
			invenItemData.name = "Horse";
			invenItemData.index = 2;

			invenItemData.price = 100000;
			invenItemData.isNew = true;
			invenItemData.quantity = 10;
			invenItemData.thumbnail = $"SPUM/SPUM_Sprites/RideSource/{invenItemData.name + "" + invenItemData.index}/{invenItemData.name + "" + invenItemData.index}";

			data.Add(invenItemData);
		}

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Save()
	{
		Debug.Log("OnClick_Save");
	}
}
