using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using VikingTest.Services;
using Zenject;

namespace VikingTest.Core
{
    public class MutantEnemy : MonoBehaviour, IRespawnableEnemy
    {
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private EnemyStats mutantEnemyStats;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private CreatureHealthComponent creatureHealthComponent;
        [SerializeField] private AutoAttackZone attackZone;
        [SerializeField] private Animator animator;
        [SerializeField] private Collider collider;

        public event Action<IRespawnableEnemy> OnDespawn;

        #region AnimatorHashes

        private static readonly int IsMovingToTarget = Animator.StringToHash("IsMovingToTarget");
        private static readonly int AttackTriggered = Animator.StringToHash("AttackTriggered");
        private static readonly int DamageTaken = Animator.StringToHash("DamageTaken");
        private static readonly int MinHealthReached = Animator.StringToHash("MinHealthReached");

        #endregion

        private CancellationTokenSource _cancellationTokenSource;
        private Pool _pool;

        private Transform _cachedCharacterTransform;

        private bool _chasingEnabled;
        private bool _isInAttackAnimation;
        private bool _isDead;
        private bool _wasInitialized;

        private int _respawnsCount;

        [Inject]
        private void Construct(Character character)
        {
            _cachedCharacterTransform = character.transform;
        }

        private void Awake()
        {
            navMeshAgent.angularSpeed = mutantEnemyStats.AngularSpeed;
            navMeshAgent.speed = mutantEnemyStats.Speed;
            navMeshAgent.acceleration = mutantEnemyStats.Acceleration;
            navMeshAgent.stoppingDistance = mutantEnemyStats.StoppingDistance;
        }

        private void OnEnable()
        {
            if (!_wasInitialized)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            navMeshAgent.enabled = false;
            _isInAttackAnimation = false;
            _isDead = false;
            collider.enabled = true;

            var mutantHealth = mutantEnemyStats.MaxHealthPoints + _respawnsCount;
            creatureHealthComponent.SetHealthStats(mutantEnemyStats.MinHealthPoints, mutantHealth, mutantHealth);

            if (_cachedCharacterTransform != null)
                EnableAI().Forget();

            Subscribe();
        }

        private void OnDisable()
        {
            if (!_wasInitialized)
            {
                _wasInitialized = true;
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            _chasingEnabled = false;
            Unsubscribe();
        }

        public void IncreaseSpawnCount() => _respawnsCount++;

        private async UniTask EnableAI()
        {
            await UniTask.Yield();

            bodyTransform.LookAt(_cachedCharacterTransform);
            navMeshAgent.enabled = true;
            _chasingEnabled = true;

            while (_chasingEnabled)
            {
                if (!_isInAttackAnimation && !navMeshAgent.SetDestination(_cachedCharacterTransform.position))
                    Debug.Log("Unable to update nav mesh agent");

                animator.SetBool(IsMovingToTarget, navMeshAgent.enabled);

                await UniTask.Delay(TimeSpan.FromSeconds(mutantEnemyStats.TargetAIUpdateTime), cancellationToken: _cancellationTokenSource.Token);
            }
        }

        private async void OnDeath()
        {
            if (_isDead) return;

            animator.SetTrigger(MinHealthReached);
            navMeshAgent.enabled = false;
            collider.enabled = false;
            _isDead = true;
            await UniTask.Delay(TimeSpan.FromSeconds(mutantEnemyStats.CorpseDisappearTime), cancellationToken: _cancellationTokenSource.Token);

            Unsubscribe();

            if (_pool == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _pool.Despawn(this);
            OnDespawn?.Invoke(this);
        }

        private void OnAttackStarted()
        {
            navMeshAgent.enabled = false;
            bodyTransform.LookAt(_cachedCharacterTransform);
            animator.SetTrigger(AttackTriggered);
            _isInAttackAnimation = true;
        }

        private void OnDamageTaken()
        {
            if (!_isDead)
                animator.SetTrigger(DamageTaken);
        }

        private void OnAttackFinished()
        {
            if (_isDead) return;

            _isInAttackAnimation = false;
            navMeshAgent.enabled = attackZone.TargetsInZone == 0;
        }

        private void OnAllTargetsLeftAttackZone()
        {
            if (_isDead) return;
            
            navMeshAgent.enabled = true;
        }

        private void Subscribe()
        {
            creatureHealthComponent.MinHealthReached += OnDeath;
            creatureHealthComponent.DamageTaken += OnDamageTaken;
            attackZone.AttackStarted += OnAttackStarted;
            attackZone.AttackFinished += OnAttackFinished;
            attackZone.AllTargetsLeftZone += OnAllTargetsLeftAttackZone;
        }

        private void Unsubscribe()
        {
            creatureHealthComponent.MinHealthReached -= OnDeath;
            creatureHealthComponent.DamageTaken -= OnDamageTaken;
            attackZone.AttackStarted -= OnAttackStarted;
            attackZone.AttackFinished -= OnAttackFinished;
            attackZone.AllTargetsLeftZone -= OnAllTargetsLeftAttackZone;
        }

        private void SetPool(Pool pool) => _pool = pool;

        public class Pool : MonoMemoryPool<Vector3, MutantEnemy>
        {
            protected override void Reinitialize(Vector3 position, MutantEnemy item)
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