using UnityEditor;
using UnityEngine;


namespace Galacron.PostProcessors
{
    public class SpriteImportSettings : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (assetPath.Contains("Sprites"))
            {
                TextureImporter importer = assetImporter as TextureImporter;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spritePixelsPerUnit = 16;
                importer.mipmapEnabled = false;
                importer.textureType = TextureImporterType.Sprite;
                //importer.spriteImportMode = SpriteImportMode.Single;
            }
        }
    }
}