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

	TMP_Text txtmp_Empty;

	protected override void Awake()
	{
		base.Awake();

		infiniteRankScroller = this.GetComponentInChildren<InfiniteRankScroller>();
		enhancedScroller = this.GetComponentInChildren<EnhancedScroller>();

		txtmp_Empty = GetUI_TMPText(nameof(txtmp_Empty), "Rank data is empty :(");
	}

	public void GetRankList()
	{
		GameManager.Backend.GetRankList();
	}

	public void Refresh(List<RankData> rankDatas)
	{
		infiniteRankScroller.Refresh(rankDatas);
	}

	public void SetEmpty(bool isEmpty)
	{
		txtmp_Empty.gameObject.SetActive(isEmpty);
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
