using Cinemachine;
using UnityEngine;

namespace VikingTest.Core.Camera
{
    public class DollyCartUnscaledTimeMovement : MonoBehaviour
    {
        [SerializeField] private CinemachineDollyCart dollyCart;

        private bool _isMovementEnabled;

        public void EnableMovement() => _isMovementEnabled = true;
        public void DisableMovement() => _isMovementEnabled = false;
    
        private void Update()
        {
            if (!_isMovementEnabled) return;

            dollyCart.m_Position += Time.unscaledDeltaTime * dollyCart.m_Speed;
        }
    }
}
