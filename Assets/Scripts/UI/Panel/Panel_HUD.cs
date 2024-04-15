using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_HUD : Panel_Base
{
	Button btn_Left;
	Button btn_Right;
	Button btn_Down;
	Button btn_Up;

	protected override void Awake()
	{
		base.Awake();

		btn_Left = GetUI_Button(nameof(btn_Left), OnClick_Left);
		btn_Right = GetUI_Button(nameof(btn_Right), OnClick_Right);
		btn_Down = GetUI_Button(nameof(btn_Down), OnClick_Down);
		btn_Up = GetUI_Button(nameof(btn_Up), OnClick_Up);

		btn_Left.UseAnimation();
		btn_Right.UseAnimation();
		btn_Down.UseAnimation();
		btn_Up.UseAnimation();
	}

	private void OnClick_Left()
	{
		Debug.Log("OnClick_Left");
	}

	private void OnClick_Right()
	{
		Debug.Log("OnClick_Right");
	}

	private void OnClick_Down()
	{
		Debug.Log("OnClick_Down");
	}

	private void OnClick_Up()
	{
		Debug.Log("OnClick_Up");
	}
}
