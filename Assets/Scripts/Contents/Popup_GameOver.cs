using System.Collections;
using UnityEngine.UI;

public class Popup_GameOver : Popup_Base
{
	Button btn_Reward;
	Button btn_Retry;
	Button btn_Home;

	protected override void Awake()
	{
		isDefault = false;

		base.Awake();

		btn_Reward = GetUI_Button(nameof(btn_Reward), OnClick_Reward);
		btn_Retry = GetUI_Button(nameof(btn_Retry), OnClick_Retry);
		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home);

		btn_Reward.UseAnimation();
		btn_Retry.UseAnimation();
		btn_Home.UseAnimation();
	}

	private void OnClick_Reward()
	{

	}

	private void OnClick_Retry()
	{
		FindObjectOfType<Scene_Game>().Retry();
	}

	private void OnClick_Home()
	{
		GameManager.Scene.LoadScene(SceneName.Main);
	}
}
