using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ModalType
{
	Confrim,
	ConfirmCancel,
}

public class Popup_Basic : Popup_Base
{
	Button btn_Confirm;

	TMP_Text txtmp_Description;
	TMP_Text txtmp_Confirm;
	TMP_Text txtmp_Close;

	public ModalType modalType = ModalType.ConfirmCancel;

	private void OnEnable()
	{
		switch (modalType)
		{
			case ModalType.Confrim:
				btn_Confirm.gameObject.SetActive(true);
				btn_Back.gameObject.SetActive(false);
				break;
			case ModalType.ConfirmCancel:
				btn_Confirm.gameObject.SetActive(true);
				btn_Confirm.gameObject.SetActive(true);
				break;
		}
	}

	private void OnDisable()
	{
		Clear();
	}

	protected override void Awake()
	{
		base.Awake();

		btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnClick_Confirm);
		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Close, () => GameManager.Sound.PlaySound(Define.SOUND_CLOSE));
		btn_Dim = GetUI_Button(nameof(btn_Dim), OnClick_Close, () => GameManager.Sound.PlaySound(Define.SOUND_CLOSE));

		txtmp_Description = GetUI_TMPText(nameof(txtmp_Description), string.Empty);
		txtmp_Confirm = GetUI_TMPText(nameof(txtmp_Confirm), "OK");
		txtmp_Close = GetUI_TMPText(nameof(txtmp_Close), "CANCEL");
	}

	public void SetPopupInfo(ModalType _modalType, string _description)
	{
		modalType = _modalType;
		txtmp_Description.text = _description;
	}

	public void Clear()
	{
		txtmp_Description.text = string.Empty;
		callback_confirm = null;
		callback_cancel = null;

		modalType = ModalType.ConfirmCancel;
	}
}