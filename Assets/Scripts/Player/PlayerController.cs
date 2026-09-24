using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownSurvival.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D rb;
        private Vector2 movementInput;
        private Animator animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            
            // Configure Rigidbody2D for top-down 2D physics
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            // Gather input using the new Input System
            Vector2 input = Vector2.zero;
            
            if (Keyboard.current != null)
            {
                float moveX = 0f;
                float moveY = 0f;

                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;

                input = new Vector2(moveX, moveY);
            }
            
            movementInput = input.normalized;

            // Tự động gửi tham số Animator nếu không có PlayerAnimator
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                bool isMoving = movementInput.sqrMagnitude > 0.0001f;
                SetAnimBoolIfExists("IsRunDow", isMoving);
                SetAnimBoolIfExists("IsRun", isMoving);
                SetAnimBoolIfExists("IsMoving", isMoving);
                SetAnimFloatIfExists("MoveX", movementInput.x);
                SetAnimFloatIfExists("MoveY", movementInput.y);
            }
        }

        private void SetAnimBoolIfExists(string paramName, bool val)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(paramName, val);
                    return;
                }
            }
        }

        private void SetAnimFloatIfExists(string paramName, float val)
        {
            foreach (var param in animator.parameters)
            {
                if (param.name == paramName && param.type == AnimatorControllerParameterType.Float)
                {
                    animator.SetFloat(paramName, val);
                    return;
                }
            }
        }

        private void FixedUpdate()
        {
            // Apply movement using Rigidbody2D physics
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }

        public Vector2 GetMovementInput()
        {
            return movementInput;
        }

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }
    }
}
