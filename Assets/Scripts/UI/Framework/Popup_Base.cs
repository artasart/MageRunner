using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Base : UI_Base
{
	protected Button btn_Dim;
	protected Button btn_Close;

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
		btn_Close = GetUI_Button(nameof(btn_Close), OnClick_Close);
		btn_Dim.onClick.RemoveListener(OpenSound);
		btn_Close.onClick.RemoveListener(OpenSound);
		btn_Close.onClick.AddListener(() => GameManager.Sound.PlaySound(Define.SOUND_CLOSE));
		btn_Dim.onClick.AddListener(() => GameManager.Sound.PlaySound(Define.SOUND_CLOSE));
	}

	protected virtual void OnClick_Confirm()
	{
		GameManager.UI.PopPopup();

		callback_confirm?.Invoke();
	}

	protected virtual void OnClick_Close()
	{
		GameManager.UI.PopPopup();

		callback_cancel?.Invoke();
	}

	protected virtual void UseDimClose()
	{
		btn_Dim = GetUI_Button(nameof(btn_Dim), OnClick_Close, _sound: CloseSound);
	}
}
