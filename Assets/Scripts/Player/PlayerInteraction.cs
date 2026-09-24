using UnityEngine;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;

namespace TopDownSurvival.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;

        private void Update()
        {
            // If E key is pressed this frame
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);
            
            IInteractable bestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (Collider2D col in colliders)
            {
                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    float dist = Vector2.Distance(transform.position, col.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        bestTarget = interactable;
                    }
                }
            }

            if (bestTarget != null)
            {
                Debug.Log($"Interacting with: {bestTarget.GetInteractionPrompt()}");
                bestTarget.Interact(gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw interaction range in editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
