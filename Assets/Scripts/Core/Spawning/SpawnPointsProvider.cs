using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using VikingTest.Services;
using Zenject;

namespace VikingTest
{
    public class SpawnPointsProvider : MonoBehaviour, ISpawnPointsProvider
    {
        [SerializeField] private Transform[] spawnTransforms;

        private const float SpawnRadius = 5f;
        
        private IEnemySpawnService _enemySpawnService;
        private readonly List<Vector3> _spawnPositions = new();

        [Inject]
        private void Construct(IEnemySpawnService enemySpawnService)
        {
            _enemySpawnService = enemySpawnService;
        }

        private void Awake()
        {
            foreach (var point in spawnTransforms)
            {
                _spawnPositions.Add(point.position);
            }

            _enemySpawnService.SetPointsController(this);
        }

        public Vector3[] GetStartingPositions(int numberOfPoints)
        {
            var positions = new Vector3[numberOfPoints];
            var spawnPointId = 0;

            for (var i = 0; i < numberOfPoints; i++)
            {
                positions[i] = GetRandomNavmeshLocation(_spawnPositions.ElementAt(spawnPointId));
                spawnPointId += 1;

                if (spawnPointId >= _spawnPositions.Count)
                    spawnPointId = 0;
            }

            return positions;
        }

        public Vector3 GetRandomPositionOnField() => GetRandomNavmeshLocation(_spawnPositions.ElementAt(Random.Range(0, _spawnPositions.Count)));

        private static Vector3 GetRandomNavmeshLocation(Vector3 startingPosition)
        {
            var randomDirection = Random.insideUnitSphere * SpawnRadius;
            var finalPosition = Vector3.zero;
            randomDirection += startingPosition;

            if (NavMesh.SamplePosition(randomDirection, out var hit, SpawnRadius * 2, 1))
            {
                finalPosition = hit.position;
            }
            else
            {
                Debug.Log($"Not found: {randomDirection}, {startingPosition}");
            }

            return finalPosition;
        }
    }
}

