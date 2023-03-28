using UnityEngine;
using Zenject;

namespace VikingTest
{
    public class PrefabsInstaller : MonoInstaller
    {
        [SerializeField] private GameObject characterPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Character>().FromComponentOn(characterPrefab).AsSingle();
        }
    }
}