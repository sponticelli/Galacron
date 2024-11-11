using System.Collections.Generic;
using UnityEngine;

namespace Galacron.Paths
{
    /// <summary>
    /// Base implementation for common path functionality
    /// </summary>
        public abstract class PathBase : MonoBehaviour, IPath
    {
        [Header("Path Settings")]
        [SerializeField] protected bool isLooped;
        [SerializeField] protected Color pathColor = Color.green;
        [SerializeField] protected List<PathPoint> pathPoints = new List<PathPoint>();
        
        [Header("Debug")]
        [SerializeField] protected bool visualizePath = true;
        
        protected List<Vector3> calculatedWorldPoints = new List<Vector3>();
        protected bool isDirty = true;
        protected float pathLength;
        
        public bool IsLooped => isLooped;
        public Color GizmoColor => pathColor;

        public IReadOnlyList<Vector3> GetWorldSpacePoints()
        {
            if (isDirty)
            {
                RecalculatePath();
            }
            return calculatedWorldPoints;
        }

        protected virtual void OnValidate()
        {
            isDirty = true;
        }

        protected virtual void OnEnable()
        {
            RecalculatePath();
        }

        public void AddPoint(Vector3 worldPosition)
        {
            pathPoints.Add(new PathPoint(worldPosition, transform));
            isDirty = true;
        }

        public void UpdatePoint(int index, Vector3 worldPosition)
        {
            if (index >= 0 && index < pathPoints.Count)
            {
                var point = pathPoints[index];
                point.position = PathUtils.ToLocalSpace(transform, worldPosition);
                pathPoints[index] = point;
                isDirty = true;
            }
        }

        public Vector3 GetWorldPointPosition(int index)
        {
            if (index >= 0 && index < pathPoints.Count)
            {
                return PathUtils.ToWorldSpace(transform, pathPoints[index].position);
            }
            return Vector3.zero;
        }

        private void OnTransformParentChanged()
        {
            isDirty = true;
        }

        private void OnDidApplyAnimationProperties()
        {
            isDirty = true;
        }

        public abstract float GetTotalLength();
        public abstract Vector3 GetPointAtDistance(float distance);
        public abstract float GetSpeedMultiplierAtDistance(float distance);
        public abstract void RecalculatePath();

#if UNITY_EDITOR
        
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        private Vector3 lastScale;
        
        protected virtual void OnDrawGizmos()
        {
            if (transform.position != lastPosition ||
                transform.rotation != lastRotation ||
                transform.localScale != lastScale)
            {
                isDirty = true;
                lastPosition = transform.position;
                lastRotation = transform.rotation;
                lastScale = transform.localScale;
            }
            
            if (!visualizePath) return;
            
            Gizmos.color = pathColor;
            var worldPoints = GetWorldSpacePoints();
            
            // Draw points
            foreach (var point in pathPoints)
            {
                Gizmos.DrawWireSphere(PathUtils.ToWorldSpace(transform, point.position), 0.3f);
            }
            
            // Draw path lines
            if (worldPoints.Count > 1)
            {
                for (int i = 0; i < worldPoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(worldPoints[i], worldPoints[i + 1]);
                }
                
                if (isLooped && worldPoints.Count > 2)
                {
                    Gizmos.DrawLine(worldPoints[^1], worldPoints[0]);
                }
            }
        }
#endif
    }
}