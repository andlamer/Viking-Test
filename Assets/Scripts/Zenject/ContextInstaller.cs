using VikingTest.Core.Services;
using Zenject;


namespace VikingTest.Zenject
{
    public class ContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IScoreService>().To<ScoreService>().AsSingle();
            Container.Bind<IGameProcessService>().To<GameProcessService>().AsSingle();
            Container.BindInterfacesTo<SpawnService>().AsSingle();
        }
    }
}

