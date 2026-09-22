using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_WeeklyCheckScreen_Masters_Activity : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI weekHeaderTitle;
        [SerializeField] private TextMeshProUGUI habitStepIndicatorText;
        [SerializeField] private Image habitIconImage;
        [SerializeField] private TextMeshProUGUI habitTitleText;
        [SerializeField] private TextMeshProUGUI habitPromptText;
        [SerializeField] private TextMeshProUGUI habitBookQuoteText;
        [SerializeField] private Button handsRaisedButton;
        [SerializeField] private Button skipHabitButton;
        [SerializeField] private TextMeshProUGUI statusFeedbackText;
        [SerializeField] private Button backToGardenButton;

        [Header("Habit Icons Database")]
        [SerializeField] private List<Sprite> allHabitIcons = new List<Sprite>();

        private List<U10_HabitData> habitsToCheck;
        private int currentHabitIndex = 0;
        private bool hasAnsweredCurrent = false;

        public void StartWeeklyCheck(List<U10_HabitData> habits, int weekNumber)
        {
            habitsToCheck = habits;
            currentHabitIndex = 0;
            hasAnsweredCurrent = false;

            if (weekHeaderTitle != null)
            {
                weekHeaderTitle.text = $"Week {weekNumber} Check-in";
            }

            if (handsRaisedButton != null)
            {
                handsRaisedButton.onClick.RemoveAllListeners();
                handsRaisedButton.onClick.AddListener(OnHandsRaisedClicked);
            }

            if (skipHabitButton != null)
            {
                skipHabitButton.onClick.RemoveAllListeners();
                skipHabitButton.onClick.AddListener(OnSkipHabitClicked);
            }

            if (backToGardenButton != null)
            {
                backToGardenButton.onClick.RemoveAllListeners();
                backToGardenButton.onClick.AddListener(OnBackToGardenClicked);
            }

            if (currentHabitIndex == 0)
            {
                U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_03");
            }

            ShowCurrentHabit();
        }

        private void ShowCurrentHabit()
        {
            if (habitsToCheck == null || habitsToCheck.Count == 0 || currentHabitIndex >= habitsToCheck.Count)
            {
                FinishWeeklyCheck();
                return;
            }

            hasAnsweredCurrent = false;
            var habit = habitsToCheck[currentHabitIndex];

            if (habitStepIndicatorText != null)
            {
                habitStepIndicatorText.text = $"Habit {currentHabitIndex + 1} of {habitsToCheck.Count}";
            }

            if (habitTitleText != null)
            {
                habitTitleText.text = habit.habitTitle;
            }

            if (habitPromptText != null)
            {
                habitPromptText.text = habit.habitPrompt;
            }

            if (habitBookQuoteText != null)
            {
                habitBookQuoteText.text = $"\"{habit.habitBookQuote}\"";
            }

            if (statusFeedbackText != null)
            {
                statusFeedbackText.text = "Raise your hand if you practiced this week!";
                statusFeedbackText.color = new Color(0.06f, 0.35f, 0.16f);
            }

            // Play specific habit prompt voiceover
            PlayHabitPromptVO(habit.habitKey);

            // Dynamically set correct habit icon
            if (habitIconImage != null)
            {
                Sprite targetIcon = null;
                if (allHabitIcons != null && allHabitIcons.Count > 0)
                {
                    targetIcon = allHabitIcons.Find(s => s != null && s.name.Equals($"Icon_{habit.habitKey}", StringComparison.OrdinalIgnoreCase));
                }
                if (targetIcon != null)
                {
                    habitIconImage.sprite = targetIcon;
                    habitIconImage.color = Color.white;
                }
            }
        }

        private void PlayHabitPromptVO(string habitKey)
        {
            if (string.IsNullOrEmpty(habitKey)) return;
            string key = habitKey.ToLower();
            if (key.Contains("water")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_04");
            else if (key.Contains("sleep")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_05");
            else if (key.Contains("outside")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_06");
            else if (key.Contains("read")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_07");
            else if (key.Contains("quiet")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_08");
            else if (key.Contains("walk")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_09");
            else if (key.Contains("give")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_10");
            else if (key.Contains("family")) U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_11");
        }

        public Sprite GetIconForHabit(string habitKey)
        {
            if (allHabitIcons != null)
            {
                string targetName = $"Icon_{habitKey}";
                var found = allHabitIcons.Find(s => s != null && s.name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
                if (found != null) return found;
            }
            return null;
        }

        private void OnHandsRaisedClicked()
        {
            if (hasAnsweredCurrent) return;
            hasAnsweredCurrent = true;

            var habit = habitsToCheck[currentHabitIndex];
            habit.totalCheckins++;

            int newStage = Mathf.Clamp((habit.totalCheckins / 2) + 1, 1, 6);
            habit.growthStage = newStage;

            if (statusFeedbackText != null)
            {
                statusFeedbackText.text = $"Wonderful! {habit.habitTitle} is growing!";
                statusFeedbackText.color = new Color(0.04f, 0.48f, 0.15f);
            }

            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tally");
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_PlantGrow");
            U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_12");

            Invoke(nameof(AdvanceToNextStep), 1.5f);
        }

        private void OnSkipHabitClicked()
        {
            if (hasAnsweredCurrent) return;
            hasAnsweredCurrent = true;

            if (statusFeedbackText != null)
            {
                statusFeedbackText.text = "Let us keep practicing next week!";
                statusFeedbackText.color = new Color(0.18f, 0.28f, 0.38f);
            }

            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            Invoke(nameof(AdvanceToNextStep), 0.8f);
        }

        private void AdvanceToNextStep()
        {
            currentHabitIndex++;
            if (currentHabitIndex < habitsToCheck.Count)
            {
                ShowCurrentHabit();
            }
            else
            {
                FinishWeeklyCheck();
            }
        }

        private void FinishWeeklyCheck()
        {
            var gm = U10_GameManager_Masters_Activity.Instance;
            if (gm != null)
            {
                gm.SaveCurrentGarden();
                gm.ShowGoldenLineModal(() =>
                {
                    gm.AdvanceToNextWeek();
                    gm.ChangeState(U10_GardenState.Garden);
                });
            }
        }

        private void OnBackToGardenClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.ChangeState(U10_GardenState.Garden);
        }
    }
}
