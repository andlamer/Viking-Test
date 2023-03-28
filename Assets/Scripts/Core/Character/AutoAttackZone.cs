using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine;
using VikingTest.Core;

namespace VikingTest
{
    public class AutoAttackZone : MonoBehaviour
    {
        [SerializeField] private AttackZoneStats attackZoneStats;

        public event Action AttackStarted;
        public event Action AttackFinished;
        public event Action AllTargetsLeftZone;

        public int TargetsInZone => _damageablesInZone.Count;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInAttackDamageWindow;
        private bool _isAbleToAttack;
        private bool _isInAttack;
        
        private readonly List<IDamageable> _damageablesInZone = new();
        private readonly List<IDamageable> _alreadyDamagedObjects = new();

        private void OnEnable()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _isAbleToAttack = true;
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!attackZoneStats.DamageTargetTags.Contains(other.tag) || !other.TryGetComponent<IDamageable>(out var damageable)) return;

            if (_isInAttackDamageWindow && !_alreadyDamagedObjects.Contains(damageable))
            {
                damageable.TakeDamage(attackZoneStats.AttackParameters.Damage);
                _alreadyDamagedObjects.Add(damageable);
            }
            else
            {
                _damageablesInZone.Add(damageable);
            }

            if (_damageablesInZone.Count > 0 && _isAbleToAttack)
            {
                StartAttacking().Forget();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!attackZoneStats.DamageTargetTags.Contains(other.tag) || !other.TryGetComponent<IDamageable>(out var damageable)) return;

            if (_damageablesInZone.Contains(damageable))
                _damageablesInZone.Remove(damageable);

            if (!_isInAttack && _damageablesInZone.IsEmpty())
            {
                AllTargetsLeftZone?.Invoke();
            }
        }

        private async UniTask StartAttacking()
        {
            var numOfSwings = 0;

            while (_damageablesInZone.Count > 0)
            {                
                _isAbleToAttack = false;
                _isInAttack = true;
                AttackStarted?.Invoke();
                
                //first swing additional delay
                if (numOfSwings == 0)
                    await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.FirstSwingDelay), cancellationToken: _cancellationTokenSource.Token);

                //swing delay
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.SwingTime), cancellationToken: _cancellationTokenSource.Token);

                //open damage window and deal damage to enemies that are already in zone
                _isInAttackDamageWindow = true;
                _damageablesInZone.ForEach(x =>
                {
                    x.TakeDamage(attackZoneStats.AttackParameters.Damage);
                    _alreadyDamagedObjects.Add(x);
                });
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.DamageTimeWindow), cancellationToken: _cancellationTokenSource.Token);

                //close damage window and finish attack
                _isInAttackDamageWindow = false;
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.AttackExitTime), cancellationToken: _cancellationTokenSource.Token);
                _isInAttack = false;
                numOfSwings++;
                AttackFinished?.Invoke();

                //send attack on cooldown
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.AttackCooldown), cancellationToken: _cancellationTokenSource.Token);
                _alreadyDamagedObjects.Clear();
                _isAbleToAttack = true;
            }
        }
    }
}