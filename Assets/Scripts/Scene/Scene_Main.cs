using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Main : SceneLogic
{
	EquipmentController equipmentController;

	private void OnDestroy()
	{
		LocalData.gameData.equipment = equipmentController.equipments;

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	protected override void Awake()
	{
		base.Awake();

		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);

		if (LocalData.masterData == null)
		{
			DebugManager.Log("WARNING!! NO MASTER DATA.", DebugColor.Data);

			GameManager.Scene.LoadScene(SceneName.Logo);

			PlayerPrefs.SetString("Version", string.Empty);

			return;
		}

		PoolManager.InitPool();


		equipmentController = FindObjectOfType<EquipmentController>();
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		//GameManager.UI.StartPanel<Panel_Main>(true);


		PoolManager.SetPoolData("Puff", 10);
	}
}