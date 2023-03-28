using System;
using UnityEngine;

namespace VikingTest
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private Transform characterObject;
        [SerializeField] private float characterRotationSpeed;
        [SerializeField] private Character character;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            var characterPosition = characterTransform.position;
            var transformPosition = transform.position;
            var viewDirection = characterPosition - new Vector3(transformPosition.x, characterPosition.y, transformPosition.z);
            orientation.forward = viewDirection.normalized;

            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            var inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            
            if (inputDirection != Vector3.zero && !character.IsDead)
                characterObject.forward = Vector3.Slerp(characterObject.forward, inputDirection.normalized, Time.deltaTime * characterRotationSpeed);
        }
    }
}

