using UnityEngine;

namespace Galacron.Paths
{
    /// <summary>
    /// Represents a point in the path with additional metadata
    /// </summary>
    [System.Serializable]
    public struct PathPoint
    {
        public Vector3 position;      // Stored in local space
        public float waitTime;
        public float speedMultiplier;
        
        
        public PathPoint(Vector3 position = default)
        {
            this.position = position;
            waitTime = 0f;
            speedMultiplier = 1f;
        }
        
        public PathPoint(Vector3 worldPosition, Transform relativeTo)
        {
            position = PathUtils.ToLocalSpace(relativeTo, worldPosition);
            waitTime = 0f;
            speedMultiplier = 1f;
        }
    }
}