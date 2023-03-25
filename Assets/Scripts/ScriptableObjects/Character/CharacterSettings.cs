using UnityEngine;
using Zenject;

namespace VikingTest.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
    public class CharacterSettings : ScriptableObjectInstaller, ICharacterSettings
    {
        [Header("Movement")]
        [SerializeField] private float xMovementSpeed = 1f;
        [SerializeField] private float yMovementSpeed = 1f;
        [SerializeField] private float gravityMultiplier = 1;
        [SerializeField] private float groundDistance = 0.1f;
        [SerializeField] private LayerMask groundMask;

        [Header("Attack")] 
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private float attackAnimationTime = 2.5f;

        public Vector2 PlayerMovementSpeed => new(xMovementSpeed, yMovementSpeed);
        public float GravityMultiplier => gravityMultiplier;
        public float GroundDistance => groundDistance;
        public LayerMask GroundMask => groundMask;
        public int AttackDamage => attackDamage;
        public float AttackAnimationSpeed => attackAnimationTime;
        
        public override void InstallBindings()
        {
            Container.Bind<ICharacterSettings>().FromInstance(this).AsSingle();
        }
    }
}