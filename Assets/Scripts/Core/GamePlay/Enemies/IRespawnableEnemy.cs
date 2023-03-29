using System;
using UnityEngine;

namespace VikingTest.Core.Gameplay
{
    public interface IRespawnableEnemy
    {
        void IncreaseSpawnCount();
        Vector3 GetCurrentWorldPosition();
        event Action<IRespawnableEnemy> OnDespawn;
    }
}