using System.Collections.Generic;
using UnityEngine;

namespace Galacron.Paths
{
    /// <summary>
    /// Bezier curve path with control points
    /// </summary>
    public class BezierPath : PathBase
    {
        [Header("Bezier Settings")]
        [Range(1, 20)]
        [SerializeField] private int lineDensity = 10;
        
        public override void RecalculatePath()
        {
            calculatedWorldPoints.Clear();
            pathLength = 0f;
            
            if (pathPoints.Count < 2) return;
            
            var worldPoints = new List<Vector3>();
            foreach (var point in pathPoints)
            {
                worldPoints.Add(PathUtils.ToWorldSpace(transform, point.position));
            }
            
            if (isLooped)
            {
                worldPoints.Add(worldPoints[0]);
                if (worldPoints.Count % 2 == 0)
                {
                    worldPoints.Add(worldPoints[1]);
                }
                else 
                {
                    worldPoints.Add(worldPoints[0]);
                    worldPoints.Add(worldPoints[1]);
                }
            }
            
            for (int i = 0; i < worldPoints.Count - 2; i += 2)
            {
                for (int j = 0; j <= lineDensity; j++)
                {
                    float t = j / (float)lineDensity;
                    calculatedWorldPoints.Add(GetBezierPoint(
                        worldPoints[i], 
                        worldPoints[i + 1], 
                        worldPoints[i + 2], 
                        t
                    ));
                }
            }
            
            for (int i = 0; i < calculatedWorldPoints.Count - 1; i++)
            {
                pathLength += Vector3.Distance(calculatedWorldPoints[i], calculatedWorldPoints[i + 1]);
            }
            
            isDirty = false;
        }
        
        private Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * p0 + 
                   2f * oneMinusT * t * p1 + 
                   t * t * p2;
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
            
            float totalLength = GetTotalLength();
            float normalizedDistance = distance / totalLength;
            
            // Find the control point segment we're in
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                float segmentStart = i / (float)(pathPoints.Count - 1);
                float segmentEnd = (i + 1) / (float)(pathPoints.Count - 1);
                
                if (normalizedDistance >= segmentStart && normalizedDistance <= segmentEnd)
                {
                    float t = (normalizedDistance - segmentStart) / (segmentEnd - segmentStart);
                    return Mathf.Lerp(pathPoints[i].speedMultiplier, pathPoints[i + 1].speedMultiplier, t);
                }
            }
            
            return pathPoints[^1].speedMultiplier;
        }
    }
}