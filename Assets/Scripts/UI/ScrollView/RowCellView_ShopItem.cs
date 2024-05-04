using DG.Tweening;
using EnhancedScrollerDemos.GridSimulation;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class RowCellView_ShopItem : RowCellView
{
	ShopItemData shopItemData;

	Button btn_Container;
	Image img_Thumbnail;

	TMP_Text txtmp_Name;
	TMP_Text txtmp_Price;
	TMP_Text txtmp_Quantity;
	TMP_Text txtmp_Title;

	Transform group_Normal;
	Transform group_Quantity;

	Image img_Outline;
	Image img_Buy;

	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_Buy, false, true);
		img_Thumbnail = Util.FindImage(this.gameObject, nameof(img_Thumbnail));
		img_Outline = Util.FindImage(this.gameObject, nameof(img_Outline));
		img_Buy = Util.FindImage(this.gameObject, nameof(img_Buy));

		txtmp_Name = Util.FindTMPText(this.gameObject, nameof(txtmp_Name));
		txtmp_Title = Util.FindTMPText(this.gameObject, nameof(txtmp_Title));
		txtmp_Price = Util.FindTMPText(this.gameObject, nameof(txtmp_Price));
		txtmp_Quantity = Util.FindTMPText(this.gameObject, nameof(txtmp_Quantity));

		group_Normal = this.transform.Search(nameof(group_Normal));
		group_Quantity = this.transform.Search(nameof(group_Quantity));

		group_Normal.gameObject.SetActive(false);
		group_Quantity.gameObject.SetActive(false);
	}

	private void OnClick_Buy()
	{
		if (shopItemData.type == "ad")
		{
			if (LocalData.gameData.isAdWatched)
			{
				GameManager.Sound.PlaySound(Define.SOUND_DENIED);

				this.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

				return;
			}
		}


		if (shopItemData.type == "gold")
		{
			if (LocalData.gameData.gold < shopItemData.price)
			{
				this.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

				var rect = GameManager.UI.FetchPanel<Panel_Main>().group_TopMenu.Search("btn_Coin").GetComponent<RectTransform>();
				rect.DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

				GameManager.Sound.PlaySound(Define.SOUND_DENIED);

				return;
			}
		}

		GameManager.Sound.PlaySound(Define.SOUND_OPEN);

		GameManager.UI.StackPopup<Popup_Basic>(true);

		bool isAd = shopItemData.type == "ad";

		var priceTag = string.Empty;

		if (!isAd)
		{
			if (shopItemData.type == "cash")
			{
				priceTag = $"{shopItemData.price}";
			}

			else
			{
				priceTag = $"{shopItemData.price} Gold";
			}
		}

		var message = isAd ? "Get Item after wathching AD?" : $"Do you want to purchase with {priceTag} ?";
		var amount = shopItemData.amount == 0 ? string.Empty : " x " + shopItemData.amount;


		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(
			ModalType.ConfirmCancel,
			$"<color=#FFC700>{shopItemData.name}</color> {amount}\n\n{message}",
			isAd ? "Reward" : "Purchase",
			() =>
			{
				GameManager.Scene.Dim(true);

				Invoke(nameof(WatchedAD), 1f);

				// GameManager.AdMob.ShowRewardedAd(() => Invoke(nameof(WatchedAD), 1f));
			},
			() =>
			{

			});
	}

	private void WatchedAD()
	{
		GameManager.Scene.callback_ShowToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(false);
		GameManager.Scene.callback_CloseToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
		GameManager.Scene.callback_ClickToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);

		GameManager.Scene.ShowToastAndDisappear($"You recevied {shopItemData.name}!!");

		BuyMethods(shopItemData.name);

		GameManager.Scene.Dim(false);

		if (shopItemData.type == "ad")
		{
			this.transform.Search("img_BlockAds").GetComponent<BlockAds>().WatchAd();

			GameManager.UI.FetchPanel<Panel_Main>().BlockUI();
		}
	}


	public void BuyMethods(string name)
	{
		if (name == "Bunny Hat")
		{
			var itemData = new InvenItemData(name);
			itemData.type = EquipmentType.Helmet;
			itemData.name = "Bunny Hat";
			itemData.index = 1;
			itemData.thumbnail = "SPUM/SPUM_Sprites/Packages/F_SR/4_Helmet/F_SR_Helmet_1";
			itemData.nameIndex = "F_SR_Helmet";
			itemData.quantity = 1;
			itemData.price = 3000000;
			itemData.spaceCount = 1;
			itemData.isRide = false;

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
			GameManager.UI.FetchPanel<Panel_Inventory>().ShowNewIcon(true);
		}

		else if (name == "Bunny Cloth")
		{
			var itemData = new InvenItemData(name);
			itemData.type = EquipmentType.Cloth;
			itemData.name = "Bunny Cloth";
			itemData.index = 1;
			itemData.thumbnail = "SPUM/SPUM_Sprites/Packages/F_SR/2_Cloth/F_SR_Cloth_1";
			itemData.nameIndex = "F_SR_Cloth";
			itemData.quantity = 1;
			itemData.price = 3000000;
			itemData.spaceCount = 1;
			itemData.isRide = false;

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
			GameManager.UI.FetchPanel<Panel_Inventory>().ShowNewIcon(true);
		}

		else if (name == "Hammer")
		{
			var itemData = new InvenItemData(name);
			itemData.type = EquipmentType.Weapons;
			itemData.name = "Hammer";
			itemData.index = 1;
			itemData.thumbnail = "SPUM/SPUM_Sprites/Packages/F_SR/6_Weapons/F_SR_Hammer_1";
			itemData.nameIndex = "F_SR_Hammer";
			itemData.quantity = 1;
			itemData.price = 3000000;
			itemData.spaceCount = 1;
			itemData.isRide = false;

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
			GameManager.UI.FetchPanel<Panel_Inventory>().ShowNewIcon(true);
		}

		else if (name == "Horse")
		{
			var itemData = new InvenItemData(name);
			itemData.name = "Horse";
			itemData.index = 1;
			itemData.thumbnail = "Sprites/Ride/Horse_1";
			itemData.nameIndex = "Horse_1";
			itemData.quantity = 1;
			itemData.price = 0;
			itemData.spaceCount = 5;
			itemData.isRide = true;
			itemData.ride = new Ride("Horse", 1);

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
		}

		else if (name == "Blue Horse")
		{
			var itemData = new InvenItemData(name);
			itemData.name = "Horse";
			itemData.index = 2;
			itemData.thumbnail = "Sprites/Ride/Horse_2";
			itemData.nameIndex = "Horse_2";
			itemData.quantity = 1;
			itemData.price = 0;
			itemData.spaceCount = 5;
			itemData.isRide = true;
			itemData.ride = new Ride("Blue Horse", 1);

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
		}

		else if (name == "Red Horse")
		{
			var itemData = new InvenItemData(name);
			itemData.name = "Horse";
			itemData.index = 4;
			itemData.thumbnail = "Sprites/Ride/Horse_4";
			itemData.nameIndex = "Horse_4";
			itemData.quantity = 1;
			itemData.price = 0;
			itemData.spaceCount = 5;
			itemData.isRide = true;
			itemData.ride = new Ride("Red Horse", 1);

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
		}

		else if (name == "Dark Horse")
		{
			var itemData = new InvenItemData(name);
			itemData.name = "Horse";
			itemData.index = 3;
			itemData.thumbnail = "Sprites/Ride/Horse_3";
			itemData.nameIndex = "Horse_3";
			itemData.quantity = 1;
			itemData.price = 0;
			itemData.spaceCount = 5;
			itemData.isRide = true;
			itemData.ride = new Ride("Dark Horse", 1);

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
		}

		else if (name == "Gold")
		{
			DebugManager.Log($"Gold is added {shopItemData.amount}", DebugColor.Data);
			Scene.main.AddGold(shopItemData.amount);
		}

		else if (name == "Energy")
		{
			DebugManager.Log($"Energy is added {shopItemData.amount}", DebugColor.Data);

			GameManager.UI.FetchPanel<Panel_Main>().AddEnergy(5);
		}

		if (shopItemData.type == "gold")
		{
			LocalData.gameData.gold -= shopItemData.price;

			GameManager.UI.FetchPanel<Panel_Main>().SetGoldUI(LocalData.gameData.gold);
		}

		GameManager.UI.FetchPanel<Panel_Shop>().Refresh();
	}


	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		if (!(data is ShopItemData itemData))
			return;

		group_Normal.gameObject.SetActive(itemData.amount == 0);
		group_Quantity.gameObject.SetActive(itemData.amount > 0);

		txtmp_Name.text = itemData.name;
		txtmp_Title.text = itemData.name;
		txtmp_Price.text = itemData.type == "cash" ? $"${itemData.price}" : "Watch AD";

		img_Outline.color = Util.HexToRGB("#FFC700");
		img_Buy.color = Util.HexToRGB("#FFC700");

		if (itemData.type == "gold")
		{
			txtmp_Price.text = $"{itemData.price} Gold";

			img_Outline.color = Util.HexToRGB("#DCDCDC");
			img_Buy.color = Util.HexToRGB("#DCDCDC");
		}

		txtmp_Quantity.text = itemData.amount.ToString("N0");

		img_Thumbnail.sprite = Resources.Load<Sprite>(itemData.thumbnailPath);

		shopItemData = itemData;

		if (itemData.type == "ad")
		{
			this.transform.Search("img_BlockAds").gameObject.SetActive(true);

			if (LocalData.gameData.isAdWatched)
			{
				this.transform.Search("img_BlockAds").GetComponent<BlockAds>().WatchAd(false);
			}
		}

		else
		{
			this.transform.Search("img_BlockAds").gameObject.SetActive(false);
		}
	}
}
