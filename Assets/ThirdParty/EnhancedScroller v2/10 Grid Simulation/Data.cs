using System;

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
		public string name;
		public string thumbnail;
		public string description;
		public int quantity;
		public int price;
        public bool isNew;
        public int spaceCount;
	}
}