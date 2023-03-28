using UnityEngine;

namespace VikingTest.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Create MeleeAttackParameters", fileName = "MeleeAttackParameters", order = 0)]
    public class MeleeAttackParameters : ScriptableObject
    {
        [SerializeField] private float attackCooldown = 5f;
        [SerializeField] private float firstSwingDelay = 0.15f;
        [SerializeField] private float swingTime = 1.25f;
        [SerializeField] private float damageTimeWindow = 0.45f;
        [SerializeField] private float attackExitTime = 0.85f;
        [SerializeField] private int damage = 1;

        public float AttackCooldown => attackCooldown;
        public float FirstSwingDelay => firstSwingDelay;
        public float SwingTime => swingTime;
        public float DamageTimeWindow => damageTimeWindow;
        public float AttackExitTime => attackExitTime;
        public int Damage => damage;
    }
}