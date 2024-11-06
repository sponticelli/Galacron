using UnityEngine;
using UnityEditor;

namespace Galacron.Formations
{
    /// <summary>
    /// Handles the 2D preview window for formation visualization
    /// </summary>
    public class FormationPreviewTool
    {
        // Preview state
        private bool showPreview = true;
        private float previewZoom = 4f;
        private Vector2 previewPan;
        private bool isDraggingPreview;
        private Vector2 lastMousePosition;
        private Vector2 lastPanPosition;

        // Visualization
        private Texture2D gridTexture;
        private Texture2D backgroundTexture;
        private GUIStyle previewStyle;
        private const float HANDLE_SIZE = 8f;
        private const float GRID_SIZE = 16f;

        // Visual settings
        private readonly Color[] slotColors = {
            new Color(1f, 0.3f, 0.3f), // Red
            new Color(0.3f, 1f, 0.3f), // Green
            new Color(0.3f, 0.3f, 1f), // Blue
            new Color(1f, 0.3f, 1f), // Pink
            new Color(0.3f, 1f, 1f), // Cyan
            new Color(0.6f, 0.6f, 1f), // Purple
            new Color(1f, 0.5f, 0.3f), // Orange
            new Color(0.5f, 0.6f, 0.3f) // Olive Green
        };

        private const float ZOOM_MIN = 0.1f;
        private const float ZOOM_MAX = 6f;
        private const float ZOOM_SPEED = 0.1f;
        private const float DEFAULT_PREVIEW_HEIGHT = 300f;
        private const float SPACING_FACTOR = 4f;

        public FormationPreviewTool()
        {
            CreateBackgroundTexture();
            CreateGridTexture();
            InitializePreviewStyle();
        }

        public void OnInspectorGUI(SerializedProperty slotsProperty, float spacing)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawPreviewHeader();

            if (showPreview)
            {
                var rect = GUILayoutUtility.GetRect(0, DEFAULT_PREVIEW_HEIGHT);
                //DrawPreviewBackground(rect);
                DrawPreview(rect, slotsProperty, spacing * SPACING_FACTOR);
                HandlePreviewInput(rect);
            }

            EditorGUILayout.EndVertical();
        }

        public void OnDisable()
        {
            CleanupTextures();
        }

        private void DrawPreviewHeader()
        {
            EditorGUILayout.BeginHorizontal();
            {
                showPreview = EditorGUILayout.Foldout(showPreview, "Formation Preview", true);

                // Zoom slider
                EditorGUI.BeginChangeCheck();
                previewZoom = EditorGUILayout.Slider(previewZoom, ZOOM_MIN, ZOOM_MAX, GUILayout.Width(200));
                if (EditorGUI.EndChangeCheck())
                {
                    GUI.changed = true;
                }

                // Reset view button
                if (GUILayout.Button("Reset View", GUILayout.Width(100)))
                {
                    ResetPreviewView();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPreviewBackground(Rect rect)
        {
            // Draw dark background
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1));

            // Calculate grid offset relative to center
            var centerX = rect.width * 0.5f;
            var centerY = rect.height * 0.5f;

            // Calculate grid offset to maintain consistency while panning
            var gridOffsetX = (previewPan.x % (GRID_SIZE * previewZoom));
            var gridOffsetY = (previewPan.y % (GRID_SIZE * previewZoom));

            // Calculate grid rect that covers the entire preview area
            var gridRect = new Rect(
                -rect.width * 0.5f + gridOffsetX,
                -rect.height * 0.5f + gridOffsetY,
                rect.width * 2,
                rect.height * 2
            );

            // Apply zoom and calculate UV rect for tiling
            var gridScale = new Vector2(
                previewZoom / GRID_SIZE,
                previewZoom / GRID_SIZE
            );

            var uvRect = new Rect(
                gridRect.x / (GRID_SIZE * previewZoom),
                gridRect.y / (GRID_SIZE * previewZoom),
                rect.width * 2 / (GRID_SIZE * previewZoom),
                rect.height * 2 / (GRID_SIZE * previewZoom)
            );

            GUI.matrix = Matrix4x4.TRS(
                new Vector3(centerX, centerY, 0),
                Quaternion.identity,
                Vector3.one
            );

            GUI.DrawTextureWithTexCoords(gridRect, gridTexture, uvRect);
    
            // Reset matrix
            GUI.matrix = Matrix4x4.identity;
        }

        private void DrawPreview(Rect rect, SerializedProperty slotsProperty, float spacing)
        {
            // Begin preview area
            GUI.BeginClip(rect);

            // Get center of preview area
            var centerX = rect.width * 0.5f;
            var centerY = rect.height * 0.5f;

            // Draw origin cross at center with pan offset
            Handles.color = Color.yellow;
            Handles.DrawLine(
                new Vector2(centerX + previewPan.x - 10, centerY + previewPan.y),
                new Vector2(centerX + previewPan.x + 10, centerY + previewPan.y));
            Handles.DrawLine(
                new Vector2(centerX + previewPan.x, centerY + previewPan.y - 10),
                new Vector2(centerX + previewPan.x, centerY + previewPan.y + 10));

            // Draw formation slots
            for (int i = 0; i < slotsProperty.arraySize; i++)
            {
                var slotProperty = slotsProperty.GetArrayElementAtIndex(i);
                DrawSlot(rect, slotProperty, spacing, i, centerX, centerY);
            }

            GUI.EndClip();
        }

        private void DrawSlot(Rect rect, SerializedProperty slotProperty, float spacing, int index, float centerX, float centerY)
        {
            var position = slotProperty.vector2Value * spacing;
            var screenPos = new Vector2(
                centerX + (position.x * previewZoom) + previewPan.x,
                centerY + (-position.y * previewZoom) + previewPan.y // Invert Y coordinate
            );

            var slotSize = HANDLE_SIZE * previewZoom;
            var slotRect = new Rect(
                screenPos.x - slotSize * 0.5f,
                screenPos.y - slotSize * 0.5f,
                slotSize,
                slotSize
            );

            // Draw connection line to previous slot
            if (index > 0)
            {
                var prevSlot = slotProperty.serializedObject
                    .FindProperty("slotPositions")
                    .GetArrayElementAtIndex(index - 1)
                    .vector2Value * spacing;
                var prevScreenPos = new Vector2(
                    centerX + (prevSlot.x * previewZoom) + previewPan.x,
                    centerY + (-prevSlot.y * previewZoom) + previewPan.y
                );

                Handles.color = new Color(1f, 1f, 1f, 0.3f);
                Handles.DrawLine(prevScreenPos, screenPos);
            }

            // Draw slot
            Handles.color = slotColors[index % slotColors.Length];
            Handles.DrawSolidDisc(screenPos, Vector3.forward, slotSize * 0.5f);

            // Draw slot index
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.black;
            var indexContent = new GUIContent(index.ToString());
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

        private void HandlePreviewInput(Rect previewRect)
        {
            var currentEvent = Event.current;
            if (!previewRect.Contains(currentEvent.mousePosition))
                return;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (currentEvent.button == 0 || currentEvent.button == 2)
                    {
                        isDraggingPreview = true;
                        lastMousePosition = currentEvent.mousePosition;
                        lastPanPosition = previewPan;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    isDraggingPreview = false;
                    break;

                case EventType.MouseDrag:
                    if (isDraggingPreview)
                    {
                        var delta = (currentEvent.mousePosition - lastMousePosition);
                        previewPan = lastPanPosition + delta;
                        GUI.changed = true;
                        currentEvent.Use();
                    }
                    break;

                case EventType.ScrollWheel:
                    var newZoom = previewZoom;
                    newZoom = Mathf.Clamp(newZoom - currentEvent.delta.y * ZOOM_SPEED, ZOOM_MIN, ZOOM_MAX);

                    if (!Mathf.Approximately(newZoom, previewZoom))
                    {
                        // Adjust pan to keep the point under the cursor in the same position
                        var mouseViewPos = currentEvent.mousePosition - new Vector2(previewRect.width * 0.5f, previewRect.height * 0.5f);
                        var oldWorldPos = (mouseViewPos - previewPan) / previewZoom;
                        var newWorldPos = (mouseViewPos - previewPan) / newZoom;
                        previewPan += (newWorldPos - oldWorldPos) * newZoom;

                        previewZoom = newZoom;
                        GUI.changed = true;
                        currentEvent.Use();
                    }
                    break;
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
                    colors[y * 32 + x] = (x == 0 || y == 0) 
                        ? new Color(1f, 1f, 1f, 0.1f) 
                        : new Color(1f, 1f, 1f, 0.05f);
                }
            }

            gridTexture.SetPixels(colors);
            gridTexture.Apply();
            gridTexture.wrapMode = TextureWrapMode.Repeat;
        }

        private void CreateBackgroundTexture()
        {
            backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1));
            backgroundTexture.Apply();
        }

        private void InitializePreviewStyle()
        {
            previewStyle = new GUIStyle();
            previewStyle.normal.background = EditorGUIUtility.whiteTexture;
        }

        private void CleanupTextures()
        {
            if (gridTexture != null)
            {
                Object.DestroyImmediate(gridTexture);
            }
            if (backgroundTexture != null)
            {
                Object.DestroyImmediate(backgroundTexture);
            }
        }

        private void ResetPreviewView()
        {
            previewZoom = 1f;
            previewPan = Vector2.zero;
            GUI.changed = true;
        }
    }
}