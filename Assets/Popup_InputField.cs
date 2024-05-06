using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Popup_InputField : Popup_Base
{
	TMP_Text txtmp_Alert;
	TMP_Text txtmp_Message;

	TMP_InputField inputField_Nickname;

	Button btn_Confirm;

	string nickname;

	private void OnEnable()
	{
		inputField_Nickname.text = string.Empty;
		nickname = string.Empty;
	}

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		txtmp_Alert = GetUI_TMPText(nameof(txtmp_Alert), "Input");
		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), "nickname already exist.");
		btn_Dim = GetUI_Button(nameof(btn_Dim), OnClick_Close);

		inputField_Nickname = GetUI_TMPInputField(nameof(inputField_Nickname), OnValueChanged);

		btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnClick_Check, useAnimation: true);
		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Close, useAnimation: true);
	}

	private void OnValueChanged(string value)
	{
		nickname = value;

		Debug.Log(nickname);
	}

	private void OnClick_Check()
	{
		GameManager.Backend.UpdateNickname(nickname);
	}
}
