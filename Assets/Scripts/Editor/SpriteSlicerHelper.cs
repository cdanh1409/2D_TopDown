#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using TopDownSurvival.Core;

namespace TopDownSurvival.Editor
{
    public static class SpriteSlicerHelper
    {
        [MenuItem("Tools/Slice and Map Sprites to Objects")]
        public static void SliceAndMap()
        {
            // 1. Slice 11.png (Trees and items)
            SliceSpriteGrid("Assets/Sprites/11.png", 32, 32);
            // 2. Slice 3.png (Nature elements)
            SliceSpriteGrid("Assets/Sprites/3.png", 32, 32);
            // 3. Slice 2.png (Rocks)
            SliceSpriteGrid("Assets/Sprites/2.png", 32, 32);

            AssetDatabase.Refresh();

            // Load sliced sub-sprites
            Sprite[] itemsSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/11.png") as Sprite[] ?? new Sprite[0];
            Sprite[] natureSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/3.png") as Sprite[] ?? new Sprite[0];
            Sprite[] rockSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/2.png") as Sprite[] ?? new Sprite[0];

            // Helper to get sprite by name or index
            Sprite treeSprite = GetSpriteFromList(natureSprites, "3_14"); // Green tree index
            Sprite rockSprite = GetSpriteFromList(rockSprites, "2_2"); // Large rock index
            Sprite bushSprite = GetSpriteFromList(natureSprites, "3_13"); // Bush element
            Sprite playerSprite = GetSpriteFromList(natureSprites, "3_0"); // Character sprite placeholder
            
            Sprite woodItemSprite = GetSpriteFromList(itemsSprites, "11_40"); // Wood chunk
            Sprite stoneItemSprite = GetSpriteFromList(rockSprites, "2_30"); // Stone chunk
            Sprite berryItemSprite = GetSpriteFromList(itemsSprites, "11_24"); // Berry / Fruit chunk

            // Assign to ItemData ScriptableObjects
            ItemData wood = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/WoodItemData.asset");
            if (wood != null && woodItemSprite != null)
            {
                wood.icon = woodItemSprite;
                EditorUtility.SetDirty(wood);
            }

            ItemData stone = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/StoneItemData.asset");
            if (stone != null && stoneItemSprite != null)
            {
                stone.icon = stoneItemSprite;
                EditorUtility.SetDirty(stone);
            }

            ItemData berry = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/BerryItemData.asset");
            if (berry != null && berryItemSprite != null)
            {
                berry.icon = berryItemSprite;
                EditorUtility.SetDirty(berry);
            }

            // Map to GameObjects in the scene
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null && playerSprite != null)
            {
                SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = playerSprite;
                    sr.color = Color.white; // Reset color to see actual sprite
                }
            }

            GameObject treeObj = GameObject.Find("Tree");
            if (treeObj != null && treeSprite != null)
            {
                SpriteRenderer sr = treeObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = treeSprite;
                    sr.color = Color.white; // Reset color to see actual sprite
                }
            }

            GameObject rockObj = GameObject.Find("Rock");
            if (rockObj != null && rockSprite != null)
            {
                SpriteRenderer sr = rockObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = rockSprite;
                    sr.color = Color.white;
                }
            }

            GameObject berryBushObj = GameObject.Find("BerryBush");
            if (berryBushObj != null && bushSprite != null)
            {
                SpriteRenderer sr = berryBushObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = bushSprite;
                    sr.color = Color.white;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sprites Slicer", "Sprites sliced and mapped successfully to Player, Items, Bush, and Resource Objects!", "OK");
        }

        private static void SliceSpriteGrid(string path, int cellWidth, int cellHeight)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null) return;

            int width  = texture.width;
            int height = texture.height;

            // Calculate how many full cells fit — do NOT go beyond texture edge
            int cols = width  / cellWidth;
            int rows = height / cellHeight;

            List<SpriteMetaData> metas = new List<SpriteMetaData>();
            int index = 0;

            string filename = System.IO.Path.GetFileNameWithoutExtension(path);

            // Unity UV: y=0 is bottom. We iterate top-to-bottom visually → bottom-to-top in UV.
            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = col * cellWidth;
                    int y = row * cellHeight;

                    // Extra safety — skip any rect outside texture (shouldn't happen but belt-and-braces)
                    if (x + cellWidth > width || y + cellHeight > height) continue;

                    SpriteMetaData meta = new SpriteMetaData
                    {
                        rect      = new Rect(x, y, cellWidth, cellHeight),
                        alignment = (int)SpriteAlignment.Center,
                        name      = $"{filename}_{index}"
                    };
                    metas.Add(meta);
                    index++;
                }
            }

#pragma warning disable CS0618
            importer.spritesheet = metas.ToArray();
#pragma warning restore CS0618
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }


        private static Sprite GetSpriteFromList(Object[] list, string name)
        {
            foreach (var obj in list)
            {
                if (obj is Sprite s && s.name == name)
                {
                    return s;
                }
            }
            // Fallback to first available sprite
            foreach (var obj in list)
            {
                if (obj is Sprite s) return s;
            }
            return null;
        }
    }
}
#endif
