using UnityEngine;

namespace Googolplex.Unit6
{
    /// <summary>
    /// U6_SafeAreaConstraint: Automatically clamps the RectTransform to the device's
    /// hardware safe area (screen cutouts, notches, home bars, and camera holes),
    /// while optionally reserving clearance for parent game host navigation controls.
    ///
    /// Core Rule:
    /// - Every interactive object lives inside this safe area.
    /// - Nothing important is positioned purely by raw screen coordinates.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class U6_SafeAreaConstraint : MonoBehaviour
    {
        [Header("Safe Area Insets")]
        [Tooltip("Extra margin inside the safe area to prevent UI elements touching bezel edges (in reference pixels).")]
        [SerializeField] private Vector2 edgePadding = new Vector2(24f, 20f);

        [Header("Host Shell Navigation Clearance")]
        [Tooltip("Reserve clearance on Top-Left (x: 0..340, y: 900..1080) for host navigation buttons.")]
        [SerializeField] private bool reserveTopLeftClearance = false;

        [Tooltip("Reserve clearance on Bottom-Left (x: 0..340, y: 0..180) for host navigation buttons.")]
        [SerializeField] private bool reserveBottomLeftClearance = false;

        [Header("Editor Simulation")]
        [Tooltip("Simulate mobile notch/cutout safe area within the Unity Editor.")]
        [SerializeField] private bool simulateInEditor = true;

        [SerializeField] private Vector4 simulatedEditorNotch = new Vector4(0f, 0f, 0f, 0f); // L, R, T, B

        private RectTransform _rectTransform;
        private Rect _lastSafeArea = Rect.zero;
        private Vector2Int _lastScreenSize = Vector2Int.zero;
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void OnEnable()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            RefreshIfNeeded();
        }

        private void OnRectTransformDimensionsChange()
        {
            RefreshIfNeeded();
        }

        private void RefreshIfNeeded()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            Rect currentSafe = GetActiveSafeArea();
            Vector2Int currentScreen = new Vector2Int(Screen.width, Screen.height);
            ScreenOrientation currentOrientation = Screen.orientation;

            if (currentSafe != _lastSafeArea || currentScreen != _lastScreenSize || currentOrientation != _lastOrientation)
            {
                ApplySafeArea();
            }
        }

        public Rect GetActiveSafeArea()
        {
            Rect safe = Screen.safeArea;

#if UNITY_EDITOR
            if (simulateInEditor && simulatedEditorNotch != Vector4.zero)
            {
                float left = simulatedEditorNotch.x;
                float right = Screen.width - simulatedEditorNotch.y;
                float top = Screen.height - simulatedEditorNotch.z;
                float bottom = simulatedEditorNotch.w;
                safe = new Rect(left, bottom, right - left, top - bottom);
            }
#endif
            return safe;
        }

        public bool ReserveTopLeftClearance => reserveTopLeftClearance;
        public bool ReserveBottomLeftClearance => reserveBottomLeftClearance;

        private void OnValidate()
        {
            ApplySafeArea();
        }

        public void ApplySafeArea()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            Rect safeArea = GetActiveSafeArea();
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;

            if (Screen.width <= 0 || Screen.height <= 0) return;

            // Convert safe area rect into 0..1 normalized anchor bounds
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Clamp anchors
            anchorMin.x = Mathf.Clamp01(anchorMin.x);
            anchorMin.y = Mathf.Clamp01(anchorMin.y);
            anchorMax.x = Mathf.Clamp01(anchorMax.x);
            anchorMax.y = Mathf.Clamp01(anchorMax.y);

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;

            // Apply deliberate internal margin padding with optional host button clearance
            float leftPad = edgePadding.x;
            float bottomPad = reserveBottomLeftClearance ? Mathf.Max(edgePadding.y, 40f) : edgePadding.y;
            float rightPad = -edgePadding.x;
            float topPad = reserveTopLeftClearance ? Mathf.Min(-edgePadding.y, -40f) : -edgePadding.y;

            _rectTransform.offsetMin = new Vector2(leftPad, bottomPad);
            _rectTransform.offsetMax = new Vector2(rightPad, topPad);
        }
    }
}
