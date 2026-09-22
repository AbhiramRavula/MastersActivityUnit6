using System;
using System.Collections;
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
        [SerializeField] private Button closeCardButton;

        [Header("Habit Icons Database")]
        [SerializeField] private List<Sprite> allHabitIcons = new List<Sprite>();

        [Header("Voiceover Clips (Inspector Assigned)")]
        [SerializeField] private AudioClip voWeeklyCheckStart;
        [SerializeField] private AudioClip voWater;
        [SerializeField] private AudioClip voSleep;
        [SerializeField] private AudioClip voOutside;
        [SerializeField] private AudioClip voRead;
        [SerializeField] private AudioClip voQuiet;
        [SerializeField] private AudioClip voWalk;
        [SerializeField] private AudioClip voGive;
        [SerializeField] private AudioClip voFamily;
        [SerializeField] private AudioClip voPlantGrowing;

        private List<U10_HabitData> habitsToCheck;
        private int currentHabitIndex = 0;
        private bool hasAnsweredCurrent = false;
        private Coroutine audioGateRoutine;

        private void Awake()
        {
            EnsureButtonAnimation(handsRaisedButton);
            EnsureButtonAnimation(skipHabitButton);
            EnsureButtonAnimation(closeCardButton);
            EnsureButtonAnimation(backToGardenButton);
        }

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

            if (closeCardButton != null)
            {
                closeCardButton.onClick.RemoveAllListeners();
                closeCardButton.onClick.AddListener(OnBackToGardenClicked);
            }

            if (currentHabitIndex == 0)
            {
                if (voWeeklyCheckStart != null)
                    U10_AudioManager_Masters_Activity.Instance?.PlayVOClip(voWeeklyCheckStart);
                else
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

            // Play specific habit prompt voiceover & start audio-gate lock
            float voDuration = PlayHabitPromptVO(habit.habitKey);

            if (audioGateRoutine != null) StopCoroutine(audioGateRoutine);
            audioGateRoutine = StartCoroutine(WaitForVOThenUnlockButtons(voDuration));
        }

        private float PlayHabitPromptVO(string habitKey)
        {
            if (string.IsNullOrEmpty(habitKey)) return 0f;
            string key = habitKey.ToLower();
            AudioClip targetClip = null;
            string voKey = null;

            if (key.Contains("water")) { targetClip = voWater; voKey = "VO_U10_04"; }
            else if (key.Contains("sleep")) { targetClip = voSleep; voKey = "VO_U10_05"; }
            else if (key.Contains("outside")) { targetClip = voOutside; voKey = "VO_U10_06"; }
            else if (key.Contains("read")) { targetClip = voRead; voKey = "VO_U10_07"; }
            else if (key.Contains("quiet")) { targetClip = voQuiet; voKey = "VO_U10_08"; }
            else if (key.Contains("walk")) { targetClip = voWalk; voKey = "VO_U10_09"; }
            else if (key.Contains("give")) { targetClip = voGive; voKey = "VO_U10_10"; }
            else if (key.Contains("family")) { targetClip = voFamily; voKey = "VO_U10_11"; }

            if (targetClip != null)
            {
                return U10_AudioManager_Masters_Activity.Instance?.PlayVOClip(targetClip) ?? targetClip.length;
            }
            else if (!string.IsNullOrEmpty(voKey))
            {
                return U10_AudioManager_Masters_Activity.Instance?.PlayVO(voKey) ?? 2.5f;
            }
            return 2.0f;
        }

        private IEnumerator WaitForVOThenUnlockButtons(float duration)
        {
            // Lock buttons while audio is playing
            if (handsRaisedButton != null)
            {
                handsRaisedButton.interactable = false;
                handsRaisedButton.transform.localScale = Vector3.one * 0.95f;
            }
            if (skipHabitButton != null)
            {
                skipHabitButton.interactable = false;
                skipHabitButton.transform.localScale = Vector3.one * 0.95f;
            }

            if (statusFeedbackText != null)
            {
                statusFeedbackText.text = "Listen carefully to the habit question...";
                statusFeedbackText.color = new Color(0.35f, 0.45f, 0.4f);
            }

            // Wait for full audio duration
            float waitTime = Mathf.Max(duration + 0.2f, 1.2f);
            yield return new WaitForSeconds(waitTime);

            // Unlock and smoothly pop-in buttons with attention animation!
            if (handsRaisedButton != null)
            {
                handsRaisedButton.interactable = true;
                var pulse = handsRaisedButton.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                if (pulse != null) pulse.PopIn(0f, 0.45f);
                else handsRaisedButton.transform.localScale = Vector3.one;
            }

            if (skipHabitButton != null)
            {
                skipHabitButton.interactable = true;
                skipHabitButton.transform.localScale = Vector3.one;
            }

            if (statusFeedbackText != null)
            {
                statusFeedbackText.text = "Raise your hand if you practiced this week!";
                statusFeedbackText.color = new Color(0.06f, 0.45f, 0.16f);
            }

            audioGateRoutine = null;
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

            if (audioGateRoutine != null)
            {
                StopCoroutine(audioGateRoutine);
                audioGateRoutine = null;
            }

            var habit = habitsToCheck[currentHabitIndex];
            habit.totalCheckins++;

            int newStage = Mathf.Clamp((habit.totalCheckins / 2) + 1, 1, 6);
            habit.growthStage = newStage;

            if (statusFeedbackText != null)
            {
                statusFeedbackText.text = $"Wonderful! {habit.habitTitle} is growing!";
                statusFeedbackText.color = new Color(0.04f, 0.52f, 0.15f);
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

            if (audioGateRoutine != null)
            {
                StopCoroutine(audioGateRoutine);
                audioGateRoutine = null;
            }

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
            if (audioGateRoutine != null)
            {
                StopCoroutine(audioGateRoutine);
                audioGateRoutine = null;
            }

            U10_AudioManager_Masters_Activity.Instance?.StopVO();
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.ChangeState(U10_GardenState.Garden);
        }

        private void EnsureButtonAnimation(Button btn)
        {
            if (btn != null && btn.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>() == null)
            {
                btn.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
            }
        }
    }
}
