using UnityEngine;

public class SceneLogic : MonoBehaviour
{
	const string GAMEMANAGER = "GameManager";

	protected virtual void Awake()
	{
		var gameManager = GameObject.Find(GAMEMANAGER) ?? Util.Instantiate(Define.PATH_CORE + GAMEMANAGER, Vector3.zero, Quaternion.identity);

		gameManager.name = GAMEMANAGER;
	}
}