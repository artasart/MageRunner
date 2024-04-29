using UnityEditor;
using UnityEngine;

public class RideManager : MonoBehaviour
{
	[Header("Horse")]
	public string rideName = "Horse";
	public int index = 2;

	SPUM_SpriteList playerSpriteList;
	SPUM_SpriteList rideSpriteList;

	private void Start()
	{
		playerSpriteList = Scene.main.playerActor.GetComponentInChildren<SPUM_SpriteList>();
		rideSpriteList = Scene.main.playerHorseActor.GetComponentInChildren<SPUM_SpriteList>();

		Scene.main.playerHorseActor.transform.position = Vector3.zero;
		Scene.main.playerHorseActor.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Ride();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			RideOff();
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			Debug.Log("Change Ride");

			ChangeRide("Horse", 2);
		}
	}

	public void ChangeRide(string name, int index)
	{
		Scene.main.playerActor.gameObject.SetActive(false);

		var path = name + "" + index;

		SetRide($"Assets/SPUM/SPUM_Sprites/RideSource/{path}/{path}.png");
		SetRideBody($"SPUM/SPUM_Sprites/RideSource/{path}");

		LocalData.gameData.ride = new Ride { name = name, index = index, };
	}

	public void Ride()
	{
		PoolManager.Spawn("Puff", Scene.main.playerActor.transform.position + Vector3.up * .5f, Quaternion.identity);

		rideSpriteList._hairListString = playerSpriteList._hairListString;
		rideSpriteList._clothListString = playerSpriteList._clothListString;
		rideSpriteList._pantListString = playerSpriteList._pantListString;
		rideSpriteList._armorListString = playerSpriteList._armorListString;
		rideSpriteList._backListString = playerSpriteList._backListString;
		rideSpriteList._weaponListString = playerSpriteList._weaponListString;

		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj.ResyncData();
		Scene.main.playerActor.gameObject.SetActive(false);
		Scene.main.playerHorseActor.SetActive(true);
		Scene.main.CameraUp();

		LocalData.gameData.ride = new Ride { name = name, index = index, };
	}

	public void RideOff()
	{
		PoolManager.Spawn("Puff", Scene.main.playerActor.transform.position + Vector3.up * .5f, Quaternion.identity);

		Scene.main.playerActor.gameObject.SetActive(true);
		Scene.main.playerHorseActor.SetActive(false);

		LocalData.gameData.ride.name = string.Empty;
		LocalData.gameData.ride.index = 0;

		Scene.main.CameraDown();
	}





	public void SetRide(string name)
	{
		Scene.main.playerHorseActor.SetActive(false);

		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._horseString = name;
		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._anim = Scene.main.playerHorseActor.transform.Search("HorseRoot").GetComponent<Animator>();
		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>().isRideHorse = true;
		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj.transform.SetParent(Scene.main.playerHorseActor.transform.Search("Root"));
		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj.transform.localPosition = Vector3.zero;
		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj._spHorseSPList = Scene.main.playerHorseActor.transform.Search("HorseRoot").GetComponent<SPUM_HorseSpriteList>();
		Scene.main.playerHorseActor.GetComponent<SPUM_Prefabs>()._spriteOBj._spHorseString = name;

		Scene.main.playerHorseActor.SetActive(true);
	}

	public void SetRideBody(string name)
	{
		SPUM_HorseSpriteList hST = Scene.main.playerHorseActor.GetComponentInChildren<SPUM_HorseSpriteList>();

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