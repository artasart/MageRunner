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

	Camera cam;

	private void OnDisable()
	{
		Util.KillCoroutine(nameof(Co_BurstParticles) + this.gameObject.GetHashCode());

		var forceOverLifetime = ps.forceOverLifetime;
		forceOverLifetime.enabled = true;
	}

	private void Start()
	{
		cam = GameObject.Find("UICamera").GetComponent<Camera>();

		if (targetUI == null) targetUI = GameObject.Find("img_Coin").GetComponent<RectTransform>();
	}

	public void Spawn(int gold)
	{
		SetBurstCount(gold);

		var forceOverLifetime = ps.forceOverLifetime;
		forceOverLifetime.enabled = true;
		ps.Play();

		Util.RunCoroutine(Co_BurstParticles(gold).Delay(delay), nameof(Co_BurstParticles) + this.gameObject.GetHashCode(), CoroutineTag.Content);
	}

	private IEnumerator<float> Co_BurstParticles(int gold)
	{
		var forceOverLifetime = ps.forceOverLifetime;
		forceOverLifetime.enabled = false;

		float lerpSpeed = 1f;
		bool particlesAlive = true;

		Vector3 targetPosition = Vector3.zero;

		if (targetUI != null)
		{
			Vector3 screenPos = cam.WorldToScreenPoint(targetUI.position);
			Vector3 worldPos;

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(targetUI, screenPos, cam, out worldPos))
			{
				targetPosition = worldPos;
			}
		}

		float lerpValue = 0;

		float[] random = new float[gold];
		for(int i = 0; i < random.Length; i++)
		{
			random[i] = Random.Range(.5f, 1f);
		}

		while (lerpSpeed <= 20f && particlesAlive)
		{
			particles = new ParticleSystem.Particle[ps.main.maxParticles];
			numParticlesAlive = ps.GetParticles(particles);

			particlesAlive = false;

			DOTween.To(() => lerpValue, x => lerpValue = x, 1f, duration).SetEase(Ease.InOutQuint);

			for (int i = 0; i < numParticlesAlive; i++)
			{
				particles[i].position = Vector3.MoveTowards(particles[i].position, targetPosition, lerpValue * random[i] * speed);

				if (Vector3.Distance(particles[i].position, targetPosition) < destroyDistanceThreshold)
				{
					particles[i].remainingLifetime = 0f;

					targetUI.DOScale(1.2f, duration).SetEase(Ease.OutExpo).OnComplete(() =>
					{
						targetUI.DOScale(1f, duration).SetEase(Ease.OutExpo);

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
