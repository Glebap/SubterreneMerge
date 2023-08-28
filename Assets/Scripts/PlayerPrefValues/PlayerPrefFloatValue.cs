using System;
using UnityEngine;

public class PlayerPrefFloatValue : PlayerPrefValue<float>
{
	public PlayerPrefFloatValue(string key, float defaultValue, params Func<float, float>[] valuePreprocessors) 
		: base(key, defaultValue, valuePreprocessors) { }

	
	protected override float GetValue(string key, float defaultValue)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	protected override void SetValue(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
	}
}