using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class Popup_Settings : Popup_Base
{
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
