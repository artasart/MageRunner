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

	//public Color outlineColor { get; set; }


	public bool isEmpty = false;
	#endregion


	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_ChangeEquipment, true);
		img_Thumbnail = Util.FindImage(this.gameObject, nameof(img_Thumbnail));

		//outlineColor = btn_Container.GetComponent<Outline>().effectColor;
	}

	//public void UseOutline(bool turnOn)
	//{
	//	if(turnOn)
	//	{
	//		btn_Container.GetComponent<Outline>().effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 1f);
	//	}

	//	else
	//	{
	//		btn_Container.GetComponent<Outline>().effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0f);
	//	}
	//}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		var invenData = data as InvenItemData;

		if (invenData != null)
		{
			if (invenData.isRide)
			{
				img_Thumbnail.sprite = Resources.Load<Sprite>(invenData.thumbnail);
			}

			else
			{
				isEmpty = invenData.name == "empty";

				var path = isEmpty ? Define.PATH_ICON + "Hand/trash" : invenData.thumbnail;
				img_Thumbnail.sprite = Resources.Load<Sprite>(path);

				if (isEmpty)
				{
					//UseOutline(false);
				}

				else
				{
					string playerEquipmnet = string.Empty;

					if (LocalData.gameData.equipment.ContainsKey(invenData.type))
					{
						playerEquipmnet = LocalData.gameData.equipment[invenData.type].name + "_" + LocalData.gameData.equipment[invenData.type].index;
					}

					string thisEquipment = invenData.nameIndex + "_" + invenData.index;

					if (playerEquipmnet == thisEquipment)
					{
						//UseOutline(true);

						//if (!GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.ContainsKey(invenData.type.ToString()))
						//{
						//	GameManager.UI.FetchPanel<Panel_Equipment>().selectedItem.Add(invenData.type.ToString(), this);
						//}
					}

					else
					{
						//UseOutline(false);
					}
				}
			}
		}

		invenItemData = invenData;
	}


	private void OnClick_ChangeEquipment()
	{
		if (invenItemData.isRide)
		{
			GameScene.main.rideManager.Ride();
			GameScene.main.rideManager.ChangeRide(invenItemData.name, invenItemData.index);

			GameManager.UI.FetchPanel<Panel_Equipment>().SetPlayerEquipSlot("Horse", invenItemData.thumbnail);
		}

		else
		{
			if (invenItemData.name == "empty")
			{
				GameScene.main.equipmentManager.spumPrefabs = GameScene.main.playerActor.GetComponent<SPUM_Prefabs>();

				if (GameManager.UI.FetchPanel<Panel_Equipment>().category == EquipmentCategoryType.All)
				{
					GameScene.main.equipmentManager.ClearEquipmentAll();

					GameManager.UI.FetchPanel<Panel_Equipment>().RemovePlayerEquipSlot(true);

					GameScene.main.rideManager.ClearEquipmentAll();
					GameScene.main.rideManager.RideOff();
				}

				else
				{
					string category = GameManager.UI.FetchPanel<Panel_Equipment>().category.ToString();

					GameScene.main.equipmentManager.ClearEquipment(Util.String2Enum<EquipmentType>(category));

					GameManager.UI.FetchPanel<Panel_Equipment>().RemovePlayerEquipSlot(false);
				}
			}

			GameScene.main.equipmentManager.ChangeEquipment(new Equipment(invenItemData.type, invenItemData.nameIndex, invenItemData.index), true);

			GameManager.UI.FetchPanel<Panel_Equipment>().SetPlayerEquipSlot(invenItemData.type.ToString(), invenItemData.thumbnail);
		}


		if(GameScene.main.rideManager.isRide)
		{
			GameScene.main.equipmentManager.spumPrefabs = GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>();
		}

		else
		{
			GameScene.main.equipmentManager.spumPrefabs = GameScene.main.playerActor.GetComponent<SPUM_Prefabs>();
		}

		GameScene.main.SaveData();
	}
}
