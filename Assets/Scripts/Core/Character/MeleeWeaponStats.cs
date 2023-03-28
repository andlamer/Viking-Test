using System.Collections.Generic;
using UnityEngine;
using VikingTest.Core;

namespace VikingTest
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Create MeleeWeaponStats", fileName = "MeleeWeaponStats", order = 0)]
    public class MeleeWeaponStats : ScriptableObject
    {
        [SerializeField] private MeleeAttackParameters attackParameters;
        [SerializeField] private List<string> damageTargetTags;

        public MeleeAttackParameters AttackParameters => attackParameters;
        public List<string> DamageTargetTags => damageTargetTags;
    }
}