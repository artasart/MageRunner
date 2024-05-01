using EnhancedScrollerDemos.GridSimulation;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel_Shop : Panel_Base
{
	InifiniteShopGridScorller infiniteGridScroller;

	Transform group_Menu;

	Button btn_Skin;
	Button btn_Resource;

	TMP_Text current;
	TMP_Text txtmp_Message;

	private void OnDisable()
	{
		if(GameManager.UI.FetchPanel<Panel_Main>() != null && Scene.main != null && Scene.main.navigator != null)
		{
			var parent = GameManager.UI.FetchPanel<Panel_Main>().transform;

			Scene.main.navigator.transform.SetParent(parent.Search("MobileSafeArea"));
			Scene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
			Scene.main.navigator.transform.SetAsLastSibling();
		}
	}

	protected override void Awake()
	{
		base.Awake();

		infiniteGridScroller = FindObjectOfType<InifiniteShopGridScorller>();

		group_Menu = this.transform.Search(nameof(group_Menu));

		btn_Skin = GetUI_Button(nameof(btn_Skin), OnClick_BuySkin, useAnimation:true);
		btn_Resource = GetUI_Button(nameof(btn_Resource), OnClick_BuyEquipment, useAnimation:true);
		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "hmm... how about purchasing some items for your character..?");
		txtmp_Message.UsePingPong();
	}

	private void Start()
	{
		OnClick_BuySkin();

		txtmp_Message.StartPingPong();
	}

	protected override void OnClick_Back()
	{
		base.OnClick_Back();

		var parent = GameManager.UI.FetchPanel<Panel_Main>().transform;

		Scene.main.navigator.transform.SetParent(parent.Search("MobileSafeArea"));
		Scene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
		Scene.main.navigator.transform.SetAsLastSibling();
	}

	public void Init()
	{
		Scene.main.navigator.transform.SetParent(this.transform.Search("MobileSafeArea"));
		Scene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
		Scene.main.navigator.transform.SetAsLastSibling();

		OnClick_BuySkin();
	}

	private void OnClick_BuySkin()
	{
		var item = LocalData.masterData.shopItem
			.Where(item => item.category == "skin")
			.Select(item => new ShopItemData(item.name, item.type, item.price, item.amount, item.thumbnailPath))
			.ToList();

		infiniteGridScroller.Refresh(item);

		SetTextColor(btn_Skin);
	}

	private void OnClick_BuyEquipment()
	{
		var test = LocalData.masterData.shopItem
			.Where(item => item.category == "resource")
			.Select(item => new ShopItemData(item.name, item.type, item.price, item.amount, item.thumbnailPath))
			.ToList();

		infiniteGridScroller.Refresh(test);

		SetTextColor(btn_Resource);
	}

	private void SetTextColor(Button btn)
	{
		if (current != null)
		{
			current.color = Util.HexToRGB("#DCDCDC");
		}

		current = btn.GetComponentInChildren<TMP_Text>();
		current.color = Util.HexToRGB("#FFC700");
	}
}
