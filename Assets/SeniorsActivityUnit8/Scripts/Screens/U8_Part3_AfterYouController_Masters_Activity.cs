using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit8
{
    public class U8_Part3_AfterYouController_Masters_Activity : MonoBehaviour
    {
        [Header("UI Prompts & Action Buttons")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Button btnTryAgain;
        [SerializeField] private Button btnProceedToPart4;

        [Header("Meera Character Avatars & Poses")]
        [SerializeField] private Image meeraAvatar;
        [SerializeField] private Sprite meeraEnteringSprite;
        [SerializeField] private Sprite meeraSlippingSprite;
        [SerializeField] private Sprite meeraUnimpressedSprite;
        [SerializeField] private Sprite meeraHappySmileSprite;

        [Header("Washroom State Overlay Visuals")]
        [SerializeField] private GameObject wetFloorPuddleObject;
        [SerializeField] private GameObject soggyTowelFloorObject;
        [SerializeField] private GameObject runningWaterStreamObject;
        [SerializeField] private GameObject splashedSinkOverlay;
        [SerializeField] private GameObject cleanSinkSparkleOverlay;

        private bool isEvaluating = false;

        private void Awake()
        {
            AutoFindUIReferences();

            if (btnTryAgain != null) btnTryAgain.onClick.AddListener(OnTryAgainClicked);
            if (btnProceedToPart4 != null) btnProceedToPart4.onClick.AddListener(OnProceedToPart4Clicked);
        }

        private void OnEnable()
        {
            StartCoroutine(EvaluateMeeraEntrySequence());
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

            if (btnTryAgain == null)
            {
                Transform t = transform.Find("Btn_TryAgain") ?? transform.Find("BottomBar/Btn_TryAgain");
                if (t != null) btnTryAgain = t.GetComponent<Button>();
            }
            if (btnProceedToPart4 == null)
            {
                Transform t = transform.Find("Btn_ProceedToPart4") ?? transform.Find("Btn_Next") ?? transform.Find("BottomBar/Btn_Proceed");
                if (t != null) btnProceedToPart4 = t.GetComponent<Button>();
            }
            if (meeraAvatar == null)
            {
                Transform t = transform.Find("MeeraAvatar") ?? transform.Find("Meera_Image");
                if (t != null) meeraAvatar = t.GetComponent<Image>();
            }
        }

        private IEnumerator EvaluateMeeraEntrySequence()
        {
            isEvaluating = true;
            if (btnTryAgain != null) btnTryAgain.gameObject.SetActive(false);
            if (btnProceedToPart4 != null) btnProceedToPart4.gameObject.SetActive(false);

            // Phase 1: 3-Second Camera Pause on Empty Washroom
            if (meeraAvatar != null) meeraAvatar.gameObject.SetActive(false);
            if (promptText != null) promptText.text = "Anu has left the washroom...";
            if (feedbackText != null) feedbackText.text = "";

            var gm = U8_GameManager_Masters_Activity.Instance;
            bool isAllClean = (gm != null && gm.AreAllStepsCompleted());

            // Set up room objects based on state
            if (wetFloorPuddleObject != null) wetFloorPuddleObject.SetActive(!isAllClean && (!gm.isSinkWiped || !gm.isTapTurnedOff));
            if (soggyTowelFloorObject != null) soggyTowelFloorObject.SetActive(!isAllClean && !gm.isTowelInBin);
            if (runningWaterStreamObject != null) runningWaterStreamObject.SetActive(!isAllClean && !gm.isTapTurnedOff);
            if (splashedSinkOverlay != null) splashedSinkOverlay.SetActive(!isAllClean && !gm.isSinkWiped);
            if (cleanSinkSparkleOverlay != null) cleanSinkSparkleOverlay.SetActive(isAllClean);

            yield return new WaitForSeconds(2.0f);

            // Phase 2: Meera Enters
            if (promptText != null) promptText.text = "Here comes Meera!";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_12"); // "Here comes Meera."
            }

            yield return new WaitForSeconds(1.2f);

            if (meeraAvatar != null)
            {
                meeraAvatar.gameObject.SetActive(true);
                if (meeraEnteringSprite != null) meeraAvatar.sprite = meeraEnteringSprite;
            }

            yield return new WaitForSeconds(1.0f);

            // Phase 3: Evaluate Consequence
            if (isAllClean)
            {
                // Clean Outcome
                if (meeraAvatar != null && meeraHappySmileSprite != null) meeraAvatar.sprite = meeraHappySmileSprite;
                if (promptText != null) promptText.text = "The washroom is clean, dry, and welcoming!";
                if (feedbackText != null) feedbackText.text = "Meera uses the sink happily. Star 2 Earned!";

                if (U8_AudioManager_Masters_Activity.Instance != null)
                {
                    U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                }

                if (gm != null) gm.AwardStar();

                yield return new WaitForSeconds(2.0f);
                if (btnProceedToPart4 != null) btnProceedToPart4.gameObject.SetActive(true);
            }
            else
            {
                // Messy / Slip Consequence Outcome
                if (meeraAvatar != null && meeraSlippingSprite != null)
                {
                    meeraAvatar.sprite = meeraSlippingSprite;
                    // Slide / Slip shake animation
                    Vector3 origPos = meeraAvatar.transform.localPosition;
                    for (int i = 0; i < 6; i++)
                    {
                        meeraAvatar.transform.localPosition = origPos + new Vector3(Random.Range(-12f, 12f), Random.Range(-6f, 6f), 0);
                        yield return new WaitForSeconds(0.04f);
                    }
                    meeraAvatar.transform.localPosition = origPos;
                }

                if (U8_AudioManager_Masters_Activity.Instance != null)
                {
                    U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Slip");
                    U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_MEE_2"); // Startled gasp
                }

                if (promptText != null) promptText.text = "Oops! Meera slipped on the wet floor!";
                if (feedbackText != null) feedbackText.text = "Water on the floor is dangerous! The room was left untidy.";

                yield return new WaitForSeconds(2.0f);

                if (meeraAvatar != null && meeraUnimpressedSprite != null) meeraAvatar.sprite = meeraUnimpressedSprite;

                if (U8_AudioManager_Masters_Activity.Instance != null)
                {
                    U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_13"); // "Oh dear. Shall we try again?"
                }

                yield return new WaitForSeconds(1.0f);

                if (btnTryAgain != null) btnTryAgain.gameObject.SetActive(true);
            }

            isEvaluating = false;
        }

        private void OnTryAgainClicked()
        {
            if (isEvaluating) return;

            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ResetUnitState();
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.Part2_Inside);
            }
        }

        private void OnProceedToPart4Clicked()
        {
            if (isEvaluating) return;

            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.Part4_EmptySoap);
            }
        }
    }
}
