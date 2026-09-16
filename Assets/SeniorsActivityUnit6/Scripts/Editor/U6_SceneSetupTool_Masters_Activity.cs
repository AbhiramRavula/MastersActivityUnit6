#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    public class U6_SceneSetupTool_Masters_Activity : EditorWindow
    {
        [MenuItem("Unit 6/Generate Complete UI Hierarchy")]
        public static void GenerateHierarchy()
        {
            // 1. Ensure EventSystem exists
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
            }

            // 2. Ensure Canvas exists with 1920x1080 Scaler
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            GameObject canvasObj;
            if (canvas == null)
            {
                canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }
            else
            {
                canvasObj = canvas.gameObject;
            }

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // 3. Create or find GameManager
            GameObject gmObj = GameObject.Find("[GameManager]");
            if (gmObj == null)
            {
                gmObj = new GameObject("[GameManager]");
                Undo.RegisterCreatedObjectUndo(gmObj, "Create GameManager");
            }
            var gameMgr = GetOrAddComponent<U6_GameManager_Masters_Activity>(gmObj);
            var audioMgr = GetOrAddComponent<U6_AudioManager_Masters_Activity>(gmObj);
            AutoWireAudioLibrary(audioMgr, gmObj);

            // 4. Create Screens Container Panels under Canvas
            GameObject liveTablePanel = CreateOrGetPanel(canvasObj.transform, "LiveTableScreen", new Color(0.92f, 0.94f, 0.96f, 1f));
            GameObject menuPanel = CreateOrGetPanel(canvasObj.transform, "MenuScreen", new Color(0.98f, 0.95f, 0.90f, 1f));
            GameObject choicePanel = CreateOrGetPanel(canvasObj.transform, "ChoiceScreen", new Color(0.90f, 0.95f, 0.98f, 1f));
            GameObject sliderPanel = CreateOrGetPanel(canvasObj.transform, "SliderScreen", new Color(0.95f, 0.92f, 0.98f, 1f));
            GameObject endingPanel = CreateOrGetPanel(canvasObj.transform, "EndingScreen", new Color(0.12f, 0.14f, 0.2f, 1f));

            // Setup Screen 1: LiveTableScreen (Part 1 - Waiting)
            SetupLiveTableScreen(liveTablePanel);

            // Setup Screen 2: MenuScreen (Part 2 - Menu)
            SetupMenuScreen(menuPanel);

            // Setup Screen 3: ChoiceScreen (Part 3 - Waiter)
            SetupChoiceScreen(choicePanel);

            // Setup Screen 4: SliderScreen (Part 4 - Volume & Leaving)
            SetupSliderScreen(sliderPanel);

            // Setup Screen 5: EndingScreen
            SetupEndingScreen(endingPanel);

            // 5. Connect screens to U6_GameManager
            SerializedObject gmSO = new SerializedObject(gameMgr);
            gmSO.FindProperty("liveTableScreen").objectReferenceValue = liveTablePanel;
            gmSO.FindProperty("menuScreen").objectReferenceValue = menuPanel;
            gmSO.FindProperty("choiceScreen").objectReferenceValue = choicePanel;
            gmSO.FindProperty("sliderScreen").objectReferenceValue = sliderPanel;
            gmSO.FindProperty("endingScreen").objectReferenceValue = endingPanel;
            gmSO.ApplyModifiedProperties();

            EditorUtility.SetDirty(canvasObj);
            EditorUtility.SetDirty(gmObj);

            Debug.Log("<color=#4CAF50><b>[Unit 6] Complete UI Hierarchy successfully generated & wired!</b></color>");
        }

        private static GameObject CreateOrGetPanel(Transform parent, string name, Color bgColor)
        {
            Transform existing = parent.Find(name);
            GameObject panelObj;
            if (existing != null)
            {
                panelObj = existing.gameObject;
            }
            else
            {
                panelObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                panelObj.transform.SetParent(parent, false);
                Undo.RegisterCreatedObjectUndo(panelObj, "Create " + name);
            }

            RectTransform rt = GetOrAddComponent<RectTransform>(panelObj);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            GetOrAddComponent<CanvasRenderer>(panelObj);
            Image img = GetOrAddComponent<Image>(panelObj);
            img.color = bgColor;

            return panelObj;
        }

        private static void SetupLiveTableScreen(GameObject screenObj)
        {
            var comp = GetOrAddComponent<U6_LiveTableScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            // Title
            CreateTMPText(screenObj.transform, "TitleText", "PART 1: WAITING FOR FOOD", 36, new Vector2(0, 420), new Vector2(1200, 80), TextAlignmentOptions.Center, Color.black);

            // Prompt Text
            var promptText = CreateTMPText(screenObj.transform, "PromptText", "The food is not here yet. Watch Anu.", 30, new Vector2(0, 260), new Vector2(1400, 100), TextAlignmentOptions.Center, new Color(0.2f, 0.2f, 0.2f));

            // Other Tables Visual Placeholders (Simulated diners)
            GameObject normalDiners = CreateUIBox(screenObj.transform, "OtherTables_Normal", "🙂 Other Diners Eating Peacefully", 22, new Vector2(0, 100), new Vector2(1000, 100), new Color(0.8f, 0.9f, 0.8f));
            GameObject lookingDiners = CreateUIBox(screenObj.transform, "OtherTables_Looking", "👀 OTHER TABLES TURN AND LOOK!", 24, new Vector2(0, 100), new Vector2(1000, 100), new Color(1f, 0.7f, 0.7f));
            lookingDiners.SetActive(false);

            // Action Button
            GameObject btnObj = CreateUIButton(screenObj.transform, "ActionButton", "STAY SEATED", new Vector2(0, -220), new Vector2(400, 90), new Color(0.2f, 0.65f, 0.35f));
            var actionBtn = btnObj.GetComponent<Button>();
            var actionBtnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            // Feedback Panel
            GameObject feedbackPanel = CreateUIBox(screenObj.transform, "FeedbackPanel", "Nice job! Anu stays seated.", 26, new Vector2(0, -360), new Vector2(1200, 80), new Color(1f, 0.95f, 0.7f));
            var feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
            feedbackPanel.SetActive(false);

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("promptText").objectReferenceValue = promptText;
            so.FindProperty("actionButton").objectReferenceValue = actionBtn;
            so.FindProperty("actionButtonText").objectReferenceValue = actionBtnText;
            so.FindProperty("feedbackPanel").objectReferenceValue = feedbackPanel;
            so.FindProperty("feedbackText").objectReferenceValue = feedbackText;

            SerializedProperty normArr = so.FindProperty("otherTablesNormal");
            normArr.arraySize = 1;
            normArr.GetArrayElementAtIndex(0).objectReferenceValue = normalDiners;

            SerializedProperty lookArr = so.FindProperty("otherTablesLooking");
            lookArr.arraySize = 1;
            lookArr.GetArrayElementAtIndex(0).objectReferenceValue = lookingDiners;

            so.ApplyModifiedProperties();
        }

        private static void SetupMenuScreen(GameObject screenObj)
        {
            var comp = GetOrAddComponent<U6_MenuScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            // Header
            CreateTMPText(screenObj.transform, "HeaderText", "O U R   M E N U", 42, new Vector2(0, 430), new Vector2(800, 80), TextAlignmentOptions.Center, new Color(0.3f, 0.15f, 0.05f));

            // Grid Container for 6 cards
            GameObject gridObj = new GameObject("CardsContainer", typeof(RectTransform), typeof(GridLayoutGroup));
            gridObj.transform.SetParent(screenObj.transform, false);
            RectTransform gridRT = gridObj.GetComponent<RectTransform>();
            gridRT.sizeDelta = new Vector2(1100, 520);
            gridRT.anchoredPosition = new Vector2(0, 80);

            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(300, 220);
            glg.spacing = new Vector2(50, 40);
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = 3;
            glg.childAlignment = TextAnchor.MiddleCenter;

            // Generate DishCard UI Prefab / Template
            GameObject cardTemplate = CreateDishCardTemplate(screenObj.transform);
            cardTemplate.SetActive(false);

            // Call Waiter Button
            GameObject callWaiterBtn = CreateUIButton(screenObj.transform, "CallWaiterButton", "CALL WAITER  🛎️", new Vector2(0, -260), new Vector2(380, 80), new Color(0.9f, 0.45f, 0.2f));

            // Read First Prompt Banner
            GameObject readFirstPrompt = CreateUIBox(screenObj.transform, "ReadFirstPrompt", "📖 READ FIRST", 38, new Vector2(0, -370), new Vector2(600, 90), new Color(1f, 0.35f, 0.35f));
            var readFirstText = readFirstPrompt.GetComponentInChildren<TextMeshProUGUI>();
            readFirstPrompt.SetActive(false);

            // Awkward Waiter Stammer Overlay
            GameObject awkwardOverlay = CreateUIBox(screenObj.transform, "AwkwardOverlay", "Ravi waits politely...\nAnu: \"Ummm... ummm...\"", 30, new Vector2(0, -370), new Vector2(850, 110), new Color(1f, 0.9f, 0.7f));
            var stammerText = awkwardOverlay.GetComponentInChildren<TextMeshProUGUI>();
            awkwardOverlay.SetActive(false);

            // Order Choice Panel (Polite vs Abrupt)
            GameObject choicePanel = new GameObject("OrderChoicePanel", typeof(RectTransform));
            choicePanel.transform.SetParent(screenObj.transform, false);
            RectTransform cpRT = choicePanel.GetComponent<RectTransform>();
            cpRT.anchoredPosition = new Vector2(0, -380);
            cpRT.sizeDelta = new Vector2(1400, 120);

            GameObject btnPolite = CreateUIButton(choicePanel.transform, "PoliteButton", "\"Could I have the dosa, please?\"", new Vector2(-360, 0), new Vector2(640, 80), new Color(0.25f, 0.68f, 0.38f));
            GameObject btnImpolite = CreateUIButton(choicePanel.transform, "ImpoliteButton", "\"I want dosa.\"", new Vector2(360, 0), new Vector2(640, 80), new Color(0.85f, 0.55f, 0.25f));
            choicePanel.SetActive(false);

            // Waiter Feedback
            GameObject waiterFeedback = CreateUIBox(screenObj.transform, "WaiterFeedback", "Ravi: \"Certainly!\"", 28, new Vector2(0, -450), new Vector2(900, 70), new Color(0.85f, 0.95f, 1f));
            var waiterDialogText = waiterFeedback.GetComponentInChildren<TextMeshProUGUI>();
            waiterFeedback.SetActive(false);

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("cardsContainer").objectReferenceValue = gridObj.transform;
            so.FindProperty("dishCardPrefab").objectReferenceValue = cardTemplate;
            so.FindProperty("callWaiterButton").objectReferenceValue = callWaiterBtn.GetComponent<Button>();
            so.FindProperty("readFirstPrompt").objectReferenceValue = readFirstPrompt;
            so.FindProperty("readFirstText").objectReferenceValue = readFirstText;
            so.FindProperty("awkwardWaiterOverlay").objectReferenceValue = awkwardOverlay;
            so.FindProperty("anuStammerText").objectReferenceValue = stammerText;
            so.FindProperty("orderChoicePanel").objectReferenceValue = choicePanel;
            so.FindProperty("politeOptionButton").objectReferenceValue = btnPolite.GetComponent<Button>();
            so.FindProperty("politeOptionText").objectReferenceValue = btnPolite.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("impoliteOptionButton").objectReferenceValue = btnImpolite.GetComponent<Button>();
            so.FindProperty("impoliteOptionText").objectReferenceValue = btnImpolite.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("waiterFeedbackPopup").objectReferenceValue = waiterFeedback;
            so.FindProperty("waiterDialogText").objectReferenceValue = waiterDialogText;
            so.ApplyModifiedProperties();
        }

        private static void SetupChoiceScreen(GameObject screenObj)
        {
            var comp = GetOrAddComponent<U6_WaiterInteractionScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            CreateTMPText(screenObj.transform, "TitleText", "PART 3: THE WAITER (RAVI)", 36, new Vector2(0, 430), new Vector2(1000, 80), TextAlignmentOptions.Center, Color.black);

            // Waiter Avatar / Box
            GameObject waiterBox = CreateUIBox(screenObj.transform, "WaiterAvatarBox", "🧑‍🍳 [ Waiter Ravi ]", 24, new Vector2(0, 220), new Vector2(300, 220), new Color(0.85f, 0.9f, 1f));
            var waiterAvatar = waiterBox.GetComponent<Image>();
            var waiterBadge = CreateTMPText(waiterBox.transform, "Badge", "Ravi", 22, new Vector2(0, -80), new Vector2(160, 40), TextAlignmentOptions.Center, new Color(0.3f, 0.3f, 0.3f));

            // Prompt Text
            var promptText = CreateTMPText(screenObj.transform, "PromptText", "Ravi brings a fresh jug of water.", 30, new Vector2(0, 40), new Vector2(1400, 100), TextAlignmentOptions.Center, Color.black);

            // Choice Container with 2 buttons
            GameObject choiceContainer = new GameObject("ChoiceContainer", typeof(RectTransform));
            choiceContainer.transform.SetParent(screenObj.transform, false);
            RectTransform ccRT = choiceContainer.GetComponent<RectTransform>();
            ccRT.anchoredPosition = new Vector2(0, -180);
            ccRT.sizeDelta = new Vector2(1400, 160);

            GameObject btnA = CreateUIButton(choiceContainer.transform, "OptionA_Button", "\"Thank you!\"", new Vector2(-360, 0), new Vector2(620, 110), new Color(0.25f, 0.68f, 0.38f));
            GameObject btnB = CreateUIButton(choiceContainer.transform, "OptionB_Button", "(Say nothing)", new Vector2(360, 0), new Vector2(620, 110), new Color(0.85f, 0.55f, 0.25f));

            // Outcome Feedback
            GameObject outcomePanel = CreateUIBox(screenObj.transform, "OutcomePanel", "Ravi smiles warmly and nods.", 28, new Vector2(0, -360), new Vector2(1200, 90), new Color(1f, 0.95f, 0.75f));
            var outcomeText = outcomePanel.GetComponentInChildren<TextMeshProUGUI>();
            outcomePanel.SetActive(false);

            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("promptText").objectReferenceValue = promptText;
            so.FindProperty("waiterAvatar").objectReferenceValue = waiterAvatar;
            so.FindProperty("waiterNameBadge").objectReferenceValue = waiterBadge;
            so.FindProperty("choiceContainer").objectReferenceValue = choiceContainer;
            so.FindProperty("optionA_Button").objectReferenceValue = btnA.GetComponent<Button>();
            so.FindProperty("optionA_Label").objectReferenceValue = btnA.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("optionB_Button").objectReferenceValue = btnB.GetComponent<Button>();
            so.FindProperty("optionB_Label").objectReferenceValue = btnB.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("outcomePanel").objectReferenceValue = outcomePanel;
            so.FindProperty("outcomeText").objectReferenceValue = outcomeText;
            so.ApplyModifiedProperties();
        }

        private static void SetupSliderScreen(GameObject screenObj)
        {
            var comp = GetOrAddComponent<U6_SliderScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            CreateTMPText(screenObj.transform, "TitleText", "PART 4: VOICES & GOODBYES", 36, new Vector2(0, 430), new Vector2(1000, 80), TextAlignmentOptions.Center, Color.black);

            // Phase 1: Volume Container
            GameObject volPhase = new GameObject("VolumePhaseContainer", typeof(RectTransform));
            volPhase.transform.SetParent(screenObj.transform, false);
            RectTransform vpRT = volPhase.GetComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.offsetMin = Vector2.zero;
            vpRT.offsetMax = Vector2.zero;

            CreateTMPText(volPhase.transform, "VolPrompt", "How loud should Anu speak in the restaurant?", 30, new Vector2(0, 260), new Vector2(1200, 80), TextAlignmentOptions.Center, Color.black);

            // UI Slider
            GameObject sliderObj = new GameObject("VolumeSlider", typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(volPhase.transform, false);
            RectTransform sRT = sliderObj.GetComponent<RectTransform>();
            sRT.sizeDelta = new Vector2(700, 40);
            sRT.anchoredPosition = new Vector2(0, 100);

            Slider slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.5f;

            // Slider Background & Fill
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(sliderObj.transform, false);
            bg.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            bg.GetComponent<RectTransform>().anchorMax = Vector2.one;
            bg.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObj.transform, false);
            fillArea.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            fillArea.GetComponent<RectTransform>().anchorMax = Vector2.one;

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            fill.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.9f);
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Zone Label
            var zoneLabel = CreateTMPText(volPhase.transform, "ZoneLabel", "JUST RIGHT ★", 34, new Vector2(0, 0), new Vector2(500, 70), TextAlignmentOptions.Center, new Color(0.2f, 0.6f, 0.3f));

            // Say It Button
            GameObject sayBtn = CreateUIButton(volPhase.transform, "SayItButton", "SAY IT  🗣️", new Vector2(0, -140), new Vector2(340, 80), new Color(0.25f, 0.65f, 0.35f));

            // Phase 2: Leaving Container
            GameObject leavingPhase = new GameObject("LeavingPhaseContainer", typeof(RectTransform));
            leavingPhase.transform.SetParent(screenObj.transform, false);
            RectTransform lpRT = leavingPhase.GetComponent<RectTransform>();
            lpRT.anchorMin = Vector2.zero;
            lpRT.anchorMax = Vector2.one;
            lpRT.offsetMin = Vector2.zero;
            lpRT.offsetMax = Vector2.zero;

            CreateTMPText(leavingPhase.transform, "LeavePrompt", "The meal is finished. Another family is waiting by the door.", 30, new Vector2(0, 260), new Vector2(1300, 80), TextAlignmentOptions.Center, Color.black);

            GameObject waitFamily = CreateUIBox(leavingPhase.transform, "WaitingFamilyVisual", "👨‍👩‍👧 Family waiting at the door with a tired child", 26, new Vector2(0, 100), new Vector2(900, 100), new Color(1f, 0.9f, 0.8f));

            GameObject btnLeave = CreateUIButton(leavingPhase.transform, "LeavePolitelyButton", "Say thank you and leave  👋", new Vector2(-340, -120), new Vector2(580, 90), new Color(0.25f, 0.68f, 0.38f));
            GameObject btnStay = CreateUIButton(leavingPhase.transform, "StayAndPlayButton", "Stay & play with spoons", new Vector2(340, -120), new Vector2(580, 90), new Color(0.85f, 0.55f, 0.25f));
            leavingPhase.SetActive(false);

            // Feedback
            GameObject feedback = CreateUIBox(screenObj.transform, "FeedbackPanel", "Feedback text", 26, new Vector2(0, -360), new Vector2(1200, 80), new Color(1f, 0.95f, 0.7f));
            var feedbackText = feedback.GetComponentInChildren<TextMeshProUGUI>();
            feedback.SetActive(false);

            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("volumePhaseContainer").objectReferenceValue = volPhase;
            so.FindProperty("volumeSlider").objectReferenceValue = slider;
            so.FindProperty("currentZoneLabel").objectReferenceValue = zoneLabel;
            so.FindProperty("sayItButton").objectReferenceValue = sayBtn.GetComponent<Button>();
            so.FindProperty("leavingPhaseContainer").objectReferenceValue = leavingPhase;
            so.FindProperty("waitingFamilyVisual").objectReferenceValue = waitFamily;
            so.FindProperty("leavePolitelyButton").objectReferenceValue = btnLeave.GetComponent<Button>();
            so.FindProperty("stayAndPlayButton").objectReferenceValue = btnStay.GetComponent<Button>();
            so.FindProperty("feedbackPanel").objectReferenceValue = feedback;
            so.FindProperty("feedbackText").objectReferenceValue = feedbackText;
            so.ApplyModifiedProperties();
        }

        private static void SetupEndingScreen(GameObject screenObj)
        {
            var comp = GetOrAddComponent<U6_EndingScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            // Stars Row
            GameObject starsRow = new GameObject("StarsContainer", typeof(RectTransform));
            starsRow.transform.SetParent(screenObj.transform, false);
            RectTransform srRT = starsRow.GetComponent<RectTransform>();
            srRT.anchoredPosition = new Vector2(0, 240);
            srRT.sizeDelta = new Vector2(600, 120);

            GameObject s1 = CreateTMPText(starsRow.transform, "Star1", "⭐", 80, new Vector2(-180, 0), new Vector2(120, 120), TextAlignmentOptions.Center, Color.yellow).gameObject;
            GameObject s2 = CreateTMPText(starsRow.transform, "Star2", "⭐", 80, new Vector2(0, 0), new Vector2(120, 120), TextAlignmentOptions.Center, Color.yellow).gameObject;
            GameObject s3 = CreateTMPText(starsRow.transform, "Star3", "⭐", 80, new Vector2(180, 0), new Vector2(120, 120), TextAlignmentOptions.Center, Color.yellow).gameObject;

            // Banner
            var bannerText = CreateTMPText(screenObj.transform, "BannerText", "THANK YOU, COME AGAIN!\n🙂 👋", 48, new Vector2(0, 40), new Vector2(1200, 140), TextAlignmentOptions.Center, Color.white);

            // Reflection Panel
            GameObject refPanel = CreateUIBox(screenObj.transform, "ReflectionPanel", "Teacher Reflection:\nWhat will you say to the waiter next time?", 32, new Vector2(0, -180), new Vector2(1200, 120), new Color(0.2f, 0.25f, 0.35f));
            var refText = refPanel.GetComponentInChildren<TextMeshProUGUI>();
            refText.color = Color.white;

            // Restart Button
            GameObject restartBtn = CreateUIButton(screenObj.transform, "RestartButton", "Play Again  🔄", new Vector2(0, -360), new Vector2(300, 70), new Color(0.3f, 0.6f, 0.9f));

            SerializedObject so = new SerializedObject(comp);
            SerializedProperty starsProp = so.FindProperty("starIcons");
            starsProp.arraySize = 3;
            starsProp.GetArrayElementAtIndex(0).objectReferenceValue = s1;
            starsProp.GetArrayElementAtIndex(1).objectReferenceValue = s2;
            starsProp.GetArrayElementAtIndex(2).objectReferenceValue = s3;

            so.FindProperty("bannerText").objectReferenceValue = bannerText;
            so.FindProperty("reflectionPanel").objectReferenceValue = refPanel;
            so.FindProperty("reflectionQuestionText").objectReferenceValue = refText;
            so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
            so.ApplyModifiedProperties();
        }

        private static GameObject CreateDishCardTemplate(Transform parent)
        {
            GameObject cardObj = new GameObject("DishCard_Template", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(U6_DishCardUI_Masters_Activity));
            cardObj.transform.SetParent(parent, false);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 220);
            cardObj.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.95f);

            // Icon Placeholder
            GameObject iconObj = new GameObject("DishIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(cardObj.transform, false);
            RectTransform iconRT = iconObj.GetComponent<RectTransform>();
            iconRT.sizeDelta = new Vector2(120, 100);
            iconRT.anchoredPosition = new Vector2(0, 35);
            iconObj.GetComponent<Image>().color = new Color(0.9f, 0.85f, 0.8f);

            // Name
            var nameText = CreateTMPText(cardObj.transform, "DishName", "Dosa", 24, new Vector2(0, -35), new Vector2(260, 40), TextAlignmentOptions.Center, Color.black);

            // Price
            var priceText = CreateTMPText(cardObj.transform, "PriceText", "₹ 60", 22, new Vector2(0, -75), new Vector2(260, 35), TextAlignmentOptions.Center, new Color(0.7f, 0.4f, 0.1f));

            // Selection Highlight
            GameObject highlight = new GameObject("SelectionHighlight", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            highlight.transform.SetParent(cardObj.transform, false);
            RectTransform hlRT = highlight.GetComponent<RectTransform>();
            hlRT.anchorMin = Vector2.zero;
            hlRT.anchorMax = Vector2.one;
            hlRT.offsetMin = Vector2.zero;
            hlRT.offsetMax = Vector2.zero;
            highlight.GetComponent<Image>().color = new Color(0.2f, 0.8f, 1f, 0.35f);
            highlight.SetActive(false);

            var cardUI = cardObj.GetComponent<U6_DishCardUI_Masters_Activity>();
            SerializedObject so = new SerializedObject(cardUI);
            so.FindProperty("dishIcon").objectReferenceValue = iconObj.GetComponent<Image>();
            so.FindProperty("dishNameText").objectReferenceValue = nameText;
            so.FindProperty("priceText").objectReferenceValue = priceText;
            so.FindProperty("selectionHighlight").objectReferenceValue = highlight;
            so.FindProperty("cardButton").objectReferenceValue = cardObj.GetComponent<Button>();
            so.ApplyModifiedProperties();

            return cardObj;
        }

        private static TextMeshProUGUI CreateTMPText(Transform parent, string name, string content, float fontSize, Vector2 anchoredPos, Vector2 sizeDelta, TextAlignmentOptions alignment, Color color)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = color;

            return tmp;
        }

        private static GameObject CreateUIBox(Transform parent, string name, string textContent, float fontSize, Vector2 anchoredPos, Vector2 sizeDelta, Color bgColor)
        {
            GameObject box = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            box.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(box);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(box);
            img.color = bgColor;

            CreateTMPText(box.transform, "Label", textContent, fontSize, Vector2.zero, sizeDelta, TextAlignmentOptions.Center, Color.black);
            return box;
        }

        private static GameObject CreateUIButton(Transform parent, string name, string labelText, Vector2 anchoredPos, Vector2 sizeDelta, Color btnColor)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(btnObj);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(btnObj);
            img.color = btnColor;

            CreateTMPText(btnObj.transform, "Label", labelText, 24, Vector2.zero, sizeDelta, TextAlignmentOptions.Center, Color.white);
            return btnObj;
        }

        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            return comp;
        }

        private static void AutoWireAudioLibrary(U6_AudioManager_Masters_Activity audioMgr, GameObject gmObj)
        {
            // Ensure 4 AudioSources exist on GameManager
            AudioSource[] sources = gmObj.GetComponents<AudioSource>();
            while (sources.Length < 4)
            {
                gmObj.AddComponent<AudioSource>();
                sources = gmObj.GetComponents<AudioSource>();
            }

            SerializedObject so = new SerializedObject(audioMgr);
            so.FindProperty("bgmSource").objectReferenceValue = sources[0];
            so.FindProperty("ambSource").objectReferenceValue = sources[1];
            so.FindProperty("sfxSource").objectReferenceValue = sources[2];
            so.FindProperty("voSource").objectReferenceValue = sources[3];

            // Search for audio clips in Assets/Audio/U6_MastersActivity_audios
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio/U6_MastersActivity_audios" });
            var soundListProp = so.FindProperty("sounds");
            soundListProp.ClearArray();

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null) continue;

                string soundId = ResolveSoundId(clip.name);

                soundListProp.InsertArrayElementAtIndex(soundListProp.arraySize);
                SerializedProperty elem = soundListProp.GetArrayElementAtIndex(soundListProp.arraySize - 1);
                elem.FindPropertyRelative("id").stringValue = soundId;
                elem.FindPropertyRelative("clip").objectReferenceValue = clip;
                elem.FindPropertyRelative("volume").floatValue = 1f;
            }

            so.ApplyModifiedProperties();
        }

        private static string ResolveSoundId(string fileName)
        {
            string lower = fileName.ToLower();
            if (lower.Contains("today anus family")) return "VO_U6_01";
            if (lower.Contains("food is not here yet")) return "VO_U6_02";
            if (lower.Contains("quick tap the button")) return "VO_U6_03";
            if (lower.Contains("everybody is happy")) return "VO_U6_04";
            if (lower.Contains("now choose what will anu eat")) return "VO_U6_05";
            if (lower.Contains("read first")) return "VO_U6_06";
            if (lower.Contains("how should anu ask")) return "VO_U6_07";
            if (lower.Contains("this is ravi")) return "VO_U6_08";
            if (lower.Contains("wrong dish")) return "VO_U6_09";
            if (lower.Contains("how loud should anu talk")) return "VO_U6_10";
            if (lower.Contains("restaurant is full now")) return "VO_U6_11";
            if (lower.Contains("three stars thank you")) return "VO_U6_12";
            if (lower.Contains("what will you say to the waiter")) return "VO_U6_13";

            if (lower.Contains("could i have the dosa")) return "VO_U6_ANU_1";
            if (lower.Contains("i want dosa")) return "VO_U6_ANU_2";
            if (lower.Contains("ummm ummm")) return "VO_U6_ANU_3";
            if (lower.Contains("thank you") && !lower.Contains("do come again")) return "VO_U6_ANU_4";
            if (lower.Contains("sorry i think i ordered dosa")) return "VO_U6_ANU_5";
            if (lower.Contains("this is wrong")) return "VO_U6_ANU_6";
            if (lower.Contains("i am so hungry")) return "VO_U6_ANU_7";
            if (lower.Contains("excuse me could i have another fork")) return "VO_U6_ANU_8";
            if (lower.Contains("i dropped my fork")) return "VO_U6_ANU_9";
            if (lower.Contains("daddy guess what")) return "VO_U6_ANU_10";

            if (lower.Contains("certainly")) return "VO_U6_WAIT_1";
            if (lower.Contains("of course one moment")) return "VO_U6_WAIT_2";
            if (lower.Contains("i am so sorry i will fix")) return "VO_U6_WAIT_3";
            if (lower.Contains("thank you do come again")) return "VO_U6_WAIT_4";

            if (lower.Contains("sorry i cannot hear you")) return "VO_U6_DAD_1";
            if (lower.Contains("anu quiet")) return "VO_U6_MUM_1";

            if (lower.Contains("busy but pleasant restaurant")) return "AMB_Restaurant";
            if (lower.Contains("same restaurant going suddenly quieter")) return "AMB_RestaurantHush";
            if (lower.Contains("metal spoon tapping")) return "SFX_GlassTing";
            if (lower.Contains("wooden chair tipping")) return "SFX_ChairWobble";
            if (lower.Contains("metal fork falling")) return "SFX_ForkDrop";
            if (lower.Contains("large laminated menu")) return "SFX_MenuOpen";
            if (lower.Contains("quick pencil scribble")) return "SFX_PadWrite";
            if (lower.Contains("hot plate being set down")) return "SFX_PlateDown";
            if (lower.Contains("baby starting to cry")) return "SFX_BabyCry";
            if (lower.Contains("small shop door chime")) return "SFX_DoorBell";
            if (lower.Contains("big magical sparkle")) return "SFX_Sparkle";
            if (lower.Contains("bright star chime")) return "SFX_Star";
            if (lower.Contains("tiny happy chirp")) return "SFX_Chirp";
            if (lower.Contains("young children clapping")) return "SFX_Clap";
            if (lower.Contains("party popper burst")) return "SFX_Confetti";
            if (lower.Contains("small soft bubble pop")) return "SFX_SliderZone";
            if (lower.Contains("gentle cheerful ukulele")) return "MUS_Loop";
            if (lower.Contains("short happy celebration tune")) return "MUS_Win";

            return fileName;
        }

        private static void ClearChildren(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(t.GetChild(i).gameObject);
            }
        }
    }
}
#endif
