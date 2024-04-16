using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneName
{
	Logo,
	Title,
	Main,
	Game
}

public class GameSceneManager : SingletonManager<GameSceneManager>
{
	#region Members

	GameObject go_MasterCanvas;

	public GameObject go_Fade { get; set; }
	public GameObject go_FadeRound { get; set; }
	public GameObject go_Dim { get; set; }

	bool isFadeDone = false;
	bool isDimDone = false;

	bool isFade = false;
	bool isDim = false;

	#endregion



	#region Initialize

	private void Awake()
	{
		CacheTransUI();
	}

	private void CacheTransUI()
	{
		go_MasterCanvas = Instantiate(UnityEngine.Resources.Load<GameObject>(Define.PATH_UI + "go_Canvas_Master"), Vector3.zero, Quaternion.identity, this.transform);
		go_MasterCanvas.name = "go_Canvas_Master";

		go_Fade = CreateTransUI(UnityEngine.Resources.Load<GameObject>(Define.PATH_UI + "go_Fade"));
		go_FadeRound = CreateTransUI(UnityEngine.Resources.Load<GameObject>(Define.PATH_UI + "go_FadeRound"));
		go_Dim = CreateTransUI(UnityEngine.Resources.Load<GameObject>(Define.PATH_UI + "go_Dim"));
	}

	private GameObject CreateTransUI(GameObject prefab)
	{
		var element = Instantiate(prefab, Vector3.zero, Quaternion.identity, go_MasterCanvas.transform);
		var rectTransform = element.GetComponent<RectTransform>();
		var canvasGroup = element.GetComponent<CanvasGroup>();

		rectTransform.localPosition = Vector3.zero;
		canvasGroup.alpha = 0f;
		canvasGroup.blocksRaycasts = false;

		element.SetActive(false);

		return element;
	}

	#endregion



	#region Core Methods

	public void LoadScene(SceneName sceneName, bool isAsync = true, float fadeSpeed = 1f) => Util.RunCoroutine(Co_LoadScene(sceneName, isAsync), nameof(Co_LoadScene));

	private IEnumerator<float> Co_LoadScene(SceneName sceneName, bool isAsync = true, float fadeSpeed = 1f)
	{
		Fade(true, fadeSpeed);

		yield return Timing.WaitUntilTrue(() => isFadeDone);

		string name = GetSceneName(sceneName);

		if (isAsync)
		{
			SceneManager.LoadSceneAsync(name);
		}

		else
		{
			SceneManager.LoadScene(name);
		}

		yield return Timing.WaitUntilTrue(() => IsSceneLoaded(name));

		DebugManager.Log($"{sceneName} is loaded.", DebugColor.Scene);
	}

	#endregion



	#region Basic Methods

	public void UnloadSceneAsync(string _sceneName)
	{
		SceneManager.UnloadSceneAsync(_sceneName);

		DebugManager.Log($"{_sceneName} is unloaded async.", DebugColor.Scene);
	}

	public bool IsSceneLoaded(string _sceneName)
	{
		return SceneManager.GetSceneByName(_sceneName).isLoaded;
	}

	public string GetCurrentSceneName()
	{
		return SceneManager.GetActiveScene().name;
	}

	public int GetCurrentSceneBuildIndex()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}

	public string GetSceneNameByBuildIndex(int _buildIndex)
	{
		Scene scene = SceneManager.GetSceneByBuildIndex(_buildIndex);

		return scene.name;
	}

	public string GetSceneName(SceneName _sceneName)
	{
		string sceneName = string.Empty;

		switch (_sceneName)
		{
			case SceneName.Logo:
				sceneName = "01_" + _sceneName.ToString();
				break;
			case SceneName.Title:
				sceneName = "02_" + _sceneName.ToString();
				break;
			case SceneName.Main:
				sceneName = "03_" + _sceneName.ToString();
				break;
			case SceneName.Game:
				sceneName = "04_" + _sceneName.ToString();
				break;
		}

		return sceneName;
	}

	#endregion



	#region Utils

	public void Fade(bool isFade, float fadeSpeed = 1f)
	{
		if (this.isFade == isFade) return;

		Util.RunCoroutine(Co_FadeTransition(go_Fade, isFade, fadeSpeed), nameof(Co_FadeTransition));

		DebugManager.Log($"Fade : {isFade}", DebugColor.UI);
	}

	private IEnumerator<float> Co_FadeTransition(GameObject meta, bool isFade, float lerpSpeed)
	{
		isFadeDone = false;

		meta.transform.SetAsLastSibling();
		meta.SetActive(true);

		var handle_Show = Timing.RunCoroutine(Co_Show(meta, isFade, lerpSpeed), meta.GetHashCode());

		yield return Timing.WaitUntilDone(handle_Show);

		if (!isFade)
		{
			meta.SetActive(false);
		}

		isFadeDone = true;
		this.isFade = isFade;
	}

	public void Dim(bool isDim, float dimSpeed = 1f)
	{
		Timing.KillCoroutines(go_Dim.GetHashCode());

		Util.RunCoroutine(Co_DimTransition(go_Dim, isDim, dimSpeed), nameof(Co_DimTransition));

		DebugManager.Log($"Dim : {isDim}", DebugColor.UI);
	}

	private IEnumerator<float> Co_DimTransition(GameObject meta, bool isDim, float lerpSpeed)
	{
		isDimDone = false;

		meta.transform.SetAsLastSibling();
		meta.SetActive(true);

		var handle_Show = Timing.RunCoroutine(Co_Show(meta, isDim, lerpSpeed), meta.GetHashCode());

		yield return Timing.WaitUntilDone(handle_Show);

		if (!isDim)
		{
			meta.SetActive(false);
		}

		isDimDone = true;
	}

	public bool IsFaded()
	{
		return isFadeDone;
	}

	private IEnumerator<float> Co_Show(GameObject gameObject, bool isShow, float lerpSpeed = 1f)
	{
		var canvasGroup = gameObject.GetComponent<CanvasGroup>();
		var targetAlpha = isShow ? 1f : 0f;
		var lerpvalue = 0f;
		var lerpspeed = lerpSpeed;

		if (!isShow) canvasGroup.blocksRaycasts = true;
		else gameObject.SetActive(true);

		while (Mathf.Abs(canvasGroup.alpha - targetAlpha) >= 0.001f)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, lerpvalue += lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		canvasGroup.alpha = targetAlpha;

		if (isShow) canvasGroup.blocksRaycasts = false;
		else gameObject.SetActive(false);
	}

	public void FadeInstant(bool enable)
	{
		isFade = enable;

		go_Fade.SetActive(enable);
		go_Fade.GetComponent<CanvasGroup>().alpha = enable ? 1f : 0f;
		go_Fade.GetComponent<CanvasGroup>().blocksRaycasts = enable;

		isFadeDone = true;
	}

	public void FadeRound(bool isFade, float fadeSpeed = .1f)
	{
		if (this.isFade == isFade) return;

		Util.RunCoroutine(Co_FadeRoundTransition(go_FadeRound, isFade, fadeSpeed), nameof(Co_FadeRoundTransition));

		DebugManager.Log($"Fade(Round) : {isFade}", DebugColor.UI);
	}

	private IEnumerator<float> Co_FadeRoundTransition(GameObject fadeRound, bool isFade, float fadeSpeed = .1f)
	{
		isFadeDone = false;

		var material = fadeRound.GetComponentInChildren<Image>().material;
		float value = isFade ? 1f : 0f;
		float target = isFade ? 0f : 1f;
		float lerpvalue = 0f;

		fadeRound.transform.SetAsLastSibling();
		fadeRound.SetActive(true);

		fadeRound.GetComponent<CanvasGroup>().alpha = 1f;
		fadeRound.GetComponent<CanvasGroup>().blocksRaycasts = true;

		while (Mathf.Abs(value - target) > 0.001f)
		{
			material.SetFloat("_Size", value);

			value = Mathf.Lerp(value, target, lerpvalue += fadeSpeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		material.SetFloat("_Size", target);

		if (!isFade)
		{
			fadeRound.SetActive(false);
		}

		isFadeDone = true;
		this.isFade = isFade;
	}

	#endregion
}
