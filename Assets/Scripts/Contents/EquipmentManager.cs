using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Enums;

public class EquipmentManager : MonoBehaviour
{
	public SerializableDictionary<EquipmentType, Equipment> equipments;

	private GameObject actor;
	private SPUM_Prefabs spumPrefabs;

	public EquipmentType type;
	public string weaponName;
	public int weaponIndex;
	public bool isRight;

	#region Initialize

	private void Awake()
	{
		actor = GameObject.Find("DefaultActor");
		spumPrefabs = actor.GetComponent<SPUM_Prefabs>();
	}

	private void Start()
	{
		LocalData.gameData = JsonManager<GameData>.LoadData(Define.JSON_GAMEDATA);

		if (LocalData.gameData == null)
		{
			LocalData.gameData = new GameData();

			equipments = new SerializableDictionary<EquipmentType, Equipment>();

			ClearEquipmentAll();
		}

		else
		{
			equipments = new SerializableDictionary<EquipmentType, Equipment>();

			foreach (var item in LocalData.gameData.equipment)
			{
				var equipment = new Equipment(item.Value.type, item.Value.name, item.Value.index);

				ChangeEquipment(equipment);
			}

			if (!string.IsNullOrEmpty(LocalData.gameData.ride.name))
			{
				FindObjectOfType<RideController>().Init(LocalData.gameData.ride.name, LocalData.gameData.ride.index);
			}

			else
			{

			}
		}
	}

	#endregion



	#region Core methods

	public void ChangeEquipment(Equipment equipment)
	{
		if (!equipments.ContainsKey(equipment.type))
		{
			equipments.Add(equipment.type, equipment);
		}

		else
		{
			equipments[equipment.type].type = equipment.type;
			equipments[equipment.type].name = equipment.name;
			equipments[equipment.type].index = equipment.index;
		}

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

		spumPrefabs._spriteOBj.ResyncData();
	}

	public void ClearEquipment(EquipmentType type)
	{
		if (!equipments.ContainsKey(type))
		{
			equipments.Add(type, new Equipment(type, string.Empty, 0));
		}

		else
		{
			equipments[type].type = type;
			equipments[type].name = string.Empty;
			equipments[type].index = 0;
		}

		switch (type)
		{
			case EquipmentType.Hair:
				SetHair(string.Empty, 0);
				break;
			case EquipmentType.FaceHair:
				SetHair(string.Empty, 3);
				break;
			case EquipmentType.Cloth:
				SetCloth(string.Empty);
				break;
			case EquipmentType.Pant:
				SetPant(string.Empty);
				break;
			case EquipmentType.Helmet:
				SetHair(string.Empty, 1);
				break;
			case EquipmentType.Armor:
				SetArmor(string.Empty);
				break;
			case EquipmentType.Back:
				SetBack(string.Empty);
				break;
			default:
				break;
		}

		spumPrefabs._spriteOBj.ResyncData();
	}

	public void ClearEquipmentAll()
	{
		equipments.Clear();

		for (int i = 0; i < Util.GetEnumLength<EquipmentType>(); i++)
		{
			var type = Util.ConvertIntToEnum<EquipmentType>(i);

			equipments.Add(type, new Equipment(type, string.Empty, 0));
		}

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

	#endregion


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