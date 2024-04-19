using EnhancedScrollerDemos.GridSimulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowCellView_InvenItem : RowCellView
{
	private void Awake()
	{
		container = this.transform.Search("Container").gameObject;
	}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		var invenItem = data as InvenItemData;

		Debug.Log(invenItem.name);
	}
}
