using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_GoldenLineModal_Masters_Activity : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI quoteText;
        [SerializeField] private Button closeButton;

        [Header("Golden Line Voiceovers (12 Weeks)")]
        [SerializeField] private List<AudioClip> goldenLineVoiceovers = new List<AudioClip>();

        private Action onCloseCallback;
        private Coroutine audioGateRoutine;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseModal);

                if (closeButton.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>() == null)
                {
                    closeButton.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                }
            }
        }

        public void ShowQuote(int weekNumber, Action onClose = null)
        {
            onCloseCallback = onClose;

            if (modalPanel != null)
            {
                modalPanel.SetActive(true);
            }

            int quoteIndex = Mathf.Clamp(weekNumber - 1, 0, U10_SaveSystem.GOLDEN_LINES_BY_WEEK.Length - 1);
            string quote = U10_SaveSystem.GOLDEN_LINES_BY_WEEK[quoteIndex];

            if (headerText != null)
            {
                headerText.text = $"Golden Quote of Week {weekNumber}";
            }

            if (quoteText != null)
            {
                quoteText.text = $"\"{quote}\"";
            }

            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_GoldenLine");
            int voWeek = Mathf.Clamp(weekNumber, 1, 12);
            float duration = 3.5f;

            if (goldenLineVoiceovers != null && goldenLineVoiceovers.Count >= voWeek && goldenLineVoiceovers[voWeek - 1] != null)
            {
                duration = U10_AudioManager_Masters_Activity.Instance?.PlayVOClip(goldenLineVoiceovers[voWeek - 1]) ?? 3.5f;
            }
            else
            {
                duration = U10_AudioManager_Masters_Activity.Instance?.PlayVO($"VO_U10_13_{voWeek:D2}") ?? 3.5f;
            }

            if (audioGateRoutine != null) StopCoroutine(audioGateRoutine);
            audioGateRoutine = StartCoroutine(WaitForQuoteVOThenUnlock(duration));
        }

        private IEnumerator WaitForQuoteVOThenUnlock(float duration)
        {
            if (closeButton != null)
            {
                closeButton.interactable = false;
                closeButton.transform.localScale = Vector3.one * 0.9f;
            }

            float waitTime = Mathf.Max(duration + 0.2f, 1.5f);
            yield return new WaitForSeconds(waitTime);

            if (closeButton != null)
            {
                closeButton.interactable = true;
                var pulse = closeButton.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                if (pulse != null) pulse.PopIn(0f, 0.45f);
                else closeButton.transform.localScale = Vector3.one;
            }

            audioGateRoutine = null;
        }

        public void CloseModal()
        {
            if (audioGateRoutine != null)
            {
                StopCoroutine(audioGateRoutine);
                audioGateRoutine = null;
            }

            U10_AudioManager_Masters_Activity.Instance?.StopVO();
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");

            if (modalPanel != null)
            {
                modalPanel.SetActive(false);
            }

            onCloseCallback?.Invoke();
            onCloseCallback = null;
        }
    }
}
