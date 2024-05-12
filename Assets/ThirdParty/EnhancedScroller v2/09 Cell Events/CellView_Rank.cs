using EnhancedScrollerDemos.CellEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView_Rank : CellView
{
	TMP_Text txtmp_Rank;
	TMP_Text txtmp_Username;
	TMP_Text txtmp_Level;

	private void Awake()
	{
		txtmp_Username = Util.FindTMPText(this.gameObject, nameof(txtmp_Username), string.Empty);
		txtmp_Level = Util.FindTMPText(this.gameObject, nameof(txtmp_Level), string.Empty);
		txtmp_Rank = Util.FindTMPText(this.gameObject, nameof(txtmp_Rank), string.Empty);
	}

	public override void SetData(Data data)
	{
		var rankData = data as RankData;

		Debug.Log("Set Data : " + rankData.nickname);

		txtmp_Username.text = rankData.nickname;
	}
}