using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class Panel_Equipment : Panel_Base
{
	InfiniteInvenGridScroller infiniteGridScroller;
	EquipmentManager eqipmentManager;

	TMP_Text txtmp_Menu;

	Button btn_Save;
	Button btn_Reset;

	Button btn_Weapon;
	Button btn_Armor;
	Button btn_Helmet;
	Button btn_Pants;
	Button btn_Backs;
	Button btn_All;

	Dictionary<EquipmentType, Image> menuThumbnail = new Dictionary<EquipmentType, Image>();

	public bool isChanged = false;

	protected override void Awake()
	{
		base.Awake();

		btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save, useAnimation: true);
		btn_Reset = GetUI_Button(nameof(btn_Reset), OnClick_Reset, useAnimation: true);

		txtmp_Menu = GetUI_TMPText(nameof(txtmp_Menu), "All");

		btn_Weapon = GetUI_Button(nameof(btn_Weapon), OnClick_Weapons, useAnimation: true);
		btn_Armor = GetUI_Button(nameof(btn_Armor), OnClick_Armor, useAnimation: true);
		btn_Helmet = GetUI_Button(nameof(btn_Helmet), OnClick_Helmet, useAnimation: true);
		btn_Pants = GetUI_Button(nameof(btn_Pants), OnClick_Pants, useAnimation: true);
		btn_Backs = GetUI_Button(nameof(btn_Backs), OnClick_Backs, useAnimation: true);
		btn_All = GetUI_Button(nameof(btn_All), OnClick_All, useAnimation: true);

		infiniteGridScroller = FindObjectOfType<InfiniteInvenGridScroller>();
		eqipmentManager = FindObjectOfType<EquipmentManager>();
				
		menuThumbnail.Add(EquipmentType.Weapons, this.transform.Search("img_Weapons").GetChild(0).GetComponent<Image>());
		menuThumbnail.Add(EquipmentType.Armor, this.transform.Search("img_Armor").GetChild(0).GetComponent<Image>());
		menuThumbnail.Add(EquipmentType.Back, this.transform.Search("img_Back").GetChild(0).GetComponent<Image>());
		menuThumbnail.Add(EquipmentType.Pant, this.transform.Search("img_Pants").GetChild(0).GetComponent<Image>());
		menuThumbnail.Add(EquipmentType.Helmet, this.transform.Search("img_Helmet").GetChild(0).GetComponent<Image>());
	}

	public void GenerateItem()
	{
		if (LocalData.gameData.equipment != null)
		{
			var equippedList = LocalData.gameData.equipment.Where(item => item.Key != EquipmentType.Cloth &&
																 item.Key != EquipmentType.Hair &&
																 item.Key != EquipmentType.FaceHair).ToList();

			foreach (var item in equippedList)
			{
				if (!string.IsNullOrEmpty(item.Value.name))
				{
					menuThumbnail[item.Key].sprite = Resources.Load<Sprite>(item.Value.path.Replace("Assets/Resources/", "").Replace(".png", ""));
					menuThumbnail[item.Key].color = Color.white;
				}

				else menuThumbnail[item.Key].color = new Color(1f, 1f, 1f, 0f);
			}
		}

		else LocalData.gameData.equipment = new SerializableDictionary<EquipmentType, Equipment>();


		var data = LocalData.invenData.invenItemData
			.Where(item => item.type != EquipmentType.Cloth &&
				   item.type != EquipmentType.Hair &&
				   item.type != EquipmentType.FaceHair &&
				   !LocalData.gameData.equipment.Any(equipmentItem =>
						equipmentItem.Value.name != string.Empty &&
						equipmentItem.Value.name == item.nameIndex))
			.ToList();


		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Save()
	{
		if (!isChanged) return;

		foreach (var item in eqipmentManager.preview)
		{
			eqipmentManager.ChangeEquipment(item);
		}

		eqipmentManager.ResyncData();

		eqipmentManager.preview.Clear();
		eqipmentManager.previewSprite.Clear();

		LocalData.gameData.equipment = eqipmentManager.equipments;

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);

		isChanged = false;
	}

	protected override void OnClick_Back()
	{
		if (isChanged)
		{
			GameManager.UI.StackPopup<Popup_Basic>(true);

			GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, "Do you want to exit without saving the changes?");

			GameManager.UI.FetchPopup<Popup_Basic>().callback_confirm = () =>
			{
				FindObjectOfType<EquipmentManager>().ResetPreview();

				GameManager.UI.PopPanel();

				isChanged = false;
			};

			return;
		}

		base.OnClick_Back();
	}

	private void OnClick_Reset()
	{
		if (!isChanged) return;

		Debug.Log("Reset");

		FindObjectOfType<EquipmentManager>().ResetPreview();

		isChanged = false;
	}

	private void OnClick_Weapons()
	{
		txtmp_Menu.text = "Weapon";

		var data = LocalData.invenData.invenItemData
					.Where(item => item.type == EquipmentType.Weapons)
					.ToList();

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Armor()
	{
		txtmp_Menu.text = "Armor";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Armor)
			.ToList();

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Helmet()
	{
		txtmp_Menu.text = "Helmet";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Helmet)
			.ToList();

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Pants()
	{
		txtmp_Menu.text = "Pants";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Pant)
			.ToList();

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Backs()
	{
		txtmp_Menu.text = "Back";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type == EquipmentType.Back)
			.ToList();

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_All()
	{
		txtmp_Menu.text = "All";

		var data = LocalData.invenData.invenItemData
			.Where(item => item.type != EquipmentType.Cloth &&
						   item.type != EquipmentType.Hair &&
						   item.type != EquipmentType.FaceHair)
			.ToList();

		infiniteGridScroller.Refresh(data);
	}

	public void SetEquippedThumbnail(EquipmentType type, string path)
	{
		menuThumbnail[type].sprite = Resources.Load<Sprite>(path);
		menuThumbnail[type].color = Color.white;
	}
}
