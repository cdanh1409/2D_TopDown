using UnityEngine;
using TopDownSurvival.Core;

namespace TopDownSurvival.World
{
    [RequireComponent(typeof(Collider2D))]
    public class WorldItem : MonoBehaviour
    {
        [Header("Item Configuration")]
        [SerializeField] private ItemData itemData;
        [SerializeField] private int amount = 1;

        private void Start()
        {
            // Ensure collider is set to Trigger
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        public void Setup(ItemData data, int count)
        {
            itemData = data;
            amount = count;

            // Configure SpriteRenderer if it exists
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && data != null)
            {
                sr.sprite = data.icon;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Try to find inventory on the entering object (Player)
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                if (inventory.AddItem(itemData, amount))
                {
                    Debug.Log($"[Inventory] Picked up: {amount}x {itemData.displayName}");
                    inventory.PrintInventory();
                    Destroy(gameObject);
                }
            }
        }
    }
}
