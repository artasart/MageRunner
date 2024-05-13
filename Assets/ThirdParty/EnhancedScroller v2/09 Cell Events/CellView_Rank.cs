using EnhancedScrollerDemos.CellEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView_Rank : CellView
{
	TMP_Text txtmp_Rank;
	TMP_Text txtmp_Nickname;
	TMP_Text txtmp_Level;
	TMP_Text txtmp_Score;

	private void Awake()
	{
		txtmp_Nickname = Util.FindTMPText(this.gameObject, nameof(txtmp_Nickname), string.Empty);
		txtmp_Level = Util.FindTMPText(this.gameObject, nameof(txtmp_Level), string.Empty);
		txtmp_Rank = Util.FindTMPText(this.gameObject, nameof(txtmp_Rank), string.Empty);
		txtmp_Score = Util.FindTMPText(this.gameObject, nameof(txtmp_Score), string.Empty);
	}

	public override void SetData(Data data)
	{
		var rankData = data as RankData;

		txtmp_Nickname.text = rankData.nickname;
		txtmp_Level.text = "Lv." + rankData.level.ToString();
		txtmp_Rank.text = rankData.rank.ToString();
		txtmp_Score.text = rankData.score.ToString("N0") + " m";

		if (rankData.isMine)
		{
			txtmp_Nickname.color = Util.HexToRGB("#FFC700");
			txtmp_Level.color = Util.HexToRGB("#FFC700");
			txtmp_Rank.color = Util.HexToRGB("#FFC700");
			txtmp_Score.color = Util.HexToRGB("#FFC700");
		}

		else
		{
			txtmp_Nickname.color = Util.HexToRGB("#DCDCDC");
			txtmp_Level.color = Util.HexToRGB("#DCDCDC");
			txtmp_Rank.color = Util.HexToRGB("#DCDCDC");
			txtmp_Score.color = Util.HexToRGB("#DCDCDC");
		}
	}
}