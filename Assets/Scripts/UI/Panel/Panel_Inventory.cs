using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Inventory : Panel_Base
{
	Image img_New;

	Button btn_Equipment;
	Button btn_Resources;
	Button btn_Craft;

	protected override void Awake()
	{
		base.Awake();

		img_New = GetUI_Image(nameof(img_New));

		btn_Equipment = GetUI_Button(nameof(btn_Equipment), OnClick_Equipment, useAnimation:true);
		btn_Resources = GetUI_Button(nameof(btn_Resources), OnClick_Resources, useAnimation: true);
		btn_Craft = GetUI_Button(nameof(btn_Craft), OnClick_Craft, useAnimation: true);

		btn_Resources.onClick.RemoveListener(OpenSound);
		btn_Craft.onClick.RemoveListener(OpenSound);
		
		btn_Resources.onClick.AddListener(() => GameManager.Sound.PlaySound("Denied"));
		btn_Craft.onClick.AddListener(() => GameManager.Sound.PlaySound("Denied"));
	}

	private void OnClick_Equipment()
	{
		ShowNewIcon(false);

		GameManager.UI.StackPanel<Panel_Equipment>(true);

		GameManager.UI.FetchPanel<Panel_Equipment>().GenerateItem();
	}

	private void OnClick_Resources()
	{
		btn_Resources.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

		GameManager.Scene.ShowToastAndDisappear("Content is locked currently.");
	}

	private void OnClick_Craft()
	{
		btn_Craft.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

		GameManager.Scene.ShowToastAndDisappear("Content is locked currently.");
	}

	public void ShowNewIcon(bool enable)
	{
		img_New.gameObject.SetActive(enable);
	}
}
