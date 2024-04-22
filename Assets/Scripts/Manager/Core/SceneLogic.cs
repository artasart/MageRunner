using MEC;
using UnityEngine;

public class SceneLogic : MonoBehaviour
{
	const string GAMEMANAGER = "GameManager";

	private void OnDisable()
	{
		Timing.KillCoroutines((int)CoroutineTag.UI);
		Timing.KillCoroutines((int)CoroutineTag.Content);
	}

	protected virtual void Awake()
	{
		var gameManager = GameObject.Find(GAMEMANAGER) ?? Util.Instantiate(Define.PATH_CORE + GAMEMANAGER, Vector3.zero, Quaternion.identity);

		gameManager.name = GAMEMANAGER;
	}
}