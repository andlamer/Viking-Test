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

        private Transform _cachedBarTransform;
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
            _cachedBarTransform = healthBar.transform;
        }

        private void OnDestroy()
        {
            creatureHealth.HealthAmountChanged -= UpdateHealthBar;
        }

        private void Update()
        {
            if (UnityEngine.Camera.main != null)
            {
                _cachedBarTransform.LookAt(UnityEngine.Camera.main.transform);
            }
            else
            {
                _cachedBarTransform.LookAt(_character.transform);
                
                var rotation = _cachedBarTransform.rotation;
                rotation = Quaternion.Euler(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
                _cachedBarTransform.rotation = rotation;
            }
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