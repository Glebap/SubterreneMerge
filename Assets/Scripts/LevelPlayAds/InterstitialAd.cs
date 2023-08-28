using UnityEngine;
using System;
using Zenject;


public class InterstitialAd : MonoBehaviour
{
    [SerializeField] private bool _showOnStart;
    [SerializeField, Min(0), Tooltip("Minutes")] private int _cooldown = 2;
    
    [Inject] private HeroesHandler _heroesHandler;

    private bool _startAdShown;
    private DateTime _lastInterstitialTime;
    private TimeSpan _interstitialCooldown => TimeSpan.FromMinutes(_cooldown);
    private bool _adIsNotReady => !IronSource.Agent.isInterstitialReady();

    
    private void OnEnable()
    {
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnLoadFailedEvent;
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        _heroesHandler.HeroesMerged += OnHeroesMerged;
    }

    private void OnDisable()
    {
        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnLoadFailedEvent;
        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
        _heroesHandler.HeroesMerged -= OnHeroesMerged;
    }

    private void Awake()
    {
        LoadAd();
    }

    public static void LoadAd()
    {
        IronSource.Agent.loadInterstitial();
    }

    private void TryShowAd()
    {
        DateTime now = DateTime.Now;
        TimeSpan timeSinceLastInterstitial = now - _lastInterstitialTime;

        if (timeSinceLastInterstitial < _interstitialCooldown) return;

        IronSource.Agent.showInterstitial("DefaultInterstitial");
    }

    private void InterstitialOnAdReadyEvent(IronSourceAdInfo info)
    {
        if (!_startAdShown)
        {
            if (_showOnStart)
                TryShowAd();
            else
                _lastInterstitialTime = DateTime.Now;

            _startAdShown = true;
            return;
        }

        TryShowAd();
    }

    private void OnHeroesMerged(HeroesMergeData data)
    {
        if (_adIsNotReady) LoadAd();
            
        TryShowAd();
    }

    private void InterstitialOnLoadFailedEvent(IronSourceError error)
    {
        Debug.Log($"Error loading Interstitial Ad: {error}");
    }

    private void InterstitialOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info)
    {
        Debug.Log($"Error showing Interstitial Ad: {error}");
    }

    private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo info)
    {
        _lastInterstitialTime = DateTime.Now;
    }
}
