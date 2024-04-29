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

	private void OnEnable()
	{
		GameManager.UI.FetchPanel<Panel_Main>().Show();
	}

	protected override void Awake()
	{
		base.Awake();

		infiniteGridScroller = FindObjectOfType<InifiniteShopGridScorller>();

		group_Menu = this.transform.Search(nameof(group_Menu));

		btn_Skin = GetUI_Button(nameof(btn_Skin), OnClick_BuySkin, useAnimation:true);
		btn_Resource = GetUI_Button(nameof(btn_Resource), OnClick_BuyEquipment, useAnimation:true);
	}

	private void Start()
	{
		OnClick_BuySkin();
	}

	public void Init() => OnClick_BuySkin();

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
