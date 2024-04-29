using EnhancedScrollerDemos.GridSimulation;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class RowCellView_InvenItem : RowCellView
{
	#region Members

	InvenItemData invenItemData;

	Button btn_Container;
	Image img_Thumnail;

	public bool isChanged = false;

	#endregion


	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_ChangeEquipment, true);
		img_Thumnail = Util.FindImage(this.gameObject, nameof(img_Thumnail));
	}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		var invenData = data as InvenItemData;

		if (invenData != null)
		{
			var path = invenData.name == "empty" ? Define.PATH_ICON + "Hand/trash" : invenData.thumbnail;

			img_Thumnail.sprite = Resources.Load<Sprite>(path);
		}

		invenItemData = invenData;
	}


	private void OnClick_ChangeEquipment()
	{
		if (invenItemData.name == "empty")
		{
			if (GameManager.UI.FetchPanel<Panel_Equipment>().category == EquipmentCategoryType.All)
			{
				Scene.main.equipmentManager.ClearEquipmentAll();
			}

			else
			{
				string category = GameManager.UI.FetchPanel<Panel_Equipment>().category.ToString();

				Scene.main.equipmentManager.ClearEquipment(Util.String2Enum<EquipmentType>(category));
			}
		}

		Scene.main.equipmentManager.ChangeEquipment(new Equipment(invenItemData.type, invenItemData.nameIndex, invenItemData.index), true);

		GameManager.UI.FetchPanel<Panel_Equipment>().SetPlayerEquipSlot(invenItemData.type.ToString(), invenItemData.thumbnail);


		//if (invenItemData.isRide)
		//{
		//	Scene.main.rideController.Init(invenItemData.nameIndex, invenItemData.index);
		//}

		//else
		//{
		//	var equipment = new Equipment(invenItemData.type, invenItemData.nameIndex, invenItemData.index);

		//	Scene.main.equipmentManager.PreviewEquipment(equipment, invenItemData.thumbnail);

		//	GameManager.UI.FetchPanel<Panel_Equipment>().SetEquippedThumbnail(invenItemData.type, invenItemData.thumbnail);
		//	GameManager.UI.FetchPanel<Panel_Equipment>().isChanged = true;

		//	if (LocalData.gameData.equipment.ContainsKey(invenItemData.type))
		//	{
		//		var type = LocalData.gameData.equipment[invenItemData.type].type;
		//		var nameIndex = LocalData.gameData.equipment[invenItemData.type].name;
		//		var index = LocalData.gameData.equipment[invenItemData.type].index;
		//		var thumbnail = LocalData.gameData.equipment[invenItemData.type].path.Replace("Assets/Resources/", "").Replace(".png", "");

		//		LocalData.gameData.equipment[invenItemData.type].type = invenItemData.type;
		//		LocalData.gameData.equipment[invenItemData.type].name = invenItemData.nameIndex;
		//		LocalData.gameData.equipment[invenItemData.type].index = invenItemData.index;
		//		LocalData.gameData.equipment[invenItemData.type].path = invenItemData.thumbnail;

		//		invenItemData.type = type;
		//		invenItemData.nameIndex = nameIndex;
		//		invenItemData.index = index;
		//		invenItemData.thumbnail = thumbnail;

		//		img_Thumnail.sprite = Resources.Load<Sprite>(invenItemData.thumbnail);

		//		Scene.main.equipmentManager.previewSprite.Remove((int)invenItemData.type);
		//	}
		//	else
		//	{

		//	}
		//}
	}
}
