using System;
using static Enums;

namespace EnhancedScrollerDemos.GridSimulation
{
	[Serializable]
	public class Data
	{
		public string message;
	}

	[Serializable]
	public class InvenItemData : Data
	{
		public EquipmentType type;
		public string name;
		public int index;

		public string thumbnail;
		public string nameIndex;
		public int quantity;

		public int price;

		public int spaceCount;

		public bool isRide;
		public Ride ride = new Ride("", 0);

		public InvenItemData(string name)
		{
			this.name = name;
		}
	}

	[Serializable]
	public class ShopItemData : Data
	{
		public string name;
		public int price;
		public int amount;
		public string thumbnailPath;
		public string type;

		public ShopItemData(string name, string type, int price, int description, string thumbnailPath)
		{
			this.name = name;
			this.price = price;
			this.type = type;
			this.amount = description;
			this.thumbnailPath = thumbnailPath;
		}
	}
}