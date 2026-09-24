using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TopDownSurvival.Core;
using TopDownSurvival.Player;

namespace TopDownSurvival.UI
{
    public class ScreenEffects : MonoBehaviour
    {
        public static ScreenEffects Instance { get; private set; }

        [Header("Damage Flash Settings")]
        [SerializeField] private Image damageOverlay;
        [SerializeField] private float flashDuration = 0.2f;
        [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.4f);

        [Header("Announcement Panel")]
        [SerializeField] private GameObject announcementPanel;
        [SerializeField] private Text announcementTitle;
        [SerializeField] private Text announcementSubtitle;
        [SerializeField] private float announcementDuration = 2.5f;

        private Coroutine flashCoroutine;
        private Coroutine announcementCoroutine;
        private SurvivalStats playerStats;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (damageOverlay != null)
            {
                damageOverlay.color = Color.clear;
                damageOverlay.enabled = true;
            }

            if (announcementPanel != null)
            {
                announcementPanel.SetActive(false);
            }

            // Bind player damage and quest events
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<SurvivalStats>();
                Damageable dmg = player.GetComponent<Damageable>();
                if (dmg != null)
                {
                    dmg.OnHealthChanged += HandlePlayerHealthChanged;
                }

                PlayerProgression prog = player.GetComponent<PlayerProgression>();
                if (prog != null)
                {
                    prog.OnLevelUp += HandleLevelUp;
                }
            }

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestUpdated += HandleQuestUpdated;
            }
        }

        private void OnDestroy()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnQuestUpdated -= HandleQuestUpdated;
            }
        }

        private void HandlePlayerHealthChanged(float current, float max)
        {
            // Flash if damage taken
            TriggerFlash();
        }

        private void HandleLevelUp(int newLevel)
        {
            TriggerAnnouncement("LEVEL UP!", $"Cấp độ hiện tại: {newLevel}", new Color(0.4f, 0.9f, 1f));
        }

        private void HandleQuestUpdated()
        {
            // Simple check: we don't have direct OnQuestCompleted event, but we can detect indexing.
            // Let's print a check statement or handle when quest changes. We can just broadcast.
        }

        public void TriggerFlash()
        {
            if (damageOverlay == null) return;
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(CoFlash());
        }

        private IEnumerator CoFlash()
        {
            float elapsed = 0f;
            damageOverlay.enabled = true;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float ratio = elapsed / flashDuration;
                damageOverlay.color = Color.Lerp(flashColor, Color.clear, ratio);
                yield return null;
            }
            damageOverlay.color = Color.clear;
        }

        public void TriggerAnnouncement(string title, string subtitle, Color titleColor)
        {
            if (announcementPanel == null) return;
            if (announcementCoroutine != null) StopCoroutine(announcementCoroutine);
            announcementCoroutine = StartCoroutine(CoAnnouncement(title, subtitle, titleColor));
        }

        private IEnumerator CoAnnouncement(string title, string subtitle, Color titleColor)
        {
            announcementPanel.SetActive(true);
            if (announcementTitle != null)
            {
                announcementTitle.text = title;
                announcementTitle.color = titleColor;
            }
            if (announcementSubtitle != null)
            {
                announcementSubtitle.text = subtitle;
            }

            yield return new WaitForSeconds(announcementDuration);
            announcementPanel.SetActive(false);
        }
    }
}
