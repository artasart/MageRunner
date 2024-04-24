using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using MEC;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
	public ParticleSystem ps;
	public RectTransform targetUI;
	public float speed = 5f;
	public float delay = 1f;
	public float destroyDistanceThreshold = 0.1f;

	private ParticleSystem.Particle[] particles;
	private int numParticlesAlive;
	private float step;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		SetBurstCount(Random.Range(5, 15));

		ps.Play();

		Util.RunCoroutine(Co_BurstParticles().Delay(delay), nameof(Co_BurstParticles));
	}

	private IEnumerator<float> Co_BurstParticles()
	{
		var forceOverLifetime = ps.forceOverLifetime;
		forceOverLifetime.enabled = false;

		float lerpSpeed = 1f;
		bool particlesAlive = true;

		while (lerpSpeed <= 20f && particlesAlive)
		{
			particles = new ParticleSystem.Particle[ps.main.maxParticles];
			numParticlesAlive = ps.GetParticles(particles);
			step = speed * Time.deltaTime;

			particlesAlive = false;

			Vector3 targetPosition = Vector3.zero;

			if (targetUI != null)
			{
				// UI 요소의 스크린 좌표를 월드 좌표로 변환
				Vector3 screenPosition = targetUI.position;
				targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);
			}

			for (int i = 0; i < numParticlesAlive; i++)
			{
				particles[i].position = Vector3.MoveTowards(particles[i].position, targetPosition, step);

				if (Vector3.Distance(particles[i].position, targetPosition) < destroyDistanceThreshold)
				{
					particles[i].remainingLifetime = 0f;

					targetUI.DOScale(1.2f, duration).SetEase(Ease.OutQuad).OnComplete(() =>
					{
						targetUI.DOScale(1f, duration).SetEase(Ease.OutQuad);
					});

					GameManager.Sound.PlaySound("Popup");
				}

				else
				{
					particlesAlive = true;
				}
			}

			ps.SetParticles(particles, numParticlesAlive);
			yield return Timing.WaitForOneFrame;
			lerpSpeed += Time.deltaTime;
		}

		ps.Stop();

		forceOverLifetime.enabled = true;
	}

	public float duration = 1f;

	public void SetBurstCount(int count)
	{
		var emission = ps.emission;
		var burst = emission.GetBurst(0);
		burst.count = count;
		emission.SetBurst(0, burst);
	}
}
