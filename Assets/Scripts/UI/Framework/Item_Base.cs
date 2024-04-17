using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Base : UI_Base
{
	protected override void Awake()
	{
		base.Awake();
	}

	public Button GetItem_Button(Action _action = null, Action _sound = null)
	{
		var button = this.GetComponent<Button>();

		if (_sound == null)
		{
			button.onClick.AddListener(OpenSound);
		}

		else button.onClick.AddListener(() => _sound?.Invoke());

		button.onClick.AddListener(() => _action?.Invoke());

		return button;
	}
}