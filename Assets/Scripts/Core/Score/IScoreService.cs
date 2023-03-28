using System;

namespace VikingTest.Services
{
    public interface IScoreService
    {
        event Action<int> ScoreUpdated;
        
        void IncreaseScore();
        int GetCurrentScore();
    }
}