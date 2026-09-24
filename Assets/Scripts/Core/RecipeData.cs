using System.Collections.Generic;
using UnityEngine;

namespace TopDownSurvival.Core
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Survival/Recipe Data")]
    public class RecipeData : ScriptableObject
    {
        [Header("Recipe Configuration")]
        public List<ItemStack> ingredients;
        public ItemStack result;
    }
}
