using System;
using UnityEngine;

public class PlayerPrefIntValue : PlayerPrefValue<int>
{
	public PlayerPrefIntValue(string key, int defaultValue, params Func<int, int>[] valuePreprocessors) 
		: base(key, defaultValue, valuePreprocessors) { }

	
	protected override int GetValue(string key, int defaultValue)
	{
		return PlayerPrefs.GetInt(key, defaultValue);
	}

	protected override void SetValue(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
	}
}