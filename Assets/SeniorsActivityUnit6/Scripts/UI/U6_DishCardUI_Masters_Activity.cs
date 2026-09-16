using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    public class U6_DishCardUI_Masters_Activity : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image dishIcon;
        [SerializeField] private TextMeshProUGUI dishNameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private GameObject selectionHighlight;
        [SerializeField] private Button cardButton;

        private U6_DishItemData_Masters_Activity currentData;
        public U6_DishItemData_Masters_Activity Data => currentData;

        public event Action<U6_DishCardUI_Masters_Activity> OnCardSelected;

        private void Awake()
        {
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(HandleClick);
            }
        }

        public void Setup(U6_DishItemData_Masters_Activity data)
        {
            currentData = data;
            if (dishNameText != null) dishNameText.text = data.dishName;
            if (priceText != null) priceText.text = $"Rs. {data.price}";
            if (dishIcon != null && data.dishSprite != null)
            {
                dishIcon.sprite = data.dishSprite;
                dishIcon.enabled = true;
            }
            SetSelected(false);
        }

        public void SetSelected(bool isSelected)
        {
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }
        }

        private void HandleClick()
        {
            OnCardSelected?.Invoke(this);
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Chirp");
            }
        }
    }
}
