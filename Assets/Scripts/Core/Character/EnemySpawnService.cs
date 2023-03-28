using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VikingTest.Core;

namespace VikingTest.Services
{
    public class EnemySpawnService : IEnemySpawnService, IDisposable
    {
        private const int MaxEnemiesNum = 10;
        private const float RespawnDelay = 1f;

        private readonly MutantEnemy.Pool _mutantEnemyPool;
        private readonly IScoreService _scoreService;

        private ISpawnPointsProvider _spawnPointsProvider;
        private CancellationTokenSource _cancellationTokenSource;

        private IEnumerable<IRespawnableEnemy> _spawnedEnemies;
        private bool _spawnEnabled;

        public EnemySpawnService(MutantEnemy.Pool mutantPool, IScoreService scoreService)
        {
            _scoreService = scoreService;
            _mutantEnemyPool = mutantPool;
        }

        public void SetPointsController(ISpawnPointsProvider spawnPointsProvider) => _spawnPointsProvider = spawnPointsProvider;

        public void Spawn()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _spawnEnabled = true;

            var spawnPositions = _spawnPointsProvider.GetStartingPositions(MaxEnemiesNum);

            for (var i = 0; i < MaxEnemiesNum; i++)
            {
                var enemy = _mutantEnemyPool.Spawn(spawnPositions[i]) as IRespawnableEnemy;
                enemy.OnDespawn += OnEnemyDespawn;
            }
        }

        private void OnEnemyDespawn(IRespawnableEnemy enemy)
        {
            _scoreService.IncreaseScore();
            enemy.IncreaseSpawnCount();
            
            if (!_spawnEnabled) return;

            _mutantEnemyPool.Spawn(_spawnPointsProvider.GetRandomPositionOnField());
        }

        public void Dispose()
        {
            foreach (var respawnableEnemy in _spawnedEnemies)
            {
                respawnableEnemy.OnDespawn -= OnEnemyDespawn;
            }

            _spawnEnabled = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }
}