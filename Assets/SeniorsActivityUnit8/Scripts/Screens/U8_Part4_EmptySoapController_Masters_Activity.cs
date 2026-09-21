using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit8
{
    public class U8_Part4_EmptySoapController_Masters_Activity : MonoBehaviour
    {
        [Header("UI Prompts & Feedback")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Choice Buttons")]
        [SerializeField] private Button btnTellTeacher;
        [SerializeField] private Button btnJustLeave;
        [SerializeField] private Button btnFinishUnit;

        [Header("Visual Elements")]
        [SerializeField] private Image meeraAvatar;
        [SerializeField] private Sprite meeraPumpingSoapSprite;
        [SerializeField] private Sprite meeraShruggingSprite;
        [SerializeField] private Sprite meeraHappySprite;
        [SerializeField] private Image soapDispenserImage;
        [SerializeField] private Sprite soapEmptySprite;
        [SerializeField] private Sprite soapFullRefilledSprite;
        [SerializeField] private GameObject otherChildrenUsingSoapOverlay;

        private bool isProcessingChoice = false;

        private void Awake()
        {
            AutoFindUIReferences();

            if (btnTellTeacher != null) btnTellTeacher.onClick.AddListener(OnTellTeacherClicked);
            if (btnJustLeave != null) btnJustLeave.onClick.AddListener(OnJustLeaveClicked);
            if (btnFinishUnit != null) btnFinishUnit.onClick.AddListener(OnFinishUnitClicked);
        }

        private void OnEnable()
        {
            AutoFindUIReferences();
            StartCoroutine(InitSoapEmptyRoutine());
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

            if (btnTellTeacher == null)
            {
                Transform t = transform.Find("ChoiceContainer/Btn_TellTeacher") ?? transform.Find("Btn_TellTeacher");
                if (t != null) btnTellTeacher = t.GetComponent<Button>();
            }
            if (btnJustLeave == null)
            {
                Transform t = transform.Find("ChoiceContainer/Btn_JustLeave") ?? transform.Find("Btn_JustLeave");
                if (t != null) btnJustLeave = t.GetComponent<Button>();
            }
            if (btnFinishUnit == null)
            {
                var allButtons = GetComponentsInChildren<Button>(true);
                foreach (var b in allButtons)
                {
                    if (b != null && (b.name.IndexOf("Finish", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                                      b.name.IndexOf("Proceed", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                                      b.name.IndexOf("Next", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                                      b.name.IndexOf("Done", System.StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        btnFinishUnit = b;
                        break;
                    }
                }
            }
            if (meeraAvatar == null)
            {
                Transform t = transform.Find("MeeraAvatar") ?? transform.Find("Meera_Image");
                if (t != null) meeraAvatar = t.GetComponent<Image>();
            }
            if (meeraAvatar != null)
            {
                meeraAvatar.preserveAspect = true;
                if (meeraPumpingSoapSprite != null && meeraAvatar.sprite == null) meeraAvatar.sprite = meeraPumpingSoapSprite;
                meeraAvatar.SetNativeSize();
            }
            if (soapDispenserImage == null)
            {
                Transform t = transform.Find("SoapDispenserImage") ?? transform.Find("SoapDispenser") ?? transform.Find("Soap");
                if (t != null) soapDispenserImage = t.GetComponent<Image>();
            }
        }

        private IEnumerator InitSoapEmptyRoutine()
        {
            isProcessingChoice = false;
            if (btnFinishUnit != null) btnFinishUnit.gameObject.SetActive(false);
            if (otherChildrenUsingSoapOverlay != null) otherChildrenUsingSoapOverlay.SetActive(false);
            if (soapDispenserImage != null && soapEmptySprite != null)
            {
                soapDispenserImage.sprite = soapEmptySprite;
                soapDispenserImage.gameObject.SetActive(true);
            }

            if (meeraAvatar != null && meeraPumpingSoapSprite != null)
            {
                meeraAvatar.sprite = meeraPumpingSoapSprite;
                meeraAvatar.preserveAspect = true;
                meeraAvatar.SetNativeSize();
            }
            if (promptText != null) promptText.text = "Meera presses the soap dispenser...";

            yield return new WaitForSeconds(0.6f);

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_SoapPump");
                yield return new WaitForSeconds(0.3f);
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_SoapEmpty");
            }

            if (feedbackText != null) feedbackText.text = "Nothing comes out! The soap is finished.";

            yield return new WaitForSeconds(1.2f);

            if (promptText != null) promptText.text = "The soap is finished. What should Meera do?";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_14"); // "The soap is finished. What should Meera do?"
            }

            SetButtonsInteractable(true);
        }

        private void SetButtonsInteractable(bool state)
        {
            if (btnTellTeacher != null) btnTellTeacher.interactable = state;
            if (btnJustLeave != null) btnJustLeave.interactable = state;
        }

        private void OnTellTeacherClicked()
        {
            if (isProcessingChoice) return;
            StartCoroutine(TellTeacherRoutine());
        }

        private IEnumerator TellTeacherRoutine()
        {
            isProcessingChoice = true;
            SetButtonsInteractable(false);

            if (feedbackText != null) feedbackText.text = "Meera finds a teacher: 'Ma'am, the soap is finished.'";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_MEE_1"); // "Ma'am, the soap is finished."
            }

            yield return new WaitForSeconds(2.0f);

            // 1. Refill dispenser with SPR_Soap_Full and punchy scale pop
            if (soapDispenserImage != null)
            {
                if (soapFullRefilledSprite != null) soapDispenserImage.sprite = soapFullRefilledSprite;
                soapDispenserImage.gameObject.SetActive(true);

                // Pop scale animation
                Vector3 origScale = soapDispenserImage.transform.localScale;
                for (float t = 0; t < 1f; t += Time.deltaTime * 5f)
                {
                    soapDispenserImage.transform.localScale = origScale * Mathf.Lerp(1.3f, 1.0f, t);
                    yield return null;
                }
                soapDispenserImage.transform.localScale = origScale;
            }

            if (otherChildrenUsingSoapOverlay != null) otherChildrenUsingSoapOverlay.SetActive(true);
            if (meeraAvatar != null && meeraHappySprite != null)
            {
                meeraAvatar.sprite = meeraHappySprite;
                meeraAvatar.preserveAspect = true;
                meeraAvatar.SetNativeSize();
            }

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
            }

            if (feedbackText != null) feedbackText.text = "The soap is refilled full! Other children can wash their hands cleanly. Star 3 Earned!";
            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.AwardStar();
            }

            yield return new WaitForSeconds(1.5f);
            if (btnFinishUnit != null)
            {
                btnFinishUnit.gameObject.SetActive(true);
                btnFinishUnit.transform.SetAsLastSibling();
                btnFinishUnit.onClick.RemoveListener(OnFinishUnitClicked);
                btnFinishUnit.onClick.AddListener(OnFinishUnitClicked);
            }
            else
            {
                // Fallback auto-proceed if no button exists
                yield return new WaitForSeconds(2.0f);
                OnFinishUnitClicked();
            }
        }

        private void OnJustLeaveClicked()
        {
            if (isProcessingChoice) return;
            StartCoroutine(JustLeaveRoutine());
        }

        private IEnumerator JustLeaveRoutine()
        {
            isProcessingChoice = true;
            SetButtonsInteractable(false);

            if (meeraAvatar != null && meeraShruggingSprite != null)
            {
                meeraAvatar.sprite = meeraShruggingSprite;
                meeraAvatar.preserveAspect = true;
                meeraAvatar.SetNativeSize();
            }
            if (feedbackText != null) feedbackText.text = "Meera shrugs and walks away...";

            yield return new WaitForSeconds(1.5f);

            if (feedbackText != null) feedbackText.text = "Three more children try the empty soap and leave in frustration.";

            yield return new WaitForSeconds(2.5f);

            if (feedbackText != null) feedbackText.text = "If we tell a teacher, it fixes the problem for everybody!";
            yield return new WaitForSeconds(1.5f);

            isProcessingChoice = false;
            SetButtonsInteractable(true);
        }

        private void OnFinishUnitClicked()
        {
            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.Ending);
            }
        }
    }
}
