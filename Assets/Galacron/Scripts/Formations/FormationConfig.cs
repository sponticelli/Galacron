using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// Configuration for the formation's behavior
    /// </summary>
    [CreateAssetMenu(fileName = "FormationConfig", menuName = "Galacron/Formations/Formation Config")]
    public class FormationConfig : ScriptableObject
    {
        [Header("Formation Settings")]
        public Vector2[] slotPositions;
        public float spacing = 1f;
        public float arrivalDelayBetweenMembers = 0.2f;
        
        [Header("Movement Settings")]
        public float idleAmplitude = 0.1f;
        public float idleFrequency = 1f;
        public float moveSpeed = 5f;
        public float rotationSpeed = 180f;
        
        [Header("Attack Settings")]
        public float attackCooldown = 2f;
        public int maxSimultaneousAttackers = 2;
        public float attackProbability = 0.3f;
        
        [Header("Paths")]
        public PathConfig entryPath;
        public PathConfig attackPath;
        public PathConfig returnPath;
    }
}