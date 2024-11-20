using UnityEngine;
using UnityEditor;

namespace Galacron.Actors
{
    public class ShieldShapeEditor : EditorWindow
    {
        private ShieldShape currentShape;
        private Vector2 scrollPosition;
        private float cellSize = 30f;
        private float padding = 2f;
        private Texture2D gridTexture;
        private bool isDragging;
        private bool dragValue;
        private bool showPreview = true;
        
        private readonly Color activeColor = new Color(0.2f, 1f, 0.2f, 1f);
        private readonly Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private readonly Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [MenuItem("Galacron/Shield Shape Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<ShieldShapeEditor>("Shield Shape Editor");
            window.minSize = new Vector2(400, 500);
            window.titleContent = new GUIContent("Shield Shape Editor");
            window.Show();
        }

        private void OnEnable()
        {
            CreateGridTexture();
        }

        private void CreateGridTexture()
        {
            gridTexture = new Texture2D(1, 1);
            gridTexture.SetPixel(0, 0, gridLineColor);
            gridTexture.Apply();
        }

        private void OnGUI()
        {
            DrawToolbar();
            
            if (currentShape == null)
            {
                EditorGUILayout.HelpBox("Select a Shield Shape asset to edit", MessageType.Info);
                return;
            }

            DrawDimensionControls();
            DrawShapeEditor();
            DrawUtilityButtons();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(currentShape);
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            // Object Field for shield shape
            var newShape = EditorGUILayout.ObjectField(currentShape, typeof(ShieldShape), false, 
                GUILayout.Width(200)) as ShieldShape;
            if (newShape != currentShape)
            {
                currentShape = newShape;
                if (currentShape != null)
                {
                    currentShape.Initialize();
                }
            }

            GUILayout.FlexibleSpace();

            GUILayout.Space(20);
            
            // Cell Size slider with custom styling
            EditorGUILayout.LabelField("Cell Size:", EditorStyles.miniLabel, GUILayout.Width(52));
            float newCellSize = GUILayout.HorizontalSlider(cellSize, 10f, 50f, GUILayout.Width(60));
            EditorGUILayout.LabelField(cellSize.ToString("F0"), EditorStyles.miniLabel, GUILayout.Width(25));
            
            if (Mathf.Abs(newCellSize - cellSize) > 0.01f)
            {
                cellSize = newCellSize;
                Repaint();
            }
            
            showPreview = GUILayout.Toggle(showPreview, "Preview", EditorStyles.toolbarButton, GUILayout.Width(60));
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDimensionControls()
        {
            EditorGUILayout.BeginHorizontal();
            
            // Width control
            EditorGUI.BeginChangeCheck();
            int newWidth = EditorGUILayout.IntField("Width", currentShape.Width);
            if (EditorGUI.EndChangeCheck() && newWidth > 0)
            {
                Undo.RecordObject(currentShape, "Change Shield Width");
                currentShape.Resize(newWidth, currentShape.Height);
            }
            
            // Height control
            EditorGUI.BeginChangeCheck();
            int newHeight = EditorGUILayout.IntField("Height", currentShape.Height);
            if (EditorGUI.EndChangeCheck() && newHeight > 0)
            {
                Undo.RecordObject(currentShape, "Change Shield Height");
                currentShape.Resize(currentShape.Width, newHeight);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawShapeEditor()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            float totalWidth = currentShape.Width * (cellSize + padding);
            float totalHeight = currentShape.Height * (cellSize + padding);

            Rect gridRect = GUILayoutUtility.GetRect(totalWidth + padding, totalHeight + padding);
            
            // Draw background grid
            if (Event.current.type == EventType.Repaint)
            {
                DrawGrid(gridRect);
            }

            // Draw cells and handle input
            HandleGridInteraction(gridRect);

            EditorGUILayout.EndScrollView();
        }

        private void DrawGrid(Rect gridRect)
        {
            // Draw cells
            for (int y = 0; y < currentShape.Height; y++)
            {
                for (int x = 0; x < currentShape.Width; x++)
                {
                    Rect cellRect = GetCellRect(gridRect, x, y);
                    bool isActive = currentShape.GetPixel(x, currentShape.Height - 1 - y);
                    EditorGUI.DrawRect(cellRect, isActive ? activeColor : inactiveColor);
                    
                    // Draw cell border
                    GUI.DrawTexture(new Rect(cellRect.x, cellRect.y, 1, cellRect.height), gridTexture);
                    GUI.DrawTexture(new Rect(cellRect.x, cellRect.y, cellRect.width, 1), gridTexture);
                }
            }
        }

        private void HandleGridInteraction(Rect gridRect)
        {
            var currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 && gridRect.Contains(currentEvent.mousePosition))
                    {
                        isDragging = true;
                        Vector2Int cell = GetCellFromMousePosition(gridRect, currentEvent.mousePosition);
                        if (IsValidCell(cell))
                        {
                            dragValue = !currentShape.GetPixel(cell.x, currentShape.Height - 1 - cell.y);
                            ToggleCell(cell);
                            currentEvent.Use();
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (isDragging && currentEvent.button == 0)
                    {
                        Vector2Int cell = GetCellFromMousePosition(gridRect, currentEvent.mousePosition);
                        if (IsValidCell(cell))
                        {
                            currentShape.SetPixel(cell.x, currentShape.Height - 1 - cell.y, dragValue);
                            GUI.changed = true;
                            currentEvent.Use();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (currentEvent.button == 0)
                    {
                        isDragging = false;
                        currentEvent.Use();
                    }
                    break;
            }
        }

        private void DrawUtilityButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Fill All"))
            {
                Undo.RecordObject(currentShape, "Fill Shield Shape");
                currentShape.Clear(true);
            }
            
            if (GUILayout.Button("Clear All"))
            {
                Undo.RecordObject(currentShape, "Clear Shield Shape");
                currentShape.Clear(false);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private Rect GetCellRect(Rect gridRect, int x, int y)
        {
            return new Rect(
                gridRect.x + x * (cellSize + padding) + padding,
                gridRect.y + y * (cellSize + padding) + padding,
                cellSize,
                cellSize
            );
        }

        private Vector2Int GetCellFromMousePosition(Rect gridRect, Vector2 mousePosition)
        {
            Vector2 localPos = mousePosition - new Vector2(gridRect.x + padding, gridRect.y + padding);
            return new Vector2Int(
                Mathf.FloorToInt(localPos.x / (cellSize + padding)),
                Mathf.FloorToInt(localPos.y / (cellSize + padding))
            );
        }

        private bool IsValidCell(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < currentShape.Width && 
                   cell.y >= 0 && cell.y < currentShape.Height;
        }

        private void ToggleCell(Vector2Int cell)
        {
            Undo.RecordObject(currentShape, "Toggle Shield Cell");
            currentShape.SetPixel(cell.x, currentShape.Height - 1 - cell.y, dragValue);
            GUI.changed = true;
        }

        private void OnDestroy()
        {
            if (gridTexture != null)
            {
                DestroyImmediate(gridTexture);
            }
        }
    }
}