using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VikingTest.Core.ScriptableObjects;

namespace VikingTest.Core.Gameplay
{
    public class AutoAttackZone : MonoBehaviour
    {
        [SerializeField] private AttackZoneStats attackZoneStats;

        public event Action AttackStarted;
        public event Action AllTargetsLeftZone;
        public event Action TargetEnteredZone;

        private CancellationTokenSource _attackCancellationTokenSource;
        private CancellationTokenSource _detectionCancellationTokenSource;

        private bool _isInAttackDamageWindow;
        private bool _isAbleToAttack;
        private bool _attackZoneIsEnabled;
        private bool _wasInterrupted;
        private bool _informedAboutNoTargetsInZone;

        private int _attacksInSequenceNum;

        private readonly List<IDamageable> _damageablesInZone = new();
        private readonly List<IDamageable> _alreadyDamagedObjects = new();

        private void OnEnable()
        {
            _attackCancellationTokenSource = new CancellationTokenSource();
            _detectionCancellationTokenSource = new CancellationTokenSource();

            _isAbleToAttack = true;
            _isInAttackDamageWindow = false;
            _attackZoneIsEnabled = true;
            _wasInterrupted = false;
            _informedAboutNoTargetsInZone = true;

            ZoneTargetsContinuousDetectionAsync().Forget();
        }

        private void OnDisable()
        {
            _damageablesInZone.Clear();
            _alreadyDamagedObjects.Clear();

            ResetAttack();

            if (!_detectionCancellationTokenSource.IsCancellationRequested)
                _detectionCancellationTokenSource.Cancel();

            _detectionCancellationTokenSource.Dispose();
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
        }

        private void OnTriggerExit(Collider other)
        {
            if (!attackZoneStats.DamageTargetTags.Contains(other.tag) || !other.TryGetComponent<IDamageable>(out var damageable)) return;

            if (_damageablesInZone.Contains(damageable))
                _damageablesInZone.Remove(damageable);
        }

        private async UniTask ZoneTargetsContinuousDetectionAsync()
        {
            try
            {
                while (_attackZoneIsEnabled)
                {
                    if (_wasInterrupted)
                    {
                        await InterruptionDelay();
                    }
                    else
                    {
                        if (_detectionCancellationTokenSource.IsCancellationRequested)
                            break;

                        await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.DetectionUpdateTime), cancellationToken: _detectionCancellationTokenSource.Token);
                        
                        switch (_damageablesInZone.Count)
                        {
                            case > 0 when _isAbleToAttack:
                            {
                                if (_informedAboutNoTargetsInZone)
                                {
                                    TargetEnteredZone?.Invoke();
                                    _informedAboutNoTargetsInZone = false;
                                }
                            
                                await PerformAttackAsync();
                                SendAttackOnCooldown().Forget();
                                break;
                            }
                            case 0 when _informedAboutNoTargetsInZone:
                                continue;
                            case 0:
                                AllTargetsLeftZone?.Invoke();
                                _informedAboutNoTargetsInZone = true;
                                break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Attack zone detection was cancelled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask PerformAttackAsync()
        {
            try
            {
                _isAbleToAttack = false;
                AttackStarted?.Invoke();

                if (_attacksInSequenceNum == 0)
                    await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.FirstSwingDelay), cancellationToken: _attackCancellationTokenSource.Token);

                _attacksInSequenceNum++;
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.SwingTime), cancellationToken: _attackCancellationTokenSource.Token);
                
                _isInAttackDamageWindow = true;
                _damageablesInZone.ForEach(x =>
                {
                    x.TakeDamage(attackZoneStats.AttackParameters.Damage);
                    _alreadyDamagedObjects.Add(x);
                });
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.DamageTimeWindow), cancellationToken: _attackCancellationTokenSource.Token);
                _isInAttackDamageWindow = false;
                
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.AttackExitTime), cancellationToken: _attackCancellationTokenSource.Token);
                _alreadyDamagedObjects.Clear();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Attack was successfully canceled");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public void InterruptAttack()
        {
            ResetAttack();
            _attackCancellationTokenSource = new CancellationTokenSource();

            _wasInterrupted = true;
            _isAbleToAttack = false;
        }

        public void DisableAttacking()
        {
            _attackZoneIsEnabled = false;

            InterruptAttack();

            _detectionCancellationTokenSource.Cancel();

            _attacksInSequenceNum = 0;
            _damageablesInZone.Clear();
            _alreadyDamagedObjects.Clear();
        }

        private async UniTask InterruptionDelay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.InterruptionDelay), cancellationToken: _detectionCancellationTokenSource.Token);
            _attacksInSequenceNum = 0;
            _isAbleToAttack = true;
            _isInAttackDamageWindow = false;
            _wasInterrupted = false;
        }

        private async UniTask SendAttackOnCooldown()
        {
            if (!_detectionCancellationTokenSource.IsCancellationRequested)
                await UniTask.Delay(TimeSpan.FromSeconds(attackZoneStats.AttackParameters.AttackCooldown), cancellationToken: _detectionCancellationTokenSource.Token);
            _isAbleToAttack = true;
        }

        private void ResetAttack()
        {
            _attackCancellationTokenSource.Cancel();
            _attackCancellationTokenSource.Dispose();
            _attackCancellationTokenSource = null;
        }
    }
}