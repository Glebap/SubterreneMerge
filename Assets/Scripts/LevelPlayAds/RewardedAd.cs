using System;
using UnityEngine;


public class RewardedAd : MonoBehaviour
{
    public static event Action RewardedAdLoaded;
    public static event Action RewardedAdFailedToLoad;
    public static event Action RewardedAdShowComplete;

    
    private void OnEnable()
    {
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailableEvent;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailableEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
    }

    private void OnDisable()
    {
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailableEvent;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailableEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoAdRewardedEvent;
    }

    public static void LoadAd()
    {
        IronSource.Agent.loadRewardedVideo();
    }

    private void RewardedVideoOnAdAvailableEvent(IronSourceAdInfo info)
    {
        RewardedVideoAvailabilityChangedEvent(true);
    }
    
    private void RewardedVideoOnAdUnavailableEvent()
    {
        RewardedVideoAvailabilityChangedEvent(true);
    }

    private void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        if (available) 
            RewardedAdLoaded?.Invoke();
        else 
            RewardedAdFailedToLoad?.Invoke();
        
        Debug.Log($"Rewarded Ad {(available ? "is" : "is not")} available");
    }

    public static void ShowAd()
    {
        Debug.Log(IronSource.Agent.isRewardedVideoAvailable());
        IronSource.Agent.showRewardedVideo("DefaultRewardedVideo");
    }

    private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
    {
        Debug.Log("Rewarded Video Ad Rewarded");
        RewardedAdShowComplete?.Invoke();
        LoadAd();
    }

    private void RewardedVideoAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info)
    {
        Debug.Log("Rewarded Video Ad Show Failed: " + error);
        LoadAd();
    }
}
