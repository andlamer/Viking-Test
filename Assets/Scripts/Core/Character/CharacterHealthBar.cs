using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace VikingTest
{
    public class CharacterHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;

        private ICharacterSettings _characterSettings;
        private Character _cachedCharacter;

        [Inject]
        private void Construct(ICharacterSettings characterSettings, Character character)
        {
            _characterSettings = characterSettings;
            _cachedCharacter = character;
        }

        private void Start()
        {
            _cachedCharacter.CharacterHealth.HealthAmountChanged += UpdateHealthBar;
            healthBar.maxValue = _characterSettings.MaxHealthPoints;
            healthBar.minValue = _characterSettings.MinHealthPoints;
            healthBar.value = _characterSettings.StartingHealthPoints;
        }

        private void OnDestroy()
        {
            _cachedCharacter.CharacterHealth.HealthAmountChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int healthPointsAmount) => healthBar.value = healthPointsAmount;
    }
}