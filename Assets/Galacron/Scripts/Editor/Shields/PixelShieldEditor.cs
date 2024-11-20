using UnityEngine;
using UnityEditor;

namespace Galacron.Actors
{
    [CustomEditor(typeof(PixelShield))]
    public class PixelShieldEditor : Editor
    {
        private Color previewColor = new Color(0.2f, 1f, 0.2f, 0.3f);
        private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.2f);
        private bool showPreview = true;

        public override void OnInspectorGUI()
        {
            var shield = (PixelShield)target;
            
            // Add preview toggle
            EditorGUI.BeginChangeCheck();
            showPreview = EditorGUILayout.Toggle("Show Preview", showPreview);
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
            
            EditorGUILayout.Space();
            
            // Draw default inspector
            DrawDefaultInspector();
            
            // Add button to open Shield Shape Editor
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Shape Editor"))
            {
                if (shield.Shape != null)
                {
                    ShieldShapeEditor.ShowWindow();
                    Selection.activeObject = shield.Shape;
                }
                else
                {
                    EditorUtility.DisplayDialog("No Shape Assigned", 
                        "Please assign a Shield Shape asset first.", "OK");
                }
            }
        }

        private void OnSceneGUI()
        {
            var shield = (PixelShield)target;
            if (!showPreview || shield.Shape == null) return;

            // Calculate grid origin (bottom-left corner)
            Vector3 origin = shield.transform.position - new Vector3(
                (shield.Shape.Width * shield.PixelSize) / 2f,
                (shield.Shape.Height * shield.PixelSize) / 2f,
                0
            );

            // Draw preview grid
            Handles.matrix = Matrix4x4.identity;
            
            for (int x = 0; x < shield.Shape.Width; x++)
            {
                for (int y = 0; y < shield.Shape.Height; y++)
                {
                    var invertedY = shield.Shape.Height - 1 - y;
                    
                    Vector3 pixelPos = origin + new Vector3(
                        x * shield.PixelSize + shield.PixelSize/2,
                        invertedY * shield.PixelSize + shield.PixelSize/2,
                        0
                    );

                    bool isActive = shield.Shape.GetPixel(x, shield.Shape.Height - 1 - y);
                    Vector3[] corners = {
                        pixelPos + new Vector3(-shield.PixelSize/2, -shield.PixelSize/2, 0),
                        pixelPos + new Vector3(shield.PixelSize/2, -shield.PixelSize/2, 0),
                        pixelPos + new Vector3(shield.PixelSize/2, shield.PixelSize/2, 0),
                        pixelPos + new Vector3(-shield.PixelSize/2, shield.PixelSize/2, 0)
                    };

                    Handles.DrawSolidRectangleWithOutline(
                        corners, 
                        isActive ? previewColor : inactiveColor,
                        Color.clear
                    );
                }
            }
        }
        
        
    }
}