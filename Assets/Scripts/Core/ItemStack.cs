using System;

namespace TopDownSurvival.Core
{
    [Serializable]
    public class ItemStack
    {
        public ItemData item;
        public int count;

        public ItemStack(ItemData item, int count)
        {
            this.item = item;
            this.count = count;
        }

        public bool IsFull()
        {
            if (item == null) return false;
            return !item.isStackable || count >= item.maxStackSize;
        }

        public int GetRemainingSpace()
        {
            if (item == null) return 0;
            if (!item.isStackable) return 0;
            return item.maxStackSize - count;
        }
    }
}
