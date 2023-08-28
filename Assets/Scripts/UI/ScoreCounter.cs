using TMPro;
using UnityEngine;
using Zenject;

public class ScoreCounter : MonoBehaviour
{
	[SerializeField] private TMP_Text _scoreView;
	[SerializeField] private TMP_Text _bestScoreView;
	
	[Inject] private HeroesHandler _heroesHandler;

	private PlayerPrefIntValue Score;
	private PlayerPrefIntValue BestScore;

	private void OnEnable()
	{
		_heroesHandler.HeroesMerged += OnHeroesMerged;
	}

	private void OnDisable()
	{
		_heroesHandler.HeroesMerged -= OnHeroesMerged;
	}

	private void Start()
	{
		Score = new PlayerPrefIntValue("Score", 0);
		BestScore = new PlayerPrefIntValue("BestScore", 0);

		UpdateScoreView();
		UpdateBestScoreView();
	}

	public void Reset()
	{
		Score.Value = 0;
		UpdateScoreView();
	}

	private void OnHeroesMerged(HeroesMergeData heroesMergeData)
	{
		int mergedLevel = heroesMergeData.MergedHeroData.Level;
		Score.Value += mergedLevel;

		if (Score > BestScore)
		{
			BestScore.Value = Score;
			UpdateBestScoreView();
		}
		
		UpdateScoreView();
	}

	private void UpdateScoreView()
	{
		_scoreView.text = $"Score: {Score.Value}";
	}

	private void UpdateBestScoreView()
	{
		_bestScoreView.text = $"Best Score: {BestScore.Value}";
	}
}