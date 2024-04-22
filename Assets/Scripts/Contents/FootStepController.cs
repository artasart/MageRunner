using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepController : MonoBehaviour
{
	Scene_Game game;
	PlayerActor actor;
	private void Awake()
	{
		actor = GetComponent<PlayerActor>();
		game = FindObjectOfType<Scene_Game>();
	}

	public void StopWalk()
	{
		Util.KillCoroutine(nameof(Co_PlaySound));
	}

	public void StartWalk()
	{
		Util.RunCoroutine(Co_PlaySound(), nameof(Co_PlaySound), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_PlaySound()
	{
		while (true)
		{
			yield return Timing.WaitUntilTrue(() => this.transform.position.y <= -0.195f && actor.isGrounded && game.gameState != GameState.Paused);

			string sound = "FootstepGrassRunning_" + Random.Range(1, 6);

			GameManager.Sound.PlaySound(sound, .5f);

			yield return Timing.WaitForSeconds(.25f);
		}
	}
}
