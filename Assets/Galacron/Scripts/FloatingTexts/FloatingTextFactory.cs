using Nexus.Core.ServiceLocation;
using Nexus.Core.Services;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.FloatingTexts
{
    [ServiceImplementation]
    public class FloatingTextFactory : IFloatingTextFactory
    {
        private IPoolingService _poolService;


        public FloatingText Create(PoolReference<FloatingText> poolRef, string message,
            Color color, Vector3 position)
        {
            var floatingText = poolRef.Get(position, Quaternion.identity);
            return Show(message, color, floatingText);
        }
        

        private static FloatingText Show(string message, Color color, FloatingText floatingText)
        {
            if (floatingText == null)
            {
                Debug.LogError("FloatingText is null");
                return null;
            }
            
            floatingText.SetText(message);
            floatingText.SetColor(color);
            floatingText.Animate();
            return floatingText;
        }
    }
}