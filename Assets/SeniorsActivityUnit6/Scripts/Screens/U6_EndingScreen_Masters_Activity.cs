using System.Collections;
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

        [Header("Controls")]
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnEnable()
        {
            StartCoroutine(CelebrationRoutine());
        }

        private IEnumerator CelebrationRoutine()
        {
            if (reflectionPanel) reflectionPanel.SetActive(false);

            if (starIcons != null)
            {
                foreach (var star in starIcons)
                {
                    if (star) star.SetActive(false);
                }

                yield return new WaitForSeconds(0.5f);

                for (int i = 0; i < starIcons.Length; i++)
                {
                    if (starIcons[i]) starIcons[i].SetActive(true);
                    if (U6_AudioManager_Masters_Activity.Instance != null)
                        U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Star");
                    yield return new WaitForSeconds(0.6f);
                }
            }

            if (bannerText) bannerText.text = "THANK YOU, COME AGAIN!\n🙂 👋";

            if (confettiParticles != null)
                confettiParticles.Play();

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Confetti");
                U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Clap");
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_12");
            }

            yield return new WaitForSeconds(4.0f);

            if (reflectionPanel) reflectionPanel.SetActive(true);
            if (reflectionQuestionText)
                reflectionQuestionText.text = "What will you say to the waiter next time?";

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_13");
            }
        }

        private void OnRestartClicked()
        {
            U6_GameManager_Masters_Activity.Instance.StartPart1();
        }
    }
}
