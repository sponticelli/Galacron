using UnityEditor;
using UnityEngine;

namespace Galacron.Actors
{
    [CustomEditor(typeof(FormationLayout))]
    public class FormationLayoutEditor : UnityEditor.Editor
    {
        private const float HANDLE_SIZE = 0.03f;

        private FormationLayout layout;
        private SerializedProperty pointsProperty;
        private SerializedProperty visualizeProperty;
        private SerializedProperty snapIncrementProperty;
        private Vector3 dragStartPos;
        private bool isDragging;
        private int selectedPoint = -1;

        private void OnEnable()
        {
            layout = (FormationLayout)target;
            pointsProperty = serializedObject.FindProperty("formationPoints");
            visualizeProperty = serializedObject.FindProperty("visualizeInGame");
            snapIncrementProperty = serializedObject.FindProperty("snapIncrement");
            Tools.hidden = true;
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Grid Settings
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(visualizeProperty);
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(snapIncrementProperty, new GUIContent("Snap Increment"));
            if (EditorGUI.EndChangeCheck())
            {
                snapIncrementProperty.floatValue = Mathf.Max(0.1f, snapIncrementProperty.floatValue);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);

            // Points list
            EditorGUILayout.LabelField("Formation Points", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox("Right-click in scene view to add points\nDrag points to reposition", MessageType.Info);

            DrawPointsList();

            // Add point button
            if (GUILayout.Button("Add Point"))
            {
                AddPoint(Vector3.zero);
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPointsList()
        {
            for (int i = 0; i < pointsProperty.arraySize; i++)
            {
                SerializedProperty point = pointsProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();

                // Point number and position
                EditorGUILayout.LabelField($"Point {i}", GUILayout.Width(50));
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = EditorGUILayout.Vector3Field("", point.FindPropertyRelative("position").vector3Value);
                if (EditorGUI.EndChangeCheck())
                {
                    point.FindPropertyRelative("position").vector3Value = SnapPosition(newPos);
                }

                // Move up/down buttons
                GUI.enabled = i > 0;
                if (GUILayout.Button("↑", GUILayout.Width(20)))
                {
                    layout.ReorderPoints(i, i - 1);
                }

                GUI.enabled = i < pointsProperty.arraySize - 1;
                if (GUILayout.Button("↓", GUILayout.Width(20)))
                {
                    layout.ReorderPoints(i, i + 1);
                }

                GUI.enabled = true;

                // Delete button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    pointsProperty.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void OnSceneGUI()
        {
            DrawSceneHandles();
            HandleSceneInput();
            DrawSelectedPointInfo();
        }

        private void DrawSceneHandles()
        {
            for (int i = 0; i < pointsProperty.arraySize; i++)
            {
                SerializedProperty point = pointsProperty.GetArrayElementAtIndex(i);
                Vector3 worldPos = layout.GetPosition(i);

                // Draw handle
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    point.FindPropertyRelative("position").vector3Value =
                        SnapPosition(layout.transform.InverseTransformPoint(newPos));
                    serializedObject.ApplyModifiedProperties();
                }

                // Draw point
                Handles.color = selectedPoint == i ? Color.yellow : Color.white;
                if (Handles.Button(worldPos, Quaternion.identity, HANDLE_SIZE, HANDLE_SIZE, Handles.DotHandleCap))
                {
                    selectedPoint = i;
                    dragStartPos = worldPos;
                    isDragging = true;
                }

                // Draw number and info
                Handles.Label(worldPos + Vector3.up * 0.4f, i.ToString());
            }
        }

        private void HandleSceneInput()
        {
            // Handle right-click to add points
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                Vector3 point = ray.origin;
                point.z = layout.transform.position.z;
                AddPoint(layout.transform.InverseTransformPoint(point));
                Event.current.Use();
            }
        }

        private void DrawSelectedPointInfo()
        {
            if (selectedPoint >= 0 && selectedPoint < pointsProperty.arraySize)
            {
                var point = pointsProperty.GetArrayElementAtIndex(selectedPoint);
                var worldPos = layout.GetPosition(selectedPoint);

                // Draw info box near selected point
                Handles.BeginGUI();
                var infoPos = HandleUtility.WorldToGUIPoint(worldPos + Vector3.up * 0.6f);
                var content = $"Point {selectedPoint}";
                var style = new GUIStyle(EditorStyles.helpBox);
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleLeft;
                GUI.Box(new Rect(infoPos.x, infoPos.y, 100, 45), content, style);
                Handles.EndGUI();
            }
        }

        private Vector3 SnapPosition(Vector3 position)
        {
            float snap = layout.SnapIncrement;
            return new Vector3(
                Mathf.Round(position.x / snap) * snap,
                Mathf.Round(position.y / snap) * snap,
                position.z
            );
        }

        private void AddPoint(Vector3 localPosition)
        {
            pointsProperty.arraySize++;
            var element = pointsProperty.GetArrayElementAtIndex(pointsProperty.arraySize - 1);
            element.FindPropertyRelative("position").vector3Value = SnapPosition(localPosition);
            element.FindPropertyRelative("speedMultiplier").floatValue = 1f;
            element.FindPropertyRelative("waitTime").floatValue = 0f;
            serializedObject.ApplyModifiedProperties();
        }
    }
}