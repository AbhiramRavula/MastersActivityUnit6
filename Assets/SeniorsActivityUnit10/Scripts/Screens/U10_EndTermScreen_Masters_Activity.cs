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

        [Header("Celebration Plants")]
        [SerializeField] private List<U10_PlantDisplayUI_Masters_Activity> celebrationPlantDisplays = new List<U10_PlantDisplayUI_Masters_Activity>();

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
            U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_14");

            if (celebrationHeader != null)
            {
                celebrationHeader.text = "Golden Garden Harvest Celebration!";
            }

            if (classNameText != null)
            {
                classNameText.text = $"Awarded to {data.className}";
            }

            if (threeEsQuoteText != null)
            {
                threeEsQuoteText.text = "\"Excellence is not an act, but a habit. You have mastered The Three Es: Energy, Empathy, and Excellence.\"";
            }

            if (statsSummaryText != null)
            {
                statsSummaryText.text = $"12 Weeks Completed   |   {data.totalMarbles} Kindness Marbles Collected   |   4 Golden Blooms Harvested";
            }

            // Animate celebration blooms
            StartCoroutine(CelebrationTimeLapseRoutine(data));
        }

        private IEnumerator CelebrationTimeLapseRoutine(U10_ClassGardenSaveData data)
        {
            yield return new WaitForSeconds(0.5f);
            foreach (var plant in celebrationPlantDisplays)
            {
                if (plant != null && plant.gameObject.activeInHierarchy)
                {
                    plant.TriggerGrowthAnimation(6);
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
