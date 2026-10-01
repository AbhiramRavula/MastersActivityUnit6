using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    public class U6_EndingScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Celebration Elements")]
        [SerializeField] private GameObject[] starIcons;
        [SerializeField] private TextMeshProUGUI bannerText;
        [SerializeField] private ParticleSystem confettiParticles;

        [Header("Teacher Closing Reflection")]
        [SerializeField] private GameObject reflectionPanel;
        [SerializeField] private TextMeshProUGUI reflectionQuestionText;

#pragma warning disable 0414
        [Header("Audios & SFX Used On This Screen")]
        [SerializeField] private string sfxStarChime = "SFX_Star (Star appearance chime)";
        [SerializeField] private string sfxConfetti = "SFX_Confetti (Party popper burst)";
        [SerializeField] private string sfxClap = "SFX_Clap (Children clapping & cheering)";
        [SerializeField] private string voCelebration = "VO_U6_12 (Three stars! Thank you, come again!)";
        [SerializeField] private AudioClip voCelebrationClip;
        [SerializeField] private string voCelebration2Stars = "VO_U6_12_2STARS (Two stars! Thank you, come again!)";
        [SerializeField] private AudioClip voCelebration2StarsClip;
        [SerializeField] private string voTeacherReflection = "VO_U6_13 (What will you say to the waiter next time?)";
        [SerializeField] private AudioClip voTeacherReflectionClip;
#pragma warning restore 0414

        [Header("Controls")]
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            AutoFindUIReferences();
#if UNITY_EDITOR
            EnsureCelebrationClips();
#endif

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnEnable()
        {
            AutoFindUIReferences();
#if UNITY_EDITOR
            EnsureCelebrationClips();
#endif
            int currentStars = (U6_GameManager_Masters_Activity.Instance != null && U6_GameManager_Masters_Activity.Instance.StarCount > 0)
                               ? U6_GameManager_Masters_Activity.Instance.StarCount
                               : ((starIcons != null && starIcons.Length > 0) ? starIcons.Length : 3);

            UpdateBannerText(currentStars);
            StartCoroutine(CelebrationRoutine());
        }

#if UNITY_EDITOR
        private void EnsureCelebrationClips()
        {
            if (voCelebrationClip == null)
                voCelebrationClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios/VO_U6_12.mp3");
            if (voCelebration2StarsClip == null)
                voCelebration2StarsClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios/VO_U6_12_2STARS.mp3");
            if (voTeacherReflectionClip == null)
                voTeacherReflectionClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios/VO_U6_13.mp3");
        }
#endif

        private void UpdateBannerText(int stars)
        {
            if (bannerText == null) return;
            bannerText.enableAutoSizing = true;
            bannerText.fontSizeMin = 22;
            bannerText.fontSizeMax = 44;
            bannerText.alignment = TextAlignmentOptions.Center;

            if (stars >= 3)
            {
                bannerText.text = "3 STARS EARNED!\nTHANK YOU, DO COME AGAIN!";
            }
            else if (stars == 2)
            {
                bannerText.text = "2 STARS EARNED!\nTHANK YOU, DO COME AGAIN!";
            }
            else if (stars == 1)
            {
                bannerText.text = "1 STAR EARNED!\nTHANK YOU, DO COME AGAIN!";
            }
            else
            {
                bannerText.text = "THANK YOU, DO COME AGAIN!";
            }
        }

        private void AutoFindUIReferences()
        {
            if (starIcons == null || starIcons.Length == 0)
            {
                Transform container = transform.Find("StarsContainer") 
                                   ?? transform.Find("SafeArea/StarsContainer") 
                                   ?? transform.Find("StarsRow") 
                                   ?? transform.Find("Stars");
                if (container != null)
                {
                    List<GameObject> stars = new List<GameObject>();
                    foreach (Transform c in container)
                    {
                        if (c.name.IndexOf("star", System.StringComparison.OrdinalIgnoreCase) >= 0)
                            stars.Add(c.gameObject);
                    }
                    if (stars.Count > 0) starIcons = stars.ToArray();
                }
                else
                {
                    List<GameObject> stars = new List<GameObject>();
                    Transform[] allChildren = GetComponentsInChildren<Transform>(true);
                    foreach (Transform c in allChildren)
                    {
                        if (c.name.IndexOf("star", System.StringComparison.OrdinalIgnoreCase) >= 0 && c != transform)
                            stars.Add(c.gameObject);
                    }
                    if (stars.Count > 0) starIcons = stars.ToArray();
                }
            }

            if (bannerText == null)
            {
                Transform b = transform.Find("BannerCard") 
                           ?? transform.Find("SafeArea/BannerCard") 
                           ?? transform.Find("TitleBanner")
                           ?? transform.Find("SafeArea/TitleBanner");
                if (b != null) bannerText = b.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (reflectionPanel == null)
            {
                Transform r = transform.Find("ReflectionPanel") ?? transform.Find("SafeArea/ReflectionPanel");
                if (r != null) reflectionPanel = r.gameObject;
            }

            if (reflectionQuestionText == null && reflectionPanel != null)
            {
                reflectionQuestionText = reflectionPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (restartButton == null)
            {
                restartButton = GetComponentInChildren<Button>(true);
            }
        }

        private IEnumerator CelebrationRoutine()
        {
            if (reflectionPanel) reflectionPanel.SetActive(false);

            int totalStars = (starIcons != null && starIcons.Length > 0) ? starIcons.Length : 3;
            int earnedStars = (U6_GameManager_Masters_Activity.Instance != null && U6_GameManager_Masters_Activity.Instance.StarCount > 0) 
                              ? U6_GameManager_Masters_Activity.Instance.StarCount 
                              : totalStars;

            earnedStars = Mathf.Clamp(earnedStars, 0, totalStars);

            // Hide all stars initially
            if (starIcons != null)
            {
                foreach (var star in starIcons)
                {
                    if (star) star.SetActive(false);
                }
            }

            yield return new WaitForSeconds(0.4f);

            // Sequentially award and pop-animate each earned star
            if (starIcons != null)
            {
                for (int i = 0; i < earnedStars && i < starIcons.Length; i++)
                {
                    if (starIcons[i] != null)
                    {
                        starIcons[i].SetActive(true);
                        StartCoroutine(PopStarAnimation(starIcons[i].transform));

                        if (U6_AudioManager_Masters_Activity.Instance != null)
                            U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Star");
                    }
                    yield return new WaitForSeconds(0.55f);
                }
            }

            UpdateBannerText(earnedStars);
            if (bannerText != null)
            {
                StartCoroutine(PopStarAnimation(bannerText.transform));
            }

            if (confettiParticles != null)
                confettiParticles.Play();

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Confetti");
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Clap");

                if (earnedStars >= 3)
                {
                    if (voCelebrationClip != null)
                        U6_AudioManager_Masters_Activity.Instance.PlayVO(voCelebrationClip);
                    else
                        U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_12");
                }
                else if (earnedStars == 2)
                {
                    if (voCelebration2StarsClip != null)
                        U6_AudioManager_Masters_Activity.Instance.PlayVO(voCelebration2StarsClip);
                    else
                        U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_12_2STARS");
                }
                else
                {
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_WAIT_4");
                }
            }

            yield return new WaitForSeconds(3.8f);

            if (reflectionPanel)
            {
                reflectionPanel.SetActive(true);
                StartCoroutine(PopStarAnimation(reflectionPanel.transform));
            }

            if (reflectionQuestionText)
                reflectionQuestionText.text = "Teacher Reflection:\nWhat will you say to the waiter next time?";

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                if (voTeacherReflectionClip != null)
                    U6_AudioManager_Masters_Activity.Instance.PlayVO(voTeacherReflectionClip);
                else
                    U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_13");
            }
        }

        private IEnumerator PopStarAnimation(Transform target)
        {
            if (target == null) yield break;
            Vector3 originalScale = Vector3.one;
            target.localScale = originalScale * 0.1f;

            float elapsed = 0f;
            float duration = 0.35f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f);
                target.localScale = Vector3.Lerp(originalScale * 0.1f, originalScale * 1.15f, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        private void OnRestartClicked()
        {
            if (U6_GameManager_Masters_Activity.Instance != null)
            {
                U6_GameManager_Masters_Activity.Instance.StartPart1();
            }
        }
    }
}
