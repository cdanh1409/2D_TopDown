using UnityEngine;

namespace TopDownSurvival.Core
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string promptText = "Interact";
        [SerializeField] private string interactLogMessage = "Object interacted!";

        public string GetInteractionPrompt()
        {
            return promptText;
        }

        public void Interact(GameObject actor)
        {
            Debug.Log($"[Interaction] {actor.name} interacted with {gameObject.name}. Msg: {interactLogMessage}");
            
            // Simple visual feedback: change color slightly or bounce
            StartCoroutine(VisualFeedback());
        }

        private System.Collections.IEnumerator VisualFeedback()
        {
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.15f;
            yield return new WaitForSeconds(0.1f);
            transform.localScale = originalScale;
        }
    }
}
