using System;
using UnityEngine;

namespace VikingTest.Core.Gameplay
{
    public class CreatureHealthComponent : MonoBehaviour, IDamageable
    {
        public event Action MinHealthReached;
        public event Action DamageTaken;

        public event Action<int> HealthAmountChanged;

        protected int CurrentHealth;
        protected int MaxHealth;
        protected int MinHealth;
        
        public void SetHealthStats(int minHealth, int maxHealth, int currentHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            MinHealth = minHealth;
        }

        public (int, int, int) GetMinMaxCurrentHpTuple() => (MinHealth, MaxHealth, CurrentHealth);

        public void TakeDamage(int damageNum)
        {
            CurrentHealth = Math.Clamp(CurrentHealth - damageNum, MinHealth, MaxHealth);
            InformAboutHpChange();

            if (CurrentHealth == MinHealth)
            {
                MinHealthReached?.Invoke();
            }
            else
            {
                DamageTaken?.Invoke();
            }
        }
        
        protected void InformAboutHpChange() => HealthAmountChanged?.Invoke(CurrentHealth);
    }
}