using UnityEngine;

namespace VikingTest.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Create EnemyStats", fileName = "EnemyStats", order = 0)]
    public class EnemyStats : ScriptableObject
    {
        [Header("Health")] 
        [SerializeField] private int maxHealthPoints = 1;
        [SerializeField] private int startingHealthPoints = 1;
        [SerializeField] private int minHealthPoints = 0;

        [Header("AI")] 
        [SerializeField] private float targetAIUpdateTime = 0.1f;
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float angularSpeed = 180f;
        [SerializeField] private float speed = 4f;
        [SerializeField] private float stoppingDistance = 1.25f;
        


        public int MaxHealthPoints => maxHealthPoints;
        public int StartingHealthPoints => startingHealthPoints;
        public int MinHealthPoints => minHealthPoints;
        public float TargetAIUpdateTime => targetAIUpdateTime;
        public float Acceleration => acceleration;
        public float AngularSpeed => angularSpeed;
        public float Speed => speed;
        public float StoppingDistance => stoppingDistance;
    }
}