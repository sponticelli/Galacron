using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actions
{
    public class SpawnFromPool : MonoBehaviour
    {
        [SerializeField] private PoolReference<MonoBehaviour> pool;
        
        
        public void Execute()
        {
            pool.Get(transform.position, transform.rotation);
        }
    }
}