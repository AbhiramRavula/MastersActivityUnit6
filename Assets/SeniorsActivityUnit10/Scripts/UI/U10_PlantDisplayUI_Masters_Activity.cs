using System;
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
                potImage.preserveAspect = true;
                potImage.enabled = true;
            }

            if (titleText != null && habit != null)
            {
                titleText.text = habit.habitTitle;
            }

            // Fallback: If stageSprites is empty, fetch directly from GameManager
            if ((currentStageSprites == null || currentStageSprites.Length == 0) && habit != null)
            {
                currentStageSprites = U10_GameManager_Masters_Activity.Instance?.GetSpritesForHabit(habit.habitKey);
            }

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (boundHabit == null) return;

            // Ensure sprites are populated
            if (currentStageSprites == null || currentStageSprites.Length == 0)
            {
                currentStageSprites = U10_GameManager_Masters_Activity.Instance?.GetSpritesForHabit(boundHabit.habitKey);
            }

            if (plantImage != null)
            {
                plantImage.enabled = true;
                plantImage.gameObject.SetActive(true);
                plantImage.preserveAspect = true;

                if (currentStageSprites != null && currentStageSprites.Length > 0)
                {
                    int stageIndex = Mathf.Clamp(boundHabit.growthStage - 1, 0, currentStageSprites.Length - 1);
                    if (stageIndex < currentStageSprites.Length && currentStageSprites[stageIndex] != null)
                    {
                        plantImage.sprite = currentStageSprites[stageIndex];
                    }
                }
            }

            if (titleText != null && boundHabit != null)
            {
                titleText.text = boundHabit.habitTitle;
            }

            if (stageBadgeText != null && boundHabit != null)
            {
                var bgImg = stageBadgeText.GetComponentInParent<Image>();
                if (bgImg != null)
                {
                    if (bgImg.sprite != null) bgImg.type = Image.Type.Sliced;
                    if (boundHabit.growthStage >= 11)
                    {
                        bgImg.color = new Color(0.78f, 0.58f, 0.08f, 0.95f);
                    }
                    else
                    {
                        bgImg.color = new Color(0.12f, 0.45f, 0.22f, 0.95f);
                    }
                }

                if (boundHabit.growthStage >= 11)
                {
                    stageBadgeText.text = "Golden Bloom";
                    stageBadgeText.color = new Color(1f, 0.95f, 0.7f);
                }
                else
                {
                    stageBadgeText.text = $"Stage {boundHabit.growthStage} of 11";
                    stageBadgeText.color = Color.white;
                }
            }

            // Glow image only activated at full bloom (Stage 11) if it has a valid soft halo sprite
            if (glowImage != null)
            {
                bool hasHaloSprite = glowImage.sprite != null && !glowImage.sprite.name.Contains("White") && !glowImage.sprite.name.Contains("Card");
                glowImage.gameObject.SetActive(hasHaloSprite && boundHabit.growthStage >= 11);
            }
        }

        public void TriggerGrowthAnimation(int targetStage)
        {
            if (boundHabit == null) return;
            boundHabit.growthStage = Mathf.Clamp(targetStage, 1, 11);
            StopAllCoroutines();
            StartCoroutine(AnimateGrowthRoutine());
        }

        private IEnumerator AnimateGrowthRoutine()
        {
            // Only show glow if it's a valid soft sprite
            if (glowImage != null && glowImage.sprite != null && !glowImage.sprite.name.Contains("White") && !glowImage.sprite.name.Contains("Card"))
            {
                glowImage.gameObject.SetActive(true);
                glowImage.color = new Color(1f, 0.9f, 0.4f, 0.6f);
            }
            else if (glowImage != null)
            {
                glowImage.gameObject.SetActive(false);
            }

            if (boundHabit.growthStage >= 10)
            {
                U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Flower");
            }
            else
            {
                U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_PlantGrow");
            }

            // Update sprite to new stage immediately so the new plant is visibly growing
            UpdateDisplay();

            // Punch scale on the plant
            if (plantImage != null)
            {
                Vector3 originalScale = Vector3.one;
                float elapsed = 0f;
                float duration = 0.65f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    float scaleMod = 1f + Mathf.Sin(t * Mathf.PI) * 0.28f;
                    plantImage.transform.localScale = originalScale * scaleMod;
                    yield return null;
                }

                plantImage.transform.localScale = originalScale;
            }

            UpdateDisplay();

            if (glowImage != null && boundHabit.growthStage < 11)
            {
                glowImage.gameObject.SetActive(false);
            }
        }
    }
}
