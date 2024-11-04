using Galacron.Formations;
using UnityEngine;

namespace Galacron.Actors.Enemies
{
    /// <summary>
    /// Enemy that can be part of a formation
    /// </summary>
    public class FormationEnemy : MonoBehaviour, IMember, IAttackingMember
    {
        [Header("Configuration")]
        [SerializeField] private EnemyConfig config;

        [Header("References")]
        [SerializeField] private Transform visualsTransform;
        [SerializeField] private Transform weaponMount;

        private IMemberState currentState;
        private IFormation currentFormation;
        private Vector2 targetPosition;
        private Vector2 currentVelocity;
        private float currentRotationVelocity;
        private Weapon weapon;
        
        public bool IsAttacking { get; private set; }

        private void Start()
        {
            // Initialize weapon if configured
            if (config.weaponPrefab != null)
            {
                weapon = Instantiate(config.weaponPrefab, weaponMount);
            }
        }

        private void Update()
        {
            // Update current state
            currentState?.Update(this, Time.deltaTime);

            // Smooth movement to target position
            Vector2 currentPosition = transform.position;
            Vector2 newPosition = Vector2.SmoothDamp(
                currentPosition, 
                targetPosition,
                ref currentVelocity,
                config.positionSmoothTime
            );
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            // Calculate and apply rotation based on movement
            if (currentVelocity.sqrMagnitude > 0.01f)
            {
                float targetRotation = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg - 90f;
                float currentRotation = visualsTransform.rotation.eulerAngles.z;

                // Normalize the angle
                if (currentRotation > 180f) currentRotation -= 360f;

                float newRotation = Mathf.SmoothDampAngle(
                    currentRotation,
                    targetRotation,
                    ref currentRotationVelocity,
                    config.rotationSmoothTime
                );

                visualsTransform.rotation = Quaternion.Euler(0, 0, newRotation);
            }
        }

        public void SetFormationPosition(Vector2 position)
        {
            targetPosition = position;
        }

        public Vector2 GetCurrentPosition()
        {
            return transform.position;
        }

        public void OnFormationAssigned(IFormation formation)
        {
            currentFormation = formation;
        }

        public void SetState(IMemberState state)
        {
            currentState?.Exit(this);
            currentState = state;
            currentState?.Enter(this);
        }

        public void StartAttack()
        {
            IsAttacking = true;
            if (weapon != null)
            {
                weapon.Shoot();
            }
        }

        public void EndAttack()
        {
            IsAttacking = false;
            if (weapon != null)
            {
                weapon.StopShooting();
            }
        }

        private void OnDestroy()
        {
            // Notify formation when destroyed
            currentFormation?.RemoveMember(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (config == null) return;
            
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.attackRange);
            
            // Draw target position
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(targetPosition, 0.2f);
                
                // Draw velocity
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + currentVelocity);
            }
        }
#endif
    }
}