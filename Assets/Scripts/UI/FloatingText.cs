using UnityEngine;
using UnityEngine.UI;

namespace TopDownSurvival.UI
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private Text textComponent;
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float lifetime = 0.8f;
        [SerializeField] private Vector3 randomScatter = new Vector3(0.5f, 0f, 0f);

        private float elapsed = 0f;
        private Color startColor;

        public void SetText(string text, Color color)
        {
            if (textComponent == null) textComponent = GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = text;
                textComponent.color = color;
                startColor = color;
            }
            
            // Randomize spawn position slightly
            transform.position += new Vector3(
                Random.Range(-randomScatter.x, randomScatter.x),
                Random.Range(-randomScatter.y, randomScatter.y),
                0f
            );
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Move upwards
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            // Fade out
            if (textComponent != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
                textComponent.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }
        }
    }
}
