﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using QFSW.QC;

namespace ModelMatch {
	
	public class AdManager : MonoBehaviour
	{
		private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
    
		private RewardedAd _rewardedAd = null;
	
		void Start()
		{
			Debug.Log("Start Init Ads");
			MobileAds.Initialize((InitializationStatus initStatue) => {
				Debug.Log($"Init Admob {initStatue}");
	    	
				LoadRewardedAd();
			});
		}
    
		[Command]
		public void LoadRewardedAd() {
			if (_rewardedAd != null) {
				_rewardedAd.Destroy();
				_rewardedAd = null;
			}
		
			Debug.Log("Loading the rewarded ad.");
		
			var adRequest = new AdRequest();
			RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) => {
				// if error is not null, the load request failed.
				if (error != null || ad == null)
				{
					Debug.LogError("Rewarded ad failed to load an ad " +
						"with error : " + error);
					return;
				}

				Debug.Log("Rewarded ad loaded with response : "
					+ ad.GetResponseInfo());

				_rewardedAd = ad;
				RegisterEventHandlers(_rewardedAd);
				RegisterReloadHandler(_rewardedAd);
			});
		}
		[Command]
		public void ShowRewardedAd() {
			const string rewardMsg =
				"Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

			if (_rewardedAd != null && _rewardedAd.CanShowAd())
			{
				_rewardedAd.Show((Reward reward) =>
				{
					// TODO: Reward the user.
					Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
				});
			} else {
				LoadRewardedAd();
			}
		}
	
		private void RegisterReloadHandler(RewardedAd ad)
		{
			// Raised when the ad closed full screen content.
			ad.OnAdFullScreenContentClosed += () =>
			{
				Debug.Log("Rewarded Ad full screen content closed.");

				// Reload the ad so that we can show another as soon as possible.
				LoadRewardedAd();
			};
			// Raised when the ad failed to open full screen content.
			ad.OnAdFullScreenContentFailed += (AdError error) =>
			{
				Debug.LogError("Rewarded ad failed to open full screen content " +
					"with error : " + error);

				// Reload the ad so that we can show another as soon as possible.
				LoadRewardedAd();
			};
		}
	
		private void RegisterEventHandlers(RewardedAd ad)
		{
			// Raised when the ad is estimated to have earned money.
			ad.OnAdPaid += (AdValue adValue) =>
			{
				Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
					adValue.Value,
					adValue.CurrencyCode));
			};
			// Raised when an impression is recorded for an ad.
			ad.OnAdImpressionRecorded += () =>
			{
				Debug.Log("Rewarded ad recorded an impression.");
			};
			// Raised when a click is recorded for an ad.
			ad.OnAdClicked += () =>
			{
				Debug.Log("Rewarded ad was clicked.");
			};
			// Raised when an ad opened full screen content.
			ad.OnAdFullScreenContentOpened += () =>
			{
				Debug.Log("Rewarded ad full screen content opened.");
			};
			// Raised when the ad closed full screen content.
			ad.OnAdFullScreenContentClosed += () =>
			{
				Debug.Log("Rewarded ad full screen content closed.");
			};
			// Raised when the ad failed to open full screen content.
			ad.OnAdFullScreenContentFailed += (AdError error) =>
			{
				Debug.LogError("Rewarded ad failed to open full screen content " +
					"with error : " + error);
			};
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}
}

