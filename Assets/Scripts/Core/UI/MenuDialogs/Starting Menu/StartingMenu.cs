using UnityEngine;
using UnityEngine.UI;
using VikingTest.Core.Camera;
using VikingTest.Core.Services;
using Zenject;

namespace VikingTest.Core.UI
{
    public class StartingMenu : MonoBehaviour, IStartingMenu
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button exitButton;

        private IGameProcessService _gameProcessService;
        private ICameraSettingsController _cameraSettingsController;
        private IGameInfoUIController _gameInfoUIController;

        [Inject]
        private void Construct(IGameProcessService gameProcessService, ICameraSettingsController cameraSettingsController, IGameInfoUIController gameInfoUIController)
        {
            _gameProcessService = gameProcessService;
            _cameraSettingsController = cameraSettingsController;
            _gameInfoUIController = gameInfoUIController;
        }

        private void Start()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _gameProcessService.PauseGame();
            _gameProcessService.ShowCursor();
            _cameraSettingsController.EnableDollyCartCameraSettings();
            _gameInfoUIController.DisableGameInfoUI();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _gameProcessService.ResumeGame();
            _gameProcessService.HideCursor();
            _cameraSettingsController.EnableThirdPersonCameraSettings();
            _gameInfoUIController.EnableGameInfoUI();
        }

        private void OnPlayButtonClicked() => Hide();
        private void OnExitButtonClicked() => _gameProcessService.QuitGame();
    }
}