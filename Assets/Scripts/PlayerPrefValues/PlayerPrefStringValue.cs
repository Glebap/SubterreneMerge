using System;
using UnityEngine;

public class PlayerPrefStringValue : PlayerPrefValue<string>
{
	public PlayerPrefStringValue(string key, string defaultValue, params Func<string, string>[] valuePreprocessors) 
		: base(key, defaultValue, valuePreprocessors) { }

	
	protected override string GetValue(string key, string defaultValue)
	{
		return PlayerPrefs.GetString(key, defaultValue);
	}

	protected override void SetValue(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
	}
}