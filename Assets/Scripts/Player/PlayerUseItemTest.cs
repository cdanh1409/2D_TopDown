using UnityEngine;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;

namespace TopDownSurvival.Player
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(SurvivalStats))]
    public class PlayerUseItemTest : MonoBehaviour
    {
        private Inventory inventory;
        private SurvivalStats stats;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            stats = GetComponent<SurvivalStats>();
        }

        private void Update()
        {
            // Listen for key F to eat/consume food
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                TryConsumeFood();
            }
        }

        private void TryConsumeFood()
        {
            ItemStack foodStack = FindFirstConsumable();

            if (foodStack != null && foodStack.item != null)
            {
                ItemData foodItem = foodStack.item;
                
                // 1. Restore stats
                if (foodItem.hungerRestoreValue > 0f)
                {
                    stats.ModifyHunger(foodItem.hungerRestoreValue);
                }
                if (foodItem.healthRestoreValue > 0f)
                {
                    stats.ModifyHealth(foodItem.healthRestoreValue);
                }

                // 2. Consume item from inventory
                inventory.RemoveItem(foodItem, 1);
                
                Debug.Log($"[Survival] Consumed 1x {foodItem.displayName}! Restored: +{foodItem.hungerRestoreValue} Hunger, +{foodItem.healthRestoreValue} Health.");
                inventory.PrintInventory();
            }
            else
            {
                Debug.LogWarning("[Survival] No consumable food items in Inventory! Harvest a Berry Bush to get some berries.");
            }
        }

        private ItemStack FindFirstConsumable()
        {
            foreach (var slot in inventory.GetSlots())
            {
                if (slot.item != null && slot.item.isConsumable && slot.count > 0)
                {
                    return slot;
                }
            }
            return null;
        }
    }
}
