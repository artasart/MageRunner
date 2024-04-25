using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Skill : Popup_Base
{
	Item_SkillCard[] item_SkillCards;

	Transform group_Skills;

	TMP_Text txtmp_Message;

	Scene_Game game;

	Button btn_Refresh;

	private void OnDisable()
	{
		if (!isInitialized) { isInitialized = true; return; }

		game.gameState = GameState.Playing;

		game.player.ToggleSimulation(true);

		GameManager.UI.FetchPanel<Panel_HUD>().Show();

		btn_Refresh.interactable = true;

		if (!txtmp_Message) txtmp_Message.StopPingPong();
	}

	private void OnEnable()
	{
		if (!isInitialized) { return; }

		game.gameState = GameState.Paused;

		game.player.ToggleSimulation(false);

		GameManager.UI.FetchPanel<Panel_HUD>().Hide();
	}

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		group_Skills = this.transform.Search(nameof(group_Skills));

		item_SkillCards = new Item_SkillCard[group_Skills.childCount];

		for (int i = 0; i < group_Skills.childCount; i++)
		{
			item_SkillCards[i] = group_Skills.GetChild(i).GetComponent<Item_SkillCard>();
		}

		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "select active skill");
		txtmp_Message.UsePingPong();

		game = FindObjectOfType<Scene_Game>();

		btn_Refresh = GetUI_Button(nameof(btn_Refresh), OnClick_Refresh);
	}

	private void OnClick_Refresh()
	{
		SetCard();

		btn_Refresh.interactable = false;
	}

	public void SetCard()
	{
		var selectedSkills = LocalData.gameData.activeSkills
			.OrderBy(x => Guid.NewGuid())
			.Where(skill => skill.level != 5)
			.Take(3)
			.ToList();

		if(selectedSkills.Count == 0)
		{
			Debug.Log("Empty..");

			GameManager.UI.PopPopup(false);
		}

		for (int i = 0; i < selectedSkills.Count; i++)
		{
			item_SkillCards[i].SetCardInfo(selectedSkills[i], i);
		}

		for (int i = 3; i > selectedSkills.Count; i--)
		{
			item_SkillCards[i - 1].gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		txtmp_Message.StartPingPong();
	}


	public void SetOtherSmall(int index)
	{
		for(int i = 0; i < item_SkillCards.Length; i++)
		{
			if (item_SkillCards[i].selectedIndex == index) continue;

			item_SkillCards[i].SetSize();
		}
	}
}

public enum Skills
{
	Gold,
	Exp,
	Damage,
	Mana,
	CoolTime,
	Critical,
	Speed,

	PowerOverWhelming,
	Thunder,
	ShockWave,
	Electricute,
	Execution,
}

[Serializable]
public class PlayerSkill
{
	public string name;
	public string description;
	public string thumbnailPath;

	public PlayerSkill(string name, string description, string thumbnailPath)
	{
		this.name = name;
		this.description = description;
		this.thumbnailPath = thumbnailPath;
	}
}

[Serializable]
public class PlayerPassiveSkill : PlayerSkill
{
	public int level;

	public PlayerPassiveSkill(string name, string description, string thumbnailPath, int level) : base(name, description, thumbnailPath)
	{
		this.level = level;
	}
}

[Serializable]
public class ActiveSkill : PlayerSkill
{
	public Skills type;
	public int level;

	public ActiveSkill(string name, string description, string thumbnailPath, Skills skills) : base(name, description, thumbnailPath)
	{
		this.type = skills;
	}
}