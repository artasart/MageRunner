using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Scene_Main : SceneLogic
{
	CinemachineVirtualCamera virtualCamera;
	EquipmentController equipmentController;

	private void OnDestroy()
	{
		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	private void OnDisable()
	{
		LocalData.gameData.equipment = equipmentController.equipments;
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

		virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_Main>(true);

		GameManager.UI.StackLastPopup<Popup_Basic>();

		PoolManager.SetPoolData("Puff", 10);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			GameManager.UI.FetchPanel<Panel_Main>().ShowRewardAd();
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			GameManager.UI.FetchPanel<Panel_Main>().HideRewardAd();
		}


		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			ZoomInCamera();
		}


		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			ZoomOutCamera();
		}
	}

	public void UpCamera()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .5f, -10f));
	}

	public void DownCamera()
	{
		Util.LerpPosition(virtualCamera.transform, new Vector3(0f, .4f, -10f));
	}

	public void ZoomInCamera()
	{
		Util.Zoom(virtualCamera, 1f, .05f);
	}

	public void ZoomOutCamera()
	{
		Util.Zoom(virtualCamera, 3f, .05f);
	}
}