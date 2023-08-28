using UnityEngine;

public class LevelPlayMediation : MonoBehaviour
{
	[SerializeField] private string _appKey;
	
	
	private void Awake()
	{
		IronSource.Agent.init (_appKey, 
			IronSourceAdUnits.REWARDED_VIDEO, 
			IronSourceAdUnits.INTERSTITIAL,
			IronSourceAdUnits.BANNER);
	}

	private void SdkInitializationCompletedEvent()
	{
		IronSource.Agent.validateIntegration();
		
		if (IronSource.Agent.isRewardedVideoAvailable())
			IronSource.Agent.showRewardedVideo();
	}
}