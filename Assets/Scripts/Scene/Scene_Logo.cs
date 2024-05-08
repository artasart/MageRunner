using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Logo : SceneLogic
{
	bool isFirst = true;

	public GameObject player { get; private set; }
	Animator animator;

	protected override void Awake()
	{
		base.Awake();

		isFirst = PlayerPrefs.GetInt(nameof(isFirst)) == 1 ? true : false;

		player = FindObjectOfType<SPUM_Prefabs>().gameObject;
		animator = player.GetComponentInChildren<Animator>();

		player.gameObject.SetActive(false);
	}

	private void Start()
	{
		GameManager.Scene.FadeInstant(true);

		GameManager.Scene.Fade(false, .5f);

		GameManager.UI.Restart();

		GameManager.UI.StartPanel<Panel_Logo>(true);

		GameManager.UI.FetchPanel<Panel_Logo>().SetMessage(PlayerPrefs.HasKey(Define.APPLEUSERID).ToString());

		if (PlayerPrefs.GetInt("isBGMPlayed") == 0)
		{
			GameManager.Sound.PlayBGM("Dawn");

			PlayerPrefs.SetInt("isBGMPlayed", 1);
		}

#if UNITY_EDITOR
		StartLogin();
#elif UNITY_IOS
		FindObjectOfType<AppleLoginManager>().Init();
#endif
	}

	public void StartLogin()
	{
		GameManager.UI.FetchPanel<Panel_Logo>().StartLogin(false, GetGameData);
	}

	public void GetGameData()
	{
		Util.RunCoroutine(Co_LogoStart(), nameof(Co_LogoStart));
	}

	private IEnumerator<float> Co_LogoStart()
	{
		yield return Timing.WaitForSeconds(.75f);

		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);

		if (LocalData.masterData == null)
		{
			GameManager.Data.GetSheet(EnterGame);

			animator.SetBool(Define.RUN, true);
			animator.SetFloat(Define.RUNSTATE, .5f);
		}

		else
		{
			GameManager.Data.GetVersion(EnterGame);
		}
	}

	private void EnterGame()
	{
		JsonManager<MasterData>.SaveData(LocalData.masterData, Define.JSON_MASTERDATA);

		if (isFirst)
		{
			GameManager.Scene.LoadScene(SceneName.Game);

			PlayerPrefs.SetInt(nameof(isFirst), Convert.ToInt32(true));
		}

		else GameManager.Scene.LoadScene(SceneName.Main);
	}
}