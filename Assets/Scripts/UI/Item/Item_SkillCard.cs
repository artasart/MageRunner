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
	Image img_Outline;

	Button btn_ItemCard;

	Transform group_Upgrade;

	ActorSkill selectedSkill;

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
		img_Outline = GetUI_Image(nameof(img_Outline));

		group_Upgrade = this.transform.Search(nameof(group_Upgrade));

		originSizeDelta = this.GetComponent<RectTransform>().sizeDelta;

		canvasGroup = this.AddComponent<CanvasGroup>();
	}

	public void SetCardInfo(ActorSkill skill, int index)
	{
		txtmp_Name.text = skill.name;

		img_Thumbnail.sprite = Resources.Load<Sprite>(Define.PATH_ICON + skill.thumbnailPath);
		txtmp_Name.color = Util.HexToRGB("#DCDCDC");
		img_Outline.color = Util.HexToRGB("#B4B4B4");

		if (skill.maxLevel == 0)
		{
			txtmp_Description.text = skill.description + " " + skill.value;
		}

		else
		{
			int level = 0;

			if (Scene.game.actorSkills.ContainsKey(skill.name)) { level = Scene.game.actorSkills[skill.name].level; }

			if (skill.type == "passive")
			{
				txtmp_Description.text = skill.description + " " + Util.ParseStringToIntArray(skill.value)[level].ToString();
			}

			else
			{
				txtmp_Description.text = skill.description;

				txtmp_Name.color = Util.HexToRGB("#339CD2");
				img_Outline.color = Util.HexToRGB("#339CD2");
			}
		}



		group_Upgrade.gameObject.SetActive(skill.maxLevel != 0);

		for (int i = 0; i < group_Upgrade.childCount; i++)
		{
			group_Upgrade.GetChild(i).GetComponent<Image>().color = new Color(0f, 0f, 0f, .43f);
		}

		if (Scene.game.actorSkills.ContainsKey(skill.name))
		{
			for (int i = 0; i < Scene.game.actorSkills[skill.name].level; i++)
			{
				group_Upgrade.GetChild(i).GetComponent<Image>().color = Color.white;
			}
		}

		selectedIndex = index;
		selectedSkill = skill;

		btn_ItemCard.interactable = true;
	}

	private void Onclick_Select()
	{
		GameManager.Sound.PlaySound("Zap");

		Scene.game.AddSkill(selectedSkill);

		for (int i = 0; i < Scene.game.actorSkills[selectedSkill.name].level; i++)
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
		this.transform.GetComponent<RectTransform>().DOScale(Vector3.one * .85f, .25f);

		btn_ItemCard.interactable = false;
	}

	private void PopPopup()
	{
		particle.GetComponent<ParticleSystem>().Stop();
		particle.GetComponent<RePoolObject>().RePool();

		GameManager.UI.PopPopup();
	}
}
