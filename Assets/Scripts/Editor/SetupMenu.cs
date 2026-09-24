#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using TopDownSurvival.Player;
using TopDownSurvival.World;
using TopDownSurvival.Core;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using TopDownSurvival.UI;


namespace TopDownSurvival.Editor
{
    public static class SetupMenu
    {
        [MenuItem("Tools/Setup TopDown Prototype")]
        public static void SetupPrototype()
        {
            if (EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("Warning", "Cannot run Setup Prototype during Play Mode! Please stop the game first.", "OK");
                return;
            }

            AssetDatabase.Refresh();

            // Fallbacks to default Unity textures
            Sprite defaultSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            Sprite knobSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");



            // --- 0. Prepare Item assets ---
            if (!AssetDatabase.IsValidFolder("Assets/Items"))
            {
                AssetDatabase.CreateFolder("Assets", "Items");
            }

            ItemData wood = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/WoodItemData.asset");
            if (wood == null)
            {
                wood = ScriptableObject.CreateInstance<ItemData>();
                wood.itemId = "wood";
                wood.displayName = "Wood";
                wood.description = "A basic building material.";
                wood.icon = defaultSprite;
                wood.isResource = true;
                AssetDatabase.CreateAsset(wood, "Assets/Items/WoodItemData.asset");
            }
            else
            {
                wood.icon = defaultSprite;
                EditorUtility.SetDirty(wood);
            }


            ItemData stone = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/StoneItemData.asset");
            if (stone == null)
            {
                stone = ScriptableObject.CreateInstance<ItemData>();
                stone.itemId = "stone";
                stone.displayName = "Stone";
                stone.description = "A sturdy crafting resource.";
                stone.icon = knobSprite;
                stone.isResource = true;
                AssetDatabase.CreateAsset(stone, "Assets/Items/StoneItemData.asset");
            }
            else
            {
                stone.icon = knobSprite;
                EditorUtility.SetDirty(stone);
            }


            ItemData axe = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/AxeItemData.asset");
            if (axe == null)
            {
                axe = ScriptableObject.CreateInstance<ItemData>();
                axe.itemId = "axe";
                axe.displayName = "Axe";
                axe.description = "A tool used for gathering wood faster.";
                axe.icon = defaultSprite;
                axe.isResource = false;
                axe.isStackable = false;
                AssetDatabase.CreateAsset(axe, "Assets/Items/AxeItemData.asset");
            }
            else
            {
                axe.icon = defaultSprite;
                EditorUtility.SetDirty(axe);
            }


            ItemData berry = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Items/BerryItemData.asset");
            if (berry == null)
            {
                berry = ScriptableObject.CreateInstance<ItemData>();
                berry.itemId = "berry";
                berry.displayName = "Berries";
                berry.description = "Delicious forest berries. Restores hunger and health.";
                berry.icon = knobSprite;
                berry.isResource = false;
                berry.isConsumable = true;
                berry.hungerRestoreValue = 25f;
                berry.healthRestoreValue = 5f;
                AssetDatabase.CreateAsset(berry, "Assets/Items/BerryItemData.asset");
            }
            else
            {
                berry.icon = knobSprite;
                EditorUtility.SetDirty(berry);
            }



            // Create Axe Recipe (requires 3 wood)
            RecipeData axeRecipe = AssetDatabase.LoadAssetAtPath<RecipeData>("Assets/Items/AxeRecipe.asset");
            if (axeRecipe == null)
            {
                axeRecipe = ScriptableObject.CreateInstance<RecipeData>();
                
                ItemStack woodCost = new ItemStack(wood, 3);
                axeRecipe.ingredients = new List<ItemStack> { woodCost };
                axeRecipe.result = new ItemStack(axe, 1);

                AssetDatabase.CreateAsset(axeRecipe, "Assets/Items/AxeRecipe.asset");
            }

            // Create Quest Data assets
            QuestData gatherWoodQuest = AssetDatabase.LoadAssetAtPath<QuestData>("Assets/Items/Quest_GatherWood.asset");
            if (gatherWoodQuest == null)
            {
                gatherWoodQuest = ScriptableObject.CreateInstance<QuestData>();
                gatherWoodQuest.questId = "gather_wood";
                gatherWoodQuest.title = "Gather Wood";
                gatherWoodQuest.description = "Chặt cây để thu thập gỗ.";
                gatherWoodQuest.type = QuestType.Gather;
                gatherWoodQuest.targetItem = wood;
                gatherWoodQuest.requiredAmount = 5;
                gatherWoodQuest.rewardExp = 100;
                AssetDatabase.CreateAsset(gatherWoodQuest, "Assets/Items/Quest_GatherWood.asset");
            }

            QuestData gatherStoneQuest = AssetDatabase.LoadAssetAtPath<QuestData>("Assets/Items/Quest_GatherStone.asset");
            if (gatherStoneQuest == null)
            {
                gatherStoneQuest = ScriptableObject.CreateInstance<QuestData>();
                gatherStoneQuest.questId = "gather_stone";
                gatherStoneQuest.title = "Gather Stone";
                gatherStoneQuest.description = "Đập đá để thu thập đá.";
                gatherStoneQuest.type = QuestType.Gather;
                gatherStoneQuest.targetItem = stone;
                gatherStoneQuest.requiredAmount = 5;
                gatherStoneQuest.rewardExp = 150;
                AssetDatabase.CreateAsset(gatherStoneQuest, "Assets/Items/Quest_GatherStone.asset");
            }

            // Create Kill Quest
            QuestData killZombieQuest = AssetDatabase.LoadAssetAtPath<QuestData>("Assets/Items/Quest_KillZombies.asset");
            if (killZombieQuest == null)
            {
                killZombieQuest = ScriptableObject.CreateInstance<QuestData>();
                killZombieQuest.questId = "kill_zombies";
                killZombieQuest.title = "Kill Zombies";
                killZombieQuest.description = "Tiêu diệt 3 kẻ thù để bảo vệ căn cứ!";
                killZombieQuest.type = QuestType.Kill;
                killZombieQuest.enemyTag = "Enemy";
                killZombieQuest.requiredAmount = 3;
                killZombieQuest.rewardExp = 200;
                AssetDatabase.CreateAsset(killZombieQuest, "Assets/Items/Quest_KillZombies.asset");
            }

            // Create LootTable
            LootTable enemyLootTable = AssetDatabase.LoadAssetAtPath<LootTable>("Assets/Items/EnemyLootTable.asset");
            if (enemyLootTable == null)
            {
                enemyLootTable = ScriptableObject.CreateInstance<LootTable>();
                enemyLootTable.entries = new System.Collections.Generic.List<LootEntry>
                {
                    new LootEntry { item = wood,  dropChance = 0.7f, minAmount = 1, maxAmount = 2 },
                    new LootEntry { item = stone, dropChance = 0.5f, minAmount = 1, maxAmount = 1 }
                };
                AssetDatabase.CreateAsset(enemyLootTable, "Assets/Items/EnemyLootTable.asset");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


            // --- 1. Find or create Player ---
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                player = GameObject.Find("Player");
                if (player == null)
                {
                    player = new GameObject("Player");
                }
                player.tag = "Player";
            }

            // --- 2. Configure Player Components ---
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
            {
                controller = player.AddComponent<PlayerController>();
            }

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            BoxCollider2D collider = player.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                player.AddComponent<BoxCollider2D>();
            }

            // Load character sprite from sprite sheet
            Sprite charSprite = GetSpriteFromSheet("Assets/Sprites/11.png", "11_1");
            if (charSprite == null) charSprite = GetSpriteFromSheet("Assets/Sprites/11.png", "11_0");
            if (charSprite == null) charSprite = defaultSprite;


            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = player.AddComponent<SpriteRenderer>();
            }
            sr.sprite = defaultSprite;
            sr.color = Color.green;
            sr.sortingOrder = 11; // Ensure player is rendered on top of fog




            PlayerInteraction interaction = player.GetComponent<PlayerInteraction>();
            if (interaction == null)
            {
                interaction = player.AddComponent<PlayerInteraction>();
            }
            SerializedObject soInt = new SerializedObject(interaction);
            SerializedProperty layerProp = soInt.FindProperty("interactableLayer");
            if (layerProp != null)
            {
                layerProp.intValue = -1; // Everything
                soInt.ApplyModifiedProperties();
            }

            Inventory inventory = player.GetComponent<Inventory>();
            if (inventory == null)
            {
                inventory = player.AddComponent<Inventory>();
            }

            PlayerCraftingTest craftingTest = player.GetComponent<PlayerCraftingTest>();
            if (craftingTest == null)
            {
                craftingTest = player.AddComponent<PlayerCraftingTest>();
            }
            
            SerializedObject soCraft = new SerializedObject(craftingTest);
            soCraft.FindProperty("testRecipe").objectReferenceValue = axeRecipe;
            soCraft.ApplyModifiedProperties();

            SurvivalStats stats = player.GetComponent<SurvivalStats>();
            if (stats == null)
            {
                stats = player.AddComponent<SurvivalStats>();
            }

            PlayerUseItemTest useItemTest = player.GetComponent<PlayerUseItemTest>();
            if (useItemTest == null)
            {
                useItemTest = player.AddComponent<PlayerUseItemTest>();
            }

            PlayerAttacker combat = player.GetComponent<PlayerAttacker>();
            if (combat == null)
            {
                combat = player.AddComponent<PlayerAttacker>();
            }
            
            SerializedObject soCombat = new SerializedObject(combat);
            soCombat.FindProperty("enemyLayer").intValue = -1; // Everything
            soCombat.ApplyModifiedProperties();

            PlayerAnimator animator = player.GetComponent<PlayerAnimator>();
            if (animator == null)
            {
                animator = player.AddComponent<PlayerAnimator>();
            }

            PlayerRoleManager roleManager = player.GetComponent<PlayerRoleManager>();
            if (roleManager == null)
            {
                roleManager = player.AddComponent<PlayerRoleManager>();
            }

            PlayerProgression progression = player.GetComponent<PlayerProgression>();
            if (progression == null)
            {
                progression = player.AddComponent<PlayerProgression>();
            }

            // Create BuildingData Assets
            BuildingData campfireData = AssetDatabase.LoadAssetAtPath<BuildingData>("Assets/Items/CampfireBuildingData.asset");
            if (campfireData == null)
            {
                campfireData = ScriptableObject.CreateInstance<BuildingData>();
                campfireData.buildingName = "Campfire";
                campfireData.ghostColor = new Color(1f, 0.6f, 0.2f, 0.4f);
                campfireData.buildingColor = new Color(1f, 0.5f, 0f, 1f);
                campfireData.size = new Vector2(1.2f, 1.2f);
                campfireData.buildCost = new List<ItemStack> { new ItemStack(wood, 3), new ItemStack(stone, 1) };
                AssetDatabase.CreateAsset(campfireData, "Assets/Items/CampfireBuildingData.asset");
            }

            BuildingData wallData = AssetDatabase.LoadAssetAtPath<BuildingData>("Assets/Items/WallBuildingData.asset");
            if (wallData == null)
            {
                wallData = ScriptableObject.CreateInstance<BuildingData>();
                wallData.buildingName = "Wall";
                wallData.ghostColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                wallData.buildingColor = new Color(0.3f, 0.3f, 0.3f, 1f);
                wallData.size = new Vector2(1f, 1f);
                wallData.buildCost = new List<ItemStack> { new ItemStack(stone, 2) };
                AssetDatabase.CreateAsset(wallData, "Assets/Items/WallBuildingData.asset");
            }

            PlayerBuilding playerBuilding = player.GetComponent<PlayerBuilding>();
            if (playerBuilding == null)
            {
                playerBuilding = player.AddComponent<PlayerBuilding>();
            }
            playerBuilding.AddBuildingOption(campfireData);
            playerBuilding.AddBuildingOption(wallData);

            // --- 3. Setup Camera ---
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                CameraController camController = mainCam.GetComponent<CameraController>();
                if (camController == null)
                {
                    camController = mainCam.gameObject.AddComponent<CameraController>();
                }
                
                SerializedObject so = new SerializedObject(camController);
                SerializedProperty targetProp = so.FindProperty("target");
                if (targetProp != null)
                {
                    targetProp.objectReferenceValue = player.transform;
                    so.ApplyModifiedProperties();
                }
            }

            // --- 4. Setup GameManager ---
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager == null)
            {
                gameManager = new GameObject("GameManager");
            }
            TimeManager timeManager = gameManager.GetComponent<TimeManager>();
            if (timeManager == null)
            {
                timeManager = gameManager.AddComponent<TimeManager>();
            }

            QuestManager questManager = gameManager.GetComponent<QuestManager>();
            if (questManager == null)
            {
                questManager = gameManager.AddComponent<QuestManager>();
            }
            SerializedObject soQM = new SerializedObject(questManager);
            SerializedProperty questListProp = soQM.FindProperty("allQuests");
            questListProp.ClearArray();
            questListProp.InsertArrayElementAtIndex(0);
            questListProp.GetArrayElementAtIndex(0).objectReferenceValue = gatherWoodQuest;
            questListProp.InsertArrayElementAtIndex(1);
            questListProp.GetArrayElementAtIndex(1).objectReferenceValue = gatherStoneQuest;
            questListProp.InsertArrayElementAtIndex(2);
            questListProp.GetArrayElementAtIndex(2).objectReferenceValue = killZombieQuest;
            soQM.ApplyModifiedProperties();


            // --- 5. Create Storage Chest ---
            GameObject oldChest = GameObject.Find("InteractableChest");
            if (oldChest != null)
            {
                Object.DestroyImmediate(oldChest);
            }

            GameObject chestObj = GameObject.Find("StorageChest");
            if (chestObj == null)
            {
                chestObj = new GameObject("StorageChest");
                chestObj.transform.position = new Vector3(2f, 2f, 0f);
                
                SpriteRenderer itemSr = chestObj.AddComponent<SpriteRenderer>();
                itemSr.sprite = knobSprite;
                itemSr.color = Color.yellow;

                BoxCollider2D itemCol = chestObj.AddComponent<BoxCollider2D>();
                itemCol.isTrigger = true;

                chestObj.AddComponent<Inventory>();
                chestObj.AddComponent<StorageChest>();
            }
            else
            {
                SpriteRenderer itemSr = chestObj.GetComponent<SpriteRenderer>();
                if (itemSr != null)
                {
                    itemSr.sprite = knobSprite;
                    itemSr.color = Color.yellow;
                }
            }

            // --- 6. Create Resource Objects (Tree, Rock, & Berry Bush) ---
            GameObject treeObj = GameObject.Find("Tree");
            if (treeObj == null)
            {
                treeObj = new GameObject("Tree");
                treeObj.transform.position = new Vector3(-3f, 2f, 0f);
                
                SpriteRenderer treeSr = treeObj.AddComponent<SpriteRenderer>();
                treeSr.sprite = defaultSprite;
                treeSr.color = new Color(0.1f, 0.7f, 0.2f);
                treeSr.sortingOrder = 5;

                
                BoxCollider2D treeCol = treeObj.AddComponent<BoxCollider2D>();
                treeCol.isTrigger = true;

                ResourceObject treeRes = treeObj.AddComponent<ResourceObject>();
                SerializedObject soTree = new SerializedObject(treeRes);
                soTree.FindProperty("resourceName").stringValue = "Tree";
                soTree.FindProperty("itemDrop").objectReferenceValue = wood;
                soTree.FindProperty("maxHitPoints").intValue = 3;
                soTree.ApplyModifiedProperties();
            }
            else
            {
                SpriteRenderer treeSr = treeObj.GetComponent<SpriteRenderer>();
                if (treeSr != null)
                {
                    treeSr.sprite = defaultSprite;
                    treeSr.color = new Color(0.1f, 0.7f, 0.2f);
                    treeSr.sortingOrder = 5;
                }
            }

            GameObject rockObj = GameObject.Find("Rock");
            if (rockObj == null)
            {
                rockObj = new GameObject("Rock");
                rockObj.transform.position = new Vector3(-3f, -2f, 0f);

                SpriteRenderer rockSr = rockObj.AddComponent<SpriteRenderer>();
                rockSr.sprite = knobSprite;
                rockSr.color = Color.gray;
                rockSr.sortingOrder = 5;


                BoxCollider2D rockCol = rockObj.AddComponent<BoxCollider2D>();
                rockCol.isTrigger = true;

                ResourceObject rockRes = rockObj.AddComponent<ResourceObject>();
                SerializedObject soRock = new SerializedObject(rockRes);
                soRock.FindProperty("resourceName").stringValue = "Rock";
                soRock.FindProperty("itemDrop").objectReferenceValue = stone;
                soRock.FindProperty("maxHitPoints").intValue = 4;
                soRock.ApplyModifiedProperties();
            }
            else
            {
                SpriteRenderer rockSr = rockObj.GetComponent<SpriteRenderer>();
                if (rockSr != null)
                {
                    rockSr.sprite = knobSprite;
                    rockSr.color = Color.gray;
                    rockSr.sortingOrder = 5;
                }
            }

            GameObject berryBushObj = GameObject.Find("BerryBush");
            if (berryBushObj == null)
            {
                berryBushObj = new GameObject("BerryBush");
                berryBushObj.transform.position = new Vector3(3f, -2f, 0f);

                SpriteRenderer bushSr = berryBushObj.AddComponent<SpriteRenderer>();
                bushSr.sprite = knobSprite;
                bushSr.color = new Color(0.8f, 0.1f, 0.1f);
                bushSr.sortingOrder = 5;


                BoxCollider2D bushCol = berryBushObj.AddComponent<BoxCollider2D>();
                bushCol.isTrigger = true;

                ResourceObject bushRes = berryBushObj.AddComponent<ResourceObject>();
                SerializedObject soBush = new SerializedObject(bushRes);
                soBush.FindProperty("resourceName").stringValue = "Berry Bush";
                soBush.FindProperty("itemDrop").objectReferenceValue = berry;
                soBush.FindProperty("maxHitPoints").intValue = 2;
                soBush.ApplyModifiedProperties();
            }
            else
            {
                SpriteRenderer bushSr = berryBushObj.GetComponent<SpriteRenderer>();
                if (bushSr != null)
                {
                    bushSr.sprite = knobSprite;
                    bushSr.color = new Color(0.8f, 0.1f, 0.1f);
                    bushSr.sortingOrder = 5;
                }
            }

            // --- 7. Create Enemy & Boss ---
            GameObject oldEnemy = GameObject.Find("Zombie");
            if (oldEnemy != null) Object.DestroyImmediate(oldEnemy);

            GameObject enemyObj = new GameObject("Zombie");
            enemyObj.transform.position = new Vector3(-5f, 0f, 0f);
            
            // Ensure tag exists in Editor
            #if UNITY_EDITOR
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            bool hasTag = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == "Enemy")
                {
                    hasTag = true;
                    break;
                }
            }
            if (!hasTag)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = "Enemy";
                tagManager.ApplyModifiedProperties();
            }
            #endif

            enemyObj.tag = "Enemy";
            
            SpriteRenderer enemySr = enemyObj.AddComponent<SpriteRenderer>();
            enemySr.sprite = defaultSprite;
            enemySr.color = Color.red;
            enemySr.sortingOrder = 5;

            BoxCollider2D enemyCol = enemyObj.AddComponent<BoxCollider2D>();
            
            Damageable enemyDmg = enemyObj.AddComponent<Damageable>();
            EnemyAI enemyAI = enemyObj.AddComponent<EnemyAI>();
            SerializedObject soEnemy = new SerializedObject(enemyAI);
            soEnemy.FindProperty("lootTable").objectReferenceValue = enemyLootTable;
            soEnemy.ApplyModifiedProperties();

            // Create prefab directory if missing and save Zombie as prefab for the spawner
            if (!Directory.Exists("Assets/Prefabs"))
            {
                Directory.CreateDirectory("Assets/Prefabs");
            }
            GameObject zombiePrefab = PrefabUtility.SaveAsPrefabAsset(enemyObj, "Assets/Prefabs/ZombiePrefab.prefab");

            GameObject oldBoss = GameObject.Find("BossBehemoth");
            if (oldBoss != null) Object.DestroyImmediate(oldBoss);

            GameObject bossObj = new GameObject("BossBehemoth");
            bossObj.transform.position = new Vector3(0f, 5f, 0f);
            bossObj.transform.localScale = new Vector3(2f, 2f, 1f);
            bossObj.tag = "Enemy";

            SpriteRenderer bossSr = bossObj.AddComponent<SpriteRenderer>();
            bossSr.sprite = defaultSprite;
            bossSr.color = new Color(0.5f, 0.1f, 0.6f);
            bossSr.sortingOrder = 5;

            BoxCollider2D bossCol = bossObj.AddComponent<BoxCollider2D>();
            
            Damageable bossDmg = bossObj.AddComponent<Damageable>();
            BossAI bossAI = bossObj.AddComponent<BossAI>();
            SerializedObject soBoss = new SerializedObject(bossAI);
            soBoss.FindProperty("lootTable").objectReferenceValue = enemyLootTable;
            soBoss.ApplyModifiedProperties();


            // --- 7b. Create Enemy Spawner ---
            GameObject oldSpawner = GameObject.Find("EnemySpawner");
            if (oldSpawner != null) Object.DestroyImmediate(oldSpawner);

            GameObject spawnerObj = new GameObject("EnemySpawner");
            EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();
            SerializedObject soSpawner = new SerializedObject(spawner);
            SerializedProperty enemyPrefabsProp = soSpawner.FindProperty("enemyPrefabs");
            enemyPrefabsProp.ClearArray();
            enemyPrefabsProp.InsertArrayElementAtIndex(0);
            enemyPrefabsProp.GetArrayElementAtIndex(0).objectReferenceValue = zombiePrefab;
            soSpawner.ApplyModifiedProperties();


            // --- 8. Setup Fog of War Grid ---
            Grid grid = Object.FindAnyObjectByType<Grid>();
            if (grid == null)
            {
                GameObject gridObj = new GameObject("Grid");
                grid = gridObj.AddComponent<Grid>();
            }

            GameObject fogObj = GameObject.Find("FogOfWarTilemap");
            if (fogObj == null)
            {
                fogObj = new GameObject("FogOfWarTilemap");
                fogObj.transform.SetParent(grid.transform);
                
                Tilemap tm = fogObj.AddComponent<Tilemap>();
                TilemapRenderer tmr = fogObj.AddComponent<TilemapRenderer>();
                tmr.sortingOrder = 10; // Set fog sorting order higher to render on top
                
                Tile fogTileAsset = AssetDatabase.LoadAssetAtPath<Tile>("Assets/Items/FogTile.asset");
                if (fogTileAsset == null)
                {
                    fogTileAsset = ScriptableObject.CreateInstance<Tile>();
                    fogTileAsset.sprite = knobSprite;
                    fogTileAsset.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
                    AssetDatabase.CreateAsset(fogTileAsset, "Assets/Items/FogTile.asset");
                }

                FogOfWar fow = fogObj.AddComponent<FogOfWar>();
                
                SerializedObject soFow = new SerializedObject(fow);
                soFow.FindProperty("fogTilemap").objectReferenceValue = tm;
                soFow.FindProperty("fogTile").objectReferenceValue = fogTileAsset;
                soFow.FindProperty("playerTransform").objectReferenceValue = player.transform;
                soFow.ApplyModifiedProperties();
            }

            // --- 9. Setup UI Canvas & HUD ---
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                // Create HUD Container
                GameObject hudPanel = new GameObject("HUDPanel");
                hudPanel.transform.SetParent(canvasObj.transform, false);
                RectTransform hudRt = hudPanel.AddComponent<RectTransform>();
                hudRt.anchorMin = new Vector2(0, 1);
                hudRt.anchorMax = new Vector2(0, 1);
                hudRt.pivot = new Vector2(0, 1);
                hudRt.anchoredPosition = new Vector2(20, -20);
                hudRt.sizeDelta = new Vector2(250, 100);

                // Helper to create UI Slider
                Slider healthS = CreateStatSlider(hudPanel, "HealthSlider", new Color(0.9f, 0.2f, 0.2f), new Vector2(0, 0));
                Slider hungerS = CreateStatSlider(hudPanel, "HungerSlider", new Color(0.9f, 0.6f, 0.1f), new Vector2(0, -30));
                Slider thirstS = CreateStatSlider(hudPanel, "ThirstSlider", new Color(0.2f, 0.5f, 0.9f), new Vector2(0, -60));

                TopDownSurvival.UI.SurvivalHUD hud = canvasObj.AddComponent<TopDownSurvival.UI.SurvivalHUD>();
                SerializedObject soHud = new SerializedObject(hud);
                soHud.FindProperty("healthSlider").objectReferenceValue = healthS;
                soHud.FindProperty("hungerSlider").objectReferenceValue = hungerS;
                soHud.FindProperty("thirstSlider").objectReferenceValue = thirstS;
                soHud.ApplyModifiedProperties();

                // Create Inventory UI Panel
                GameObject invPanel = new GameObject("InventoryPanel");
                invPanel.transform.SetParent(canvasObj.transform, false);
                RectTransform invRt = invPanel.AddComponent<RectTransform>();
                invRt.anchorMin = new Vector2(0.5f, 0.5f);
                invRt.anchorMax = new Vector2(0.5f, 0.5f);
                invRt.pivot = new Vector2(0.5f, 0.5f);
                invRt.anchoredPosition = Vector2.zero;
                invRt.sizeDelta = new Vector2(400, 300);
                Image invImg = invPanel.AddComponent<Image>();
                invImg.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

                // Create Container for Slots
                GameObject containerObj = new GameObject("SlotsContainer");
                containerObj.transform.SetParent(invPanel.transform, false);
                RectTransform contRt = containerObj.AddComponent<RectTransform>();
                contRt.anchorMin = Vector2.zero;
                contRt.anchorMax = Vector2.one;
                contRt.sizeDelta = new Vector2(-40, -40); // margin
                GridLayoutGroup gridGroup = containerObj.AddComponent<GridLayoutGroup>();
                gridGroup.cellSize = new Vector2(50, 50);
                gridGroup.spacing = new Vector2(10, 10);
                gridGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridGroup.constraintCount = 5;

                // Create 20 Slots
                for (int i = 0; i < 20; i++)
                {
                    GameObject slot = new GameObject($"Slot_{i}");
                    slot.transform.SetParent(containerObj.transform, false);
                    Image slotImg = slot.AddComponent<Image>();
                    slotImg.color = new Color(0.25f, 0.25f, 0.25f, 1f);

                    // Child for Icon
                    GameObject iconObj = new GameObject("Icon");
                    iconObj.transform.SetParent(slot.transform, false);
                    RectTransform iconRt = iconObj.AddComponent<RectTransform>();
                    iconRt.anchorMin = Vector2.zero;
                    iconRt.anchorMax = Vector2.one;
                    iconRt.sizeDelta = Vector2.zero;
                    Image iconImg = iconObj.AddComponent<Image>();
                    iconImg.enabled = false;

                    // Child for Count text
                    GameObject countObj = new GameObject("Count");
                    countObj.transform.SetParent(slot.transform, false);
                    RectTransform countRt = countObj.AddComponent<RectTransform>();
                    countRt.anchorMin = new Vector2(0.5f, 0);
                    countRt.anchorMax = Vector2.one;
                    countRt.sizeDelta = Vector2.zero;
                    Text countTxt = countObj.AddComponent<Text>();
                    countTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    countTxt.alignment = TextAnchor.LowerRight;
                    countTxt.fontSize = 12;
                    countTxt.color = Color.white;
                    countTxt.enabled = false;
                }

                TopDownSurvival.UI.InventoryUI invUI = canvasObj.AddComponent<TopDownSurvival.UI.InventoryUI>();
                SerializedObject soInv = new SerializedObject(invUI);
                soInv.FindProperty("uiPanel").objectReferenceValue = invPanel;
                soInv.FindProperty("slotsContainer").objectReferenceValue = containerObj.transform;
                soInv.ApplyModifiedProperties();

                // Create Crafting UI Panel
                GameObject craftPanel = new GameObject("CraftingPanel");
                craftPanel.transform.SetParent(invPanel.transform, false);
                RectTransform craftRt = craftPanel.AddComponent<RectTransform>();
                craftRt.anchorMin = new Vector2(1, 0.5f);
                craftRt.anchorMax = new Vector2(1, 0.5f);
                craftRt.pivot = new Vector2(0, 0.5f);
                craftRt.anchoredPosition = new Vector2(20, 0);
                craftRt.sizeDelta = new Vector2(200, 200);
                Image craftImg = craftPanel.AddComponent<Image>();
                craftImg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

                // Recipe text
                GameObject textObj = new GameObject("RecipeText");
                textObj.transform.SetParent(craftPanel.transform, false);
                RectTransform textRt = textObj.AddComponent<RectTransform>();
                textRt.anchorMin = new Vector2(0, 0.3f);
                textRt.anchorMax = Vector2.one;
                textRt.sizeDelta = new Vector2(-20, -20);
                Text recipeText = textObj.AddComponent<Text>();
                recipeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                recipeText.fontSize = 14;
                recipeText.color = Color.white;
                recipeText.alignment = TextAnchor.UpperLeft;

                // Craft Button
                GameObject btnObj = new GameObject("CraftButton");
                btnObj.transform.SetParent(craftPanel.transform, false);
                RectTransform btnRt = btnObj.AddComponent<RectTransform>();
                btnRt.anchorMin = new Vector2(0.5f, 0);
                btnRt.anchorMax = new Vector2(0.5f, 0);
                btnRt.pivot = new Vector2(0.5f, 0);
                btnRt.anchoredPosition = new Vector2(0, 15);
                btnRt.sizeDelta = new Vector2(120, 30);
                Image btnImg = btnObj.AddComponent<Image>();
                btnImg.color = new Color(0.4f, 0.8f, 0.4f);
                Button btn = btnObj.AddComponent<Button>();

                GameObject btnTextObj = new GameObject("Text");
                btnTextObj.transform.SetParent(btnObj.transform, false);
                Text btnTxt = btnTextObj.AddComponent<Text>();
                btnTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                btnTxt.fontSize = 14;
                btnTxt.color = Color.black;
                btnTxt.text = "CRAFT";
                btnTxt.alignment = TextAnchor.MiddleCenter;

                TopDownSurvival.UI.CraftingUI craftUI = canvasObj.AddComponent<TopDownSurvival.UI.CraftingUI>();
                SerializedObject soCraftUI = new SerializedObject(craftUI);
                soCraftUI.FindProperty("recipeToCraft").objectReferenceValue = axeRecipe;
                soCraftUI.FindProperty("craftButton").objectReferenceValue = btn;
                soCraftUI.FindProperty("recipeCostText").objectReferenceValue = recipeText;
                soCraftUI.ApplyModifiedProperties();

                // --- Quest HUD Panel (Top Right) ---
                GameObject questPanel = new GameObject("QuestHUDPanel");
                questPanel.transform.SetParent(canvasObj.transform, false);
                RectTransform questRt = questPanel.AddComponent<RectTransform>();
                questRt.anchorMin = new Vector2(1, 1);
                questRt.anchorMax = new Vector2(1, 1);
                questRt.pivot = new Vector2(1, 1);
                questRt.anchoredPosition = new Vector2(-20, -20);
                questRt.sizeDelta = new Vector2(260, 140);
                Image questBg = questPanel.AddComponent<Image>();
                questBg.color = new Color(0.1f, 0.1f, 0.1f, 0.75f);

                // Quest Text
                GameObject questTxtObj = new GameObject("QuestText");
                questTxtObj.transform.SetParent(questPanel.transform, false);
                RectTransform qtRt = questTxtObj.AddComponent<RectTransform>();
                qtRt.anchorMin = new Vector2(0, 0.35f);
                qtRt.anchorMax = Vector2.one;
                qtRt.offsetMin = new Vector2(10, 0);
                qtRt.offsetMax = new Vector2(-10, -10);
                Text questTxt = questTxtObj.AddComponent<Text>();
                questTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                questTxt.fontSize = 13;
                questTxt.color = new Color(0.9f, 0.85f, 0.6f);
                questTxt.alignment = TextAnchor.UpperLeft;
                questTxt.text = "Nhiệm vụ:\nChặt Gỗ (0/5)";

                // Level text
                GameObject lvlObj = new GameObject("LevelText");
                lvlObj.transform.SetParent(questPanel.transform, false);
                RectTransform lvlRt = lvlObj.AddComponent<RectTransform>();
                lvlRt.anchorMin = new Vector2(0, 0.2f);
                lvlRt.anchorMax = new Vector2(0.4f, 0.37f);
                lvlRt.offsetMin = new Vector2(10, 0);
                lvlRt.offsetMax = new Vector2(0, 0);
                Text lvlTxt = lvlObj.AddComponent<Text>();
                lvlTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                lvlTxt.fontSize = 14;
                lvlTxt.color = new Color(0.6f, 0.9f, 1f);
                lvlTxt.alignment = TextAnchor.MiddleLeft;
                lvlTxt.text = "Lv. 1";

                // EXP Slider
                Slider expSlider = CreateStatSlider(questPanel, "EXPSlider", new Color(0.4f, 0.85f, 0.4f), new Vector2(10, -10));
                RectTransform expSRt = expSlider.GetComponent<RectTransform>();
                expSRt.anchorMin = new Vector2(0, 0);
                expSRt.anchorMax = new Vector2(1, 0.22f);
                expSRt.pivot = new Vector2(0, 0);
                expSRt.offsetMin = new Vector2(10, 5);
                expSRt.offsetMax = new Vector2(-10, 0);

                TopDownSurvival.UI.QuestHUD questHUD = canvasObj.AddComponent<TopDownSurvival.UI.QuestHUD>();
                SerializedObject soQH = new SerializedObject(questHUD);
                soQH.FindProperty("questText").objectReferenceValue = questTxt;
                soQH.FindProperty("levelText").objectReferenceValue = lvlTxt;
                soQH.FindProperty("expSlider").objectReferenceValue = expSlider;
                soQH.ApplyModifiedProperties();

                // --- Floating Text World Canvas & Prefab ---
                GameObject ftPrefab = new GameObject("FloatingTextPrefab");
                ftPrefab.transform.position = Vector3.zero;
                Canvas ftCanvas = ftPrefab.AddComponent<Canvas>();
                ftCanvas.renderMode = RenderMode.WorldSpace;
                RectTransform ftCanvasRt = ftPrefab.GetComponent<RectTransform>();
                ftCanvasRt.sizeDelta = new Vector3(2f, 1f, 1f);
                ftPrefab.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 100;
                
                GameObject ftTextObj = new GameObject("Text");
                ftTextObj.transform.SetParent(ftPrefab.transform, false);
                RectTransform ftTextRt = ftTextObj.AddComponent<RectTransform>();
                ftTextRt.anchorMin = Vector2.zero;
                ftTextRt.anchorMax = Vector2.one;
                ftTextRt.sizeDelta = Vector2.zero;
                Text ftText = ftTextObj.AddComponent<Text>();
                ftText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                ftText.fontSize = 24;
                ftText.alignment = TextAnchor.MiddleCenter;
                ftText.color = Color.white;
                // Add shadow for better readability
                ftTextObj.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.7f);

                FloatingText ftScript = ftPrefab.AddComponent<FloatingText>();
                // Save dynamically as prefab
                if (!Directory.Exists("Assets/Prefabs"))
                {
                    Directory.CreateDirectory("Assets/Prefabs");
                }
                GameObject ftPrefabFile = PrefabUtility.SaveAsPrefabAsset(ftPrefab, "Assets/Prefabs/FloatingText.prefab");
                Object.DestroyImmediate(ftPrefab);

                // Wire FloatingText prefab to Player, Zombie, Boss
                if (player != null)
                {
                    Damageable pDmg = player.GetComponent<Damageable>();
                    if (pDmg != null)
                    {
                        SerializedObject soPD = new SerializedObject(pDmg);
                        soPD.FindProperty("floatingTextPrefab").objectReferenceValue = ftPrefabFile;
                        soPD.ApplyModifiedProperties();
                    }
                    PlayerRoleManager pRole = player.GetComponent<PlayerRoleManager>();
                    if (pRole != null)
                    {
                        SerializedObject soPR = new SerializedObject(pRole);
                        soPR.FindProperty("floatingTextPrefab").objectReferenceValue = ftPrefabFile;
                        soPR.ApplyModifiedProperties();
                    }
                }
                if (zombiePrefab != null)
                {
                    Damageable zDmg = zombiePrefab.GetComponent<Damageable>();
                    if (zDmg != null)
                    {
                        SerializedObject soZD = new SerializedObject(zDmg);
                        soZD.FindProperty("floatingTextPrefab").objectReferenceValue = ftPrefabFile;
                        soZD.ApplyModifiedProperties();
                    }
                }
                if (bossObj != null)
                {
                    Damageable bDmg = bossObj.GetComponent<Damageable>();
                    if (bDmg != null)
                    {
                        SerializedObject soBD = new SerializedObject(bDmg);
                        soBD.FindProperty("floatingTextPrefab").objectReferenceValue = ftPrefabFile;
                        soBD.ApplyModifiedProperties();
                    }
                }

                // --- 10. Damage Overlay (Red Screen Flash) ---
                GameObject dmgOverlayObj = new GameObject("DamageOverlay");
                dmgOverlayObj.transform.SetParent(canvasObj.transform, false);
                RectTransform doRt = dmgOverlayObj.AddComponent<RectTransform>();
                doRt.anchorMin = Vector2.zero;
                doRt.anchorMax = Vector2.one;
                doRt.sizeDelta = Vector2.zero;
                Image doImg = dmgOverlayObj.AddComponent<Image>();
                doImg.color = Color.clear;

                // --- 11. Announcement Panel (Level Up Popup) ---
                GameObject announcePanel = new GameObject("AnnouncementPanel");
                announcePanel.transform.SetParent(canvasObj.transform, false);
                RectTransform apRt = announcePanel.AddComponent<RectTransform>();
                apRt.anchorMin = new Vector2(0.5f, 0.65f);
                apRt.anchorMax = new Vector2(0.5f, 0.65f);
                apRt.pivot = new Vector2(0.5f, 0.5f);
                apRt.anchoredPosition = Vector2.zero;
                apRt.sizeDelta = new Vector2(400, 100);
                
                Image apImg = announcePanel.AddComponent<Image>();
                apImg.color = new Color(0.05f, 0.05f, 0.05f, 0.85f);

                // Title (e.g. LEVEL UP!)
                GameObject apTitleObj = new GameObject("TitleText");
                apTitleObj.transform.SetParent(announcePanel.transform, false);
                RectTransform aptRt = apTitleObj.AddComponent<RectTransform>();
                aptRt.anchorMin = new Vector2(0, 0.5f);
                aptRt.anchorMax = Vector2.one;
                aptRt.offsetMin = new Vector2(10, 0);
                aptRt.offsetMax = new Vector2(-10, -10);
                Text apTitle = apTitleObj.AddComponent<Text>();
                apTitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                apTitle.fontSize = 26;
                apTitle.alignment = TextAnchor.MiddleCenter;
                apTitle.color = new Color(0.4f, 0.9f, 1f);

                // Subtitle (e.g. Current Level: 2)
                GameObject apSubObj = new GameObject("SubtitleText");
                apSubObj.transform.SetParent(announcePanel.transform, false);
                RectTransform apsRt = apSubObj.AddComponent<RectTransform>();
                apsRt.anchorMin = Vector2.zero;
                apsRt.anchorMax = new Vector2(1, 0.5f);
                apsRt.offsetMin = new Vector2(10, 5);
                apsRt.offsetMax = new Vector2(-10, 0);
                Text apSub = apSubObj.AddComponent<Text>();
                apSub.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                apSub.fontSize = 15;
                apSub.alignment = TextAnchor.MiddleCenter;
                apSub.color = Color.white;

                // ScreenEffects component config
                ScreenEffects fx = canvasObj.AddComponent<ScreenEffects>();
                SerializedObject soFx = new SerializedObject(fx);
                soFx.FindProperty("damageOverlay").objectReferenceValue = doImg;
                soFx.FindProperty("announcementPanel").objectReferenceValue = announcePanel;
                soFx.FindProperty("announcementTitle").objectReferenceValue = apTitle;
                soFx.FindProperty("announcementSubtitle").objectReferenceValue = apSub;
                soFx.ApplyModifiedProperties();
            }


            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                
                #if ENABLE_INPUT_SYSTEM
                eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                #else
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                #endif
            }

            // Save the scene
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

            Debug.Log("Prototype setup completed successfully! Placeholder shapes configured.");
        }

        private static Slider CreateStatSlider(GameObject parent, string name, Color fillColor, Vector2 pos)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent.transform, false);
            RectTransform rt = sliderObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(200, 20);

            Slider slider = sliderObj.AddComponent<Slider>();

            // Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRt = bgObj.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.sizeDelta = Vector2.zero;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform faRt = fillArea.AddComponent<RectTransform>();
            faRt.anchorMin = Vector2.zero;
            faRt.anchorMax = Vector2.one;
            faRt.sizeDelta = Vector2.zero;

            // Fill
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fillObj.AddComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = new Vector2(1, 1); // fill width controlled by slider
            fillRt.sizeDelta = Vector2.zero;
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = fillColor;

            slider.fillRect = fillRt;
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = 100f;

            return slider;
        }

        /// <summary>
        /// Loads a named sub-sprite from a sprite sheet (sliced texture).
        /// AssetDatabase.LoadAssetAtPath does NOT work for sub-sprites — must use LoadAllAssetsAtPath.
        /// </summary>
        private static Sprite GetSpriteFromSheet(string sheetPath, string spriteName)
        {
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(sheetPath);
            foreach (Object asset in allAssets)
            {
                if (asset is Sprite spr && spr.name == spriteName)
                    return spr;
            }
            return null;
        }
    }
}
#endif
