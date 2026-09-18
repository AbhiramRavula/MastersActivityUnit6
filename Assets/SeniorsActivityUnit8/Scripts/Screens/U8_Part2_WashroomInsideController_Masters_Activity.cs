using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit8
{
    public class U8_Part2_WashroomInsideController_Masters_Activity : MonoBehaviour
    {
        [Header("UI & Step Tracking")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Button btnNextStep;
        [SerializeField] private Button btnProceedToPart3;

        [Header("1. Cubicle (Left)")]
        [SerializeField] private Image cubicleDoorImage;
        [SerializeField] private Sprite cubicleOpenSprite;
        [SerializeField] private Sprite cubicleClosedSprite;
        [SerializeField] private Button btnCloseCubicle;
        [SerializeField] private Button btnFlush;
        [SerializeField] private Button btnAnuExitCubicle;

        [Header("2. Sink, Tap & Soap (Center)")]
        [SerializeField] private Image sinkImage;
        [SerializeField] private Sprite sinkCleanSprite;
        [SerializeField] private Sprite sinkSplashedSprite;
        [SerializeField] private Image tapWaterStream;
        [SerializeField] private Button btnWashHands;
        [SerializeField] private Button btnTurnTapOff;
        [SerializeField] private Button btnWipeSink;

        [Header("3. Paper Towels & Bin (Right)")]
        [SerializeField] private Button btnPaperTowel;
        [SerializeField] private Image towelInAirImage;
        [SerializeField] private Image soggyTowelOnFloorImage;
        [SerializeField] private Image binImage;

        [Header("Anu Character Avatar")]
        [SerializeField] private Image anuInsideAvatar;
        [SerializeField] private Sprite anuNormalSprite;
        [SerializeField] private Sprite anuWashingSprite;
        [SerializeField] private Sprite anuWipingSprite;

        private int currentStepIndex = 1;

        private void Awake()
        {
            AutoFindUIReferences();
            RegisterListeners();
        }

        private void OnEnable()
        {
            // If returning from Handwash Screen, continue from Step 5
            if (U8_GameManager_Masters_Activity.Instance != null && U8_GameManager_Masters_Activity.Instance.isHandsWashed)
            {
                currentStepIndex = 5;
                if (anuInsideAvatar != null)
                {
                    anuInsideAvatar.gameObject.SetActive(true);
                    anuInsideAvatar.rectTransform.anchoredPosition = new Vector2(-80, -80);
                    if (anuNormalSprite != null) anuInsideAvatar.sprite = anuNormalSprite;
                }
                UpdateStepVisuals();
                if (feedbackText != null) feedbackText.text = "Hands clean! Now let's turn off the tap.";
                if (U8_AudioManager_Masters_Activity.Instance != null)
                {
                    U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_09"); // "Turn the tap off."
                }
            }
            else
            {
                ResetWashroomState();
                if (U8_AudioManager_Masters_Activity.Instance != null)
                {
                    U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_04"); // "Now, what next?"
                }
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
            if (feedbackText == null)
            {
                Transform t = transform.Find("FeedbackCard/FeedbackText") ?? transform.Find("FeedbackText");
                if (t != null) feedbackText = t.GetComponent<TextMeshProUGUI>();
            }

            if (btnProceedToPart3 == null) btnProceedToPart3 = FindButtonByKeywords("Proceed", "Next");
            if (btnCloseCubicle == null) btnCloseCubicle = FindButtonByKeywords("CloseDoor", "CloseCubicle", "Close");
            if (btnFlush == null) btnFlush = FindButtonByKeywords("Flush");
            if (btnAnuExitCubicle == null) btnAnuExitCubicle = FindButtonByKeywords("ComeOut", "Exit", "StepOut", "Sink");
            if (btnWashHands == null) btnWashHands = FindButtonByKeywords("WashHands", "Wash");
            if (btnTurnTapOff == null) btnTurnTapOff = FindButtonByKeywords("TurnTapOff", "TapOff", "TurnOff");
            if (btnPaperTowel == null) btnPaperTowel = FindButtonByKeywords("Towel", "PaperTowel");
            if (btnWipeSink == null) btnWipeSink = FindButtonByKeywords("WipeSink", "Wipe");

            if (cubicleDoorImage == null) cubicleDoorImage = FindImageByKeywords("CubicleDoor", "DoorImage", "Cubicle");
            if (sinkImage == null) sinkImage = FindImageByKeywords("SinkImage", "Sink");
            if (tapWaterStream == null) tapWaterStream = FindImageByKeywords("TapWaterStream", "WaterStream", "Tap");
            if (towelInAirImage == null) towelInAirImage = FindImageByKeywords("TowelInAir");
            if (soggyTowelOnFloorImage == null) soggyTowelOnFloorImage = FindImageByKeywords("SoggyTowel", "TowelOnFloor");
            if (anuInsideAvatar == null) anuInsideAvatar = FindImageByKeywords("AnuInsideAvatar", "AnuAvatar", "Anu");
        }

        private Button FindButtonByKeywords(params string[] keywords)
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var kw in keywords)
            {
                foreach (var b in buttons)
                {
                    if (b != null && b.name.IndexOf(kw, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return b;
                }
            }
            return null;
        }

        private Image FindImageByKeywords(params string[] keywords)
        {
            var images = GetComponentsInChildren<Image>(true);
            foreach (var kw in keywords)
            {
                foreach (var img in images)
                {
                    if (img != null && img.name.IndexOf(kw, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return img;
                }
            }
            return null;
        }

        private void RegisterListeners()
        {
            if (btnCloseCubicle != null)
            {
                btnCloseCubicle.onClick.RemoveListener(OnCloseCubicleClicked);
                btnCloseCubicle.onClick.AddListener(OnCloseCubicleClicked);
            }
            if (btnFlush != null)
            {
                btnFlush.onClick.RemoveListener(OnFlushClicked);
                btnFlush.onClick.AddListener(OnFlushClicked);
            }
            if (btnAnuExitCubicle != null)
            {
                btnAnuExitCubicle.onClick.RemoveListener(OnAnuExitCubicleClicked);
                btnAnuExitCubicle.onClick.AddListener(OnAnuExitCubicleClicked);
            }
            if (btnWashHands != null)
            {
                btnWashHands.onClick.RemoveListener(OnWashHandsClicked);
                btnWashHands.onClick.AddListener(OnWashHandsClicked);
            }
            if (btnTurnTapOff != null)
            {
                btnTurnTapOff.onClick.RemoveListener(OnTurnTapOffClicked);
                btnTurnTapOff.onClick.AddListener(OnTurnTapOffClicked);
            }
            if (btnPaperTowel != null)
            {
                btnPaperTowel.onClick.RemoveListener(OnPaperTowelClicked);
                btnPaperTowel.onClick.AddListener(OnPaperTowelClicked);
            }
            if (btnWipeSink != null)
            {
                btnWipeSink.onClick.RemoveListener(OnWipeSinkClicked);
                btnWipeSink.onClick.AddListener(OnWipeSinkClicked);
            }
            if (btnProceedToPart3 != null)
            {
                btnProceedToPart3.onClick.RemoveListener(OnProceedToPart3Clicked);
                btnProceedToPart3.onClick.AddListener(OnProceedToPart3Clicked);
            }
        }

        public void ResetWashroomState()
        {
            currentStepIndex = 1;

            if (cubicleDoorImage != null && cubicleOpenSprite != null) cubicleDoorImage.sprite = cubicleOpenSprite;
            if (tapWaterStream != null) tapWaterStream.gameObject.SetActive(true);
            if (sinkImage != null && sinkSplashedSprite != null) sinkImage.sprite = sinkSplashedSprite;
            if (soggyTowelOnFloorImage != null) soggyTowelOnFloorImage.gameObject.SetActive(false);
            if (towelInAirImage != null) towelInAirImage.gameObject.SetActive(false);
            if (btnProceedToPart3 != null) btnProceedToPart3.gameObject.SetActive(false);
            if (anuInsideAvatar != null)
            {
                anuInsideAvatar.gameObject.SetActive(false);
                anuInsideAvatar.rectTransform.anchoredPosition = new Vector2(-240, -80);
                if (anuNormalSprite != null) anuInsideAvatar.sprite = anuNormalSprite;
            }

            UpdateStepVisuals();
        }

        private void UpdateStepVisuals()
        {
            if (btnCloseCubicle != null) btnCloseCubicle.interactable = (currentStepIndex == 1);
            if (btnFlush != null) btnFlush.interactable = (currentStepIndex == 2);
            if (btnAnuExitCubicle != null) btnAnuExitCubicle.interactable = (currentStepIndex == 3);
            if (btnWashHands != null) btnWashHands.interactable = (currentStepIndex == 4);
            if (btnTurnTapOff != null) btnTurnTapOff.interactable = (currentStepIndex == 5);
            if (btnPaperTowel != null) btnPaperTowel.interactable = (currentStepIndex == 6);
            if (btnWipeSink != null) btnWipeSink.interactable = (currentStepIndex == 7);

            switch (currentStepIndex)
            {
                case 1:
                    if (promptText != null) promptText.text = "Step 1: Close the cubicle door first!";
                    break;
                case 2:
                    if (promptText != null) promptText.text = "Step 2: Flush the toilet.";
                    break;
                case 3:
                    if (promptText != null) promptText.text = "Step 3: Step out to the wash basin.";
                    break;
                case 4:
                    if (promptText != null) promptText.text = "Step 4: Wash hands with soap and water!";
                    break;
                case 5:
                    if (promptText != null) promptText.text = "Step 5: Turn the tap off properly.";
                    break;
                case 6:
                    if (promptText != null) promptText.text = "Step 6: Put the used paper towel in the bin.";
                    break;
                case 7:
                    if (promptText != null) promptText.text = "Step 7: Wipe off the sink for the next person.";
                    break;
                case 8:
                    if (promptText != null) promptText.text = "All set! Let's see how the washroom is left for Meera.";
                    if (btnProceedToPart3 != null) btnProceedToPart3.gameObject.SetActive(true);
                    break;
            }
        }

        public void OnCloseCubicleClicked()
        {
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_BoltClick");
            }
            if (cubicleDoorImage != null && cubicleClosedSprite != null) cubicleDoorImage.sprite = cubicleClosedSprite;
            if (U8_GameManager_Masters_Activity.Instance != null) U8_GameManager_Masters_Activity.Instance.isCubicleClosedFirst = true;

            currentStepIndex = 2;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "Door bolted safely! Remember to flush.";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_05"); // "Do not forget to flush."
            }
        }

        public void OnFlushClicked()
        {
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Flush");
            }
            if (U8_GameManager_Masters_Activity.Instance != null) U8_GameManager_Masters_Activity.Instance.isFlushed = true;

            currentStepIndex = 3;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "Flushed cleanly! Now step out to wash hands.";
        }

        public void OnAnuExitCubicleClicked()
        {
            Debug.Log("[U8_Part2] Step 3 Clicked: Anu stepping out of cubicle to sink.");
            StartCoroutine(AnuExitCubicleRoutine());
        }

        private IEnumerator AnuExitCubicleRoutine()
        {
            if (cubicleDoorImage != null && cubicleOpenSprite != null) cubicleDoorImage.sprite = cubicleOpenSprite;
            
            if (anuInsideAvatar != null)
            {
                anuInsideAvatar.gameObject.SetActive(true);
                if (anuNormalSprite != null) anuInsideAvatar.sprite = anuNormalSprite;
                
                // Smooth walk over to the sink
                Vector2 startPos = new Vector2(-240, -80);
                Vector2 sinkPos = new Vector2(-80, -80);
                for (float t = 0; t < 1f; t += Time.deltaTime * 2.5f)
                {
                    anuInsideAvatar.rectTransform.anchoredPosition = Vector2.Lerp(startPos, sinkPos, t);
                    yield return null;
                }
                anuInsideAvatar.rectTransform.anchoredPosition = sinkPos;
                if (anuWashingSprite != null) anuInsideAvatar.sprite = anuWashingSprite;
            }

            currentStepIndex = 4;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "At the sink! Washing hands with soap and water...";

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_06"); // "Now wash your hands. Keep tapping!"
            }

            yield return new WaitForSeconds(0.9f);

            // Auto-transition into the Handwashing minigame
            var gm = U8_GameManager_Masters_Activity.Instance ?? FindFirstObjectByType<U8_GameManager_Masters_Activity>(FindObjectsInactive.Include);
            if (currentStepIndex == 4 && gm != null && !gm.isHandsWashed)
            {
                Debug.Log("[U8_Part2] Auto-transitioning to Handwash Screen...");
                gm.ShowPart(U8_GamePart.HandwashScreen);
            }
        }

        public void OnWashHandsClicked()
        {
            Debug.Log("[U8_Part2] Step 4 Clicked: Opening Handwash Screen immediately.");
            var gm = U8_GameManager_Masters_Activity.Instance ?? FindFirstObjectByType<U8_GameManager_Masters_Activity>(FindObjectsInactive.Include);
            if (gm != null)
            {
                gm.ShowPart(U8_GamePart.HandwashScreen);
            }
        }

        public void OnTurnTapOffClicked()
        {
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_TapOff");
            }
            if (tapWaterStream != null) tapWaterStream.gameObject.SetActive(false);
            if (U8_GameManager_Masters_Activity.Instance != null) U8_GameManager_Masters_Activity.Instance.isTapTurnedOff = true;

            currentStepIndex = 6;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "Water saved! Now dry hands and throw the towel in the bin.";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_10"); // "Towel in the bin."
            }
        }

        public void OnPaperTowelClicked()
        {
            StartCoroutine(PaperTowelRoutine());
        }

        private IEnumerator PaperTowelRoutine()
        {
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_TowelPull");
            }

            if (towelInAirImage != null)
            {
                towelInAirImage.gameObject.SetActive(true);
                // Quick arc animation towards bin
                Vector3 startPos = towelInAirImage.transform.localPosition;
                for (float t = 0; t < 1f; t += Time.deltaTime * 3f)
                {
                    towelInAirImage.transform.localPosition = startPos + new Vector3(t * 80f, Mathf.Sin(t * Mathf.PI) * 40f - (t * 60f), 0);
                    yield return null;
                }
                towelInAirImage.gameObject.SetActive(false);
            }

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_BinDrop");
            }

            if (U8_GameManager_Masters_Activity.Instance != null) U8_GameManager_Masters_Activity.Instance.isTowelInBin = true;

            currentStepIndex = 7;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "Towel in the bin! Final step: wipe off the sink.";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_11"); // "Wipe the sink."
            }
        }

        public void OnWipeSinkClicked()
        {
            StartCoroutine(WipeSinkRoutine());
        }

        private IEnumerator WipeSinkRoutine()
        {
            if (anuInsideAvatar != null && anuWipingSprite != null) anuInsideAvatar.sprite = anuWipingSprite;

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Wipe");
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
            }

            if (sinkImage != null && sinkCleanSprite != null) sinkImage.sprite = sinkCleanSprite;
            if (U8_GameManager_Masters_Activity.Instance != null) U8_GameManager_Masters_Activity.Instance.isSinkWiped = true;

            yield return new WaitForSeconds(1.2f);

            if (anuInsideAvatar != null && anuNormalSprite != null) anuInsideAvatar.sprite = anuNormalSprite;

            currentStepIndex = 8;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "Sink wiped clean and sparkling! Ready for the next person.";
        }

        public void OnProceedToPart3Clicked()
        {
            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.Part3_AfterYou);
            }
        }
    }
}
