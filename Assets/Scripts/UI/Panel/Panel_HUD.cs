using System;
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
		
		btn_Down = this.transform.Search(nameof(btn_Down)).GetComponent<Button>();
		btn_Down.UseAnimation();
		btn_Down.UseClickAction();
		btn_Down.GetComponent<ButtonClickAction>().action_PointerDown += OnClick_Down;

		btn_Up = this.transform.Search(nameof(btn_Up)).GetComponent<Button>();
		btn_Up.UseAnimation();
		btn_Up.UseClickAction();
		btn_Up.GetComponent<ButtonClickAction>().action_PointerDown += OnClick_Up;

		//btn_Down = GetUI_Button(nameof(btn_Down), OnClick_Down, useAnimation: true);
		//btn_Up = GetUI_Button(nameof(btn_Up), OnClick_Up, useAnimation: true);
		btn_Pause = GetUI_Button(nameof(btn_Pause), OnClick_Pause, useAnimation: true);

		group_Skills = this.transform.Search(nameof(group_Skills));

		btn_Up.onClick.RemoveListener(OpenSound);
		btn_Down.onClick.RemoveListener(OpenSound);

		slider_Level = GetUI_Slider(nameof(slider_Level));
	}

	private void Start()
	{
		slider_Level.value = 0f;
		slider_Level.maxValue = LocalData.masterData.inGameLevel[Scene.game.level - 1].exp;
		txtmp_Level = GetUI_TMPText(nameof(txtmp_Level), "Lv.1");

		items_Skill.Add(Skills.PowerOverWhelming, btn_Down.GetComponent<Item_Skill>());
	}

	private void OnClick_Left()
	{
		Debug.Log("OnClick_Left");
	}

	private void OnClick_Right()
	{
		Debug.Log("OnClick_Right");
	}

	public void OnClick_Down()
	{
		if (Scene.game.playerActor.isFlyMode) return;

		if (Scene.game.playerActor.isDead) return;

		if (isUsed.ContainsKey(Skills.PowerOverWhelming) && isUsed[Skills.PowerOverWhelming]) return;

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

		foreach (var item in items_Skill)
		{
			item.Value.Refresh();
		}
		slotSet.Clear();
		items_Skill.Clear();
		isUsed.Clear();
		slotIndex = 0;
		slider_Level.value = 0f;
		slider_Level.maxValue = LocalData.masterData.inGameLevel[0].exp;
		txtmp_Level.text = "Lv.1";

		items_Skill.Add(Skills.PowerOverWhelming, btn_Down.GetComponent<Item_Skill>());
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

			Invoke(nameof(OpenSKillPopup), .25f);
		}
	}

	public void OpenSKillPopup()
	{
		GameManager.UI.StartPopup<Popup_Skill>();

		GameManager.UI.FetchPopup<Popup_Skill>().SetCard();
	}

	public void UseSkill(Skills skillType, float coolTime, float delay = 0f, Action action = null)
	{
		Debug.Log("Skill is used : " + skillType.ToString());

		if (!isUsed.ContainsKey(skillType)) isUsed.Add(skillType, true);

		items_Skill[skillType].UseSkill(coolTime, () =>
		{
			action?.Invoke();

			isUsed[skillType] = false;
		});
	}

	int slotIndex = 0;

	public void SetSlot(Skills skillType)
	{
		if (slotSet.Contains(skillType)) return;

		slotSet.Add(skillType);

		var sprite = Resources.Load<Sprite>(Define.PATH_ICON + "HandDrawn/Icon_ItemIcon_Skull");

		group_Skills.GetChild(slotIndex).GetComponent<Item_Skill>().Refresh();
		group_Skills.GetChild(slotIndex).Search("img_Thumbnail").GetComponent<Image>().sprite = sprite;
		group_Skills.GetChild(slotIndex).Search("img_Thumbnail").gameObject.SetActive(true);

		items_Skill.Add(skillType, group_Skills.GetChild(slotIndex).GetComponent<Item_Skill>());

		slotIndex++;
	}

	List<Skills> slotSet = new List<Skills>();

	public Dictionary<Skills, bool> isUsed = new Dictionary<Skills, bool>();
}


public enum SkillType
{
	Passive_1 = 0,
	Passive_2,
	Passive_3,
}