using System;
using UnityEngine;

namespace VikingTest
{
    public class CreatureHealthComponent : MonoBehaviour, IDamageable, IHealable
    {
        public event Action MinHealthReached;
        public event Action<int> HealthAmountChanged;
        public event Action DamageTaken;

        private int _currentHealth;
        private int _maxHealth;
        private int _minHealth;
        
        public void SetHealthStats(int minHealth, int maxHealth, int currentHealth)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;
            _minHealth = minHealth;
        }

        public void TakeDamage(int damageNum)
        {
            _currentHealth = Math.Clamp(_currentHealth - damageNum, _minHealth, _maxHealth);
            HealthAmountChanged?.Invoke(_currentHealth);

            if (_currentHealth == _minHealth)
            {
                MinHealthReached?.Invoke();
            }
            else
            {
                DamageTaken?.Invoke();
            }
        }
        
        public void Heal(int healNum)
        {
            _currentHealth = Math.Clamp(_currentHealth + healNum, _minHealth, _maxHealth);
            HealthAmountChanged?.Invoke(_currentHealth);
        }
    }
}