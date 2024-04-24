using EnhancedScrollerDemos.GridSimulation;
using UnityEngine;
using UnityEngine.UI;

public class RowCellView_InvenItem : RowCellView
{
	InvenItemData invenItemData;

	Button btn_Container;
	Image img_Thumnail;

	public bool isChanged = false;

	private void Awake()
	{
		btn_Container = Util.FindButton(this.gameObject, nameof(btn_Container), OnClick_EquipItem, true);
		img_Thumnail = Util.FindImage(this.gameObject, nameof(img_Thumnail));
	}

	public override void SetData(EnhancedScrollerDemos.GridSimulation.Data data)
	{
		base.SetData(data);

		var invenData = data as InvenItemData;

		if(invenData != null)
		{
			if(invenData.thumbnail != null)
			{
				var sprite = Resources.Load<Sprite>(invenData.thumbnail);

				img_Thumnail.sprite = sprite;
			}
		}

		invenItemData = invenData;
	}

	private void OnClick_EquipItem()
	{
		if (invenItemData.isRide)
		{
			FindObjectOfType<RideController>().Init(invenItemData.nameIndex, invenItemData.index);
		}

		else
		{
			var equipment = new Equipment(invenItemData.type, invenItemData.nameIndex, invenItemData.index);

			FindObjectOfType<EquipmentManager>().PreviewEquipment(equipment, invenItemData.thumbnail);

			GameManager.UI.FetchPanel<Panel_Equipment>().SetEquippedThumbnail(invenItemData.type, invenItemData.thumbnail);
			GameManager.UI.FetchPanel<Panel_Equipment>().isChanged = true;

			if (LocalData.gameData.equipment.ContainsKey(invenItemData.type))
			{
				var type = LocalData.gameData.equipment[invenItemData.type].type;
				var nameIndex = LocalData.gameData.equipment[invenItemData.type].name;
				var index = LocalData.gameData.equipment[invenItemData.type].index;
				var thumbnail = LocalData.gameData.equipment[invenItemData.type].path.Replace("Assets/Resources/", "").Replace(".png", "");

				LocalData.gameData.equipment[invenItemData.type].type = invenItemData.type;
				LocalData.gameData.equipment[invenItemData.type].name = invenItemData.nameIndex;
				LocalData.gameData.equipment[invenItemData.type].index = invenItemData.index;
				LocalData.gameData.equipment[invenItemData.type].path = invenItemData.thumbnail;

				invenItemData.type = type;
				invenItemData.nameIndex = nameIndex;
				invenItemData.index = index;
				invenItemData.thumbnail = thumbnail;

				img_Thumnail.sprite = Resources.Load<Sprite>(invenItemData.thumbnail);

				FindObjectOfType<EquipmentManager>().previewSprite.Remove((int)invenItemData.type);
			}
			else
			{

			}
		}
	}
}
