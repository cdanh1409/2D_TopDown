using UnityEngine;
using TopDownSurvival.Player;

namespace TopDownSurvival.Core
{
    public static class CraftingSystem
    {
        // Engineer perk: crafting costs are halved (rounded up minimum 1)
        private static int GetEffectiveCost(int baseCost, bool isEngineer)
        {
            if (!isEngineer) return baseCost;
            return Mathf.Max(1, Mathf.CeilToInt(baseCost * 0.5f));
        }

        public static bool CanCraft(Inventory inventory, RecipeData recipe, bool engineerDiscount = false)
        {
            if (inventory == null || recipe == null) return false;

            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient.item == null || ingredient.count <= 0) continue;

                int required = GetEffectiveCost(ingredient.count, engineerDiscount);
                if (inventory.GetTotalCount(ingredient.item) < required)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Craft(Inventory inventory, RecipeData recipe, GameObject crafter = null)
        {
            // Check for Engineer role
            bool isEngineer = false;
            if (crafter != null)
            {
                PlayerRoleManager role = crafter.GetComponent<PlayerRoleManager>();
                if (role != null && role.IsEngineer())
                {
                    isEngineer = true;
                }
            }

            if (!CanCraft(inventory, recipe, isEngineer))
            {
                Debug.LogWarning($"[Crafting] Cannot craft {recipe.result.item.displayName}. Insufficient ingredients.");
                return false;
            }

            // Remove all ingredients (with Engineer discount)
            foreach (var ingredient in recipe.ingredients)
            {
                int cost = GetEffectiveCost(ingredient.count, isEngineer);
                inventory.RemoveItem(ingredient.item, cost);
                
                if (isEngineer && cost < ingredient.count)
                {
                    Debug.Log($"[Crafting - Engineer] Discount applied! Cost: {cost} (instead of {ingredient.count}) {ingredient.item.displayName}");
                }
            }

            // Add result to inventory
            inventory.AddItem(recipe.result.item, recipe.result.count);
            Debug.Log($"[Crafting] Successfully crafted: {recipe.result.count}x {recipe.result.item.displayName}");
            inventory.PrintInventory();
            return true;
        }
    }
}
