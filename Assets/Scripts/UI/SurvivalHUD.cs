using UnityEngine;
using UnityEngine.UI;

namespace TopDownSurvival.UI
{
    public class SurvivalHUD : MonoBehaviour
    {
        [Header("Survival Stats Reference")]
        [SerializeField] private Core.SurvivalStats stats;

        [Header("UI Sliders")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider thirstSlider;

        private void Start()
        {
            if (stats == null)
            {
                stats = Object.FindAnyObjectByType<Core.SurvivalStats>();
            }
        }

        private void Update()
        {
            if (stats == null) return;

            if (healthSlider != null)
            {
                healthSlider.maxValue = stats.MaxHealth;
                healthSlider.value = stats.CurrentHealth;
            }

            if (hungerSlider != null)
            {
                hungerSlider.maxValue = stats.MaxHunger;
                hungerSlider.value = stats.CurrentHunger;
            }

            if (thirstSlider != null)
            {
                thirstSlider.maxValue = stats.MaxThirst;
                thirstSlider.value = stats.CurrentThirst;
            }
        }
    }
}
