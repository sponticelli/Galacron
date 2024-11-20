using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Actors
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int initialHealth = 1;

        public UnityEvent onDeath;
        public UnityEvent<float> onDamage;
        public int CurrentHealth => health;

        private int health;
        
        private void OnEnable()
        {
            health = initialHealth;
        }

        public void TakeDamage(int amount)
        {
            if (health <= 0)
            {
                return;
            }
            health -= amount;
            if (health <= 0)
            {
                Debug.Log($"{gameObject.name} has died!");
                onDeath?.Invoke();
                gameObject.ReturnToPool();
            } else
            {
                onDamage?.Invoke((float)health / initialHealth);
            }
        }
    }
}