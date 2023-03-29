using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VikingTest.Core.ScriptableObjects;
using Zenject;

namespace VikingTest.Core.Gameplay
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform orientationTransform;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private BaseMeleeWeapon meleeWeapon;
        [SerializeField] private CreatureHealthComponent healthComponent;

        public event Action CharacterDead;
        public CreatureHealthComponent CharacterHealth => healthComponent;

        private const float Gravity = -9.81f;
        private const float GravityResetVelocityY = -2f;

        #region AnimatorHashes

        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int AttackButtonClicked = Animator.StringToHash("AttackButtonClicked");
        private static readonly int DamageTaken = Animator.StringToHash("DamageTaken");
        private static readonly int LethalDamageTaken = Animator.StringToHash("LethalDamageTaken");
        private static readonly int CharacterDied = Animator.StringToHash("CharacterDied");

        #endregion

        private ICharacterSettings _characterSettings;

        private CancellationTokenSource _cancellationTokenSource;
        private Vector3 _velocity;
        private float _attackCooldown;
        private bool _movingEnabled;
        private bool _isDead;

        [Inject]
        private void Construct(ICharacterSettings characterSettings)
        {
            _characterSettings = characterSettings;
        }

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _movingEnabled = true;
            _isDead = false;
            healthComponent.SetHealthStats(_characterSettings.MinHealthPoints, _characterSettings.MaxHealthPoints, _characterSettings.StartingHealthPoints);
            healthComponent.DamageTaken += OnDamaged;
            healthComponent.MinHealthReached += OnMinHealthReached;
            meleeWeapon.AttackStarted += OnAttack;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            healthComponent.DamageTaken -= OnDamaged;
            healthComponent.MinHealthReached -= OnMinHealthReached;
            meleeWeapon.AttackStarted -= OnAttack;
        }

        private void Update()
        {
            if (_isDead) return;
            
            MoveCharacter();
            ApplyGravity();
        }

        #region CharacterMovement

        private void MoveCharacter()
        {
            var horizontalMovement = Input.GetAxis("Horizontal");
            var verticalMovement = Input.GetAxis("Vertical");

            var bodyMovement = orientationTransform.right * horizontalMovement * _characterSettings.PlayerMovementSpeed.x
                               + orientationTransform.forward * verticalMovement * _characterSettings.PlayerMovementSpeed.y;

            if (Input.GetAxis("Fire1") > 0)
            {
                meleeWeapon.Attack();
            }

            characterAnimator.SetBool(IsRunning, bodyMovement != Vector3.zero && _movingEnabled);

            if (_movingEnabled)
            {
                characterController.Move(bodyMovement * Time.deltaTime);
            }
        }

        private void ApplyGravity()
        {
            var isGrounded = Physics.Raycast(characterTransform.position,
                Vector3.down,
                characterController.height * 0.5f + _characterSettings.GroundDistance,
                _characterSettings.GroundMask);

            if (isGrounded)
                _velocity.y = GravityResetVelocityY;

            _velocity.y += Gravity * Time.deltaTime * _characterSettings.GravityMultiplier;
            characterController.Move(_velocity * Time.deltaTime);
        }

        #endregion

        private void OnAttack()
        {
            characterAnimator.SetTrigger(AttackButtonClicked);
        }
        
        private void OnDamaged()
        {
            if (_isDead) return;

            meleeWeapon.InterruptAttack();
            characterAnimator.SetTrigger(DamageTaken);
        }

        private async void OnMinHealthReached()
        {
            if (_isDead) return;
            
            meleeWeapon.InterruptAttack();
            _movingEnabled = false;
            characterAnimator.SetTrigger(LethalDamageTaken);
            characterAnimator.SetTrigger(CharacterDied);
            _isDead = true;

            await UniTask.Delay(TimeSpan.FromSeconds(_characterSettings.DeathDelay), cancellationToken: _cancellationTokenSource.Token);
            CharacterDead?.Invoke();
        }
    }
}