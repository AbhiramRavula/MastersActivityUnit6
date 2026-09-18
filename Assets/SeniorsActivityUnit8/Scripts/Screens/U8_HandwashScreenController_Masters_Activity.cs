using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit8
{
    public class U8_HandwashScreenController_Masters_Activity : MonoBehaviour
    {
        [Header("UI Prompts & Feedback")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Slider timerSlider;
        [SerializeField] private Button btnScrubArea;

        [Header("Hands & Soap Visuals")]
        [SerializeField] private Image handsImage;
        [SerializeField] private Sprite handsNormalSprite;
        [SerializeField] private Sprite handsSoapySprite;
        [SerializeField] private Sprite handsSparklingSprite;
        [SerializeField] private Image waterStreamImage;
        [SerializeField] private GameObject bubbleContainer;
        [SerializeField] private List<Image> bubbleSprites = new List<Image>();

        [Header("4 Friendly Germ Blobs")]
        [SerializeField] private List<Image> germBlobs = new List<Image>();

        [Header("Settings & Timer")]
        [SerializeField] private float songDurationSeconds = 20.0f;
        [SerializeField] private int scrubsPerGerm = 4;

        private float currentTimer = 0f;
        private int scrubCount = 0;
        private int germsCleared = 0;
        private bool isHandwashingActive = false;
        private float timeSinceLastScrub = 0f;
        private Coroutine handwashCoroutine;

        private void Awake()
        {
            AutoFindUIReferences();
            if (btnScrubArea != null)
            {
                btnScrubArea.onClick.AddListener(OnScrubClicked);
            }
        }

        private void OnEnable()
        {
            StartHandwashingSession();
        }

        private void OnDisable()
        {
            if (handwashCoroutine != null) StopCoroutine(handwashCoroutine);
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.StopBGM();
            }
        }

        private void AutoFindUIReferences()
        {
            if (promptText == null)
            {
                Transform t = transform.Find("PromptCard/PromptText") ?? transform.Find("PromptText");
                if (t != null) promptText = t.GetComponent<TextMeshProUGUI>();
                else promptText = GetComponentInChildren<TextMeshProUGUI>(true);
            }
            if (timerText == null)
            {
                Transform t = transform.Find("TimerBadge/TimerText") ?? transform.Find("TimerText");
                if (t != null) timerText = t.GetComponent<TextMeshProUGUI>();
            }

            if (btnScrubArea == null)
            {
                Transform t = transform.Find("ScrubAreaButton") ?? transform.Find("HandsImage") ?? transform.Find("Btn_Scrub");
                if (t != null) btnScrubArea = t.GetComponent<Button>();
            }
            if (handsImage == null)
            {
                Transform t = transform.Find("HandsImage") ?? transform.Find("Hands");
                if (t != null) handsImage = t.GetComponent<Image>();
            }
            if (bubbleContainer == null)
            {
                Transform t = transform.Find("Bubbles") ?? transform.Find("BubbleContainer");
                if (t != null) bubbleContainer = t.gameObject;
            }
            if (germBlobs == null || germBlobs.Count == 0)
            {
                germBlobs = new List<Image>();
                for (int i = 1; i <= 4; i++)
                {
                    Transform t = transform.Find($"Germ_{i}") ?? transform.Find($"Germs/Germ_{i}");
                    if (t != null)
                    {
                        Image g = t.GetComponent<Image>();
                        if (g != null) germBlobs.Add(g);
                    }
                }
            }
        }

        public void StartHandwashingSession()
        {
            isHandwashingActive = true;
            currentTimer = songDurationSeconds;
            scrubCount = 0;
            germsCleared = 0;
            timeSinceLastScrub = 0f;

            if (handsImage != null && handsSoapySprite != null) handsImage.sprite = handsSoapySprite;
            if (bubbleContainer != null) bubbleContainer.SetActive(true);

            // Reset all 4 friendly germ blobs
            foreach (var blob in germBlobs)
            {
                if (blob != null)
                {
                    blob.gameObject.SetActive(true);
                    blob.transform.localScale = Vector3.one;
                }
            }

            if (promptText != null) promptText.text = "Keep tapping to scrub hands with soap!";
            if (timerSlider != null)
            {
                timerSlider.maxValue = songDurationSeconds;
                timerSlider.value = songDurationSeconds;
            }

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_06"); // "Now wash your hands. Keep tapping!"
                U8_AudioManager_Masters_Activity.Instance.PlayBGM("MUS_HandwashSong", false);
            }

            if (handwashCoroutine != null) StopCoroutine(handwashCoroutine);
            handwashCoroutine = StartCoroutine(HandwashTimerRoutine());
        }

        public void OnScrubClicked()
        {
            if (!isHandwashingActive) return;

            scrubCount++;
            timeSinceLastScrub = 0f;

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Scrub");
                if (scrubCount % 2 == 0) U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Bubble");
            }

            // Animate bubble growth
            if (bubbleSprites != null)
            {
                foreach (var bubble in bubbleSprites)
                {
                    if (bubble != null)
                    {
                        bubble.transform.localScale = Vector3.one * Mathf.Clamp(0.5f + (scrubCount * 0.05f), 0.5f, 1.3f);
                    }
                }
            }

            // Check if a germ blob slides off
            int targetGermsToClear = Mathf.Clamp(scrubCount / scrubsPerGerm, 0, germBlobs.Count);
            if (targetGermsToClear > germsCleared)
            {
                int indexToClear = germsCleared;
                germsCleared = targetGermsToClear;
                if (indexToClear < germBlobs.Count && germBlobs[indexToClear] != null)
                {
                    StartCoroutine(SlideOffGermRoutine(germBlobs[indexToClear]));
                }
            }
        }

        private IEnumerator SlideOffGermRoutine(Image germImage)
        {
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_GermOff");
            }

            Vector3 startPos = germImage.transform.localPosition;
            for (float t = 0; t < 1f; t += Time.deltaTime * 3f)
            {
                germImage.transform.localPosition = startPos + new Vector3(t * 60f, -t * 80f, 0);
                germImage.transform.localScale = Vector3.one * (1f - t);
                yield return null;
            }
            germImage.gameObject.SetActive(false);
            germImage.transform.localPosition = startPos;
        }

        private IEnumerator HandwashTimerRoutine()
        {
            while (currentTimer > 0f)
            {
                currentTimer -= Time.deltaTime;
                timeSinceLastScrub += Time.deltaTime;

                if (timerSlider != null) timerSlider.value = currentTimer;
                if (timerText != null) timerText.text = $"{Mathf.CeilToInt(currentTimer)}s";

                // Encouragement if student pauses tapping
                if (timeSinceLastScrub > 3.0f && isHandwashingActive)
                {
                    if (promptText != null) promptText.text = "Keep going!";
                    if (U8_AudioManager_Masters_Activity.Instance != null)
                    {
                        U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_07"); // "Keep going!"
                    }
                    timeSinceLastScrub = 0f; // Reset warning cooldown
                }

                yield return null;
            }

            FinishHandwashing();
        }

        private void FinishHandwashing()
        {
            isHandwashingActive = false;

            // Clear remaining germs
            foreach (var blob in germBlobs)
            {
                if (blob != null) blob.gameObject.SetActive(false);
            }

            if (handsImage != null && handsSparklingSprite != null) handsImage.sprite = handsSparklingSprite;
            if (promptText != null) promptText.text = "All clean! Star 1 Earned!";

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_08"); // "All clean! One star."
            }

            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.isHandsWashed = true;
                U8_GameManager_Masters_Activity.Instance.AwardStar();
            }

            StartCoroutine(ReturnToWashroomRoutine());
        }

        private IEnumerator ReturnToWashroomRoutine()
        {
            yield return new WaitForSeconds(2.2f);
            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.Part2_Inside);
            }
        }
    }
}
