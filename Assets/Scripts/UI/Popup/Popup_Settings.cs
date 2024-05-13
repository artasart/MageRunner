using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class Popup_Settings : Popup_Base
{
	TMP_Text txtmp_PopupName;

	Slider slider_BGM;
	Slider slider_SFX;

	TMP_Text txtmp_BGMVolume;
	TMP_Text txtmp_SFXVolume;

	Color onColor;
	Color offColor;

	Button btn_SignOut;
	Button btn_Sound;
	Button btn_License;
	Button btn_LogOut;

	Transform group_Sound;
	Transform group_License;

	public LoginType loginType;

	private void OnEnable()
	{
		OnClick_Sound();
	}

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

		btn_SignOut = GetUI_Button(nameof(btn_SignOut), OnClick_SignOut, useAnimation: true);
		btn_LogOut = GetUI_Button(nameof(btn_LogOut), OnClick_LogOut, useAnimation: true);

		btn_License = GetUI_Button(nameof(btn_License), OnClick_License, useAnimation: true);
		btn_Sound = GetUI_Button(nameof(btn_Sound), OnClick_Sound, useAnimation: true);

		group_Sound = this.transform.Search(nameof(group_Sound));
		group_License = this.transform.Search(nameof(group_License));
	}

	private void Start()
	{
		loginType = Util.String2Enum<LoginType>(PlayerPrefs.GetString(Define.LOGINTYPE));

		slider_BGM.value = GameManager.Sound.bgmVolume;
		slider_SFX.value = GameManager.Sound.sfxVolume;

		txtmp_BGMVolume.text = (GameManager.Sound.bgmVolume * 100).ToString("N0");
		txtmp_SFXVolume.text = (GameManager.Sound.sfxVolume * 100).ToString("N0");
	}

	private void OnClick_LogOut()
	{
		GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(
			ModalType.ConfirmCancel,
			$"Do you want to sign out account?\n", "Notice",
			() =>
			{
				LocalData.gameData.isAdWatched = false;
				LocalData.gameData.adWatchTime = System.DateTime.Now.AddSeconds(90).ToString("yyyy-MM-ddTHH:mm:ss");

				if (loginType == LoginType.Google)
				{
					FindObjectOfType<GoogleLoginManager>().SignOutGoogleLogin();
				}

				else if (loginType == LoginType.Apple)
				{
					PlayerPrefs.SetString(Define.LOGINTYPE, string.Empty);
					PlayerPrefs.DeleteKey(Define.APPLEUSERID);

					GameManager.Scene.LoadScene(SceneName.Logo);
				}
			}
		);
	}

	private void OnClick_SignOut()
	{
		GameManager.UI.StackPopup<Popup_Basic>(true).SetPopupInfo(ModalType.ConfirmCancel,
			$"Do you really want to <color=#FFC700>withdraw account</color>?\n\n" +
			$"<size=30><color=#A90000>your account and personal data will be deleted.</size></color>",
			"Notice",
			() =>
			{
				GameManager.Scene.Dim(true);

				LocalData.gameData.nickname = string.Empty;
				LocalData.gameData.runnerTag = 1;
				LocalData.InitGameData();
				LocalData.InitInvenData();

				GameScene.main.SaveData();

				if (loginType == LoginType.Apple)
				{
					PlayerPrefs.DeleteKey(Define.APPLEUSERID);

					GameManager.UI.FetchPanel<Panel_Main>().GetComponent<CanvasGroup>().blocksRaycasts = false;
				}

				GameManager.Backend.WithdrawAccount();

				Invoke(nameof(QuitApp), .75f);
			});
	}

	private void QuitApp()
	{
		GameManager.Scene.Dim(false);

		GameManager.UI.PopPopup(true);

		PlayerPrefs.SetString(Define.LOGINTYPE, string.Empty);

		if (loginType == LoginType.Apple) GetComponent<AppleRevoker>().Revoke();

		else if (loginType == LoginType.Google) FindObjectOfType<GoogleLoginManager>().Revoke();
	}



	private void OnClick_License()
	{
		group_Sound.gameObject.SetActive(false);
		group_License.gameObject.SetActive(true);

		btn_License.transform.GetChild(0).GetComponent<Image>().color = Util.HexToRGB("#FFC700");
		btn_Sound.transform.GetChild(0).GetComponent<Image>().color = Util.HexToRGB("#DCDCDC");
	}

	private void OnClick_Sound()
	{
		group_Sound.gameObject.SetActive(true);
		group_License.gameObject.SetActive(false);


		btn_License.transform.GetChild(0).GetComponent<Image>().color = Util.HexToRGB("#DCDCDC");
		btn_Sound.transform.GetChild(0).GetComponent<Image>().color = Util.HexToRGB("#FFC700");
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