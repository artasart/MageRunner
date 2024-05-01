using EnhancedScrollerDemos.GridSimulation;
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

	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_Buy, true);
		img_Thumbnail = Util.FindImage(this.gameObject, nameof(img_Thumbnail));

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
		GameManager.UI.StackPopup<Popup_Basic>(true);

		bool isAd = shopItemData.type == "ad";
		var message = isAd ? "Get Item after wathching AD?" : $"Do you want to purchase with ${shopItemData.price} ?";
		var amount = shopItemData.amount == 0 ? string.Empty : " x " + shopItemData.amount;

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(
			ModalType.ConfirmCancel,
			$"<color=#FFC700>{shopItemData.name}</color> {amount}\n\n{message}",
			isAd ? "Reward" : "Purchase",
			() =>
			{
				GameManager.Scene.Dim(true);

				Invoke(nameof(WatchedAD), 1f);
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

		else if(name == "Bunny Cloth")
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

		else if(name == "Hammer")
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

		else if(name == "Horse")
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

			LocalData.invenData.invenItemData.Add(itemData);

			GameManager.UI.FetchPanel<Panel_Main>().ShowNewIcon(true);
		}

		else if (name == "Gold")
		{
			DebugManager.Log($"Gold is added {shopItemData.amount}", DebugColor.Data);
			Scene.main.AddGold(shopItemData.amount);
		}
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
		txtmp_Quantity.text = itemData.amount.ToString("N0");

		img_Thumbnail.sprite = Resources.Load<Sprite>(itemData.thumbnailPath);

		shopItemData = itemData;
	}
}
