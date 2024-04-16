using UnityEngine;

public class MobileSafeArea : MonoBehaviour
{
#if UNITY_ANDROID
	RectTransform _rectTransform;

	Rect _safeArea;
	Vector2 _minAnchor;
	Vector2 _maxAnchor;

	private void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();

		_safeArea = Screen.safeArea;
		_minAnchor = _safeArea.position;
		_maxAnchor = _minAnchor + _safeArea.size;

		_minAnchor.x /= Screen.width;
		_minAnchor.y /= Screen.height;
		_maxAnchor.x /= Screen.width;
		_maxAnchor.y /= Screen.height;

		_rectTransform.anchorMin = _minAnchor;
		_rectTransform.anchorMax = _maxAnchor;
	}
#endif
}