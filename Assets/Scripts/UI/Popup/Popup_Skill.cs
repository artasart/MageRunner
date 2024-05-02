using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enums;

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

		game.playerActor.ToggleSimulation(true);

		GameManager.UI.FetchPanel<Panel_HUD>().Show();

		btn_Refresh.interactable = true;

		if (!txtmp_Message) txtmp_Message.StopPingPong();
	}

	private void OnEnable()
	{
		if (!isInitialized) { return; }

		game.gameState = GameState.Paused;

		game.playerActor.ToggleSimulation(false);

		GameManager.UI.FetchPanel<Panel_HUD>().Hide();

		btn_Refresh.interactable = Scene.game.gold > 100;
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
		Scene.game.gold -= 100;

		SetCard();

		btn_Refresh.interactable = false;

		for (int i = 0; i < group_Skills.childCount; i++)
		{
			item_SkillCards[i].GetComponent<RectTransform>().DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false);
		}
	}

	public void ShakeCard(int index)
	{
		item_SkillCards[index].GetComponent<RectTransform>().DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false);
	}

	public float shakeDuration = 0.35f;
	public Vector3 shakeStrength = new Vector3(10, 10, 0);
	public int shakeVibrato = 40;
	public float shakeRandomness = 90;

	public void SetCard()
	{
		var actorSkills = LocalData.gameData.actorSkills
			.OrderBy(x => Guid.NewGuid())
			.Where(skill => skill.level != 5)
			.Take(3)
			.ToList();

		if (actorSkills.Count == 0)
		{
			DebugManager.Log("Card is now empty..", DebugColor.Game);

			GameManager.UI.PopPopup(false);
		}

		for (int i = 0; i < actorSkills.Count; i++)
		{
			item_SkillCards[i].SetCardInfo(actorSkills[i], i);
		}

		for (int i = 3; i > actorSkills.Count; i--)
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
			item_SkillCards[i].GetComponent<Button>().interactable = false;
			item_SkillCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
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

	Execution,
	Electricute,
	PowerOverWhelming,

	Thunder,
	ShockWave,
}

[Serializable]
public class ActorSkill
{
	public string name;
	public string type;
	public string description;
	public string thumbnailPath;
	public int maxLevel;
	public string value;

	public int level;

	public ActorSkill(string name, string type, string description, string thumbnailPath, int maxLevel, string value, int level)
	{
		this.name = name;
		this.type = type;
		this.description = description;
		this.thumbnailPath = thumbnailPath;
		this.maxLevel = maxLevel;
		this.value = value;
		this.level = level;
	}
}