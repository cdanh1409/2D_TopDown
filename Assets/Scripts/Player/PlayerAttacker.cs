using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;

namespace TopDownSurvival.Player
{
    public class PlayerAttacker : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private string attackStateName = "PlayerAttacker";
        [SerializeField] private string attackTriggerName = "Attack";

        [Header("Attack Settings")]
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private float attackRadius = 2.2f;
        [SerializeField] private float knockbackForce = 3.5f;
        [SerializeField] private LayerMask enemyLayer = -1; // -1 means Everything

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            bool attackPressed = false;

            // 1. Check New Input System (Space, J, Left Click)
            if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.jKey.wasPressedThisFrame))
            {
                attackPressed = true;
            }
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Only attack on click if not over UI element
                if (UnityEngine.EventSystems.EventSystem.current == null || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    attackPressed = true;
                }
            }

            if (attackPressed)
            {
                Attack();
            }
        }

        public void Attack()
        {
            // 1. Play Attack Animation
            PlayerAnimator playerAnimator = GetComponent<PlayerAnimator>();
            if (playerAnimator == null) playerAnimator = GetComponentInChildren<PlayerAnimator>();

            if (playerAnimator != null)
            {
                playerAnimator.PlayAttack(attackStateName);
            }
            else if (animator != null && animator.runtimeAnimatorController != null)
            {
                bool triggerSet = false;
                foreach (var param in animator.parameters)
                {
                    if (param.name == attackTriggerName && param.type == AnimatorControllerParameterType.Trigger)
                    {
                        animator.SetTrigger(attackTriggerName);
                        triggerSet = true;
                        break;
                    }
                }

                if (!triggerSet)
                {
                    animator.Play(attackStateName, 0, 0f);
                }
            }

            // 2. Perform Hit Detection (Ignore layer filter if set to 0/Nothing)
            LayerMask maskToUse = (enemyLayer.value == 0) ? (LayerMask)(-1) : enemyLayer;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, maskToUse);
            
            int hitCount = 0;
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                Damageable damageable = hit.GetComponent<Damageable>();
                if (damageable == null) damageable = hit.GetComponentInParent<Damageable>();

                if (damageable != null && !damageable.IsDead())
                {
                    damageable.TakeDamage(attackDamage);
                    hitCount++;

                    // Knockback enemy away from player
                    Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                    if (enemyRb == null) enemyRb = hit.GetComponentInParent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                        if (knockbackDir == Vector2.zero) knockbackDir = Vector2.right;
                        enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                    }

                    Debug.Log($"[PlayerAttack] Hit {hit.name} for {attackDamage} damage!");
                }
            }

            if (hitCount == 0)
            {
                Debug.Log("[PlayerAttack] Attacked, but no targets were in range.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }

        public float AttackDamage
        {
            get => attackDamage;
            set => attackDamage = value;
        }

        public float AttackRadius
        {
            get => attackRadius;
            set => attackRadius = value;
        }
    }
}
