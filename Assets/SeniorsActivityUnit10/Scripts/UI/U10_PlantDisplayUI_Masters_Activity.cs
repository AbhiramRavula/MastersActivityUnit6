using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_PlantDisplayUI_Masters_Activity : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image potImage;
        [SerializeField] private Image plantImage;
        [SerializeField] private Image glowImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI stageBadgeText;

        private U10_HabitData boundHabit;
        private Sprite[] currentStageSprites; // 6 sprites for this plant type

        public U10_HabitData BoundHabit => boundHabit;

        public void BindHabit(U10_HabitData habit, Sprite[] stageSprites, Sprite potSprite)
        {
            boundHabit = habit;
            currentStageSprites = stageSprites;

            if (potImage != null && potSprite != null)
            {
                potImage.sprite = potSprite;
            }

            if (titleText != null)
            {
                titleText.text = habit.habitTitle;
            }

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (boundHabit == null || currentStageSprites == null || currentStageSprites.Length == 0) return;

            int stageIndex = Mathf.Clamp(boundHabit.growthStage - 1, 0, currentStageSprites.Length - 1);
            if (plantImage != null && stageIndex < currentStageSprites.Length)
            {
                plantImage.sprite = currentStageSprites[stageIndex];
                plantImage.preserveAspect = true;
            }

            if (stageBadgeText != null)
            {
                if (boundHabit.growthStage >= 6)
                {
                    stageBadgeText.text = "Golden Bloom";
                    stageBadgeText.color = new Color(1f, 0.88f, 0.2f);
                }
                else
                {
                    stageBadgeText.text = $"Stage {boundHabit.growthStage} of 6";
                    stageBadgeText.color = Color.white;
                }
            }

            if (glowImage != null)
            {
                glowImage.gameObject.SetActive(boundHabit.growthStage >= 6);
            }
        }

        public void TriggerGrowthAnimation(int targetStage)
        {
            if (boundHabit == null) return;
            boundHabit.growthStage = Mathf.Clamp(targetStage, 1, 6);
            StopAllCoroutines();
            StartCoroutine(AnimateGrowthRoutine());
        }

        private IEnumerator AnimateGrowthRoutine()
        {
            if (glowImage != null)
            {
                glowImage.gameObject.SetActive(true);
                glowImage.color = new Color(1f, 0.9f, 0.4f, 0.85f);
            }

            if (boundHabit.growthStage >= 5)
            {
                U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Flower");
            }
            else
            {
                U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_PlantGrow");
            }

            // Punch scale
            Vector3 originalScale = Vector3.one;
            float elapsed = 0f;
            float duration = 0.65f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float scaleMod = 1f + Mathf.Sin(t * Mathf.PI) * 0.25f;
                plantImage.transform.localScale = originalScale * scaleMod;
                yield return null;
            }

            plantImage.transform.localScale = originalScale;
            UpdateDisplay();

            if (glowImage != null && boundHabit.growthStage < 6)
            {
                glowImage.gameObject.SetActive(false);
            }
        }
    }
}
