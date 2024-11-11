using UnityEngine;

namespace Galacron.Paths
{
    public static class PathUtils
    {
        // Helper methods for coordinate conversions
        public static Vector3 ToWorldSpace(Transform transform, Vector3 localPoint)
        {
            return transform.TransformPoint(localPoint);
        }
        
        public static Vector3 ToLocalSpace(Transform transform, Vector3 worldPoint)
        {
            return transform.InverseTransformPoint(worldPoint);
        }
    }
}