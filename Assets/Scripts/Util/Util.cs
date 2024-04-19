using Cinemachine;
using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EasingFunction;

public static class Util
{
	public static Transform Search(this Transform _target, string _name)
	{
		if (_target.name == _name) return _target;

		for (int i = 0; i < _target.childCount; ++i)
		{
			var result = Search(_target.GetChild(i), _name);

			if (result != null) return result;
		}

		return null;
	}

	public static T GetComponentInSibilings<T>(Transform _transform) where T : Component
	{
		var parent = _transform.parent;

		return parent.GetComponentInChildren<T>();
	}


	public static IEnumerator<float> Co_Flik<T>(T _image, int _interval, float _lerpSpeed = 1f) where T : MaskableGraphic
	{
		float alpha = _image.color.a;

		for (int i = 0; i < _interval; i++)
		{
			CoroutineHandle handle = Timing.RunCoroutine(Co_FadeColor(_image, .25f, _lerpSpeed), nameof(Co_FadeColor));

			yield return Timing.WaitUntilDone(handle);

			handle = Timing.RunCoroutine(Co_FadeColor(_image, alpha, _lerpSpeed), nameof(Co_FadeColor));

			yield return Timing.WaitUntilDone(handle);
		}
	}

	public static IEnumerator<float> Co_FadeColor<T>(T _image, float _alpha, float _lerpSpeed = 1f) where T : MaskableGraphic
	{
		var targetColor = new Color(_image.color.r, _image.color.g, _image.color.b, _alpha);
		var lerpvalue = 0f;

		while (GetColorDistance(_image.color, targetColor) >= 0.001f)
		{
			_image.color = Color.Lerp(_image.color, targetColor, lerpvalue += _lerpSpeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		_image.color = targetColor;
	}

	public static void LerpColor<T>(T current, Color target, float _lerpSpeed = 1f) where T : MaskableGraphic
	{
		RunCoroutine(Co_LerpColor(current, target, _lerpSpeed), nameof(Co_LerpColor) + current.GetHashCode());
	}

	private static IEnumerator<float> Co_LerpColor<T>(T current, Color target, float _lerpSpeed = 1f) where T : MaskableGraphic
	{
		var targetColor = new Color(target.r, target.g, target.b, 1f);
		var lerpvalue = 0f;

		while (GetColorDistance(current.color, targetColor) >= 0.001f)
		{
			current.color = Color.Lerp(current.color, targetColor, lerpvalue += _lerpSpeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		current.color = targetColor;
	}

	public static void BlinkColor<T>(T current, float _lerpSpeed = 1f, Action callback = null) where T : MaskableGraphic
	{
		RunCoroutine(Co_BlinkColor(current, _lerpSpeed, callback), nameof(Co_BlinkColor) + current.GetHashCode());
	}

	private static IEnumerator<float> Co_BlinkColor<T>(T current, float _lerpSpeed = 1f, Action callback = null) where T : MaskableGraphic
	{
		var targetColor = new Color(current.color.r, current.color.g, current.color.b, 0f);
		var lerpvalue = 0f;

		while (GetColorDistance(current.color, targetColor) >= 0.001f)
		{
			current.color = Color.Lerp(current.color, targetColor, lerpvalue += _lerpSpeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		current.color = targetColor;

		callback?.Invoke();

		targetColor = new Color(current.color.r, current.color.g, current.color.b, 1f);

		while (GetColorDistance(current.color, targetColor) >= 0.001f)
		{
			current.color = Color.Lerp(current.color, targetColor, lerpvalue += _lerpSpeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		current.color = targetColor;
	}

	public static float GetColorDistance(Color _colorA, Color _colorB)
	{
		float r = _colorA.r - _colorB.r;
		float g = _colorA.g - _colorB.g;
		float b = _colorA.b - _colorB.b;
		float a = _colorA.a - _colorB.a;

		return Mathf.Sqrt(r * r + g * g + b * b + a * a);
	}

	public static string RGBToHex(int _red, int _green, int _blue)
	{
		string hex = "#" + _red.ToString("X2") + _green.ToString("X2") + _blue.ToString("X2");

		return hex;
	}

	public static Color HexToRGB(string hex)
	{
		if (hex.StartsWith("#"))
		{
			hex = hex.Substring(1);
		}

		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

		return new Color(r, g, b, 255) / 255f;
	}

	public static Color RGBToColor(int _red, int _green, int _blue, int _alpha = 255)
	{
		return new Color((float)_red / 255, (float)_green / 255, (float)_blue / 255, (float)_alpha / 255);
	}

	public static void EaseScale(Transform _transform, float _scale, float _lerpSpeed = 1f, Ease _easeMode = Ease.EaseOutBack) => Timing.RunCoroutine(Co_SetLocalScale(_transform, _scale, _lerpSpeed, _easeMode), (int)CoroutineLayer.Util);

	private static IEnumerator<float> Co_SetLocalScale(Transform _transform, float _size, float _lerpSpeed, Ease _easeMode)
	{
		float lerpvalue = 0f;

		while (lerpvalue <= 1f)
		{
			Function function = GetEasingFunction(_easeMode);

			float x = function(_transform.localScale.x, _size, lerpvalue);
			float y = function(_transform.localScale.y, _size, lerpvalue);
			float z = function(_transform.localScale.z, _size, lerpvalue);

			lerpvalue += _lerpSpeed * Time.deltaTime;

			_transform.localScale = new Vector3(x, y, z);

			yield return Timing.WaitForOneFrame;
		}

		_transform.transform.localScale = Vector3.one * _size;
	}

	public static string ConvertAfterAtToLower(string _input)
	{
		string pattern = @"(@.+)";

		Match match = Regex.Match(_input, pattern);

		if (match.Success)
		{
			string matchText = match.Groups[1].Value;
			string convertedText = matchText.ToLower();

			return _input.Replace(matchText, convertedText);
		}

		return _input;
	}

	public static T String2Enum<T>(string _value)
	{
		try
		{
			return (T)Enum.Parse(typeof(T), _value, true);
		}

		catch (Exception)
		{
			return default(T);
		}
	}

	public static DateTime Timestamp2DateTime(long timeStamp)
	{
		DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		return epochStart.AddMilliseconds(timeStamp).ToLocalTime();
	}

	public static string GenerateRandomString(int _length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		System.Random random = new();
		char[] randomString = new char[_length];

		for (int i = 0; i < _length; i++)
		{
			randomString[i] = chars[random.Next(chars.Length)];
		}

		return new string(randomString);
	}

	public static void RunCoroutine(IEnumerator<float> _coroutine, string _tag = null, CoroutineTag _layer = CoroutineTag.None)
	{
		Timing.KillCoroutines(_tag);

		Timing.RunCoroutine(_coroutine, (int)_layer, _tag);
	}

	public static void KillCoroutine(string _tag = null)
	{
		Timing.KillCoroutines(_tag);
	}


	public static TMP_Text FindTMPText(GameObject _gameObject, string _name, string _message = null)
	{
		TMP_Text element = _gameObject.transform.Search(_name).GetComponent<TMP_Text>();
		element.text = _message;

		return element;
	}

	public static Button FindButton(GameObject _gameObject, string _name, Action _action = null, bool useAnimation = false)
	{
		Button element = _gameObject.transform.Search(_name).GetComponent<Button>();
		element.onClick.AddListener(() => GameManager.Sound.PlaySound(Define.SOUND_OPEN));
		element.onClick.AddListener(() => _action?.Invoke());
		if(useAnimation) element.UseAnimation();

		return element;
	}

	public static Image FindImage(GameObject _gameObject, string _name)
	{
		Image element = _gameObject.transform.Search(_name).GetComponent<Image>();

		return element;
	}

	public static Slider FindSlider(GameObject _gameObject, string _name)
	{
		Slider element = _gameObject.transform.Search(_name).GetComponent<Slider>();

		return element;
	}


	public static string MoneyWithAbbreviation(double amount)
	{
		if (amount >= 1000000000000000)
		{
			return (amount / 1000000000000000f).ToString("N2") + " QT";
		}
		else if (amount >= 1000000000000)
		{
			return (amount / 1000000000000f).ToString("N2") + " T";
		}
		else if (amount >= 1000000000)
		{
			return (amount / 1000000000f).ToString("N2") + " B";
		}
		else if (amount >= 1000000)
		{
			return (amount / 1000000f).ToString("N2") + " M";
		}
		else if (amount >= 1000)
		{
			return (amount / 1000f).ToString("N2") + " K";
		}
		return amount.ToString("N0");
	}

	public static long MoneyFromAbbreviation(string _money)
	{
		_money = _money.Trim();

		if (_money.EndsWith(" Qt"))
		{
			return (long)(float.Parse(_money.Replace(" QT", "")) * 1000000000000000);
		}
		else if (_money.EndsWith(" T"))
		{
			return (long)(float.Parse(_money.Replace(" T", "")) * 1000000000000);
		}
		else if (_money.EndsWith(" B"))
		{
			return (long)(float.Parse(_money.Replace(" B", "")) * 1000000000);
		}
		else if (_money.EndsWith(" M"))
		{
			return (long)(float.Parse(_money.Replace(" M", "")) * 1000000);
		}
		else if (_money.EndsWith(" K"))
		{
			return (long)(float.Parse(_money.Replace(" K", "")) * 1000);
		}
		else
		{
			return long.Parse(_money);
		}
	}

	public static DateTime String2DateTime(string dateString)
	{
		DateTime result;

		if (DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ss.fffZ", null, System.Globalization.DateTimeStyles.AssumeUniversal, out result))
		{
			return result;
		}
		else
		{
			throw new Exception("???? ?? ???? ???????? ?????????? ????????????.");
		}
	}

	public static GameObject Instantiate(string _path, Vector3 _position, Quaternion _rotation, Transform _parent = null)
	{
		var prefab = UnityEngine.Resources.Load(_path);
		var gameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

		if (_parent != null) gameObject.transform.SetParent(_parent);

		gameObject.transform.localPosition = _position;
		gameObject.transform.localRotation = _rotation;

		return gameObject;
	}

	public static Sprite GetSpriteByName(Sprite[] _sprites, string spriteName)
	{
		foreach (Sprite sprite in _sprites)
		{
			if (sprite.name == spriteName)
			{
				return sprite;
			}
		}

		return null;
	}

	public static int GetTodayDayIndex()
	{
		DateTime today = DateTime.Now;
		DayOfWeek dayOfWeek = today.DayOfWeek;

		int index = ((int)dayOfWeek + 1) % 7;

		return index - 1;
	}

	public static T StringToEnum<T>(string value) where T : System.Enum
	{
		return (T)System.Enum.Parse(typeof(T), value);
	}

	public static long ConvertToJavaScriptTimestamp(DateTime dateTime)
	{
		TimeSpan span = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return (long)span.TotalMilliseconds;
	}

	public static int GetCurrentWeekNumber()
	{
		DateTime currentDate = System.DateTime.Now;

		System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
		int weekCount = ciCurr.Calendar.GetWeekOfYear(currentDate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

		return weekCount;
	}

	public static int GetWeekOfMonth(DateTime currentDate)
	{
		// 현재 날짜의 월의 첫 번째 날을 가져옵니다.
		DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

		// 현재 날짜가 속한 주를 계산합니다.
		int weekOfMonth = (currentDate.Day - 1 + (int)firstDayOfMonth.DayOfWeek) / 7 + 1;

		return weekOfMonth;
	}

	public static string GetMonthName(DateTime date)
	{
		return date.ToString("MMMM");
	}

	public static string GetDayOfWeek(int index)
	{
		string day;

		switch (index)
		{
			case 0:
				day = "Sun";
				break;
			case 1:
				day = "Mon";
				break;
			case 2:
				day = "Tue";
				break;
			case 3:
				day = "Wed";
				break;
			case 4:
				day = "Thu";
				break;
			case 5:
				day = "Fri";
				break;
			case 6:
				day = "Sat";
				break;
			default:
				day = "Invalid day";
				break;
		}

		return day;
	}

	public static string GetMonthName(int month)
	{
		string monthName;

		switch (month)
		{
			case 1:
				monthName = "January";
				break;
			case 2:
				monthName = "February";
				break;
			case 3:
				monthName = "March";
				break;
			case 4:
				monthName = "April";
				break;
			case 5:
				monthName = "May";
				break;
			case 6:
				monthName = "June";
				break;
			case 7:
				monthName = "July";
				break;
			case 8:
				monthName = "August";
				break;
			case 9:
				monthName = "September";
				break;
			case 10:
				monthName = "October";
				break;
			case 11:
				monthName = "November";
				break;
			case 12:
				monthName = "December";
				break;
			default:
				monthName = "Invalid month";
				break;
		}

		return monthName;
	}

	public static string RemovePrefix(string _origin, string _prefix)
	{
		if (_origin.StartsWith(_prefix))
		{
			return _origin.Substring(_prefix.Length);
		}
		else
		{
			return _origin;
		}
	}

	public static TEnum ConvertIntToEnum<TEnum>(int value)
	{
		if (System.Enum.IsDefined(typeof(TEnum), value))
		{
			return (TEnum)System.Enum.ToObject(typeof(TEnum), value);
		}

		else
		{
			Debug.LogWarning($"Enum value {value} is not defined in {typeof(TEnum)}.");
			return default(TEnum);
		}
	}

	public static int GetEnumLength<T>()
	{
		return System.Enum.GetNames(typeof(T)).Length;
	}


	public static string FormatTime(float seconds)
	{
		TimeSpan time = TimeSpan.FromSeconds(seconds);
		return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
	}




	public static void Zoom(CinemachineVirtualCamera virtualCam, float amount, float speed, float delay = 0f)
	{
		RunCoroutine(Co_ZoomCamera(virtualCam, amount, speed).Delay(delay), nameof(Co_ZoomCamera));
	}

	private static IEnumerator<float> Co_ZoomCamera(CinemachineVirtualCamera virtualCam, float amount, float speed)
	{
		float lerpvalue = 0f;

		while (Mathf.Abs(virtualCam.m_Lens.OrthographicSize - amount) > 0.01f)
		{
			virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(virtualCam.m_Lens.OrthographicSize, amount, lerpvalue += speed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		virtualCam.m_Lens.OrthographicSize = amount;
	}

	public static string GetLanguage()
	{
		SystemLanguage deviceLanguage = Application.systemLanguage;

		if (deviceLanguage != SystemLanguage.English && deviceLanguage != SystemLanguage.Korean)
		{
			return "eng";
		}

		return "kor";
	}

	public static T DeepCopy<T>(T obj)
	{
		if (!typeof(T).IsSerializable)
		{
			throw new ArgumentException("The type must be serializable.", nameof(obj));
		}

		if (ReferenceEquals(obj, null))
		{
			return default(T);
		}

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();

		using (stream)
		{
			formatter.Serialize(stream, obj);
			stream.Seek(0, SeekOrigin.Begin);
			return (T)formatter.Deserialize(stream);
		}
	}

	public static Sprite LoadTextureSheet(Texture2D spriteSheet, int spriteWidth, int spriteHeight, int index)
	{
		int spritesPerRow = spriteSheet.width / spriteWidth;
		int row = index / spritesPerRow;
		int col = index % spritesPerRow;

		Rect spriteRect = new Rect(col * spriteWidth, row * spriteHeight, spriteWidth, spriteHeight);
		Sprite sprite = Sprite.Create(spriteSheet, spriteRect, new Vector2(0.5f, 0.5f));

		return sprite;
	}


	public static void FadeRectSize(RectTransform rect, Vector2 target, float lerpSpeed = 1f)
	{
		RunCoroutine(Co_FadeRectSize(rect, target, lerpSpeed), nameof(Co_FadeRectSize));
	}

	private static IEnumerator<float> Co_FadeRectSize(RectTransform rect, Vector2 target, float lerpSpeed = 1f)
	{
		Vector2 current = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
		float lerpValue = 0f;

		while (Vector2.Distance(current, target) > 0.01f)
		{
			current = Vector2.Lerp(current, target, lerpValue += lerpSpeed * Time.deltaTime);

			rect.sizeDelta = current;

			yield return Timing.WaitForOneFrame;
		}

		rect.sizeDelta = target;
	}

	public static void LerpPosition(Transform transform, Vector3 target, float lerpSpeed = 1f)
	{
		RunCoroutine(Co_LerpPosition(transform, target, lerpSpeed), nameof(Co_LerpPosition) + transform.GetHashCode());
	}

	private static IEnumerator<float> Co_LerpPosition(Transform transform, Vector3 target, float lerpSpeed = 1f)
	{
		Vector3 current = transform.position;
		float lerpValue = 0f;

		while (Vector3.Distance(current, target) > 0.01f)
		{
			current = Vector3.Lerp(current, target, lerpValue += lerpSpeed * Time.deltaTime);

			transform.position = current;

			yield return Timing.WaitForOneFrame;
		}

		transform.position = target;
	}


	public static void LerpRectPosition(RectTransform rect, Vector2 target, float lerpSpeed = 1f)
	{
		RunCoroutine(Co_LerpRectPosition(rect, target, lerpSpeed), nameof(LerpRectPosition));
	}

	private static IEnumerator<float> Co_LerpRectPosition(RectTransform rect, Vector2 target, float lerpSpeed = 1f)
	{
		Vector2 current = rect.anchoredPosition;
		float lerpValue = 0f;

		while (Vector2.Distance(current, target) > 0.01f)
		{
			current = Vector2.Lerp(current, target, lerpValue += lerpSpeed * Time.deltaTime);

			rect.anchoredPosition = current;

			yield return Timing.WaitForOneFrame;
		}

		rect.anchoredPosition = target;
	}

	public static void FadeCanvasGroup(CanvasGroup current, float target, float lerpspeed = 1f, float delay = 0f, Action _start = null, Action end = null)
	{
		Util.RunCoroutine(Co_FadeCanvasGroup(current, target, lerpspeed, _start, end).Delay(delay), current.GetHashCode().ToString(), CoroutineTag.UI);
	}

	private static IEnumerator<float> Co_FadeCanvasGroup(CanvasGroup current, float target, float lerpspeed = 1f, Action start = null, Action end = null)
	{
		float lerpvalue = 0f;

		start?.Invoke();

		if (target == 0f) current.blocksRaycasts = false;

		while (Mathf.Abs(current.alpha - target) >= 0.001f)
		{
			current.alpha = Mathf.Lerp(current.alpha, target, lerpvalue += lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		current.alpha = target;

		if (target == 1f) current.blocksRaycasts = true;

		end?.Invoke();
	}

	public static void PingPongCanvasGroup(CanvasGroup current, float target, float lerpSpeed = 1f, float delay = 0f)
	{
		RunCoroutine(Co_PingPongCanvasGroup(current, target, lerpSpeed).Delay(delay), current.GetHashCode().ToString(), CoroutineTag.UI);
	}

	private static IEnumerator<float> Co_PingPongCanvasGroup(CanvasGroup current, float target, float lerpspeed = 1f)
	{
		float lerpvalue = 0f;
		float startAlpha = current.alpha;
		bool pingPongForward = true;

		while (true)
		{
			while ((pingPongForward && Mathf.Abs(current.alpha - target) >= 0.001f) || (!pingPongForward && Mathf.Abs(current.alpha - startAlpha) >= 0.001f))
			{
				float newAlpha = Mathf.Lerp(current.alpha, pingPongForward ? target : startAlpha, lerpvalue += lerpspeed * Time.deltaTime);

				current.alpha = newAlpha;

				yield return Timing.WaitForOneFrame;
			}

			pingPongForward = !pingPongForward;
			lerpvalue = 0f; // 핑퐁 방향이 변경될 때 lerpvalue를 초기화합니다.

			yield return Timing.WaitForOneFrame;
		}
	}

	public static void AnimateText(TMP_Text text, int target, float _duration = .25f) => Timing.RunCoroutine(Co_AnimateText(text, target, _duration));

	private static IEnumerator<float> Co_AnimateText(TMP_Text txtmp, int target, float _duration = .25f)
	{
		yield return Timing.WaitForOneFrame;

		float elapsedTime = 0f;

		float start = Convert.ToInt32(txtmp.text);

		while (elapsedTime < _duration)
		{
			float time = Mathf.SmoothStep(0f, 1f, elapsedTime / _duration);
			int value = Mathf.RoundToInt(Mathf.Lerp(start, target, time));

			txtmp.text = value.ToString();

			elapsedTime += Time.deltaTime;

			yield return Timing.WaitForOneFrame;
		}

		txtmp.text = target.ToString();
	}

	public static string FormatNumber(int number)
	{
		if (number <= 1000000) return number.ToString("N0");

		double exp = Math.Floor(Math.Log10(number) / 3.0);
		int index = Math.Max(0, Math.Min(((int)exp) - 1, 3));
		string suffix = "kMBT"[index].ToString();
		return (number / Math.Pow(10, exp * 3)).ToString("0.####") + suffix;
	}
}


public static class TMPTextExtensions
{
	public static void UsePingPong(this TMP_Text txtmp)
	{
		txtmp.gameObject.AddComponent<TextAnimation>();
		txtmp.gameObject.AddComponent<CanvasGroup>();
	}
	public static void StartPingPong(this TMP_Text txtmp, float pingpongSpeed = 1f)
	{
		txtmp.GetComponent<TextAnimation>().StartPingPong(pingpongSpeed);
	}

	public static void StopPingPong(this TMP_Text txtmp)
	{
		txtmp.GetComponent<TextAnimation>().StopPingPong();
	}
}

public static class ButtonExtensions
{
	public static void UseAnimation(this Button button)
	{
		button.gameObject.AddComponent<ButtonAnimation>();
	}

	public static void UseClickAction(this Button button, Action _pointerDown = null, Action _pointerUp = null)
	{
		button.gameObject.AddComponent<ButtonClickAction>();

		button.gameObject.GetComponent<ButtonClickAction>().action_PointerDown = _pointerDown;
		button.gameObject.GetComponent<ButtonClickAction>().action_PointerUp = _pointerUp;
	}
}

public enum CoroutineLayer
{
	None,
	Util,
	Game,
	Login,
}

public enum CoroutineTag
{
	None = -1,
	UI,
	Web,
	Content,
}