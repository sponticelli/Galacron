using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Galacron.Formations
{
    /// <summary>
    /// Handles editing of formation slots in both Inspector and Scene view
    /// </summary>
    public class FormationSlotEditor
    {
        private SerializedProperty slotsProperty;
        private bool isEditing;
        private Tool lastTool;
        private int selectedSlotIndex = -1;
        private Vector2 dragStartPosition;

        // Constants
        private const float HANDLE_SIZE = 0.2f;
        private const float LABEL_OFFSET = 0.3f;

        // Colors
        private static readonly Color[] slotColors = {
            new Color(1f, 0.3f, 0.3f), // Red
            new Color(0.3f, 1f, 0.3f), // Green
            new Color(0.3f, 0.3f, 1f), // Blue
            new Color(1f, 0.3f, 1f), // Pink
            new Color(0.3f, 1f, 1f), // Cyan
            new Color(0.6f, 0.6f, 1f), // Purple
            new Color(1f, 0.5f, 0.3f), // Orange
            new Color(0.5f, 0.6f, 0.3f) // Olive Green
        };

        public bool IsEditing => isEditing;

        public FormationSlotEditor(SerializedProperty slotsProperty)
        {
            this.slotsProperty = slotsProperty;
            lastTool = Tools.current;
        }

        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // Header
            EditorGUILayout.LabelField("Formation Slots", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Edit mode toggle
            DrawEditModeToggle();

            // Slot listing and controls
            if (isEditing)
            {
                DrawSlotControls();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEditModeToggle()
        {
            GUI.backgroundColor = isEditing ? Color.green : Color.white;
            if (GUILayout.Button(isEditing ? "Stop Editing Slots" : "Edit Slots"))
            {
                isEditing = !isEditing;
                if (isEditing)
                {
                    lastTool = Tools.current;
                    Tools.current = Tool.None;
                }
                else
                {
                    Tools.current = lastTool;
                    selectedSlotIndex = -1;
                }
                SceneView.RepaintAll();
            }
            GUI.backgroundColor = Color.white;
        }

        private void DrawSlotControls()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Slots: {slotsProperty.arraySize}", EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Slot"))
                {
                    AddSlot();
                }

                GUI.enabled = selectedSlotIndex >= 0;
                if (GUILayout.Button("Remove Selected"))
                {
                    RemoveSelectedSlot();
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();

            // Selected slot properties
            if (selectedSlotIndex >= 0 && selectedSlotIndex < slotsProperty.arraySize)
            {
                DrawSelectedSlotProperties();
            }
        }

        private void DrawSelectedSlotProperties()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField($"Editing Slot {selectedSlotIndex}", EditorStyles.boldLabel);

                var slotProperty = slotsProperty.GetArrayElementAtIndex(selectedSlotIndex);
                EditorGUI.BeginChangeCheck();
                Vector2 newPosition = EditorGUILayout.Vector2Field("Position", slotProperty.vector2Value);
                if (EditorGUI.EndChangeCheck())
                {
                    slotProperty.vector2Value = newPosition;
                    slotsProperty.serializedObject.ApplyModifiedProperties();
                }

                // Additional slot properties could be added here
            }
            EditorGUILayout.EndVertical();
        }

        public void OnSceneGUI(FormationGridTool gridTool, float spacing)
        {
            if (!isEditing) return;

            Color cachedHandleColor = Handles.color;

            for (int i = 0; i < slotsProperty.arraySize; i++)
            {
                var slotProperty = slotsProperty.GetArrayElementAtIndex(i);
                var position = slotProperty.vector2Value * spacing;
                bool isSelected = i == selectedSlotIndex;

                DrawSlotConnections(i, position, spacing);
                DrawSlotHandle(i, position, isSelected, spacing);
                DrawSlotLabel(i, position);
            }

            HandleSlotSelection(spacing);

            Handles.color = cachedHandleColor;
        }

        private void DrawSlotConnections(int index, Vector2 position, float spacing)
        {
            if (index <= 0) return;

            var prevSlot = slotsProperty.GetArrayElementAtIndex(index - 1).vector2Value * spacing;
            Handles.color = new Color(1f, 1f, 1f, 0.3f);
            Handles.DrawLine(position, prevSlot);
        }

        private void DrawSlotHandle(int index, Vector2 position, bool isSelected, float spacing)
        {
            // Set handle color
            Handles.color = isSelected ? Color.yellow : slotColors[index % slotColors.Length];

            // Draw handle
            float handleSize = HandleUtility.GetHandleSize(position) * HANDLE_SIZE;
            if (Handles.Button(position, Quaternion.identity, handleSize, handleSize, Handles.DotHandleCap))
            {
                selectedSlotIndex = index;
                SceneView.RepaintAll();
            }

            // Position handle for selected slot
            if (isSelected)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.DoPositionHandle(position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    var slotProperty = slotsProperty.GetArrayElementAtIndex(index);
                    slotProperty.vector2Value = newPosition / spacing;
                    slotsProperty.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawSlotLabel(int index, Vector2 position)
        {
            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            Vector3 labelPos = position + Vector2.up * LABEL_OFFSET;
            Handles.Label(labelPos, index.ToString(), labelStyle);
        }

        private void HandleSlotSelection(float spacing)
        {
            var currentEvent = Event.current;
            var mousePos = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition).origin;

            // Handle click selection
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                float minDistance = float.MaxValue;
                int nearestSlot = -1;

                for (int i = 0; i < slotsProperty.arraySize; i++)
                {
                    var slotPos = slotsProperty.GetArrayElementAtIndex(i).vector2Value * spacing;
                    float distance = Vector2.Distance(mousePos, slotPos);
                    if (distance < minDistance && distance < HANDLE_SIZE)
                    {
                        minDistance = distance;
                        nearestSlot = i;
                    }
                }

                if (nearestSlot >= 0)
                {
                    selectedSlotIndex = nearestSlot;
                    currentEvent.Use();
                    SceneView.RepaintAll();
                }
            }
        }
        
        public void AddSlot()
        {
            Vector2 newPosition = Vector2.zero;

            // If there are existing slots, place the new one relative to the last one
            if (slotsProperty.arraySize > 0)
            {
                var lastSlot = slotsProperty.GetArrayElementAtIndex(slotsProperty.arraySize - 1);
                newPosition = lastSlot.vector2Value + Vector2.right;
            }

            slotsProperty.InsertArrayElementAtIndex(slotsProperty.arraySize);
            var newSlot = slotsProperty.GetArrayElementAtIndex(slotsProperty.arraySize - 1);
            newSlot.vector2Value = newPosition;

            selectedSlotIndex = slotsProperty.arraySize - 1;
            slotsProperty.serializedObject.ApplyModifiedProperties();

            SceneView.RepaintAll();
        }

        private void RemoveSelectedSlot()
        {
            if (selectedSlotIndex < 0 || selectedSlotIndex >= slotsProperty.arraySize)
                return;

            slotsProperty.DeleteArrayElementAtIndex(selectedSlotIndex);
            slotsProperty.serializedObject.ApplyModifiedProperties();

            if (selectedSlotIndex >= slotsProperty.arraySize)
            {
                selectedSlotIndex = slotsProperty.arraySize - 1;
            }

            SceneView.RepaintAll();
        }

        public void OnDisable()
        {
            if (isEditing)
            {
                Tools.current = lastTool;
            }
        }

        private Vector2 SnapToNearestSlot(Vector2 position, float spacing)
        {
            float minDistance = float.MaxValue;
            Vector2 snappedPosition = position;

            for (int i = 0; i < slotsProperty.arraySize; i++)
            {
                if (i == selectedSlotIndex) continue;

                var slotPos = slotsProperty.GetArrayElementAtIndex(i).vector2Value * spacing;
                float distance = Vector2.Distance(position, slotPos);
                
                if (distance < minDistance && distance < spacing * 0.5f)
                {
                    minDistance = distance;
                    snappedPosition = slotPos;
                }
            }

            return snappedPosition;
        }
    }
}