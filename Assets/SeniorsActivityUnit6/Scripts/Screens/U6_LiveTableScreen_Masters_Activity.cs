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
        [TextArea] public string situationPrompt;
        public Sprite correctPoseSprite;
        public Sprite incorrectPoseSprite;
        public Sprite tableVisualOnFail;
        public string successFeedback;
        public string failFeedback;
        public string sfxOnFail;
        public string sfxOnSuccess;
    }

    public class U6_LiveTableScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Title Banner (Shown Only at Beginning)")]
        [SerializeField] private GameObject titleBanner;
        [SerializeField] private float titleBannerDuration = 2.8f;
        private Coroutine hideTitleBannerCoroutine;

        [Header("UI & Prompts")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Button leftPoseButton;
        [SerializeField] private Button rightPoseButton;
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI actionButtonText;
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Card Borders")]
        [SerializeField] private GameObject borderImageLeft;
        [SerializeField] private GameObject borderImageRight;

        [Header("Anu Character Visual (Family Table)")]
        [SerializeField] private Image anuCharacterImage;
        [SerializeField] private Sprite anuSittingStraightSprite;

        [Header("Other Tables Reactions")]
        [SerializeField] private GameObject[] otherTablesNormal;
        [SerializeField] private GameObject[] otherTablesLooking;

        [Header("Waiting Events Configuration")]
        [SerializeField] private List<U6_WaitingEventData_Masters_Activity> waitingEvents = new List<U6_WaitingEventData_Masters_Activity>();

        private int currentEventIndex = 0;
        private bool leftIsCorrect = false;
        private bool isEventActive = false;
        private Coroutine pulseCardsCoroutine;
        private Vector3 initialAnuScale = Vector3.one;

        private void Awake()
        {
            AutoFindUIReferences();

            if (anuCharacterImage != null)
            {
                initialAnuScale = anuCharacterImage.transform.localScale;
                if (initialAnuScale == Vector3.zero) initialAnuScale = Vector3.one;
            }

            if (leftPoseButton != null)
            {
                leftPoseButton.onClick.RemoveAllListeners();
                leftPoseButton.onClick.AddListener(() => OnPoseSelected(true));
            }
            if (rightPoseButton != null)
            {
                rightPoseButton.onClick.RemoveAllListeners();
                rightPoseButton.onClick.AddListener(() => OnPoseSelected(false));
            }
            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() =>
                {
                    if (U6_AudioManager_Masters_Activity.Instance != null)
                    {
                        U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_03");
                    }
                });
            }

            SetPoseCardsActive(false);
        }

        private void OnEnable()
        {
            AutoFindUIReferences();
            if (anuCharacterImage != null)
            {
                if (initialAnuScale == Vector3.one && anuCharacterImage.transform.localScale != Vector3.zero)
                    initialAnuScale = anuCharacterImage.transform.localScale;
                anuCharacterImage.transform.localScale = initialAnuScale;
            }
            SetPoseCardsActive(false);
#if UNITY_EDITOR
            AutoLoadSpritesAndEvents();
#else
            InitializeDefaultEvents();
#endif
            if (titleBanner != null)
            {
                titleBanner.SetActive(true);
                CanvasGroup cg = titleBanner.GetComponent<CanvasGroup>();
                if (cg == null) cg = titleBanner.AddComponent<CanvasGroup>();
                cg.alpha = 1f;

                if (hideTitleBannerCoroutine != null) StopCoroutine(hideTitleBannerCoroutine);
                hideTitleBannerCoroutine = StartCoroutine(HideTitleBannerRoutine());
            }

            StartCoroutine(InitAndStartSequence());
        }

        private void OnDisable()
        {
            StopCardIdlePulse();
            StopAllCoroutines();
        }

        private IEnumerator HideTitleBannerRoutine()
        {
            if (titleBanner == null) yield break;
            yield return new WaitForSeconds(titleBannerDuration);

            CanvasGroup cg = titleBanner.GetComponent<CanvasGroup>();
            if (cg == null) cg = titleBanner.AddComponent<CanvasGroup>();

            float elapsed = 0f;
            float fadeDuration = 0.45f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                if (cg != null) cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            if (titleBanner != null)
            {
                titleBanner.SetActive(false);
                if (cg != null) cg.alpha = 1f;
            }
        }

        public void AutoFindUIReferences()
        {
            if (titleBanner == null)
            {
                Transform tb = transform.Find("TitleBanner") ?? transform.Find("SafeArea/TitleBanner");
                if (tb != null) titleBanner = tb.gameObject;
            }
            if (promptText == null) promptText = GetComponentInChildren<TextMeshProUGUI>(true);

            // Locate card borders if present
            if (borderImageLeft == null)
            {
                Transform t = transform.Find("SafeArea/Border_Image_Left") ?? transform.Find("Border_Image_Left");
                if (t != null) borderImageLeft = t.gameObject;
            }
            if (borderImageRight == null)
            {
                Transform t = transform.Find("SafeArea/Border_Image_Right") ?? transform.Find("Border_Image_Right");
                if (t != null) borderImageRight = t.gameObject;
            }

            // Locate or wire LeftPoseButton and RightPoseButton
            if (leftPoseButton == null || rightPoseButton == null)
            {
                Button[] btns = GetComponentsInChildren<Button>(true);
                foreach (var btn in btns)
                {
                    string bName = btn.name.ToLower();
                    if (leftPoseButton == null && (bName.Contains("left") || bName.Contains("pose1")))
                        leftPoseButton = btn;
                    else if (rightPoseButton == null && (bName.Contains("right") || bName.Contains("pose2")))
                        rightPoseButton = btn;
                }
            }

            // Fallback: search by path
            if (leftPoseButton == null)
            {
                Transform t = transform.Find("SafeArea/Border_Image_Left/LeftPoseButton") ??
                              transform.Find("Border_Image_Left/LeftPoseButton") ??
                              transform.Find("SafeArea/LeftPoseButton") ??
                              transform.Find("LeftPoseButton");
                if (t != null) leftPoseButton = t.GetComponent<Button>();
            }
            if (rightPoseButton == null)
            {
                Transform t = transform.Find("SafeArea/Border_Image_Right/RightPoseButton") ??
                              transform.Find("Border_Image_Right/RightPoseButton") ??
                              transform.Find("SafeArea/RightPoseButton") ??
                              transform.Find("RightPoseButton");
                if (t != null) rightPoseButton = t.GetComponent<Button>();
            }

            if (borderImageLeft == null && leftPoseButton != null && leftPoseButton.transform.parent != null && leftPoseButton.transform.parent.name.Contains("Border"))
            {
                borderImageLeft = leftPoseButton.transform.parent.gameObject;
            }
            if (borderImageRight == null && rightPoseButton != null && rightPoseButton.transform.parent != null && rightPoseButton.transform.parent.name.Contains("Border"))
            {
                borderImageRight = rightPoseButton.transform.parent.gameObject;
            }

            // Create buttons dynamically if they don't exist yet
            Transform parent = transform.Find("SafeArea") ?? transform;
            if (leftPoseButton == null)
            {
                Transform btnParent = borderImageLeft != null ? borderImageLeft.transform : parent;
                GameObject leftObj = new GameObject("LeftPoseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                leftObj.transform.SetParent(btnParent, false);
                RectTransform rt = leftObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = borderImageLeft != null ? Vector2.zero : new Vector2(-460f, -320f);
                rt.sizeDelta = new Vector2(300f, 300f);
                leftPoseButton = leftObj.GetComponent<Button>();
            }

            if (rightPoseButton == null)
            {
                Transform btnParent = borderImageRight != null ? borderImageRight.transform : parent;
                GameObject rightObj = new GameObject("RightPoseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                rightObj.transform.SetParent(btnParent, false);
                RectTransform rt = rightObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = borderImageRight != null ? Vector2.zero : new Vector2(446f, -320f);
                rt.sizeDelta = new Vector2(300f, 300f);
                rightPoseButton = rightObj.GetComponent<Button>();
            }

            // Locate ActionButton and its label
            if (actionButton == null)
            {
                Transform ab = transform.Find("SafeArea/ActionButton") ?? transform.Find("ActionButton");
                if (ab != null) actionButton = ab.GetComponent<Button>();
                if (actionButton == null)
                {
                    Button[] btns = GetComponentsInChildren<Button>(true);
                    foreach (var b in btns)
                    {
                        if (b != leftPoseButton && b != rightPoseButton && b.gameObject.name.IndexOf("action", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            actionButton = b;
                            break;
                        }
                    }
                }
            }
            if (actionButton != null && actionButtonText == null)
            {
                actionButtonText = actionButton.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (feedbackPanel == null)
            {
                Transform fb = transform.Find("FeedbackPanel") ?? transform.Find("SafeArea/FeedbackPanel");
                if (fb != null) feedbackPanel = fb.gameObject;
            }
            if (feedbackText == null && feedbackPanel != null) feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>(true);

            if (anuCharacterImage == null)
            {
                Transform t = transform.Find("AnuAvatar") ?? transform.Find("SafeArea/AnuAvatar") ?? transform.Find("AnuCharacterVisual") ?? transform.Find("SafeArea/AnuCharacterVisual");
                if (t != null) anuCharacterImage = t.GetComponent<Image>();
            }

            // Auto-discover Normal Diners vs Looking Diners
            if (otherTablesLooking == null || otherTablesLooking.Length == 0)
            {
                List<GameObject> lookings = new List<GameObject>();
                Transform[] allChildren = GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child == transform) continue;
                    string name = child.name.ToLower();
                    if (name.Contains("screen")) continue;
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
                Transform[] allChildren = GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child == transform) continue;
                    string name = child.name.ToLower();
                    if (name.Contains("screen")) continue;
                    if ((name.Contains("diner") || name.Contains("dinner") || name.Contains("table")) && !name.Contains("look") && !name.Contains("lokk") && !name.Contains("turn") && !name.Contains("stare") && !name.Contains("react"))
                    {
                        normals.Add(child.gameObject);
                    }
                }
                if (normals.Count > 0) otherTablesNormal = normals.ToArray();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Auto-Assign Everything (Fix for APK)")]
        public void AutoLoadSpritesAndEvents()
        {
            AutoFindUIReferences();

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

            anuSittingStraightSprite = GetSprite("U6_MAct_Family_SittingStraight") ?? GetSprite("SPR_Anu_SittingStraight");

            Sprite sittingStraight = GetSprite("SPR_Anu_SittingStraight") ?? anuSittingStraightSprite;
            Sprite slidingChair = GetSprite("SPR_Anu_SlidingChair") ?? GetSprite("SlidingChair");
            Sprite callPolite = GetSprite("SPR_Anu_CallPolite") ?? sittingStraight;
            Sprite callHitGlass = GetSprite("SPR_Anu_CallHitGlass") ?? GetSprite("SPR_Anu_TappingGlass");
            Sprite waitPolite = GetSprite("SPR_Anu_WaitPolite") ?? sittingStraight;
            Sprite shoutHungry = GetSprite("SPR_Anu_WaitBangCutlery") ?? GetSprite("SPR_Anu_ShoutingHungry");
            Sprite kneelChair = GetSprite("SPR_Anu_KneelingChair") ?? slidingChair;

            // In-Scene Family Table Visuals from U6_seniorsActivityBook.png
            Sprite tableSliding = GetSprite("U6_MAct_Family_SlidingChair") ?? slidingChair;
            Sprite tableGlass = GetSprite("U6_MAct_Family_TappingGlass") ?? callHitGlass;
            Sprite tableHungry = GetSprite("U6_MAct_Family_ShoutingHungry") ?? shoutHungry;
            Sprite tableKneel = GetSprite("U6_MAct_Family_KneelingChair") ?? kneelChair;

            waitingEvents.Clear();

            // 1. Sliding off chair vs Sitting straight
            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Sliding on Chair",
                situationPrompt = "Anu spots the fish tank and starts sliding off her chair! Which behavior is polite and safe?",
                correctPoseSprite = sittingStraight,
                incorrectPoseSprite = slidingChair,
                tableVisualOnFail = tableSliding,
                successFeedback = "Good job! Sitting straight keeps you safe in your chair.",
                failFeedback = "Sliding off can cause a fall! Try again.",
                sfxOnFail = "SFX_ChairWobble",
                sfxOnSuccess = "SFX_Sparkle"
            });

            // 2. Hitting glass with spoon vs Spoon resting politely
            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Hitting Glass with Spoon",
                situationPrompt = "Anu wants to make noise with her spoon while waiting. What should she do?",
                correctPoseSprite = callPolite,
                incorrectPoseSprite = callHitGlass,
                tableVisualOnFail = tableGlass,
                successFeedback = "Nice! Keeping cutlery quiet is polite to other diners.",
                failFeedback = "Ting ting ting! Tapping glass is loud and disturbing. Try again.",
                sfxOnFail = "SFX_GlassTing",
                sfxOnSuccess = "SFX_Sparkle"
            });

            // 3. Shouting vs Waiting Patiently
            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Waiting for Food",
                situationPrompt = "The food is taking a while to arrive and Anu is hungry. How should she wait?",
                correctPoseSprite = waitPolite,
                incorrectPoseSprite = shoutHungry,
                tableVisualOnFail = tableHungry,
                successFeedback = "Great patience! Food is being prepared fresh.",
                failFeedback = "Shouting or banging cutlery disturbs the restaurant! Try again.",
                sfxOnFail = "VO_U6_ANU_7",
                sfxOnSuccess = "SFX_Sparkle"
            });

            // 4. Kneeling on chair vs Sitting properly
            waitingEvents.Add(new U6_WaitingEventData_Masters_Activity
            {
                eventName = "Kneeling on Chair",
                situationPrompt = "Anu wants to kneel up on her chair with feet underneath. What should she do?",
                correctPoseSprite = sittingStraight,
                incorrectPoseSprite = kneelChair,
                tableVisualOnFail = tableKneel,
                successFeedback = "Both feet down! Sitting properly keeps the chair steady.",
                failFeedback = "The chair will wobble and tip over! Try again.",
                sfxOnFail = "SFX_ChairWobble",
                sfxOnSuccess = "SFX_Sparkle"
            });

            Debug.Log($"[U6_LiveTableScreen] Auto-Loaded Choice Sprites successfully! Events: {waitingEvents.Count}");

            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                if (gameObject.scene.IsValid())
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
            }
        }
#endif

        private void InitializeDefaultEvents()
        {
            if (waitingEvents == null || waitingEvents.Count == 0)
            {
                waitingEvents = new List<U6_WaitingEventData_Masters_Activity>();
            }
        }

        private void SetPoseCardsActive(bool active)
        {
            if (leftPoseButton != null) leftPoseButton.gameObject.SetActive(active);
            if (borderImageLeft != null) borderImageLeft.SetActive(active);

            if (rightPoseButton != null) rightPoseButton.gameObject.SetActive(active);
            if (borderImageRight != null) borderImageRight.SetActive(active);
        }

        private IEnumerator InitAndStartSequence()
        {
            int retries = 0;
            while (U6_AudioManager_Masters_Activity.Instance == null && retries < 15)
            {
                yield return null;
                retries++;
            }

            if (!gameObject.activeInHierarchy) yield break;

            var audioMgr = U6_AudioManager_Masters_Activity.Instance ?? FindFirstObjectByType<U6_AudioManager_Masters_Activity>();

            currentEventIndex = 0;
            SetOtherTablesLooking(false);
            SetAnuSprite(anuSittingStraightSprite);

            // Hide both pose buttons, borders, and action button during the intro narration
            SetPoseCardsActive(false);
            if (actionButton) actionButton.gameObject.SetActive(false);
            if (feedbackPanel) feedbackPanel.SetActive(false);

            if (promptText) promptText.text = "Today Anu's family is eating out at a restaurant...";
            if (audioMgr != null)
            {
                audioMgr.PlayAmbience("AMB_Restaurant");
                audioMgr.PlayVO("VO_U6_01"); // "Today Anu's family is eating out."
            }

            yield return new WaitForSeconds(3.0f);
            if (!gameObject.activeInHierarchy) yield break;

            if (promptText) promptText.text = "The food is not here yet. Help Anu choose polite manners...";
            if (audioMgr != null)
            {
                audioMgr.PlayVO("VO_U6_02"); // "The food is not here yet. Watch Anu."
            }

            yield return new WaitForSeconds(2.8f);
            if (!gameObject.activeInHierarchy) yield break;

            StartNextEvent();
        }

        private void StartNextEvent()
        {
            if (!gameObject.activeInHierarchy) return;

            if (titleBanner != null && titleBanner.activeSelf)
            {
                if (hideTitleBannerCoroutine != null)
                {
                    StopCoroutine(hideTitleBannerCoroutine);
                    hideTitleBannerCoroutine = null;
                }
                titleBanner.SetActive(false);
            }

            if (currentEventIndex >= waitingEvents.Count)
            {
                StartCoroutine(CompletePart1Sequence());
                return;
            }

            U6_WaitingEventData_Masters_Activity currentEvt = waitingEvents[currentEventIndex];
            if (promptText) promptText.text = currentEvt.situationPrompt;
            if (feedbackPanel) feedbackPanel.SetActive(false);
            if (actionButton) actionButton.gameObject.SetActive(false);

            // Randomize which side gets the polite vs impolite choice
            leftIsCorrect = Random.value > 0.5f;

            Sprite leftSprite = leftIsCorrect ? currentEvt.correctPoseSprite : currentEvt.incorrectPoseSprite;
            Sprite rightSprite = !leftIsCorrect ? currentEvt.correctPoseSprite : currentEvt.incorrectPoseSprite;

            if (leftPoseButton)
            {
                Image leftImg = leftPoseButton.GetComponent<Image>();
                if (leftImg != null)
                {
                    leftImg.sprite = leftSprite;
                    leftImg.color = Color.white;
                    leftImg.preserveAspect = true;
                }
                leftPoseButton.gameObject.SetActive(true);
            }

            if (borderImageLeft != null)
            {
                borderImageLeft.SetActive(true);
                StartCoroutine(PopAvatarAnimation(borderImageLeft.transform));
            }
            else if (leftPoseButton != null)
            {
                StartCoroutine(PopAvatarAnimation(leftPoseButton.transform));
            }

            if (rightPoseButton)
            {
                Image rightImg = rightPoseButton.GetComponent<Image>();
                if (rightImg != null)
                {
                    rightImg.sprite = rightSprite;
                    rightImg.color = Color.white;
                    rightImg.preserveAspect = true;
                }
                rightPoseButton.gameObject.SetActive(true);
            }

            if (borderImageRight != null)
            {
                borderImageRight.SetActive(true);
                StartCoroutine(PopAvatarAnimation(borderImageRight.transform));
            }
            else if (rightPoseButton != null)
            {
                StartCoroutine(PopAvatarAnimation(rightPoseButton.transform));
            }

            isEventActive = true;
            SetOtherTablesLooking(false);

            // Play voiceover prompt to guide the child
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_03"); // "Quick! Tap to help Anu!"
            }

            // Start gentle rhythmic breathing animation on the cards
            StartCardIdlePulse();
        }

        private void OnPoseSelected(bool isLeftSelected)
        {
            if (!isEventActive) return;

            bool isCorrect = (isLeftSelected == leftIsCorrect);
            U6_WaitingEventData_Masters_Activity currentEvt = waitingEvents[currentEventIndex];

            if (isCorrect)
            {
                isEventActive = false;
                StopCardIdlePulse();
                SetPoseCardsActive(false);
                if (actionButton) actionButton.gameObject.SetActive(false);

                // Set Anu character visual back to polite sitting straight!
                SetAnuSprite(anuSittingStraightSprite);

                if (U6_AudioManager_Masters_Activity.Instance != null && !string.IsNullOrEmpty(currentEvt.sfxOnSuccess))
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX(currentEvt.sfxOnSuccess);
                }

                StartCoroutine(ShowEventFeedback(true));
            }
            else
            {
                // Wrong choice! Stop pulse during shake
                StopCardIdlePulse();

                // Wrong choice! Change Anu's family table visual to show the misbehavior from U6_seniorsActivityBook.png
                if (currentEvt.tableVisualOnFail != null)
                {
                    SetAnuSprite(currentEvt.tableVisualOnFail);
                }
                else if (currentEvt.incorrectPoseSprite != null)
                {
                    SetAnuSprite(currentEvt.incorrectPoseSprite);
                }

                // Show "TRY AGAIN!" in the action button label
                if (actionButton != null)
                {
                    if (actionButtonText != null)
                    {
                        actionButtonText.text = "TRY AGAIN!";
                    }
                    actionButton.gameObject.SetActive(true);
                    StartCoroutine(PopAvatarAnimation(actionButton.transform));
                }

                // Wrong choice! Shake the wrong button and show reaction
                if (U6_AudioManager_Masters_Activity.Instance != null && !string.IsNullOrEmpty(currentEvt.sfxOnFail))
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX(currentEvt.sfxOnFail);
                }

                SetOtherTablesLooking(true);
                Transform wrongTarget = isLeftSelected
                    ? (borderImageLeft != null ? borderImageLeft.transform : leftPoseButton?.transform)
                    : (borderImageRight != null ? borderImageRight.transform : rightPoseButton?.transform);
                if (wrongTarget != null) StartCoroutine(ShakeAnimation(wrongTarget));

                // Resume breathing pulse after shake
                StartCoroutine(ResumePulseAfterDelay(0.45f));
            }
        }

        private void StartCardIdlePulse()
        {
            StopCardIdlePulse();
            if (gameObject.activeInHierarchy)
            {
                pulseCardsCoroutine = StartCoroutine(IdlePulseCardsRoutine());
            }
        }

        private void StopCardIdlePulse()
        {
            if (pulseCardsCoroutine != null)
            {
                StopCoroutine(pulseCardsCoroutine);
                pulseCardsCoroutine = null;
            }

            Transform leftT = borderImageLeft != null ? borderImageLeft.transform : leftPoseButton?.transform;
            Transform rightT = borderImageRight != null ? borderImageRight.transform : rightPoseButton?.transform;
            if (leftT != null) leftT.localScale = Vector3.one;
            if (rightT != null) rightT.localScale = Vector3.one;
        }

        private IEnumerator IdlePulseCardsRoutine()
        {
            // Give pop animation 0.25s to finish first
            yield return new WaitForSeconds(0.25f);

            Transform leftT = borderImageLeft != null ? borderImageLeft.transform : leftPoseButton?.transform;
            Transform rightT = borderImageRight != null ? borderImageRight.transform : rightPoseButton?.transform;

            float timer = 0f;
            while (isEventActive)
            {
                timer += Time.deltaTime * 3.2f;
                // Subtle sine wave pulse: 1.0f to 1.045f
                float scale = 1f + 0.045f * Mathf.Sin(timer);

                if (leftT != null) leftT.localScale = new Vector3(scale, scale, 1f);
                if (rightT != null) rightT.localScale = new Vector3(scale, scale, 1f);

                yield return null;
            }

            if (leftT != null) leftT.localScale = Vector3.one;
            if (rightT != null) rightT.localScale = Vector3.one;
        }

        private IEnumerator ResumePulseAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (isEventActive)
            {
                StartCardIdlePulse();
            }
        }

        private IEnumerator ShowEventFeedback(bool success)
        {
            U6_WaitingEventData_Masters_Activity currentEvt = waitingEvents[currentEventIndex];

            if (feedbackPanel) feedbackPanel.SetActive(true);
            if (feedbackText) feedbackText.text = currentEvt.successFeedback;

            SetOtherTablesLooking(false);
            SetAnuSprite(anuSittingStraightSprite);

            yield return new WaitForSeconds(2.8f);
            if (!gameObject.activeInHierarchy) yield break;

            if (feedbackPanel) feedbackPanel.SetActive(false);
            currentEventIndex++;
            StartNextEvent();
        }

        private void SetAnuSprite(Sprite sp)
        {
            if (anuCharacterImage == null) AutoFindUIReferences();

            if (anuCharacterImage != null && sp != null)
            {
                anuCharacterImage.sprite = sp;
                anuCharacterImage.preserveAspect = true;
                anuCharacterImage.enabled = true;
                anuCharacterImage.transform.localScale = (initialAnuScale != Vector3.zero) ? initialAnuScale : Vector3.one;
            }
        }

        private IEnumerator PopAvatarAnimation(Transform target, Vector3 baseScale = default)
        {
            if (target == null) yield break;
            Vector3 originalScale = (baseScale != default && baseScale != Vector3.zero) ? baseScale : Vector3.one;
            target.localScale = originalScale * 0.88f;

            float elapsed = 0f;
            float duration = 0.22f;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f);
                target.localScale = Vector3.Lerp(originalScale * 0.88f, originalScale * 1.05f, t);
                yield return null;
            }

            if (target != null) target.localScale = originalScale;
        }

        private IEnumerator ShakeAnimation(Transform target)
        {
            if (target == null) yield break;
            Vector3 startPos = target.localPosition;
            float elapsed = 0f;
            float duration = 0.4f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float offset = Mathf.Sin(elapsed * 40f) * 15f * (1f - (elapsed / duration));
                target.localPosition = startPos + new Vector3(offset, 0f, 0f);
                yield return null;
            }

            target.localPosition = startPos;
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
                    if (go && go != gameObject) go.SetActive(!looking);
                }
            }
            if (otherTablesLooking != null)
            {
                foreach (var go in otherTablesLooking)
                {
                    if (go && go != gameObject)
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
            StopCardIdlePulse();
            SetPoseCardsActive(false);
            if (actionButton) actionButton.gameObject.SetActive(false);
            SetAnuSprite(anuSittingStraightSprite);
            if (promptText) promptText.text = "Well done! The family waited nicely.";
            if (feedbackPanel) feedbackPanel.SetActive(true);
            if (feedbackText) feedbackText.text = "Star 1 Earned!";

            U6_GameManager_Masters_Activity.Instance.AwardStar();

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                audioMgrPlayVO("VO_U6_04"); // "Everybody is happy. One star!"
            }

            yield return new WaitForSeconds(3.5f);
            if (!gameObject.activeInHierarchy) yield break;

            U6_GameManager_Masters_Activity.Instance.StartPart2();
        }

        private void audioMgrPlayVO(string id)
        {
            if (U6_AudioManager_Masters_Activity.Instance != null)
                U6_AudioManager_Masters_Activity.Instance.PlayVO(id);
        }
    }
}
