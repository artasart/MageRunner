using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Game : SceneLogic
{
	public int coin;
	public int highScore;
	public int score;

	private void OnDestroy()
	{

	}

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		GameManager.Scene.Fade(false, .1f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_HUD>();
	}

	public void SaveScore()
	{
		DebugManager.Log("Score is saved.");

		coin = 0;
	}

	public void SetScore()
	{
		GameManager.UI.FetchPanel<Panel_HUD>().SetScoreUI(score);
	}
	public void AddCoin(int amount)
	{
		coin += amount;

		GameManager.UI.FetchPanel<Panel_HUD>().SetCoinUI(coin);
	}
}