using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Actors
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private int health = 1;

        public UnityEvent onDeath;
        
        
        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                onDeath?.Invoke();
                gameObject.ReturnToPool();
            }
        }
    }
}