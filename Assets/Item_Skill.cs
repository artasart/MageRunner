using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using System;

public class Item_Skill : Item_Base
{
	Image img_CoolTime;
	TMP_Text txtmp_SkillTime;

	protected override void Awake()
	{
		base.Awake();

		img_CoolTime = GetUI_Image(nameof(img_CoolTime));
		txtmp_SkillTime = GetUI_TMPText(nameof(txtmp_SkillTime), string.Empty);
	}

	public void Refresh()
	{
		img_CoolTime.fillAmount = 0;
		txtmp_SkillTime.text = string.Empty;

		Util.KillCoroutine(nameof(Co_UseSkill) + this.GetHashCode());
	}

	public void UseSkill(float time, Action callback = null)
	{
		Util.RunCoroutine(Co_UseSkill(time, callback), nameof(Co_UseSkill) + this.GetHashCode(), CoroutineTag.Content);
	}

	IEnumerator<float> Co_UseSkill(float time, Action callback = null)
	{
		float totalTime = time;

		img_CoolTime.fillAmount = 1f;

		while (time >= 0)
		{
			img_CoolTime.fillAmount = 1f - (time / totalTime);

			if (time < 1f) txtmp_SkillTime.text = time.ToString("N1");
			else txtmp_SkillTime.text = time.ToString("N0");

			time -= Time.deltaTime;

			yield return Timing.WaitForOneFrame;
		}

		img_CoolTime.fillAmount = 0;
		txtmp_SkillTime.text = string.Empty;

		callback?.Invoke();
	}
}
