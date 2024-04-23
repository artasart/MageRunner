using EnhancedScrollerDemos.GridSimulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowCellView_InvenItem : RowCellView
{
	Button btn_Container;

	Image img_Thumnail;

	public InvenItemData invenItemData;

	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_EquipItem, true);
		img_Thumnail = Util.FindImage(this.gameObject, nameof(img_Thumnail));
	}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		var invenData = data as InvenItemData;

		if(invenData != null)
		{
			if(invenData.thumbnail != null)
			{
				var sprite = Resources.Load<Sprite>(invenData.thumbnail);

				img_Thumnail.sprite = sprite;
			}
		}

		invenItemData = invenData;
	}

	private void OnClick_EquipItem()
	{
		if (invenItemData.isRide)
		{
			FindObjectOfType<RideController>().Init(invenItemData.nameIndex, invenItemData.index);
		}

		else
		{
			var equipment = new Equipment(invenItemData.type, invenItemData.nameIndex, invenItemData.index);

			Debug.Log(equipment.type);

			FindObjectOfType<EquipmentManager>().PreviewEquipment(equipment, invenItemData.thumbnail);
		}

		GameManager.UI.FetchPanel<Panel_Equipment>().isChanged = true;

		Debug.Log($"{invenItemData.nameIndex}_{invenItemData.index} is selected..");
	}

	public bool isChanged = false;
}
