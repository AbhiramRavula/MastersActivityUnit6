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

        [Header("Other Tables Reactions")]
        [SerializeField] private GameObject[] otherTablesNormal;
        [SerializeField] private GameObject[] otherTablesLooking;

        [Header("Waiting Events Configuration")]
        [SerializeField] private List<U6_WaitingEventData_Masters_Activity> waitingEvents = new List<U6_WaitingEventData_Masters_Activity>();
        [SerializeField] private float eventDuration = 5f;

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
            StartWaitingSequence();
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
                    actionButtonLabel = "PUT IT DOWN",
                    successFeedback = "Nice! The table remains quiet and polite.",
                    failFeedback = "*Ting ting ting!* The other tables turn and stare.",
                    sfxOnFail = "SFX_GlassTing",
                    voOnFail = ""
                });

                waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
                {
                    eventName = "Hungry Shout",
                    situationPrompt = "Anu is about to shout how hungry she is!",
                    actionButtonLabel = "WAIT QUIETLY",
                    successFeedback = "Great patience! Food is being prepared.",
                    failFeedback = "\"I am SO hungry! Where is my food?\" Mother looks embarrassed.",
                    sfxOnFail = "",
                    voOnFail = "VO_U6_ANU_7"
                });

                waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
                {
                    eventName = "Kneeling on Chair",
                    situationPrompt = "Anu kneels up on the chair with feet underneath!",
                    actionButtonLabel = "FEET ON FLOOR",
                    successFeedback = "Both feet on the floor! Sitting straight.",
                    failFeedback = "The chair wobbled and nearly tipped over!",
                    sfxOnFail = "SFX_ChairWobble",
                    voOnFail = ""
                });
            }
        }

        private void StartWaitingSequence()
        {
            currentEventIndex = 0;
            SetOtherTablesLooking(false);

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayAmbience("AMB_Restaurant");
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_02"); // "The food is not here yet. Watch Anu."
            }

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

            SetOtherTablesLooking(false);

            if (activeEventCoroutine != null) StopCoroutine(activeEventCoroutine);
            activeEventCoroutine = StartCoroutine(EventTimerRoutine());
        }

        private IEnumerator EventTimerRoutine()
        {
            isEventActive = true;
            float elapsed = 0f;

            while (elapsed < eventDuration)
            {
                elapsed += Time.deltaTime;
                if (timeRemainingSlider != null)
                    timeRemainingSlider.value = 1f - (elapsed / eventDuration);
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
                SetOtherTablesLooking(false);
                if (U6_AudioManager_Masters_Activity.Instance != null)
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Chirp");
            }
            else
            {
                SetOtherTablesLooking(true);
                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    if (!string.IsNullOrEmpty(currentEvt.sfxOnFail))
                        U6_AudioManager_Masters_Activity.Instance.PlaySFX(currentEvt.sfxOnFail);
                    if (!string.IsNullOrEmpty(currentEvt.voOnFail))
                        U6_AudioManager_Masters_Activity.Instance.PlayVO(currentEvt.voOnFail);
                }
            }

            yield return new WaitForSeconds(3.5f);

            currentEventIndex++;
            StartNextEvent();
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
            if (promptText) promptText.text = "Well done! The family waited nicely.";
            if (feedbackPanel) feedbackPanel.SetActive(true);
            if (feedbackText) feedbackText.text = "★ Star 1 Earned!";

            U6_GameManager_Masters_Activity.Instance.AwardStar();

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_04"); // "Everybody is happy. One star!"
            }

            yield return new WaitForSeconds(3.0f);

            U6_GameManager_Masters_Activity.Instance.StartPart2();
        }
    }
}
