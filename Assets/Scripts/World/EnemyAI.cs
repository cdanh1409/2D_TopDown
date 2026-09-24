using System;
using System.Collections.Generic;
using UnityEngine;
using TopDownSurvival.Core;

namespace TopDownSurvival.World
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Damageable))]
    public class EnemyAI : MonoBehaviour
    {
        public enum AIState { Idle, Chase, Attack }

        [Header("AI Parameters")]
        [SerializeField] protected float moveSpeed = 2.5f;
        [SerializeField] protected float chaseRadius = 50f;
        [SerializeField] protected float attackRadius = 0.4f;
        [SerializeField] protected float attackDamage = 10f;
        [SerializeField] protected float attackRate = 1.5f; // seconds between attacks

        [Header("Loot & Rewards")]
        [SerializeField] private LootTable lootTable;
        [SerializeField] private int expReward = 50;

        /// <summary>Fired when this enemy dies. Args: enemyTag, expReward.</summary>
        public static event Action<string, int> OnEnemyKilled;

        protected Transform playerTransform;
        protected SurvivalStats playerStats;
        protected Rigidbody2D rb;
        protected Damageable damageable;
        protected Animator anim;

        protected AIState currentState = AIState.Idle;
        protected float nextAttackTime = 0f;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            damageable = GetComponent<Damageable>();
            anim = GetComponent<Animator>();

            // Rigidbody settings
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            // Find Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerStats = player.GetComponent<SurvivalStats>();
            }

            damageable.OnDeath += HandleDeath;
        }

        protected virtual void Update()
        {
            if (damageable.IsDead() || playerTransform == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Tính toán khoảng cách đánh thực tế vừa vặn theo kích thước (Scale) của quái
            float effectiveAttackRadius = attackRadius + (Mathf.Abs(transform.localScale.x) * 0.15f);

            // State Transitions
            if (distanceToPlayer <= effectiveAttackRadius)
            {
                currentState = AIState.Attack;
            }
            else if (distanceToPlayer <= chaseRadius)
            {
                currentState = AIState.Chase;
            }
            else
            {
                currentState = AIState.Idle;
            }

            // Update Animation states
            if (anim != null && anim.runtimeAnimatorController != null)
            {
                if (currentState == AIState.Attack)
                {
                    if (HasParameter(anim, "IsMove")) anim.SetBool("IsMove", false);
                    if (HasParameter(anim, "IsAttack")) anim.SetBool("IsAttack", true);
                }
                else if (currentState == AIState.Chase)
                {
                    if (HasParameter(anim, "IsAttack")) anim.SetBool("IsAttack", false);
                    if (HasParameter(anim, "IsMove")) anim.SetBool("IsMove", true);
                }
                else // Idle
                {
                    if (HasParameter(anim, "IsAttack")) anim.SetBool("IsAttack", false);
                    if (HasParameter(anim, "IsMove")) anim.SetBool("IsMove", false);
                }
            }

            // State Execution
            switch (currentState)
            {
                case AIState.Idle:
                    rb.linearVelocity = Vector2.zero;
                    break;
                case AIState.Chase:
                    ChasePlayer();
                    break;
                case AIState.Attack:
                    rb.linearVelocity = Vector2.zero;
                    TryAttackPlayer();
                    break;
            }
        }

        protected virtual void ChasePlayer()
        {
            Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // Flip sprite depending on direction chỉ khi thay đổi hướng x rõ rệt (tránh giật lag mỗi frame)
            if (Mathf.Abs(direction.x) > 0.1f)
            {
                float absX = Mathf.Abs(transform.localScale.x);
                float desiredScaleX = direction.x > 0 ? absX : -absX;
                if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(desiredScaleX))
                {
                    transform.localScale = new Vector3(desiredScaleX, transform.localScale.y, transform.localScale.z);
                }
            }
        }

        protected virtual void TryAttackPlayer()
        {
            if (Time.time >= nextAttackTime && playerStats != null)
            {
                playerStats.ApplyDamage(attackDamage);
                nextAttackTime = Time.time + attackRate;
                Debug.Log($"[Enemy Combat] {gameObject.name} bit the Player for {attackDamage} damage!");
            }
        }

        protected virtual void HandleDeath()
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            if (anim != null && anim.runtimeAnimatorController != null)
            {
                if (HasParameter(anim, "IsMove")) anim.SetBool("IsMove", false);
                if (HasParameter(anim, "IsAttack")) anim.SetBool("IsAttack", false);
                if (HasParameter(anim, "IsDead")) anim.SetBool("IsDead", true);
            }

            // --- Drop Loot ---
            if (lootTable != null)
            {
                List<ItemStack> drops = lootTable.GetDrops();
                foreach (ItemStack drop in drops)
                {
                    // Spawn WorldItem slightly scattered around death position
                    Vector2 scatter = UnityEngine.Random.insideUnitCircle * 0.6f;
                    Vector3 spawnPos = transform.position + new Vector3(scatter.x, scatter.y, 0f);

                    GameObject worldItemObj = new GameObject($"Drop_{drop.item.displayName}");
                    worldItemObj.transform.position = spawnPos;
                    worldItemObj.AddComponent<CircleCollider2D>().isTrigger = true;
                    WorldItem wi = worldItemObj.AddComponent<WorldItem>();
                    wi.Setup(drop.item, drop.count);

                    Debug.Log($"[Loot] Dropped {drop.count}x {drop.item.displayName}");
                }
            }

            // --- Broadcast kill event ---
            OnEnemyKilled?.Invoke(gameObject.tag, expReward);
            Debug.Log($"[Enemy AI] {gameObject.name} killed. EXP reward: {expReward}");
        }

        private bool HasParameter(Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName) return true;
            }
            return false;
        }
    }
}
