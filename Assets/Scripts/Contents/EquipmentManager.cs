using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class EquipmentManager : MonoBehaviour
{
	#region Members

	public SerializableDictionary<EquipmentType, Equipment> equipments { get; private set; }

	SPUM_Prefabs spumPrefabs;

	#endregion



	#region Initialize

	private void Start()
	{
		if (Scene.main != null)
		{
			spumPrefabs = Scene.main.playerActor.GetComponent<SPUM_Prefabs>();
		}
		if (Scene.game != null)
		{
			spumPrefabs = Scene.game.playerActor.GetComponent<SPUM_Prefabs>();
		}

		equipments = new SerializableDictionary<EquipmentType, Equipment>();

		ClearEquipmentAll(true);

		foreach (var item in LocalData.gameData.equipment)
		{
			if (item.Value.name == string.Empty) continue;

			ChangeEquipment(new Equipment(item.Value.type, item.Value.name, item.Value.index));
		}

		ResyncData();
	}

	#endregion



	#region Core methods

	public void ChangeEquipment(Equipment equipment, bool resyncData = false)
	{
		DebugManager.Log($"Change Equipment -> {equipment.name}", DebugColor.Game);

		if (!equipments.ContainsKey(equipment.type))
		{
			equipments.Add(equipment.type, equipment);
		}

		else
		{
			equipments[equipment.type].type = equipment.type;
			equipments[equipment.type].name = equipment.name;
			equipments[equipment.type].index = equipment.index;
			equipments[equipment.type].path = equipment.path;
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
				SetWeapon(path, !equipment.name.Contains("Shield"));
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

		if (resyncData) spumPrefabs._spriteOBj.ResyncData();
	}

	public void ClearEquipment(EquipmentType type)
	{
		DebugManager.Log($"Clear Equipment -> {type}", DebugColor.Game);

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

	public void ClearEquipmentAll(bool resync = false)
	{
		DebugManager.Log("Clear Equipment All", DebugColor.Game);

		equipments.Clear();

		for (int i = 0; i < Util.GetEnumLength<EquipmentType>(); i++)
		{
			equipments.Add(Util.ConvertIntToEnum<EquipmentType>(i), new Equipment(Util.ConvertIntToEnum<EquipmentType>(i), string.Empty, 0));
		}

		SetHair(string.Empty, 0);
		SetHair(string.Empty, 3);
		SetCloth(string.Empty);
		SetPant(string.Empty);
		SetHair(string.Empty, 1);
		SetArmor(string.Empty);
		SetBack(string.Empty);
		SetWeapon(string.Empty, true);

		if (resync) spumPrefabs._spriteOBj.ResyncData();
	}

	public void EquipAll(Dictionary<EquipmentType, Equipment> equipments)
	{
		foreach (var item in equipments)
		{
			if (item.Value.name == string.Empty) continue;

			ChangeEquipment(item.Value, false);
		}

		ResyncData();
	}

	public void ResyncData() => spumPrefabs._spriteOBj.ResyncData();

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