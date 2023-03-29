using System;

namespace VikingTest.Core.Gameplay
{
    public class HealableHealthComponent : CreatureHealthComponent, IHealable
    {
        public void Heal(int healNum)
        {
            CurrentHealth = Math.Clamp(CurrentHealth + healNum, MinHealth, MaxHealth);
            InformAboutHpChange();
        }
    }
}