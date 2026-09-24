using System;
using UnityEngine;
using TopDownSurvival.Core;
using TopDownSurvival.World;

namespace TopDownSurvival.Player
{
    public class PlayerProgression : MonoBehaviour
    {
        [Header("Progression Stats")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentExp = 0;
        [SerializeField] private int expNeeded = 100;

        public event Action<int, int, int> OnProgressionChanged; // level, exp, neededExp
        public event Action<int> OnLevelUp;

        private PlayerRoleManager roleManager;

        private void Awake()
        {
            roleManager = GetComponent<PlayerRoleManager>();
        }

        private void Start()
        {
            TriggerProgressionChanged();
            EnemyAI.OnEnemyKilled += OnEnemyKilledHandler;
        }

        private void OnDestroy()
        {
            EnemyAI.OnEnemyKilled -= OnEnemyKilledHandler;
        }

        private void OnEnemyKilledHandler(string enemyTag, int expReward)
        {
            AddExp(expReward);
        }

        public void AddExp(int amount)
        {
            if (amount <= 0) return;

            currentExp += amount;
            Debug.Log($"[Progression] +{amount} EXP gained! ({currentExp}/{expNeeded})");

            while (currentExp >= expNeeded)
            {
                LevelUp();
            }

            TriggerProgressionChanged();
        }

        private void LevelUp()
        {
            currentExp -= expNeeded;
            currentLevel++;
            expNeeded = Mathf.RoundToInt(expNeeded * 1.5f); // Scale next level requirement

            Debug.Log($"[Progression] LEVEL UP! Reached Level {currentLevel}");

            ApplyLevelUpBonuses();

            OnLevelUp?.Invoke(currentLevel);
        }

        private void ApplyLevelUpBonuses()
        {
            if (roleManager == null) return;

            // Apply specific role upgrades on level up
            switch (roleManager.GetCurrentRole())
            {
                case Core.RoleType.Warrior:
                    PlayerAttacker combat = GetComponent<PlayerAttacker>();
                    if (combat != null)
                    {
                        combat.AttackDamage += 5f; // Warrior gains +5 attack damage per level
                        Debug.Log($"[Warrior LevelUp] Damage increased to {combat.AttackDamage}!");
                    }
                    break;

                case Core.RoleType.Ranger:
                    PlayerController controller = GetComponent<PlayerController>();
                    if (controller != null)
                    {
                        controller.MoveSpeed += 0.5f; // Ranger gains +0.5 movement speed per level
                        Debug.Log($"[Ranger LevelUp] Movement Speed increased to {controller.MoveSpeed}!");
                    }
                    break;

                case Core.RoleType.Engineer:
                    SurvivalStats stats = GetComponent<SurvivalStats>();
                    if (stats != null)
                    {
                        stats.ModifyHealth(20f); // Engineer gains health boost
                        Debug.Log("[Engineer LevelUp] Restored +20 Health!");
                    }
                    break;
            }
        }

        public void TriggerProgressionChanged()
        {
            OnProgressionChanged?.Invoke(currentLevel, currentExp, expNeeded);
        }

        public int GetLevel() => currentLevel;
        public int GetExp() => currentExp;
        public int GetExpNeeded() => expNeeded;
    }
}
