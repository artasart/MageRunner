using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Rank : Popup_Base
{
	public InfiniteRankScroller infiniteRankScroller { get; private set; }
	public EnhancedScroller enhancedScroller { get; private set; }


	protected override void Awake()
	{
		base.Awake();

		infiniteRankScroller = this.GetComponentInChildren<InfiniteRankScroller>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Refresh();
		}
	}

	public void GetRankList()
	{
		GameManager.Backend.GetRankList();
	}

	public void Refresh()
	{
		var rankDatas = new List<RankData>();

		var rankData = new RankData();
		rankData.nickname = "artasart";

		rankDatas.Add(rankData);

		infiniteRankScroller.Refresh(rankDatas);
	}
}

[System.Serializable]
public class RankRes : DefaultRes
{
	public RankReq[] ranks;
}

[System.Serializable]
public class RankReq
{
	public string nickname;
	public string thumbnail;
	public int level;
	public int point;
}
