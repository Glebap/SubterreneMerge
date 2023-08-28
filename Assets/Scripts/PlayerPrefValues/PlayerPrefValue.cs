public abstract class PlayerPrefValue<T>
{
	private readonly string _key;
	private readonly T _defaultValue;
	private readonly System.Func<T, T>[] _valuePreprocessors;

	public T Value
	{
		get => GetValue(_key, _defaultValue);
		set => SetValue(_key, GetProcessedValue(value));
	}
	
	protected PlayerPrefValue(string key, T defaultValue, params System.Func<T, T>[] valuePreprocessors)
	{
		_key = key;
		_defaultValue = defaultValue;
		_valuePreprocessors = valuePreprocessors;
		Value = Value;
	}
	
	public static implicit operator T(PlayerPrefValue<T> ppInt) 
		=> ppInt.Value;

	protected abstract T GetValue(string key, T defaultValue);
	
	protected abstract void SetValue(string key, T value);
	
	private T GetProcessedValue(T value)
	{
		if (_valuePreprocessors == null) 
			return value;
		
		foreach (var valuePreprocessor in _valuePreprocessors)
			value = valuePreprocessor(value);

		return value;
	}
}