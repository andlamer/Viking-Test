using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VikingTest.Core.ScriptableObjects;
using Zenject;

namespace VikingTest.Core.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class HealingOrb : MonoBehaviour
    {
        [SerializeField] private HealingOrbParameters healingOrbParameters;

        private CancellationTokenSource _cancellationTokenSource;
        private Pool _pool;
        private bool _wasInitialized;

        private void OnEnable()
        {
            if (!_wasInitialized)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            StartAutoDestroyCountDown().Forget();
        }

        private async UniTask StartAutoDestroyCountDown()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(healingOrbParameters.AutoDestroyTime), cancellationToken: _cancellationTokenSource.Token);
            _pool.Despawn(this);
        }

        private void OnDisable()
        {
            if (!_wasInitialized)
            {
                _wasInitialized = true;
                return;
            }
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!healingOrbParameters.HealTargets.Contains(other.tag) || !other.TryGetComponent<IHealable>(out var healable)) return;
            
            healable.Heal(healingOrbParameters.HealthRecovery);
            _pool.Despawn(this);
        }

        private void SetPool(Pool pool) => _pool = pool;
        
        public class Pool : MonoMemoryPool<Vector3, HealingOrb>
        {
            protected override void Reinitialize(Vector3 position, HealingOrb item)
            {
                var transform = item.transform;
                transform.position = position;
                transform.rotation = Quaternion.identity;
                transform.SetParent(null);
                item.SetPool(this);
            }
        }
    }
}
