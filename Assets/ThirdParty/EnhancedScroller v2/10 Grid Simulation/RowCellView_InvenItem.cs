using EnhancedScrollerDemos.GridSimulation;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class RowCellView_InvenItem : RowCellView
{
	#region Members

	InvenItemData invenItemData;

	Button btn_Container;
	Image img_Thumbnail;

	public Color outlineColor { get; set; }


	public bool isEmpty = false;
	#endregion


	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_ChangeEquipment, true);
		img_Thumbnail = Util.FindImage(this.gameObject, nameof(img_Thumbnail));

		outlineColor = btn_Container.GetComponent<Outline>().effectColor;
	}

	public void UseOutline(bool turnOn)
	{
		if(turnOn)
		{
			btn_Container.GetComponent<Outline>().effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 1f);
		}

		else
		{
			btn_Container.GetComponent<Outline>().effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0f);
		}
	}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		var invenData = data as InvenItemData;

		if (invenData != null)
		{
			isEmpty = invenData.name == "empty";

			var path = isEmpty ? Define.PATH_ICON + "Hand/trash" : invenData.thumbnail;
			img_Thumbnail.sprite = Resources.Load<Sprite>(path);

			if(isEmpty)
			{
				UseOutline(false);
			}

			else
			{
				string playerEquipmnet = string.Empty;

				if (LocalData.gameData.equipment.ContainsKey(invenData.type))
				{
					playerEquipmnet = LocalData.gameData.equipment[invenData.type].name + "_" + LocalData.gameData.equipment[invenData.type].index;
				}

				string thisEquipment = invenData.nameIndex + "_" + invenData.index;

				Debug.Log(playerEquipmnet);
				Debug.Log(thisEquipment);

				if (playerEquipmnet == thisEquipment)
				{
					Debug.Log("Is equipped Item..!");

					UseOutline(true);

					if (!GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.ContainsKey(invenData.type.ToString()))
					{
						GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.Add(invenData.type.ToString(), this);
					}
				}

				else
				{
					Debug.Log("Not equippeded");

					UseOutline(false);
				}
			}
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

				GameManager.UI.FetchPanel<Panel_Equipment>().RemovePlayerEquipSlot(true);
			}

			else
			{
				string category = GameManager.UI.FetchPanel<Panel_Equipment>().category.ToString();

				Scene.main.equipmentManager.ClearEquipment(Util.String2Enum<EquipmentType>(category));

				GameManager.UI.FetchPanel<Panel_Equipment>().RemovePlayerEquipSlot(false);
			}
		}

		else
		{
			if (GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.ContainsKey(invenItemData.type.ToString()))
			{
				GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem[invenItemData.type.ToString()].UseOutline(false);

				GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.Remove(invenItemData.type.ToString());
			}

			UseOutline(true);

			GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.Add(invenItemData.type.ToString(), this);
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
