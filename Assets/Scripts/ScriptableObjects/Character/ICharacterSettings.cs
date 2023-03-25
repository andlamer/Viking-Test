using UnityEngine;

namespace VikingTest
{
    internal interface ICharacterSettings
    {
        Vector2 PlayerMovementSpeed { get; }
        
        float GravityMultiplier { get; }
        float GroundDistance { get; }
        LayerMask GroundMask { get; }
        int AttackDamage { get; }
        float AttackAnimationSpeed { get; }
    }
}