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

        private Coroutine bounceCoroutine;
        private Vector3 baseScale = Vector3.one;

        public void SetSelected(bool isSelected)
        {
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }

            // Slight elevation and scale pop for the chosen dish
            transform.localScale = isSelected ? baseScale * 1.05f : baseScale;
        }

        private void HandleClick()
        {
            OnCardSelected?.Invoke(this);

            // 1. Play delightful juicy pop chime
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_PopJuice");
            }

            // 2. Play squishy elastic bounce
            if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
            bounceCoroutine = StartCoroutine(SquishyBounceRoutine());
        }

        private System.Collections.IEnumerator SquishyBounceRoutine()
        {
            float elapsed = 0f;
            float duration = 0.22f;
            Vector3 startScale = baseScale * 1.15f;
            Vector3 endScale = (selectionHighlight != null && selectionHighlight.activeSelf) ? baseScale * 1.05f : baseScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Elastic bounce curve
                float curve = Mathf.Sin(t * Mathf.PI * 1.5f);
                transform.localScale = Vector3.Lerp(startScale, endScale, curve);
                yield return null;
            }

            transform.localScale = endScale;
            bounceCoroutine = null;
        }
    }
}
