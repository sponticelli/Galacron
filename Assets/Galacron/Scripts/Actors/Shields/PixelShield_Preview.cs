using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Galacron.Actors
{
    public partial class PixelShield
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (shieldShape == null) return;
            
            // Cache colors for restoration
            Color originalGizmoColor = Gizmos.color;
            Color originalHandleColor = Handles.color;
            
            // Calculate shield bounds
            Vector2 startPos = transform.position - new Vector3(
                (shieldShape.Width * pixelSize) / 2f,
                (shieldShape.Height * pixelSize) / 2f
            );
            
            // Draw overall bounds
            Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.2f);
            Gizmos.DrawWireCube(
                transform.position,
                new Vector3(shieldShape.Width * pixelSize, shieldShape.Height * pixelSize, 0)
            );
            
            // Draw individual pixels
            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    if (!shieldShape.GetPixel(x, y)) continue;

                    Vector3 pixelPos = startPos + new Vector2(
                        x * pixelSize + pixelSize/2,
                        y * pixelSize + pixelSize/2
                    );
                    
                    // Draw filled pixels
                    Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.3f);
                    Gizmos.DrawCube(pixelPos, Vector3.one * pixelSize * 0.9f);
                    
                    // Draw pixel outlines
                    Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.8f);
                    Gizmos.DrawWireCube(pixelPos, Vector3.one * pixelSize);
                    
                    // If in play mode, show exposed pixels differently
                    if (Application.isPlaying && pixels != null)
                    {
                        if (pixels[x,y] != null)
                        {
                            if (pixels[x,y].IsAlive())
                            {
                                if (exposedPixels.Contains(pixels[x,y]))
                                {
                                    // Highlight exposed pixels
                                    Gizmos.color = new Color(1f, 0.1f, 0.11f, 1f);
                                    Gizmos.DrawWireCube(pixelPos, Vector3.one * pixelSize * 1.1f);
                                }
                            }
                            else
                            {
                                // Show destroyed pixels
                                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
                                Gizmos.DrawWireCube(pixelPos, Vector3.one * pixelSize * 0.8f);
                            }
                        }
                    }
                }
            }
            
            // Draw shield integrity in play mode
            if (Application.isPlaying)
            {
                float integrity = GetIntegrity();
                Vector3 labelPos = transform.position + Vector3.up * (shieldShape.Height * pixelSize / 2f + 0.2f);
                Handles.color = Color.white;
                Handles.Label(labelPos, $"Shield Integrity: {integrity:P0}");
            }
            
            // Restore original colors
            Gizmos.color = originalGizmoColor;
            Handles.color = originalHandleColor;
        }
#endif
    }
}