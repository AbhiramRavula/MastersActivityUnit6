using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    [System.Serializable]
    public class U6_WaiterMomentData_Masters_Activity
    {
        public string situationTitle;
        public string situationPrompt;
        [TextArea] public string optionA_Text;
        [TextArea] public string optionA_Outcome;
        public string optionA_VO;
        public U6_WaiterFaceState optionA_Face;

        [TextArea] public string optionB_Text;
        [TextArea] public string optionB_Outcome;
        public string optionB_VO;
        public U6_WaiterFaceState optionB_Face;
    }

    public class U6_WaiterInteractionScreen_Masters_Activity : MonoBehaviour
    {
        [Header("Situation Prompt")]
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Waiter Visuals & State")]
        [SerializeField] private Image waiterAvatar;
        [SerializeField] private Sprite waiterSmileSprite;
        [SerializeField] private Sprite waiterNeutralSprite;
        [SerializeField] private Sprite waiterStiffSprite;
        [SerializeField] private TextMeshProUGUI waiterNameBadge;

        [Header("Speech Options")]
        [SerializeField] private GameObject choiceContainer;
        [SerializeField] private Button optionA_Button;
        [SerializeField] private TextMeshProUGUI optionA_Label;
        [SerializeField] private Button optionB_Button;
        [SerializeField] private TextMeshProUGUI optionB_Label;

        [Header("Outcome Feedback")]
        [SerializeField] private GameObject outcomePanel;
        [SerializeField] private TextMeshProUGUI outcomeText;

        [Header("Moments List")]
        [SerializeField] private List<U6_WaiterMomentData_Masters_Activity> moments = new List<U6_WaiterMomentData_Masters_Activity>();

        private int currentMomentIndex = 0;
        private bool isResolving = false;

        private void Awake()
        {
            if (optionA_Button != null)
                optionA_Button.onClick.AddListener(() => OnOptionSelected(true));

            if (optionB_Button != null)
                optionB_Button.onClick.AddListener(() => OnOptionSelected(false));
        }

        private void OnEnable()
        {
            InitializeMoments();
            currentMomentIndex = 0;
            if (waiterNameBadge) waiterNameBadge.text = "Ravi";

            if (U6_AudioManager_Masters_Activity.Instance != null)
            {
                U6_AudioManager_Masters_Activity.Instance.PlayVO("VO_U6_08");
            }

            DisplayCurrentMoment();
        }

        private void InitializeMoments()
        {
            if (moments.Count == 0)
            {
                moments.Add(new U6_WaiterMomentData_Masters_Activity
                {
                    situationTitle = "Water Served",
                    situationPrompt = "Ravi brings a fresh jug of water to the table.",
                    optionA_Text = "\"Thank you!\"",
                    optionA_Outcome = "Ravi smiles warmly and nods.",
                    optionA_VO = "VO_U6_ANU_4",
                    optionA_Face = U6_WaiterFaceState.WarmSmile,
                    optionB_Text = "(Say nothing)",
                    optionB_Outcome = "Ravi is polite, but neutral.",
                    optionB_VO = "",
                    optionB_Face = U6_WaiterFaceState.BlankNeutral
                });

                moments.Add(new U6_WaiterMomentData_Masters_Activity
                {
                    situationTitle = "Calling Ravi",
                    situationPrompt = "Anu needs something and Ravi is across the room.",
                    optionA_Text = "Raise a hand gently and make eye contact",
                    optionA_Outcome = "Ravi notices and comes over calmly.",
                    optionA_VO = "",
                    optionA_Face = U6_WaiterFaceState.WarmSmile,
                    optionB_Text = "Wave both arms and shout across the room",
                    optionB_Outcome = "Ravi hurries over while other tables look over.",
                    optionB_VO = "",
                    optionB_Face = U6_WaiterFaceState.BlankNeutral
                });

                moments.Add(new U6_WaiterMomentData_Masters_Activity
                {
                    situationTitle = "Wrong Dish",
                    situationPrompt = "Ravi brings the wrong dish by mistake!",
                    optionA_Text = "\"Sorry, I think I ordered dosa.\"",
                    optionA_Outcome = "Ravi apologises warmly: \"I will fix that right away!\"",
                    optionA_VO = "VO_U6_WAIT_3",
                    optionA_Face = U6_WaiterFaceState.WarmSmile,
                    optionB_Text = "\"This is WRONG!\" (loudly)",
                    optionB_Outcome = "Ravi apologises stiffly and fixes it quietly.",
                    optionB_VO = "VO_U6_ANU_6",
                    optionB_Face = U6_WaiterFaceState.StiffPolite
                });

                moments.Add(new U6_WaiterMomentData_Masters_Activity
                {
                    situationTitle = "Food Served",
                    situationPrompt = "Ravi sets the hot dosa down in front of Anu.",
                    optionA_Text = "\"Thank you, Ravi!\"",
                    optionA_Outcome = "Ravi nods happily. Enjoy your meal!",
                    optionA_VO = "VO_U6_ANU_4",
                    optionA_Face = U6_WaiterFaceState.WarmSmile,
                    optionB_Text = "(Grab fork and eat immediately without looking)",
                    optionB_Outcome = "Ravi steps away quietly.",
                    optionB_VO = "",
                    optionB_Face = U6_WaiterFaceState.BlankNeutral
                });

                moments.Add(new U6_WaiterMomentData_Masters_Activity
                {
                    situationTitle = "Dropped Fork",
                    situationPrompt = "Anu accidentally drops her fork on the floor.",
                    optionA_Text = "\"Excuse me, could I have another fork please?\"",
                    optionA_Outcome = "Ravi brings a clean fork right away with a smile.",
                    optionA_VO = "VO_U6_ANU_8",
                    optionA_Face = U6_WaiterFaceState.WarmSmile,
                    optionB_Text = "\"I dropped my fork!\" (shouted)",
                    optionB_Outcome = "Ravi brings one while the nearby diners look up.",
                    optionB_VO = "VO_U6_ANU_9",
                    optionB_Face = U6_WaiterFaceState.StiffPolite
                });
            }
        }

        private void DisplayCurrentMoment()
        {
            if (currentMomentIndex >= moments.Count)
            {
                StartCoroutine(CompletePart3Sequence());
                return;
            }

            U6_WaiterMomentData_Masters_Activity m = moments[currentMomentIndex];
            if (promptText) promptText.text = m.situationPrompt;
            if (optionA_Label) optionA_Label.text = m.optionA_Text;
            if (optionB_Label) optionB_Label.text = m.optionB_Text;

            SetWaiterFace(U6_WaiterFaceState.BlankNeutral);

            if (choiceContainer) choiceContainer.SetActive(true);
            if (outcomePanel) outcomePanel.SetActive(false);

            isResolving = false;
        }

        private void OnOptionSelected(bool choseA)
        {
            if (isResolving) return;
            isResolving = true;

            StartCoroutine(ResolveMomentSequence(choseA));
        }

        private IEnumerator ResolveMomentSequence(bool choseA)
        {
            U6_WaiterMomentData_Masters_Activity m = moments[currentMomentIndex];
            if (choiceContainer) choiceContainer.SetActive(false);
            if (outcomePanel) outcomePanel.SetActive(true);

            if (choseA)
            {
                if (outcomeText) outcomeText.text = m.optionA_Outcome;
                SetWaiterFace(m.optionA_Face);
                if (U6_AudioManager_Masters_Activity.Instance != null)
                {
                    U6_AudioManager_Masters_Activity.Instance.PlaySFX("SFX_Sparkle");
                    if (!string.IsNullOrEmpty(m.optionA_VO))
                        U6_AudioManager_Masters_Activity.Instance.PlayVO(m.optionA_VO);
                }
            }
            else
            {
                if (outcomeText) outcomeText.text = m.optionB_Outcome;
                SetWaiterFace(m.optionB_Face);
                if (U6_AudioManager_Masters_Activity.Instance != null && !string.IsNullOrEmpty(m.optionB_VO))
                {
                    U6_AudioManager_Masters_Activity.Instance.PlayVO(m.optionB_VO);
                }
            }

            yield return new WaitForSeconds(3.5f);

            currentMomentIndex++;
            DisplayCurrentMoment();
        }

        private void SetWaiterFace(U6_WaiterFaceState state)
        {
            if (waiterAvatar == null) return;

            switch (state)
            {
                case U6_WaiterFaceState.WarmSmile:
                    if (waiterSmileSprite != null) waiterAvatar.sprite = waiterSmileSprite;
                    break;
                case U6_WaiterFaceState.BlankNeutral:
                    if (waiterNeutralSprite != null) waiterAvatar.sprite = waiterNeutralSprite;
                    break;
                case U6_WaiterFaceState.StiffPolite:
                    if (waiterStiffSprite != null) waiterAvatar.sprite = waiterStiffSprite;
                    break;
            }
        }

        private IEnumerator CompletePart3Sequence()
        {
            if (promptText) promptText.text = "Great job showing kindness to Ravi!";
            if (outcomePanel) outcomePanel.SetActive(true);
            if (outcomeText) outcomeText.text = "Star 2 Earned!";

            U6_GameManager_Masters_Activity.Instance.AwardStar();

            yield return new WaitForSeconds(3.0f);

            U6_GameManager_Masters_Activity.Instance.StartPart4();
        }
    }
}
