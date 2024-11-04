using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// Configuration for formation movement paths
    /// </summary>
    [System.Serializable]
    public class PathConfig
    {
        public PathType pathType = PathType.Linear;
        public Vector2[] controlPoints;
        public float duration = 1f;
        public AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public bool orientToPath = true;
    }
}