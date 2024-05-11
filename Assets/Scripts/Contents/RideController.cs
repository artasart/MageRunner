using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideController : MonoBehaviour
{
	public void Ride()
	{
		if (!string.IsNullOrEmpty(LocalData.gameData.ride.name))
		{
			var path = LocalData.gameData.ride.name + "" + LocalData.gameData.ride.index;

			SetRide($"Assets/SPUM/SPUM_Sprites/RideSource/{path}/{path}.png");
			SetRideBody($"SPUM/SPUM_Sprites/RideSource/{path}");
		}
	}

	public void SetRide(string name)
	{
		GameScene.game.playerActor.gameObject.SetActive(false);

		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>()._horseString = name;
		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>()._anim = GameScene.game.playerActor.transform.Search("UnitRoot").GetComponent<Animator>();
		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>().isRideHorse = true;
		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>()._spriteOBj.transform.SetParent(GameScene.game.playerActor.transform.Search("Root"));
		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>()._spriteOBj.transform.localPosition = Vector3.zero;
		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>()._spriteOBj._spHorseSPList = GameScene.game.playerActor.transform.Search("UnitRoot").GetComponent<SPUM_HorseSpriteList>();
		GameScene.game.playerActor.GetComponent<SPUM_Prefabs>()._spriteOBj._spHorseString = name;

		GameScene.game.playerActor.gameObject.SetActive(true);
	}

	public void SetRideBody(string name)
	{
		SPUM_HorseSpriteList hST = GameScene.game.playerActor.GetComponentInChildren<SPUM_HorseSpriteList>();

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
