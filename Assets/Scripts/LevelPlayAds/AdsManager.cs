using UnityEngine;

public class AdsManager : MonoBehaviour
{
	[SerializeField] private AdsInitializer _adsInitializer;

	public void OnEnable()
	{
		_adsInitializer.InitializationComplete += OnInitializationComplete;
	}

	public void OnDisable()
	{
		_adsInitializer.InitializationComplete += OnInitializationComplete;
	}
	
	private void OnApplicationPause(bool isPaused) 
	{                 
		IronSource.Agent.onApplicationPause(isPaused);
	}
	
	private void OnInitializationComplete()
	{
		LoadAds();
	}

	private void LoadAds()
	{
		BannerAd.LoadAd();
		InterstitialAd.LoadAd();
		RewardedAd.LoadAd();
	}
}