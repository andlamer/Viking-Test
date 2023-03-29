using UnityEngine;

namespace VikingTest.Core.ScriptableObjects
{
    public interface ISpawnSettings
    {
        int MaxEnemiesNum { get; }
        int HealingOrbDropChance { get; }
        Vector3 SpawnOffset { get; }
    }
}