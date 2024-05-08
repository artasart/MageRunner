using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Popup_InputField : Popup_Base
{
	TMP_Text txtmp_Alert;
	TMP_Text txtmp_Message;

	TMP_InputField inputField_Nickname;

	Button btn_Confirm;
	Transform group_Modal;

	string nickname;

	private void OnEnable()
	{
		inputField_Nickname.text = LocalData.gameData.nickname;
		nickname = LocalData.gameData.nickname;
	}

	protected override void OnClick_Close()
	{
		if (string.IsNullOrEmpty( inputField_Nickname.text))
		{
			group_Modal.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			txtmp_Message.gameObject.SetActive(true);
			txtmp_Message.text = "must enter nickname";
			CancelInvoke(nameof(Hide));
			Invoke(nameof(Hide), 2f);

			return;
		}

		GameManager.UI.PopPopup();
	}

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		txtmp_Alert = GetUI_TMPText(nameof(txtmp_Alert), "Input");
		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "nickname already exist");
		txtmp_Message.gameObject.SetActive(false);

		btn_Dim = GetUI_Button(nameof(btn_Dim), OnClick_Close);

		inputField_Nickname = GetUI_TMPInputField(nameof(inputField_Nickname), OnValueChanged);

		btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnClick_Check, useAnimation: true);
		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Close, useAnimation: true);

		group_Modal = this.transform.Search(nameof(group_Modal));
	}

	private void OnValueChanged(string value)
	{
		nickname = value;
	}

	private void OnClick_Check()
	{
		if (string.IsNullOrEmpty(nickname))
		{
			group_Modal.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);

			txtmp_Message.gameObject.SetActive(true);
			txtmp_Message.text = "must enter nickname";
			CancelInvoke(nameof(Hide));
			Invoke(nameof(Hide), 2f);

			return;
		}

		GameManager.Backend.SetNickname(nickname, () =>
		{
			GameManager.UI.PopPopup();

			GameManager.UI.FetchPanel<Panel_Main>().SetUserNickname(nickname);
		},
		() =>
		{
			if (nickname == GameManager.Backend.GetNickname())
			{
				GameManager.UI.PopPopup();

				GameManager.UI.FetchPanel<Panel_Main>().SetUserNickname(nickname);

				return;
			}

			group_Modal.GetComponent<RectTransform>().DOShakePosition(.35f, new Vector3(10, 10, 0), 40, 90, false);
			txtmp_Message.gameObject.SetActive(true);
			txtmp_Message.text = "nickname already exist";
			CancelInvoke(nameof(Hide));
			Invoke(nameof(Hide), 2f);
		});
	}

	private void Hide()
	{
		txtmp_Message.gameObject.SetActive(false);
	}
}
