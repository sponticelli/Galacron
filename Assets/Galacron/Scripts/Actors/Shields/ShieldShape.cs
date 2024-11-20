using UnityEngine;

namespace Galacron.Actors
{
    [CreateAssetMenu(fileName = "ShieldShape", menuName = "Galacron/Shield Shape")]
    public class ShieldShape : ScriptableObject
    {
        [SerializeField] private int width = 11;
        [SerializeField] private int height = 8;
        [SerializeField] private bool[] shape;
        
        public int Width => width;
        public int Height => height;
        public bool[] Shape => shape;

        public void Initialize()
        {
            int size = width * height;
            if (shape == null || shape.Length != size)
            {
                shape = new bool[size];
                for (int i = 0; i < size; i++)
                {
                    shape[i] = true;
                }
            }
        }

        public void Resize(int newWidth, int newHeight)
        {
            // Create new array with new dimensions
            bool[] newShape = new bool[newWidth * newHeight];
            
            // Copy existing data where possible
            for (int y = 0; y < Mathf.Min(height, newHeight); y++)
            {
                for (int x = 0; x < Mathf.Min(width, newWidth); x++)
                {
                    int oldIndex = y * width + x;
                    int newIndex = y * newWidth + x;
                    if (oldIndex < shape.Length)
                    {
                        newShape[newIndex] = shape[oldIndex];
                    }
                    else
                    {
                        newShape[newIndex] = true; // Default value for new cells
                    }
                }
            }

            // Set new dimensions and shape
            width = newWidth;
            height = newHeight;
            shape = newShape;
        }

        public bool GetPixel(int x, int y)
        {
            int index = y * width + x;
            return index >= 0 && index < shape.Length && shape[index];
        }

        public void SetPixel(int x, int y, bool value)
        {
            int index = y * width + x;
            if (index >= 0 && index < shape.Length)
            {
                shape[index] = value;
            }
        }

        public void Clear(bool defaultValue = true)
        {
            for (int i = 0; i < shape.Length; i++)
            {
                shape[i] = defaultValue;
            }
        }

        private void OnValidate()
        {
            width = Mathf.Max(1, width);
            height = Mathf.Max(1, height);
            Initialize();
        }
    }
}