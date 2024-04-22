using EnhancedScrollerDemos.GridSimulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class Tab_Equipment : Tab_Base
{
	InfiniteInvenGridScroller infiniteGridScroller;
	Button btn_Save;
	Button btn_Reset;

	public bool isChanged = false;

	protected override void Awake()
	{
		base.Awake();

		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);
		btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save);
		btn_Reset = GetUI_Button(nameof(btn_Reset), OnClick_Reset);

		infiniteGridScroller = FindObjectOfType<InfiniteInvenGridScroller>();
	}

	public void GenerateItem()
	{
		var data = FindObjectOfType<Scene_Main>().equipmentData;

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Save()
	{
		isChanged = false;
		Debug.Log("OnClick_Save");

		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	private void OnClick_Back()
	{
		if(isChanged)
		{
			Debug.Log("변경사항이 있습니다. 저장안하시고 나가겠어요?");

			return;
		}

		GameManager.UI.FetchPanel<Panel_Inventory>().CloseTab<Tab_Equipment>();
	}

	private void OnClick_Reset()
	{
		Debug.Log("Reset");

		FindObjectOfType<EquipmentManager>().ClearEquipmentAll();

		isChanged = false;
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Debug.Log("아바타 저장 완료!");

			isChanged = false;
		}
	}
}
