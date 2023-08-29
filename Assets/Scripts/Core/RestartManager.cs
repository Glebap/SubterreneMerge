using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class RestartManager : MonoBehaviour
{
	[SerializeField] private TutorialManager _tutorialManager;
	[SerializeField] private ScoreCounter _scoreCounter;
	[SerializeField] private Button _restartButton;
	[SerializeField] private DialogWindow _confirmWindow;
	
	[Inject] private Board _board;
	[Inject] private HeroesPool _heroesPool;

	
	private void OnEnable()
	{
		_confirmWindow.AcceptButtonCLicked += OnAcceptButtonCLicked;
		_confirmWindow.DeclineButtonCLicked += OnDeclineButtonCLicked;
		_tutorialManager.TutorialCompleted += OnTutorialCompleted;
		_restartButton.onClick.AddListener(OnRestartButtonClicked);
	}

	private void OnDisable()
	{
		_confirmWindow.AcceptButtonCLicked -= OnAcceptButtonCLicked;
		_confirmWindow.DeclineButtonCLicked -= OnDeclineButtonCLicked;
		_tutorialManager.TutorialCompleted -= OnTutorialCompleted;
		_restartButton.onClick.RemoveListener(OnRestartButtonClicked);
	}

	private void Start()
	{
		SetBoosterButtonsActive(TutorialManager.IsCompleted);
	}

	private void OnAcceptButtonCLicked()
	{
		_scoreCounter.Reset();
		_board.ClearBoard();
		_heroesPool.UpdatePool();
		_confirmWindow.Hide();
	}
	
	private void OnDeclineButtonCLicked()
	{
		_confirmWindow.Hide();
	}

	private void OnRestartButtonClicked()
	{
		_confirmWindow.Show();
	}
	
	private void OnTutorialCompleted()
	{
		SetBoosterButtonsActive(true);
	}
	
	private void SetBoosterButtonsActive(bool isActive)
	{
		_restartButton.interactable = isActive;
	}
}