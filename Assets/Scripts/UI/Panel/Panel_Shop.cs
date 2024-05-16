using EnhancedScrollerDemos.GridSimulation;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class Panel_Shop : Panel_Base
{
	InifiniteShopGridScorller infiniteGridScroller;

	Transform group_Menu;

	Button btn_Skin;
	Button btn_Horse;
	Button btn_Resource;

	TMP_Text current;
	TMP_Text txtmp_Message;
	TMP_Text txtmp_Empty;

	ShopMenu menu;

	private void OnDisable()
	{
		if(GameManager.UI.FetchPanel<Panel_Main>() != null && GameScene.main != null && GameScene.main.navigator != null)
		{
			var parent = GameManager.UI.FetchPanel<Panel_Main>().transform;

			GameScene.main.navigator.transform.SetParent(parent.Search("MobileSafeArea"));
			GameScene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
			GameScene.main.navigator.transform.SetAsLastSibling();
		}
	}

	protected override void Awake()
	{
		base.Awake();

		infiniteGridScroller = FindObjectOfType<InifiniteShopGridScorller>();

		group_Menu = this.transform.Search(nameof(group_Menu));

		btn_Skin = GetUI_Button(nameof(btn_Skin), OnClick_Equipment, useAnimation: true);
		btn_Resource = GetUI_Button(nameof(btn_Resource), OnClick_Resources, useAnimation: true);
		btn_Horse = GetUI_Button(nameof(btn_Horse), OnCLick_Horse, useAnimation: true);

		txtmp_Empty = GetUI_TMPText(nameof(txtmp_Empty), "content is empty :(");
		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "hmm... how about purchasing some items for your character..?");
		txtmp_Message.UsePingPong();
	}

	private void Start()
	{
		OnClick_Equipment();

		txtmp_Message.StartPingPong();
	}

	protected override void OnClick_Back()
	{
		base.OnClick_Back();

		var parent = GameManager.UI.FetchPanel<Panel_Main>().transform;

		if (GameManager.UI.FetchPanel<Panel_Main>() != null && GameScene.main != null && GameScene.main.navigator != null)
		{
			GameScene.main.navigator.transform.SetParent(parent.Search("MobileSafeArea"));
			GameScene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
			GameScene.main.navigator.transform.SetAsLastSibling();
		}
	}

	public void Init()
	{
		if (GameManager.UI.FetchPanel<Panel_Main>() != null && GameScene.main != null && GameScene.main.navigator != null)
		{
			GameScene.main.navigator.transform.SetParent(this.transform.Search("MobileSafeArea"));
			GameScene.main.navigator.GetComponent<RectTransform>().localScale = Vector3.one;
			GameScene.main.navigator.transform.SetAsLastSibling();
		}

		OnClick_Equipment();
	}

	private void OnClick_Equipment()
	{
		menu = ShopMenu.Equipment;

		var data = LocalData.masterData.shopItem
			.Where(item => item.category == "skin" && !item.name.Contains("Horse"))
			.Select(item => new ShopItemData(item.name, item.type, item.price, item.amount, item.thumbnailPath))
			.Where(shopItem => !LocalData.invenData.invenItemData.Any(invenItem => invenItem.name == shopItem.name))
			.ToList();

		infiniteGridScroller.Refresh(data);

		txtmp_Empty.gameObject.SetActive(data.Count <= 0);

		SetTextColor(btn_Skin);
	}

	private void OnCLick_Horse()
	{
		menu = ShopMenu.Horse;

		var data = LocalData.masterData.shopItem
			.Where(item => item.category == "skin" && item.name.Contains("Horse"))
			.Select(item => new ShopItemData(item.name, item.type, item.price, item.amount, item.thumbnailPath))
			.Where(shopItem => !LocalData.invenData.invenItemData.Any(invenItem => invenItem.ride.name == shopItem.name))
			.ToList();

		infiniteGridScroller.Refresh(data);

		txtmp_Empty.gameObject.SetActive(data.Count <= 0);

		SetTextColor(btn_Horse);
	}

	private void OnClick_Resources()
	{
		menu = ShopMenu.Resources;

		var data = LocalData.masterData.shopItem
					.Where(item => item.category == "resource")
					.Select(item => new ShopItemData(item.name, item.type, item.price, item.amount, item.thumbnailPath))
					.ToList();

		infiniteGridScroller.Refresh(data);

		txtmp_Empty.gameObject.SetActive(data.Count <= 0);

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

	public void Refresh()
	{
		switch(menu)
		{
			case ShopMenu.Equipment:
				OnClick_Equipment();
				break;
			case ShopMenu.Horse:
				OnCLick_Horse();
				break;
			case ShopMenu.Resources:
				OnClick_Resources();
				break;
			default:
				break;
		}
	}
}
