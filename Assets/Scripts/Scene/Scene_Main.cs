using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Main : SceneLogic
{
	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_Main>(true);
	}
}