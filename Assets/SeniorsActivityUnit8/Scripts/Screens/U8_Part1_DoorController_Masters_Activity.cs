using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit8
{
    public class U8_Part1_DoorController_Masters_Activity : MonoBehaviour
    {
        [Header("Door Visuals")]
        [SerializeField] private Image doorImage;
        [SerializeField] private Sprite doorClosedSprite;
        [SerializeField] private Sprite doorOpenSprite;
        [SerializeField] private GameObject shutSignObject;

        [Header("Character Avatars")]
        [SerializeField] private Image anuAvatar;
        [SerializeField] private Sprite anuNormalSprite;
        [SerializeField] private Sprite anuKnockingSprite;
        [SerializeField] private Sprite anuSheepishSprite;

        [Header("UI Prompts & Feedback")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Choice Buttons")]
        [SerializeField] private Button btnKnockAndWait;
        [SerializeField] private Button btnBangLoudly;
        [SerializeField] private Button btnPushOpen;

        private bool isProcessingChoice = false;

        private void Awake()
        {
            AutoFindUIReferences();

            if (btnKnockAndWait != null) btnKnockAndWait.onClick.AddListener(OnKnockAndWaitClicked);
            if (btnBangLoudly != null) btnBangLoudly.onClick.AddListener(OnBangLoudlyClicked);
            if (btnPushOpen != null) btnPushOpen.onClick.AddListener(OnPushOpenClicked);
        }

        private void OnEnable()
        {
            ResetScreen();
            StartCoroutine(PlayScreen1OpeningVO());
        }

        private IEnumerator PlayScreen1OpeningVO()
        {
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_01"); // "Anu needs the washroom."
                yield return new WaitForSeconds(1.8f);
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_02"); // "The door is closed. What should Anu do?"
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

            if (btnKnockAndWait == null)
            {
                Transform t = transform.Find("Btn_Knock") ?? transform.Find("ChoiceContainer/Btn_Knock") ?? transform.Find("Buttons/Btn_Knock");
                if (t != null) btnKnockAndWait = t.GetComponent<Button>();
            }
            if (btnBangLoudly == null)
            {
                Transform t = transform.Find("Btn_Bang") ?? transform.Find("ChoiceContainer/Btn_Bang") ?? transform.Find("Buttons/Btn_Bang");
                if (t != null) btnBangLoudly = t.GetComponent<Button>();
            }
            if (btnPushOpen == null)
            {
                Transform t = transform.Find("Btn_Push") ?? transform.Find("ChoiceContainer/Btn_Push") ?? transform.Find("Buttons/Btn_Push");
                if (t != null) btnPushOpen = t.GetComponent<Button>();
            }
            if (doorImage == null)
            {
                Transform t = transform.Find("DoorImage") ?? transform.Find("WashroomDoor");
                if (t != null) doorImage = t.GetComponent<Image>();
            }
            if (anuAvatar == null)
            {
                Transform t = transform.Find("AnuAvatar") ?? transform.Find("Anu_Image");
                if (t != null) anuAvatar = t.GetComponent<Image>();
            }
            if (anuAvatar != null)
            {
                anuAvatar.preserveAspect = true;
                if (anuNormalSprite != null && anuAvatar.sprite == null) anuAvatar.sprite = anuNormalSprite;
                anuAvatar.SetNativeSize();
            }
        }

        public void ResetScreen()
        {
            isProcessingChoice = false;
            if (doorImage != null && doorClosedSprite != null) doorImage.sprite = doorClosedSprite;
            if (shutSignObject != null) shutSignObject.SetActive(true);
            if (anuAvatar != null)
            {
                anuAvatar.preserveAspect = true;
                if (anuNormalSprite != null) anuAvatar.sprite = anuNormalSprite;
                anuAvatar.SetNativeSize();
            }

            if (promptText != null) promptText.text = "The door is closed. What should Anu do?";
            if (feedbackText != null) feedbackText.text = "";

            SetButtonsInteractable(true);
        }

        private void SetButtonsInteractable(bool state)
        {
            if (btnKnockAndWait != null) btnKnockAndWait.interactable = state;
            if (btnBangLoudly != null) btnBangLoudly.interactable = state;
            if (btnPushOpen != null) btnPushOpen.interactable = state;
        }

        private void OnKnockAndWaitClicked()
        {
            if (isProcessingChoice) return;
            StartCoroutine(KnockAndWaitRoutine());
        }

        private IEnumerator KnockAndWaitRoutine()
        {
            isProcessingChoice = true;
            SetButtonsInteractable(false);

            if (anuAvatar != null && anuKnockingSprite != null)
            {
                anuAvatar.sprite = anuKnockingSprite;
                anuAvatar.preserveAspect = true;
                anuAvatar.SetNativeSize();
            }
            if (feedbackText != null) feedbackText.text = "Knock, knock! Anu waits patiently...";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Knock");
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_03"); // "Knock, and wait."
            }

            yield return new WaitForSeconds(1.8f);

            // Door opens
            if (anuAvatar != null && anuNormalSprite != null)
            {
                anuAvatar.sprite = anuNormalSprite;
                anuAvatar.preserveAspect = true;
                anuAvatar.SetNativeSize();
            }
            if (doorImage != null && doorOpenSprite != null) doorImage.sprite = doorOpenSprite;
            if (shutSignObject != null) shutSignObject.SetActive(false);
            if (feedbackText != null) feedbackText.text = "The other student comes out. Anu goes in!";

            yield return new WaitForSeconds(1.5f);

            // Transition to Part 2
            if (U8_GameManager_Masters_Activity.Instance != null)
            {
                U8_GameManager_Masters_Activity.Instance.ShowPart(U8_GamePart.Part2_Inside);
            }
        }

        private void OnBangLoudlyClicked()
        {
            if (isProcessingChoice) return;
            StartCoroutine(BangLoudlyRoutine());
        }

        private IEnumerator BangLoudlyRoutine()
        {
            isProcessingChoice = true;
            SetButtonsInteractable(false);

            if (feedbackText != null) feedbackText.text = "BANG! BANG! BANG!";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_BangDoor");
            }

            yield return new WaitForSeconds(0.6f);

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_VOICE_1"); // "Just a minute!" (muffled)
            }

            if (anuAvatar != null && anuSheepishSprite != null)
            {
                anuAvatar.sprite = anuSheepishSprite;
                anuAvatar.preserveAspect = true;
                anuAvatar.SetNativeSize();
            }
            if (feedbackText != null) feedbackText.text = "Someone is inside: 'Just a minute!' Anu feels sheepish.";

            yield return new WaitForSeconds(2.0f);

            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayVO("VO_U8_ANU_1"); // "Sorry!"
            }

            yield return new WaitForSeconds(1.5f);

            // Reset for retry
            if (anuAvatar != null && anuNormalSprite != null)
            {
                anuAvatar.sprite = anuNormalSprite;
                anuAvatar.preserveAspect = true;
                anuAvatar.SetNativeSize();
            }
            if (feedbackText != null) feedbackText.text = "Let's try again with polite manners!";
            isProcessingChoice = false;
            SetButtonsInteractable(true);
        }

        private void OnPushOpenClicked()
        {
            if (isProcessingChoice) return;
            StartCoroutine(PushOpenRoutine());
        }

        private IEnumerator PushOpenRoutine()
        {
            isProcessingChoice = true;
            SetButtonsInteractable(false);

            if (feedbackText != null) feedbackText.text = "Anu pushes the door...";
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_BoltRattle");
            }

            // Small shake effect on door
            if (doorImage != null)
            {
                Vector3 originalPos = doorImage.transform.localPosition;
                for (int i = 0; i < 4; i++)
                {
                    doorImage.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
                    yield return new WaitForSeconds(0.05f);
                }
                doorImage.transform.localPosition = originalPos;
            }

            if (feedbackText != null) feedbackText.text = "It is bolted! Closed doors require patience.";

            yield return new WaitForSeconds(2.0f);

            isProcessingChoice = false;
            SetButtonsInteractable(true);
        }
    }
}
