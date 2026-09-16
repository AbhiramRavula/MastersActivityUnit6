using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Waiter Reaction Feedback")]
        [SerializeField] private GameObject waiterFeedbackPopup;
        [SerializeField] private TextMeshProUGUI waiterDialogText;

        private List<U6_DishCardUI_Masters_Activity> spawnedCards = new List<U6_DishCardUI_Masters_Activity>();
        private U6_DishItemData_Masters_Activity selectedDish = null;
        private bool hasOrdered = false;

        private void Awake()
        {
            if (callWaiterButton != null)
                callWaiterButton.onClick.AddListener(OnCallWaiterClicked);

            if (politeOptionButton != null)
                politeOptionButton.onClick.AddListener(() => SelectOrderWay(true));

            if (impoliteOptionButton != null)
                impoliteOptionButton.onClick.AddListener(() => SelectOrderWay(false));
        }

        private void OnEnable()
        {
            InitializeDefaultDishes();
            ResetScreen();
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_MenuOpen");
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_05"); // "Now choose. What will Anu eat?"
            }
        }

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
        }

        private void HandleDishSelected(U6_DishCardUI_Masters_Activity selectedCard)
        {
            selectedDish = selectedCard.Data;
            foreach (var card in spawnedCards)
            {
                card.SetSelected(card == selectedCard);
            }
            Debug.Log($"[U6_MenuScreen] Selected dish: {selectedDish.dishName}");
        }

        private void OnCallWaiterClicked()
        {
            if (selectedDish == null)
            {
                StartCoroutine(AwkwardOrderTooEarlySequence());
            }
            else
            {
                ShowOrderOptions();
            }
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
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_PadWrite");
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_WAIT_1");
                }
            }
            else
            {
                if (waiterDialogText)
                    waiterDialogText.text = "Ravi writes politely. Mother gives a gentle look.";

                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_PadWrite");
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_MUM_1");
                }
            }

            yield return new WaitForSeconds(3.0f);

            if (waiterFeedbackPopup) waiterFeedbackPopup.SetActive(false);

            U6_GameManager_Masters_Activity.Instance.StartPart3();
        }
    }
}
