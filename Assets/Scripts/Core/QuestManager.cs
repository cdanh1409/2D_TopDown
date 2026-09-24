using System;
using System.Collections.Generic;
using UnityEngine;
using TopDownSurvival.World;

namespace TopDownSurvival.Core
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Quests Setup")]
        [SerializeField] private List<QuestData> allQuests = new List<QuestData>();
        
        private int currentQuestIndex = 0;
        private int currentProgress = 0;
        private int killCount = 0; // tracks kills for Kill-type quests
        private bool isQuestSystemActive = false;

        public event Action OnQuestUpdated;

        private Player.PlayerProgression playerProgression;
        private Inventory playerInventory;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerProgression = player.GetComponent<Player.PlayerProgression>();
                playerInventory = player.GetComponent<Inventory>();
                
                if (playerInventory != null)
                {
                    playerInventory.OnInventoryChanged += OnInventoryModified;
                }
            }

            if (allQuests.Count > 0)
            {
                isQuestSystemActive = true;
                EvaluateQuestProgress();
            }

            EnemyAI.OnEnemyKilled += OnEnemyKilledHandler;
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged -= OnInventoryModified;
            }
            EnemyAI.OnEnemyKilled -= OnEnemyKilledHandler;
        }

        private void OnEnemyKilledHandler(string enemyTag, int expFromKill)
        {
            if (!isQuestSystemActive || currentQuestIndex >= allQuests.Count) return;
            QuestData active = allQuests[currentQuestIndex];
            if (active.type != QuestType.Kill) return;
            if (active.enemyTag != enemyTag) return;

            killCount++;
            currentProgress = killCount;
            Debug.Log($"[Quest Kill] {enemyTag} killed. Progress: {killCount}/{active.requiredAmount}");

            if (killCount >= active.requiredAmount)
                CompleteQuest();
            else
                OnQuestUpdated?.Invoke();
        }

        private void OnInventoryModified()
        {
            EvaluateQuestProgress();
        }

        private void EvaluateQuestProgress()
        {
            if (!isQuestSystemActive || currentQuestIndex >= allQuests.Count) return;

            QuestData currentQuest = allQuests[currentQuestIndex];
            if (currentQuest == null) return;

            // Kill quests are evaluated via OnEnemyKilledHandler, not inventory
            if (currentQuest.type == QuestType.Kill)
            {
                currentProgress = killCount;
                OnQuestUpdated?.Invoke();
                return;
            }

            if (playerInventory == null) return;
            int count = playerInventory.GetTotalCount(currentQuest.targetItem);
            currentProgress = count;

            if (currentProgress >= currentQuest.requiredAmount)
            {
                CompleteQuest();
            }
            else
            {
                OnQuestUpdated?.Invoke();
            }
        }

        private void CompleteQuest()
        {
            if (currentQuestIndex >= allQuests.Count) return;

            QuestData finished = allQuests[currentQuestIndex];
            Debug.Log($"[Quest] Completed: {finished.title}! Awarding {finished.rewardExp} EXP.");

            // Consume required materials from inventory if it's a gather quest
            if (finished.type == QuestType.Gather && playerInventory != null)
            {
                playerInventory.RemoveItem(finished.targetItem, finished.requiredAmount);
            }

            // Award EXP
            if (playerProgression != null)
            {
                playerProgression.AddExp(finished.rewardExp);
            }

            // Advance to next quest — reset killCount
            currentQuestIndex++;
            currentProgress = 0;
            killCount = 0;

            if (currentQuestIndex < allQuests.Count)
            {
                Debug.Log($"[Quest] Unlocked next quest: {allQuests[currentQuestIndex].title}");
                EvaluateQuestProgress();
            }
            else
            {
                isQuestSystemActive = false;
                Debug.Log("[Quest] Congratulations! All survival quests completed.");
                OnQuestUpdated?.Invoke();
            }
        }

        public string GetQuestDescription()
        {
            if (!isQuestSystemActive || currentQuestIndex >= allQuests.Count)
            {
                return "All Quests Completed!";
            }

            QuestData active = allQuests[currentQuestIndex];
            return $"{active.title}\n{active.description}\nProgress: {currentProgress} / {active.requiredAmount}";
        }

        public void AddQuest(QuestData q)
        {
            if (!allQuests.Contains(q))
            {
                allQuests.Add(q);
                isQuestSystemActive = true;
                EvaluateQuestProgress();
            }
        }
    }
}
