using UnityEngine;

namespace Galacron.Actors.Enemies
{
    /// <summary>
    /// Configuration for enemy behavior in formations
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Galacron/Formations/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 180f;
        public float positionSmoothTime = 0.1f;
        public float rotationSmoothTime = 0.1f;
        
        [Header("Attack")]
        public float attackDamage = 10f;
        public float attackRange = 2f;
        public Weapon weaponPrefab;
    }
}