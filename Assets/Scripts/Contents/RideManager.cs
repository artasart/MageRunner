using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RideManager : MonoBehaviour
{
	SPUM_SpriteList playerSpriteList;
	SPUM_SpriteList rideSpriteList;

	public bool isRide = false;

	private void Start()
	{
		playerSpriteList = GameScene.main.playerActor.GetComponentInChildren<SPUM_SpriteList>();
		rideSpriteList = GameScene.main.playerHorseActor.GetComponentInChildren<SPUM_SpriteList>();

		GameScene.main.playerHorseActor.transform.position = Vector3.zero;
		GameScene.main.playerHorseActor.SetActive(false);
	}

	public void ChangeRide(string rideName, int rideIndex)
	{
		GameScene.main.playerActor.gameObject.SetActive(false);

		var path = rideName + "" + rideIndex;

		SetRide($"Assets/SPUM/SPUM_Sprites/RideSource/{path}/{path}.png");
		SetRideBody($"SPUM/SPUM_Sprites/RideSource/{path}");

		LocalData.gameData.ride = new Ride(rideName, rideIndex);
	}

	public void Ride()
	{
		rideSpriteList._hairListString = playerSpriteList._hairListString;
		rideSpriteList._clothListString = playerSpriteList._clothListString;
		rideSpriteList._pantListString = playerSpriteList._pantListString;
		rideSpriteList._armorListString = playerSpriteList._armorListString;
		rideSpriteList._backListString = playerSpriteList._backListString;
		rideSpriteList._weaponListString = playerSpriteList._weaponListString;

		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj.ResyncData();
		GameScene.main.playerActor.gameObject.SetActive(false);
		GameScene.main.playerHorseActor.SetActive(true);
		GameScene.main.CameraUp();

		LocalData.gameData.ride = new Ride("Horse", 1);

		GameManager.UI.FetchPanel<Panel_Equipment>().SetRideAbility(51, 8);

		isRide = true;
	}

	public void RideOff()
	{
		PoolManager.Spawn("Puff", GameScene.main.playerActor.transform.position + Vector3.up * .5f, Quaternion.identity);

		GameScene.main.playerActor.gameObject.SetActive(true);
		GameScene.main.playerHorseActor.SetActive(false);

		LocalData.gameData.ride.name = string.Empty;
		LocalData.gameData.ride.index = 0;

		GameScene.main.CameraDown();

		GameManager.UI.FetchPanel<Panel_Equipment>().SetRideAbility(1, 5);

		isRide = false;
	}





	public void SetRide(string name)
	{
		GameScene.main.playerHorseActor.SetActive(false);

		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._horseString = name;
		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._anim = GameScene.main.playerHorseActor.transform.Search("HorseRoot").GetComponent<Animator>();
		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>().isRideHorse = true;
		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj.transform.SetParent(GameScene.main.playerHorseActor.transform.Search("Root"));
		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj.transform.localPosition = Vector3.zero;
		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj._spHorseSPList = GameScene.main.playerHorseActor.transform.Search("HorseRoot").GetComponent<SPUM_HorseSpriteList>();
		GameScene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj._spHorseString = name;

		GameScene.main.playerHorseActor.SetActive(true);
	}

	public void ClearEquipmentAll( )
	{
		ClearList(rideSpriteList._hairListString);
		ClearList(rideSpriteList._clothListString);
		ClearList(rideSpriteList._armorListString);
		ClearList(rideSpriteList._pantListString);
		ClearList(rideSpriteList._weaponListString);
		ClearList(rideSpriteList._backListString);

		rideSpriteList.ResyncData();
	}

	public static void ClearList(List<string> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = string.Empty;
		}
	}

	public void SetRideBody(string name)
	{
		SPUM_HorseSpriteList hST = GameScene.main.playerHorseActor.GetComponentInChildren<SPUM_HorseSpriteList>();

		Object[] sprites = Resources.LoadAll(name);

		for (var j = 0; j < sprites.Length; j++)
		{
			if (sprites[j].GetType() == typeof(Sprite))
			{
				Sprite tSP = (Sprite)sprites[j];
				switch (sprites[j].name)
				{
					case "Head":
						hST._spList[0].sprite = tSP;
						break;

					case "Neck":
						hST._spList[1].sprite = tSP;
						break;

					case "BodyFront":
						hST._spList[2].sprite = tSP;
						break;

					case "BodyBack":
						hST._spList[3].sprite = tSP;
						break;

					case "FootFrontTop":
						hST._spList[4].sprite = tSP;
						hST._spList[5].sprite = tSP;
						break;

					case "FootFrontBottom":
						hST._spList[6].sprite = tSP;
						hST._spList[7].sprite = tSP;
						break;

					case "FootBackTop":
						hST._spList[8].sprite = tSP;
						hST._spList[9].sprite = tSP;
						break;

					case "FootBackBottom":
						hST._spList[10].sprite = tSP;
						hST._spList[11].sprite = tSP;
						break;

					case "Tail":
						hST._spList[12].sprite = tSP;
						break;

					case "Acc":
						hST._spList[13].sprite = tSP;
						break;
				}
			}
		}
	}
}