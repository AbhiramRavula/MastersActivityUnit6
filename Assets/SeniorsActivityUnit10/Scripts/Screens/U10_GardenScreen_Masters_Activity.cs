using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_GardenScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Header UI")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI weekStatusText;
        [SerializeField] private TextMeshProUGUI classNameText;
        [Tooltip("Type the class name here in the Inspector (e.g. Class 3-A, Class 4-B, Class 3 and 4).")]
        [SerializeField] private string customClassName = "Class 3 and 4";

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (classNameText != null && !string.IsNullOrEmpty(customClassName))
            {
                classNameText.text = customClassName;
            }
        }
#endif

        [Header("Plant Displays (4 Pots)")]
        [SerializeField] private List<U10_PlantDisplayUI_Masters_Activity> plantDisplays = new List<U10_PlantDisplayUI_Masters_Activity>();

        [Header("Kindness Jar")]
        [SerializeField] private Image kindnessJarImage;
        [SerializeField] private TextMeshProUGUI marbleCountText;
        [SerializeField] private Button addMarbleButton;

        [Header("Action Buttons")]
        [SerializeField] private Button weeklyCheckButton;
        [SerializeField] private Button endTermButton;
        [SerializeField] private Button showQuoteButton;
        [SerializeField] private Button teacherMenuButton;
        [SerializeField] private GameObject teacherMenuPanel;
        [SerializeField] private Button nextWeekButton;
        [SerializeField] private Button resetTermButton;
        [SerializeField] private Button closeTeacherMenuButton;

        [Header("Jar Sprites (Empty to Full)")]
        [SerializeField] private Sprite[] jarStageSprites; // 5 sprites

        [Header("Voiceover Audio Clips")]
        [SerializeField] private AudioClip voGardenWelcome;

        private void Start()
        {
            InitButtonListeners();
        }

        private void InitButtonListeners()
        {
            if (addMarbleButton != null)
            {
                addMarbleButton.onClick.RemoveAllListeners();
                addMarbleButton.onClick.AddListener(OnAddMarbleClicked);
                EnsureButtonAnimation(addMarbleButton);
            }

            if (weeklyCheckButton != null)
            {
                weeklyCheckButton.onClick.RemoveAllListeners();
                weeklyCheckButton.onClick.AddListener(OnWeeklyCheckClicked);
                EnsureButtonAnimation(weeklyCheckButton);
            }

            if (endTermButton != null)
            {
                endTermButton.onClick.RemoveAllListeners();
                endTermButton.onClick.AddListener(OnEndTermClicked);
                EnsureButtonAnimation(endTermButton);
            }

            if (showQuoteButton != null)
            {
                showQuoteButton.onClick.RemoveAllListeners();
                showQuoteButton.onClick.AddListener(OnShowQuoteClicked);
            }

            if (teacherMenuButton != null)
            {
                teacherMenuButton.onClick.RemoveAllListeners();
                teacherMenuButton.onClick.AddListener(ToggleTeacherMenu);
            }

            if (nextWeekButton != null)
            {
                nextWeekButton.onClick.RemoveAllListeners();
                nextWeekButton.onClick.AddListener(OnNextWeekClicked);
            }

            if (resetTermButton != null)
            {
                resetTermButton.onClick.RemoveAllListeners();
                resetTermButton.onClick.AddListener(OnResetTermClicked);
            }

            if (closeTeacherMenuButton != null)
            {
                closeTeacherMenuButton.onClick.RemoveAllListeners();
                closeTeacherMenuButton.onClick.AddListener(CloseTeacherMenu);
            }
        }

        public void RefreshGarden(U10_ClassGardenSaveData data)
        {
            if (data == null) return;

            if (titleText != null)
            {
                titleText.text = "The Golden Garden";
            }

            if (weekStatusText != null)
            {
                weekStatusText.text = $"Week {data.currentWeek} of 12";
            }

            if (classNameText != null)
            {
                classNameText.text = !string.IsNullOrEmpty(customClassName) ? customClassName : data.className;
            }

            // Update 4 Plant Pots
            for (int i = 0; i < plantDisplays.Count; i++)
            {
                if (i < data.chosenHabits.Count)
                {
                    plantDisplays[i].gameObject.SetActive(true);
                    plantDisplays[i].UpdateDisplay();
                }
                else
                {
                    plantDisplays[i].gameObject.SetActive(false);
                }
            }

            // Update Kindness Jar
            UpdateJarDisplay(data.totalMarbles);

            // Weekly Check Button: Dynamic action text and attention pulsing
            if (weeklyCheckButton != null)
            {
                var btnText = weeklyCheckButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = $"Start Week {data.currentWeek} Check-In!";
                }

                var pulse = weeklyCheckButton.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                if (pulse == null)
                {
                    pulse = weeklyCheckButton.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                }
                pulse.PopIn(0.15f);
            }

            // Button states
            if (endTermButton != null)
            {
                bool isTermEnd = data.currentWeek >= 12;
                endTermButton.gameObject.SetActive(isTermEnd);
                if (isTermEnd)
                {
                    var termPulse = endTermButton.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                    if (termPulse == null)
                    {
                        termPulse = endTermButton.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                    }
                    termPulse.PopIn(0.25f);
                }
            }

            // Ensure teacher menu starts closed
            if (teacherMenuPanel != null)
            {
                teacherMenuPanel.SetActive(false);
            }

            if (data.currentWeek == 1 && data.totalMarbles == 0)
            {
                if (voGardenWelcome != null)
                    U10_AudioManager_Masters_Activity.Instance?.PlayVOClip(voGardenWelcome);
                else
                    U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_01");
            }
        }

        public void UpdateJarDisplay(int totalMarbles)
        {
            if (marbleCountText != null)
            {
                marbleCountText.text = $"{totalMarbles} Marbles";
            }

            if (kindnessJarImage != null && jarStageSprites != null && jarStageSprites.Length > 0)
            {
                int stageIdx = 0;
                if (totalMarbles >= 50) stageIdx = 4;
                else if (totalMarbles >= 30) stageIdx = 3;
                else if (totalMarbles >= 15) stageIdx = 2;
                else if (totalMarbles >= 5) stageIdx = 1;

                stageIdx = Mathf.Clamp(stageIdx, 0, jarStageSprites.Length - 1);
                kindnessJarImage.sprite = jarStageSprites[stageIdx];
            }
        }

        private void OnAddMarbleClicked()
        {
            U10_GameManager_Masters_Activity.Instance?.AddKindnessMarble();
        }

        private void OnWeeklyCheckClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.ChangeState(U10_GardenState.WeeklyCheck);
        }

        private void OnEndTermClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.ChangeState(U10_GardenState.EndTerm);
        }

        private void OnShowQuoteClicked()
        {
            var gm = U10_GameManager_Masters_Activity.Instance;
            if (gm != null)
            {
                gm.ShowGoldenLineModal(null);
            }
        }

        private void ToggleTeacherMenu()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            if (teacherMenuPanel != null)
            {
                teacherMenuPanel.SetActive(!teacherMenuPanel.activeSelf);
            }
        }

        private void CloseTeacherMenu()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            if (teacherMenuPanel != null)
            {
                teacherMenuPanel.SetActive(false);
            }
        }

        public void AnimatePlantsGrowth()
        {
            var data = U10_GameManager_Masters_Activity.Instance?.ActiveData;
            if (data == null || data.chosenHabits == null) return;
            for (int i = 0; i < plantDisplays.Count; i++)
            {
                if (i < data.chosenHabits.Count && plantDisplays[i] != null)
                {
                    plantDisplays[i].TriggerGrowthAnimation(data.chosenHabits[i].growthStage);
                }
            }
        }

        private void OnNextWeekClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.AdvanceToNextWeek(true);
            if (teacherMenuPanel != null) teacherMenuPanel.SetActive(false);
        }

        private void OnResetTermClicked()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");
            U10_GameManager_Masters_Activity.Instance?.ResetTermData();
            if (teacherMenuPanel != null) teacherMenuPanel.SetActive(false);
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
