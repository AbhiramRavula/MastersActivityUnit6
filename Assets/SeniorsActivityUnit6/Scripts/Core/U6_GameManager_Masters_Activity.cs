using System;
using UnityEngine;

namespace Googolplex.Unit6
{
    public class U6_GameManager_Masters_Activity : MonoBehaviour
    {
        public static U6_GameManager_Masters_Activity Instance { get; private set; }

        [Header("Current State")]
        [SerializeField] private U6_GamePart currentPart = U6_GamePart.Intro;

        [Header("Star Tracking (Total: 3 Stars)")]
        [SerializeField] private int starCount = 0;

        [Header("Player Choices")]
        public string OrderedDishName = "Dosa";
        public Sprite OrderedDishSprite = null;

        public void SetOrderedDish(string name, Sprite sprite)
        {
            OrderedDishName = name;
            OrderedDishSprite = sprite;
            Debug.Log($"[U6_GameManager] Player ordered: {name}");
        }

        [Header("Screens")]
        [SerializeField] private GameObject liveTableScreen;
        [SerializeField] private GameObject menuScreen;
        [SerializeField] private GameObject choiceScreen;
        [SerializeField] private GameObject sliderScreen;
        [SerializeField] private GameObject endingScreen;

        // Events
        public event Action<U6_GamePart> OnPartChanged;
        public event Action<int> OnStarEarned;

        public U6_GamePart CurrentPart => currentPart;
        public int StarCount => starCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Auto-find screen references if left unassigned in manual hierarchy
            if (liveTableScreen == null)
            {
                var s = FindFirstObjectByType<U6_LiveTableScreen_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) liveTableScreen = s.gameObject;
            }
            if (menuScreen == null)
            {
                var s = FindFirstObjectByType<U6_MenuScreen_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) menuScreen = s.gameObject;
            }
            if (choiceScreen == null)
            {
                var s = FindFirstObjectByType<U6_WaiterInteractionScreen_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) choiceScreen = s.gameObject;
            }
            if (sliderScreen == null)
            {
                var s = FindFirstObjectByType<U6_SliderScreen_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) sliderScreen = s.gameObject;
            }
            if (endingScreen == null)
            {
                var s = FindFirstObjectByType<U6_EndingScreen_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) endingScreen = s.gameObject;
            }

            // Ensure AudioManager is present
            if (GetComponent<U6_AudioManager_Masters_Activity>() == null && FindFirstObjectByType<U6_AudioManager_Masters_Activity>() == null)
            {
                gameObject.AddComponent<U6_AudioManager_Masters_Activity>();
            }
        }

        private void Start()
        {
            ChangeState(U6_GamePart.Intro);
        }

        public void ChangeState(U6_GamePart nextPart)
        {
            if (nextPart != U6_GamePart.Intro && nextPart != U6_GamePart.Part1_Waiting)
            {
                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.StopAmbience();
                }
            }

            currentPart = nextPart;
            UpdateScreenVisibility();
            OnPartChanged?.Invoke(currentPart);
            Debug.Log($"[U6_GameManager] Transitioned to: {currentPart}");
        }

        public void AwardStar()
        {
            starCount = Mathf.Clamp(starCount + 1, 0, 3);
            OnStarEarned?.Invoke(starCount);
            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Star");
            }
            Debug.Log($"[U6_GameManager] Star Awarded! Total Stars: {starCount}/3");
        }

        private void UpdateScreenVisibility()
        {
            if (liveTableScreen) liveTableScreen.SetActive(false);
            if (menuScreen) menuScreen.SetActive(false);
            if (choiceScreen) choiceScreen.SetActive(false);
            if (sliderScreen) sliderScreen.SetActive(false);
            if (endingScreen) endingScreen.SetActive(false);

            switch (currentPart)
            {
                case U6_GamePart.Intro:
                case U6_GamePart.Part1_Waiting:
                    if (liveTableScreen) liveTableScreen.SetActive(true);
                    break;
                case U6_GamePart.Part2_Menu:
                    if (menuScreen) menuScreen.SetActive(true);
                    break;
                case U6_GamePart.Part3_Waiter:
                    if (choiceScreen) choiceScreen.SetActive(true);
                    break;
                case U6_GamePart.Part4_VoicesAndLeaving:
                    if (sliderScreen) sliderScreen.SetActive(true);
                    break;
                case U6_GamePart.Ending:
                    if (endingScreen) endingScreen.SetActive(true);
                    break;
            }
        }

        public void StartPart1() => ChangeState(U6_GamePart.Part1_Waiting);
        public void StartPart2() => ChangeState(U6_GamePart.Part2_Menu);
        public void StartPart3() => ChangeState(U6_GamePart.Part3_Waiter);
        public void StartPart4() => ChangeState(U6_GamePart.Part4_VoicesAndLeaving);
        public void StartEnding() => ChangeState(U6_GamePart.Ending);
    }
}
