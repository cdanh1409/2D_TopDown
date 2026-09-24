#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TopDownSurvival.Editor
{
    public static class SpriteImporterHelper
    {
        [MenuItem("Tools/Import and Configure Copied Sprites")]
        public static void ConfigureSprites()
        {
            string[] texturePaths = new string[]
            {
                "Assets/Sprites/11.png",
                "Assets/Sprites/2.png",
                "Assets/Sprites/3.png",
                "Assets/Sprites/WoodTileset.png"
            };

            foreach (string path in texturePaths)
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    Debug.LogWarning($"[Sprite Importer] Texture not found at path: {path}");
                    continue;
                }

                // Make sure it's 2D Sprite Multiple
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                
                // Pixel Art best settings
                importer.spritePixelsPerUnit = 16;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                // Configure platform settings
                TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings
                {
                    maxTextureSize = 2048,
                    format = TextureImporterFormat.RGBA32,
                    textureCompression = TextureImporterCompression.Uncompressed
                };
                importer.SetPlatformTextureSettings(settings);

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                Debug.Log($"[Sprite Importer] Successfully configured: {path}");
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Sprites Importer", "All copied sprites configured to Sprite Multiple / Point Filter / 16 PPU!", "OK");
        }
    }
}
#endif
