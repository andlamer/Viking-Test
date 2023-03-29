using UnityEngine;
using UnityEngine.SceneManagement;

namespace VikingTest.Core.Services
{
    public class GameProcessService : IGameProcessService
    {
        public void RestartGame()
        {
            var currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneBuildIndex, LoadSceneMode.Single);
        }

        public void QuitGame() => Application.Quit();
        
        public void PauseGame() => Time.timeScale = 0;
        public void ResumeGame() => Time.timeScale = 1;

        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
