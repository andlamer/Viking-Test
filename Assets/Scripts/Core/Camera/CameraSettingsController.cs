using UnityEngine;

namespace VikingTest.Core.Camera
{
    public class CameraSettingsController : MonoBehaviour, ICameraSettingsController
    {
        [SerializeField] private GameObject thirdPersonCameraSettings;
        [SerializeField] private GameObject dollyCartCameraSettings;
        [SerializeField] private DollyCartUnscaledTimeMovement dollyCartUnscaledTimeMovement;

        public void EnableThirdPersonCameraSettings()
        {
            thirdPersonCameraSettings.SetActive(true);
            dollyCartCameraSettings.SetActive(false);
            dollyCartUnscaledTimeMovement.DisableMovement();
        }

        public void EnableDollyCartCameraSettings()
        {
            thirdPersonCameraSettings.SetActive(false);
            dollyCartCameraSettings.SetActive(true);
            dollyCartUnscaledTimeMovement.EnableMovement();
        }
    }
}