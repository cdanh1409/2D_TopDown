using System;
using UnityEngine;

namespace TopDownSurvival.Core
{
    public class Damageable : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float currentHealth = 50f;

        [Header("UX Reference")]
        [SerializeField] private GameObject floatingTextPrefab;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private bool isDead = false;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth = Mathf.Max(0f, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            Debug.Log($"[Damage] {gameObject.name} took {damage} damage! HP: {currentHealth}/{maxHealth}");

            // --- Floating Damage Text ---
            if (floatingTextPrefab != null)
            {
                GameObject ftObj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                TopDownSurvival.UI.FloatingText ft = ftObj.GetComponent<TopDownSurvival.UI.FloatingText>();
                if (ft != null)
                {
                    // Different color for Player vs. Resources/Enemies
                    Color textColor = gameObject.CompareTag("Player") ? new Color(1f, 0.3f, 0.3f) : new Color(1f, 0.75f, 0.2f);
                    ft.SetText($"-{damage:F0}", textColor);
                }
            }

            // Visual feedback: briefly turn red
            StartCoroutine(FlashRed());

            if (currentHealth <= 0f)
            {
                Die();
            }
        }


        private System.Collections.IEnumerator FlashRed()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color originalColor = sr.color;
                sr.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                // Return to original color if not dead (or handled by Boss phase color)
                if (!isDead)
                {
                    sr.color = originalColor;
                }
            }
        }

        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Die()
        {
            isDead = true;
            OnDeath?.Invoke();
            Debug.Log($"[Damage] {gameObject.name} has died!");
            
            // Simple death behavior: destroy after brief delay
            Destroy(gameObject, 0.2f);
        }

        public bool IsDead() => isDead;
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
    }
}
