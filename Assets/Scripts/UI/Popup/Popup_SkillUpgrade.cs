using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_SkillUpgrade : Popup_Base
{
	Transform group_PassiveSkills;

	Button[] passiveButtons;

	TMP_Text txtmp_Message;

	protected override void Awake()
	{
		base.Awake();

		group_PassiveSkills = this.transform.Search(nameof(group_PassiveSkills));

		passiveButtons = new Button[group_PassiveSkills.childCount];

		for (int i = 0; i < group_PassiveSkills.childCount; i++)
		{
			int index = i;
			passiveButtons[i] = group_PassiveSkills.GetChild(i).GetComponent<Button>();
			passiveButtons[i].onClick.AddListener(() => OnClick_Action(index));
			passiveButtons[i].onClick.AddListener(OpenSound);
			passiveButtons[i].UseAnimation();

			var sprite = Resources.Load<Sprite>(Define.PATH_HAND_SKILLS + Util.ConvertIntToEnum<Skills>(index).ToString());

			passiveButtons[i].transform.Search("img_Thumbnail").GetComponent<Image>().sprite = sprite;
		}

		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), string.Empty);
	}

	public void Test()
	{
		//int index = 0;

		//foreach (var item in passiveButtons)
		//{
		//	var level = LocalData.gameData.passiveSkills[Util.ConvertIntToEnum<Skills>(index)].level;

		//	if (level == LocalData.masterData.skillUpgradeData.Count)
		//	{
		//		item.interactable = false;

		//		// enable max Level
		//	}

		//	item.transform.Search("txtmp_SkillName").GetComponent<TMP_Text>().text = LocalData.masterData.skillData[index].name;
		//	item.transform.Search("txtmp_CurrentLevel").GetComponent<TMP_Text>().text = $"upgrade to <color=orange>Lv.{level+1}</color>";
		//	item.transform.Search("txtmp_UpgradeGold").GetComponent<TMP_Text>().text = LocalData.masterData.skillUpgradeData[level-1].upgradeGold.ToString();

		//	index++;
		//}
	}

	public void OnClick_Action(int index)
	{
		var type = Util.ConvertIntToEnum<Skills>(index);
		// var skill = LocalData.gameData.passiveSkills[type];
		// var upgradeData = LocalData.masterData.skillUpgradeData[skill.level];

		//CancelInvoke(nameof(Hide));

		//if (LocalData.gameData.gold >= Convert.ToInt32(upgradeData.upgradeGold))
		//{
		//	LocalData.gameData.gold -= Convert.ToInt32(upgradeData.upgradeGold);
		//	GameManager.UI.FetchPanel<Panel_Main>().SetGold(LocalData.gameData.gold);

		//	//skill.level++;
		//	// LocalData.gameData.passiveSkills[type] = skill;

		//	//if (skill.level == LocalData.masterData.skillUpgradeData.Count)
		//	//{
		//	//	passiveButtons[index].interactable = false;
		//	//}

		//	var currentLevelText = passiveButtons[index].transform.Search("txtmp_CurrentLevel").GetComponent<TMP_Text>();
		//	// currentLevelText.text = $"upgrade to <color=orange>Lv.{skill.level + 1}</color>";

		//	var upgradeGoldText = passiveButtons[index].transform.Search("txtmp_UpgradeGold").GetComponent<TMP_Text>();
		//	// upgradeGoldText.text = LocalData.masterData.skillUpgradeData[skill.level - 1].upgradeGold.ToString();

		//	JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);

		//	Debug.Log("Upgrade!!");
		//}

		//else
		//{
		//	Debug.Log("Not Enough Money..!");

		//	this.transform.Search("img_Panel").GetComponent<RectTransform>().DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false);
		//	txtmp_Message.text = "<color=red>not enough gold</color>";

		//	Invoke(nameof(Hide), .75f);
		//}
	}

	public float shakeDuration = 0.5f;
	public Vector3 shakeStrength = new Vector3(20, 0, 0);
	public int shakeVibrato = 15;
	public float shakeRandomness = 90;

	private void Hide()
	{
		txtmp_Message.text = string.Empty;
	}
}
