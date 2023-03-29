using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using VikingTest.Core.ScriptableObjects;
using Zenject;

namespace VikingTest.Core.Gameplay
{
    public class MutantEnemy : MonoBehaviour, IRespawnableEnemy
    {
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private EnemyStats mutantEnemyStats;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private CreatureHealthComponent creatureHealthComponent;
        [SerializeField] private AutoAttackZone attackZone;
        [SerializeField] private Animator animator;
        [SerializeField] private Collider mutantBodyCollider;

        private const float StoppingOffset = 0.1f;
        
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
            _isDead = false;
            mutantBodyCollider.enabled = true;

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

        private void Update()
        {
            if (_isDead || navMeshAgent.enabled && (!navMeshAgent.enabled || !(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + StoppingOffset))) return;
            
            var targetRotation = Quaternion.LookRotation(_cachedCharacterTransform.position - bodyTransform.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, mutantEnemyStats.CloseDistanceRotationSpeed * Time.deltaTime);
        }

        public Vector3 GetCurrentWorldPosition() => bodyTransform.transform.position;
        public void IncreaseSpawnCount() => _respawnsCount++;

        private async UniTask EnableAI()
        {
            try
            {
                await UniTask.Yield();

                bodyTransform.LookAt(_cachedCharacterTransform);
                navMeshAgent.enabled = true;
                _chasingEnabled = true;

                while (_chasingEnabled)
                {
                    if (navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
                    {
                        navMeshAgent.SetDestination(_cachedCharacterTransform.position);
                    }

                    animator.SetBool(IsMovingToTarget, navMeshAgent.enabled);

                    await UniTask.Delay(TimeSpan.FromSeconds(mutantEnemyStats.TargetAIUpdateTime), cancellationToken: _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("AI update was successfully canceled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void OnDamageTaken()
        {
            animator.SetTrigger(DamageTaken);
            attackZone.InterruptAttack();
        }

        private void OnTargetEnteredZone() => navMeshAgent.enabled = false;
        private void OnAllTargetsLeftAttackZone() => navMeshAgent.enabled = true;

        private async void OnDeath()
        {
            if (_isDead) return;

            Unsubscribe();
            attackZone.DisableAttacking();
            animator.SetTrigger(MinHealthReached);
            navMeshAgent.enabled = false;
            mutantBodyCollider.enabled = false;
            _isDead = true;
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(mutantEnemyStats.CorpseDisappearTime), cancellationToken: _cancellationTokenSource.Token);

                if (_pool == null)
                {
                    gameObject.SetActive(false);
                    return;
                }

                _pool.Despawn(this);
                OnDespawn?.Invoke(this);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Operation was successfully canceled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void OnAttackStarted()
        {
            bodyTransform.LookAt(_cachedCharacterTransform);
            animator.SetTrigger(AttackTriggered);
        }

        private void Subscribe()
        {
            creatureHealthComponent.MinHealthReached += OnDeath;
            creatureHealthComponent.DamageTaken += OnDamageTaken;
            attackZone.AttackStarted += OnAttackStarted;
            attackZone.TargetEnteredZone += OnTargetEnteredZone;
            attackZone.AllTargetsLeftZone += OnAllTargetsLeftAttackZone;
        }

        private void Unsubscribe()
        {
            creatureHealthComponent.MinHealthReached -= OnDeath;
            creatureHealthComponent.DamageTaken -= OnDamageTaken;
            attackZone.AttackStarted -= OnAttackStarted;
            attackZone.TargetEnteredZone -= OnTargetEnteredZone;
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