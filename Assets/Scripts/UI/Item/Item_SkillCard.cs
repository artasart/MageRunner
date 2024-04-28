using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class Item_SkillCard : Item_Base
{
	TMP_Text txtmp_Name;
	TMP_Text txtmp_Description;
	Image img_Thumbnail;

	Button btn_ItemCard;

	Transform group_Upgrade;

	ActiveSkill selectedSkill;

	public float shakeDuration = 0.35f;
	public Vector3 shakeStrength = new Vector3(10, 10, 0);
	public int shakeVibrato = 40;
	public float shakeRandomness = 90;

	Vector2 originSizeDelta;
	public int selectedIndex = -1;

	GameObject particle;

	CanvasGroup canvasGroup;

	private void OnDisable()
	{
		this.GetComponent<RectTransform>().sizeDelta = originSizeDelta;
		this.GetComponent<RectTransform>().localScale = Vector3.one;

		canvasGroup.blocksRaycasts = true;
	}

	protected override void Awake()
	{
		base.Awake();

		btn_ItemCard = this.GetComponent<Button>();
		btn_ItemCard.onClick.AddListener(Onclick_Select);
		btn_ItemCard.onClick.AddListener(OpenSound);
		btn_ItemCard.UseAnimation();

		txtmp_Name = GetUI_TMPText(nameof(txtmp_Name), string.Empty);
		txtmp_Description = GetUI_TMPText(nameof(txtmp_Description), string.Empty);
		img_Thumbnail = GetUI_Image(nameof(img_Thumbnail), null);

		group_Upgrade = this.transform.Search(nameof(group_Upgrade));

		originSizeDelta = this.GetComponent<RectTransform>().sizeDelta;

		canvasGroup = this.AddComponent<CanvasGroup>();
	}

	public void SetCardInfo(ActiveSkill skill, int index)
	{
		selectedIndex = index;

		if (Scene.game.skills.ContainsKey(skill.type))
		{
			btn_ItemCard.interactable = Scene.game.skills[skill.type].level < 5 ? true : false;
		}

		else btn_ItemCard.interactable = true;

		if(skill.skilltype == "Passive")
		{
			group_Upgrade.gameObject.SetActive(false);
		}

		else group_Upgrade.gameObject.SetActive(true);

		txtmp_Name.text = skill.name;
		txtmp_Description.text = skill.description + AddSkillDescription(skill.type);
		img_Thumbnail.sprite = Resources.Load<Sprite>(skill.thumbnailPath);

		for (int i = 0; i < group_Upgrade.childCount; i++)
		{
			group_Upgrade.GetChild(i).GetComponent<Image>().color = new Color(0f, 0f, 0f, .43f);
		}

		if (Scene.game.skills.ContainsKey(skill.type))
		{
			for (int i = 0; i < Scene.game.skills[skill.type].level; i++)
			{
				group_Upgrade.GetChild(i).GetComponent<Image>().color = Color.white;
			}
		}

		selectedSkill = skill;
	}

	private void Onclick_Select()
	{
		GameManager.Sound.PlaySound("Zap");

		Scene.game.AddSkill(selectedSkill);

		for (int i = 0; i < Scene.game.skills[selectedSkill.type].level; i++)
		{
			group_Upgrade.GetChild(i).GetComponent<Image>().color = Color.white;
		}

		Invoke(nameof(PopPopup), .75f);

		particle = PoolManager.Spawn(Define.VFX_UI_ELECTRIC_MESH, new Vector3(0f, 0f, -1f), Quaternion.identity, this.transform);
		particle.transform.localScale = Vector3.one * 180f;
		particle.GetComponent<ParticleSystem>().Play();

		GameManager.UI.FetchPopup<Popup_Skill>().SetOtherSmall(selectedIndex);

		btn_ItemCard.interactable = false;
	}

	public void SetSize()
	{
		var size = this.transform.GetComponent<RectTransform>().sizeDelta;

		this.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .85f, .25f);

		btn_ItemCard.interactable = false;
	}

	private void PopPopup()
	{
		particle.GetComponent<ParticleSystem>().Stop();
		particle.GetComponent<RePoolObject>().RePool();

		GameManager.UI.PopPopup();
	}


	public string AddSkillDescription(Skills skillType)
	{
		var value = string.Empty;

		switch(skillType)
		{
			case Skills.Gold:
				value = (Scene.game.goldMultiplier + 1).ToString();
				break;
			case Skills.Exp:
				value = (Scene.game.expMultiplier + 1).ToString();
				break;
			case Skills.Damage:
				value = "5";
				break;
			case Skills.Mana:
				value = "10";
				break;
			case Skills.CoolTime:
				value = "10%";
				break;
			case Skills.Speed:
				value = "10%";
				break;
			default:
				break;
		}

		return value;
	}
}
