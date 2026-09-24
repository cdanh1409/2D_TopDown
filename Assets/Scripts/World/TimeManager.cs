using UnityEngine;
#if UNITY_2021_2_OR_NEWER
using UnityEngine.Rendering.Universal;
#endif

namespace TopDownSurvival.World
{
    public class TimeManager : MonoBehaviour
    {
        public enum DayState
        {
            Sunrise,
            Day,
            Sunset,
            Night
        }

        [Header("Cycle Settings")]
        [SerializeField] private float dayDurationInSeconds = 120f; // 2 minutes for a full day cycle
        
#if UNITY_2021_2_OR_NEWER
        [Header("Lighting References")]
        [SerializeField] private Light2D globalLight;
#endif
        
        [Header("Lighting Gradients")]
        [SerializeField] private Gradient lightColorGradient;

        private float currentTime = 0f;
        private DayState currentState = DayState.Day;

        public System.Action<DayState> OnStateChanged;

        private void Start()
        {
            // Initial gradient setup if not configured
            if (lightColorGradient == null || lightColorGradient.colorKeys.Length <= 1)
            {
                var gradient = new Gradient();
                var colorKeys = new GradientColorKey[4];
                var alphaKeys = new GradientAlphaKey[4];

                colorKeys[0] = new GradientColorKey(new Color(1f, 0.6f, 0.4f), 0.0f); // Sunrise (warm orange)
                colorKeys[1] = new GradientColorKey(Color.white, 0.25f);              // Day (white)
                colorKeys[2] = new GradientColorKey(new Color(0.9f, 0.4f, 0.2f), 0.7f); // Sunset (deep orange)
                colorKeys[3] = new GradientColorKey(new Color(0.1f, 0.1f, 0.25f), 0.85f); // Night (dark blue)

                alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphaKeys[1] = new GradientAlphaKey(1.0f, 0.25f);
                alphaKeys[2] = new GradientAlphaKey(1.0f, 0.7f);
                alphaKeys[3] = new GradientAlphaKey(1.0f, 0.85f);

                gradient.SetKeys(colorKeys, alphaKeys);
                lightColorGradient = gradient;
            }

#if UNITY_2021_2_OR_NEWER
            if (globalLight == null)
            {
                globalLight = FindAnyObjectByType<Light2D>();
            }
#endif
        }

        private void Update()
        {
            // Advance time
            currentTime += Time.deltaTime;
            float progress = (currentTime % dayDurationInSeconds) / dayDurationInSeconds;

            // Apply light colors based on gradient
            UpdateLighting(progress);

            // Determine state transitions
            UpdateState(progress);
        }

        private void UpdateLighting(float progress)
        {
#if UNITY_2021_2_OR_NEWER
            if (globalLight != null && lightColorGradient != null)
            {
                globalLight.color = lightColorGradient.Evaluate(progress);
            }
#endif
        }

        private void UpdateState(float progress)
        {
            DayState newState;
            if (progress >= 0f && progress < 0.2f)
            {
                newState = DayState.Sunrise;
            }
            else if (progress >= 0.2f && progress < 0.65f)
            {
                newState = DayState.Day;
            }
            else if (progress >= 0.65f && progress < 0.8f)
            {
                newState = DayState.Sunset;
            }
            else
            {
                newState = DayState.Night;
            }

            if (newState != currentState)
            {
                currentState = newState;
                OnStateChanged?.Invoke(currentState);
                Debug.Log($"Day/Night State changed to: {currentState}");
            }
        }

        public DayState GetCurrentState()
        {
            return currentState;
        }

        public float GetTimeProgress()
        {
            return (currentTime % dayDurationInSeconds) / dayDurationInSeconds;
        }
    }
}
