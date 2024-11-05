using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Galacron.Formations
{
    [CustomEditor(typeof(FormationConfig))]
    public class FormationConfigEditor : UnityEditor.Editor
    {
        private bool editingSlots;
        private Tool lastTool;
        private int selectedSlotIndex = -1;
        private Vector2 dragStartPosition;
        private SerializedProperty slotPositionsProperty;
        private SerializedProperty spacingProperty;
        private SerializedProperty arrivalDelayProperty;

        // Grid Settings
        private bool showGridSettings = true;
        private bool enableGrid = true;
        private float gridSize = 0.5f;
        private bool showGrid = true;
        private Color gridColor = new Color(1f, 1f, 1f, 0.2f);
        private float gridOpacity = 0.2f;
        private int gridSubdivisions = 2;

        // Preview settings
        private bool showPreview = true;
        private Vector2 previewScrollPosition;
        private float previewZoom = 1f;
        private Vector2 previewPan;
        private bool isDraggingPreview;
        private Vector2 lastMousePosition;
        private GUIStyle previewStyle;
        private Texture2D gridTexture;
        private Color[] previewColors = new[] 
        {
            new Color(1f, 0.3f, 0.3f),   // Red
            new Color(0.3f, 1f, 0.3f),   // Green
            new Color(0.3f, 0.3f, 1f),   // Blue
            new Color(1f, 0.3f, 1f),     // Pink
            new Color(0.3f, 1f, 1f),     // Cyan
            new Color(0.6f, 0.6f, 1f),   // Purple
            new Color(1f, 0.5f, 0.3f),   // Orange
            new Color(0.5f, 0.6f, 0.3f)    // Olive Green
        };

        private const float HandleSize = 0.2f;
        private const float GridSize = 16f;

        private void OnEnable()
        {
            slotPositionsProperty = serializedObject.FindProperty("slotPositions");
            spacingProperty = serializedObject.FindProperty("spacing");
            arrivalDelayProperty = serializedObject.FindProperty("arrivalDelayBetweenMembers");
            SceneView.duringSceneGui += OnSceneGUI;
            lastTool = Tools.current;
            CreateGridTexture();
            InitializePreviewStyle();

            // Load grid preferences
            enableGrid = EditorPrefs.GetBool("FormationConfigEditor_EnableGrid", true);
            gridSize = EditorPrefs.GetFloat("FormationConfigEditor_GridSize", 0.5f);
            showGrid = EditorPrefs.GetBool("FormationConfigEditor_ShowGrid", true);
            gridOpacity = EditorPrefs.GetFloat("FormationConfigEditor_GridOpacity", 0.2f);
            gridSubdivisions = EditorPrefs.GetInt("FormationConfigEditor_GridSubdivisions", 2);
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Tools.current = lastTool;
            if (gridTexture != null)
            {
                DestroyImmediate(gridTexture);
            }

            // Save grid preferences
            EditorPrefs.SetBool("FormationConfigEditor_EnableGrid", enableGrid);
            EditorPrefs.SetFloat("FormationConfigEditor_GridSize", gridSize);
            EditorPrefs.SetBool("FormationConfigEditor_ShowGrid", showGrid);
            EditorPrefs.SetFloat("FormationConfigEditor_GridOpacity", gridOpacity);
            EditorPrefs.SetInt("FormationConfigEditor_GridSubdivisions", gridSubdivisions);
        }
        
        private void DrawGridSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showGridSettings = EditorGUILayout.Foldout(showGridSettings, "Grid Settings", true);
        
            if (showGridSettings)
            {
                EditorGUI.indentLevel++;
            
                enableGrid = EditorGUILayout.Toggle("Enable Snapping", enableGrid);
                showGrid = EditorGUILayout.Toggle("Show Grid", showGrid);
            
                using (new EditorGUI.DisabledScope(!enableGrid))
                {
                    gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
                    gridSubdivisions = EditorGUILayout.IntSlider("Subdivisions", gridSubdivisions, 1, 4);
                }
            
                using (new EditorGUI.DisabledScope(!showGrid))
                {
                    gridOpacity = EditorGUILayout.Slider("Grid Opacity", gridOpacity, 0.1f, 1f);
                    gridColor = EditorGUILayout.ColorField("Grid Color", gridColor);
                }

                if (GUILayout.Button("Reset Grid Settings"))
                {
                    ResetGridSettings();
                }
            
                EditorGUI.indentLevel--;
            }
        
            EditorGUILayout.EndVertical();
        }
        
        private void ResetGridSettings()
        {
            enableGrid = true;
            gridSize = 0.5f;
            showGrid = true;
            gridOpacity = 0.2f;
            gridSubdivisions = 2;
            gridColor = new Color(1f, 1f, 1f, 0.2f);
        }
        
        private Vector2 SnapToGrid(Vector2 position)
        {
            if (!enableGrid) return position;
        
            float subdivisionSize = gridSize / gridSubdivisions;
            float x = Mathf.Round(position.x / subdivisionSize) * subdivisionSize;
            float y = Mathf.Round(position.y / subdivisionSize) * subdivisionSize;
            return new Vector2(x, y);
        }
        
        private void DrawGridGuides(Rect previewArea, Vector2 center)
        {
            if (!showGrid) return;

            float effectiveGridSize = gridSize * previewZoom;
            float subdivSize = effectiveGridSize / gridSubdivisions;

            // Calculate grid bounds
            float left = center.x - previewArea.width / 2;
            float right = center.x + previewArea.width / 2;
            float top = center.y - previewArea.height / 2;
            float bottom = center.y + previewArea.height / 2;

            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridColor.a * gridOpacity);

            // Draw main grid lines
            for (float x = Mathf.Floor(left / effectiveGridSize) * effectiveGridSize; x <= right; x += effectiveGridSize)
            {
                Handles.DrawLine(new Vector3(x, top), new Vector3(x, bottom));
            }

            for (float y = Mathf.Floor(top / effectiveGridSize) * effectiveGridSize; y <= bottom; y += effectiveGridSize)
            {
                Handles.DrawLine(new Vector3(left, y), new Vector3(right, y));
            }

            // Draw subdivisions with lower opacity
            if (gridSubdivisions > 1)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridColor.a * gridOpacity * 0.5f);
            
                for (float x = Mathf.Floor(left / subdivSize) * subdivSize; x <= right; x += subdivSize)
                {
                    if (Mathf.Approximately(x % effectiveGridSize, 0)) continue;
                    Handles.DrawLine(new Vector3(x, top), new Vector3(x, bottom));
                }

                for (float y = Mathf.Floor(top / subdivSize) * subdivSize; y <= bottom; y += subdivSize)
                {
                    if (Mathf.Approximately(y % effectiveGridSize, 0)) continue;
                    Handles.DrawLine(new Vector3(left, y), new Vector3(right, y));
                }
            }
        }

        private void CreateGridTexture()
        {
            gridTexture = new Texture2D(32, 32);
            var colors = new Color[32 * 32];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    colors[y * 32 + x] = (x == 0 || y == 0) ? new Color(1f, 1f, 1f, 0.1f) : new Color(1f, 1f, 1f, 0.05f);
                }
            }
            gridTexture.SetPixels(colors);
            gridTexture.Apply();
            gridTexture.wrapMode = TextureWrapMode.Repeat;
        }

        private void InitializePreviewStyle()
        {
            previewStyle = new GUIStyle();
            previewStyle.normal.background = EditorGUIUtility.whiteTexture;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGridSettings();
            EditorGUILayout.Space(10);
            
            // Preview Section
            DrawPreviewSection();
            
            EditorGUILayout.Space(10);

            // Slot editing section
            DrawSlotEditingSection();

            EditorGUILayout.Space(10);

            // Configuration section
            DrawConfigurationSection();

            // Draw rest of properties
            EditorGUILayout.Space(10);
            DrawPropertiesExcluding(serializedObject, 
                "m_Script", 
                "slotPositions", 
                "spacing", 
                "arrivalDelayBetweenMembers");

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPreviewSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            showPreview = EditorGUILayout.Foldout(showPreview, "Formation Preview", true);
            previewZoom = EditorGUILayout.Slider(previewZoom, 0.1f, 5f, GUILayout.Width(200));
            if (GUILayout.Button("Reset View", GUILayout.Width(100)))
            {
                ResetPreviewView();
            }
            EditorGUILayout.EndHorizontal();

            if (showPreview)
            {
                var rect = GUILayoutUtility.GetRect(0, 300);
                DrawPreview(rect);
                HandlePreviewInput(rect);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawPreview(Rect rect)
        {
            // Draw background
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1));

            // Begin preview area
            GUI.BeginClip(rect);

            // Draw grid
            var gridOffset = new Vector2(
                previewPan.x % (GridSize * previewZoom),
                previewPan.y % (GridSize * previewZoom)
            );
            var gridRect = new Rect(gridOffset.x, gridOffset.y, rect.width, rect.height);
            var gridScale = new Vector2(previewZoom / GridSize, previewZoom / GridSize);
            GUI.DrawTextureWithTexCoords(
                gridRect, 
                gridTexture,
                new Rect(0, 0, rect.width / (GridSize * previewZoom), rect.height / (GridSize * previewZoom))
            );

            // Draw origin
            var centerX = rect.width * 0.5f + previewPan.x;
            var centerY = rect.height * 0.5f + previewPan.y;
            Handles.color = Color.yellow;
            Handles.DrawLine(
                new Vector2(centerX - 10, centerY),
                new Vector2(centerX + 10, centerY));
            Handles.DrawLine(
                new Vector2(centerX, centerY - 10),
                new Vector2(centerX, centerY + 10));

            // Draw formation slots
            var spacing = spacingProperty.floatValue;
            for (int i = 0; i < slotPositionsProperty.arraySize; i++)
            {
                var slotProperty = slotPositionsProperty.GetArrayElementAtIndex(i);
                var position = slotProperty.vector2Value * spacing;
                var screenPos = new Vector2(
                    centerX + position.x * previewZoom,
                    centerY - position.y * previewZoom  // Invert Y coordinate
                );

                var slotSize = HandleSize * 20 * previewZoom;
                var slotRect = new Rect(
                    screenPos.x - slotSize * 0.5f,
                    screenPos.y - slotSize * 0.5f,
                    slotSize,
                    slotSize
                );

                // Draw slot connections
                if (i > 0)
                {
                    var prevSlot = slotPositionsProperty.GetArrayElementAtIndex(i - 1).vector2Value * spacing;
                    var prevScreenPos = new Vector2(
                        centerX + prevSlot.x * previewZoom,
                        centerY - prevSlot.y * previewZoom  // Invert Y coordinate
                    );
                    
                    Handles.color = new Color(1f, 1f, 1f, 0.3f);
                    Handles.DrawLine(prevScreenPos, screenPos);
                }

                // Draw slot
                var color = i == selectedSlotIndex ? Color.yellow : previewColors[i % previewColors.Length];
                Handles.color = color;
                Handles.DrawSolidDisc(screenPos, Vector3.forward, slotSize * 0.5f);
                
                // Draw slot index
                var style = new GUIStyle(EditorStyles.boldLabel);
                style.normal.textColor = Color.black;
                var indexContent = new GUIContent(i.ToString());
                var labelSize = style.CalcSize(indexContent);
                GUI.Label(
                    new Rect(
                        screenPos.x - labelSize.x * 0.5f,
                        screenPos.y - labelSize.y * 0.5f,
                        labelSize.x,
                        labelSize.y
                    ),
                    indexContent,
                    style
                );
            }

            GUI.EndClip();
        }

        private void HandlePreviewInput(Rect previewRect)
        {
            var currentEvent = Event.current;
            
            if (!previewRect.Contains(currentEvent.mousePosition))
                return;

            // Handle preview dragging
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 || currentEvent.button == 2)
                    {
                        isDraggingPreview = true;
                        lastMousePosition = currentEvent.mousePosition;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    isDraggingPreview = false;
                    break;

                case EventType.MouseDrag:
                    if (isDraggingPreview)
                    {
                        previewPan += (currentEvent.mousePosition - lastMousePosition);
                        lastMousePosition = currentEvent.mousePosition;
                        Repaint();
                        currentEvent.Use();
                    }
                    break;

                case EventType.ScrollWheel:
                    previewZoom = Mathf.Clamp(previewZoom - currentEvent.delta.y * 0.1f, 0.1f, 5f);
                    Repaint();
                    currentEvent.Use();
                    break;
            }
        }

        private void ResetPreviewView()
        {
            previewZoom = 1f;
            previewPan = Vector2.zero;
            Repaint();
        }

        private void DrawSlotEditingSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Formation Slots", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
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

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Slots: {slotPositionsProperty.arraySize}", EditorStyles.miniLabel);
            
            if (editingSlots)
            {
                EditorGUILayout.BeginHorizontal();
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
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);
                
                if (selectedSlotIndex >= 0 && selectedSlotIndex < slotPositionsProperty.arraySize)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"Editing Slot {selectedSlotIndex}", EditorStyles.boldLabel);
                    
                    var slotProperty = slotPositionsProperty.GetArrayElementAtIndex(selectedSlotIndex);
                    EditorGUI.BeginChangeCheck();
                    Vector2 newPosition = EditorGUILayout.Vector2Field("Position", slotProperty.vector2Value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        slotProperty.vector2Value = newPosition;
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawConfigurationSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spacingProperty);
            EditorGUILayout.PropertyField(arrivalDelayProperty);
            EditorGUILayout.EndVertical();
        }

         private void OnSceneGUI(SceneView sceneView)
    {
        if (!editingSlots) return;

        var config = (FormationConfig)target;
        var spacing = config.spacing;

        // Draw slots
        for (int i = 0; i < slotPositionsProperty.arraySize; i++)
        {
            var slotProperty = slotPositionsProperty.GetArrayElementAtIndex(i);
            var position = slotProperty.vector2Value * spacing;

            // Draw handle
            float handleSize = HandleUtility.GetHandleSize(position) * HandleSize;
            bool isSelected = i == selectedSlotIndex;

            Handles.color = isSelected ? Color.yellow : previewColors[i % previewColors.Length];
            if (Handles.Button(position, Quaternion.identity, handleSize, handleSize, Handles.DotHandleCap))
            {
                selectedSlotIndex = i;
                Repaint();
            }

            // Draw slot index
            Handles.Label(position + Vector2.up * handleSize, $"Slot {i}");

            // Position handle for selected slot with grid snapping
            if (isSelected)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPosition = Handles.DoPositionHandle(position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Move Formation Slot");
                    Vector2 snappedPosition = SnapToGrid(new Vector2(newPosition.x, newPosition.y));
                    slotProperty.vector2Value = snappedPosition / spacing;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        // Draw grid in scene view if enabled
        if (showGrid)
        {
            var viewRect = sceneView.position;
            viewRect.x = 0;
            viewRect.y = 0;
            DrawGridGuides(viewRect, Vector2.zero);
        }

        // Draw origin reference
        Handles.color = Color.blue;
        Handles.DrawWireCube(Vector3.zero, Vector3.one * 0.5f);
        
        // Consume input to prevent default tool behavior
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }

        private void AddSlot()
        {
            Vector2 newPosition = Vector2.zero;
            
            // If there are existing slots, place the new one relative to the last one
            if (slotPositionsProperty.arraySize > 0)
            {
                var lastSlot = slotPositionsProperty.GetArrayElementAtIndex(slotPositionsProperty.arraySize - 1);
                newPosition = lastSlot.vector2Value + Vector2.right;
            }

            slotPositionsProperty.InsertArrayElementAtIndex(slotPositionsProperty.arraySize);
            var newSlot = slotPositionsProperty.GetArrayElementAtIndex(slotPositionsProperty.arraySize - 1);
            newSlot.vector2Value = newPosition;
            
            selectedSlotIndex = slotPositionsProperty.arraySize - 1;
            serializedObject.ApplyModifiedProperties();
            
            SceneView.RepaintAll();
        }

        private void RemoveSelectedSlot()
        {
            if (selectedSlotIndex < 0 || selectedSlotIndex >= slotPositionsProperty.arraySize)
                return;

            slotPositionsProperty.DeleteArrayElementAtIndex(selectedSlotIndex);
            selectedSlotIndex = -1;
            serializedObject.ApplyModifiedProperties();
            
            SceneView.RepaintAll();
        }
    }
}