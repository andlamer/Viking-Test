using UnityEngine;
using VikingTest.Core.UI;
using Zenject;

namespace VikingTest.Zenject
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private GameInfoUIController gameInfoUI;
        [SerializeField] private StartingMenu startingMenu;
        [SerializeField] private GameFinishMenu gameFinishMenu;

        public override void InstallBindings()
        {
            Container.Bind<IGameInfoUIController>().FromInstance(gameInfoUI).AsSingle();
            Container.Bind<IStartingMenu>().FromInstance(startingMenu).AsSingle();
            Container.Bind<IGameFinishMenu>().FromInstance(gameFinishMenu).AsSingle();
        }
    }
}