using UnityEngine;
using TopDownSurvival.Core;

namespace TopDownSurvival.World
{
    [RequireComponent(typeof(Inventory))]
    public class StorageChest : MonoBehaviour, IInteractable
    {
        [SerializeField] private string chestName = "Wooden Chest";

        private Inventory chestInventory;

        private void Awake()
        {
            chestInventory = GetComponent<Inventory>();
        }

        public string GetInteractionPrompt()
        {
            return $"Open {chestName}";
        }

        public void Interact(GameObject actor)
        {
            Debug.Log($"[Storage] {actor.name} opened {chestName}.");
            
            // Get player's inventory
            Inventory playerInventory = actor.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                // Simple demo interaction: move 1 item of any resource from player to chest
                bool transferred = false;
                foreach (var slot in playerInventory.GetSlots())
                {
                    if (slot.item != null && slot.count > 0)
                    {
                        ItemData itemToTransfer = slot.item;
                        // Transfer 1 unit
                        if (playerInventory.RemoveItem(itemToTransfer, 1))
                        {
                            chestInventory.AddItem(itemToTransfer, 1);
                            Debug.Log($"[Storage] Transferred 1x {itemToTransfer.displayName} from {actor.name} to {chestName}.");
                            transferred = true;
                            break;
                        }
                    }
                }

                if (!transferred)
                {
                    Debug.Log("[Storage] Player inventory is empty. Nothing to deposit!");
                }
            }

            // Print chest inventory
            chestInventory.PrintInventory();
        }
    }
}
