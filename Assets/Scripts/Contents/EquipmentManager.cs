using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class EquipmentManager : MonoBehaviour
{
	#region Members

	public SerializableDictionary<EquipmentType, Equipment> equipments;
	public List<Equipment> preview = new List<Equipment>();
	public Dictionary<int, Sprite[]> previewSprite = new Dictionary<int, Sprite[]>();

	public GameObject actor;
	public SPUM_Prefabs spumPrefabs;

	#endregion



	#region Initialize

	private void Awake()
	{
		actor = GameObject.Find("PlayerActor");
		spumPrefabs = actor.GetComponent<SPUM_Prefabs>();
	}

	private void Start()
	{
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

			ResyncData();

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

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			ClearEquipmentAll();
		}
	}

	public void ChangeEquipment(Equipment equipment, bool resyncData = false)
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
		equipments.Clear();
		preview.Clear();
		previewSprite.Clear();

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

		if (resync) spumPrefabs._spriteOBj.ResyncData();
	}

	public void PreviewEquipment(Equipment equipment, string thumnailPath)
	{
		var spriteList = new List<SpriteRenderer>();
		var targetSprites = Resources.LoadAll<Sprite>(thumnailPath);
		var targetIndex = (int)equipment.type;

		switch (equipment.type)
		{
			case EquipmentType.Hair:
				spriteList = spumPrefabs._spriteOBj._hairList;
				break;
			case EquipmentType.FaceHair:
				spriteList = spumPrefabs._spriteOBj._hairList;
				break;
			case EquipmentType.Helmet:
				spriteList = spumPrefabs._spriteOBj._hairList;
				break;
			case EquipmentType.Cloth:
				spriteList = spumPrefabs._spriteOBj._clothList;
				break;
			case EquipmentType.Pant:
				spriteList = spumPrefabs._spriteOBj._pantList;
				break;
			case EquipmentType.Weapons:
				spriteList = spumPrefabs._spriteOBj._weaponList;
				break;
			case EquipmentType.Armor:
				spriteList = spumPrefabs._spriteOBj._armorList;
				break;
			case EquipmentType.Back:
				spriteList = spumPrefabs._spriteOBj._backList;
				break;
			default:
				break;
		}

		if (targetIndex == (int)EquipmentType.Hair)
		{
			if (!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[0].sprite });
			}

			spriteList[0].sprite = targetSprites[0];
		}

		else if (targetIndex == (int)EquipmentType.FaceHair)
		{
			if (!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[3].sprite });
			}

			spriteList[3].sprite = targetSprites[0];
		}

		else if (targetIndex == (int)EquipmentType.Helmet)
		{
			if (!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[1].sprite });
			}

			spriteList[1].sprite = targetSprites[0];
		}

		else if (targetIndex == (int)EquipmentType.Cloth)
		{
			previewSprite.Add(targetIndex, new Sprite[] { spriteList[0].sprite, spriteList[1].sprite, spriteList[2].sprite });

			spriteList[0].sprite = targetSprites[0];
			spriteList[1].sprite = targetSprites[1];
			spriteList[2].sprite = targetSprites[2];
		}

		else if (targetIndex == (int)EquipmentType.Pant)
		{
			if(!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[0].sprite, spriteList[1].sprite });
			}

			spriteList[0].sprite = targetSprites[0];
			spriteList[1].sprite = targetSprites[1];
		}

		else if (targetIndex == (int)EquipmentType.Weapons)
		{
			if (!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[0].sprite });
			}

			if (targetSprites[0].name.Contains("Shield"))
			{
				spriteList[3].sprite = targetSprites[0];
			}

			else
			{
				spriteList[0].sprite = targetSprites[0];
			}
			// Weapons
			//spriteList[0].sprite = targetSprites[0]; // right
			//spriteList[2].sprite = targetSprites[0]; // left

			// Shields
			// spriteList[1].sprite = targetSprites[0]; // right
			//spriteList[3].sprite = targetSprites[0]; // left
		}

		else if (targetIndex == (int)EquipmentType.Armor)
		{
			if (!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[0].sprite, spriteList[1].sprite, spriteList[2].sprite });
			}

			for(int i = 0; i < targetSprites.Length;i++)
			{
				spriteList[i].sprite = targetSprites[i];
			}
		}

		else if (targetIndex == (int)EquipmentType.Back)
		{
			if (!previewSprite.ContainsKey(targetIndex))
			{
				previewSprite.Add(targetIndex, new Sprite[] { spriteList[0].sprite });
			}

			spriteList[0].sprite = targetSprites[0];
		}

		preview.Add(equipment);
	}

	public void ResetPreview()
	{
		if (previewSprite.ContainsKey((int)EquipmentType.Hair))
		{
			spumPrefabs._spriteOBj._hairList[0].sprite = previewSprite[(int)EquipmentType.Hair][0];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.FaceHair))
		{
			spumPrefabs._spriteOBj._hairList[3].sprite = previewSprite[(int)EquipmentType.FaceHair][0];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.Helmet))
		{
			spumPrefabs._spriteOBj._hairList[1].sprite = previewSprite[(int)EquipmentType.Helmet][0];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.Cloth))
		{
			spumPrefabs._spriteOBj._clothList[0].sprite = previewSprite[(int)EquipmentType.Cloth][0];
			spumPrefabs._spriteOBj._clothList[1].sprite = previewSprite[(int)EquipmentType.Cloth][1];
			spumPrefabs._spriteOBj._clothList[2].sprite = previewSprite[(int)EquipmentType.Cloth][2];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.Pant))
		{
			spumPrefabs._spriteOBj._pantList[0].sprite = previewSprite[(int)EquipmentType.Pant][0];
			spumPrefabs._spriteOBj._pantList[1].sprite = previewSprite[(int)EquipmentType.Pant][1];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.Armor))
		{
			spumPrefabs._spriteOBj._armorList[0].sprite = previewSprite[(int)EquipmentType.Armor][0];
			spumPrefabs._spriteOBj._armorList[1].sprite = previewSprite[(int)EquipmentType.Armor][1];
			spumPrefabs._spriteOBj._armorList[2].sprite = previewSprite[(int)EquipmentType.Armor][2];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.Weapons))
		{
			spumPrefabs._spriteOBj._weaponList[0].sprite = previewSprite[(int)EquipmentType.Weapons][0];
		}

		if (previewSprite.ContainsKey((int)EquipmentType.Back))
		{
			spumPrefabs._spriteOBj._backList[0].sprite = previewSprite[(int)EquipmentType.Back][0];
		}

		previewSprite.Clear();
		preview.Clear();
	}

	public void ResyncData() => spumPrefabs._spriteOBj.ResyncData();

	public void EquipAll(Dictionary<EquipmentType, Equipment> equipments)
	{
		foreach (var item in equipments)
		{
			ChangeEquipment(item.Value, false);
		}

		ResyncData();
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