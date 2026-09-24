using System.Collections.Generic;
using UnityEngine;

namespace TopDownSurvival.Core
{
    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "Survival/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Header("General Settings")]
        public string buildingName = "Campfire";
        public Color ghostColor = new Color(1f, 0.6f, 0.2f, 0.4f);
        public Color buildingColor = new Color(1f, 0.5f, 0f, 1f);
        public Vector2 size = new Vector2(1f, 1f);

        [Header("Required Materials")]
        public List<ItemStack> buildCost = new List<ItemStack>();
    }
}
