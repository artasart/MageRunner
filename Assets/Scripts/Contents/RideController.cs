using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class RideController : MonoBehaviour
{
	[Header("Horse")]
	public string horseName = "Horse";
	public int index = 2;

	GameObject horseAvatar;
	GameObject defaultActor;

	SPUM_SpriteList actorSpriteList;
	SPUM_SpriteList rideSpriteList;
	SPUM_Prefabs spumPrefabs;
	AnimatorController controller;

	private void Awake()
	{
		defaultActor = GameObject.Find("PlayerActor");
		horseAvatar = GameObject.Find("PlayerHorseActor");
		controller = Resources.Load<AnimatorController>("SPUM/SPUM_Sprites/RideSource/Horse1/Animation/Horse1_Animation");
		
		spumPrefabs = horseAvatar.GetComponent<SPUM_Prefabs>();
		horseAvatar.transform.position = Vector3.zero;
	}

	private void Start()
	{
		horseAvatar.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			PoolManager.Spawn("Puff", defaultActor.transform.position + Vector3.up * .5f, Quaternion.identity);

			RideAndSave();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			PoolManager.Spawn("Puff", defaultActor.transform.position + Vector3.up * .5f, Quaternion.identity);

			defaultActor.gameObject.SetActive(false);

			RideAndSave(true);
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			RideOff();
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			PoolManager.Spawn("Puff", defaultActor.transform.position + Vector3.up * .5f, Quaternion.identity);

			ChangeRide(horseName, index);
		}
	}
	public void Init(string name, int index)
	{
		defaultActor.gameObject.SetActive(false);

		RideAndSave();

		ChangeRide(name, index);
	}

	public void ChangeRide(string rideName, int rideIndex)
	{
		var path = rideName + "" + rideIndex;

		SetRide($"Assets/SPUM/SPUM_Sprites/RideSource/{path}/{path}.png");
		SetRideBody($"Assets/SPUM/SPUM_Sprites/RideSource/{path}/{path}.png");

		LocalData.gameData.ride = new Ride();
		LocalData.gameData.ride.name = rideName;
		LocalData.gameData.ride.index = rideIndex;
	}

	public void RideOff()
	{
		PoolManager.Spawn("Puff", defaultActor.transform.position + Vector3.up * .5f, Quaternion.identity);

		defaultActor.gameObject.SetActive(true);
		horseAvatar.SetActive(false);

		LocalData.gameData.ride.name = string.Empty;
		LocalData.gameData.ride.index = 0;

		FindObjectOfType<Scene_Main>().DownCamera();
	}

	public void RideAndSave(bool isPreview = false)
	{
		defaultActor.gameObject.SetActive(false);
		horseAvatar.SetActive(true);

		actorSpriteList = defaultActor.GetComponentInChildren<SPUM_SpriteList>();
		rideSpriteList = horseAvatar.GetComponentInChildren<SPUM_SpriteList>();
		rideSpriteList._hairListString = actorSpriteList._hairListString;
		rideSpriteList._clothListString = actorSpriteList._clothListString;
		rideSpriteList._pantListString = actorSpriteList._pantListString;
		rideSpriteList._armorListString = actorSpriteList._armorListString;
		rideSpriteList._backListString = actorSpriteList._backListString;
		rideSpriteList._weaponListString = actorSpriteList._weaponListString;

		horseAvatar.GetComponentInChildren<Animator>().runtimeAnimatorController = controller;
		horseAvatar.GetComponent<SPUM_Prefabs>()._spriteOBj.ResyncData();

		if (isPreview) LocalData.gameData.ride = new Ride { name = name, index = index, };

		FindObjectOfType<Scene_Main>().UpCamera();
	}

	public void SetRide(string name)
	{
		horseAvatar.SetActive(false);

		spumPrefabs._horseString = name;
		spumPrefabs._anim = horseAvatar.transform.Search("HorseRoot").GetComponent<Animator>();
		spumPrefabs.isRideHorse = true;
		spumPrefabs._spriteOBj.transform.SetParent(horseAvatar.transform.Search("Root"));
		spumPrefabs._spriteOBj.transform.localPosition = Vector3.zero;
		spumPrefabs._spriteOBj._spHorseSPList = horseAvatar.transform.Search("HorseRoot").GetComponent<SPUM_HorseSpriteList>();
		spumPrefabs._spriteOBj._spHorseString = name;

		horseAvatar.SetActive(true);
	}

	public void SetRideBody(string name)
	{
		SPUM_HorseSpriteList hST = horseAvatar.GetComponentInChildren<SPUM_HorseSpriteList>();

		Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(name);
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