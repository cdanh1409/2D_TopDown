using UnityEngine;

namespace TopDownSurvival.World
{
    public class GroundElevationTrigger : MonoBehaviour
    {
        public enum TriggerTargetState
        {
            GoToHighGround,
            GoToLowGround,
            AutoByMovementDirection
        }

        [Header("Elevation Trigger Mode")]
        [SerializeField] private TriggerTargetState targetState = TriggerTargetState.AutoByMovementDirection;

        [Header("Sorting Order Settings")]
        [Tooltip("Order in Layer when Player is on Ground 2 (High ground)")]
        [SerializeField] private int highGroundOrder = 10;

        [Tooltip("Order in Layer when Player is on Ground 1 (Low ground)")]
        [SerializeField] private int lowGroundOrder = 1;

        [Header("Optional Cliff Wall Collider to Toggle")]
        [Tooltip("Optional: Cliff Wall Collider to disable when player is on High Ground")]
        [SerializeField] private Collider2D cliffWallCollider;

        private void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") || other.GetComponent<Player.PlayerController>() != null)
            {
                SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
                if (sr == null) sr = other.GetComponentInChildren<SpriteRenderer>();

                if (sr != null)
                {
                    bool setHigh = false;

                    if (targetState == TriggerTargetState.GoToHighGround)
                    {
                        setHigh = true;
                    }
                    else if (targetState == TriggerTargetState.GoToLowGround)
                    {
                        setHigh = false;
                    }
                    else
                    {
                        // Auto detect based on vertical movement direction
                        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            setHigh = rb.linearVelocity.y > 0 || other.transform.position.y >= transform.position.y;
                        }
                        else
                        {
                            setHigh = other.transform.position.y >= transform.position.y;
                        }
                    }

                    if (setHigh)
                    {
                        sr.sortingOrder = highGroundOrder;
                        if (cliffWallCollider != null) cliffWallCollider.enabled = false;
                        Debug.Log("[Elevation] Player stepped onto High Ground (Ground 2)");
                    }
                    else
                    {
                        sr.sortingOrder = lowGroundOrder;
                        if (cliffWallCollider != null) cliffWallCollider.enabled = true;
                        Debug.Log("[Elevation] Player stepped down to Low Ground (Ground 1)");
                    }
                }
            }
        }
    }
}
