using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using System;
using static Enums;
using Unity.VisualScripting;

public class Item_Skill : Item_Base
{
	Image img_CoolTime;
	TMP_Text txtmp_SkillTime;
	Image img_Thumbnail;

	CanvasGroup canvasGroup;

	protected override void Awake()
	{
		base.Awake();

		img_CoolTime = GetUI_Image(nameof(img_CoolTime));
		txtmp_SkillTime = GetUI_TMPText(nameof(txtmp_SkillTime), string.Empty);
		img_Thumbnail = GetUI_Image(nameof(img_Thumbnail));

		canvasGroup = img_CoolTime.AddComponent<CanvasGroup>();
	}

	public void Refresh()
	{
		Util.KillCoroutine(nameof(Co_UseSkill) + this.GetHashCode());
		Util.KillCoroutine(canvasGroup.GetHashCode().ToString());

		canvasGroup.alpha = 0;
		img_CoolTime.fillAmount = 0;
		txtmp_SkillTime.text = string.Empty;

		if (img_Thumbnail) img_Thumbnail.sprite = null;
		if (img_Thumbnail) img_Thumbnail.gameObject.SetActive(false);
	}

	public void UseSkill(float time, Action callback = null)
	{
		Util.RunCoroutine(Co_UseSkill(time, callback), nameof(Co_UseSkill) + this.GetHashCode(), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_UseSkill(float time, Action callback = null)
	{
		float totalTime = time;

		img_CoolTime.fillAmount = 1f;

		canvasGroup.alpha = 1f;

		while (time >= 0 && img_CoolTime != null)
		{
			yield return Timing.WaitUntilTrue(() => Scene.game.gameState == GameState.Playing);

			img_CoolTime.fillAmount = 1f - (time / totalTime);

			if (time < 1f) txtmp_SkillTime.text = time.ToString("N1");
			else txtmp_SkillTime.text = time.ToString("N0");

			time -= Time.deltaTime;

			yield return Timing.WaitForOneFrame;
		}

		txtmp_SkillTime.text = string.Empty;

		Util.FadeCanvasGroup(canvasGroup, 0f, .5f, end: () => img_CoolTime.fillAmount = 0);

		callback?.Invoke();
	}
}
