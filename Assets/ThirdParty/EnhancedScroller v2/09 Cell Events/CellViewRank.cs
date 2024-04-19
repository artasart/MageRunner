using EnhancedScrollerDemos.CellEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellViewRank : CellView
{
	Button btn_Send;

	TMP_Text txtmp_Rank;
	TMP_Text txtmp_Username;
	TMP_Text txtmp_Level;

	private void Awake()
	{
		btn_Send = Util.FindButton(this.gameObject, nameof(btn_Send), OnClick_Send);
	}

	private void Start()
	{
		btn_Send = Util.FindButton(this.gameObject, nameof(btn_Send), OnClick_Send);
	}

	public override void SetData(Data data)
	{
		var rankData = data as RankData;

		Debug.Log("Set Data : " + rankData.nickname);

		txtmp_Username = Util.FindTMPText(this.gameObject, nameof(txtmp_Username), rankData.nickname);
		txtmp_Level = Util.FindTMPText(this.gameObject, nameof(txtmp_Level), $"LV {rankData.level} <color=#BC8252>{rankData.point}</color>");
		txtmp_Rank = Util.FindTMPText(this.gameObject, nameof(txtmp_Rank), rankData.rank.ToString());
	}

	private void OnClick_Send()
	{

	}
}