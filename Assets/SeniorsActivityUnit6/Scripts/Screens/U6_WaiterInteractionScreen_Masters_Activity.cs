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
    public class U6_WaiterMomentData_Masters_Activity
    {
        public string situationTitle;
        public string situationPrompt;
        public Sprite fullBodyPoseSprite;
        public Sprite propSprite;
        public Sprite anuReactionSprite;

        [TextArea] public string optionA_Text;
        [TextArea] public string optionA_Outcome;
        public string optionA_VO;
        public U6_WaiterFaceState optionA_Face;

        [TextArea] public string optionB_Text;
        [TextArea] public string optionB_Outcome;
        public string optionB_VO;
        public U6_WaiterFaceState optionB_Face;
    }

    public class U6_WaiterInteractionScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Situation Prompt")]
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Full Body Characters & Props Stage")]
        [SerializeField] private Image waiterFullBodyAvatar;
        [SerializeField] private Image anuCharacterAvatar;
        [SerializeField] private Image propItemImage;

        [Header("Waiter Reaction Badge & State")]
        [SerializeField] private Image waiterExpressionBadge;
        [SerializeField] private Sprite waiterSmileSprite;
        [SerializeField] private Sprite waiterNeutralSprite;
        [SerializeField] private Sprite waiterStiffSprite;
        [SerializeField] private TextMeshProUGUI waiterNameBadge;

        [Header("Speech Options")]
        [SerializeField] private GameObject choiceContainer;
        [SerializeField] private Button optionA_Button;
        [SerializeField] private TextMeshProUGUI optionA_Label;
        [SerializeField] private Button optionB_Button;
        [SerializeField] private TextMeshProUGUI optionB_Label;

        [Header("Outcome Feedback")]
        [SerializeField] private GameObject outcomePanel;
        [SerializeField] private TextMeshProUGUI outcomeText;

#pragma warning disable 0414
        [Header("Audios & SFX Used On This Screen")]
        [SerializeField] private string voRaviIntro = "VO_U6_08 (This is Ravi...)";
        [SerializeField] private string voWrongDish = "VO_U6_09 (Oh! That is the wrong dish!)";
        [SerializeField] private string voAnuThankYou = "VO_U6_ANU_4 (Thank you!)";
        [SerializeField] private string voAnuWrongDishPolite = "VO_U6_ANU_5 (Sorry, I think I ordered dosa)";
        [SerializeField] private string voAnuWrongDishLoud = "VO_U6_ANU_6 (This is WRONG!)";
        [SerializeField] private string voAnuDroppedFork = "VO_U6_ANU_8 (Excuse me, could I have another fork?)";
        [SerializeField] private string voAnuDroppedForkShout = "VO_U6_ANU_9 (I dropped my fork!)";
        [SerializeField] private string voWaiterApologize = "VO_U6_WAIT_3 (I am so sorry, I will fix that!)";
        [SerializeField] private string sfxPlateDown = "SFX_PlateDown (Plate set down)";
        [SerializeField] private string sfxForkDrop = "SFX_ForkDrop (Fork dropped on floor)";
        [SerializeField] private string sfxSparkle = "SFX_Sparkle (Polite outcome)";
#pragma warning restore 0414

        [Header("Moments List")]
        [SerializeField] private List<U6_WaiterMomentData_Masters_Activity> moments = new List<U6_WaiterMomentData_Masters_Activity>();

        private int currentMomentIndex = 0;
        private int politeAnswersCount = 0;
        private bool isResolving = false;

        private void Awake()
        {
            if (optionA_Button != null)
                optionA_Button.onClick.AddListener(() => OnOptionSelected(true));

            if (optionB_Button != null)
                optionB_Button.onClick.AddListener(() => OnOptionSelected(false));

            AutoFindUIReferences();
        }

        private void OnEnable()
        {
            AutoFindUIReferences();
#if UNITY_EDITOR
            AutoLoadSpritesAndMoments();
#else
            InitializeDefaultMoments();
#endif
            currentMomentIndex = 0;
            politeAnswersCount = 0;
            if (waiterNameBadge) waiterNameBadge.text = "Ravi";

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_08");
            }

            DisplayCurrentMoment();
        }

        private void AutoFindUIReferences()
        {
            if (promptText == null) promptText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (choiceContainer == null)
            {
                Transform ct = transform.Find("ChoiceContainer");
                if (ct != null) choiceContainer = ct.gameObject;
            }
            if (optionA_Button == null && choiceContainer != null)
            {
                Button[] btns = choiceContainer.GetComponentsInChildren<Button>(true);
                if (btns.Length > 0) optionA_Button = btns[0];
                if (btns.Length > 1) optionB_Button = btns[1];
            }
            if (optionA_Label == null && optionA_Button != null) optionA_Label = optionA_Button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (optionB_Label == null && optionB_Button != null) optionB_Label = optionB_Button.GetComponentInChildren<TextMeshProUGUI>(true);

            if (waiterFullBodyAvatar == null)
            {
                Transform t = transform.Find("WaiterFullBody") 
                           ?? transform.Find("WaiterAvatarBox") 
                           ?? transform.Find("Waiter_Image") 
                           ?? transform.Find("RaviAvatar");
                if (t != null) waiterFullBodyAvatar = t.GetComponentInChildren<Image>(true);
            }
            if (anuCharacterAvatar == null)
            {
                Transform t = transform.Find("AnuAvatar") 
                           ?? transform.Find("Anu_Avatar") 
                           ?? transform.Find("Anu") 
                           ?? transform.Find("AnuCharacter");
                if (t != null) anuCharacterAvatar = t.GetComponentInChildren<Image>(true);
            }
            if (propItemImage == null)
            {
                Transform t = transform.Find("PropItem") 
                           ?? transform.Find("Prop_Item") 
                           ?? transform.Find("Prop") 
                           ?? transform.Find("PropImage") 
                           ?? transform.Find("DishImage");
                if (t != null) propItemImage = t.GetComponentInChildren<Image>(true);
            }
            if (waiterExpressionBadge == null)
            {
                Transform t = transform.Find("ExpressionBadge") 
                           ?? transform.Find("WaiterBadge")
                           ?? transform.Find("Badge");
                if (t != null) waiterExpressionBadge = t.GetComponentInChildren<Image>(true);
            }
            if (outcomePanel == null)
            {
                Transform t = transform.Find("OutcomePanel");
                if (t != null) outcomePanel = t.gameObject;
            }
            if (outcomeText == null && outcomePanel != null)
            {
                outcomeText = outcomePanel.GetComponentInChildren<TextMeshProUGUI>(true);
            }
        }

#if UNITY_EDITOR
        private void AutoLoadSpritesAndMoments()
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

            waiterSmileSprite = GetSprite("SPR_Ravi_WarmSmile");
            waiterNeutralSprite = GetSprite("SPR_Ravi_NeutralBlank");
            waiterStiffSprite = GetSprite("SPR_Ravi_StiffPolite");

            Sprite raviWater = GetSprite("SPR_Ravi_WaterJug");
            Sprite raviPad = GetSprite("SPR_Ravi_StandingPad");
            Sprite raviPlate = GetSprite("SPR_Ravi_ServingPlate");

            Sprite anuStraight = GetSprite("SPR_Anu_SittingStraight");
            Sprite anuHandRaise = GetSprite("SPR_Anu_HandRaise");

            Sprite emptyGlass = GetSprite("SPR_EmptyGlass");
            Sprite spoonFork = GetSprite("SPR_Spoon_and_Fork");
            Sprite dishDosa = GetSprite("SPR_Dish_Dosa");
            Sprite dishNoodles = GetSprite("SPR_Dish_Noodles");

            moments.Clear();

            // 1. Water Served
            moments.Add(new U6_WaiterMomentData_Masters_Activity
            {
                situationTitle = "Water Served",
                situationPrompt = "Ravi brings a fresh jug of water to the table.",
                fullBodyPoseSprite = raviWater,
                propSprite = emptyGlass,
                anuReactionSprite = anuStraight,
                optionA_Text = "\"Thank you!\"",
                optionA_Outcome = "Ravi smiles warmly and nods.",
                optionA_VO = "VO_U6_ANU_4",
                optionA_Face = U6_WaiterFaceState.WarmSmile,
                optionB_Text = "(Say nothing)",
                optionB_Outcome = "Ravi is polite, but neutral.",
                optionB_VO = "",
                optionB_Face = U6_WaiterFaceState.BlankNeutral
            });

            // 2. Calling Ravi
            moments.Add(new U6_WaiterMomentData_Masters_Activity
            {
                situationTitle = "Calling Ravi",
                situationPrompt = "Anu needs something and Ravi is across the room.",
                fullBodyPoseSprite = raviPad,
                propSprite = null,
                anuReactionSprite = anuHandRaise,
                optionA_Text = "Raise a hand gently and make eye contact",
                optionA_Outcome = "Ravi notices and comes over calmly: \"Of course, one moment!\"",
                optionA_VO = "VO_U6_WAIT_2",
                optionA_Face = U6_WaiterFaceState.WarmSmile,
                optionB_Text = "Wave both arms and shout across the room",
                optionB_Outcome = "Ravi hurries over while other tables look over.",
                optionB_VO = "VO_U6_MUM_1",
                optionB_Face = U6_WaiterFaceState.BlankNeutral
            });

            // 3. Wrong Dish (No food prop displayed on table to avoid contradiction)
            moments.Add(new U6_WaiterMomentData_Masters_Activity
            {
                situationTitle = "Wrong Dish",
                situationPrompt = "Ravi brings the wrong dish by mistake!",
                fullBodyPoseSprite = raviPlate,
                propSprite = null,
                anuReactionSprite = anuStraight,
                optionA_Text = "\"Sorry, I think this is the wrong dish.\"",
                optionA_Outcome = "Ravi apologises warmly: \"I will fix that right away!\"",
                optionA_VO = "VO_U6_WAIT_3",
                optionA_Face = U6_WaiterFaceState.WarmSmile,
                optionB_Text = "\"This is WRONG!\" (loudly)",
                optionB_Outcome = "Ravi apologises stiffly and fixes it quietly.",
                optionB_VO = "VO_U6_ANU_6",
                optionB_Face = U6_WaiterFaceState.StiffPolite
            });

            // 4. Food Served
            moments.Add(new U6_WaiterMomentData_Masters_Activity
            {
                situationTitle = "Food Served",
                situationPrompt = "Ravi sets the hot dish down in front of Anu.",
                fullBodyPoseSprite = raviPlate,
                propSprite = dishDosa,
                anuReactionSprite = anuStraight,
                optionA_Text = "\"Thank you, Ravi!\"",
                optionA_Outcome = "Ravi nods happily. Enjoy your meal!",
                optionA_VO = "VO_U6_ANU_4",
                optionA_Face = U6_WaiterFaceState.WarmSmile,
                optionB_Text = "(Grab fork and eat immediately without looking)",
                optionB_Outcome = "Ravi steps away quietly.",
                optionB_VO = "",
                optionB_Face = U6_WaiterFaceState.BlankNeutral
            });

            // 5. Dropped Fork
            moments.Add(new U6_WaiterMomentData_Masters_Activity
            {
                situationTitle = "Dropped Fork",
                situationPrompt = "Anu accidentally drops her fork on the floor.",
                fullBodyPoseSprite = raviPad,
                propSprite = spoonFork,
                anuReactionSprite = anuStraight,
                optionA_Text = "\"Excuse me, could I have another fork please?\"",
                optionA_Outcome = "Ravi brings a clean fork right away with a smile.",
                optionA_VO = "VO_U6_ANU_8",
                optionA_Face = U6_WaiterFaceState.WarmSmile,
                optionB_Text = "\"I dropped my fork!\" (shouted)",
                optionB_Outcome = "Ravi brings one while the nearby diners look up.",
                optionB_VO = "VO_U6_ANU_9",
                optionB_Face = U6_WaiterFaceState.StiffPolite
            });

            Debug.Log($"[U6_WaiterInteractionScreen] Auto-Loaded all 5 moments with full-body Ravi poses & table props.");
        }
#endif

        private void InitializeDefaultMoments()
        {
            if (moments == null || moments.Count == 0)
            {
                moments = new List<U6_WaiterMomentData_Masters_Activity>();
            }
        }

        private void DisplayCurrentMoment()
        {
            if (currentMomentIndex >= moments.Count)
            {
                StartCoroutine(CompletePart3Sequence());
                return;
            }

            U6_WaiterMomentData_Masters_Activity m = moments[currentMomentIndex];
            
            // Dynamic check for ordered dish from Part 2 Menu
            string orderedName = (U6_GameManager_Masters_Activity.Instance != null) ? U6_GameManager_Masters_Activity.Instance.OrderedDishName : "dosa";
            Sprite orderedSprite = (U6_GameManager_Masters_Activity.Instance != null) ? U6_GameManager_Masters_Activity.Instance.OrderedDishSprite : null;

            if (currentMomentIndex == 2) // Wrong Dish
            {
                m.propSprite = null; // Don't show any food prop on table
                if (!string.IsNullOrEmpty(orderedName))
                {
                    m.optionA_Text = $"\"Sorry, I think I ordered {orderedName.ToLower()}.\"";
                }
                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_09"); // "Oh! That is the wrong dish!"
                }
            }
            else if (currentMomentIndex == 3) // Food Served
            {
                if (orderedSprite != null)
                {
                    m.propSprite = orderedSprite; // Show the exact ordered dish
                }
                if (!string.IsNullOrEmpty(orderedName))
                {
                    m.situationPrompt = $"Ravi sets the hot {orderedName.ToLower()} down in front of Anu.";
                }
            }

            if (promptText) promptText.text = m.situationPrompt;
            if (optionA_Label) optionA_Label.text = m.optionA_Text;
            if (optionB_Label) optionB_Label.text = m.optionB_Text;

            // 1. Update Full Body Ravi with visual bounce transition
            if (waiterFullBodyAvatar != null)
            {
                if (m.fullBodyPoseSprite != null)
                {
                    waiterFullBodyAvatar.sprite = m.fullBodyPoseSprite;
                    waiterFullBodyAvatar.preserveAspect = true;
                    waiterFullBodyAvatar.gameObject.SetActive(true);
                    StartCoroutine(PopAvatarAnimation(waiterFullBodyAvatar.transform));
                    Debug.Log($"[U6_WaiterInteractionScreen] Moment {currentMomentIndex + 1}: Ravi Pose set to '{m.fullBodyPoseSprite.name}'");
                }
            }

            // 2. Update Anu
            if (anuCharacterAvatar != null)
            {
                if (m.anuReactionSprite != null)
                {
                    anuCharacterAvatar.sprite = m.anuReactionSprite;
                    anuCharacterAvatar.preserveAspect = true;
                    anuCharacterAvatar.gameObject.SetActive(true);
                    StartCoroutine(PopAvatarAnimation(anuCharacterAvatar.transform));
                }
            }

            // 3. Update Table Prop (Water Glass, Noodles, Dosa, Dropped Fork)
            if (propItemImage != null)
            {
                if (m.propSprite != null)
                {
                    propItemImage.sprite = m.propSprite;
                    propItemImage.preserveAspect = true;
                    propItemImage.gameObject.SetActive(true);
                    StartCoroutine(PopAvatarAnimation(propItemImage.transform));
                }
                else
                {
                    propItemImage.gameObject.SetActive(false);
                }
            }

            if (waiterExpressionBadge != null)
            {
                waiterExpressionBadge.gameObject.SetActive(false);
            }

            if (choiceContainer) choiceContainer.SetActive(true);
            if (outcomePanel) outcomePanel.SetActive(false);

            isResolving = false;
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

        private void OnOptionSelected(bool choseA)
        {
            if (isResolving) return;
            isResolving = true;

            StartCoroutine(ResolveMomentSequence(choseA));
        }

        private IEnumerator ResolveMomentSequence(bool choseA)
        {
            U6_WaiterMomentData_Masters_Activity m = moments[currentMomentIndex];
            if (choiceContainer) choiceContainer.SetActive(false);
            if (outcomePanel) outcomePanel.SetActive(true);

            if (choseA)
            {
                politeAnswersCount++;
                if (outcomeText) outcomeText.text = m.optionA_Outcome;
                SetWaiterFace(m.optionA_Face);
                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                    if (!string.IsNullOrEmpty(m.optionA_VO))
                        U6_AudioManager_Masters_Activity.Instance.PlayVO(m.optionA_VO);
                }
            }
            else
            {
                if (outcomeText) outcomeText.text = m.optionB_Outcome;
                SetWaiterFace(m.optionB_Face);
                if (U6_AudioManager_Masters_Activity.Instance != null && !string.IsNullOrEmpty(m.optionB_VO))
                {
                    U6_AudioManager_Masters_Activity.Instance.PlayVO(m.optionB_VO);
                }
            }

            yield return new WaitForSeconds(3.5f);

            currentMomentIndex++;
            DisplayCurrentMoment();
        }

        private void SetWaiterFace(U6_WaiterFaceState state)
        {
            Sprite targetFace = null;
            Color tintColor = Color.white;

            switch (state)
            {
                case U6_WaiterFaceState.WarmSmile:
                    targetFace = waiterSmileSprite;
                    tintColor = new Color(0.95f, 1f, 0.95f, 1f);
                    break;
                case U6_WaiterFaceState.BlankNeutral:
                    targetFace = waiterNeutralSprite;
                    tintColor = Color.white;
                    break;
                case U6_WaiterFaceState.StiffPolite:
                    targetFace = waiterStiffSprite;
                    tintColor = new Color(1f, 0.92f, 0.92f, 1f);
                    break;
            }

            // Show expression reaction directly in place of the main waiter avatar
            if (waiterFullBodyAvatar != null && targetFace != null)
            {
                waiterFullBodyAvatar.sprite = targetFace;
                waiterFullBodyAvatar.color = tintColor;
                StartCoroutine(PopAvatarAnimation(waiterFullBodyAvatar.transform));
            }

            if (waiterExpressionBadge != null)
            {
                waiterExpressionBadge.gameObject.SetActive(false);
            }
        }

        private IEnumerator CompletePart3Sequence()
        {
            if (outcomePanel) outcomePanel.SetActive(true);

            if (politeAnswersCount >= 3)
            {
                if (promptText) promptText.text = "Great job showing kindness to Ravi!";
                if (outcomeText) outcomeText.text = $"Star 2 Earned! ({politeAnswersCount}/5 Polite Choices)";

                if (U6_GameManager_Masters_Activity.Instance != null)
                {
                    U6_GameManager_Masters_Activity.Instance.AwardStar();
                }
            }
            else
            {
                if (promptText) promptText.text = "Remember to be polite to the waiter next time!";
                if (outcomeText) outcomeText.text = $"Be gentler next time to earn Star 2 ({politeAnswersCount}/5 Polite Choices)";
            }

            yield return new WaitForSeconds(3.2f);

            if (U6_GameManager_Masters_Activity.Instance != null)
            {
                U6_GameManager_Masters_Activity.Instance.StartPart4();
            }
        }
    }
}
