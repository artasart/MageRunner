using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums
{
	public enum EquipmentType
	{
		Hair = 0,
		FaceHair = 1,
		Cloth = 2,
		Pant = 3,
		Helmet = 4,
		Armor = 5,
		Weapons = 6,
		Back = 7,
	}

	public enum EquipmentThumbnailType
	{
		Weapons = 0,
		Armor = 1,
		Back = 2,
		Pant = 3,
		Helmet = 4,
		Horse = 5,
		Ring = 6,
		Pet = 7,
		Cloth = 8,
	}
	public enum EquipmentCategoryType
	{
		All = -1,
		Weapons = 0,
		Armor = 1,
		Back = 2,
		Pant = 3,
		Helmet = 4,
		Horse = 5,
		Ring = 6,
		Pet = 7,
		Cloth = 8,
	}

	public enum GameState
	{
		Playing,
		Paused
	}

	public enum ShopMenu
	{
		Equipment,
		Horse,
		Resources
	}

	public enum UpgradeType
	{
		Damage,
		Mana,
		Speed,
	}
}
