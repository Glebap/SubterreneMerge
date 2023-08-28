using System;
using UnityEngine;

public abstract class Booster : MonoBehaviour
{
	[SerializeField] private int _defaultAmount;
	[SerializeField] private int _amountToAdd;
	
	public event Action Used;

	public abstract string Name { get; }

	public int DefaultAmount => _defaultAmount;

	private int _amount;
	public int Amount
	{
		get => _amount;
		set => _amount = Mathf.Max(0, value);
	}

	public abstract void Use();
	
	protected void OnBoosterUsed()
	{
		Used?.Invoke();
	}

	public void AddDefaultAmount()
	{
		Amount += _amountToAdd;
	}
}