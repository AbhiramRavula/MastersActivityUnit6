using System;
using UnityEngine;

namespace Googolplex.Unit8
{
    public enum U8_GamePart
    {
        Part1_Door = 0,
        Part2_Inside = 1,
        HandwashScreen = 2,
        Part3_AfterYou = 3,
        Part4_EmptySoap = 4,
        Ending = 5
    }

    public class U8_GameManager_Masters_Activity : MonoBehaviour
    {
        public static U8_GameManager_Masters_Activity Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private U8_GamePart currentPart = U8_GamePart.Part1_Door;
        [SerializeField] private int starCount = 0;

        [Header("Part 2 Washroom Step Flags")]
        public bool isCubicleClosedFirst = false;
        public bool isFlushed = false;
        public bool isHandsWashed = false;
        public bool isTapTurnedOff = false;
        public bool isTowelInBin = false;
        public bool isSinkWiped = false;

        [Header("Screen References")]
        [SerializeField] private GameObject part1DoorScreen;
        [SerializeField] private GameObject part2InsideScreen;
        [SerializeField] private GameObject handwashScreen;
        [SerializeField] private GameObject part3AfterYouScreen;
        [SerializeField] private GameObject part4EmptySoapScreen;
        [SerializeField] private GameObject endingScreen;

        // Events
        public event Action<U8_GamePart> OnPartChanged;
        public event Action<int> OnStarEarned;

        public U8_GamePart CurrentPart => currentPart;
        public int StarCount => starCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            AutoFindScreens();
        }

        private void Start()
        {
            ShowPart(U8_GamePart.Part1_Door);
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlayAmbience("AMB_Washroom");
            }
        }

        private void AutoFindScreens()
        {
            if (part1DoorScreen == null)
            {
                var s = FindFirstObjectByType<U8_Part1_DoorController_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) part1DoorScreen = s.gameObject;
            }
            if (part2InsideScreen == null)
            {
                var s = FindFirstObjectByType<U8_Part2_WashroomInsideController_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) part2InsideScreen = s.gameObject;
            }
            if (handwashScreen == null)
            {
                var s = FindFirstObjectByType<U8_HandwashScreenController_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) handwashScreen = s.gameObject;
            }
            if (part3AfterYouScreen == null)
            {
                var s = FindFirstObjectByType<U8_Part3_AfterYouController_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) part3AfterYouScreen = s.gameObject;
            }
            if (part4EmptySoapScreen == null)
            {
                var s = FindFirstObjectByType<U8_Part4_EmptySoapController_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) part4EmptySoapScreen = s.gameObject;
            }
            if (endingScreen == null)
            {
                var s = FindFirstObjectByType<U8_EndingScreen_Masters_Activity>(FindObjectsInactive.Include);
                if (s != null) endingScreen = s.gameObject;
            }
        }

        public void ShowPart(U8_GamePart part)
        {
            currentPart = part;
            AutoFindScreens();

            if (part1DoorScreen != null) part1DoorScreen.SetActive(part == U8_GamePart.Part1_Door);
            if (part2InsideScreen != null) part2InsideScreen.SetActive(part == U8_GamePart.Part2_Inside);
            if (handwashScreen != null) handwashScreen.SetActive(part == U8_GamePart.HandwashScreen);
            if (part3AfterYouScreen != null) part3AfterYouScreen.SetActive(part == U8_GamePart.Part3_AfterYou);
            if (part4EmptySoapScreen != null) part4EmptySoapScreen.SetActive(part == U8_GamePart.Part4_EmptySoap);
            if (endingScreen != null) endingScreen.SetActive(part == U8_GamePart.Ending);

            OnPartChanged?.Invoke(part);
            Debug.Log($"[U8_GameManager] Switched active screen to: {part}");
        }

        public void AwardStar()
        {
            starCount = Mathf.Clamp(starCount + 1, 0, 3);
            if (U8_AudioManager_Masters_Activity.Instance != null)
            {
                U8_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Star");
            }
            OnStarEarned?.Invoke(starCount);
            Debug.Log($"[U8_GameManager] ⭐ Star Earned! Total stars: {starCount}/3");
        }

        public void SetStarCount(int count)
        {
            starCount = Mathf.Clamp(count, 0, 3);
            OnStarEarned?.Invoke(starCount);
        }

        /// <summary>
        /// Resets Part 2 washroom state flags for the "TRY AGAIN" classroom retry flow.
        /// </summary>
        public void ResetUnitState()
        {
            isCubicleClosedFirst = false;
            isFlushed = false;
            isHandsWashed = false;
            isTapTurnedOff = false;
            isTowelInBin = false;
            isSinkWiped = false;
            Debug.Log("[U8_GameManager] Washroom state flags reset. Replaying Part 2...");
        }

        public bool AreAllStepsCompleted()
        {
            return isCubicleClosedFirst && isFlushed && isHandsWashed && isTapTurnedOff && isTowelInBin && isSinkWiped;
        }
    }
}
