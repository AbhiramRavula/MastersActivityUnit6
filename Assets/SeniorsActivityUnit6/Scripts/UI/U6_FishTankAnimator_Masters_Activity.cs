using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Googolplex.Unit6
{
    /// <summary>
    /// Animates the goldfish swimming inside the aquarium tank by cycling through
    /// the 16 sliced sprites from 'fish swimming sprites.png'.
    /// Supports direct tap interaction for playful bubble sounds and fish excitement.
    /// </summary>
    [DisallowMultipleComponent]
    public class U6_FishTankAnimator_Masters_Activity : MonoBehaviour, IPointerClickHandler
    {
        [Header("Frame Animation")]
        [Tooltip("The 32 frames of the fish swimming animation in sequential order.")]
        [SerializeField] private Sprite[] swimFrames;

        [Tooltip("Playback speed in frames per second. 6 to 8 is a smooth, natural swim for 32 frames.")]
        [Range(2f, 20f)]
        [SerializeField] private float framesPerSecond = 8f;

        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool loop = true;

        [Header("Interactive Tap")]
        [Tooltip("Tapping the aquarium makes the fish wiggle excitedly and plays a bubble sound.")]
        [SerializeField] private bool enableTapInteraction = true;
        [SerializeField] private string bubbleSfx = "SFX_Bubble";

        private Image targetImage;
        private SpriteRenderer targetSpriteRenderer;
        private Coroutine animCoroutine;
        private Coroutine tapWiggleCoroutine;
        private int currentFrameIndex = 0;
        private float currentFps;
        private Vector3 originalScale = Vector3.one;

        private void Awake()
        {
            targetImage = GetComponent<Image>();
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;
            currentFps = framesPerSecond;

            // Ensure RaycastTarget is enabled if tap interaction is desired
            if (targetImage != null && enableTapInteraction)
            {
                targetImage.raycastTarget = true;
            }
        }

        private void Update()
        {
            if (tapWiggleCoroutine == null)
            {
                currentFps = framesPerSecond;
            }
        }

        private void OnEnable()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            currentFps = framesPerSecond;
            animCoroutine = StartCoroutine(AnimateFrames());
        }

        public void Stop()
        {
            if (animCoroutine != null)
            {
                StopCoroutine(animCoroutine);
                animCoroutine = null;
            }
        }

        public void SetFrames(Sprite[] newFrames)
        {
            swimFrames = newFrames;
            if (swimFrames != null && swimFrames.Length > 0)
            {
                ApplyFrame(0);
            }
        }

        private IEnumerator AnimateFrames()
        {
            if (swimFrames == null || swimFrames.Length == 0) yield break;

            while (true)
            {
                ApplyFrame(currentFrameIndex);

                float delay = currentFps > 0f ? (1f / currentFps) : 0.125f;
                yield return new WaitForSeconds(delay);

                currentFrameIndex++;
                if (currentFrameIndex >= swimFrames.Length)
                {
                    if (loop)
                    {
                        currentFrameIndex = 0;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }

        private void ApplyFrame(int index)
        {
            if (swimFrames == null || index < 0 || index >= swimFrames.Length) return;
            Sprite frame = swimFrames[index];
            if (frame == null) return;

            if (targetImage != null)
            {
                targetImage.sprite = frame;
            }
            else if (targetSpriteRenderer != null)
            {
                targetSpriteRenderer.sprite = frame;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!enableTapInteraction) return;

            // 1. Play bubble SFX
            if (U6_AudioManager_Masters_Activity.Instance != null && !string.IsNullOrEmpty(bubbleSfx))
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX(bubbleSfx);
            }

            // 2. Play bounce & boost swim speed temporarily
            if (tapWiggleCoroutine != null) StopCoroutine(tapWiggleCoroutine);
            tapWiggleCoroutine = StartCoroutine(TapFeedbackRoutine());
        }

        private IEnumerator TapFeedbackRoutine()
        {
            // Boost FPS gently so the fish wiggles excitedly without racing
            currentFps = Mathf.Min(framesPerSecond * 2f, 6.5f);

            // Squash & stretch pulse
            float elapsed = 0f;
            float duration = 0.4f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Bouncy curve: elastic dampening
                float scaleOffset = Mathf.Sin(t * Mathf.PI * 3f) * (1f - t) * 0.12f;
                transform.localScale = originalScale + new Vector3(scaleOffset, -scaleOffset * 0.7f, 0f);
                yield return null;
            }

            transform.localScale = originalScale;

            // Wait a brief moment before returning to normal swim speed
            yield return new WaitForSeconds(0.8f);
            currentFps = framesPerSecond;
            tapWiggleCoroutine = null;
        }

#if UNITY_EDITOR
        [ContextMenu("Auto-Load Frames From Spritesheet")]
        public void AutoLoadFramesFromSpritesheet()
        {
            string path = "Assets/SeniorsActivityUnit6/Art/U6_MastersActivitySprites/fish swimming sprites.png";
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            List<Sprite> loaded = new List<Sprite>();
            foreach (var asset in allAssets)
            {
                if (asset is Sprite sp)
                {
                    loaded.Add(sp);
                }
            }

            if (loaded.Count > 0)
            {
                // Natural sort by trailing number (e.g. fish_swim_0, fish_swim_1 ... fish_swim_15)
                loaded.Sort((a, b) =>
                {
                    int numA = ExtractNumber(a.name);
                    int numB = ExtractNumber(b.name);
                    return numA.CompareTo(numB);
                });

                swimFrames = loaded.ToArray();
                EditorUtility.SetDirty(this);
                Debug.Log($"[U6_FishTankAnimator] Successfully loaded {swimFrames.Length} animation frames from {path}!");

                if (targetImage == null) targetImage = GetComponent<Image>();
                if (targetImage != null && swimFrames.Length > 0)
                {
                    targetImage.sprite = swimFrames[0];
                    EditorUtility.SetDirty(targetImage);
                }
            }
            else
            {
                Debug.LogWarning($"[U6_FishTankAnimator] No sprites found at {path}. Make sure the texture import settings are Multiple and sliced.");
            }
        }

        private int ExtractNumber(string name)
        {
            string digits = "";
            for (int i = name.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(name[i]))
                {
                    digits = name[i] + digits;
                }
                else if (digits.Length > 0)
                {
                    break;
                }
            }
            return int.TryParse(digits, out int val) ? val : 0;
        }

        private void Reset()
        {
            targetImage = GetComponent<Image>();
            targetSpriteRenderer = GetComponent<SpriteRenderer>();
            AutoLoadFramesFromSpritesheet();
        }

        private void OnValidate()
        {
            if (swimFrames == null || swimFrames.Length == 0)
            {
                AutoLoadFramesFromSpritesheet();
            }
        }
#endif
    }
}
