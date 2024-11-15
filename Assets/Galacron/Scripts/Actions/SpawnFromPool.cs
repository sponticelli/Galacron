using Nexus.Core.ServiceLocation;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actions
{
    public class SpawnFromPool : MonoBehaviour
    {
        [SerializeField] private Pools pool;
        
        public void Execute()
        {
            var poolName = PoolIdConverter.GetId(pool);
            var poolService = ServiceLocator.Instance.GetService<IPoolingService>();
            
            var obj = poolService.GetFromPool(poolName, transform.position, transform.rotation);
        }
    }
}