using UnityEngine;
using TopDownSurvival.Core;

namespace TopDownSurvival.World
{
    public class BossAI : EnemyAI
    {
        [Header("Boss Phased Mechanics")]
        [SerializeField] private float phase2HealthPercentage = 0.5f; // 50% HP
        [SerializeField] private float speedMultiplierPhase2 = 1.8f;
        [SerializeField] private float damageMultiplierPhase2 = 1.5f;
        [SerializeField] private Color phase2Color = new Color(1f, 0.2f, 0.2f); // Rage red color

        private bool isPhase2 = false;
        private SpriteRenderer spriteRenderer;

        protected override void Start()
        {
            base.Start();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void Update()
        {
            if (damageable.IsDead() || playerTransform == null) return;

            // Check Phase Transition
            if (!isPhase2)
            {
                float hpPercent = damageable.GetCurrentHealth() / damageable.GetMaxHealth();
                if (hpPercent <= phase2HealthPercentage)
                {
                    EnterPhase2();
                }
            }

            base.Update();
        }

        private void EnterPhase2()
        {
            isPhase2 = true;
            moveSpeed *= speedMultiplierPhase2;
            attackDamage *= damageMultiplierPhase2;

            // Visual feedback: Turn red permanently for Phase 2
            if (spriteRenderer != null)
            {
                spriteRenderer.color = phase2Color;
            }

            // Screen shake / Scale pop effect
            transform.localScale = transform.localScale * 1.3f;

            Debug.Log($"[BOSS] {gameObject.name} enters PHASE 2: RAGE! Speed and Damage increased!");
        }
    }
}
