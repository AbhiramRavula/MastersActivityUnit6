using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_EndTermScreen_Masters_Activity : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI celebrationHeader;
        [SerializeField] private TextMeshProUGUI certificateTitle;
        [SerializeField] private TextMeshProUGUI classNameText;
        [SerializeField] private TextMeshProUGUI threeEsQuoteText;
        [SerializeField] private TextMeshProUGUI statsSummaryText;
        [SerializeField] private Button returnToGardenButton;
        [SerializeField] private Button resetForNewTermButton;
        [SerializeField] private Image certificateBackground;

        [Header("Certificate Class Configuration")]
        [Tooltip("Type the class name here in the Inspector (e.g. Class 3-A, Class 4-B, Class 3 and 4).")]
        [SerializeField] private string customClassName = "Class 3 and 4";

        [Header("Celebration Plants")]
        [SerializeField] private List<U10_PlantDisplayUI_Masters_Activity> celebrationPlantDisplays = new List<U10_PlantDisplayUI_Masters_Activity>();

        [Header("Voiceover Audio Clips")]
        [SerializeField] private AudioClip voHarvestCelebration;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (classNameText != null && !string.IsNullOrEmpty(customClassName))
            {
                classNameText.text = $"Awarded to {customClassName}";
            }
        }
#endif

        private void Start()
        {
            if (returnToGardenButton != null)
            {
                returnToGardenButton.onClick.RemoveAllListeners();
                returnToGardenButton.onClick.AddListener(OnReturnToGardenClicked);
            }

            if (resetForNewTermButton != null)
            {
                resetForNewTermButton.onClick.RemoveAllListeners();
                resetForNewTermButton.onClick.AddListener(OnResetForNewTermClicked);
            }
        }

        public void ShowCelebration(U10_ClassGardenSaveData data)
        {
            U10_AudioManager_Masters_Activity.Instance?.PlayMusic("MUS_EndTerm", true);
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_JarFull");
            if (voHarvestCelebration != null)
                U10_AudioManager_Masters_Activity.Instance?.PlayVOClip(voHarvestCelebration);
            else
                U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_14");

            if (celebrationHeader != null)
            {
                celebrationHeader.text = "Golden Garden Harvest Celebration!";
            }

            string displayClass = !string.IsNullOrEmpty(customClassName) ? customClassName : (data != null && !string.IsNullOrEmpty(data.className) ? data.className : "Class 3 and 4");
            if (classNameText != null)
            {
                classNameText.text = $"Awarded to {displayClass}";
            }

            if (data != null)
            {
                data.className = displayClass;
            }

            if (threeEsQuoteText != null)
            {
                threeEsQuoteText.text = "THE THREE Es: ENERGY · ENTHUSIASM · EMPATHY\nAwarded for growing a golden garden";
            }

            if (statsSummaryText != null)
            {
                statsSummaryText.text = $"12 Weeks Completed   |   {data.totalMarbles} Kindness Marbles Collected   |   4 Golden Blooms Harvested";
            }

            // Animate celebration blooms to full Stage 11 Golden Bloom
            StartCoroutine(CelebrationTimeLapseRoutine(data));
        }

        private IEnumerator CelebrationTimeLapseRoutine(U10_ClassGardenSaveData data)
        {
            yield return new WaitForSeconds(0.5f);
            foreach (var plant in celebrationPlantDisplays)
            {
                if (plant != null && plant.gameObject.activeInHierarchy)
                {
                    plant.TriggerGrowthAnimation(11);
                    yield return new WaitForSeconds(0.4f);
                }
            }
        }

        private void OnReturnToGardenClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_AudioManager_Masters_Activity.Instance?.PlayMusic("MUS_Garden", true);
            U10_GameManager_Masters_Activity.Instance?.ChangeState(U10_GardenState.Garden);
        }

        private void OnResetForNewTermClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.ResetTermData();
        }
    }
}
