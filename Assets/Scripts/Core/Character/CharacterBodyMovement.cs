using UnityEngine;
using Zenject;

namespace VikingTest
{
    public class CharacterBodyMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform orientationTransform;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Animator characterAnimator;

        private const float Gravity = -9.81f;
        private const float GravityResetVelocityY = -2f;

        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int AttackButtonClicked = Animator.StringToHash("AttackButtonClicked");

        private ICharacterSettings _characterSettings;

        private Vector3 _velocity;
        private float _attackCooldown;
        private bool _isAttacking;

        [Inject]
        private void Construct(ICharacterSettings characterSettings)
        {
            _characterSettings = characterSettings;
        }

        private void Update()
        {
            MoveCharacter();
            ApplyGravity();
        }

        private void MoveCharacter()
        {
            var horizontalMovement = Input.GetAxis("Horizontal");
            var verticalMovement = Input.GetAxis("Vertical");

            var bodyMovement = orientationTransform.right * horizontalMovement * _characterSettings.PlayerMovementSpeed.x
                               + orientationTransform.forward * verticalMovement * _characterSettings.PlayerMovementSpeed.y;

            if (Input.GetAxis("Fire1") > 0 && !_isAttacking)
            {
                characterAnimator.SetTrigger(AttackButtonClicked);
                _isAttacking = true;
            }

            if (_isAttacking && _attackCooldown < _characterSettings.AttackAnimationSpeed)
            {
                _attackCooldown += Time.deltaTime;
            }
            else
            {
                _isAttacking = false;
                _attackCooldown = 0;
            }
            
            characterAnimator.SetBool(IsRunning, bodyMovement != Vector3.zero && !_isAttacking);

            if (!_isAttacking)
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
    }
}