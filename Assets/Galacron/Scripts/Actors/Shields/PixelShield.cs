using System.Collections.Generic;
using Nexus.Pooling;
using UnityEngine;

namespace Galacron.Actors
{
    public class PixelShield : MonoBehaviour
    {
        [SerializeField] private PoolReference<PixelShieldBrick> pixelBrickPrefab;
        [SerializeField] private ShieldShape shieldShape;
        [SerializeField] private float pixelSize = 0.125f;
        [SerializeField] private bool optimizeColliders = true;
        
        private PixelBlock[,] pixels;
        private HashSet<PixelBlock> exposedPixels = new HashSet<PixelBlock>();

        public ShieldShape Shape => shieldShape;
        public float PixelSize => pixelSize;

        private void OnEnable()
        {
            if (shieldShape != null)
            {
                BuildShield();
            }
            else
            {
                Debug.LogError("No shield shape assigned!", this);
            }
        }

        private void BuildShield()
        {
            pixels = new PixelBlock[shieldShape.Width, shieldShape.Height];
            
            Vector2 startPos = transform.position - new Vector3(
                (shieldShape.Width * pixelSize) / 2f,
                (shieldShape.Height * pixelSize) / 2f
            );

            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    // Skip if this pixel is disabled in the shape
                    if (!shieldShape.GetPixel(x, y)) continue;

                    Vector3 pixelPos = startPos + new Vector2(
                        x * pixelSize + pixelSize/2,
                        y * pixelSize + pixelSize/2
                    );

                    var pixel = pixelBrickPrefab.Get(pixelPos, Quaternion.identity);
                    pixel.transform.parent = transform;
                    pixel.name = $"Pixel_{x}_{y}";
                    
                    var pixelBlock = new PixelBlock(this, pixel, x, y);
                    pixelBlock.SetColliderEnabled(!optimizeColliders);
                    pixels[x, y] = pixelBlock;
                }
            }
            if (optimizeColliders) UpdateExposedPixels();
        }

        private void UpdateExposedPixels()
        {
            exposedPixels.Clear();

            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    if (pixels[x, y] == null || !pixels[x, y].IsAlive()) continue;

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
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (newX < 0 || newX >= shieldShape.Width || 
                    newY < 0 || newY >= shieldShape.Height ||
                    !shieldShape.GetPixel(newX, newY) || 
                    pixels[newX, newY] == null || !pixels[newX, newY].IsAlive())
                {
                    return true;
                }
            }

            return false;
        }
        
        public void OnPixelDestroyed(PixelBlock pixel)
        {
            exposedPixels.Remove(pixel);

            if (optimizeColliders)
            {
                UpdateExposedPixels();
            }
        }

        public float GetIntegrity()
        {
            int totalPixels = 0;
            int remainingPixels = 0;

            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    if (!shieldShape.GetPixel(x, y)) continue;
                    
                    totalPixels++;
                    if (pixels[x,y] != null && pixels[x,y].IsAlive())
                    {
                        remainingPixels++;
                    }
                }
            }

            return totalPixels > 0 ? (float)remainingPixels / totalPixels : 0f;
        }
    }
}