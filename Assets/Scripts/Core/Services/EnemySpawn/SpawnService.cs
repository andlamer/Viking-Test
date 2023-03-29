using System;
using System.Collections.Generic;
using System.Threading;
using VikingTest.Core.Gameplay;
using VikingTest.Core.ScriptableObjects;

namespace VikingTest.Core.Services
{
    public class SpawnService : IEnemySpawnService, IDisposable
    {
        private readonly MutantEnemy.Pool _mutantEnemyPool;
        private readonly HealingOrb.Pool _healingOrbsPool;
        private readonly IScoreService _scoreService;
        private readonly ISpawnSettings _spawnSettings;
        private readonly List<IRespawnableEnemy> _spawnedEnemies = new();
        private readonly Random _random = new ();

        private ISpawnPointsProvider _spawnPointsProvider;
        private CancellationTokenSource _cancellationTokenSource;

        private bool _spawnEnabled;

        public SpawnService(ISpawnSettings spawnSettings, HealingOrb.Pool healingOrbsPool, MutantEnemy.Pool mutantPool, IScoreService scoreService)
        {
            _spawnSettings = spawnSettings;
            _healingOrbsPool = healingOrbsPool;
            _scoreService = scoreService;
            _mutantEnemyPool = mutantPool;
        }

        public void SetPointsController(ISpawnPointsProvider spawnPointsProvider) => _spawnPointsProvider = spawnPointsProvider;

        public void Spawn()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _spawnEnabled = true;

            var spawnPositions = _spawnPointsProvider.GetStartingPositions(_spawnSettings.MaxEnemiesNum);

            for (var i = 0; i < _spawnSettings.MaxEnemiesNum; i++)
            {
                var enemy = _mutantEnemyPool.Spawn(spawnPositions[i]) as IRespawnableEnemy;
                _spawnedEnemies.Add(enemy);
                enemy.OnDespawn += OnEnemyDespawn;
            }
        }

        private void OnEnemyDespawn(IRespawnableEnemy enemy)
        {
            _scoreService.IncreaseScore();
            enemy.IncreaseSpawnCount();

            if (_random.Next(100) > 100 - _spawnSettings.HealingOrbDropChance)
            {
                _healingOrbsPool.Spawn(enemy.GetCurrentWorldPosition() + _spawnSettings.SpawnOffset);
            }

            if (!_spawnEnabled) return;

            _mutantEnemyPool.Spawn(_spawnPointsProvider.GetRandomPositionOnField());
        }

        public void Dispose()
        {
            foreach (var respawnableEnemy in _spawnedEnemies)
            {
                if (respawnableEnemy != null)
                    respawnableEnemy.OnDespawn -= OnEnemyDespawn;
            }

            _spawnEnabled = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}