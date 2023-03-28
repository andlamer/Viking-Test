using UnityEngine;
using VikingTest.Core;
using Zenject;

namespace VikingTest
{
    public class PrefabsInstaller : MonoInstaller
    {
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private MutantEnemy mutantPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Character>().FromComponentOn(characterPrefab).AsSingle();
            Container.BindMemoryPool<MutantEnemy, MutantEnemy.Pool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(mutantPrefab);
        }
    }
}