using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class GoogleAdMobManager : SingletonManager<GoogleAdMobManager>
{
#if UNITY_ANDROID
	public string banneAdUnitId = "ca-app-pub-3940256099942544/6300978111";
	public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
	public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
		public string rewardedinterstitialAdUnitId = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IOS
	public string banneAdUnitId = "ca-app-pub-3940256099942544/2934735716";
	public string interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
	public string rewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";		
	public string rewardedinterstitialAdUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
	public string banneAdUnitId = "unused";
	public string interstitialAdUnitId = "unused";
	public string rewardedAdUnitId = "unused";
	public string rewardedinterstitialAdUnitId = "unused";
#endif

	BannerView bannerView;
	RewardedAd rewardedAd;
	InterstitialAd interstitialAd;
	RewardedInterstitialAd rewardedInterstitialAd;

	public Action initialClosed;

	private void Awake()
	{
#if UNITY_EDITOR
		banneAdUnitId = "unused";
		interstitialAdUnitId = "unused";
		rewardedAdUnitId = "unused";
		rewardedinterstitialAdUnitId = "unused";
#endif
		MobileAds.Initialize((InitializationStatus initStatus) =>
		{

		});
	}

	private void Start()
	{
		// Banner
		//LoadAd();
		//ListenToBannerAdEvents();

		// InterstitialAd
		LoadInterstitialAd();

		// Rewarded
		// LoadRewardedAD();

		// Rewarded InterstitialAd
		LoadRewardedInterstitialAd();
	}

	#region InterstitialAd

	public void LoadInterstitialAd()
	{
		Debug.Log("Loading the interstitial ad.");

		if (interstitialAd != null)
		{
			interstitialAd.Destroy();
			interstitialAd = null;
		}

		var adRequest = new AdRequest();

		InterstitialAd.Load(interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
		{
			if (error != null || ad == null)
			{
				Debug.LogError("interstitial ad failed to load an ad " +
							   "with error : " + error);
				return;
			}

			Debug.Log("Interstitial ad loaded with response : "
					  + ad.GetResponseInfo());

			interstitialAd = ad;

			RegisterEventHandlers(interstitialAd);
			RegisterReloadHandler(interstitialAd);
		});
	}

	public void ShowInterstitialAd()
	{
		if (interstitialAd != null && interstitialAd.CanShowAd())
		{
			Debug.Log("Showing interstitial ad.");

			interstitialAd.Show();
		}

		else
		{
			Debug.LogError("Interstitial ad is not ready yet.");
		}
	}

	private void RegisterEventHandlers(InterstitialAd interstitialAd)
	{
		interstitialAd.OnAdPaid += (AdValue adValue) =>
		{
			Debug.Log(String.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
		};

		interstitialAd.OnAdImpressionRecorded += () =>
		{
			Debug.Log("Interstitial ad recorded an impression.");
		};

		interstitialAd.OnAdClicked += () =>
		{
			Debug.Log("Interstitial ad was clicked.");
		};

		interstitialAd.OnAdFullScreenContentOpened += () =>
		{
			Debug.Log("Interstitial ad full screen content opened.");
		};

		interstitialAd.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Interstitial ad full screen content closed.");

			initialClosed?.Invoke();
		};

		interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Interstitial ad failed to open full screen content with error : " + error);
		};
	}

	private void RegisterReloadHandler(InterstitialAd interstitialAd)
	{
		interstitialAd.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Interstitial Ad full screen content closed.");

			LoadInterstitialAd();
		};

		interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Interstitial ad failed to open full screen content with error : " + error);

			LoadInterstitialAd();
		};
	}

	#endregion



	#region Rewarded

	public void LoadRewardedAD()
	{
		Debug.Log("Loading the rewarded ad.");

		if (rewardedAd != null)
		{
			rewardedAd.Destroy();
			rewardedAd = null;
		}

		var adRequest = new AdRequest();

		RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd rewardedAd, LoadAdError error) =>
		{
			if (error != null || rewardedAd == null)
			{
				Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);

				return;
			}

			Debug.Log("Rewarded ad loaded with response : " + rewardedAd.GetResponseInfo());

			this.rewardedAd = rewardedAd;

			RegisterEventHandlers();
		});
	}

	private void RegisterEventHandlers()
	{
		rewardedAd.OnAdPaid += (AdValue adValue) =>
		{
			Debug.Log(String.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
		};

		rewardedAd.OnAdImpressionRecorded += () =>
		{
			Debug.Log("Rewarded ad recorded an impression.");
		};

		rewardedAd.OnAdClicked += () =>
		{
			Debug.Log("Rewarded ad was clicked.");
		};

		rewardedAd.OnAdFullScreenContentOpened += () =>
		{
			Debug.Log("Rewarded ad full screen content opened.");
		};

		rewardedAd.OnAdFullScreenContentClosed += () =>
		{
			Debug.Log("Rewarded ad full screen content closed.");

			LoadRewardedAD();

			GameManager.Scene.Dim(false);
		};

		rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
		{
			Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);

			LoadRewardedAD();

			GameManager.Scene.Dim(false);
		};
	}

	public void ShowRewardedAd(Action _reward = null)
	{
		LoadRewardedAD();

		const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

		if (rewardedAd != null && rewardedAd.CanShowAd())
		{
			rewardedAd.Show((Reward reward) =>
			{
				Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));

				Debug.Log("Reward user here.");

				_reward?.Invoke();
			});
		}
	}

	#endregion



	#region Banner

	private void LoadAd()
	{
		Debug.Log("Load banner view.");

		if (bannerView == null)
		{
			CreateBannerView();
		}

		var adRequest = new AdRequest();

		bannerView.LoadAd(adRequest);
	}

	private void CreateBannerView()
	{
		Debug.Log("Creating banner view.");

		if (bannerView != null)
		{
			bannerView.Destroy();
			bannerView = null;
		}

		var adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

		bannerView = new BannerView(banneAdUnitId, adaptiveSize, AdPosition.Bottom);

		// bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
		// bannerView = new BannerView(adUnitId, AdSize.IABBanner, AdPosition.Bottom);
		// bannerView = new BannerView(adUnitId, AdSize.FullWidth, AdPosition.Bottom);
	}

	private void ListenToBannerAdEvents()
	{
		bannerView.OnBannerAdLoaded += () =>
		{
			Debug.Log("Banner view loaded an ad with response : " + bannerView.GetResponseInfo());
		};

		bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
		{
			Debug.LogError("Banner view failed to load an ad with error : " + error);

			CreateBannerView();
		};

		bannerView.OnAdPaid += (AdValue adValue) =>
		{
			Debug.Log(string.Format("Banner view paid {0} {1}.",
				adValue.Value,
				adValue.CurrencyCode));
		};

		bannerView.OnAdImpressionRecorded += () =>
		{
			Debug.Log("Banner view recorded an impression.");
		};

		bannerView.OnAdClicked += () =>
		{
			Debug.Log("Banner view was clicked.");
		};

		bannerView.OnAdFullScreenContentOpened += (null);
		{
			Debug.Log("Banner view full screen content opened.");
		};

		bannerView.OnAdFullScreenContentClosed += (null);
		{
			Debug.Log("Banner view full screen content closed.");
		};
	}

	public void ShowBanner()
	{
		if (bannerView != null)
		{
			Debug.Log("Show banner ad.");

			bannerView.Show();
		}

		else
		{
			CreateBannerView();
		}
	}

	public void HideBanner()
	{
		if (bannerView != null)
		{
			Debug.Log("Hide banner ad.");

			bannerView.Hide();
		}
	}

	#endregion


	#region

	public void LoadRewardedInterstitialAd()
	{
		// Clean up the old ad before loading a new one.
		if (rewardedInterstitialAd != null)
		{
			rewardedInterstitialAd.Destroy();
			rewardedInterstitialAd = null;
		}

		Debug.Log("Loading the rewarded interstitial ad.");

		// create our request used to load the ad.
		var adRequest = new AdRequest();
		adRequest.Keywords.Add("unity-admob-sample");

		// send the request to load the ad.
		RewardedInterstitialAd.Load(rewardedinterstitialAdUnitId, adRequest,
			(RewardedInterstitialAd ad, LoadAdError error) => {
				// if error is not null, the load request failed.
				if (error != null || ad == null)
				{
					Debug.LogError("rewarded interstitial ad failed to load an ad " +
						"with error : " + error);
					return;
				}

				Debug.Log("Rewarded interstitial ad loaded with response : " +
					ad.GetResponseInfo());

				rewardedInterstitialAd = ad;
			});
	}

	public void ShowRewardedInterstitialAd(Action _reward)
	{
		Debug.Log("Show Rearded Interstitial Ad");

		const string rewardMsg =
			"Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";

		if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
		{
			rewardedInterstitialAd.Show((Reward reward) => {

				Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

				_reward?.Invoke();
			});
		}

		LoadRewardedInterstitialAd();
	}

	#endregion
}
