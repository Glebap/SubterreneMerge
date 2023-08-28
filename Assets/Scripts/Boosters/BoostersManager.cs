using System;
using System.Collections.Generic;
using UnityEngine;


public class BoostersManager : MonoBehaviour
{
	[SerializeField] private TutorialManager _tutorialManager;
	
	[SerializeField] private Booster[] _boosters;
	[SerializeField] private BoosterButton[] _buttons;

	private Booster _lastClickedShowAdButtonBooster;

	private Dictionary<Booster, BoosterButton> _boosterButtonsDict;

	public event Action<Booster> BoostedUsed;

	
	private void Awake()
	{
		_boosterButtonsDict = new Dictionary<Booster, BoosterButton>();
		for (var index = 0; index < _boosters.Length; index++)
			_boosterButtonsDict[_boosters[index]] = _buttons[index];
	}

	private void OnEnable()
	{
		_tutorialManager.TutorialCompleted += OnTutorialCompleted;
		RewardedAd.RewardedAdShowComplete += OnRewardedAdShowComplete;
		RewardedAd.RewardedAdFailedToLoad += OnRewardedAdFailedToLoad;
		RewardedAd.RewardedAdLoaded += OnRewardedAdLoaded;
	}
	
	private void Start()
	{
		InitializeBoosters();
		SetBoosterButtonsActive(TutorialManager.IsCompleted);
	}

	private void InitializeBoosters()
	{
		foreach ((Booster booster, BoosterButton button) in _boosterButtonsDict)
		{
			booster.Amount = PlayerPrefs.GetInt(booster.Name, booster.DefaultAmount);
			booster.Used += () => OnBoosterUsed(booster);
			button.AddListener(() => TryUseBooster(booster));
			button.UpdateAmountView(booster.Amount);
			button.ShowAdButton.onClick.AddListener(() => TryShowAdd(booster));
		}
	}
	
	private void TryUseBooster(Booster booster)
	{
		if (booster.Amount == 0) return;
		
		booster.Use();
	}

	private void OnBoosterUsed(Booster booster)
	{
		booster.Amount--;
		PlayerPrefs.SetInt(booster.Name, booster.Amount);
		_boosterButtonsDict[booster].UpdateAmountView(booster.Amount);
		BoostedUsed?.Invoke(booster);
	}

	private void OnTutorialCompleted()
	{
		SetBoosterButtonsActive(true);
	}
	
	private void SetBoosterButtonsActive(bool isActive)
	{
		foreach (var button  in _buttons)
			button.Interactable = isActive;
	}

	private void OnRewardedAdFailedToLoad()
	{
		ToggleAllVideoAvailabilityViews(false);
	}
	
	private void OnRewardedAdLoaded()
	{
		ToggleAllVideoAvailabilityViews(true);
	}

	private void OnRewardedAdShowComplete()
	{
		var booster = _lastClickedShowAdButtonBooster;
		
		booster.AddDefaultAmount();
		_boosterButtonsDict[booster].UpdateAmountView(booster.Amount);
		PlayerPrefs.SetInt(booster.Name, booster.Amount);
	}

	private void ToggleAllVideoAvailabilityViews(bool value)
	{
		foreach (var button in _buttons)
			button.ToggleVideoAvailabilityView(value);
	}

	private void TryShowAdd(Booster booster)
	{
		_lastClickedShowAdButtonBooster = booster;
		_boosterButtonsDict[booster].ToggleVideoAvailabilityView(false);
		RewardedAd.ShowAd();
	}
}