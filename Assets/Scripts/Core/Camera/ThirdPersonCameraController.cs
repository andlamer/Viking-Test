using System;
using UnityEngine;
using VikingTest.Core.Gameplay;

namespace VikingTest.Core.Camera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Transform characterObject;
        [SerializeField] private float characterRotationSpeed;
        [SerializeField] private Character character;

        private bool _rotationEnabled = true;

        private void Start()
        {
            character.CharacterDead += OnCharacterDeath;
        }

        private void OnDestroy()
        {
            character.CharacterDead -= OnCharacterDeath;
        }

        private void OnCharacterDeath() => _rotationEnabled = false;

        private void Update()
        {
            var characterPosition = characterTransform.position;
            var transformPosition = transform.position;
            var viewDirection = characterPosition - new Vector3(transformPosition.x, characterPosition.y, transformPosition.z);
            orientation.forward = viewDirection.normalized;

            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            var inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            
            if (inputDirection != Vector3.zero && _rotationEnabled)
                characterObject.forward = Vector3.Slerp(characterObject.forward, inputDirection.normalized, Time.deltaTime * characterRotationSpeed);
        }
    }
}

