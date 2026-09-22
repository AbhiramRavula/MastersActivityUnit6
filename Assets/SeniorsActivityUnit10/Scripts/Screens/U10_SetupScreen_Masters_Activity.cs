using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_SetupScreen_Masters_Activity : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI headerTitleText;
        [SerializeField] private TextMeshProUGUI selectionCountText;
        [SerializeField] private Button plantGardenButton;
        [SerializeField] private Transform cardsContainer;

        [Header("Card Template / Buttons")]
        [SerializeField] private List<Button> habitOptionButtons = new List<Button>();
        [SerializeField] private List<Image> habitCheckmarkIcons = new List<Image>();
        [SerializeField] private List<TextMeshProUGUI> habitTitleTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<TextMeshProUGUI> habitQuoteTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<Image> habitIconImages = new List<Image>();

        private HashSet<int> selectedIndices = new HashSet<int>();

        private void OnEnable()
        {
            U10_AudioManager_Masters_Activity.Instance?.PlayVO("VO_U10_02");
        }

        private void Start()
        {
            InitCards();
            UpdateUI();
        }

        public void InitCards()
        {
            selectedIndices.Clear();

            for (int i = 0; i < habitOptionButtons.Count; i++)
            {
                int index = i;
                if (index < U10_SaveSystem.ALL_AVAILABLE_HABITS.Length)
                {
                    var habit = U10_SaveSystem.ALL_AVAILABLE_HABITS[index];
                    if (habitTitleTexts.Count > index && habitTitleTexts[index] != null)
                        habitTitleTexts[index].text = habit.title;
                    if (habitQuoteTexts.Count > index && habitQuoteTexts[index] != null)
                        habitQuoteTexts[index].text = habit.quote;

                    habitOptionButtons[index].onClick.RemoveAllListeners();
                    habitOptionButtons[index].onClick.AddListener(() => OnHabitCardClicked(index));
                }
            }

            if (plantGardenButton != null)
            {
                plantGardenButton.onClick.RemoveAllListeners();
                plantGardenButton.onClick.AddListener(OnPlantGardenClicked);
            }
        }

        private void OnHabitCardClicked(int index)
        {
            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Tap");

            if (selectedIndices.Contains(index))
            {
                selectedIndices.Remove(index);
            }
            else
            {
                if (selectedIndices.Count < 4)
                {
                    selectedIndices.Add(index);
                }
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int i = 0; i < habitOptionButtons.Count; i++)
            {
                bool isSelected = selectedIndices.Contains(i);
                if (habitCheckmarkIcons.Count > i && habitCheckmarkIcons[i] != null)
                {
                    habitCheckmarkIcons[i].gameObject.SetActive(isSelected);
                }

                var btnImg = habitOptionButtons[i].GetComponent<Image>();
                if (btnImg != null)
                {
                    btnImg.color = isSelected ? new Color(0.88f, 0.98f, 0.9f, 1f) : Color.white;
                }
            }

            if (selectionCountText != null)
            {
                selectionCountText.text = $"Selected: {selectedIndices.Count} of 4 Habits";
                selectionCountText.color = selectedIndices.Count == 4 ? new Color(0.1f, 0.55f, 0.2f) : new Color(0.25f, 0.3f, 0.25f);
            }

            if (plantGardenButton != null)
            {
                plantGardenButton.interactable = (selectedIndices.Count == 4);
            }
        }

        private void OnPlantGardenClicked()
        {
            if (selectedIndices.Count != 4) return;

            U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_PlantGrow");

            var gm = U10_GameManager_Masters_Activity.Instance;
            if (gm != null)
            {
                List<U10_HabitData> chosen = new List<U10_HabitData>();
                foreach (int idx in selectedIndices)
                {
                    var info = U10_SaveSystem.ALL_AVAILABLE_HABITS[idx];
                    chosen.Add(new U10_HabitData(info.key, info.title, info.prompt, info.quote));
                }

                gm.CompleteSetup(chosen);
            }
        }
    }
}
