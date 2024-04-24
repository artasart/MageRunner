using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Base : UI_Base
{
	protected Button btn_Dim;
	protected Button btn_Back;

	public Action callback_confirm;
	public Action callback_cancel;

	protected bool isDefault = true;
	protected bool isInitialized = false;


	public Button DIM { get => btn_Dim; }

	protected override void Awake()
	{
		base.Awake();

		GameObject group_Modal = this.transform.Search(nameof(group_Modal))?.gameObject;

		if (group_Modal != null) group_Modal.GetComponent<RectTransform>().localScale = Vector3.zero;

		CloseTabAll();

		if (!isDefault) return;

		btn_Dim = GetUI_Button(nameof(btn_Dim), OnClick_Close);
		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Close);
		btn_Dim.onClick.RemoveListener(OpenSound);
		btn_Back.onClick.RemoveListener(OpenSound);
		btn_Back.onClick.AddListener(() => GameManager.Sound.PlaySound(Define.SOUND_CLOSE));
		btn_Dim.onClick.AddListener(() => GameManager.Sound.PlaySound(Define.SOUND_CLOSE));
	}

	protected virtual void OnClick_Confirm()
	{
		callback_confirm?.Invoke();

		GameManager.UI.PopPopup(isInstant);
	}

	protected virtual void OnClick_Close()
	{
		callback_cancel?.Invoke();

		GameManager.UI.PopPopup(isInstant);
	}

	protected virtual void UseDimClose()
	{
		btn_Dim = GetUI_Button(nameof(btn_Dim), OnClick_Close, sound: CloseSound);
	}
}
