using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;
using DG.Tweening;

public class Item_StatUpgrade : Item_Base
{
	Button btn_Upgrade;
	TMP_Text txtmp_Name;
	TMP_Text txtmp_Rate;
	TMP_Text txtmp_Level;
	TMP_Text txtmp_Upgrade;

	Image img_Coin;

	public UpgradeType upgradeType;

	int level;

	protected override void Awake()
	{
		base.Awake();

		upgradeType = Util.ConvertIntToEnum<UpgradeType>(this.transform.GetSiblingIndex());

		btn_Upgrade = GetUI_Button(nameof(btn_Upgrade), OnClick_Upgarde, useAnimation: true);
		btn_Upgrade.onClick.RemoveListener(OpenSound);

		txtmp_Name = GetUI_TMPText(nameof(txtmp_Name), upgradeType.ToString());
		txtmp_Level = GetUI_TMPText(nameof(txtmp_Level), "Lv.1");
		txtmp_Upgrade = GetUI_TMPText(nameof(txtmp_Upgrade), "1,000");

		img_Coin = GetUI_Image(nameof(img_Coin));
	}

	private void Start()
	{
		txtmp_Rate = GetUI_TMPText(nameof(txtmp_Rate), GetCurrentValue().ToString("N0"));
	}


	public void Init()
	{
		switch (upgradeType)
		{
			case UpgradeType.Damage:
				txtmp_Level.text = "Lv." + LocalData.gameData.damageLevel;
				level = LocalData.gameData.damageLevel;
				break;
			case UpgradeType.Mana:
				txtmp_Level.text = "Lv." + LocalData.gameData.manaLevel;
				level = LocalData.gameData.manaLevel;
				break;
			case UpgradeType.Speed:
				txtmp_Level.text = "Lv." + LocalData.gameData.speedLevel;
				level = LocalData.gameData.speedLevel;
				break;
		}

		if (level == 10)
		{
			img_Coin.gameObject.SetActive(false);
			txtmp_Upgrade.text = "Max level";
			btn_Upgrade.interactable = false;
		}

		else
		{
			txtmp_Upgrade.text = GetUpgradeGold().ToString("N0");
		}
	}

	private void OnClick_Upgarde()
	{
		if (level == 10) return;

		if (LocalData.gameData.gold < GetUpgradeGold())
		{
			GameManager.Sound.PlaySound(Define.SOUND_DENIED);

			btn_Upgrade.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			return;
		}

		GameManager.Sound.PlaySound(Define.SOUND_OPEN);

		GameManager.UI.StackPopup<Popup_Basic>(true);

		GameManager.UI.FetchPopup<Popup_Basic>().SetPopupInfo(ModalType.ConfirmCancel, $"Do you want to upgrade <color=#FFC700>{upgradeType}</color>\n using {GetUpgradeGold()} Gold?", "Upgrade",
		() =>
		{
			GameManager.Scene.Dim(true);

			Invoke(nameof(UpgradeCompleted), .5f);
		},

		() =>
		{

		});
	}

	public int GetUpgradeGold()
	{
		int value = 0;

		switch (upgradeType)
		{
			case UpgradeType.Damage:
				value = LocalData.masterData.upgradeData[LocalData.gameData.damageLevel - 1].gold;
				break;
			case UpgradeType.Mana:
				value = LocalData.masterData.upgradeData[LocalData.gameData.manaLevel - 1].gold;
				break;
			case UpgradeType.Speed:
				value = LocalData.masterData.upgradeData[LocalData.gameData.speedLevel - 1].gold;
				break;
			default: break;
		}

		return value;
	}

	public int GetUpgradeAmount()
	{
		int value = 0;

		switch (upgradeType)
		{
			case UpgradeType.Damage:
				value = LocalData.masterData.upgradeData[LocalData.gameData.damageLevel - 1].damage;
				break;
			case UpgradeType.Mana:
				value = LocalData.masterData.upgradeData[LocalData.gameData.manaLevel - 1].mana;
				break;
			case UpgradeType.Speed:
				value = LocalData.masterData.upgradeData[LocalData.gameData.speedLevel - 1].speed;
				break;
			default: break;
		}

		return value;
	}


	private void UpgradeCompleted()
	{
		GameManager.Scene.ShowToastAndDisappear($"{upgradeType} Upgraded..!");

		GameManager.Scene.Dim(false);

		LocalData.gameData.gold -= GetUpgradeGold();

		GameManager.UI.FetchPanel<Panel_Main>().SetGoldUI(LocalData.gameData.gold);

		switch (upgradeType)
		{
			case UpgradeType.Damage:
				LocalData.gameData.damage += GetUpgradeAmount();
				LocalData.gameData.damageLevel++;
				txtmp_Rate.text = LocalData.gameData.damage.ToString("N0");
				txtmp_Level.text = "Lv." + LocalData.gameData.damageLevel.ToString();
				level = LocalData.gameData.damageLevel;
				break;
			case UpgradeType.Mana:
				LocalData.gameData.mana += GetUpgradeAmount();
				LocalData.gameData.manaLevel++;
				txtmp_Rate.text = LocalData.gameData.mana.ToString("N0");
				txtmp_Level.text = "Lv." + LocalData.gameData.manaLevel.ToString();
				level = LocalData.gameData.manaLevel;
				break;
			case UpgradeType.Speed:
				LocalData.gameData.speed += GetUpgradeAmount() * 0.01f;
				LocalData.gameData.speedLevel++;
				txtmp_Rate.text = LocalData.gameData.speed.ToString("N0");
				txtmp_Level.text = "Lv." + LocalData.gameData.speedLevel.ToString();
				level = LocalData.gameData.speedLevel;
				break;
			default: break;
		}

		GameScene.main.SaveData();


		if (level != 10) txtmp_Upgrade.text = GetUpgradeGold().ToString("N0");
		else
		{
			img_Coin.gameObject.SetActive(false);
			txtmp_Upgrade.text = "Max level";
			btn_Upgrade.interactable = false;
		}
	}

	public float GetCurrentValue()
	{
		float value = 0;

		switch (upgradeType)
		{
			case UpgradeType.Damage:
				value = LocalData.gameData.damage;
				break;
			case UpgradeType.Mana:
				value = LocalData.gameData.mana;
				break;
			case UpgradeType.Speed:
				value = LocalData.gameData.speed;
				break;
			default: break;
		}

		return value;
	}
}
