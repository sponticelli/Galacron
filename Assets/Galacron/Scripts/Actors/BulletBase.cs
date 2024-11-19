using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actors
{
    public class BulletBase : MonoBehaviour
    {
        [SerializeField]
        private Vector3 velocity;
        [SerializeField]
        private int damage = 1;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // Ensure proper Rigidbody2D setup
            if (rb != null)
            {
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
            }
        }

        public Vector3 Velocity
        {
            get => velocity;
            set 
            { 
                velocity = value;
                if (rb != null)
                {
                    rb.velocity = velocity;
                }
            }
        }
        
        private void OnEnable()
        {
            if (rb != null)
            {
                rb.velocity = velocity;
            }
        }

        public void SetDamage(int bulletDamage)
        {
            damage = bulletDamage;
        }

        private void HandleHit(GameObject hitObject, Vector2 hitPoint)
        {
            Debug.Log($"Bullet hit {hitObject.name} at {hitPoint}");

            // If not a shield, check for Health component
            var health = hitObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                gameObject.ReturnToPool();
                return;
            }

            // If we hit something without either component, still return to pool
            gameObject.ReturnToPool();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleHit(other.gameObject, other.transform.position);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector2 hitPoint = collision.contacts[0].point;
            HandleHit(collision.gameObject, hitPoint);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Visualize the bullet's collider and velocity
            Gizmos.color = Color.yellow;
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
                // Draw velocity direction
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + velocity.normalized);
            }
        }
#endif
    }
}