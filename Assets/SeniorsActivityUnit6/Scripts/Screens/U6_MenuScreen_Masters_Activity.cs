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
    public class U6_MenuScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Menu Grid")]
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private GameObject dishCardPrefab;
        [SerializeField] private List<U6_DishItemData_Masters_Activity> defaultDishes = new List<U6_DishItemData_Masters_Activity>();

        [Header("Actions")]
        [SerializeField] private Button callWaiterButton;

        [Header("Feedback Overlays")]
        [SerializeField] private GameObject readFirstPrompt;
        [SerializeField] private TextMeshProUGUI readFirstText;
        [SerializeField] private GameObject awkwardWaiterOverlay;
        [SerializeField] private TextMeshProUGUI anuStammerText;

        [Header("Ordering Choice Popup")]
        [SerializeField] private GameObject orderChoicePanel;
        [SerializeField] private Button politeOptionButton;
        [SerializeField] private TextMeshProUGUI politeOptionText;
        [SerializeField] private Button impoliteOptionButton;
        [SerializeField] private TextMeshProUGUI impoliteOptionText;
        [SerializeField] private Button politeAudioPreviewButton;
        [SerializeField] private Button impoliteAudioPreviewButton;

#pragma warning disable 0414
        [Header("Audios & SFX Used On This Screen")]
        [SerializeField] private string sfxMenuOpen = "SFX_MenuOpen (Menu opening sound)";
        [SerializeField] private string voChooseDish = "VO_U6_05 (Now choose: What will Anu eat?)";
        [SerializeField] private string voReadFirst = "VO_U6_06 (Read first!)";
        [SerializeField] private string voUmmm = "VO_U6_ANU_3 (Ummm... ummm...)";
        [SerializeField] private string voHowToAsk = "VO_U6_07 (How should Anu ask?)";
        [SerializeField] private string voPoliteOrder = "VO_U6_ANU_1 (Could I have the dosa please?)";
        [SerializeField] private string voBluntOrder = "VO_U6_ANU_2 (I want dosa!)";
        [SerializeField] private string voWaiterResponse = "VO_U6_WAIT_1 (Certainly!)";
        [SerializeField] private string sfxPadWrite = "SFX_PadWrite (Waiter taking note)";
        [SerializeField] private string sfxSparkle = "SFX_Sparkle (Order success)";
#pragma warning restore 0414

        [Header("Full Body Waiter")]
        [SerializeField] private Image waiterStandingVisual;

        [Header("Waiter Reaction Feedback")]
        [SerializeField] private GameObject waiterFeedbackPopup;
        [SerializeField] private TextMeshProUGUI waiterDialogText;

        [Header("Choice Avatars (Pre-Reader)")]
        [SerializeField] private Sprite politeAvatarSprite;
        [SerializeField] private Sprite impoliteAvatarSprite;

        private List<U6_DishCardUI_Masters_Activity> spawnedCards = new List<U6_DishCardUI_Masters_Activity>();
        private U6_DishItemData_Masters_Activity selectedDish = null;
        private bool hasOrdered = false;
        private Coroutine buttonFeedbackRoutine = null;

        private void Awake()
        {
            AutoFindUIReferences();

            if (callWaiterButton != null)
                callWaiterButton.onClick.AddListener(OnCallWaiterClicked);

            if (politeOptionButton != null)
                politeOptionButton.onClick.AddListener(() => SelectOrderWay(true));

            if (impoliteOptionButton != null)
                impoliteOptionButton.onClick.AddListener(() => SelectOrderWay(false));

            if (politeAudioPreviewButton != null)
                politeAudioPreviewButton.onClick.AddListener(PlayPoliteAudioPreview);

            if (impoliteAudioPreviewButton != null)
                impoliteAudioPreviewButton.onClick.AddListener(PlayImpoliteAudioPreview);
        }

        private void AutoFindUIReferences()
        {
            if (waiterStandingVisual == null)
            {
                Transform t = transform.Find("Waiter_Image")
                           ?? transform.Find("WaiterStandingVisual")
                           ?? transform.Find("SafeArea/WaiterStandingVisual")
                           ?? transform.Find("SafeArea/Waiter_Image");
                if (t != null) waiterStandingVisual = t.GetComponent<Image>();
            }

            if (waiterStandingVisual != null)
            {
                waiterStandingVisual.gameObject.SetActive(false);
            }

            if (orderChoicePanel == null)
            {
                orderChoicePanel = transform.Find("SafeArea/OrderChoicePanel")?.gameObject
                                ?? transform.Find("OrderChoicePanel")?.gameObject;
            }

            if (orderChoicePanel != null)
            {
                if (politeOptionButton == null)
                    politeOptionButton = orderChoicePanel.transform.Find("PoliteButton")?.GetComponent<Button>();
                if (impoliteOptionButton == null)
                    impoliteOptionButton = orderChoicePanel.transform.Find("ImpoliteButton")?.GetComponent<Button>();
            }

            if (politeOptionButton != null)
            {
                Transform lbl = politeOptionButton.transform.Find("Label");
                if (lbl != null) politeOptionText = lbl.GetComponent<TextMeshProUGUI>();
            }
            if (impoliteOptionButton != null)
            {
                Transform lbl = impoliteOptionButton.transform.Find("Label");
                if (lbl != null) impoliteOptionText = lbl.GetComponent<TextMeshProUGUI>();
            }

            // Auto-discover audio preview buttons if created in prefab/hierarchy
            if (politeAudioPreviewButton == null && politeOptionButton != null)
            {
                politeAudioPreviewButton = politeOptionButton.transform.Find("HearPoliteButton")?.GetComponent<Button>()
                                        ?? politeOptionButton.GetComponentInChildren<Button>();
            }
            if (impoliteAudioPreviewButton == null && impoliteOptionButton != null)
            {
                impoliteAudioPreviewButton = impoliteOptionButton.transform.Find("HearImpoliteButton")?.GetComponent<Button>()
                                          ?? impoliteOptionButton.GetComponentInChildren<Button>();
            }
        }

        private void OnEnable()
        {
            AutoFindUIReferences();
#if UNITY_EDITOR
            AutoLoadMenuSprites();
#else
            InitializeDefaultDishes();
#endif
            ResetScreen();
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_MenuOpen");
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_05"); // "Now choose. What will Anu eat?"
            }
        }

#if UNITY_EDITOR
        private void AutoLoadMenuSprites()
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

            if (waiterStandingVisual != null && (waiterStandingVisual.sprite == null || waiterStandingVisual.sprite.name != "SPR_Ravi_StandingPad"))
            {
                Sprite sp = GetSprite("SPR_Ravi_StandingPad");
                if (sp != null)
                {
                    waiterStandingVisual.sprite = sp;
                    waiterStandingVisual.preserveAspect = true;
                }
            }

            // Solo character sprites for Anu's emotions instead of entire 4-person table
            politeAvatarSprite = GetSprite("SPR_Anu_HandRaise") ?? GetSprite("SPR_Anu_SittingStraight") ?? GetSprite("U6_MAct_Family_HandRaise");
            impoliteAvatarSprite = GetSprite("SPR_Anu_ShoutingHungry") ?? GetSprite("U6_MAct_Family_ShoutingHungry");

            defaultDishes.Clear();
            defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "dosa", dishName = "Dosa", price = 60, dishSprite = GetSprite("SPR_Dish_Dosa") });
            defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "idli", dishName = "Idli", price = 40, dishSprite = GetSprite("SPR_Dish_Idli") });
            defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "noodles", dishName = "Noodles", price = 90, dishSprite = GetSprite("SPR_Dish_Noodles") });
            defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "rice", dishName = "Rice", price = 80, dishSprite = GetSprite("SPR_Dish_Rice") });
            defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "roti", dishName = "Roti", price = 30, dishSprite = GetSprite("SPR_Dish_Roti") });
            defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "icecream", dishName = "Ice Cream", price = 50, dishSprite = GetSprite("SPR_Dish_IceCream") });

            InitializeDefaultDishes();
        }
#endif

        private void InitializeDefaultDishes()
        {
            if (defaultDishes.Count == 0)
            {
                defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "dosa", dishName = "Dosa", price = 60 });
                defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "idli", dishName = "Idli", price = 40 });
                defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "noodles", dishName = "Noodles", price = 90 });
                defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "rice", dishName = "Rice", price = 80 });
                defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "roti", dishName = "Roti", price = 30 });
                defaultDishes.Add(new U6_DishItemData_Masters_Activity { dishId = "icecream", dishName = "Ice Cream", price = 50 });
            }

            if (cardsContainer != null && dishCardPrefab != null && spawnedCards.Count == 0)
            {
                foreach (var dish in defaultDishes)
                {
                    GameObject cardObj = Instantiate(dishCardPrefab, cardsContainer);
                    cardObj.SetActive(true);
                    U6_DishCardUI_Masters_Activity cardUI = cardObj.GetComponent<U6_DishCardUI_Masters_Activity>();
                    if (cardUI != null)
                    {
                        cardUI.Setup(dish);
                        cardUI.OnCardSelected += HandleDishSelected;
                        spawnedCards.Add(cardUI);
                    }
                }
            }
            else
            {
                foreach (var card in spawnedCards)
                {
                    if (card != null) card.gameObject.SetActive(true);
                }
            }
        }

        private void ResetScreen()
        {
            selectedDish = null;
            hasOrdered = false;
            foreach (var card in spawnedCards)
            {
                card.SetSelected(false);
            }

            if (buttonFeedbackRoutine != null)
            {
                StopCoroutine(buttonFeedbackRoutine);
                buttonFeedbackRoutine = null;
            }

            if (callWaiterButton != null)
            {
                callWaiterButton.gameObject.SetActive(true);
                callWaiterButton.interactable = true;
                var txt = callWaiterButton.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = "CALL WAITER";
            }

            if (readFirstPrompt) readFirstPrompt.SetActive(false);
            if (awkwardWaiterOverlay) awkwardWaiterOverlay.SetActive(false);
            if (orderChoicePanel) orderChoicePanel.SetActive(false);
            if (waiterFeedbackPopup) waiterFeedbackPopup.SetActive(false);
            if (waiterStandingVisual) waiterStandingVisual.gameObject.SetActive(false);
        }

        private void HandleDishSelected(U6_DishCardUI_Masters_Activity selectedCard)
        {
            if (buttonFeedbackRoutine != null)
            {
                StopCoroutine(buttonFeedbackRoutine);
                buttonFeedbackRoutine = null;
            }

            var dishData = selectedCard.Data;
            selectedDish = dishData;
            foreach (var card in spawnedCards)
            {
                card.SetSelected(card.Data == dishData);
            }
            if (U6_GameManager_Masters_Activity.Instance != null && selectedDish != null)
            {
                U6_GameManager_Masters_Activity.Instance.SetOrderedDish(selectedDish.dishName, selectedDish.dishSprite);
            }

            // Immediately update and bounce Call Waiter button so the child sees direct feedback!
            if (callWaiterButton != null && selectedDish != null)
            {
                var txt = callWaiterButton.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    txt.text = $"ORDER {selectedDish.dishName.ToUpper()} (CALL WAITER)";
                }
                StartCoroutine(SquishyButtonSelectRoutine(callWaiterButton.transform, null));
            }

            // If the choices panel is already open when user changes dish, update choice cards dynamically!
            if (orderChoicePanel != null && orderChoicePanel.activeSelf)
            {
                ShowOrderOptions();
            }

            Debug.Log($"[U6_MenuScreen] Selected dish: {selectedDish.dishName}");
        }

        private void OnCallWaiterClicked()
        {
            if (selectedDish == null)
            {
                // No item selected yet: Do NOT play awkward stammer audio or show popup overlays.
                // Simply provide gentle visual feedback directly on the Call Waiter button!
                if (buttonFeedbackRoutine != null) StopCoroutine(buttonFeedbackRoutine);
                buttonFeedbackRoutine = StartCoroutine(ShowSelectDishFirstRoutine());
            }
            else
            {
                // Dish IS selected -> Hide call waiter button to eliminate any overlap!
                if (callWaiterButton != null)
                {
                    callWaiterButton.gameObject.SetActive(false);
                }

                // Waiter appears with his notepad ready to take the order!
                if (waiterStandingVisual)
                {
                    waiterStandingVisual.gameObject.SetActive(true);
                    StartCoroutine(PopAvatarAnimation(waiterStandingVisual.transform));
                    Debug.Log($"[U6_MenuScreen] Waiter Ravi stepped in to take order for: {selectedDish.dishName}");
                }
                ShowOrderOptions();
            }
        }

        private IEnumerator ShowSelectDishFirstRoutine()
        {
            if (callWaiterButton == null) yield break;
            var txt = callWaiterButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = "SELECT A DISH FIRST";

            yield return StartCoroutine(SquishyButtonSelectRoutine(callWaiterButton.transform, null));
            yield return new WaitForSeconds(1.4f);

            if (txt != null && selectedDish == null)
            {
                txt.text = "CALL WAITER";
            }
            buttonFeedbackRoutine = null;
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

        private string GetDishAudioId(U6_DishItemData_Masters_Activity dish, bool isPolite)
        {
            if (dish == null)
            {
                return isPolite ? "VO_U6_ANU_1" : "VO_U6_ANU_2";
            }

            string name = dish.dishName != null ? dish.dishName.ToLower().Trim() : "";
            string id = dish.dishId != null ? dish.dishId.ToLower().Trim() : "";

            if (name.Contains("dosa") || id.Contains("dosa"))
            {
                return isPolite ? "VO_U6_ANU_1" : "VO_U6_ANU_2";
            }
            if (name.Contains("idli") || id.Contains("idli"))
            {
                return isPolite ? "VO_U6_ORD_IDLI_POLITE" : "VO_U6_ORD_IDLI_BLUNT";
            }
            if (name.Contains("noodle") || id.Contains("noodle"))
            {
                return isPolite ? "VO_U6_ORD_NOODLES_POLITE" : "VO_U6_ORD_NOODLES_BLUNT";
            }
            if (name.Contains("rice") || id.Contains("rice"))
            {
                return isPolite ? "VO_U6_ORD_RICE_POLITE" : "VO_U6_ORD_RICE_BLUNT";
            }
            if (name.Contains("roti") || id.Contains("roti"))
            {
                return isPolite ? "VO_U6_ORD_ROTI_POLITE" : "VO_U6_ORD_ROTI_BLUNT";
            }
            if (name.Contains("ice") || name.Contains("cream") || id.Contains("icecream"))
            {
                return isPolite ? "VO_U6_ORD_ICECREAM_POLITE" : "VO_U6_ORD_ICECREAM_BLUNT";
            }
            if (name.Contains("sandwich") || id.Contains("sandwich"))
            {
                return isPolite ? "VO_U6_ORD_SANDWICH_POLITE" : "VO_U6_ORD_SANDWICH_BLUNT";
            }
            if (name.Contains("juice") || id.Contains("juice"))
            {
                return isPolite ? "VO_U6_ORD_JUICE_POLITE" : "VO_U6_ORD_JUICE_BLUNT";
            }

            return isPolite ? "VO_U6_ANU_1" : "VO_U6_ANU_2";
        }

        public void PlayPoliteAudioPreview()
        {
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                string soundId = GetDishAudioId(selectedDish, isPolite: true);
                U6_AudioManager_Masters_Activity.Instance.PlayVO(soundId);
            }
        }

        public void PlayImpoliteAudioPreview()
        {
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                string soundId = GetDishAudioId(selectedDish, isPolite: false);
                U6_AudioManager_Masters_Activity.Instance.PlayVO(soundId);
            }
        }

        private void ShowOrderOptions()
        {
            if (orderChoicePanel == null) return;

            string dish = selectedDish != null ? selectedDish.dishName : "dish";

            EnsurePreReaderChoiceUI();

            if (politeOptionText != null)
            {
                // If auto-sizing is enabled, ensure max size allows large readable text up to at least 38pt
                if (politeOptionText.enableAutoSizing && politeOptionText.fontSizeMax < 36f)
                {
                    politeOptionText.fontSizeMax = 38f;
                }
                politeOptionText.text = $"<color=#D4EFDF><size=78%><b>[POLITE: PLEASE]</b></size></color>\n<b>\"Could I have the {dish.ToLower()}, please?\"</b>";
            }

            if (impoliteOptionText != null)
            {
                if (impoliteOptionText.enableAutoSizing && impoliteOptionText.fontSizeMax < 36f)
                {
                    impoliteOptionText.fontSizeMax = 38f;
                }
                impoliteOptionText.text = $"<color=#FADBD8><size=78%><b>[BLUNT: DEMAND]</b></size></color>\n<b>\"I want {dish.ToLower()}!\"</b>";
            }

            orderChoicePanel.SetActive(true);

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_07");
            }
        }

        private void EnsurePreReaderChoiceUI()
        {
            // Do NOT overwrite user-configured RectTransform on orderChoicePanel!
            SetupChoiceCard(politeOptionButton, true);
            SetupChoiceCard(impoliteOptionButton, false);
        }

        private void SetupChoiceCard(Button btn, bool isPolite)
        {
            if (btn == null) return;

            // Do NOT overwrite user-configured RectTransform position or size on the buttons!

            // Find dialogue text label to assign reference without overriding user-configured Inspector font size
            Transform lbl = btn.transform.Find("Label");
            if (lbl != null)
            {
                var tmp = lbl.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.alignment = TextAlignmentOptions.Center;
                    if (isPolite) politeOptionText = tmp;
                    else impoliteOptionText = tmp;
                }
            }

            // Setup / Refresh Solo Avatar Portrait (only create if missing, NEVER overwrite existing RectTransform)
            Sprite targetSprite = isPolite ? politeAvatarSprite : impoliteAvatarSprite;
            Transform avT = btn.transform.Find("AvatarPortrait");
            if (avT == null)
            {
                GameObject avObj = new GameObject("AvatarPortrait", typeof(RectTransform), typeof(Image));
                avObj.transform.SetParent(btn.transform, false);
                var avRT = avObj.GetComponent<RectTransform>();
                avRT.anchorMin = new Vector2(0f, 0.5f);
                avRT.anchorMax = new Vector2(0f, 0.5f);
                avRT.pivot = new Vector2(0f, 0.5f);
                avRT.anchoredPosition = new Vector2(14f, 0f);
                avRT.sizeDelta = new Vector2(84f, 84f);

                var img = avObj.GetComponent<Image>();
                img.sprite = targetSprite;
                img.preserveAspect = true;
                img.raycastTarget = false;
            }
            else
            {
                var img = avT.GetComponent<Image>();
                if (img != null)
                {
                    if (targetSprite != null && (img.sprite == null || img.sprite.name.Contains("Family")))
                    {
                        img.sprite = targetSprite;
                    }
                    img.preserveAspect = true;
                }
                // Do NOT touch avT RectTransform - respect user's Inspector layout!
            }

            // Setup / Refresh Listen Button (only create if missing, NEVER overwrite existing RectTransform)
            string listenName = isPolite ? "HearPoliteButton" : "HearImpoliteButton";
            Transform hT = btn.transform.Find(listenName);
            if (hT == null)
            {
                GameObject hObj = new GameObject(listenName, typeof(RectTransform), typeof(Image), typeof(Button));
                hObj.transform.SetParent(btn.transform, false);
                var hRT = hObj.GetComponent<RectTransform>();
                hRT.anchorMin = new Vector2(1f, 0.5f);
                hRT.anchorMax = new Vector2(1f, 0.5f);
                hRT.pivot = new Vector2(1f, 0.5f);
                hRT.anchoredPosition = new Vector2(-14f, 0f);
                hRT.sizeDelta = new Vector2(98f, 50f);
                hObj.GetComponent<Image>().color = isPolite ? new Color(0.12f, 0.42f, 0.20f) : new Color(0.65f, 0.25f, 0.10f);

                GameObject hLbl = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                hLbl.transform.SetParent(hObj.transform, false);
                var lRT = hLbl.GetComponent<RectTransform>();
                lRT.anchorMin = Vector2.zero;
                lRT.anchorMax = Vector2.one;
                lRT.sizeDelta = Vector2.zero;
                var tmp = hLbl.GetComponent<TextMeshProUGUI>();
                tmp.text = "LISTEN";
                tmp.fontSize = 19;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;

                Button hBtn = hObj.GetComponent<Button>();
                hBtn.onClick.RemoveAllListeners();
                if (isPolite)
                {
                    politeAudioPreviewButton = hBtn;
                    politeAudioPreviewButton.onClick.AddListener(PlayPoliteAudioPreview);
                }
                else
                {
                    impoliteAudioPreviewButton = hBtn;
                    impoliteAudioPreviewButton.onClick.AddListener(PlayImpoliteAudioPreview);
                }
            }
            else
            {
                // Already exists: keep user's RectTransform completely untouched!
                Button hBtn = hT.GetComponent<Button>();
                if (hBtn != null)
                {
                    hBtn.onClick.RemoveAllListeners();
                    if (isPolite)
                    {
                        politeAudioPreviewButton = hBtn;
                        politeAudioPreviewButton.onClick.AddListener(PlayPoliteAudioPreview);
                    }
                    else
                    {
                        impoliteAudioPreviewButton = hBtn;
                        impoliteAudioPreviewButton.onClick.AddListener(PlayImpoliteAudioPreview);
                    }
                }
            }
        }

        private void SelectOrderWay(bool isPolite)
        {
            if (hasOrdered) return;
            hasOrdered = true;

            Button btn = isPolite ? politeOptionButton : impoliteOptionButton;
            StartCoroutine(SquishyButtonSelectRoutine(btn != null ? btn.transform : null, () =>
            {
                if (orderChoicePanel) orderChoicePanel.SetActive(false);
                StartCoroutine(OrderResolutionSequence(isPolite));
            }));
        }

        private IEnumerator SquishyButtonSelectRoutine(Transform btnTransform, System.Action onComplete)
        {
            if (btnTransform != null)
            {
                Vector3 original = btnTransform.localScale;
                float elapsed = 0f;
                float duration = 0.2f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    float s = 1f + Mathf.Sin(t * Mathf.PI) * 0.12f;
                    btnTransform.localScale = original * s;
                    yield return null;
                }
                btnTransform.localScale = original;
            }
            onComplete?.Invoke();
        }

        private IEnumerator OrderResolutionSequence(bool isPolite)
        {
            // Anu speaks her chosen order line out loud!
            string orderAudio = GetDishAudioId(selectedDish, isPolite);
            if (U6_AudioManager_Masters_Activity.Instance != null && !string.IsNullOrEmpty(orderAudio))
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO(orderAudio);
                yield return new WaitForSeconds(1.8f);
            }

            if (waiterFeedbackPopup) waiterFeedbackPopup.SetActive(true);

            if (isPolite)
            {
                if (waiterDialogText)
                    waiterDialogText.text = "Ravi smiles: \"Certainly!\"";

                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_WAIT_1");
                }
            }
            else
            {
                if (waiterDialogText)
                    waiterDialogText.text = "Ravi writes politely. Mother gives a gentle look.";

                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_MUM_1");
                }
            }

            yield return new WaitForSeconds(3.0f);

            if (waiterFeedbackPopup) waiterFeedbackPopup.SetActive(false);

            U6_GameManager_Masters_Activity.Instance.StartPart3();
        }
    }
}
