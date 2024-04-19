using EnhancedScrollerDemos.GridSimulation;
using EnhancedUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteInvenGridScroller : Controller
{
	List<InvenItemData> lists = new List<InvenItemData>();

	public void Refresh(List<InvenItemData> _list)
	{
		lists = Util.DeepCopy(_list);

		Application.targetFrameRate = 60;

		scroller.Delegate = this;

		LoadData();
	}

	protected override void LoadData()
	{
		_data = new SmallList<EnhancedScrollerDemos.GridSimulation.Data>();

		for (int i = 0; i < lists.Count; i++)
		{
			_data.Add(lists[i]);
		}

		scroller.ReloadData();
	}
}
