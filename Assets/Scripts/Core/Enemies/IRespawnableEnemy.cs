using System;

namespace VikingTest.Core
{
    public interface IRespawnableEnemy
    {
        void IncreaseSpawnCount();
        event Action<IRespawnableEnemy> OnDespawn;
    }
}