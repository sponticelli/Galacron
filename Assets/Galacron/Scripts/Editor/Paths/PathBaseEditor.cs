using UnityEngine;

namespace Galacron.Paths
{
    [UnityEditor.CustomEditor(typeof(PathBase), true)]
    public class PathBaseEditor : UnityEditor.Editor
    {
        protected virtual void OnSceneGUI()
        {
            var path = target as PathBase;
            if (path == null) return;

            for (int i = 0; i < path.GetWorldSpacePoints().Count; i++)
            {
                Vector3 worldPos = path.GetWorldPointPosition(i);
                
                UnityEditor.EditorGUI.BeginChangeCheck();
                Vector3 newPos = UnityEditor.Handles.PositionHandle(worldPos, Quaternion.identity);
                
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    UnityEditor.Undo.RecordObject(path, "Move Path Point");
                    path.UpdatePoint(i, newPos);
                    UnityEditor.EditorUtility.SetDirty(path);
                }
            }
        }
    }
}