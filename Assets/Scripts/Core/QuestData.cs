using UnityEngine;

namespace TopDownSurvival.Core
{
    public enum QuestType
    {
        Gather,
        Craft,
        Kill
    }

    [CreateAssetMenu(fileName = "NewQuestData", menuName = "Survival/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [Header("Quest Info")]
        public string questId = "gather_wood";
        public string title = "Gather Wood";
        public string description = "Harvest wood from trees in the forest.";

        [Header("Objective")]
        public QuestType type = QuestType.Gather;
        public ItemData targetItem;       // Used for Gather / Craft
        public string enemyTag = "Enemy"; // Used for Kill
        public int requiredAmount = 3;

        [Header("Rewards")]
        public int rewardExp = 100;
    }
}
