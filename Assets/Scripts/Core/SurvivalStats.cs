using System;
using UnityEngine;

namespace TopDownSurvival.Core
{
    public class SurvivalStats : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;

        [Header("Hunger Settings")]
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float currentHunger = 100f;
        [SerializeField] private float hungerDecreaseRate = 0.5f; // hunger units lost per second

        [Header("Starvation Settings")]
        [SerializeField] private float starvationDamage = 2f;      // damage taken per starvation tick
        [SerializeField] private float starvationTickInterval = 3f; // seconds between damage ticks

        private float starvationTimer = 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action<float, float> OnHungerChanged;
        public event Action OnDeath;

        private void Start()
        {
            currentHealth = maxHealth;
            currentHunger = maxHunger;
        }

        private void Update()
        {
            // 1. Decrease hunger over time
            if (currentHunger > 0f)
            {
                ModifyHunger(-hungerDecreaseRate * Time.deltaTime);
            }

            // 2. Starvation logic
            if (currentHunger <= 0f)
            {
                starvationTimer += Time.deltaTime;
                if (starvationTimer >= starvationTickInterval)
                {
                    starvationTimer = 0f;
                    ApplyDamage(starvationDamage);
                    Debug.Log($"[Survival] Starving! Health: {currentHealth}/{maxHealth}");
                }
            }
            else
            {
                starvationTimer = 0f;
            }
        }

        public void ApplyDamage(float damage)
        {
            if (currentHealth <= 0f) return;

            currentHealth = Mathf.Max(0f, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                OnDeath?.Invoke();
                Debug.Log($"[Survival] {gameObject.name} has died of starvation/damage!");
            }
        }

        public void ModifyHealth(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log($"[Survival] Health modified by {amount}. Current: {currentHealth}/{maxHealth}");
        }

        public void ModifyHunger(float amount)
        {
            currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
            OnHungerChanged?.Invoke(currentHunger, maxHunger);
            
            // Log hunger changes occasionally
            if (Time.frameCount % 300 == 0)
            {
                Debug.Log($"[Survival] Hunger: {currentHunger:F1}/{maxHunger}");
            }
        }

        public float GetCurrentHealth() => currentHealth;
        public float GetCurrentHunger() => currentHunger;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float MaxHunger => maxHunger;
        public float CurrentHunger => currentHunger;

        // Thirst fallback properties (since Thirst isn't implemented in stats yet)
        public float MaxThirst => 100f;
        public float CurrentThirst => 100f;
    }
}
