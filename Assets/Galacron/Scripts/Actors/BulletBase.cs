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


        public Vector3 Velocity
        {
            get => velocity;
            set => velocity = value;
        }
        
        void Update()
        {
            transform.position += velocity * Time.deltaTime;
        }

        public void SetDamage(int bulletDamage)
        {
            damage = bulletDamage;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<Health>();
            DamageTarget(health);
        }

        private void DamageTarget(Health health)
        {
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            gameObject.ReturnToPool();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var health = other.gameObject.GetComponent<Health>();
            DamageTarget(health);
        }
        
    }
}