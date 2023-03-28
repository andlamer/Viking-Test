using System.Collections.Generic;
using UnityEngine;

namespace VikingTest.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Create MutantEnemyAttackZoneStats", fileName = "MutantEnemyAttackZoneStats", order = 0)]
    public class AttackZoneStats : ScriptableObject
    {
        [Header("Attack")] 
        [SerializeField] private MeleeAttackParameters attackParameters;
        [SerializeField] private List<string> damageTargetTags = new (){"Player"};

        public MeleeAttackParameters AttackParameters => attackParameters;
        public List<string> DamageTargetTags => damageTargetTags;
    }
}