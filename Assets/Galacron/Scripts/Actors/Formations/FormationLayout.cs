// FormationLayout.cs
using UnityEngine;
using System.Collections.Generic;

namespace Galacron.Actors
{
    public class FormationLayout : MonoBehaviour, IFormationLayout 
    {
        [System.Serializable]
        public struct FormationPoint
        {
            public Vector3 position;
        }

        [SerializeField] private List<FormationPoint> formationPoints = new List<FormationPoint>();
        [SerializeField] private bool visualizeInGame = true;
        [SerializeField] private float snapIncrement = 0.5f;  // Added snap increment

        public float SnapIncrement => snapIncrement;  // Expose for editor
        public int PointCount => formationPoints.Count;

        public Vector3 GetPosition(int index)
        {
            if (index < 0 || index >= formationPoints.Count)
                return transform.position;
            
            return transform.TransformPoint(formationPoints[index].position);
        }

        public void ReorderPoints(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= formationPoints.Count || 
                newIndex < 0 || newIndex >= formationPoints.Count)
                return;

            var point = formationPoints[oldIndex];
            formationPoints.RemoveAt(oldIndex);
            formationPoints.Insert(newIndex, point);
        }

        private void OnDrawGizmos()
        {
            if (!visualizeInGame) return;

            // Draw grid
            DrawGrid(20);

            // Draw points
            for (int i = 0; i < formationPoints.Count; i++)
            {
                var worldPos = GetPosition(i);
            
                // Draw points
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(worldPos, 0.3f);

                // Draw order numbers
#if UNITY_EDITOR
                UnityEditor.Handles.Label(worldPos + Vector3.up * 0.5f, i.ToString());
#endif
            }
        }

        private void DrawGrid(int size)
        {
            if (snapIncrement <= 0) return;
            
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            Vector3 center = transform.position;

            for (float x = -size; x <= size; x += snapIncrement)
            {
                Gizmos.DrawLine(
                    new Vector3(center.x + x, center.y - size, center.z),
                    new Vector3(center.x + x, center.y + size, center.z)
                );
            }

            for (float y = -size; y <= size; y += snapIncrement)
            {
                Gizmos.DrawLine(
                    new Vector3(center.x - size, center.y + y, center.z),
                    new Vector3(center.x + size, center.y + y, center.z)
                );
            }
        }
    }
}