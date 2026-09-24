using UnityEngine;

namespace TopDownSurvival.Core
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Survival/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemId;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Stack Settings")]
        public bool isStackable = true;
        public int maxStackSize = 99;

        [Header("Resource Settings")]
        public bool isResource = true;

        [Header("Consumable Settings")]
        public bool isConsumable = false;
        public float hungerRestoreValue = 0f;
        public float healthRestoreValue = 0f;
    }
}
