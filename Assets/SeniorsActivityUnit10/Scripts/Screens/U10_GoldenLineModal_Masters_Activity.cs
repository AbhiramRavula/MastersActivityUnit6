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
            var btnImg = closeButton != null ? closeButton.GetComponent<Image>() : null;
            var btnTMP = closeButton != null ? closeButton.GetComponentInChildren<TextMeshProUGUI>() : null;
            Color originalImgColor = btnImg != null ? btnImg.color : new Color(0.16f, 0.62f, 0.28f, 1f);

            // 1. Visually and interactively lock the button while listening to the quote
            if (closeButton != null)
            {
                closeButton.interactable = false;
                closeButton.transform.localScale = Vector3.one * 0.92f;
            }

            if (btnImg != null)
            {
                // Muted translucent slate while audio is playing
                btnImg.color = new Color(0.52f, 0.58f, 0.62f, 0.7f);
            }

            if (btnTMP != null)
            {
                btnTMP.text = "Listen";
                btnTMP.color = new Color(1f, 1f, 1f, 0.6f);
            }

            float waitTime = Mathf.Max(duration + 0.2f, 1.5f);
            yield return new WaitForSeconds(waitTime);

            // 2. Unlock and restore vibrant button styling once voiceover finishes
            if (btnImg != null)
            {
                btnImg.color = (originalImgColor.a > 0.1f && originalImgColor.g > 0.3f) ? originalImgColor : new Color(0.16f, 0.62f, 0.28f, 1f);
            }

            if (btnTMP != null)
            {
                btnTMP.text = "Continue";
                btnTMP.color = Color.white;
            }

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
