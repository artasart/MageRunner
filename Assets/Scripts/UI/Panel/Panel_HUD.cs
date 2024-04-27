using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

public class Panel_HUD : Panel_Base
{
	Button btn_Left;
	Button btn_Right;
	Button btn_Down;
	Button btn_Up;

	Button btn_Pause;

	TMP_Text txtmp_Coin;
	TMP_Text txtmp_Score;

	Transform group_Skills;
	Dictionary<Skills, Item_Skill> items_Skill = new Dictionary<Skills, Item_Skill>();

	Slider slider_Level;
	TMP_Text txtmp_Level;



	protected override void Awake()
	{
		base.Awake();

		txtmp_Coin = GetUI_TMPText(nameof(txtmp_Coin), "0");
		txtmp_Score = GetUI_TMPText(nameof(txtmp_Score), "0");

		btn_Left = GetUI_Button(nameof(btn_Left), OnClick_Left, useAnimation: true);
		btn_Right = GetUI_Button(nameof(btn_Right), OnClick_Right, useAnimation: true);
		btn_Down = GetUI_Button(nameof(btn_Down), OnClick_Down, useAnimation: true);
		btn_Up = GetUI_Button(nameof(btn_Up), OnClick_Up, useAnimation: true);
		btn_Pause = GetUI_Button(nameof(btn_Pause), OnClick_Pause, useAnimation: true);

		group_Skills = this.transform.Search(nameof(group_Skills));

		items_Skill.Add(Skills.PowerOverWhelming, btn_Down.GetComponent<Item_Skill>());

		//for (int i = 0; i < group_Skills.childCount; i++)
		//{
		//	items_Skill.Add(group_Skills.GetChild(i).GetComponent<Item_Skill>());
		//}

		btn_Up.onClick.RemoveListener(OpenSound);
		btn_Down.onClick.RemoveListener(OpenSound);

		slider_Level = GetUI_Slider(nameof(slider_Level));
	}

	private void Start()
	{
		items_Skill[Skills.PowerOverWhelming].Refresh();
		
		slider_Level.value = 0f;
		slider_Level.maxValue = LocalData.masterData.inGameLevel[Scene.game.level - 1].exp;
		txtmp_Level = GetUI_TMPText(nameof(txtmp_Level), "Lv.1");
	}

	private void OnClick_Left()
	{
		Debug.Log("OnClick_Left");
	}

	private void OnClick_Right()
	{
		Debug.Log("OnClick_Right");
	}

	private void OnClick_Down()
	{
		Debug.Log("OnClick_Down");

		UseSkill(Skills.PowerOverWhelming, 10);

		FindObjectOfType<PlayerActor>().StartFly();
	}

	private void OnClick_Up()
	{
		Debug.Log("OnClick_Up");

		FindObjectOfType<PlayerActor>().Jump();
	}

	private void OnClick_Pause()
	{
		Hide();

		FindObjectOfType<Scene_Game>().gameState = GameState.Paused;

		GameManager.UI.StackPopup<Popup_Pause>(true);
	}

	public void Refresh()
	{
		txtmp_Score.text = 0.ToString("N0");
		txtmp_Coin.text = 0.ToString("N0");

		items_Skill[Skills.PowerOverWhelming].Refresh();

		slider_Level.value = 0f;
		slider_Level.maxValue = LocalData.masterData.inGameLevel[0].exp;
		txtmp_Level.text = "Lv.1";
	}

	public void SetScoreUI(int amount)
	{
		txtmp_Score.text = amount.ToString("N0");
	}

	public void SetCoinUI(int amount)
	{
		txtmp_Coin.text = amount.ToString("N0");
	}

	public void SetExpUI(int amount)
	{
		if (slider_Level.value + amount < slider_Level.maxValue)
		{
			slider_Level.value += amount;
		}

		else
		{
			int leftOver = (int)slider_Level.maxValue - (int)slider_Level.value;

			amount -= leftOver;

			slider_Level.value = amount;

			Scene.game.level++;

			slider_Level.maxValue = (int)LocalData.masterData.inGameLevel[Scene.game.level - 1].exp;

			txtmp_Level.text = "Lv." + Scene.game.level.ToString();

			if (Scene.game.level == 30) slider_Level.value = slider_Level.maxValue;

			Scene.game.playerActor.AddDamage(5);

			GameManager.UI.StartPopup<Popup_Skill>();

			GameManager.UI.FetchPopup<Popup_Skill>().SetCard();
		}
	}

	public void UseSkill(Skills skillType, int coolTime, float delay = 0f)
	{
		btn_Down.enabled = false;

		Debug.Log("Skill is used : " + skillType.ToString());

		items_Skill[skillType].UseSkill(coolTime, () => btn_Down.enabled = true);
	}
}


public enum SkillType
{
	Passive_1 = 0,
	Passive_2,
	Passive_3,
}