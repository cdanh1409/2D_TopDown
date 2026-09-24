using UnityEngine;
using UnityEngine.UI;
using TopDownSurvival.Core;
using TopDownSurvival.Player;

namespace TopDownSurvival.UI
{
    public class QuestHUD : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private Text questText;
        [SerializeField] private Text levelText;
        [SerializeField] private Slider expSlider;

        private PlayerProgression playerProg;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerProg = player.GetComponent<PlayerProgression>();
                if (playerProg != null)
                {
                    playerProg.OnProgressionChanged += UpdateProgressionUI;
                }
            }

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestUpdated += UpdateQuestUI;
            }

            UpdateQuestUI();
            
            if (playerProg != null)
            {
                playerProg.TriggerProgressionChanged();
            }
        }

        private void OnDestroy()
        {
            if (playerProg != null)
            {
                playerProg.OnProgressionChanged -= UpdateProgressionUI;
            }

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestUpdated -= UpdateQuestUI;
            }
        }

        private void UpdateQuestUI()
        {
            if (questText != null && QuestManager.Instance != null)
            {
                questText.text = QuestManager.Instance.GetQuestDescription();
            }
        }

        private void UpdateProgressionUI(int level, int exp, int neededExp)
        {
            if (levelText != null)
            {
                levelText.text = $"Lv. {level}";
            }

            if (expSlider != null)
            {
                expSlider.maxValue = neededExp;
                expSlider.value = exp;
            }
        }
    }
}
