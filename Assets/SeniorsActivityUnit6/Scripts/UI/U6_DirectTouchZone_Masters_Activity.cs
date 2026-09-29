using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    /// <summary>
    /// Invisible direct-touch raycast zone placed over characters or table areas.
    /// Allows children to tap or swipe the scene directly with zero visual overlay clutter.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class U6_DirectTouchZone_Masters_Activity : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private Image _touchHitArea;
        private RectTransform _rt;
        private Vector2 _pointerDownPos;
        private bool _isInteractable = false;
        private bool _wobbleActive = false;
        private Transform _parentCharacterTransform;

        public event Action OnInteracted;
        public bool IsInteractable => _isInteractable;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            EnsureComponents();
        }

        private void EnsureComponents()
        {
            if (_touchHitArea == null)
            {
                _touchHitArea = GetComponent<Image>();
                if (_touchHitArea == null) _touchHitArea = gameObject.AddComponent<Image>();
            }

            // Completely transparent raycast receiver - no visual graphics or overlays
            _touchHitArea.color = Color.clear;
            _touchHitArea.raycastTarget = true;

            // Clean up any legacy visual overlay children if present from previous iterations
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child.name == "PulseRing" || child.name == "FloatingHint")
                {
                    if (Application.isPlaying) Destroy(child.gameObject);
                    else DestroyImmediate(child.gameObject);
                }
            }
        }

        public void Setup(Transform characterRoot)
        {
            _parentCharacterTransform = characterRoot;
            EnsureComponents();
        }

        [Header("Transform Locking")]
        [Tooltip("When enabled, the position and size assigned by the user in the Inspector will NEVER be altered by code.")]
        [SerializeField] private bool lockTransformInPlace = true;

        public void ConfigureState(bool wobble = false, bool vibrate = false)
        {
            EnsureComponents();
            _wobbleActive = wobble;
            _isInteractable = true;
            gameObject.SetActive(true);
        }

        public void ConfigureZone(Vector2 localPosition, Vector2 size, string hint = "", bool wobble = false, bool vibrate = false)
        {
            EnsureComponents();
            if (_rt == null) _rt = GetComponent<RectTransform>();

            // Never change anchoredPosition or sizeDelta if locked
            if (!lockTransformInPlace && _rt != null)
            {
                _rt.anchoredPosition = localPosition;
                _rt.sizeDelta = size;
            }

            _wobbleActive = wobble;
            _isInteractable = true;
            gameObject.SetActive(true);
        }

        public void Dismiss()
        {
            _isInteractable = false;
            _wobbleActive = false;

            if (_parentCharacterTransform != null)
            {
                _parentCharacterTransform.localRotation = Quaternion.identity;
            }

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_isInteractable) return;

            // Procedural chair wobble effect on character visual during chair kneeling fidget
            if (_wobbleActive && _parentCharacterTransform != null)
            {
                float wobble = Mathf.Sin(Time.time * 16f) * 2.2f;
                _parentCharacterTransform.localRotation = Quaternion.Euler(0f, 0f, wobble);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isInteractable) return;

            float deltaY = eventData.position.y - _pointerDownPos.y;
            // Recognize a quick tap OR an intuitive upward swipe
            if (deltaY > 30f || Vector2.Distance(eventData.position, _pointerDownPos) < 40f)
            {
                TriggerInteraction();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable) return;
            TriggerInteraction();
        }

        private void TriggerInteraction()
        {
            if (!_isInteractable) return;
            _isInteractable = false;

            if (_parentCharacterTransform != null)
            {
                _parentCharacterTransform.localRotation = Quaternion.identity;
            }

            OnInteracted?.Invoke();
        }

        /// <summary>
        /// Juicy squishy squash-and-stretch bounce feedback when direct touch succeeds!
        /// </summary>
        public IEnumerator PlayBoingBounceRoutine(Transform target, Action onComplete = null)
        {
            if (target == null)
            {
                onComplete?.Invoke();
                yield break;
            }

            Vector3 baseScale = Vector3.one;
            Vector3 basePos = target.localPosition;

            // Phase 1: Squash down
            float duration1 = 0.08f;
            float elapsed = 0f;
            while (elapsed < duration1)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration1;
                target.localScale = new Vector3(Mathf.Lerp(1f, 1.18f, progress), Mathf.Lerp(1f, 0.82f, progress), 1f);
                yield return null;
            }

            // Phase 2: Stretch & Boing Jump Up!
            float duration2 = 0.16f;
            elapsed = 0f;
            while (elapsed < duration2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration2;
                target.localScale = new Vector3(Mathf.Lerp(1.18f, 0.88f, progress), Mathf.Lerp(0.82f, 1.25f, progress), 1f);
                target.localPosition = basePos + new Vector3(0f, Mathf.Sin(progress * Mathf.PI) * 36f, 0f);
                yield return null;
            }

            // Phase 3: Settle back smoothly
            float duration3 = 0.12f;
            elapsed = 0f;
            while (elapsed < duration3)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration3;
                target.localScale = Vector3.Lerp(new Vector3(0.88f, 1.25f, 1f), baseScale, progress);
                target.localPosition = Vector3.Lerp(target.localPosition, basePos, progress);
                yield return null;
            }

            target.localScale = baseScale;
            target.localPosition = basePos;
            onComplete?.Invoke();
        }
    }
}
