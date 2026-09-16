using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    public class U6_SliderScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Phase 1: Volume Slider")]
        [SerializeField] private GameObject volumePhaseContainer;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI currentZoneLabel;
        [SerializeField] private Button sayItButton;

        [Header("Phase 2: Leaving The Restaurant")]
        [SerializeField] private GameObject leavingPhaseContainer;
        [SerializeField] private GameObject waitingFamilyVisual;
        [SerializeField] private Button leavePolitelyButton;
        [SerializeField] private Button stayAndPlayButton;

        [Header("Feedback Panel")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;

        private U6_VolumeZone currentZone = U6_VolumeZone.JustRight;

        private void Awake()
        {
            if (volumeSlider != null)
                volumeSlider.onValueChanged.AddListener(OnSliderMoved);

            if (sayItButton != null)
                sayItButton.onClick.AddListener(OnSayItClicked);

            if (leavePolitelyButton != null)
                leavePolitelyButton.onClick.AddListener(() => OnLeavingDecision(true));

            if (stayAndPlayButton != null)
                stayAndPlayButton.onClick.AddListener(() => OnLeavingDecision(false));
        }

        private void OnEnable()
        {
            ResetScreen();
        }

        private void ResetScreen()
        {
            if (volumePhaseContainer) volumePhaseContainer.SetActive(true);
            if (leavingPhaseContainer) leavingPhaseContainer.SetActive(false);
            if (feedbackPanel) feedbackPanel.SetActive(false);

            if (volumeSlider != null)
            {
                volumeSlider.value = 0.5f;
                OnSliderMoved(0.5f);
            }

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_10");
            }
        }

        private void OnSliderMoved(float value)
        {
            if (value < 0.33f)
            {
                currentZone = U6_VolumeZone.Whisper;
                if (currentZoneLabel) currentZoneLabel.text = "WHISPER";
            }
            else if (value < 0.67f)
            {
                currentZone = U6_VolumeZone.JustRight;
                if (currentZoneLabel) currentZoneLabel.text = "JUST RIGHT ★";
            }
            else
            {
                currentZone = U6_VolumeZone.BigVoice;
                if (currentZoneLabel) currentZoneLabel.text = "BIG VOICE";
            }

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_SliderZone");
            }
        }

        private void OnSayItClicked()
        {
            StartCoroutine(ResolveVolumeSequence());
        }

        private IEnumerator ResolveVolumeSequence()
        {
            if (feedbackPanel) feedbackPanel.SetActive(true);

            switch (currentZone)
            {
                case U6_VolumeZone.Whisper:
                    if (feedbackText) feedbackText.text = "Father leans in: \"Sorry? I cannot hear you at all.\"";
                    if (U6_AudioManager_Masters_Activity.Instance != null)
                    {
                        U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_DAD_1");
                    }
                    yield return new WaitForSeconds(3.0f);
                    if (feedbackPanel) feedbackPanel.SetActive(false);
                    break;

                case U6_VolumeZone.BigVoice:
                    if (feedbackText) feedbackText.text = "Too loud! Heads turn and a nearby baby starts crying.";
                    if (U6_AudioManager_Masters_Activity.Instance != null)
                    {
                        U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_BabyCry");
                    }
                    yield return new WaitForSeconds(3.0f);
                    if (feedbackPanel) feedbackPanel.SetActive(false);
                    break;

                case U6_VolumeZone.JustRight:
                    if (feedbackText) feedbackText.text = "Perfect! Father hears clearly and the room is undisturbed.";
                    if (U6_AudioManager_Masters_Activity.Instance != null)
                    {
                        U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                    }
                    yield return new WaitForSeconds(2.5f);

                    ShowLeavingPhase();
                    break;
            }
        }

        private void ShowLeavingPhase()
        {
            if (volumePhaseContainer) volumePhaseContainer.SetActive(false);
            if (feedbackPanel) feedbackPanel.SetActive(false);
            if (leavingPhaseContainer) leavingPhaseContainer.SetActive(true);

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_11");
            }
        }

        private void OnLeavingDecision(bool leaveNow)
        {
            StartCoroutine(ResolveLeavingSequence(leaveNow));
        }

        private IEnumerator ResolveLeavingSequence(bool leaveNow)
        {
            if (feedbackPanel) feedbackPanel.SetActive(true);

            if (leaveNow)
            {
                if (feedbackText)
                    feedbackText.text = "Anu thanks Ravi and leaves. The waiting family gets a table! ★ Star 3 Earned!";

                U6_GameManager_Masters_Activity.Instance.AwardStar();

                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_WAIT_4");
                }

                yield return new WaitForSeconds(3.5f);

                U6_GameManager_Masters_Activity.Instance.StartEnding();
            }
            else
            {
                if (feedbackText)
                    feedbackText.text = "The family with the tired child is still standing and waiting at the door...";

                yield return new WaitForSeconds(3.5f);
                if (feedbackPanel) feedbackPanel.SetActive(false);
            }
        }
    }
}
