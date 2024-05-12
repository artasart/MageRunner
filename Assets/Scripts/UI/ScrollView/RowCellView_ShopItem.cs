using DG.Tweening;
using EnhancedScrollerDemos.GridSimulation;
using MEC;
using System.Collections.Generic;
using TMPro;
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

	Transform img_BlockAds;

	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_Buy, false, true);
		img_Thumbnail = Util.FindImage(this.gameObject, nameof(img_Thumbnail));
		img_Outline = Util.FindImage(this.gameObject, nameof(img_Outline));
		img_Buy = Util.FindImage(this.gameObject, nameof(img_Buy));
		img_BlockAds = Util.FindTransform(this.gameObject, nameof(img_BlockAds));


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

		// after wathching AD?
		var message = isAd ? "Get item after watching AD?" : $"Do you want to purchase with {priceTag} ?";
		var amount = shopItemData.amount == 0 ? string.Empty : " x " + shopItemData.amount;


		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(
			ModalType.ConfirmCancel,
			$"<color=#FFC700>{shopItemData.name}</color> {amount}\n\n{message}",
			isAd ? "Reward" : "Purchase",
			() =>
			{
				GameManager.Scene.Dim(true);

				if (shopItemData.type == "ad")
				{
					// Util.RunCoroutine(Co_WatchedAD(shopItemData.name).Delay(.5f), nameof(Co_WatchedAD), CoroutineTag.Content);
					GameManager.AdMob.ShowRewardedInterstitialAd(() => Util.RunCoroutine(Co_WatchedAD(shopItemData.name).Delay(.5f), nameof(Co_WatchedAD), CoroutineTag.Content));
				}

				else
				{
					Util.RunCoroutine(Co_WatchedAD(shopItemData.name).Delay(.5f), nameof(Co_WatchedAD), CoroutineTag.Content);
				}
			},
			() =>
			{

			});
	}

	public void BuyMethods(string name)
	{
		switch (name)
		{
			case "Bunny Hat":
				BuyBunnyHat();
				break;
			case "Bunny Cloth":
				BuyBunnyCloth();
				break;
			case "Hammer":
				BuyHammer();
				break;
			case "Horse":
				BuyHorse("Horse_1", "Sprites/Ride/Horse_1", 1);
				break;
			case "Blue Horse":
				BuyHorse("Horse_2", "Sprites/Ride/Horse_2", 2);
				break;
			case "Red Horse":
				BuyHorse("Horse_4", "Sprites/Ride/Horse_4", 4);
				break;
			case "Dark Horse":
				BuyHorse("Horse_3", "Sprites/Ride/Horse_3", 3);
				break;
			case "Gold":
				DebugManager.Log($"Gold is added {shopItemData.amount}", DebugColor.Data);
				GameScene.main.AddGold(shopItemData.amount);
				break;
			case "Energy":
				DebugManager.Log($"Energy is added {shopItemData.amount}", DebugColor.Data);
				GameManager.UI.FetchPanel<Panel_Main>().AddEnergy(5);
				break;
			default:
				break;
		}

		if (shopItemData.type == "gold")
		{
			LocalData.gameData.gold -= shopItemData.price;
			GameManager.UI.FetchPanel<Panel_Main>().SetGoldUI(LocalData.gameData.gold);
		}

		GameManager.UI.FetchPanel<Panel_Shop>().Refresh();
	}

	private void BuyBunnyHat()
	{
		var itemData = new InvenItemData("Bunny Hat");
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

	private void BuyBunnyCloth()
	{
		var itemData = new InvenItemData("Bunny Cloth");
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

	private void BuyHammer()
	{
		var itemData = new InvenItemData("Hammer");
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

	private void BuyHorse(string nameIndex, string thumbnailPath, int index)
	{
		var itemData = new InvenItemData("Horse");
		itemData.name = "Horse";
		itemData.index = index;
		itemData.thumbnail = thumbnailPath;
		itemData.nameIndex = nameIndex;
		itemData.quantity = 1;
		itemData.price = 0;
		itemData.spaceCount = 5;
		itemData.isRide = true;
		itemData.ride = new Ride("Horse", 1);

		LocalData.invenData.invenItemData.Add(itemData);

		GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
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
		txtmp_Price.text = itemData.type == "cash" ? $"${itemData.price}" : "Free"; // Watch AD

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
			img_BlockAds.gameObject.SetActive(true);

			if (LocalData.gameData.isAdWatched)
			{
				img_BlockAds.GetComponent<BlockAds>().WatchAd(false);
			}
		}

		else
		{
			img_BlockAds.gameObject.SetActive(false);
		}
	}


	private IEnumerator<float> Co_WatchedAD(string name)
	{
		yield return Timing.WaitForOneFrame;

		GameManager.Scene.Dim(false);

		BuyMethods(name);

		if (shopItemData.type == "ad")
		{
			GameManager.UI.StackSplash<Splash_Gold>();
			GameManager.UI.FetchSplash<Splash_Gold>().OpenBox();

			img_BlockAds.GetComponent<BlockAds>().WatchAd();

			GameManager.UI.FetchPanel<Panel_Main>().BlockUI();
		}

		else
		{
			GameManager.Scene.callback_ShowToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(false);
			GameManager.Scene.callback_CloseToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
			GameManager.Scene.callback_ClickToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
			GameManager.Scene.ShowToastAndDisappear($"You owned {name}!");
		}
	}
}