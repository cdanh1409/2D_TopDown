using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownSurvival.Core
{
    [Serializable]
    public class LootEntry
    {
        public ItemData item;
        [Range(0f, 1f)] public float dropChance = 0.5f;
        public int minAmount = 1;
        public int maxAmount = 2;
    }

    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Survival/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [Header("Loot Entries")]
        public List<LootEntry> entries = new List<LootEntry>();

        /// <summary>
        /// Rolls each entry independently and returns the resulting drops.
        /// </summary>
        public List<ItemStack> GetDrops()
        {
            List<ItemStack> drops = new List<ItemStack>();

            foreach (LootEntry entry in entries)
            {
                if (entry.item == null) continue;

                float roll = UnityEngine.Random.value;
                if (roll <= entry.dropChance)
                {
                    int amount = UnityEngine.Random.Range(entry.minAmount, entry.maxAmount + 1);
                    drops.Add(new ItemStack(entry.item, amount));
                }
            }

            return drops;
        }
    }
}
