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

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"<color=#FFC700>{shopItemData.name}</color>\n\n Get Item after wathching AD?", shopItemData.type == "ad" ? "Reward" : "Purchase",
		() =>
		{
			Invoke(nameof(WatchedAD), 1f);
		},
		() =>
		{

		});
	}

	private void WatchedAD()
	{
		GameManager.Scene.callback_ShowToast = () => GameManager.UI.FetchPanel<Panel_Main>().ShowTopMenu(false);
		GameManager.Scene.callback_CloseToast = () => GameManager.UI.FetchPanel<Panel_Main>().ShowTopMenu(true);

		GameManager.Scene.ShowToastAndDisappear($"You recevied {shopItemData.name}!!");
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
