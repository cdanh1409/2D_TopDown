using System.Collections;
using UnityEngine;

namespace TopDownSurvival.Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Animation Direct Control")]
        [Tooltip("Nếu bật, Code sẽ trực tiếp chuyển Animation State (PlayerRunDown, Playeridle) dựa theo hướng đi.")]
        [SerializeField] private bool directStateControl = true;

        [Header("Animation State Names")]
        [SerializeField] private string idleStateName = "Playeridle";
        [SerializeField] private string runStateName = "PlayerMove";
        [SerializeField] private string attackStateName = "PlayerAttacker";

        [Tooltip("Nếu bật, Code sẽ cập nhật các tham số (IsRunDow, IsAttack, MoveX, MoveY, Speed) cho Animator Controller.")]
        [SerializeField] private bool useAnimatorParameters = true;

        [Header("Procedural Walk / Idle")]
        [SerializeField] private bool useProceduralScaleBounce = false;
        [SerializeField] private float walkBounceSpeed = 12f;
        [SerializeField] private float walkBounceAmount = 0.15f;
        [SerializeField] private float idlePulseSpeed = 3f;
        [SerializeField] private float idlePulseAmount = 0.03f;

        private PlayerController controller;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        
        private Vector3 originalScale;
        private float animationTimer;
        private string currentState = "";

        // Hashes for Animator parameters
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsRunDowHash = Animator.StringToHash("IsRunDow");
        private static readonly int IsAttackHash = Animator.StringToHash("IsAttack");

        private bool isAttacking = false;
        private float attackLockTimer = 0f;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            originalScale = transform.localScale;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        private void Start()
        {
            ResetAttackBoolParameters();
        }

        private void Update()
        {
            if (attackLockTimer > 0f)
            {
                attackLockTimer -= Time.deltaTime;
                if (attackLockTimer <= 0f)
                {
                    isAttacking = false;
                    currentState = "";
                    ResetAttackBoolParameters();
                }
            }
            else if (isAttacking)
            {
                // Backup check nếu animation đã kết thúc
                if (animator != null && !IsPlayingAttackState())
                {
                    isAttacking = false;
                    currentState = "";
                    ResetAttackBoolParameters();
                }
            }

            Vector2 input = controller.GetMovementInput();
            bool isMoving = input.sqrMagnitude > 0.0001f;

            // 1. Update Unity Animator Parameters
            if (useAnimatorParameters && animator != null && animator.runtimeAnimatorController != null)
            {
                if (HasParameter(animator, IsRunDowHash)) animator.SetBool(IsRunDowHash, isMoving && !isAttacking);
                if (HasParameter(animator, MoveXHash)) animator.SetFloat(MoveXHash, input.x);
                if (HasParameter(animator, MoveYHash)) animator.SetFloat(MoveYHash, input.y);
                if (HasParameter(animator, IsMovingHash)) animator.SetBool(IsMovingHash, isMoving);
                if (HasParameter(animator, SpeedHash)) animator.SetFloat(SpeedHash, input.magnitude);
            }

            // 2. Tự động phát animation mượt bằng C# Code (không bị phụ thuộc vào mũi tên Unity)
            if (directStateControl && animator != null && !isAttacking)
            {
                string targetState = isMoving ? runStateName : idleStateName;
                ChangeAnimationState(targetState);
            }

            // 3. Optional Procedural Animation (Squash & Stretch scale)
            if (useProceduralScaleBounce)
            {
                HandleProceduralBounce(isMoving);
            }
        }

        public void PlayAttack(string customAttackState = "PlayerAttacker", float duration = 0.5f)
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();

            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("[PlayerAnimator] Animator hoặc AnimatorController chưa được gán trên Player!");
                return;
            }

            // Tránh spam chém liên tục nếu đang chém
            if (isAttacking && attackLockTimer > 0.05f) return;

            isAttacking = true;
            attackLockTimer = duration;

            bool playedAny = false;

            // 1. Kích hoạt mọi Parameter chứa chữ 'attack' (Bool hoặc Trigger: IsAttack, Attack...)
            foreach (var param in animator.parameters)
            {
                string pName = param.name.ToLower();
                if (pName.Contains("attack"))
                {
                    if (param.type == AnimatorControllerParameterType.Trigger)
                    {
                        animator.SetTrigger(param.name);
                        playedAny = true;
                    }
                    else if (param.type == AnimatorControllerParameterType.Bool)
                    {
                        animator.SetBool(param.name, true);
                        playedAny = true;
                    }
                }
            }

            // 2. Thử Play State trực tiếp bằng các tên phổ biến
            string[] possibleStates = new string[] { customAttackState, attackStateName, "PlayerAttacker", "PlayerAttack", "Attack", "Slash" };
            foreach (string state in possibleStates)
            {
                if (string.IsNullOrEmpty(state)) continue;
                int hash = Animator.StringToHash(state);
                if (animator.HasState(0, hash))
                {
                    animator.Play(hash, 0, 0f);
                    currentState = state;
                    playedAny = true;
                    break;
                }
            }

            // Lấy thời lượng thực tế của clip animation nếu có
            if (animator != null)
            {
                animator.Update(0f);
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.length > 0.05f)
                {
                    attackLockTimer = stateInfo.length;
                }
            }

            // Parameter IsAttack sẽ tự động được Reset về false khi attackLockTimer đếm xong trong Update()

            // 3. Nếu vẫn chưa phát được, báo rõ nguyên nhân
            if (!playedAny)
            {
                Debug.LogWarning($"[PlayerAnimator] Không tìm thấy State hoặc Parameter nào tên '{customAttackState}' / 'IsAttack' trong Animator Controller!");
            }
        }

        private IEnumerator ClearAttackBoolsNextFrame()
        {
            yield return null;
            ResetAttackBoolParameters();
        }

        private void ResetAttackBoolParameters()
        {
            if (animator == null || animator.runtimeAnimatorController == null) return;
            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Bool && param.name.ToLower().Contains("attack"))
                {
                    animator.SetBool(param.name, false);
                }
            }
        }

        private bool IsPlayingAttackState()
        {
            if (animator == null) return false;

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName("PlayerAttacker") || info.IsName("PlayerAttack") || info.IsName("Attack") || info.IsName("Slash");
        }

        private void HandleDirectAnimation(Vector2 input, bool isMoving)
        {
            string targetState = isMoving ? runStateName : idleStateName;
            ChangeAnimationState(targetState);
        }

        private void LateUpdate()
        {
            if (controller == null || spriteRenderer == null) return;

            Vector2 input = controller.GetMovementInput();
            if (input.sqrMagnitude < 0.0001f) return;

            if (Mathf.Abs(input.x) > 0.01f)
            {
                float yAngle = input.x < 0 ? 180f : 0f;
                spriteRenderer.transform.localRotation = Quaternion.Euler(0, yAngle, 0);
            }
        }

        private void ChangeAnimationState(string newState)
        {
            if (string.IsNullOrEmpty(newState) || animator == null || animator.runtimeAnimatorController == null)
                return;

            string validStateName = GetValidStateName(newState);
            if (string.IsNullOrEmpty(validStateName))
                return;

            if (currentState == validStateName)
                return;

            animator.Play(validStateName);
            currentState = validStateName;
        }

        private string GetValidStateName(string stateName)
        {
            if (animator == null) return null;

            int hash = Animator.StringToHash(stateName);
            if (animator.HasState(0, hash))
                return stateName;

            string altName = stateName;
            if (stateName.Equals("PlayerIdle", System.StringComparison.OrdinalIgnoreCase) || stateName.Equals("Playeridle", System.StringComparison.OrdinalIgnoreCase))
            {
                altName = stateName.Equals("Playeridle") ? "PlayerIdle" : "Playeridle";
            }

            int altHash = Animator.StringToHash(altName);
            if (altName != stateName && animator.HasState(0, altHash))
                return altName;

            if (stateName.StartsWith("PlayerRun", System.StringComparison.OrdinalIgnoreCase)
                || stateName.Equals("PlayerMove", System.StringComparison.OrdinalIgnoreCase))
            {
                if (animator.HasState(0, Animator.StringToHash("PlayerMove")))
                    return "PlayerMove";
                if (animator.HasState(0, Animator.StringToHash("PlayerRunDown")))
                    return "PlayerRunDown";
                if (animator.HasState(0, Animator.StringToHash("PlayerRun")))
                    return "PlayerRun";
            }

            return null;
        }

        private bool HasParameter(Animator anim, int nameHash)
        {
            if (anim == null || anim.runtimeAnimatorController == null) return false;
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.nameHash == nameHash) return true;
            }
            return false;
        }

        private void HandleProceduralBounce(bool isMoving)
        {
            animationTimer += Time.deltaTime;

            if (isMoving)
            {
                float bounce = Mathf.Sin(animationTimer * walkBounceSpeed);
                float scaleX = originalScale.x + (bounce * walkBounceAmount);
                float scaleY = originalScale.y - (bounce * walkBounceAmount);
                transform.localScale = new Vector3(scaleX, scaleY, originalScale.z);
            }
            else
            {
                float breath = Mathf.Sin(animationTimer * idlePulseSpeed);
                float scaleX = originalScale.x - (breath * idlePulseAmount);
                float scaleY = originalScale.y + (breath * idlePulseAmount);
                transform.localScale = new Vector3(scaleX, scaleY, originalScale.z);
            }
        }
    }
}
