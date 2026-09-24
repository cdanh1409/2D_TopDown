using UnityEngine;
using UnityEngine.UI;
using TopDownSurvival.Core;

namespace TopDownSurvival.UI
{
    public class CraftingUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RecipeData recipeToCraft;
        [SerializeField] private Button craftButton;
        [SerializeField] private Text recipeCostText;

        private Inventory playerInventory;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }

            if (craftButton != null)
            {
                craftButton.onClick.AddListener(OnCraftClicked);
            }

            UpdateRecipeUI();
        }

        private void Update()
        {
            // Update cost list live to reflect player's inventory changes
            UpdateRecipeUI();
        }

        private void UpdateRecipeUI()
        {
            if (recipeToCraft == null || playerInventory == null || recipeCostText == null) return;

            string info = $"Craft {recipeToCraft.result.item.displayName}\nCost:\n";
            Player.PlayerRoleManager role = playerInventory.GetComponent<Player.PlayerRoleManager>();
            bool isEngineer = role != null && role.IsEngineer();

            foreach (var ing in recipeToCraft.ingredients)
            {
                int baseCost = ing.count;
                // Apply half cost if Engineer (rounded up)
                int required = isEngineer ? Mathf.Max(1, Mathf.CeilToInt(baseCost * 0.5f)) : baseCost;
                int current = playerInventory.GetTotalCount(ing.item);

                info += $"- {ing.item.displayName}: {current}/{required}\n";
            }

            recipeCostText.text = info;

            // Optional: disable button if cannot craft
            if (craftButton != null)
            {
                craftButton.interactable = CraftingSystem.CanCraft(playerInventory, recipeToCraft, isEngineer);
            }
        }

        private void OnCraftClicked()
        {
            if (playerInventory == null || recipeToCraft == null) return;

            Debug.Log($"[UI Crafting] Button clicked to craft: {recipeToCraft.result.item.displayName}");
            CraftingSystem.Craft(playerInventory, recipeToCraft, playerInventory.gameObject);
        }
    }
}
