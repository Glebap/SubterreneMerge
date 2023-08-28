using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour
{
	[SerializeField] private string _appKey;

	public event Action InitializationComplete;

	public void OnEnable()
	{
		IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
	}

	public void OnDisable()
	{
		IronSourceEvents.onSdkInitializationCompletedEvent -= SdkInitializationCompletedEvent;
	}
	
	private void Awake()
	{
		IronSource.Agent.init (_appKey, 
			IronSourceAdUnits.REWARDED_VIDEO, 
			IronSourceAdUnits.INTERSTITIAL,
			IronSourceAdUnits.BANNER);
		
		IronSource.Agent.validateIntegration();
	}

	private void SdkInitializationCompletedEvent()
	{
		Debug.Log("Unity Ads initialization complete.");
		IronSource.Agent.validateIntegration();
		InitializationComplete?.Invoke();
	}
}