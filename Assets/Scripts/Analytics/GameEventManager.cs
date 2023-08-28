using GameAnalyticsSDK;

public static class GameEventManager
{
	public static void SendHeroUnlockedEvent(HeroData heroData)
	{
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "HeroUnlocked", $"{heroData.Race}", heroData.Level);
	}

	public static void SendLevelStartedEvent()
	{
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "LevelStarted");
	}

	public static void SendLevelFailedEvent()
	{
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "LevelFailed");
	}
}