using System;

namespace VikingTest.Core.Services
{
    public class ScoreService : IScoreService
    {
        public event Action<int> ScoreUpdated;

        private int _currentScore;

        public void IncreaseScore()
        {
            _currentScore += 1;
            ScoreUpdated?.Invoke(_currentScore);
        }

        public int GetCurrentScore() => _currentScore;
    }
}