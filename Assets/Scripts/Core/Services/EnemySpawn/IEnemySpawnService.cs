using VikingTest.Core.Gameplay;

namespace VikingTest.Core.Services
{
    public interface IEnemySpawnService
    {
        void SetPointsController(ISpawnPointsProvider spawnPointsProvider);
        void Spawn();
    }
}