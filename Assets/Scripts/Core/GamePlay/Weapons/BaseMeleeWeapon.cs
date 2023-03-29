using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VikingTest.Core.ScriptableObjects;

namespace VikingTest.Core.Gameplay
{
    public class BaseMeleeWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private MeleeWeaponStats meleeWeaponStats;

        public event Action AttackStarted;
        
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInAttackDamageWindow;
        private bool _isAbleToAttack;

        private readonly List<IDamageable> _damageablesAffected = new();
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
            if (!meleeWeaponStats.DamageTargetTags.Contains(other.tag) || !other.TryGetComponent<IDamageable>(out var damageable)) return;

            if (_isInAttackDamageWindow && !_alreadyDamagedObjects.Contains(damageable))
            {
                damageable.TakeDamage(meleeWeaponStats.AttackParameters.Damage);
                _alreadyDamagedObjects.Add(damageable);
            }
            else
            {
                _damageablesAffected.Add(damageable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!meleeWeaponStats.DamageTargetTags.Contains(other.tag) || !other.TryGetComponent<IDamageable>(out var damageable)) return;

            if (_damageablesAffected.Contains(damageable))
                _damageablesAffected.Remove(damageable);
        }

        public void Attack() => AttackAsync().Forget();
        
        private async UniTask AttackAsync()
        {
            if (!_isAbleToAttack)
                return;

            try
            {
                _isAbleToAttack = false;
                AttackStarted?.Invoke();

                //swing delay
                await UniTask.Delay(TimeSpan.FromSeconds(meleeWeaponStats.AttackParameters.SwingTime), cancellationToken: _cancellationTokenSource.Token);

                //open damage window and deal damage to affected targets
                _isInAttackDamageWindow = true;
                _damageablesAffected.ForEach(x =>
                {
                    x.TakeDamage(meleeWeaponStats.AttackParameters.Damage);
                    _alreadyDamagedObjects.Add(x);
                });
                await UniTask.Delay(TimeSpan.FromSeconds(meleeWeaponStats.AttackParameters.DamageTimeWindow), cancellationToken: _cancellationTokenSource.Token);

                //close damage window and finish attack
                _isInAttackDamageWindow = false;
                await UniTask.Delay(TimeSpan.FromSeconds(meleeWeaponStats.AttackParameters.AttackExitTime), cancellationToken: _cancellationTokenSource.Token);

                //send attack on cooldown
                await UniTask.Delay(TimeSpan.FromSeconds(meleeWeaponStats.AttackParameters.AttackCooldown), cancellationToken: _cancellationTokenSource.Token);
                _damageablesAffected.Clear();
                _alreadyDamagedObjects.Clear();
                _isAbleToAttack = true;
            }
            
            catch (OperationCanceledException)
            {
                Debug.Log("Weapon attack was successfully canceled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
        
        public void InterruptAttack()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _isAbleToAttack = true;
            _isInAttackDamageWindow = false;
        }
    }
}