using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Splash_Congrates : Splash_Base
{
	TMP_Text txtmp_Score;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void Awake()
	{
		base.Awake();

		txtmp_Score = GetUI_TMPText(nameof(txtmp_Score), "0");
	}

	public void SetScore(int score)
	{
		txtmp_Score.text = string.Empty;

		Util.AnimateText(txtmp_Score, score, 1f, .25f, "m");
	}
}
