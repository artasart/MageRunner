using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class FootStepController : MonoBehaviour
{
	Scene_Game game;
	PlayerActor actor;

	private void OnDestroy()
	{
		StopWalk();
	}

	private void Awake()
	{
		actor = GetComponent<PlayerActor>();
		game = FindObjectOfType<Scene_Game>();
	}

	public void StopWalk()
	{
		Util.KillCoroutine(nameof(Co_PlaySound));
	}

	public void StartWalk(float delay = .25f)
	{
		Util.RunCoroutine(Co_PlaySound().Delay(delay), nameof(Co_PlaySound), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_PlaySound()
	{
		while (true)
		{
			yield return Timing.WaitUntilTrue(() => actor.isGrounded && !actor.isFlyMode && !actor.isSliding && game.gameState != GameState.Paused);

			string sound = "FootstepGrassRunning_" + Random.Range(1, 6);

			GameManager.Sound.PlaySound(sound, .5f);

			yield return Timing.WaitForSeconds(.25f);
		}
	}
}
