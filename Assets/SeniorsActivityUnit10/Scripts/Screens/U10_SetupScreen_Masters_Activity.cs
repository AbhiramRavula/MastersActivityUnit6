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

        [Header("Card Theme & Selection Colors")]
        [SerializeField] private Color defaultCardColor = Color.white; // Pure white by default
        [SerializeField] private Color selectedCardColor = new Color(0.24f, 0.72f, 0.38f, 1f); // Vibrant green when selected
        [SerializeField] private Color defaultTitleColor = new Color(0.06f, 0.20f, 0.10f); // Dark Forest Green
        [SerializeField] private Color selectedTitleColor = Color.white; // Crisp White for maximum contrast
        [SerializeField] private Color defaultQuoteColor = new Color(0.15f, 0.32f, 0.20f); // Dark Slate
        [SerializeField] private Color selectedQuoteColor = new Color(0.94f, 0.99f, 0.95f); // Soft Mint White

        [Header("Voiceover Audio Clips")]
        [SerializeField] private AudioClip voSetupIntro;
        
        private HashSet<int> selectedIndices = new HashSet<int>();

        private void OnEnable()
        {
            if (voSetupIntro != null)
                U10_AudioManager_Masters_Activity.Instance?.PlayVOClip(voSetupIntro);
            else
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
                    btnImg.color = isSelected ? selectedCardColor : defaultCardColor;
                }

                if (habitTitleTexts.Count > i && habitTitleTexts[i] != null)
                {
                    habitTitleTexts[i].color = isSelected ? selectedTitleColor : defaultTitleColor;
                }

                if (habitQuoteTexts.Count > i && habitQuoteTexts[i] != null)
                {
                    habitQuoteTexts[i].color = isSelected ? selectedQuoteColor : defaultQuoteColor;
                }
            }

            if (selectionCountText != null)
            {
                selectionCountText.text = $"Selected: {selectedIndices.Count} of 4 Habits";
                selectionCountText.color = selectedIndices.Count == 4 ? new Color(0.1f, 0.55f, 0.2f) : new Color(0.25f, 0.3f, 0.25f);
            }

            if (plantGardenButton != null)
            {
                bool wasInteractable = plantGardenButton.interactable;
                bool nowInteractable = (selectedIndices.Count == 4);
                plantGardenButton.interactable = nowInteractable;

                if (!wasInteractable && nowInteractable)
                {
                    var pulse = plantGardenButton.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                    if (pulse == null) pulse = plantGardenButton.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                    pulse.PopIn(0f, 0.45f);
                }
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
