using UnityEngine;
using System.Collections.Generic;
using Nexus.Pooling;

namespace Galacron.Actors
{
    public partial class PixelShield : MonoBehaviour
    {
        [Header("Shield Configuration")] 
        [SerializeField] private ShieldShape shieldShape;
        [SerializeField] private float pixelSize = 0.125f;
        [SerializeField] private bool optimizeColliders = true;

        [Header("Scrolling Configuration")] 
        [SerializeField] private float scrollSpeed = 1f;
        [SerializeField] private bool enableScrolling = false;

        [Header("Rendering")] 
        [SerializeField] private PoolReference<PixelShieldBrick> pixelBrickPrefab;

        private PixelBlock[,] pixels;
        private HashSet<PixelBlock> exposedPixels = new HashSet<PixelBlock>();
        private List<PixelShieldBrick> activeBricks = new List<PixelShieldBrick>();
        private Vector3 lastPosition;
        private Vector2 shieldCenter;
        private bool isInitialized = false;
        
        public ShieldShape Shape => shieldShape;
        public float PixelSize => pixelSize;

        private void OnEnable()
        {
            if (!isInitialized && shieldShape != null)
            {
                BuildShield();
                isInitialized = true;
            }
            else if (isInitialized)
            {
                RebuildShield();
            }
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (enableScrolling)
            {
                Vector3 deltaPosition = transform.position - lastPosition;
                ScrollPixels(deltaPosition);
                lastPosition = transform.position;
            }
        }
        

        private void ScrollPixels(Vector3 parentDelta)
        {
            float scrollAmount = scrollSpeed * Time.deltaTime;
            shieldCenter = transform.position;
            float width = shieldShape.Width * pixelSize;
            float halfWidth = width / 2f;

            // Update all pixel positions
            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    if (pixels[x, y] == null || !pixels[x, y].IsAlive()) continue;

                    // Get the pixel's position relative to the shield before scrolling
                    Vector2 relativePos = pixels[x, y].WorldPosition - (Vector2)(transform.position - parentDelta);

                    // Apply scrolling
                    relativePos.x += scrollAmount;

                    // Wrap position if necessary
                    if (relativePos.x > halfWidth)
                    {
                        relativePos.x = -halfWidth + (relativePos.x - halfWidth);
                    }
                    else if (relativePos.x < -halfWidth)
                    {
                        relativePos.x = halfWidth - (-halfWidth - relativePos.x);
                    }

                    // Update the pixel's world position
                    Vector2 newWorldPos = (Vector2)transform.position + relativePos;
                    pixels[x, y].SetWorldPosition(newWorldPos);
                }
            }

            // Update grid positions after moving all pixels
            UpdatePixelGrid();
        }

        private void UpdatePixelGrid()
        {
            PixelBlock[,] newGrid = new PixelBlock[shieldShape.Width, shieldShape.Height];
            float halfWidth = (shieldShape.Width * pixelSize) / 2f;
            float halfHeight = (shieldShape.Height * pixelSize) / 2f;

            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    if (pixels[x, y] == null || !pixels[x, y].IsAlive()) continue;

                    // Calculate position relative to shield center
                    Vector2 relativePos = pixels[x, y].WorldPosition - (Vector2)transform.position;

                    // Convert to grid coordinates
                    int newX = Mathf.RoundToInt((relativePos.x + halfWidth) / pixelSize - 0.5f);
                    newX = WrapIndex(newX, shieldShape.Width);

                    // Update pixel's grid position
                    pixels[x, y].UpdateGridPosition(newX, y);
                    newGrid[newX, y] = pixels[x, y];
                }
            }

            pixels = newGrid;
            if (optimizeColliders) UpdateExposedPixels();
        }


        private int WrapIndex(int index, int max)
        {
            return ((index % max) + max) % max;
        }

        private void RebuildShield()
        {
            if (shieldShape == null) return;
            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    var pixelBlock = pixels[x, y];
                    if (pixelBlock != null)
                    {
                        if (pixelBlock.Brick == null)
                        {
                            var pixelPos = pixelBlock.WorldPosition;
                            var pixel = pixelBrickPrefab.Get(pixelPos, Quaternion.identity);
                            pixel.transform.parent = transform;
                            pixel.name = $"Pixel_{x}_{y}";
                            pixelBlock.Brick = pixel;
                        }
                        pixelBlock.SetColliderEnabled(!optimizeColliders);
                        pixelBlock.Revive();
                    }
                }
            }
            if (optimizeColliders) UpdateExposedPixels();
            
        }

        private void BuildShield()
        {
            // Clean up any existing bricks first
            foreach (Transform child in transform)
            {
                if (child.GetComponent<PixelShieldBrick>() != null)
                {
                    child.gameObject.ReturnToPool();
                }
            }

            pixels = new PixelBlock[shieldShape.Width, shieldShape.Height];
            shieldCenter = transform.position;
            activeBricks.Clear();

            float halfWidth = (shieldShape.Width * pixelSize) / 2f;
            float halfHeight = (shieldShape.Height * pixelSize) / 2f;
            Vector2 startPosition = shieldCenter - new Vector2(halfWidth, halfHeight);

            for (int x = 0; x < shieldShape.Width; x++)
            {
                for (int y = 0; y < shieldShape.Height; y++)
                {
                    if (!shieldShape.GetPixel(x, y)) continue;

                    Vector3 pixelPos = startPosition + new Vector2(
                        x * pixelSize + pixelSize / 2,
                        y * pixelSize + pixelSize / 2
                    );

                    var pixel = pixelBrickPrefab.Get(pixelPos, Quaternion.identity);
                    pixel.transform.parent = transform; // Set parent directly
                    pixel.name = $"Pixel_{x}_{y}";
                    
                    activeBricks.Add(pixel);

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
                    else
                    {
                        pixels[x, y].SetColliderEnabled(false); // Make sure to disable non-exposed pixels
                    }
                }
            }
        }

        private bool IsExposedPixel(int x, int y)
        {
            // All 8 directions: up, up-right, right, down-right, down, down-left, left, up-left
            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                // Check if the neighboring position is outside the shield or doesn't have an alive pixel
                if (newX < 0 || newX >= shieldShape.Width ||
                    newY < 0 || newY >= shieldShape.Height ||
                    !shieldShape.GetPixel(newX, newY) ||
                    pixels[newX, newY] == null ||
                    !pixels[newX, newY].IsAlive())
                {
                    return true; // This pixel is exposed
                }
            }

            return false; // This pixel is surrounded by other pixels
        }

        public void OnPixelDestroyed(PixelBlock pixel)
        {
            exposedPixels.Remove(pixel);
            // Remove from activeBricks list when destroyed
            if (pixel?.Brick != null)
            {
                activeBricks.Remove(pixel.Brick);
                // Let the pooling system handle returning to pool
            }

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
                    if (pixels[x, y] != null && pixels[x, y].IsAlive())
                    {
                        remainingPixels++;
                    }
                }
            }

            return totalPixels > 0 ? (float)remainingPixels / totalPixels : 0f;
        }
    }
}