using Nexus.Core.Services;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.FloatingTexts
{
    [ServiceInterface]
    public interface IFloatingTextFactory
    {
        FloatingText Create(PoolReference<FloatingText> enemyDeathFloatingText, string message,
            Color enemyDeathFloatingTextColor, Vector3 enemyEventPosition);
    }
}