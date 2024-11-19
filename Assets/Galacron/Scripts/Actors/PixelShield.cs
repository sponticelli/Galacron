using UnityEngine;

namespace Galacron.Actors
{
   public class PixelShield : MonoBehaviour
    {
        [SerializeField] private GameObject pixelPrefab;  // 2x2 sprite with collider and health
        [SerializeField] private int shieldWidth = 11;    // Width in pixels
        [SerializeField] private int shieldHeight = 8;    // Height in pixels
        [SerializeField] private float pixelSize = 0.125f;  // Size of each pixel in Unity units
        
        private PixelBlock[,] pixels;

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
                    // Skip pixels for classic Space Invaders shape
                    if (!ShouldCreatePixel(x, y)) continue;

                    Vector3 pixelPos = startPos + new Vector2(
                        x * pixelSize + pixelSize/2,  // Center of pixel
                        y * pixelSize + pixelSize/2
                    );

                    GameObject pixel = Instantiate(pixelPrefab, pixelPos, Quaternion.identity, transform);
                    pixel.name = $"Pixel_{x}_{y}";
                    
                    pixels[x, y] = new PixelBlock(pixel, x, y);
                }
            }
        }

        private bool ShouldCreatePixel(int x, int y)
        {
            return true;
        }

        // Optional: Method to check shield integrity
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

    // Helper class to manage individual pixels
    public class PixelBlock
    {
        private GameObject gameObject;
        private Health health;
        private int x, y;

        public PixelBlock(GameObject obj, int xPos, int yPos)
        {
            gameObject = obj;
            health = obj.GetComponent<Health>();
            x = xPos;
            y = yPos;
        }

        public bool IsValid() => gameObject != null;
        public bool IsAlive() => IsValid() && health != null && health.CurrentHealth > 0;
        public Vector2 Position => new Vector2(x, y);
    }
}