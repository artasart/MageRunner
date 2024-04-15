using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TabNavigation : MonoBehaviour
{
	public Selectable[] selectables;
	private int currentIndex = 0;

	void Start()
	{
		Refresh();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			currentIndex = (currentIndex + 1) % selectables.Length;
			selectables[currentIndex].Select();
			DebugManager.Log(selectables[currentIndex].name + " is selected.");
		}
	}

	public void Refresh()
	{
		selectables = GetComponentsInChildren<Selectable>();

		selectables = selectables.Where(x => !(x.GetComponent<Text>() != null)).ToArray();

		currentIndex = 0;
		if (selectables.Length > 0)
		{
			selectables[currentIndex].Select();
		}
	}
}