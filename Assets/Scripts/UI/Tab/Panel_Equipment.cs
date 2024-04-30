using EnhancedScrollerDemos.GridSimulation;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;
using static UnityEditor.Progress;

public class Panel_Equipment : Panel_Base
{
	#region Members

	public EquipmentCategoryType category;

	InfiniteInvenGridScroller infiniteGridScroller;

	TMP_Text txtmp_Menu;
	TMP_Text txtmp_SpaceAmount;
	TMP_Text txtmp_DamageRate;
	TMP_Text txtmp_SpeedRate;

	Button btn_BuyStash;

	Button btn_Weapon;
	Button btn_Armor;
	Button btn_Cloth;
	Button btn_Helmet;
	Button btn_Pants;
	Button btn_Backs;
	Button btn_All;

	public SerializableDictionary<string, Image> thumbnails = new SerializableDictionary<string, Image>();

	Transform group_Normal;
	Transform group_Special;

	#endregion


	#region Initialize

	protected override void Awake()
	{
		base.Awake();

		infiniteGridScroller = FindObjectOfType<InfiniteInvenGridScroller>();

		txtmp_Menu = GetUI_TMPText(nameof(txtmp_Menu), "All");
		txtmp_SpaceAmount = GetUI_TMPText(nameof(txtmp_SpaceAmount), $"{0}/{100}");
		txtmp_DamageRate = GetUI_TMPText(nameof(txtmp_DamageRate), "1");
		txtmp_SpeedRate = GetUI_TMPText(nameof(txtmp_SpeedRate), "5");

		btn_Weapon = GetUI_Button(nameof(btn_Weapon), OnClick_Weapons, useAnimation: true);
		btn_Armor = GetUI_Button(nameof(btn_Armor), OnClick_Armor, useAnimation: true);
		btn_Helmet = GetUI_Button(nameof(btn_Helmet), OnClick_Helmet, useAnimation: true);
		btn_Cloth = GetUI_Button(nameof(btn_Cloth), OnClick_Cloth, useAnimation: true);
		btn_Pants = GetUI_Button(nameof(btn_Pants), OnClick_Pants, useAnimation: true);
		btn_Backs = GetUI_Button(nameof(btn_Backs), OnClick_Backs, useAnimation: true);
		btn_All = GetUI_Button(nameof(btn_All), OnClick_All, useAnimation: true);

		btn_BuyStash = GetUI_Button(nameof(btn_BuyStash), OnClick_BuyStash, useAnimation: true);

		group_Normal = this.transform.Search(nameof(group_Normal));
		group_Special = this.transform.Search(nameof(group_Special));

		int enumLength = Util.GetEnumLength<EquipmentThumbnailType>();

		for (int i = 0; i < enumLength - 4; i++)
		{
			var name = Util.ConvertIntToEnum<EquipmentThumbnailType>(i).ToString();

			thumbnails.Add(name, group_Normal.GetChild(i).GetChild(0).GetComponent<Image>());
		}

		for (int i = 0; i < 4; i++)
		{
			var name = Util.ConvertIntToEnum<EquipmentThumbnailType>(i + 5).ToString();

			thumbnails.Add(name, group_Special.GetChild(i).GetChild(0).GetComponent<Image>());
		}
	}

	private void Start()
	{
		txtmp_SpaceAmount.text = $"{LocalData.invenData.amount}/{LocalData.invenData.totalAmount}";
	}

	#endregion



	public void GenerateItem()
	{
		RemovePlayerEquipSlot(true);

		var equipment = Scene.main.equipmentManager.equipments.
			Where(item => item.Key != EquipmentType.Hair &&
						  item.Key != EquipmentType.FaceHair).ToList();

		foreach (var item in equipment)
		{
			if (!string.IsNullOrEmpty(item.Value.name))
			{
				thumbnails[item.Key.ToString()].sprite = Resources.Load<Sprite>(item.Value.path.Replace("Assets/Resources/", "").Replace(".png", ""));
				thumbnails[item.Key.ToString()].gameObject.SetActive(true);
			}

			else
			{
				thumbnails[item.Key.ToString()].gameObject.SetActive(false);
			}
		}

		if (!string.IsNullOrEmpty(LocalData.gameData.ride.name))
		{
			string filename = LocalData.gameData.ride.name + "_" + LocalData.gameData.ride.index;

			SetPlayerEquipSlot("Horse", $"Sprites/Ride/{filename}");

			thumbnails["Horse"].gameObject.SetActive(true);
		}

		else
		{
			thumbnails["Horse"].gameObject.SetActive(false);
		}

		OnClick_All();
	}

	private void OnClick_BuyStash()
	{
		GameManager.UI.StackPopup<Popup_Basic>(true);
		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"Do you want to buy stash?\n\ncost : <color=orange>{Scene.main.payAmount}</color>", "Purchase",
			() =>
			{
				if (LocalData.invenData.stashLevel > 10) return;

				if (LocalData.gameData.gold > Scene.main.payAmount)
				{
					LocalData.gameData.gold -= Scene.main.payAmount;
					LocalData.invenData.totalAmount += 10;
					LocalData.invenData.stashLevel++;

					txtmp_SpaceAmount.text = $"{LocalData.invenData.amount}/{LocalData.invenData.totalAmount}";

					GameManager.UI.FetchPanel<Panel_Main>().SetGold(LocalData.gameData.gold);
				}

				else
				{
					Debug.Log("Not Enough Money..!");
				}
			},
			() => { Debug.Log("Canceled.."); }
		);
	}

	private void OnClick_Weapons()
	{
		category = EquipmentCategoryType.Weapons;
		txtmp_Menu.text = "Weapon";

		var data = LocalData.invenData.invenItemData
					.Where(item => item.type == EquipmentType.Weapons)
					.ToList();
		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Armor()
	{
		category = EquipmentCategoryType.Armor;
		txtmp_Menu.text = "Armor";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Armor)
			.ToList();
		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Cloth()
	{
		category = EquipmentCategoryType.Cloth;
		txtmp_Menu.text = "Cloth";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Cloth)
			.ToList();
		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Helmet()
	{
		category = EquipmentCategoryType.Helmet;
		txtmp_Menu.text = "Helmet";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Helmet)
			.ToList();
		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Pants()
	{
		category = EquipmentCategoryType.Pant;
		txtmp_Menu.text = "Pants";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Pant)
			.ToList();
		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Backs()
	{
		category = EquipmentCategoryType.Back;
		txtmp_Menu.text = "Back";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Back)
			.ToList();
		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_All()
	{
		category = EquipmentCategoryType.All;
		txtmp_Menu.text = "All";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type != EquipmentType.Hair &&
						   item.type != EquipmentType.FaceHair)
			.ToList();

		var ride = LocalData.invenData.invenItemData.Where(item => item.isRide == true).ToList();

		for (int i = 0; i < ride.Count; i++)
		{
			data.Add(ride[i]);
		}

		data.Insert(0, new InvenItemData("empty"));

		infiniteGridScroller.Refresh(data);
	}


	public void SetPlayerEquipSlot(string type, string path)
	{
		if (!thumbnails.ContainsKey(type)) return;

		thumbnails[type].gameObject.SetActive(true);

		thumbnails[type].sprite = Resources.Load<Sprite>(path);
	}

	public void RemovePlayerEquipSlot(bool isAll = false)
	{
		if (isAll)
		{
			for (int i = 0; i < Util.GetEnumLength<EquipmentThumbnailType>(); i++)
			{
				var value = Util.ConvertIntToEnum<EquipmentThumbnailType>(i).ToString();

				thumbnails[value].gameObject.SetActive(false);
				thumbnails[value].sprite = null;
			}
		}

		else
		{
			thumbnails[category.ToString()].gameObject.SetActive(false);
		}
	}

	public void SetRideAbility(int damage, int speed)
	{
		txtmp_DamageRate.text = damage.ToString();
		txtmp_SpeedRate.text = speed.ToString();
	}
}
