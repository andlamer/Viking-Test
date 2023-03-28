namespace VikingTest.Services
{
    public interface IEnemySpawnService
    {
        void SetPointsController(ISpawnPointsProvider spawnPointsProvider);
        void Spawn();
    }
}