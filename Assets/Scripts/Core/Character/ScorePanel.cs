using System;
using TMPro;
using UnityEngine;
using VikingTest.Services;
using Zenject;

namespace VikingTest
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        private const string ScoreFormat = "Scores: {0}";
        
        private IScoreService _scoreService;

        [Inject]
        private void Construct(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        private void Start()
        {
            _scoreService.ScoreUpdated += OnScoreUpdated;
            scoreText.text = string.Format(ScoreFormat, _scoreService.GetCurrentScore());
        }

        private void OnDestroy()
        {
            _scoreService.ScoreUpdated -= OnScoreUpdated;
        }

        private void OnScoreUpdated(int scoreAmount) => scoreText.text = string.Format(ScoreFormat, scoreAmount);
    }
}