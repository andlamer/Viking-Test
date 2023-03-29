using UnityEngine;

namespace VikingTest.Core.ScriptableObjects
{
    internal interface ICharacterSettings
    {
        public int MaxHealthPoints { get; }
        public int StartingHealthPoints { get; }
        public int MinHealthPoints { get; }

        Vector2 PlayerMovementSpeed { get; }
        
        float GravityMultiplier { get; }
        float GroundDistance { get; }
        LayerMask GroundMask { get; }
        
        float DeathDelay { get; }
    }
}