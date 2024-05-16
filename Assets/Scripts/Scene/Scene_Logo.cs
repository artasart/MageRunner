using BackEnd;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Scene_Logo : SceneLogic
{
	bool isFirst = true;

	public GameObject player { get; private set; }
	Animator animator;

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Util.KillCoroutine(nameof(Co_LogoStart));
	}

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

		GameManager.Sound.PlayBGM("Dawn");

		StartLogin();
	}

	public void StartLogin()
	{
		GameManager.UI.FetchPanel<Panel_Logo>().StartLogin();

		Util.RunCoroutine(Co_LogoStart(), nameof(Co_LogoStart));
	}

	private IEnumerator<float> Co_LogoStart()
	{
		yield return Timing.WaitForSeconds(.75f);

		LocalData.masterData = JsonManager<MasterData>.LoadData(Define.JSON_MASTERDATA);

		if (LocalData.masterData == null)
		{
			GameManager.Data.GetSheet(ShowLoginPopup);

			animator.SetBool(Define.RUN, true);
			animator.SetFloat(Define.RUNSTATE, .5f);
		}

		else
		{
			GameManager.Data.GetVersion(ShowLoginPopup);
		}
	}

	private void ShowLoginPopup()
	{
		JsonManager<MasterData>.SaveData(LocalData.masterData, Define.JSON_MASTERDATA);

		if(PlayerPrefs.GetInt(Define.QUICKLOGIN) == 1)
		{
			if (PlayerPrefs.GetString(Define.LOGINTYPE) == LoginType.Guest.ToString())
			{
				BackendReturnObject bro = Backend.BMember.GuestLogin("Sign in with Guest");

				if (bro.IsSuccess())
				{
					DebugManager.Log("Guest Success.");

					GameManager.Scene.LoadScene(SceneName.Main);
				}

				return;
			}

#if UNITY_EDITOR

#elif UNITY_IOS
		FindObjectOfType<AppleLoginManager>().Init();
		FindObjectOfType<GoogleLoginManager>().Init();
#endif
		}

		else
		{
			GameManager.UI.FetchPanel<Panel_Logo>().HideDownload();

			GameManager.UI.StackPopup<Popup_Login>();
		}
	}
}