using UnityEngine;
using UnityEditor;

namespace Galacron.Formations
{
    [CustomEditor(typeof(Formation))]
    public class FormationEditor : UnityEditor.Editor
    {
        private Formation formation;
        private FormationConfig config;
        private SerializedProperty configProperty;
        private bool editingSlots;
        private Tool lastTool;
        private int selectedSlotIndex = -1;
        private Vector2 dragStartPosition;
        private Vector3 handleOffset;

        private void OnEnable()
        {
            formation = (Formation)target;
            configProperty = serializedObject.FindProperty("config");
            config = configProperty.objectReferenceValue as FormationConfig;
            
            // Register to scene GUI callback
            SceneView.duringSceneGui += OnSceneGUI;
            lastTool = Tools.current;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Tools.current = lastTool;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Formation config field
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(configProperty);
            if (EditorGUI.EndChangeCheck())
            {
                config = configProperty.objectReferenceValue as FormationConfig;
            }

            if (config == null)
            {
                EditorGUILayout.HelpBox("Please assign a Formation Config", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();

            // Slot editing controls
            GUI.backgroundColor = editingSlots ? Color.green : Color.white;
            if (GUILayout.Button(editingSlots ? "Stop Editing Slots" : "Edit Slots"))
            {
                editingSlots = !editingSlots;
                if (editingSlots)
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

            if (editingSlots)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Slot Controls", EditorStyles.boldLabel);

                // Add slot button
                if (GUILayout.Button("Add Slot"))
                {
                    Undo.RecordObject(config, "Add Formation Slot");
                    ArrayUtility.Add(ref config.slotPositions, Vector2.zero);
                    EditorUtility.SetDirty(config);
                }

                // Remove selected slot button
                GUI.enabled = selectedSlotIndex >= 0 && selectedSlotIndex < config.slotPositions.Length;
                if (GUILayout.Button("Remove Selected Slot"))
                {
                    Undo.RecordObject(config, "Remove Formation Slot");
                    ArrayUtility.RemoveAt(ref config.slotPositions, selectedSlotIndex);
                    selectedSlotIndex = -1;
                    EditorUtility.SetDirty(config);
                }
                GUI.enabled = true;

                if (selectedSlotIndex >= 0 && selectedSlotIndex < config.slotPositions.Length)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"Editing Slot {selectedSlotIndex}", EditorStyles.boldLabel);
                    
                    EditorGUI.BeginChangeCheck();
                    Vector2 newPos = EditorGUILayout.Vector2Field("Position", config.slotPositions[selectedSlotIndex]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(config, "Edit Slot Position");
                        config.slotPositions[selectedSlotIndex] = newPos;
                        EditorUtility.SetDirty(config);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!editingSlots || config == null) return;

            // Get formation world matrix
            Matrix4x4 formationMatrix = formation.transform.localToWorldMatrix;

            // Draw all slots
            for (int i = 0; i < config.slotPositions.Length; i++)
            {
                Vector3 worldPos = formationMatrix.MultiplyPoint3x4(config.slotPositions[i]);
                
                // Draw slot handle
                float handleSize = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                bool isSelected = i == selectedSlotIndex;

                Handles.color = isSelected ? Color.yellow : Color.white;
                if (Handles.Button(worldPos, Quaternion.identity, handleSize, handleSize, Handles.DotHandleCap))
                {
                    selectedSlotIndex = i;
                    Repaint();
                }

                // Draw slot index
                Handles.Label(worldPos + Vector3.up * handleSize, $"Slot {i}");

                // Draw position handle for selected slot
                if (isSelected)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 newPos = Handles.DoPositionHandle(worldPos, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(config, "Move Formation Slot");
                        // Convert back to local space
                        config.slotPositions[i] = formation.transform.worldToLocalMatrix.MultiplyPoint3x4(newPos);
                        EditorUtility.SetDirty(config);
                    }
                }
            }

            // Draw connections between slots
            Handles.color = new Color(1, 1, 1, 0.5f);
            for (int i = 0; i < config.slotPositions.Length - 1; i++)
            {
                Vector3 start = formationMatrix.MultiplyPoint3x4(config.slotPositions[i]);
                Vector3 end = formationMatrix.MultiplyPoint3x4(config.slotPositions[i + 1]);
                Handles.DrawDottedLine(start, end, 5f);
            }

            // Draw formation forward direction
            Handles.color = Color.blue;
            Vector3 center = formation.transform.position;
            Handles.DrawDottedLine(center, center + formation.transform.up * 2f, 5f);

            // Consume input to prevent default tool behavior
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
    }
}