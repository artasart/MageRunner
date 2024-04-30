using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class EquipmentController : MonoBehaviour
{
	private SPUM_Prefabs spumPrefabs;

	bool isInit = false;

	public Dictionary<string, int> monsters = new Dictionary<string, int>();

	private void OnEnable()
	{
		if (isInit) return;

		RefreshEquipment();

		isInit = true;
	}

	private void Awake()
	{
		spumPrefabs = this.GetComponent<SPUM_Prefabs>();

		monsters = new Dictionary<string, int> { { "Orc", 4 }, { "Skeleton", 1 }, { "Devil", 1 } };
	}

	public void ChangeSkin(string name, int index)
	{
		Object[] sprites = Resources.LoadAll($"SPUM/SPUM_Sprites/BodySource/Species/{name}/{name + "_" + index}", typeof(Sprite));

		spumPrefabs._spriteOBj._bodyList[0].sprite = (Sprite)sprites[5];
		spumPrefabs._spriteOBj._bodyList[1].sprite = (Sprite)sprites[2];
		spumPrefabs._spriteOBj._bodyList[2].sprite = (Sprite)sprites[0];
		spumPrefabs._spriteOBj._bodyList[3].sprite = (Sprite)sprites[1];
		spumPrefabs._spriteOBj._bodyList[4].sprite = (Sprite)sprites[3];
		spumPrefabs._spriteOBj._bodyList[5].sprite = (Sprite)sprites[4];
	}

	public void ChangeRandomSkin()
	{
		var element = monsters.OrderBy(x => UnityEngine.Random.value).First();

		var name = element.Key;
		var index = UnityEngine.Random.Range(1, element.Value + 1);

		ChangeSkin(name, index);
	}

	public void RefreshEquipment()
	{
		ClearEquipmentAll();

		ChangeRandomSkin();

		//GetRandomEquipmentType(EquipmentType.Hair);
		//GetRandomEquipmentType(EquipmentType.FaceHair);
		GetRandomEquipmentType(EquipmentType.Cloth);
		GetRandomEquipmentType(EquipmentType.Pant);
		GetRandomEquipmentType(EquipmentType.Helmet);
		GetRandomEquipmentType(EquipmentType.Armor);
		GetRandomEquipmentType(EquipmentType.Weapons);
		GetRandomEquipmentType(EquipmentType.Back);

		ResyncData();
	}

	public void GetRandomEquipmentType(EquipmentType type)
	{
		var filteredItems = LocalData.masterData.itemData
			.Where(item => item.rank == "Normal" && Util.StringToEnum<EquipmentType>(item.type) == type)
			.ToList();

		if (filteredItems.Count == 0)
		{
			Debug.Log("No items found matching.");

			return;
		}

		System.Random random = new System.Random();
		int randomIndex = random.Next(0, filteredItems.Count);
		var randomItem = filteredItems[randomIndex];

		ChangeEquipment(new Equipment(type, randomItem.filename));
	}

	public void ChangeEquipment(Equipment equipment, bool resyncData = false)
	{
		var path = equipment.GetPath();

		if (string.IsNullOrEmpty(equipment.name))
		{
			path = string.Empty;
		}

		switch (equipment.type)
		{
			case EquipmentType.Hair:
				SetHair(path, 0);
				break;
			case EquipmentType.FaceHair:
				SetHair(path, 3);
				break;
			case EquipmentType.Cloth:
				SetCloth(path);
				break;
			case EquipmentType.Pant:
				SetPant(path);
				break;
			case EquipmentType.Helmet:
				SetHair(path, 1);
				break;
			case EquipmentType.Weapons:
				SetWeapon(path, true);
				break;
			case EquipmentType.Armor:
				SetArmor(path);
				break;
			case EquipmentType.Back:
				SetBack(path);
				break;
			default:
				break;
		}

		if(resyncData) spumPrefabs._spriteOBj.ResyncData();
	}

	public void ClearEquipmentAll()
	{
		SetHair(string.Empty, 0);
		SetHair(string.Empty, 3);
		SetCloth(string.Empty);
		SetPant(string.Empty);
		SetHair(string.Empty, 1);
		SetArmor(string.Empty);
		SetBack(string.Empty);
		SetWeapon(string.Empty, true);

		spumPrefabs._spriteOBj.ResyncData();
	}

	public void ResyncData() => spumPrefabs._spriteOBj.ResyncData();


	#region Util

	private void SetHair(string path, int index)
	{
		spumPrefabs._spriteOBj._hairListString[index] = path;
	}

	private void SetCloth(string path)
	{
		for (int i = 0; i < 3; i++)
		{
			spumPrefabs._spriteOBj._clothListString[i] = path;
		}
	}

	private void SetPant(string path)
	{
		for (int i = 0; i < 2; i++)
		{
			spumPrefabs._spriteOBj._pantListString[i] = path;
		}
	}

	private void SetArmor(string path)
	{
		for (int i = 0; i < 3; i++)
		{
			spumPrefabs._spriteOBj._armorListString[i] = path;
		}
	}

	private void SetWeapon(string path, bool isRight)
	{
		var index = isRight ? 0 : 2;

		spumPrefabs._spriteOBj._weaponListString[index] = path;
	}

	private void SetBack(string path)
	{
		spumPrefabs._spriteOBj._backListString[0] = path;
	}

	#endregion
}
