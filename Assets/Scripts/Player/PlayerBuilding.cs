using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;
using TopDownSurvival.World;

namespace TopDownSurvival.Player
{
    [RequireComponent(typeof(Inventory))]
    public class PlayerBuilding : MonoBehaviour
    {
        [Header("Available Buildings")]
        [SerializeField] private List<BuildingData> availableBuildings = new List<BuildingData>();

        private Inventory inventory;
        private PlayerRoleManager roleManager;
        private int selectedIndex = 0;
        private bool isBuildMode = false;

        // Ghost Preview
        private GameObject ghostObject;
        private SpriteRenderer ghostRenderer;
        private Sprite defaultSprite;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            roleManager = GetComponent<PlayerRoleManager>();

            // Find a sprite to use for ghost
            SpriteRenderer playerSr = GetComponent<SpriteRenderer>();
            if (playerSr != null)
            {
                defaultSprite = playerSr.sprite;
            }
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // Toggle Build Mode with B
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                isBuildMode = !isBuildMode;
                if (isBuildMode)
                {
                    CreateGhost();
                }
                else
                {
                    DestroyGhost();
                }
                Debug.Log($"[Building] Build Mode: {isBuildMode}");
            }

            if (!isBuildMode) return;

            // Switch structure using Q / E
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                SwitchSelection(-1);
            }
            else if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SwitchSelection(1);
            }

            // Update ghost position to mouse world coordinates
            UpdateGhostPosition();

            // Place building with Left Click (or Enter)
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryPlaceBuilding();
            }
        }

        private void CreateGhost()
        {
            if (availableBuildings.Count == 0) return;

            ghostObject = new GameObject("BuildGhost");
            ghostRenderer = ghostObject.AddComponent<SpriteRenderer>();
            ghostRenderer.sprite = defaultSprite;
            ghostRenderer.sortingOrder = 10;
            
            UpdateGhostVisuals();
        }

        private void DestroyGhost()
        {
            if (ghostObject != null)
            {
                Destroy(ghostObject);
                ghostObject = null;
                ghostRenderer = null;
            }
        }

        private void SwitchSelection(int direction)
        {
            if (availableBuildings.Count <= 1) return;

            selectedIndex = (selectedIndex + direction + availableBuildings.Count) % availableBuildings.Count;
            UpdateGhostVisuals();
            Debug.Log($"[Building] Selected: {availableBuildings[selectedIndex].buildingName}");
        }

        private void UpdateGhostVisuals()
        {
            if (ghostRenderer == null || availableBuildings.Count == 0) return;

            BuildingData current = availableBuildings[selectedIndex];
            ghostRenderer.color = current.ghostColor;
            ghostObject.transform.localScale = new Vector3(current.size.x, current.size.y, 1f);
        }

        private void UpdateGhostPosition()
        {
            if (ghostObject == null || Camera.main == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
            worldPos.z = 0f; // Force 2D
            
            // Snap to grid of 0.5 units for better alignment
            worldPos.x = Mathf.Round(worldPos.x * 2f) / 2f;
            worldPos.y = Mathf.Round(worldPos.y * 2f) / 2f;

            ghostObject.transform.position = worldPos;
        }

        private void TryPlaceBuilding()
        {
            if (availableBuildings.Count == 0 || ghostObject == null) return;

            BuildingData target = availableBuildings[selectedIndex];
            bool isEngineer = roleManager != null && roleManager.IsEngineer();

            // 1. Check resources
            foreach (var cost in target.buildCost)
            {
                int required = GetCostWithPerk(cost.count, isEngineer);
                if (inventory.GetTotalCount(cost.item) < required)
                {
                    Debug.LogWarning($"[Building] Missing materials to place {target.buildingName}! Need {required}x {cost.item.displayName}.");
                    return;
                }
            }

            // 2. Consume resources
            foreach (var cost in target.buildCost)
            {
                int required = GetCostWithPerk(cost.count, isEngineer);
                inventory.RemoveItem(cost.item, required);
                if (isEngineer && required < cost.count)
                {
                    Debug.Log($"[Building - Engineer] Discount applied: consumed {required} instead of {cost.count} {cost.item.displayName}");
                }
            }

            // 3. Spawn building
            GameObject placed = new GameObject(target.buildingName);
            placed.transform.position = ghostObject.transform.position;
            placed.transform.localScale = new Vector3(target.size.x, target.size.y, 1f);

            SpriteRenderer sr = placed.AddComponent<SpriteRenderer>();
            sr.sprite = defaultSprite;
            sr.sortingOrder = 4; // Below items, above ground

            BoxCollider2D col = placed.AddComponent<BoxCollider2D>();
            // Campfires are triggers (so player can walk over/interact), Walls are solid blockades
            if (target.buildingName == "Campfire")
            {
                col.isTrigger = true;
            }
            else
            {
                col.isTrigger = false;
            }

            PlacedBuilding pb = placed.AddComponent<PlacedBuilding>();
            pb.Setup(target.buildingName, target.buildingColor);

            Debug.Log($"[Building] Placed {target.buildingName} at {placed.transform.position}");
            inventory.PrintInventory();
        }

        private int GetCostWithPerk(int originalCount, bool isEngineer)
        {
            if (!isEngineer) return originalCount;
            return Mathf.Max(1, Mathf.CeilToInt(originalCount * 0.5f));
        }

        public void AddBuildingOption(BuildingData data)
        {
            if (!availableBuildings.Contains(data))
            {
                availableBuildings.Add(data);
            }
        }
    }
}
