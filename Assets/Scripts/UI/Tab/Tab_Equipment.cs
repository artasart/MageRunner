using EnhancedScrollerDemos.GridSimulation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tab_Equipment : Tab_Base
{
	GameObject renderTextureCamrea;
	InfiniteInvenGridScroller infiniteGridScroller;

	protected override void Awake()
	{
		base.Awake();

		btn_Back = GetUI_Button(nameof(btn_Back), () => GameManager.UI.FetchPanel<Panel_Inventory>().CloseTab<Tab_Equipment>());

		renderTextureCamrea = GameObject.Find("RenderTextureCamrea");

		infiniteGridScroller = FindObjectOfType<InfiniteInvenGridScroller>();
	}

	List<InvenItemData> data = new List<InvenItemData>();

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			GenerateItem();
		}
	}

	private void GenerateItem()
	{
		data.Clear();

		for(int i = 0; i < 10; i++)
		{
			var invenItemData = new InvenItemData();
			invenItemData.name = "Random_Item_" + UnityEngine.Random.Range(0, 100);

			data.Add(invenItemData);
		}

		infiniteGridScroller.Refresh(data);
	}

	private void OnClick_Save()
	{
		Debug.Log("OnClick_Save");
	}

	private void SetRenderCameraY(float y)
	{
		var position = renderTextureCamrea.transform.position;

		renderTextureCamrea.transform.position = new Vector3(position.x, y, position.z);
	}
}
