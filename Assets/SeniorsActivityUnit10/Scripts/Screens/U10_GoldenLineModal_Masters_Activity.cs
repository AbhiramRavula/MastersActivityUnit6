using System;
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

        private Action onCloseCallback;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseModal);
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
            U10_AudioManager_Masters_Activity.Instance?.PlayVO($"VO_U10_13_{voWeek:D2}");
        }

        public void CloseModal()
        {
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
