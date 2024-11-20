using System.Collections.Generic;
using Nexus.Pooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Galacron.Actors
{
   public class PixelShield : MonoBehaviour
    {
        [FormerlySerializedAs("pixelPrefab")] [SerializeField] private PoolReference<PixelShieldBrick> pixelBrickPrefab;  // 2x2 sprite with collider and health
        [SerializeField] private int shieldWidth = 11;    // Width in pixels
        [SerializeField] private int shieldHeight = 8;    // Height in pixels
        [SerializeField] private float pixelSize = 0.125f;  // Size of each pixel in Unity units
        
        private PixelBlock[,] pixels;
        private HashSet<PixelBlock> exposedPixels = new HashSet<PixelBlock>();


        private void Awake()
        {
            BuildShield();
        }

        private void BuildShield()
        {
            pixels = new PixelBlock[shieldWidth, shieldHeight];
            
            // Calculate start position to center the shield
            Vector2 startPos = transform.position - new Vector3(
                (shieldWidth * pixelSize) / 2f,
                (shieldHeight * pixelSize) / 2f
            );

            // Build the shield pixel by pixel
            for (int x = 0; x < shieldWidth; x++)
            {
                for (int y = 0; y < shieldHeight; y++)
                {
                    Vector3 pixelPos = startPos + new Vector2(
                        x * pixelSize + pixelSize/2,  // Center of pixel
                        y * pixelSize + pixelSize/2
                    );

                    var pixel = pixelBrickPrefab.Get(pixelPos, Quaternion.identity);
                    pixel.transform.parent = transform;

                    pixel.name = $"Pixel_{x}_{y}";
                    
                    var pixelBlock = new PixelBlock(this, pixel, x, y);
                    pixelBlock.SetColliderEnabled(false);
                    pixels[x, y] = pixelBlock;
                }
            }
            
            UpdateExposedPixels();
        }
        
        private void UpdateExposedPixels()
        {
            exposedPixels.Clear();

            for (int x = 0; x < shieldWidth; x++)
            {
                for (int y = 0; y < shieldHeight; y++)
                {
                    if (pixels[x, y] == null || !pixels[x, y].IsAlive()) continue;

                    // Check if this pixel is exposed (has an empty or destroyed neighbor)
                    if (IsExposedPixel(x, y))
                    {
                        exposedPixels.Add(pixels[x, y]);
                        pixels[x, y].SetColliderEnabled(true);
                    }
                }
            }
        }
        
        private bool IsExposedPixel(int x, int y)
        {
            // Check all 8 neighboring positions
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                // If position is outside bounds or empty/destroyed, this pixel is exposed
                if (newX < 0 || newX >= shieldWidth || newY < 0 || newY >= shieldHeight ||
                    pixels[newX, newY] == null || !pixels[newX, newY].IsAlive())
                {
                    return true;
                }
            }

            return false;
        }
        
        public void OnPixelDestroyed(PixelBlock pixel)
        {
            Debug.Log($"Pixel at {pixel.X}, {pixel.Y} destroyed!");
            // Remove the destroyed pixel from exposed set
            exposedPixels.Remove(pixel);

            // Check neighbors and update their exposure status
            int x = pixel.X;
            int y = pixel.Y;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    if (newX >= 0 && newX < shieldWidth && newY >= 0 && newY < shieldHeight &&
                        pixels[newX, newY] != null && pixels[newX, newY].IsAlive())
                    {
                        Debug.Log($"Checking neighbor at {newX}, {newY}");
                        
                        // If this neighbor is now exposed, enable its collider
                        if (IsExposedPixel(newX, newY))
                        {
                            Debug.Log($"Neighbor at {newX}, {newY} is now exposed!");
                            pixels[newX, newY].SetColliderEnabled(true);
                            exposedPixels.Add(pixels[newX, newY]);
                        }
                    }
                }
            }
        }
        
        


        public float GetIntegrity()
        {
            int totalPixels = 0;
            int remainingPixels = 0;

            for (int x = 0; x < shieldWidth; x++)
            {
                for (int y = 0; y < shieldHeight; y++)
                {
                    if (pixels[x,y] != null && pixels[x,y].IsValid())
                    {
                        totalPixels++;
                        if (pixels[x,y].IsAlive())
                        {
                            remainingPixels++;
                        }
                    }
                }
            }

            return totalPixels > 0 ? (float)remainingPixels / totalPixels : 0f;
        }
    }
}