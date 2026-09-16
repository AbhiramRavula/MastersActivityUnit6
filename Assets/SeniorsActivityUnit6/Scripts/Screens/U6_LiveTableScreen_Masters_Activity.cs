using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    [System.Serializable]
    public class U6_WaitingEventData_Masters_Activity
    {
        public string eventName;
        public string situationPrompt;
        public string actionButtonLabel;
        public string successFeedback;
        public string failFeedback;
        public string sfxOnFail;
        public string voOnFail;
        public Sprite fidgetSprite;
    }

    public class U6_LiveTableScreen_Masters_Activity : MonoBehaviour
    {
        [Header("UI & Prompts")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI actionButtonText;
        [SerializeField] private Slider timeRemainingSlider;
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Anu Character Visual")]
        [SerializeField] private Image anuCharacterImage;
        [SerializeField] private Sprite anuSittingStraightSprite;

        [Header("Other Tables Reactions")]
        [SerializeField] private GameObject[] otherTablesNormal;
        [SerializeField] private GameObject[] otherTablesLooking;

        [Header("Waiting Events Configuration")]
        [SerializeField] private List<U6_WaitingEventData_Masters_Activity> waitingEvents = new List<U6_WaitingEventData_Masters_Activity>();
        [SerializeField] private float eventDuration = 6.5f;

        private int currentEventIndex = 0;
        private bool isEventActive = false;
        private Coroutine activeEventCoroutine;

        private void Awake()
        {
            if (actionButton != null)
                actionButton.onClick.AddListener(OnActionButtonClicked);
        }

        private void OnEnable()
        {
            InitializeDefaultEvents();
            StartCoroutine(InitAndStartSequence());
        }

        private void OnDisable()
        {
            if (activeEventCoroutine != null) StopCoroutine(activeEventCoroutine);
        }

        private void InitializeDefaultEvents()
        {
            if (waitingEvents.Count == 0)
            {
                waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
                {
                    eventName = "Fish Tank",
                    situationPrompt = "Anu spots a fish tank and starts sliding off her chair!",
                    actionButtonLabel = "STAY SEATED",
                    successFeedback = "Good job! Anu stays safely in her seat.",
                    failFeedback = "Anu ran across! The waiter had to swerve around her.",
                    sfxOnFail = "",
                    voOnFail = ""
                });

                waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
                {
                    eventName = "Glass Tapping",
                    situationPrompt = "Anu picks up a spoon and starts tapping the glass!",
                    actionButtonLabel = "PUT SPOON DOWN",
                    successFeedback = "Nice! The table remains quiet and polite.",
                    failFeedback = "Ting ting ting! The other tables turn and stare.",
                    sfxOnFail = "SFX_GlassTing",
                    voOnFail = ""
                });

                waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
                {
                    eventName = "Hungry Shout",
                    situationPrompt = "Anu is getting impatient and wants to shout how hungry she is!",
                    actionButtonLabel = "WAIT QUIETLY",
                    successFeedback = "Great patience! Food is being prepared.",
                    failFeedback = "\"I am SO hungry! Where is my food?\" Mother looks embarrassed.",
                    sfxOnFail = "",
                    voOnFail = "VO_U6_ANU_7"
                });

                waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
                {
                    eventName = "Kneeling on Chair",
                    situationPrompt = "Anu kneels up on the chair with feet underneath! The chair is wobbling.",
                    actionButtonLabel = "FEET ON FLOOR",
                    successFeedback = "Both feet on the floor! Sitting straight and safe.",
                    failFeedback = "The chair wobbled and nearly tipped over!",
                    sfxOnFail = "SFX_ChairWobble",
                    voOnFail = ""
                });
            }
        }

        private IEnumerator InitAndStartSequence()
        {
            // Wait 1 frame so AudioManager is guaranteed initialized
            yield return null;

            currentEventIndex = 0;
            SetOtherTablesLooking(false);
            SetAnuSprite(anuSittingStraightSprite);

            if (promptText) promptText.text = "Today Anu's family is eating out at a restaurant...";
            if (actionButton) actionButton.gameObject.SetActive(false);
            if (feedbackPanel) feedbackPanel.SetActive(false);

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayBGM("BGM_Main");
                U6_AudioManager_Masters_Activity.Instance.PlayAmbience("AMB_Restaurant");
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_01"); // "Today Anu's family is eating out."
            }

            yield return new WaitForSeconds(3.0f);

            if (promptText) promptText.text = "The food is not here yet. Watch Anu...";
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_02"); // "The food is not here yet. Watch Anu."
            }

            yield return new WaitForSeconds(2.8f);

            StartNextEvent();
        }

        private void StartNextEvent()
        {
            if (currentEventIndex >= waitingEvents.Count)
            {
                StartCoroutine(CompletePart1Sequence());
                return;
            }

            U6_WaitingEventData_Masters_Activity currentEvt = waitingEvents[currentEventIndex];
            if (promptText) promptText.text = currentEvt.situationPrompt;
            if (actionButtonText) actionButtonText.text = currentEvt.actionButtonLabel;
            if (actionButton) actionButton.gameObject.SetActive(true);
            if (feedbackPanel) feedbackPanel.SetActive(false);

            // Show Anu doing the fidget
            if (currentEvt.fidgetSprite != null)
            {
                SetAnuSprite(currentEvt.fidgetSprite);
            }
            else
            {
                SetAnuSprite(anuSittingStraightSprite);
            }

            SetOtherTablesLooking(false);

            // Play situation sound cues
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                if (currentEventIndex == 0)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_03"); // "Quick! Tap the button!"
                }
                else if (currentEvt.eventName.Contains("Glass"))
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_GlassTing");
                }
                else if (currentEvt.eventName.Contains("Kneel"))
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_ChairWobble");
                }
            }

            if (activeEventCoroutine != null) StopCoroutine(activeEventCoroutine);
            activeEventCoroutine = StartCoroutine(EventTimerRoutine());
        }

        private IEnumerator EventTimerRoutine()
        {
            isEventActive = true;
            float duration = 10.0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (timeRemainingSlider != null)
                    timeRemainingSlider.value = 1f - (elapsed / duration);
                yield return null;
            }

            OnEventFailed();
        }

        private void OnActionButtonClicked()
        {
            if (!isEventActive) return;
            isEventActive = false;

            if (activeEventCoroutine != null) StopCoroutine(activeEventCoroutine);
            StartCoroutine(ShowEventFeedback(true));
        }

        private void OnEventFailed()
        {
            isEventActive = false;
            StartCoroutine(ShowEventFeedback(false));
        }

        private IEnumerator ShowEventFeedback(bool success)
        {
            U6_WaitingEventData_Masters_Activity currentEvt = waitingEvents[currentEventIndex];
            if (actionButton) actionButton.gameObject.SetActive(false);

            if (feedbackPanel) feedbackPanel.SetActive(true);
            if (feedbackText) feedbackText.text = success ? currentEvt.successFeedback : currentEvt.failFeedback;

            if (success)
            {
                // Anu sits straight politely
                SetAnuSprite(anuSittingStraightSprite);
                SetOtherTablesLooking(false);
                if (U6_AudioManager_Masters_Activity.Instance != null)
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");

                yield return new WaitForSeconds(2.5f);
                currentEventIndex++;
                StartNextEvent();
            }
            else
            {
                // Failed - other tables look over
                SetOtherTablesLooking(true);
                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    if (!string.IsNullOrEmpty(currentEvt.sfxOnFail))
                        U6_AudioManager_Masters_Activity.Instance.PlaySFX(currentEvt.sfxOnFail);
                    if (!string.IsNullOrEmpty(currentEvt.voOnFail))
                        U6_AudioManager_Masters_Activity.Instance.PlayVO(currentEvt.voOnFail);
                    else
                        U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_MUM_1"); // "Anu, quiet..."
                }

                // Give student time to see what happened, then retry this event
                yield return new WaitForSeconds(3.5f);
                SetOtherTablesLooking(false);
                SetAnuSprite(anuSittingStraightSprite);
                if (feedbackPanel) feedbackPanel.SetActive(false);
                StartNextEvent();
            }
        }

        private void SetAnuSprite(Sprite sp)
        {
            if (anuCharacterImage != null && sp != null)
            {
                anuCharacterImage.sprite = sp;
                anuCharacterImage.preserveAspect = true;
                anuCharacterImage.enabled = true;
            }
        }

        private void SetOtherTablesLooking(bool looking)
        {
            if (otherTablesNormal != null)
            {
                foreach (var go in otherTablesNormal)
                    if (go) go.SetActive(!looking);
            }
            if (otherTablesLooking != null)
            {
                foreach (var go in otherTablesLooking)
                    if (go) go.SetActive(looking);
            }
        }

        private IEnumerator CompletePart1Sequence()
        {
            SetAnuSprite(anuSittingStraightSprite);
            if (promptText) promptText.text = "Well done! The family waited nicely.";
            if (feedbackPanel) feedbackPanel.SetActive(true);
            if (feedbackText) feedbackText.text = "Star 1 Earned!";

            U6_GameManager_Masters_Activity.Instance.AwardStar();

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_04"); // "Everybody is happy. One star!"
            }

            yield return new WaitForSeconds(3.5f);

            U6_GameManager_Masters_Activity.Instance.StartPart2();
        }
    }
}
