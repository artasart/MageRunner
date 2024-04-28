using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class Popup_Settings : Popup_Base
{
	//TMP_Text txtmp_NoticeOnOff;
	//TMP_Text txtmp_SavePowerOnOff;

	//Button btn_GameCenter;
	//Button btn_Leaderboard;
	//Button btn_Achieve;
	//Button btn_Coupon;

	//Toggle toggle_Notice;
	//Toggle toggle_SavePower;
	TMP_Text txtmp_PopupName;

	Slider slider_BGM;
	Slider slider_SFX;

	TMP_Text txtmp_BGMVolume;
	TMP_Text txtmp_SFXVolume;

	Color onColor;
	Color offColor;

	protected override void Awake()
	{
		base.Awake();

		//btn_GameCenter = GetUI_Button(nameof(btn_GameCenter), OnClick_GameCenter);
		//btn_Leaderboard = GetUI_Button(nameof(btn_Leaderboard), OnClick_LeaderBoard);
		//btn_Achieve = GetUI_Button(nameof(btn_Achieve), OnClick_Achieve);
		//btn_Coupon = GetUI_Button(nameof(btn_Coupon), OnClick_Coupon);

		//txtmp_NoticeOnOff = GetUI_TMPText(nameof(txtmp_NoticeOnOff), "ON");
		//txtmp_SavePowerOnOff = GetUI_TMPText(nameof(txtmp_SavePowerOnOff), "OFF");

		//toggle_Notice = GetUI_Toggle(nameof(toggle_Notice), true, OnValueChanged_Notice);
		//toggle_SavePower = GetUI_Toggle(nameof(toggle_SavePower), false, OnValueChanged_SavePower);


		txtmp_PopupName = GetUI_TMPText(nameof(txtmp_PopupName), "Setting");

		txtmp_SFXVolume = GetUI_TMPText(nameof(txtmp_SFXVolume), string.Empty);
		txtmp_BGMVolume = GetUI_TMPText(nameof(txtmp_BGMVolume), string.Empty);

		slider_BGM = GetUI_Slider(nameof(slider_BGM), OnValueChanged_BGM);
		slider_SFX = GetUI_Slider(nameof(slider_SFX), OnValueChanged_SFX);

		onColor = Util.HexToRGB("#03AC97");
		offColor = Util.HexToRGB("#6F604E");
	}

	private void Start()
	{
		slider_BGM.value = GameManager.Sound.bgmVolume;
		slider_SFX.value = GameManager.Sound.sfxVolume;

		txtmp_BGMVolume.text = (GameManager.Sound.bgmVolume * 100).ToString("N0");
		txtmp_SFXVolume.text = (GameManager.Sound.sfxVolume * 100).ToString("N0");
	}



	private void OnClick_GameCenter()
	{
		Debug.Log("GameCenter Clicked");
	}

	private void OnClick_LeaderBoard()
	{
		Debug.Log("LeaderBoard Clicked");
	}

	private void OnClick_Achieve()
	{
		Debug.Log("Achieve Clicked");
	}

	private void OnClick_Coupon()
	{
		Debug.Log("Coupon Clicked");
	}



	//private void OnValueChanged_Notice(bool isOn)
	//{
	//	UpdateToggleState(toggle_Notice, txtmp_NoticeOnOff, isOn);
	//}

	//private void OnValueChanged_SavePower(bool isOn)
	//{
	//	UpdateToggleState(toggle_SavePower, txtmp_SavePowerOnOff, isOn);
	//}

	private void UpdateToggleState(Toggle toggle, TMP_Text text, bool isOn)
	{
		Vector2 position = isOn ? new Vector2(-48.5f, 0.2f) : new Vector2(48.5f, 0.2f);
		TextAlignmentOptions alignment = isOn ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
		string stateText = isOn ? "ON" : "OFF";
		Color color = isOn ? onColor : offColor;

		toggle.GetComponent<RectTransform>().anchoredPosition = position;
		text.alignment = alignment;
		text.text = stateText;
		text.color = color;
	}



	private void OnValueChanged_BGM(float value)
	{
		GameManager.Sound.bgmVolume = value;

		GameManager.Sound.bgm.volume = value;

		txtmp_BGMVolume.text = (value * 100).ToString("N0");
	}

	private void OnValueChanged_SFX(float value)
	{
		GameManager.Sound.sfxVolume = value;

		GameManager.Sound.soundEffect.volume = value;

		txtmp_SFXVolume.text = (value * 100).ToString("N0");
	}
}
