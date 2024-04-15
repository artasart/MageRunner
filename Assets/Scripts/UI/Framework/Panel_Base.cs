using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Panel_Base : UI_Base
{
	protected Button btn_Close;
	protected string panelName;

	[HideInInspector] public bool isInitialized = false;

	protected override void Awake()
	{
		base.Awake();
				
		CloseTabAll();

		var closeButton = this.transform.Search(nameof(btn_Close));

		if(closeButton)
		{
			btn_Close = GetUI_Button(nameof(btn_Close), OnClick_Close);
			btn_Close.onClick.RemoveListener(PlaySound);
			btn_Close.onClick.AddListener(() => GameManager.Sound.PlaySound("Click_1"));
		}
	}

	private void Start()
	{
		panelName = this.gameObject.name;
	}

	public void Show(bool _show, float _lerpSpeed = 1f, bool _isInstant = false) 
	{
		if (_isInstant)
		{
			this.GetComponent<CanvasGroup>().alpha = _show ? 1f : 0f;
			this.GetComponent<CanvasGroup>().blocksRaycasts = _show;

			return;
		}

		Util.RunCoroutine(Co_Show(_show, _lerpSpeed, _isInstant), nameof(Co_Show) + panelName);
	}


	private IEnumerator<float> Co_Show(bool _isShow, float _lerpSpeed = 1f, bool _isInstant = false)
	{
		var canvasGroup = this.GetComponent<CanvasGroup>();
		var targetAlpha = _isShow ? 1f : 0f;
		var lerpvalue = 0f;
		var lerpspeed = _lerpSpeed;

		if (!_isShow) canvasGroup.blocksRaycasts = false;

		while (Mathf.Abs(canvasGroup.alpha - targetAlpha) >= 0.001f)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, lerpvalue += lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		canvasGroup.alpha = targetAlpha;

		if (_isShow) canvasGroup.blocksRaycasts = true;
	}

	protected virtual void OnClick_Close()
	{
		GameManager.UI.PopPanel();
	}

	public void HidePanel(Action start = null, Action end = null)
	{
		var canvasGroup = this.GetComponent<CanvasGroup>();

		GameManager.UI.FadeCanvasGroup(canvasGroup, 0f, .75f, _start: start, _end: end);
	}

	public void ShowPanel(Action start = null, Action end = null)
	{
		var canvasGroup = this.GetComponent<CanvasGroup>();

		GameManager.UI.FadeCanvasGroup(canvasGroup, 1f, .75f, _start: start, _end: end);
	}
}