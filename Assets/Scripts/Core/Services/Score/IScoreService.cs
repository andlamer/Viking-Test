using System;

namespace VikingTest.Core.Services
{
    public interface IScoreService
    {
        event Action<int> ScoreUpdated;
        
        void IncreaseScore();
        int GetCurrentScore();
    }
}