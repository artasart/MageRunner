using System.Collections.Generic;
using UnityEngine;
using EnhancedScrollerDemos.CellEvents;


public class InfiniteRankScroller : Controller
{
	List<RankData> lists = new List<RankData>();

	protected override void Start()
	{

	}

	public void Refresh(List<RankData> _list)
	{
		lists.Clear();

		foreach (var item in _list)
		{
			lists.Add(item);
		}

		Application.targetFrameRate = 60;

		scroller.Delegate = this;

		LoadData();
	}

	protected override void LoadData()
	{
		_data = new List<Data>();

		for (int i = 0; i < lists.Count; i++)
		{
			_data.Add(lists[i]);
		}

		GameManager.UI.FetchPopup<Popup_Rank>().enhancedScroller.ReloadData();
	}
}
