using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Googolplex.Unit10
{
    /// <summary>
    /// Smooth pop-up attention guidance and gentle breathing animation for child-facing interactive buttons.
    /// </summary>
    public class U10_ButtonAttentionPulse_Masters_Activity : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private bool autoPopInOnEnable = false;
        [SerializeField] private bool idlePulseEnabled = true;
        [SerializeField] private float pulseScale = 1.045f;
        [SerializeField] private float pulseFrequency = 2.2f;

        private RectTransform rt;
        private Button btn;
        private Vector3 originalScale = Vector3.one;
        private Coroutine activeAnimRoutine;
        private bool isPulsing = false;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            btn = GetComponent<Button>();
            if (rt != null)
            {
                originalScale = rt.localScale == Vector3.zero ? Vector3.one : rt.localScale;
            }
        }

        private void OnEnable()
        {
            if (autoPopInOnEnable)
            {
                PopIn(0f);
            }
            else if (idlePulseEnabled)
            {
                StartPulse();
            }
        }

        private void OnDisable()
        {
            StopAnimation();
            if (rt != null) rt.localScale = originalScale;
        }

        /// <summary>
        /// Smoothly pops up the button from scale 0 to 1 with an overshoot spring bounce.
        /// </summary>
        public void PopIn(float delay = 0f, float duration = 0.45f)
        {
            if (!gameObject.activeInHierarchy) return;
            StopAnimation();
            activeAnimRoutine = StartCoroutine(PopInRoutine(delay, duration));
        }

        private IEnumerator PopInRoutine(float delay, float duration)
        {
            if (rt == null) yield break;
            rt.localScale = Vector3.zero;

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Smooth elastic overshoot easing: f(t) = sin(t * pi/2) + overshoot
                float scaleFactor;
                if (t < 0.7f)
                {
                    // Overshoot up to 1.12x
                    float subT = t / 0.7f;
                    scaleFactor = Mathf.Lerp(0f, 1.12f, Mathf.Sin(subT * Mathf.PI * 0.5f));
                }
                else
                {
                    // Settle back to 1.0x
                    float subT = (t - 0.7f) / 0.3f;
                    scaleFactor = Mathf.Lerp(1.12f, 1.0f, Mathf.SmoothStep(0f, 1f, subT));
                }

                rt.localScale = originalScale * scaleFactor;
                yield return null;
            }

            rt.localScale = originalScale;

            if (idlePulseEnabled)
            {
                StartPulse();
            }
        }

        public void StartPulse()
        {
            if (!gameObject.activeInHierarchy || isPulsing) return;
            isPulsing = true;
            activeAnimRoutine = StartCoroutine(IdlePulseRoutine());
        }

        public void StopPulse()
        {
            isPulsing = false;
            StopAnimation();
            if (rt != null) rt.localScale = originalScale;
        }

        private IEnumerator IdlePulseRoutine()
        {
            while (isPulsing)
            {
                // Only pulse if button is interactable (or if no button attached)
                if (btn == null || btn.interactable)
                {
                    float sine = Mathf.Sin(Time.time * pulseFrequency);
                    // Map -1..1 to 0..1
                    float normalized = (sine + 1f) * 0.5f;
                    float scaleFactor = Mathf.Lerp(1.0f, pulseScale, Mathf.SmoothStep(0f, 1f, normalized));
                    if (rt != null)
                    {
                        rt.localScale = originalScale * scaleFactor;
                    }
                }
                else
                {
                    if (rt != null) rt.localScale = originalScale;
                }
                yield return null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (btn != null && !btn.interactable) return;
            if (rt != null)
            {
                rt.localScale = originalScale * 0.94f;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (btn != null && !btn.interactable) return;
            if (rt != null)
            {
                rt.localScale = originalScale;
            }
        }

        private void StopAnimation()
        {
            if (activeAnimRoutine != null)
            {
                StopCoroutine(activeAnimRoutine);
                activeAnimRoutine = null;
            }
        }
    }
}
