using UnityEngine;
using VikingTest.Core.Camera;
using VikingTest.Core.Gameplay;
using Zenject;

namespace VikingTest.Zenject
{
    public class PrefabsInstaller : MonoInstaller
    {
        [SerializeField] private Character characterObject;
        [SerializeField] private CameraSettingsController cameraSettingsController;
        [SerializeField] private MutantEnemy mutantPrefab;
        [SerializeField] private HealingOrb healingOrbPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Character>().FromInstance(characterObject).AsSingle();
            Container.Bind<ICameraSettingsController>().FromInstance(cameraSettingsController).AsSingle();
            
            Container.BindMemoryPool<MutantEnemy, MutantEnemy.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(mutantPrefab);
            Container.BindMemoryPool<HealingOrb, HealingOrb.Pool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(healingOrbPrefab);
        }
    }
}