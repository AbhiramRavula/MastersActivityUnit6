using System;
using System.Collections.Generic;
using UnityEngine;

namespace Googolplex.Unit10
{
    public enum U10_GardenState
    {
        Setup,
        Garden,
        WeeklyCheck,
        EndTerm
    }

    public class U10_GameManager_Masters_Activity : MonoBehaviour
    {
        public static U10_GameManager_Masters_Activity Instance { get; private set; }

        [Header("State")]
        [SerializeField] private U10_GardenState currentState = U10_GardenState.Garden;

        [Header("Screens")]
        [SerializeField] private GameObject setupScreen;
        [SerializeField] private GameObject gardenScreen;
        [SerializeField] private GameObject weeklyCheckScreen;
        [SerializeField] private GameObject endTermScreen;
        [SerializeField] private GameObject goldenLineModal;

        [Header("Sprites Database")]
        [SerializeField] private Sprite potSprite;
        [SerializeField] private List<Sprite> allPlantSprites = new List<Sprite>();

        private U10_ClassGardenSaveData activeData;

        public U10_ClassGardenSaveData ActiveData => activeData;
        public U10_GardenState CurrentState => currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadOrInitData();
        }

        private void Start()
        {
            if (!activeData.isSetupCompleted || activeData.chosenHabits == null || activeData.chosenHabits.Count != 4)
            {
                ChangeState(U10_GardenState.Setup);
            }
            else
            {
                ChangeState(U10_GardenState.Garden);
            }
        }

        public void LoadOrInitData()
        {
            activeData = U10_SaveSystem.Load();
        }

        public void ChangeState(U10_GardenState newState)
        {
            currentState = newState;

            if (setupScreen) setupScreen.SetActive(false);
            if (gardenScreen) gardenScreen.SetActive(false);
            if (weeklyCheckScreen) weeklyCheckScreen.SetActive(false);
            if (endTermScreen) endTermScreen.SetActive(false);

            switch (currentState)
            {
                case U10_GardenState.Setup:
                    if (setupScreen) setupScreen.SetActive(true);
                    break;

                case U10_GardenState.Garden:
                    if (gardenScreen)
                    {
                        gardenScreen.SetActive(true);
                        var gScript = gardenScreen.GetComponent<U10_GardenScreen_Masters_Activity>();
                        if (gScript != null)
                        {
                            InitPlantDisplays(gScript);
                            gScript.RefreshGarden(activeData);
                        }
                    }
                    break;

                case U10_GardenState.WeeklyCheck:
                    if (weeklyCheckScreen)
                    {
                        weeklyCheckScreen.SetActive(true);
                        var wScript = weeklyCheckScreen.GetComponent<U10_WeeklyCheckScreen_Masters_Activity>();
                        if (wScript != null)
                        {
                            wScript.StartWeeklyCheck(activeData.chosenHabits, activeData.currentWeek);
                        }
                    }
                    break;

                case U10_GardenState.EndTerm:
                    if (endTermScreen)
                    {
                        endTermScreen.SetActive(true);
                        var eScript = endTermScreen.GetComponent<U10_EndTermScreen_Masters_Activity>();
                        if (eScript != null)
                        {
                            eScript.ShowCelebration(activeData);
                        }
                    }
                    break;
            }

            Debug.Log($"[U10_GameManager] State changed to: {currentState}");
        }

        public void CompleteSetup(List<U10_HabitData> chosen)
        {
            activeData.chosenHabits = chosen;
            activeData.isSetupCompleted = true;
            activeData.currentWeek = 1;
            U10_SaveSystem.Save(activeData);
            ChangeState(U10_GardenState.Garden);
        }

        public void AddKindnessMarble()
        {
            activeData.totalMarbles++;
            U10_SaveSystem.Save(activeData);

            if (activeData.totalMarbles >= 50)
            {
                U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_JarFull");
            }
            else
            {
                U10_AudioManager_Masters_Activity.Instance?.PlaySFX("SFX_Marble");
            }

            if (gardenScreen != null && gardenScreen.activeInHierarchy)
            {
                var gScript = gardenScreen.GetComponent<U10_GardenScreen_Masters_Activity>();
                gScript?.UpdateJarDisplay(activeData.totalMarbles);
            }
        }

        public void AdvanceToNextWeek()
        {
            if (activeData.currentWeek < 12)
            {
                activeData.currentWeek++;
            }
            U10_SaveSystem.Save(activeData);

            if (currentState == U10_GardenState.Garden && gardenScreen != null)
            {
                var gScript = gardenScreen.GetComponent<U10_GardenScreen_Masters_Activity>();
                gScript?.RefreshGarden(activeData);
            }
        }

        public void ResetTermData()
        {
            U10_SaveSystem.ClearSave();
            activeData = U10_SaveSystem.CreateDefaultData();
            ChangeState(U10_GardenState.Setup);
        }

        public void SaveCurrentGarden()
        {
            U10_SaveSystem.Save(activeData);
        }

        public void ShowGoldenLineModal(Action onClose)
        {
            if (goldenLineModal != null)
            {
                var modal = goldenLineModal.GetComponent<U10_GoldenLineModal_Masters_Activity>();
                if (modal != null)
                {
                    modal.ShowQuote(activeData.currentWeek, onClose);
                }
            }
            else
            {
                onClose?.Invoke();
            }
        }

        private void InitPlantDisplays(U10_GardenScreen_Masters_Activity gScript)
        {
            if (activeData.chosenHabits == null) return;

            var plantDisplays = gScript.GetComponentsInChildren<U10_PlantDisplayUI_Masters_Activity>(true);
            for (int i = 0; i < plantDisplays.Length; i++)
            {
                if (i < activeData.chosenHabits.Count)
                {
                    var habit = activeData.chosenHabits[i];
                    Sprite[] stageSprites = GetSpritesForHabit(habit.habitKey);
                    plantDisplays[i].BindHabit(habit, stageSprites, potSprite);
                }
            }
        }

        public Sprite[] GetSpritesForHabit(string habitKey)
        {
            List<Sprite> list = new List<Sprite>();
            for (int stage = 1; stage <= 6; stage++)
            {
                string targetName = $"Plant_{habitKey}_{stage}";
                Sprite s = allPlantSprites.Find(x => x != null && x.name == targetName);
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }
    }
}
