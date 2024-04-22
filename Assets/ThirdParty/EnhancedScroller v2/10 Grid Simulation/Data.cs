﻿using System;
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
	}
}