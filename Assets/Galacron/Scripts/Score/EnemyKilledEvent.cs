using UnityEngine;

namespace Galacron.Score
{
    public struct EnemyKilledEvent
    {
        public int PointValue { get; }
        public Vector3 Position { get; }
        
        public EnemyKilledEvent(int pointValue, Vector3 position)
        {
            PointValue = pointValue;
            Position = position;
        }
    }
}