using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

#pragma warning disable 0414
        [Header("Audios & SFX Used On This Screen")]
        [SerializeField] private string ambRestaurant = "AMB_Restaurant (Background Diner Ambience)";
        [SerializeField] private string voIntro1 = "VO_U6_01 (Today Anu's family is eating out)";
        [SerializeField] private string voIntro2 = "VO_U6_02 (The food is not here yet. Watch Anu)";
        [SerializeField] private string voQuickTap = "VO_U6_03 (Quick! Tap the button!)";
        [SerializeField] private string sfxGlassTing = "SFX_GlassTing (Glass tapping fidget)";
        [SerializeField] private string sfxChairWobble = "SFX_ChairWobble (Chair kneeling fidget)";
        [SerializeField] private string voHungryShout = "VO_U6_ANU_7 (I am SO hungry!)";
        [SerializeField] private string voMomQuiet = "VO_U6_MUM_1 (Anu, quiet...)";
        [SerializeField] private string sfxSuccess = "SFX_Sparkle (Polite action success)";
        [SerializeField] private string voStar1Earned = "VO_U6_04 (Everybody is happy. One star!)";
#pragma warning restore 0414

        [Header("Waiting Events Configuration")]
        [SerializeField] private List<U6_WaitingEventData_Masters_Activity> waitingEvents = new List<U6_WaitingEventData_Masters_Activity>();
        [SerializeField] private float eventDuration = 10.0f;

        private int currentEventIndex = 0;
        private bool isEventActive = false;
        private Coroutine activeEventCoroutine;

        private void Awake()
        {
            if (actionButton != null)
                actionButton.onClick.AddListener(OnActionButtonClicked);

            AutoFindUIReferences();
        }

        private void OnEnable()
        {
            AutoFindUIReferences();
#if UNITY_EDITOR
            AutoLoadSpritesAndEvents();
#else
            InitializeDefaultEvents();
#endif
            StartCoroutine(InitAndStartSequence());
        }

        private void AutoFindUIReferences()
        {
            if (promptText == null) promptText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (actionButton == null) actionButton = GetComponentInChildren<Button>(true);
            if (actionButtonText == null && actionButton != null) actionButtonText = actionButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (timeRemainingSlider == null) timeRemainingSlider = GetComponentInChildren<Slider>(true);
            
            if (feedbackPanel == null)
            {
                Transform fb = transform.Find("FeedbackPanel");
                if (fb != null) feedbackPanel = fb.gameObject;
            }
            if (feedbackText == null && feedbackPanel != null) feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>(true);

            if (anuCharacterImage == null)
            {
                Transform t = transform.Find("AnuAvatar") 
                           ?? transform.Find("AnuCharacterImage") 
                           ?? transform.Find("AnuVisual")
                           ?? transform.Find("Anu");
                if (t != null) anuCharacterImage = t.GetComponent<Image>();
                if (anuCharacterImage == null)
                {
                    Image[] imgs = GetComponentsInChildren<Image>(true);
                    foreach (var img in imgs)
                    {
                        if (img.gameObject.name.IndexOf("Anu", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            anuCharacterImage = img;
                            break;
                        }
                    }
                }
            }

            // Auto-discover Normal Diners vs Looking Diners (supports Dinner_look1, Dinner_look2, Diners_Looking, etc.)
            if (otherTablesLooking == null || otherTablesLooking.Length == 0)
            {
                List<GameObject> lookings = new List<GameObject>();
                foreach (Transform child in transform)
                {
                    string name = child.name.ToLower();
                    if (name.Contains("look") || name.Contains("lokk") || name.Contains("turn") || name.Contains("stare") || name.Contains("react"))
                    {
                        lookings.Add(child.gameObject);
                    }
                }
                if (lookings.Count > 0) otherTablesLooking = lookings.ToArray();
            }

            if (otherTablesNormal == null || otherTablesNormal.Length == 0)
            {
                List<GameObject> normals = new List<GameObject>();
                foreach (Transform child in transform)
                {
                    string name = child.name.ToLower();
                    if ((name.Contains("diner") || name.Contains("dinner") || name.Contains("table")) && !name.Contains("look") && !name.Contains("lokk") && !name.Contains("turn") && !name.Contains("stare") && !name.Contains("react"))
                    {
                        normals.Add(child.gameObject);
                    }
                }
                if (normals.Count > 0) otherTablesNormal = normals.ToArray();
            }
        }

        private void OnDisable()
        {
            if (activeEventCoroutine != null) StopCoroutine(activeEventCoroutine);
        }

#if UNITY_EDITOR
        private void AutoLoadSpritesAndEvents()
        {
            Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>(System.StringComparer.OrdinalIgnoreCase);
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/SeniorsActivityUnit6/Art" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object obj in allObjects)
                {
                    if (obj is Sprite sp && !spriteDict.ContainsKey(sp.name))
                    {
                        spriteDict[sp.name] = sp;
                    }
                }
            }

            Sprite GetSprite(string name)
            {
                if (spriteDict.TryGetValue(name, out Sprite s)) return s;
                foreach (var kvp in spriteDict)
                {
                    if (kvp.Key.IndexOf(name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        return kvp.Value;
                }
                return null;
            }

            anuSittingStraightSprite = GetSprite("SPR_Anu_SittingStraight");
            Sprite fidgetFish = GetSprite("SPR_Anu_SlidingChair");
            Sprite fidgetGlass = GetSprite("SPR_Anu_TappingGlass");
            Sprite fidgetHungry = GetSprite("SPR_Anu_ShoutingHungry");
            Sprite fidgetKneel = GetSprite("SPR_Anu_KneelingChair");

            waitingEvents.Clear();

            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Fish Tank",
                situationPrompt = "Anu spots a fish tank and starts sliding off her chair!",
                actionButtonLabel = "STAY SEATED",
                successFeedback = "Good job! Anu stays safely in her seat.",
                failFeedback = "Anu ran across! The waiter had to swerve around her.",
                sfxOnFail = "",
                voOnFail = "",
                fidgetSprite = fidgetFish
            });

            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Glass Tapping",
                situationPrompt = "Anu picks up a spoon and starts tapping the glass!",
                actionButtonLabel = "PUT SPOON DOWN",
                successFeedback = "Nice! The table remains quiet and polite.",
                failFeedback = "Ting ting ting! The other tables turn and stare.",
                sfxOnFail = "SFX_GlassTing",
                voOnFail = "",
                fidgetSprite = fidgetGlass
            });

            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Hungry Shout",
                situationPrompt = "Anu is getting impatient and wants to shout how hungry she is!",
                actionButtonLabel = "WAIT QUIETLY",
                successFeedback = "Great patience! Food is being prepared.",
                failFeedback = "\"I am SO hungry! Where is my food?\" Mother looks embarrassed.",
                sfxOnFail = "",
                voOnFail = "VO_U6_ANU_7",
                fidgetSprite = fidgetHungry
            });

            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Kneeling on Chair",
                situationPrompt = "Anu kneels up on the chair with feet underneath! The chair is wobbling.",
                actionButtonLabel = "FEET ON FLOOR",
                successFeedback = "Both feet on the floor! Sitting straight and safe.",
                failFeedback = "The chair wobbled and nearly tipped over!",
                sfxOnFail = "SFX_ChairWobble",
                voOnFail = "",
                fidgetSprite = fidgetKneel
            });

            Debug.Log($"[U6_LiveTableScreen] Auto-Loaded fidget sprites: Fish={fidgetFish?.name}, Glass={fidgetGlass?.name}, Hungry={fidgetHungry?.name}, Kneel={fidgetKneel?.name}");
        }
#endif

        private void InitializeDefaultEvents()
        {
            if (waitingEvents == null || waitingEvents.Count == 0)
            {
                waitingEvents = new List<U6_WaitingEventData_Masters_Activity>();
            }
        }

        private IEnumerator InitAndStartSequence()
        {
            // Wait until AudioManager instance is ready
            int retries = 0;
            while (U6_AudioManager_Masters_Activity.Instance == null && retries < 15)
            {
                yield return null;
                retries++;
            }

            var audioMgr = U6_AudioManager_Masters_Activity.Instance ?? FindFirstObjectByType<U6_AudioManager_Masters_Activity>();

            currentEventIndex = 0;
            SetOtherTablesLooking(false);
            SetAnuSprite(anuSittingStraightSprite);

            if (promptText) promptText.text = "Today Anu's family is eating out at a restaurant...";
            if (actionButton) actionButton.gameObject.SetActive(false);
            if (feedbackPanel) feedbackPanel.SetActive(false);

            if (audioMgr != null)
            {
                audioMgr.PlayAmbience("AMB_Restaurant");
                audioMgr.PlayVO("VO_U6_01"); // "Today Anu's family is eating out."
            }

            yield return new WaitForSeconds(3.0f);

            if (promptText) promptText.text = "The food is not here yet. Watch Anu...";
            if (audioMgr != null)
            {
                audioMgr.PlayVO("VO_U6_02"); // "The food is not here yet. Watch Anu."
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

            // When Anu makes noise / fidgets, other diners turn around to look!
            SetOtherTablesLooking(true);

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
            float duration = eventDuration > 0 ? eventDuration : 10.0f;
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
            if (anuCharacterImage == null) AutoFindUIReferences();

            if (anuCharacterImage != null && sp != null)
            {
                anuCharacterImage.sprite = sp;
                anuCharacterImage.preserveAspect = true;
                anuCharacterImage.enabled = true;
                StartCoroutine(PopAvatarAnimation(anuCharacterImage.transform));
                Debug.Log($"[U6_LiveTableScreen] Anu Sprite set to: '{sp.name}'");
            }
        }

        private IEnumerator PopAvatarAnimation(Transform target)
        {
            if (target == null) yield break;
            Vector3 originalScale = Vector3.one;
            target.localScale = originalScale * 0.88f;

            float elapsed = 0f;
            float duration = 0.22f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f);
                target.localScale = Vector3.Lerp(originalScale * 0.88f, originalScale * 1.05f, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        private void SetOtherTablesLooking(bool looking)
        {
            if (otherTablesNormal == null || otherTablesNormal.Length == 0 || otherTablesLooking == null || otherTablesLooking.Length == 0)
            {
                AutoFindUIReferences();
            }

            if (otherTablesNormal != null)
            {
                foreach (var go in otherTablesNormal)
                {
                    if (go) go.SetActive(!looking);
                }
            }
            if (otherTablesLooking != null)
            {
                foreach (var go in otherTablesLooking)
                {
                    if (go)
                    {
                        go.SetActive(looking);
                        if (looking && gameObject.activeInHierarchy)
                        {
                            StartCoroutine(PopAvatarAnimation(go.transform));
                        }
                    }
                }
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
