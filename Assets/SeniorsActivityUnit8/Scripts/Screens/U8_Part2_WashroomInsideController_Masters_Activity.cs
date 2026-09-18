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
            if (promptText == null) promptText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (btnProceedToPart3 == null)
            {
                Transform t = transform.Find("Btn_ProceedToPart3") ?? transform.Find("Btn_Next") ?? transform.Find("BottomBar/Btn_Proceed");
                if (t != null) btnProceedToPart3 = t.GetComponent<Button>();
            }
            if (btnCloseCubicle == null)
            {
                Transform t = transform.Find("Cubicle/Btn_CloseDoor") ?? transform.Find("Btn_CloseCubicle");
                if (t != null) btnCloseCubicle = t.GetComponent<Button>();
            }
            if (btnFlush == null)
            {
                Transform t = transform.Find("Cubicle/Btn_Flush") ?? transform.Find("Btn_Flush");
                if (t != null) btnFlush = t.GetComponent<Button>();
            }
            if (btnAnuExitCubicle == null)
            {
                Transform t = transform.Find("Cubicle/Btn_Exit") ?? transform.Find("Btn_ComeOut");
                if (t != null) btnAnuExitCubicle = t.GetComponent<Button>();
            }
            if (btnWashHands == null)
            {
                Transform t = transform.Find("Sink/Btn_WashHands") ?? transform.Find("Btn_WashHands");
                if (t != null) btnWashHands = t.GetComponent<Button>();
            }
            if (btnTurnTapOff == null)
            {
                Transform t = transform.Find("Sink/Btn_TurnTapOff") ?? transform.Find("Btn_TurnTapOff");
                if (t != null) btnTurnTapOff = t.GetComponent<Button>();
            }
            if (btnPaperTowel == null)
            {
                Transform t = transform.Find("TowelHolder/Btn_Towel") ?? transform.Find("Btn_PaperTowel");
                if (t != null) btnPaperTowel = t.GetComponent<Button>();
            }
            if (btnWipeSink == null)
            {
                Transform t = transform.Find("Sink/Btn_WipeSink") ?? transform.Find("Btn_WipeSink");
                if (t != null) btnWipeSink = t.GetComponent<Button>();
            }
        }

        private void RegisterListeners()
        {
            if (btnCloseCubicle != null) btnCloseCubicle.onClick.AddListener(OnCloseCubicleClicked);
            if (btnFlush != null) btnFlush.onClick.AddListener(OnFlushClicked);
            if (btnAnuExitCubicle != null) btnAnuExitCubicle.onClick.AddListener(OnAnuExitCubicleClicked);
            if (btnWashHands != null) btnWashHands.onClick.AddListener(OnWashHandsClicked);
            if (btnTurnTapOff != null) btnTurnTapOff.onClick.AddListener(OnTurnTapOffClicked);
            if (btnPaperTowel != null) btnPaperTowel.onClick.AddListener(OnPaperTowelClicked);
            if (btnWipeSink != null) btnWipeSink.onClick.AddListener(OnWipeSinkClicked);
            if (btnProceedToPart3 != null) btnProceedToPart3.onClick.AddListener(OnProceedToPart3Clicked);
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
            if (cubicleDoorImage != null && cubicleOpenSprite != null) cubicleDoorImage.sprite = cubicleOpenSprite;
            if (anuInsideAvatar != null) anuInsideAvatar.gameObject.SetActive(true);

            currentStepIndex = 4;
            UpdateStepVisuals();
            if (feedbackText != null) feedbackText.text = "Tap the sink to scrub hands!";
        }

        public void OnWashHandsClicked()
        {
            // Transition to dedicated Handwash Screen
            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.HandwashScreen);
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
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Wipe");
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
            }

            if (sinkImage != null && sinkCleanSprite != null) sinkImage.sprite = sinkCleanSprite;
            if (U8_GameManager_Masters_Activity.Instance != null) U8_GameManager_Masters_Activity.Instance.isSinkWiped = true;

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
