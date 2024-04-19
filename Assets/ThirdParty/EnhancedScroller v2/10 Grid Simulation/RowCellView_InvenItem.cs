using EnhancedScrollerDemos.GridSimulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowCellView_InvenItem : RowCellView
{
	Button btn_Container;
	InvenItemData invenItemData;

	Image img_Thumnail;
	Image img_New;

	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_EquipItem, true);
		img_Thumnail = Util.FindImage(this.gameObject, nameof(img_Thumnail));
	}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		//DebugManager.ClearLog(invenItemData.thumbnail);

		//var sprite = Resources.Load<Sprite>(invenItemData.thumbnail);

		//img_Thumnail.sprite = sprite;

		invenItemData = data as InvenItemData;
	}

	private void OnClick_EquipItem()
	{
		if(invenItemData.isRide)
		{
			FindObjectOfType<RideController>().Init(invenItemData.name, invenItemData.index);
		}

		Debug.Log(invenItemData.name + " is selected..");
	}
}
