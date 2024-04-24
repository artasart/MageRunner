using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

public class CoinSpawner : MonoBehaviour
{
	public ParticleSystem ps;
	public RectTransform targetUI;
	public float speed = 5f;
	public float delay = 1f;
	public float destroyDistanceThreshold = 0.1f;

	private ParticleSystem.Particle[] particles;
	private int numParticlesAlive;
	private float step;

	public void Spawn(int gold)
	{
		if (targetUI == null) targetUI = GameObject.Find("img_Coin").GetComponent<RectTransform>();

		SetBurstCount(gold);

		ps.Play();

		Util.RunCoroutine(Co_BurstParticles(gold).Delay(delay), nameof(Co_BurstParticles) + this.GetHashCode());
	}

	private IEnumerator<float> Co_BurstParticles(int gold)
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

						FindObjectOfType<Scene_Game>().AddGold(1);
					});

					GameManager.Sound.PlaySound("Pick", .125f);
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

		this.GetComponent<RePoolObject>().RePool();
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
