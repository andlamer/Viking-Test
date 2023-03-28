using VikingTest.Services;
using Zenject;


namespace VikingTest
{
    public class ContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IScoreService>().To<ScoreService>().AsSingle();
            Container.Bind<IEnemySpawnService>().To<EnemySpawnService>().AsSingle();
        }
    }
}

