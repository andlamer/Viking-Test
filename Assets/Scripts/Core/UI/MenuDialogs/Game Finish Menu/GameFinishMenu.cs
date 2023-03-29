using System;
using UnityEngine;
using UnityEngine.UI;
using VikingTest.Core.Camera;
using VikingTest.Core.Services;
using Zenject;

namespace VikingTest.Core.UI
{
    public class GameFinishMenu : MonoBehaviour, IGameFinishMenu
    {
        [SerializeField] private Button restartButton;
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
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
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

        public void Hide() {}

        private void OnRestartButtonClicked() => _gameProcessService.RestartGame();
        private void OnExitButtonClicked() => _gameProcessService.QuitGame();
    }
}