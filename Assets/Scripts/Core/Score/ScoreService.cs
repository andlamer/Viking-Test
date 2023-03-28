namespace VikingTest.Services
{
    public class ScoreService : IScoreService
    {
        private int _killedEnemiesAmount;

        public void ResetKilledEnemiesCounter() => _killedEnemiesAmount = 0;
        public void IncreaseKilledEnemiesCounter() => _killedEnemiesAmount += 1;
        public int GetKilledEnemiesCounter() => _killedEnemiesAmount;
    }
}