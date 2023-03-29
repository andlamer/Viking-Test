using System;
using UnityEngine;
using UnityEngine.UI;
using VikingTest.Core.Gameplay;
using Zenject;

namespace VikingTest.Core.UI
{
    public class EnemyWorldUIHpBar : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private CreatureHealthComponent creatureHealth;

        private Character _character;

        [Inject]
        private void Construct(Character character)
        {
            _character = character;
        }
        
        private void OnEnable()
        {
            SetHealthStats();
        }

        private void Start()
        {
            creatureHealth.HealthAmountChanged += UpdateHealthBar;
        }

        private void OnDestroy()
        {
            creatureHealth.HealthAmountChanged -= UpdateHealthBar;
        }

        private void Update()
        {
            var gameObjectTransform = healthBar.gameObject.transform;
            gameObjectTransform.LookAt(_character.transform);
            
            var rotation = gameObjectTransform.rotation;
            rotation = Quaternion.Euler(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            gameObjectTransform.rotation = rotation;
        }

        private void SetHealthStats()
        {
            var (minHealth, maxHealth, currentHealth) = creatureHealth.GetMinMaxCurrentHpTuple();
            healthBar.maxValue = maxHealth;
            healthBar.minValue = minHealth;
            healthBar.value = currentHealth;
        }
        
        private void UpdateHealthBar(int healthPointsAmount) => healthBar.value = healthPointsAmount;
    }
}