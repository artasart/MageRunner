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
	}

	private void OnClick_Equipment()
	{
		img_Background.sprite = null;

		ShowNewIcon(false);

		GameManager.UI.StackPanel<Panel_Equipment>(true);

		GameManager.UI.FetchPanel<Panel_Equipment>().GenerateItem();
	}

	private void OnClick_Resources()
	{

	}

	private void OnClick_Craft()
	{

	}

	public void ShowNewIcon(bool enable)
	{
		img_New.gameObject.SetActive(enable);
	}
}
