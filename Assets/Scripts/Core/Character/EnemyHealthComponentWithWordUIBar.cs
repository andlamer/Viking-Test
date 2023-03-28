using UnityEngine;
using UnityEngine.UI;

namespace VikingTest
{
    public class EnemyHealthComponentWithWordUIBar : CreatureHealthComponent
    {
        [SerializeField] private Slider healthBar;

        public override void SetHealthStats(int minHealth, int maxHealth, int currentHealth)
        {
            base.SetHealthStats(minHealth, maxHealth, currentHealth);
            healthBar.maxValue = maxHealth;
            healthBar.minValue = minHealth;
            healthBar.value = currentHealth;
        }

        private void Start()
        {
            HealthAmountChanged += UpdateHealthBar;
        }

        private void OnDestroy()
        {
            HealthAmountChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int healthPointsAmount) => healthBar.value = healthPointsAmount;
    }
}