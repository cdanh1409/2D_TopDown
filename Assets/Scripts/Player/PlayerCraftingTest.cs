using UnityEngine;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;

namespace TopDownSurvival.Player
{
    [RequireComponent(typeof(Inventory))]
    public class PlayerCraftingTest : MonoBehaviour
    {
        [Header("Test Recipe")]
        [SerializeField] private RecipeData testRecipe;

        private Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
            {
                if (testRecipe != null)
                {
                    Debug.Log($"[Crafting Test] Attempting to craft {testRecipe.result.item.displayName} (Cost: {testRecipe.ingredients[0].count}x {testRecipe.ingredients[0].item.displayName})");
                    CraftingSystem.Craft(inventory, testRecipe, gameObject);
                }
                else
                {
                    Debug.LogWarning("[Crafting Test] No test recipe assigned to PlayerCraftingTest component!");
                }
            }
        }
    }
}
