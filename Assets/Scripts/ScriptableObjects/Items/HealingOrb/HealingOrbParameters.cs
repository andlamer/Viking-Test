using System.Collections.Generic;
using UnityEngine;

namespace VikingTest.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Create HealingOrbParameters", fileName = "HealingOrbParameters", order = 0)]
    public class HealingOrbParameters : ScriptableObject
    {
        [SerializeField] private int healthRecovery;
        [SerializeField] private List<string> healTargets;
        [SerializeField] private float autoDestroyTime = 3f;

        public int HealthRecovery => healthRecovery;
        public List<string> HealTargets => healTargets;
        public float AutoDestroyTime => autoDestroyTime;
    }
}