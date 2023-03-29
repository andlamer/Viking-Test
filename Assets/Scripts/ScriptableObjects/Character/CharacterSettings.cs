using UnityEngine;
using Zenject;

namespace VikingTest.Core.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CharacterSettings", fileName = "CharacterSettings", order = 0)]
    public class CharacterSettings : ScriptableObjectInstaller, ICharacterSettings
    {
        [Header("Health")] 
        [SerializeField] private int maxHealthPoints = 20;
        [SerializeField] private int startingHealthPoints = 20;
        [SerializeField] private int minHealthPoints = 0;

        [Header("Movement")]
        [SerializeField] private float xMovementSpeed = 1f;
        [SerializeField] private float yMovementSpeed = 1f;
        [SerializeField] private float gravityMultiplier = 1;
        [SerializeField] private float groundDistance = 0.1f;
        [SerializeField] private LayerMask groundMask;

        [Header("Death")]
        [SerializeField] private float deathDelay = 6f;
        
        public int MaxHealthPoints => maxHealthPoints;
        public int StartingHealthPoints => startingHealthPoints;
        public int MinHealthPoints => minHealthPoints;
        public Vector2 PlayerMovementSpeed => new(xMovementSpeed, yMovementSpeed);
        public float GravityMultiplier => gravityMultiplier;
        public float GroundDistance => groundDistance;
        public LayerMask GroundMask => groundMask;
        public float DeathDelay => deathDelay;

        public override void InstallBindings()
        {
            Container.Bind<ICharacterSettings>().FromInstance(this).AsSingle();
        }
    }
}