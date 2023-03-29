using UnityEngine;
using Zenject;

namespace VikingTest.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Create SpawnSettings", fileName = "SpawnSettings", order = 0)]
    public class SpawnSettings : ScriptableObjectInstaller, ISpawnSettings
    {
        [Header("Enemies")]
        [SerializeField] private int maxEnemiesNum = 1;
        
        [Header("HealingOrbs")]
        [SerializeField] private int healingOrbDropChance = 20;
        [SerializeField] private Vector3 spawnOffset = new(0, 1f, 0);

        public int MaxEnemiesNum => maxEnemiesNum;
        public int HealingOrbDropChance => healingOrbDropChance;
        public Vector3 SpawnOffset => spawnOffset;

        public override void InstallBindings()
        {
            Container.Bind<ISpawnSettings>().FromInstance(this).AsSingle();
        }
    }
}