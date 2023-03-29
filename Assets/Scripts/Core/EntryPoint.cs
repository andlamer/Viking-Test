using UnityEngine;
using VikingTest.Core.Gameplay;
using VikingTest.Core.Services;
using VikingTest.Core.UI;
using Zenject;

namespace VikingTest.Core
{
    public class EntryPoint : MonoBehaviour
    {
        private IEnemySpawnService _enemySpawnService;
        private IStartingMenu _startingMenu;
        private IGameFinishMenu _gameFinishMenu;
        private Character _character;

        [Inject]
        private void Construct(IEnemySpawnService enemySpawnService, IStartingMenu startingMenu, IGameFinishMenu gameFinishMenu, Character character)
        {
            _enemySpawnService = enemySpawnService;
            _startingMenu = startingMenu;
            _gameFinishMenu = gameFinishMenu;
            _character = character;
        }

        private void Start()
        {
            _enemySpawnService.Spawn();
            _startingMenu.Show();

            _character.CharacterDead += OnCharacterDeath;
        }

        private void OnDestroy()
        {
            _character.CharacterDead -= OnCharacterDeath;
        }

        private void OnCharacterDeath()
        {
            _character.CharacterDead -= OnCharacterDeath;
            _gameFinishMenu.Show();
        }
    }   
}
