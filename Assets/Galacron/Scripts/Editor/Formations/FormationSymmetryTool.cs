using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Galacron.Formations
{
    /// <summary>
    /// Handles symmetry operations and visualization for formation editing
    /// </summary>
    public class FormationSymmetryTool
    {
        private enum SymmetryMode
        {
            None,
            Horizontal,
            Vertical,
            Diagonal,
            Radial
        }

        // UI State
        private bool showSymmetryTools = true;
        private SymmetryMode currentSymmetryMode = SymmetryMode.None;

        // Symmetry Settings
        private Vector2 symmetryAxisOrigin = Vector2.zero;
        private float symmetryAngle = 0f;
        private int radialSegments = 4;
        private float radialRadius = 2f;

        // Constants
        private const float HANDLE_SIZE = 0.5f;
        private const float GUIDE_LENGTH = 10f;
        private const int MIN_SEGMENTS = 2;
        private const int MAX_SEGMENTS = 12;

        public void OnInspectorGUI(SerializedProperty slotsProperty)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showSymmetryTools = EditorGUILayout.Foldout(showSymmetryTools, "Symmetry Tools", true);

            if (showSymmetryTools)
            {
                DrawSymmetryControls(slotsProperty);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSymmetryControls(SerializedProperty slotsProperty)
        {
            EditorGUI.indentLevel++;

            // Mode selection
            EditorGUI.BeginChangeCheck();
            currentSymmetryMode = (SymmetryMode)EditorGUILayout.EnumPopup("Symmetry Mode", currentSymmetryMode);

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            // Mode-specific settings
            if (currentSymmetryMode != SymmetryMode.None)
            {
                EditorGUILayout.Space(5);
                DrawModeSettings();
                EditorGUILayout.Space(5);
                DrawSymmetryButtons(slotsProperty);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawModeSettings()
        {
            switch (currentSymmetryMode)
            {
                case SymmetryMode.Horizontal:
                case SymmetryMode.Vertical:
                case SymmetryMode.Diagonal:
                    symmetryAxisOrigin = EditorGUILayout.Vector2Field("Axis Origin", symmetryAxisOrigin);
                    
                    if (currentSymmetryMode == SymmetryMode.Diagonal)
                    {
                        symmetryAngle = EditorGUILayout.Slider("Angle", symmetryAngle, -180f, 180f);
                    }
                    break;

                case SymmetryMode.Radial:
                    EditorGUI.BeginChangeCheck();
                    radialSegments = EditorGUILayout.IntSlider("Segments", radialSegments, MIN_SEGMENTS, MAX_SEGMENTS);
                    radialRadius = EditorGUILayout.FloatField("Radius", radialRadius);
                    if (EditorGUI.EndChangeCheck())
                    {
                        SceneView.RepaintAll();
                    }
                    break;
            }
        }

        private void DrawSymmetryButtons(SerializedProperty slotsProperty)
        {
            if (GUILayout.Button("Apply Symmetry"))
            {
                ApplySymmetry(slotsProperty);
            }

            if (GUILayout.Button("Clear All Slots"))
            {
                if (EditorUtility.DisplayDialog("Clear All Slots",
                    "Are you sure you want to clear all slots?", "Clear", "Cancel"))
                {
                    ClearAllSlots(slotsProperty);
                }
            }
        }

        public void OnSceneGUI()
        {
            if (currentSymmetryMode == SymmetryMode.None) return;

            Color originalColor = Handles.color;
            Handles.color = new Color(1f, 1f, 0f, 0.5f); // Semi-transparent yellow
            
            DrawSymmetryGuides();
            DrawAxisOriginHandle();
            
            Handles.color = originalColor;
        }

        private void DrawSymmetryGuides()
        {
            switch (currentSymmetryMode)
            {
                case SymmetryMode.Horizontal:
                    DrawHorizontalGuide();
                    break;

                case SymmetryMode.Vertical:
                    DrawVerticalGuide();
                    break;

                case SymmetryMode.Diagonal:
                    DrawDiagonalGuide();
                    break;

                case SymmetryMode.Radial:
                    DrawRadialGuides();
                    break;
            }
        }

        private void DrawHorizontalGuide()
        {
            Handles.DrawLine(
                symmetryAxisOrigin + Vector2.left * GUIDE_LENGTH,
                symmetryAxisOrigin + Vector2.right * GUIDE_LENGTH
            );
        }

        private void DrawVerticalGuide()
        {
            Handles.DrawLine(
                symmetryAxisOrigin + Vector2.up * GUIDE_LENGTH,
                symmetryAxisOrigin + Vector2.down * GUIDE_LENGTH
            );
        }

        private void DrawDiagonalGuide()
        {
            var rad = symmetryAngle * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            Handles.DrawLine(
                symmetryAxisOrigin - direction * GUIDE_LENGTH,
                symmetryAxisOrigin + direction * GUIDE_LENGTH
            );
        }

        private void DrawRadialGuides()
        {
            // Draw circle
            Handles.DrawWireDisc(symmetryAxisOrigin, Vector3.forward, radialRadius);

            // Draw segment lines
            for (int i = 0; i < radialSegments; i++)
            {
                var angle = (2 * Mathf.PI * i) / radialSegments;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Handles.DrawLine(
                    symmetryAxisOrigin,
                    symmetryAxisOrigin + direction * radialRadius
                );
            }
        }

        private void DrawAxisOriginHandle()
        {
            EditorGUI.BeginChangeCheck();
            var newOrigin = Handles.PositionHandle(symmetryAxisOrigin, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                symmetryAxisOrigin = newOrigin;
                SceneView.RepaintAll();
            }
        }

        private void ApplySymmetry(SerializedProperty slotsProperty)
        {
            var positions = GetExistingPositions(slotsProperty);
            var newPositions = new List<Vector2>();

            switch (currentSymmetryMode)
            {
                case SymmetryMode.Horizontal:
                    ApplyHorizontalSymmetry(positions, newPositions);
                    break;

                case SymmetryMode.Vertical:
                    ApplyVerticalSymmetry(positions, newPositions);
                    break;

                case SymmetryMode.Diagonal:
                    ApplyDiagonalSymmetry(positions, newPositions);
                    break;

                case SymmetryMode.Radial:
                    ApplyRadialSymmetry(positions, newPositions);
                    break;
            }

            // Remove duplicates and update
            newPositions = newPositions.Distinct().ToList();
            UpdateSlotPositions(slotsProperty, newPositions);
        }

        private void ApplyHorizontalSymmetry(List<Vector2> positions, List<Vector2> newPositions)
        {
            foreach (var pos in positions)
            {
                newPositions.Add(pos);
                newPositions.Add(new Vector2(pos.x, -pos.y + (2 * symmetryAxisOrigin.y)));
            }
        }

        private void ApplyVerticalSymmetry(List<Vector2> positions, List<Vector2> newPositions)
        {
            foreach (var pos in positions)
            {
                newPositions.Add(pos);
                newPositions.Add(new Vector2(-pos.x + (2 * symmetryAxisOrigin.x), pos.y));
            }
        }

        private void ApplyDiagonalSymmetry(List<Vector2> positions, List<Vector2> newPositions)
        {
            var rad = symmetryAngle * Mathf.Deg2Rad;
            var rotationMatrix = new Matrix4x4(
                new Vector4(Mathf.Cos(rad), -Mathf.Sin(rad), 0, 0),
                new Vector4(Mathf.Sin(rad), Mathf.Cos(rad), 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
            );

            foreach (var pos in positions)
            {
                newPositions.Add(pos);
                var localPos = pos - symmetryAxisOrigin;
                var reflected = rotationMatrix.MultiplyPoint(new Vector3(-localPos.x, localPos.y, 0));
                newPositions.Add(new Vector2(reflected.x, reflected.y) + symmetryAxisOrigin);
            }
        }

        private void ApplyRadialSymmetry(List<Vector2> positions, List<Vector2> newPositions)
        {
            foreach (var pos in positions)
            {
                var localPos = pos - symmetryAxisOrigin;
                var angle = Mathf.Atan2(localPos.y, localPos.x);
                var radius = localPos.magnitude;

                for (int i = 0; i < radialSegments; i++)
                {
                    var segmentAngle = (2 * Mathf.PI * i) / radialSegments;
                    var rotatedPos = new Vector2(
                        Mathf.Cos(segmentAngle) * radius,
                        Mathf.Sin(segmentAngle) * radius
                    );
                    newPositions.Add(rotatedPos + symmetryAxisOrigin);
                }
            }
        }

        private List<Vector2> GetExistingPositions(SerializedProperty slotsProperty)
        {
            var positions = new List<Vector2>();
            for (int i = 0; i < slotsProperty.arraySize; i++)
            {
                var element = slotsProperty.GetArrayElementAtIndex(i);
                positions.Add(element.vector2Value);
            }
            return positions;
        }

        private void UpdateSlotPositions(SerializedProperty slotsProperty, List<Vector2> positions)
        {
            slotsProperty.ClearArray();
            foreach (var position in positions)
            {
                slotsProperty.InsertArrayElementAtIndex(slotsProperty.arraySize);
                var element = slotsProperty.GetArrayElementAtIndex(slotsProperty.arraySize - 1);
                element.vector2Value = position;
            }
            slotsProperty.serializedObject.ApplyModifiedProperties();
        }

        private void ClearAllSlots(SerializedProperty slotsProperty)
        {
            slotsProperty.ClearArray();
            slotsProperty.serializedObject.ApplyModifiedProperties();
            SceneView.RepaintAll();
        }

        public void OnDisable()
        {
            // Cleanup if needed
        }
    }
}