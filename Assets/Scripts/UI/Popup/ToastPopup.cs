using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastPopup : UI_Base
{
	TMP_Text txtmp_ToastMessage;

	Button btn_ToastPopup;
	Button btn_CloseReward;

	public Action callback_Click;
	public Action callback_Close;

	protected override void Awake()
	{
		base.Awake();

		btn_ToastPopup = this.GetComponent<Button>();
		btn_ToastPopup.onClick.AddListener(OnClick_ToastPopup);
		btn_ToastPopup.onClick.AddListener(OpenSound);
		btn_ToastPopup.UseAnimation();

		txtmp_ToastMessage = GetUI_TMPText(nameof(txtmp_ToastMessage), string.Empty);
		btn_CloseReward = GetUI_Button(nameof(btn_CloseReward), OnClick_CloseRewardAd, useAnimation: true);
	}

	bool isClicked = true;

	private void Start()
	{
		HideRewardAd(true);
	}

	private void OnClick_ToastPopup()
	{
		if (isClicked) return;

		callback_Click?.Invoke();

		Debug.Log("Toast CLicked");

		HideRewardAd();

		isClicked = true;
	}

	private void OnClick_CloseRewardAd()
	{
		callback_Close?.Invoke();

		HideRewardAd();
	}

	public void ShowRewardAd(string message, bool isCancel)
	{
		isClicked = false;

		txtmp_ToastMessage.text = message;

		btn_CloseReward.gameObject.SetActive(isCancel);

		GameManager.UI.Move(this.transform.gameObject, new Vector3(0f, -110f, 0f));

		GameManager.UI.FetchPanel<Panel_Main>().ShowTopMenu(false);
	}

	public void HideRewardAd(bool isInstant = false)
	{
		if (isInstant)
		{
			this.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 80f, 0f);

			return;
		}

		GameManager.UI.Move(this.transform.gameObject, new Vector3(0f, 80f, 0f), callback: () =>
		{
			callback_Click = null;
			callback_Close = null;
			txtmp_ToastMessage.text = string.Empty;
		});

		GameManager.UI.FetchPanel<Panel_Main>().ShowTopMenu(true);
	}
}
