using UnityEngine;

namespace Galacron.Paths
{
    /// <summary>
    /// Simple linear path between points
    /// </summary>
    public class LinearPath : PathBase
    {
        public override void RecalculatePath()
        {
            calculatedWorldPoints.Clear();
            pathLength = 0f;
            
            if (pathPoints.Count < 2) return;
            
            foreach (var point in pathPoints)
            {
                calculatedWorldPoints.Add(PathUtils.ToWorldSpace(transform, point.position));
            }
            
            for (int i = 0; i < calculatedWorldPoints.Count - 1; i++)
            {
                pathLength += Vector3.Distance(calculatedWorldPoints[i], calculatedWorldPoints[i + 1]);
            }
            
            if (isLooped && calculatedWorldPoints.Count > 2)
            {
                pathLength += Vector3.Distance(calculatedWorldPoints[^1], calculatedWorldPoints[0]);
            }
            
            isDirty = false;
        }
        
        public override float GetTotalLength() => pathLength;
        
        public override Vector3 GetPointAtDistance(float distance)
        {
            if (calculatedWorldPoints.Count < 2) return Vector3.zero;
            
            float currentDistance = 0f;
            for (int i = 0; i < calculatedWorldPoints.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(calculatedWorldPoints[i], calculatedWorldPoints[i + 1]);
                
                if (currentDistance + segmentLength >= distance)
                {
                    float t = (distance - currentDistance) / segmentLength;
                    return Vector3.Lerp(calculatedWorldPoints[i], calculatedWorldPoints[i + 1], t);
                }
                
                currentDistance += segmentLength;
            }
            
            return calculatedWorldPoints[^1];
        }
        
        public override float GetSpeedMultiplierAtDistance(float distance)
        {
            if (pathPoints.Count < 2) return 1f;
            
            float currentDistance = 0f;
            for (int i = 0; i < calculatedWorldPoints.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(calculatedWorldPoints[i], calculatedWorldPoints[i + 1]);
                if (currentDistance + segmentLength >= distance)
                {
                    return pathPoints[i].speedMultiplier;
                }
                currentDistance += segmentLength;
            }
            
            return pathPoints[^1].speedMultiplier;
        }
    }
}