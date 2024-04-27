using MEC;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static EasingFunction;

public class GameUIManager : SingletonManager<GameUIManager>
{
	#region Members

	[NonReorderable] Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
	[NonReorderable] Dictionary<string, GameObject> popups = new Dictionary<string, GameObject>();
	[NonReorderable] Dictionary<string, GameObject> splashs = new Dictionary<string, GameObject>();

	[NonReorderable] public List<string> ignorePanels = new List<string>();
	[NonReorderable] public List<string> ignorePopups = new List<string>();

	Stack<string> openPanels = new Stack<string>();
	Stack<string> openPopups = new Stack<string>();
	Stack<string> openSplashs = new Stack<string>();

	GameObject popup_LastStack;

	GameObject group_MasterCanvas;
	GameObject group_Panel;
	GameObject group_Popup;
	GameObject group_Splash;

	bool isInitialized = false;

	public Canvas MasterCanvas { get => group_MasterCanvas.GetComponent<Canvas>(); }

	#endregion



	#region Initialize

	private void Awake()
	{
		group_MasterCanvas = GameObject.Find("go_Canvas");

		group_Panel = GameObject.Find(nameof(group_Panel));
		group_Popup = GameObject.Find(nameof(group_Popup));
		group_Splash = GameObject.Find(nameof(group_Splash));

		CacheUI(group_Panel, panels);
		CacheUI(group_Popup, popups);
		CacheUI(group_Splash, splashs);

		isInitialized = true;
	}

	private void CacheUI(GameObject _parent, Dictionary<string, GameObject> _objects)
	{
		for (int i = 0; i < _parent.transform.childCount; i++)
		{
			var child = _parent.transform.GetChild(i);
			var name = child.name;

			if (_objects.ContainsKey(name))
			{
				DebugManager.Log($"Same Key is registered in {_parent.name}", DebugColor.UI);
				continue;
			}

			child.gameObject.SetActive(true);
			child.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
			child.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
			child.gameObject.SetActive(false);

			_objects[name] = child.gameObject;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Back();
		}
	}

	public void Back()
	{
		if (openPopups.Count > 0)
		{
			if (ignorePopups.Contains(openPopups.Peek()))
			{
				DebugManager.ClearLog("This popup is ignore popup.", DebugColor.UI);

				return;
			}

			else
			{
				PopPopup();

				GameManager.Sound.PlaySound(Define.SOUND_CLOSE);
			}
		}

		else if (openPanels.Count > 0)
		{
			if (ignorePanels.Contains(openPanels.Peek()))
			{
				if (openPopups.Count <= 0)
				{
					var popupName = popup_LastStack.name;

					if (popups.ContainsKey(popupName))
					{
						openPopups.Push(popup_LastStack.name);

						popup_LastStack.transform.SetAsLastSibling();
						
						ShowPopup(popups[popupName], true);

						GameManager.Sound.PlaySound(Define.SOUND_OPEN);

						DebugManager.Log($"Push: {popupName}", DebugColor.UI);
					}

					else { DebugManager.Log($"No Popup name {popupName}", DebugColor.UI); }
				}

				return;
			}

			PopPanel();

			GameManager.Sound.PlaySound(Define.SOUND_CLOSE);
		}
	}

	public void Restart()
	{
		if (!isInitialized) return;

		panels.Clear();
		popups.Clear();
		openPanels.Clear();
		openPopups.Clear();
		ignorePanels.Clear();

		group_MasterCanvas = GameObject.Find("go_Canvas");

		group_Panel = GameObject.Find(nameof(group_Panel));
		group_Popup = GameObject.Find(nameof(group_Popup));

		popup_LastStack = null;

		CacheUI(group_Panel, panels);
		CacheUI(group_Popup, popups);
	}

	public void StackLastPopup<T>()
	{
		string popupName = typeof(T).Name;

		if (!popups.ContainsKey(popupName)) return;

		popup_LastStack = popups[popupName];
	}

	#endregion



	#region Core Methods

	public T FetchPanel<T>() where T : Panel_Base
	{
		if (!panels.ContainsKey(typeof(T).ToString())) return null;

		return panels[typeof(T).ToString()].GetComponent<T>();
	}

	public void StackPanel<T>(bool instant = false) where T : Panel_Base
	{
		string panelName = typeof(T).ToString();

		if (openPanels.Contains(panelName)) return;

		if (panels.ContainsKey(panelName))
		{
			var peekPanel = (openPanels.Count > 0) ? openPanels.Peek() : string.Empty;

			if (!string.IsNullOrEmpty(peekPanel))
			{
				panels[peekPanel].GetComponent<CanvasGroup>().alpha = 0f;
				panels[peekPanel].GetComponent<CanvasGroup>().blocksRaycasts = false;
			}

			openPanels.Push(panelName);

			panels[panelName].transform.SetAsLastSibling();

			if (instant)
			{
				panels[panelName].SetActive(true);
				panels[panelName].GetComponent<CanvasGroup>().alpha = 1f;
				panels[panelName].GetComponent<CanvasGroup>().blocksRaycasts = true;
				panels[panelName].GetComponent<Panel_Base>().isInstant = true;
			}

			else ShowPanel(panels[panelName], true);

			DebugManager.Log($"Push: {panelName}", DebugColor.UI);
		}

		else DebugManager.Log($"{panelName} does not exist in this scene.", DebugColor.UI);
	}

	public void SwitchPanel<T>(bool instant = false) where T : Panel_Base
	{
		string panelName = typeof(T).ToString();

		if (openPanels.Contains(panelName)) return;

		if (panels.ContainsKey(panelName))
		{
			var peekPanel = (openPanels.Count > 0) ? openPanels.Peek() : string.Empty;

			if (!string.IsNullOrEmpty(peekPanel) && !ignorePanels.Contains(openPanels.Peek()))
			{
				panels[peekPanel].GetComponent<CanvasGroup>().alpha = 0f;
				panels[peekPanel].GetComponent<CanvasGroup>().blocksRaycasts = false;

				openPanels.Pop();
			}

			openPanels.Push(panelName);

			panels[panelName].transform.SetAsLastSibling();

			if (instant)
			{
				panels[panelName].SetActive(true);
				panels[panelName].GetComponent<CanvasGroup>().alpha = 1f;
				panels[panelName].GetComponent<CanvasGroup>().blocksRaycasts = true;
				panels[panelName].GetComponent<Panel_Base>().isInstant = true;
			}

			else ShowPanel(panels[panelName], true);

			DebugManager.Log($"Push: {panelName}", DebugColor.UI);
		}

		else DebugManager.Log($"{panelName} does not exist in this scene.", DebugColor.UI);
	}

	public void PopPanel()
	{
		if (openPanels.Count <= 0) return;

		var panelName = openPanels.Pop();
		var peekPanel = (openPanels.Count > 0) ? openPanels.Peek() : string.Empty;

		if (panels[panelName].GetComponent<Panel_Base>().isInstant)
		{
			panels[panelName].SetActive(false);
			panels[panelName].GetComponent<CanvasGroup>().alpha = 0f;
			panels[panelName].GetComponent<CanvasGroup>().blocksRaycasts = false;
			panels[panelName].GetComponent<Panel_Base>().isInstant = false;

			if (!string.IsNullOrEmpty(peekPanel))
			{
				panels[peekPanel].GetComponent<CanvasGroup>().alpha = 1f;
				panels[peekPanel].GetComponent<CanvasGroup>().blocksRaycasts = true;
			}
		}

		else ShowPanel(panels[panelName], false, _callback: () =>
		{
			if (!string.IsNullOrEmpty(peekPanel))
			{
				panels[peekPanel].GetComponent<Panel_Base>().Show(true);
			}
		});

		DebugManager.Log($"Pop: {panelName}", DebugColor.UI);
	}

	public void PopPanelAll(bool _instant = false)
	{
		while (openPanels.Count > 1)
		{
			var panelName = openPanels.Pop();

			if (panels[panelName].GetComponent<Panel_Base>().isInstant)
			{
				panels[panelName].SetActive(false);
				panels[panelName].GetComponent<CanvasGroup>().alpha = 0f;
				panels[panelName].GetComponent<CanvasGroup>().blocksRaycasts = false;
				panels[panelName].GetComponent<Panel_Base>().isInstant = false;
			}

			else ShowPanel(panels[panelName], false);

			DebugManager.Log($"Pop: {panelName}", DebugColor.UI);
		}
	}

	public bool IsPanelOpen()
	{
		return openPanels.Count > 1;
	}

	public void StartPanel<T>(bool instant = false) where T : Panel_Base
	{
		string panelName = typeof(T).ToString();

		ignorePanels.Add(panelName);

		StackPanel<T>(instant);
	}

	public T FetchPopup<T>() where T : Popup_Base
	{
		if (!popups.ContainsKey(typeof(T).ToString())) return null;

		return popups[typeof(T).ToString()].GetComponent<T>();
	}

	public void StackPopup<T>(bool _instant = false) where T : Popup_Base
	{
		string popupName = typeof(T).Name;

		if (openPopups.Contains(popupName)) return;

		if (popups.ContainsKey(popupName))
		{
			openPopups.Push(popupName);

			popups[popupName].transform.SetAsLastSibling();

			if (_instant)
			{
				popups[popupName].SetActive(true);
				popups[popupName].GetComponent<CanvasGroup>().alpha = 1f;
				popups[popupName].GetComponent<CanvasGroup>().blocksRaycasts = true;
				popups[popupName].GetComponent<Popup_Base>().isInstant = true;
				popups[popupName].transform.Search("group_Modal").localScale = Vector3.one;
			}

			else ShowPopup(popups[popupName], true);

			DebugManager.Log($"Push: {popupName}", DebugColor.UI);
		}

		else DebugManager.Log($"{popupName} does not exist in this scene.", DebugColor.UI);
	}

	public void PopPopup(bool instant = false)
	{
		if (openPopups.Count <= 0) return;

		var popupName = openPopups.Pop();

		if (instant)
		{
			popups[popupName].SetActive(false);
			popups[popupName].GetComponent<CanvasGroup>().alpha = 0f;
			popups[popupName].GetComponent<CanvasGroup>().blocksRaycasts = false;
			popups[popupName].GetComponent<Popup_Base>().isInstant = false;
		}

		else ShowPopup(popups[popupName], false);

		DebugManager.Log($"Pop: {popupName}", DebugColor.UI);
	}

	public void PopPopupAll(bool _instant = false)
	{
		while (openPopups.Count > 0)
		{
			var popupName = openPopups.Pop();

			if (popups[popupName].GetComponent<Popup_Base>().isInstant)
			{
				popups[popupName].SetActive(false);
				popups[popupName].GetComponent<CanvasGroup>().alpha = 0f;
				popups[popupName].GetComponent<CanvasGroup>().blocksRaycasts = false;
				popups[popupName].GetComponent<Popup_Base>().isInstant = false;
			}

			else ShowPopup(popups[popupName], false);

			DebugManager.Log($"Pop: {popupName}", DebugColor.UI);
		}
	}

	public void StartPopup<T>(bool isInstant = false) where T : Popup_Base
	{
		string popupName = typeof(T).ToString();

		ignorePopups.Add(popupName);

		StackPopup<T>(isInstant);
	}

	public bool IsPopupOpen()
	{
		return openPopups.Count > 0;
	}



	public T FetchSplash<T>() where T : Splash_Base
	{
		if (!splashs.ContainsKey(typeof(T).ToString())) return null;

		return splashs[typeof(T).ToString()].GetComponent<T>();
	}

	public void StackSplash<T>(bool _instant = false) where T : Splash_Base
	{
		string splashName = typeof(T).Name;

		if (splashs.ContainsKey(splashName))
		{
			openSplashs.Push(splashName);

			splashs[splashName].transform.SetAsLastSibling();

			if (_instant)
			{
				splashs[splashName].SetActive(true);
				splashs[splashName].GetComponent<CanvasGroup>().alpha = 1f;
				splashs[splashName].GetComponent<CanvasGroup>().blocksRaycasts = true;
				splashs[splashName].GetComponent<Popup_Base>().isInstant = true;
			}

			else Show(splashs[splashName], true);

			DebugManager.Log($"Push: {splashName}", DebugColor.UI);
		}

		else DebugManager.Log($"{splashName} does not exist in this scene.", DebugColor.UI);
	}

	public void PopSplash()
	{
		if (openSplashs.Count <= 0) return;

		var splashName = openSplashs.Pop();

		if (splashs[splashName].GetComponent<Splash_Base>().isInstant)
		{
			splashs[splashName].SetActive(false);
			splashs[splashName].GetComponent<CanvasGroup>().alpha = 0f;
			splashs[splashName].GetComponent<CanvasGroup>().blocksRaycasts = false;
			splashs[splashName].GetComponent<Splash_Base>().isInstant = false;
		}

		else Show(splashs[splashName], false);

		DebugManager.Log($"Pop: {splashName}", DebugColor.UI);
	}


	#endregion



	#region Basic Methods

	public void ShowPanel(GameObject _gameObject, bool _isShow, Action _callback = null)
	{
		Util.RunCoroutine(Co_Show(_gameObject, _isShow, .75f, _callback), nameof(Co_Show) + _gameObject.GetHashCode(), CoroutineTag.UI);
	}

	public void ShowPopup(GameObject gameObject, bool isShow)
	{
		float showDelay = isShow ? 0f : 0f;
		float easeDelay = isShow ? 0f : .1f;
		float lerpSpeed = isShow ? 3f : 1.5f;

		Action action_Target = () =>
		{
			var popup = gameObject.GetComponent<Popup_Base>();

			if (popup == null || popup.DIM == null) return;

			popup.DIM.interactable = isShow;
		};

		Util.RunCoroutine(Co_Show(gameObject, isShow, callback: action_Target).Delay(easeDelay + showDelay), nameof(Co_Show) + gameObject.GetHashCode(), CoroutineTag.UI);
		Util.RunCoroutine(Co_Ease(gameObject, isShow, lerpSpeed).Delay(.1f - easeDelay), nameof(Co_Ease) + gameObject.GetHashCode(), CoroutineTag.UI);
	}

	public void Show(GameObject _gameObject, bool _isShow, float _lerpspeed = 1f, Action _callback = null)
	{
		Util.RunCoroutine(Co_Show(_gameObject, _isShow, _lerpspeed, _callback), nameof(Co_Show) + _gameObject.GetHashCode(), CoroutineTag.UI);
	}

	private IEnumerator<float> Co_Show(GameObject _gameObject, bool _isShow, float _lerpspeed = 1f, Action callback = null)
	{
		var canvasGroup = _gameObject.GetComponent<CanvasGroup>();
		var targetAlpha = _isShow ? 1f : 0f;
		var lerpvalue = 0f;
		var lerpspeed = _lerpspeed;

		if (canvasGroup == null) yield break;

		if (!_isShow) canvasGroup.blocksRaycasts = false;
		else _gameObject.SetActive(true);

		while (canvasGroup != null && Mathf.Abs(canvasGroup.alpha - targetAlpha) >= 0.001f)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, lerpvalue += lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		if (canvasGroup == null) yield break;

		canvasGroup.alpha = targetAlpha;

		if (_isShow) canvasGroup.blocksRaycasts = true;
		else _gameObject.SetActive(false);

		callback?.Invoke();
	}

	private IEnumerator<float> Co_Ease(GameObject _gameObject, bool _show, float _lerpspeed = 1f)
	{
		GameObject group_Modal = _gameObject.transform.Search(nameof(group_Modal))?.gameObject;

		if (group_Modal == null) yield break;

		float current = group_Modal.GetComponent<RectTransform>().localScale.x;
		float target = _show ? 1f : 0f;
		var fucntion = _show ? Ease.EaseOutBack : Ease.EaseOutQuint;

		float lerpvalue = 0f;

		group_Modal.GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Abs(1 - target);

		while (lerpvalue <= 1f)
		{
			Function function = GetEasingFunction(fucntion);

			float x = function(current, target, lerpvalue);
			float y = function(current, target, lerpvalue);
			float z = function(current, target, lerpvalue);

			group_Modal.GetComponent<RectTransform>().localScale = new Vector3(x, y, z);

			lerpvalue += _lerpspeed * Time.deltaTime;

			yield return Timing.WaitForOneFrame;
		}

		group_Modal.GetComponent<RectTransform>().localScale = Vector3.one * target;
	}



	public void EaseUI(GameObject _gameObject, bool _show, float _lerpspeed = 1f)
	{
		Util.RunCoroutine(Co_EaseUI(_gameObject, _show, _lerpspeed), nameof(Co_EaseUI) + _gameObject.GetHashCode(), CoroutineTag.UI);
	}

	private IEnumerator<float> Co_EaseUI(GameObject _gameObject, bool _show, float _lerpspeed = 1f)
	{
		float current = _gameObject.GetComponent<RectTransform>().localScale.x;
		float target = _show ? 1f : 0f;
		var fucntion = _show ? Ease.EaseOutBack : Ease.EaseOutQuint;

		float lerpvalue = 0f;

		_gameObject.GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Abs(1 - target);

		while (lerpvalue <= 1f)
		{
			Function function = GetEasingFunction(fucntion);

			float x = function(current, target, lerpvalue);
			float y = function(current, target, lerpvalue);
			float z = function(current, target, lerpvalue);

			_gameObject.GetComponent<RectTransform>().localScale = new Vector3(x, y, z);

			lerpvalue += _lerpspeed * Time.deltaTime;

			yield return Timing.WaitForOneFrame;
		}

		_gameObject.GetComponent<RectTransform>().localScale = Vector3.one * target;
	}


	public void FadeCanvasGroup(CanvasGroup _current, float _target, float _lerpspeed = 1f, float _delay = 0f, Action _start = null, Action _end = null)
	{
		Util.RunCoroutine(Co_FadeCanvasGroup(_current, _target, _lerpspeed, _start, _end).Delay(_delay), _current.GetHashCode().ToString(), CoroutineTag.UI);
	}

	private IEnumerator<float> Co_FadeCanvasGroup(CanvasGroup _current, float _target, float _lerpspeed = 1f, Action _start = null, Action _end = null)
	{
		if (_current == null) yield break;

		float lerpvalue = 0f;

		_start?.Invoke();

		if (_target == 0f) _current.blocksRaycasts = false;

		while (Mathf.Abs(_current.alpha - _target) >= 0.001f)
		{
			if (_current == null) yield break;

			_current.alpha = Mathf.Lerp(_current.alpha, _target, lerpvalue += _lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		_current.alpha = _target;

		if (_target == 1f) _current.blocksRaycasts = true;

		_end?.Invoke();
	}



	public void FadeMaskableGrahpic<T>(T _current, float _target, float _lerpspeed = 1f, float _delay = 0f, Action _start = null, Action _end = null) where T : MaskableGraphic
	{
		Util.RunCoroutine(Co_FadeMaskableGraphic(_current, _target, _lerpspeed, _start, _end).Delay(_delay), _current.GetHashCode().ToString(), CoroutineTag.UI);
	}

	private IEnumerator<float> Co_FadeMaskableGraphic<T>(T _current, float _target, float _lerpspeed = 1f, Action _start = null, Action _end = null) where T : MaskableGraphic
	{
		float lerpvalue = 0f;
		float alpha = _current.color.a;

		_start?.Invoke();

		_current.raycastTarget = true;

		while (Mathf.Abs(_current.color.a - _target) >= 0.001f)
		{
			alpha = Mathf.Lerp(alpha, _target, lerpvalue += _lerpspeed * Time.deltaTime);

			_current.color = new Color(_current.color.r, _current.color.g, _current.color.b, alpha);

			yield return Timing.WaitForOneFrame;
		}

		_current.color = new Color(_current.color.r, _current.color.g, _current.color.b, _target);

		_end?.Invoke();
	}



	public void Move(GameObject _target, Vector3 _targetPosition, float _lerpSpeed = 1f)
	{
		Util.RunCoroutine(Co_Move(_target, _targetPosition, _lerpSpeed), nameof(Co_Move) + _target.GetHashCode(), CoroutineTag.UI);
	}

	private IEnumerator<float> Co_Move(GameObject _target, Vector3 _targetPosition, float _lerpSpeed = 1f)
	{
		float lerpvalue = 0f;

		Vector3 currentPosition = _target.GetComponent<RectTransform>().anchoredPosition;
		Vector3 targetPosition = _targetPosition;

		while (Vector3.Distance(currentPosition, targetPosition) > 0.001f)
		{
			currentPosition = Vector3.Lerp(currentPosition, targetPosition, lerpvalue += _lerpSpeed * Time.deltaTime);

			_target.GetComponent<RectTransform>().anchoredPosition = currentPosition;

			yield return Timing.WaitForOneFrame;
		}

		_target.GetComponent<RectTransform>().anchoredPosition = targetPosition;
	}

	#endregion
}