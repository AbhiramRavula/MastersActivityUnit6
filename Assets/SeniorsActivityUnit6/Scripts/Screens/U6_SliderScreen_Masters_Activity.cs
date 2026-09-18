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
        [SerializeField] private Image sliderFillImage;
        [SerializeField] private Image handleKnobImage;
        [SerializeField] private TextMeshProUGUI currentZoneLabel;
        [SerializeField] private Button sayItButton;

        [Header("Zone Card Highlights")]
        [SerializeField] private GameObject whisperHighlight;
        [SerializeField] private GameObject justRightHighlight;
        [SerializeField] private GameObject bigVoiceHighlight;

        [Header("Phase 2: Leaving The Restaurant")]
        [SerializeField] private GameObject leavingPhaseContainer;
        [SerializeField] private GameObject waitingFamilyVisual;
        [SerializeField] private Button leavePolitelyButton;
        [SerializeField] private Button stayAndPlayButton;

#pragma warning disable 0414
        [Header("Audios & SFX Used On This Screen")]
        [SerializeField] private string voVolumeIntro = "VO_U6_10 (How loud should Anu talk here?)";
        [SerializeField] private string sfxSliderZone = "SFX_SliderZone (Zone tick bubble sound)";
        [SerializeField] private string voDadCantHear = "VO_U6_DAD_1 (Sorry, I cannot hear you at all)";
        [SerializeField] private string sfxBabyCry = "SFX_BabyCry (Baby crying if too loud)";
        [SerializeField] private string sfxSparkle = "SFX_Sparkle (Just right volume success)";
        [SerializeField] private string voLeavingIntro = "VO_U6_11 (The restaurant is full now...)";
        [SerializeField] private string voWaiterGoodbye = "VO_U6_WAIT_4 (Thank you, do come again!)";
        [SerializeField] private string sfxStar = "SFX_Star (Star 3 Earned)";
#pragma warning restore 0414

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
                volumeSlider.minValue = 0f;
                volumeSlider.maxValue = 2f;
                volumeSlider.wholeNumbers = true;
                volumeSlider.value = 1f; // Starts in the middle: Just Right
                OnSliderMoved(1f);
            }

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_10");
            }
        }

        private void OnSliderMoved(float rawValue)
        {
            int stepIndex = Mathf.Clamp(Mathf.RoundToInt(rawValue), 0, 2);
            if (volumeSlider != null && Mathf.RoundToInt(volumeSlider.value) != stepIndex)
            {
                volumeSlider.value = stepIndex;
            }

            U6_VolumeZone previousZone = currentZone;
            Color zoneColor;

            switch (stepIndex)
            {
                case 0: // Left Step: Whisper
                    currentZone = U6_VolumeZone.Whisper;
                    zoneColor = new Color(0.2f, 0.55f, 0.95f);
                    if (currentZoneLabel)
                    {
                        currentZoneLabel.text = "WHISPER  (Too Soft)";
                        currentZoneLabel.color = zoneColor;
                    }
                    break;

                case 1: // Center Step: Just Right
                default:
                    currentZone = U6_VolumeZone.JustRight;
                    zoneColor = new Color(0.15f, 0.72f, 0.32f);
                    if (currentZoneLabel)
                    {
                        currentZoneLabel.text = "JUST RIGHT  (Polite & Clear)";
                        currentZoneLabel.color = zoneColor;
                    }
                    break;

                case 2: // Right Step: Big Voice
                    currentZone = U6_VolumeZone.BigVoice;
                    zoneColor = new Color(0.92f, 0.3f, 0.22f);
                    if (currentZoneLabel)
                    {
                        currentZoneLabel.text = "BIG VOICE  (Too Loud!)";
                        currentZoneLabel.color = zoneColor;
                    }
                    break;
            }

            if (sliderFillImage) sliderFillImage.color = zoneColor;
            if (handleKnobImage) handleKnobImage.color = zoneColor;

            if (whisperHighlight) whisperHighlight.SetActive(currentZone == U6_VolumeZone.Whisper);
            if (justRightHighlight) justRightHighlight.SetActive(currentZone == U6_VolumeZone.JustRight);
            if (bigVoiceHighlight) bigVoiceHighlight.SetActive(currentZone == U6_VolumeZone.BigVoice);

            // Only trigger tick sound when entering a new step
            if (currentZone != previousZone && U6_AudioManager_Masters_Activity.Instance != null)
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
                    feedbackText.text = "Anu thanks Ravi and leaves. The waiting family gets a table! Star 3 Earned!";

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
