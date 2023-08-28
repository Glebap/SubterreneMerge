using UnityEngine;

public class BannerAd : MonoBehaviour
{
	private void OnEnable()
	{
		IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
		IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
	}
	
	private void OnDisable()
	{
		IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
		IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
	}

	public static void LoadAd()
	{
		IronSource.Agent.loadBanner(
			size: IronSourceBannerSize.BANNER, 
			position: IronSourceBannerPosition.BOTTOM, 
			placementName: "DefaultBanner"
			);

	}

	private void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo) 
	{
		Debug.Log($"Showing Interstitial Ad: {adInfo}");
	}
	
	private void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError) 
	{
		Debug.Log($"Error showing Interstitial Ad: {ironSourceError}");
	}
}