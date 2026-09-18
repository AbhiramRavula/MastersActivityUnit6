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

        private List<U6_DishCardUI_Masters_Activity> spawnedCards = new List<U6_DishCardUI_Masters_Activity>();
        private U6_DishItemData_Masters_Activity selectedDish = null;
        private bool hasOrdered = false;

        private void Awake()
        {
            AutoFindUIReferences();

            if (callWaiterButton != null)
                callWaiterButton.onClick.AddListener(OnCallWaiterClicked);

            if (politeOptionButton != null)
                politeOptionButton.onClick.AddListener(() => SelectOrderWay(true));

            if (impoliteOptionButton != null)
                impoliteOptionButton.onClick.AddListener(() => SelectOrderWay(false));
        }

        private void AutoFindUIReferences()
        {
            if (waiterStandingVisual == null)
            {
                Transform t = transform.Find("Waiter_Image");
                if (t == null) t = transform.Find("WaiterStandingVisual");
                if (t != null) waiterStandingVisual = t.GetComponent<Image>();
            }

            if (waiterStandingVisual != null)
            {
                waiterStandingVisual.gameObject.SetActive(false);
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

            if (readFirstPrompt) readFirstPrompt.SetActive(false);
            if (awkwardWaiterOverlay) awkwardWaiterOverlay.SetActive(false);
            if (orderChoicePanel) orderChoicePanel.SetActive(false);
            if (waiterFeedbackPopup) waiterFeedbackPopup.SetActive(false);
            if (waiterStandingVisual) waiterStandingVisual.gameObject.SetActive(false);
        }

        private void HandleDishSelected(U6_DishCardUI_Masters_Activity selectedCard)
        {
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
            Debug.Log($"[U6_MenuScreen] Selected dish: {selectedDish.dishName}");
        }

        private void OnCallWaiterClicked()
        {
            if (selectedDish == null)
            {
                // No item selected yet -> Keep waiter hidden and trigger awkward stammer / Read First prompt
                if (waiterStandingVisual) waiterStandingVisual.gameObject.SetActive(false);
                StartCoroutine(AwkwardOrderTooEarlySequence());
            }
            else
            {
                // Dish IS selected -> Waiter appears with his notepad ready to take the order!
                if (waiterStandingVisual)
                {
                    waiterStandingVisual.gameObject.SetActive(true);
                    StartCoroutine(PopAvatarAnimation(waiterStandingVisual.transform));
                    Debug.Log($"[U6_MenuScreen] Waiter Ravi stepped in to take order for: {selectedDish.dishName}");
                }
                ShowOrderOptions();
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

        private IEnumerator AwkwardOrderTooEarlySequence()
        {
            if (callWaiterButton) callWaiterButton.interactable = false;

            if (awkwardWaiterOverlay) awkwardWaiterOverlay.SetActive(true);
            if (anuStammerText) anuStammerText.text = "Ummm... ummm...";

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_ANU_3");
            }

            yield return new WaitForSeconds(3.5f);

            if (awkwardWaiterOverlay) awkwardWaiterOverlay.SetActive(false);

            if (readFirstPrompt)
            {
                readFirstPrompt.SetActive(true);
                if (readFirstText) readFirstText.text = "READ FIRST";
            }

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_06");
            }

            yield return new WaitForSeconds(2.5f);

            if (readFirstPrompt) readFirstPrompt.SetActive(false);
            if (callWaiterButton) callWaiterButton.interactable = true;
        }

        private void ShowOrderOptions()
        {
            if (orderChoicePanel == null) return;

            string dish = selectedDish != null ? selectedDish.dishName.ToLower() : "dish";

            if (politeOptionText)
                politeOptionText.text = $"\"Could I have the {dish}, please?\"";

            if (impoliteOptionText)
                impoliteOptionText.text = $"\"I want {dish}.\"";

            orderChoicePanel.SetActive(true);

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_07");
            }
        }

        private void SelectOrderWay(bool isPolite)
        {
            if (hasOrdered) return;
            hasOrdered = true;
            orderChoicePanel.SetActive(false);

            StartCoroutine(OrderResolutionSequence(isPolite));
        }

        private IEnumerator OrderResolutionSequence(bool isPolite)
        {
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
