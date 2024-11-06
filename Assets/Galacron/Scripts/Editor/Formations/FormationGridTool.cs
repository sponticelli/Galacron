using UnityEngine;
using UnityEditor;

namespace Galacron.Formations
{
    /// <summary>
    /// Tool for managing grid functionality in the Formation Editor
    /// </summary>
    public class FormationGridTool
    {
        // Preference keys
        private const string PREF_ENABLE_GRID = "FormationConfigEditor_EnableGrid";
        private const string PREF_GRID_SIZE = "FormationConfigEditor_GridSize";
        private const string PREF_SHOW_GRID = "FormationConfigEditor_ShowGrid";
        private const string PREF_GRID_OPACITY = "FormationConfigEditor_GridOpacity";
        private const string PREF_GRID_SUBDIVISIONS = "FormationConfigEditor_GridSubdivisions";
        
        // Default values
        private const float DEFAULT_GRID_SIZE = 0.5f;
        private const float DEFAULT_OPACITY = 0.2f;
        private const int DEFAULT_SUBDIVISIONS = 2;

        // UI State
        private bool showGridSettings = true;
        
        // Grid Settings
        private bool enableGrid;
        private float gridSize;
        private bool showGrid;
        private Color gridColor = new Color(1f, 1f, 1f, 0.2f);
        private float gridOpacity;
        private int gridSubdivisions;
        
        // Runtime references
        private Color cachedHandleColor;

        public FormationGridTool()
        {
            LoadPreferences();
        }

        /// <summary>
        /// Draws the grid settings UI in the inspector
        /// </summary>
        public void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Header with foldout
            showGridSettings = EditorGUILayout.Foldout(showGridSettings, "Grid Settings", true);
            if (!showGridSettings)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUI.indentLevel++;
            {
                // Main grid toggles
                EditorGUI.BeginChangeCheck();
                enableGrid = EditorGUILayout.Toggle(new GUIContent("Enable Snapping", "Snap slot positions to grid"), enableGrid);
                showGrid = EditorGUILayout.Toggle(new GUIContent("Show Grid", "Display grid in scene view"), showGrid);
                
                // Grid size and subdivisions
                using (new EditorGUI.DisabledScope(!enableGrid))
                {
                    gridSize = EditorGUILayout.FloatField(new GUIContent("Grid Size", "Size of each grid cell"), gridSize);
                    gridSubdivisions = EditorGUILayout.IntSlider(
                        new GUIContent("Subdivisions", "Number of subdivisions per grid cell"), 
                        gridSubdivisions, 1, 4);
                }

                // Visual settings
                using (new EditorGUI.DisabledScope(!showGrid))
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Visual Settings", EditorStyles.boldLabel);
                    
                    gridOpacity = EditorGUILayout.Slider(
                        new GUIContent("Grid Opacity", "Transparency of grid lines"), 
                        gridOpacity, 0.1f, 1f);
                    
                    gridColor = EditorGUILayout.ColorField(
                        new GUIContent("Grid Color", "Color of grid lines"), 
                        gridColor);
                }

                // Reset button
                EditorGUILayout.Space(5);
                if (GUILayout.Button("Reset Grid Settings"))
                {
                    ResetGridSettings();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    SceneView.RepaintAll();
                }
            }
            EditorGUI.indentLevel--;
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the grid in the scene view
        /// </summary>
        public void OnSceneGUI(SceneView sceneView)
        {
            if (!showGrid) return;

            // Store current handle color
            cachedHandleColor = Handles.color;
            
            var viewRect = sceneView.position;
            viewRect.x = 0;
            viewRect.y = 0;
            DrawGridGuides(viewRect, Vector2.zero);
            
            // Restore handle color
            Handles.color = cachedHandleColor;
        }

        /// <summary>
        /// Snaps a position to the current grid
        /// </summary>
        public Vector2 SnapToGrid(Vector2 position)
        {
            if (!enableGrid) return position;

            float subdivisionSize = gridSize / gridSubdivisions;
            float x = Mathf.Round(position.x / subdivisionSize) * subdivisionSize;
            float y = Mathf.Round(position.y / subdivisionSize) * subdivisionSize;
            return new Vector2(x, y);
        }

        /// <summary>
        /// Cleanup when editor is disabled
        /// </summary>
        public void OnDisable()
        {
            SavePreferences();
        }

        private void DrawGridGuides(Rect previewArea, Vector2 center)
        {
            // Calculate effective grid sizes
            float effectiveGridSize = gridSize;
            float subdivSize = effectiveGridSize / gridSubdivisions;

            // Calculate visible grid bounds
            float extendedSize = Mathf.Max(previewArea.width, previewArea.height) * 2f;
            float left = center.x - extendedSize;
            float right = center.x + extendedSize;
            float top = center.y - extendedSize;
            float bottom = center.y + extendedSize;

            // Draw main grid
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridColor.a * gridOpacity);
            DrawGridLines(left, right, top, bottom, effectiveGridSize);

            // Draw subdivisions with reduced opacity
            if (gridSubdivisions > 1)
            {
                Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridColor.a * gridOpacity * 0.5f);
                DrawSubdivisions(left, right, top, bottom, effectiveGridSize, subdivSize);
            }
        }

        private void DrawGridLines(float left, float right, float top, float bottom, float spacing)
        {
            // Vertical lines
            for (float x = Mathf.Floor(left / spacing) * spacing; x <= right; x += spacing)
            {
                Handles.DrawLine(new Vector3(x, top), new Vector3(x, bottom));
            }

            // Horizontal lines
            for (float y = Mathf.Floor(top / spacing) * spacing; y <= bottom; y += spacing)
            {
                Handles.DrawLine(new Vector3(left, y), new Vector3(right, y));
            }
        }

        private void DrawSubdivisions(float left, float right, float top, float bottom, float mainSpacing, float subSpacing)
        {
            // Vertical subdivisions
            for (float x = Mathf.Floor(left / subSpacing) * subSpacing; x <= right; x += subSpacing)
            {
                // Skip if this is a main grid line
                if (Mathf.Approximately(x % mainSpacing, 0)) continue;
                Handles.DrawLine(new Vector3(x, top), new Vector3(x, bottom));
            }

            // Horizontal subdivisions
            for (float y = Mathf.Floor(top / subSpacing) * subSpacing; y <= bottom; y += subSpacing)
            {
                // Skip if this is a main grid line
                if (Mathf.Approximately(y % mainSpacing, 0)) continue;
                Handles.DrawLine(new Vector3(left, y), new Vector3(right, y));
            }
        }

        private void LoadPreferences()
        {
            enableGrid = EditorPrefs.GetBool(PREF_ENABLE_GRID, true);
            gridSize = EditorPrefs.GetFloat(PREF_GRID_SIZE, DEFAULT_GRID_SIZE);
            showGrid = EditorPrefs.GetBool(PREF_SHOW_GRID, true);
            gridOpacity = EditorPrefs.GetFloat(PREF_GRID_OPACITY, DEFAULT_OPACITY);
            gridSubdivisions = EditorPrefs.GetInt(PREF_GRID_SUBDIVISIONS, DEFAULT_SUBDIVISIONS);
        }

        private void SavePreferences()
        {
            EditorPrefs.SetBool(PREF_ENABLE_GRID, enableGrid);
            EditorPrefs.SetFloat(PREF_GRID_SIZE, gridSize);
            EditorPrefs.SetBool(PREF_SHOW_GRID, showGrid);
            EditorPrefs.SetFloat(PREF_GRID_OPACITY, gridOpacity);
            EditorPrefs.SetInt(PREF_GRID_SUBDIVISIONS, gridSubdivisions);
        }

        private void ResetGridSettings()
        {
            enableGrid = true;
            gridSize = DEFAULT_GRID_SIZE;
            showGrid = true;
            gridOpacity = DEFAULT_OPACITY;
            gridSubdivisions = DEFAULT_SUBDIVISIONS;
            gridColor = new Color(1f, 1f, 1f, 0.2f);
            SceneView.RepaintAll();
        }
    }
}