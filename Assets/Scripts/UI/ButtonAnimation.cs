using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MEC;
using static EasingFunction;

public class ButtonAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Ease easeFunction = Ease.EaseOutBack;
    public float targetScaleY = .95f;

    private Button button;

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_SizeTransform) + this.GetHashCode());
	}

	void Awake()
    {
        button = GetComponent<Button>();
    }

	public void OnPointerDown(PointerEventData eventData)
	{
		Util.RunCoroutine(Co_SizeTransform(targetScaleY), nameof(Co_SizeTransform) + this.GetHashCode());
	}

	public void OnPointerUp(PointerEventData eventData)
	{
        Util.RunCoroutine(Co_SizeTransform(1f), nameof(Co_SizeTransform) + this.GetHashCode());
	}

	private IEnumerator<float> Co_SizeTransform(float _size)
    {
        Vector3 current = button.GetComponent<RectTransform>().localScale;

        float lerpvalue = 0f;

        while (lerpvalue <= 1f)
        {
            if (button == null || button.GetComponent<RectTransform>() == null) yield break;

            Function function = GetEasingFunction(easeFunction);

            float x = function(current.x, _size, lerpvalue);
            float y = function(current.y, _size, lerpvalue);
            float z = function(current.z, _size, lerpvalue);

            lerpvalue += 3f * Time.deltaTime;

            button.GetComponent<RectTransform>().localScale = new Vector3(x, y, z);

            yield return Timing.WaitForOneFrame;
        }

        button.GetComponent<RectTransform>().localScale = Vector3.one * _size;
    }
}