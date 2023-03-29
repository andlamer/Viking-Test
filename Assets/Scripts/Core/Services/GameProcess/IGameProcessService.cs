namespace VikingTest.Core.Services
{
    public interface IGameProcessService
    {
        void RestartGame();
        void QuitGame();
        void PauseGame();
        void ResumeGame();
        void HideCursor();
        void ShowCursor();
    }
}