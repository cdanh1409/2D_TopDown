using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownSurvival.Core
{
    public class Inventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        [SerializeField] private int slotsCount = 20;
        [SerializeField] private List<ItemStack> slots = new List<ItemStack>();

        public event Action OnInventoryChanged;

        private void Awake()
        {
            // Initialize empty slots
            for (int i = slots.Count; i < slotsCount; i++)
            {
                slots.Add(new ItemStack(null, 0));
            }
        }

        public bool AddItem(ItemData item, int amount)
        {
            if (item == null || amount <= 0) return false;

            int remainingAmount = amount;

            // 1. Try to add to existing stacks if stackable
            if (item.isStackable)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].item == item && !slots[i].IsFull())
                    {
                        int space = slots[i].GetRemainingSpace();
                        int toAdd = Mathf.Min(remainingAmount, space);
                        slots[i].count += toAdd;
                        remainingAmount -= toAdd;

                        if (remainingAmount <= 0)
                        {
                            OnInventoryChanged?.Invoke();
                            return true;
                        }
                    }
                }
            }

            // 2. Try to add to empty slots
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item == null)
                {
                    int maxStack = item.isStackable ? item.maxStackSize : 1;
                    int toAdd = Mathf.Min(remainingAmount, maxStack);
                    slots[i].item = item;
                    slots[i].count = toAdd;
                    remainingAmount -= toAdd;

                    if (remainingAmount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }

            OnInventoryChanged?.Invoke();
            // If remainingAmount > 0, inventory was full, but we still added some
            return remainingAmount < amount;
        }

        public bool RemoveItem(ItemData item, int amount)
        {
            if (item == null || amount <= 0) return false;

            // Check if we have enough items first
            if (GetTotalCount(item) < amount) return false;

            int remainingToRemove = amount;

            for (int i = slots.Count - 1; i >= 0; i--)
            {
                if (slots[i].item == item)
                {
                    if (slots[i].count > remainingToRemove)
                    {
                        slots[i].count -= remainingToRemove;
                        remainingToRemove = 0;
                        break;
                    }
                    else
                    {
                        remainingToRemove -= slots[i].count;
                        slots[i].item = null;
                        slots[i].count = 0;
                    }
                }
            }

            OnInventoryChanged?.Invoke();
            return remainingToRemove == 0;
        }

        public int GetTotalCount(ItemData item)
        {
            int total = 0;
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    total += slot.count;
                }
            }
            return total;
        }

        public List<ItemStack> GetSlots()
        {
            return slots;
        }

        public void PrintInventory()
        {
            string log = $"--- Inventory of {gameObject.name} --- \n";
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item != null)
                {
                    log += $"Slot {i}: {slots[i].item.displayName} x{slots[i].count}\n";
                }
            }
            Debug.Log(log);
        }
    }
}
