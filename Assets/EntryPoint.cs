using System;
using UnityEngine;
using VikingTest.Services;
using Zenject;

namespace VikingTest
{
    public class EntryPoint : MonoBehaviour
    {
        private IEnemySpawnService _enemySpawnService;

        [Inject]
        private void Construct(IEnemySpawnService enemySpawnService)
        {
            _enemySpawnService = enemySpawnService;
        }

        private void Start()
        {
            _enemySpawnService.Spawn();
        }
    }   
}
