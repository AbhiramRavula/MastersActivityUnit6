#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    /// <summary>
    /// U6_SceneSetupTool_Masters_Activity
    /// Complete Scene Generator & Non-Destructive Layout Engine for Unit 6 (Senior Etiquette).
    ///
    /// Core Architectural Rules Enforced:
    /// 1. Every visual object has a deliberate pivot (grounded feet, seated base, top plaque, etc.).
    /// 2. Every UI object has deliberate anchors (top-docked, bottom-docked, relative margins).
    /// 3. Every interactive object has a safe-area constraint (contained in SafeArea, host button clearance).
    /// 4. Nothing important is positioned purely by raw screen coordinates.
    /// </summary>
    public class U6_SceneSetupTool_Masters_Activity : EditorWindow
    {
        private const string SPRITES_PATH = "Assets/SeniorsActivityUnit6/Art/U6_MastersActivitySprites";
        private const string AUDIOS_PATH = "Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios";
        private const string SFX_PATH = "Assets/SeniorsActivityUnit6/SFX";

        // =========================================================================
        // Menu Items
        // =========================================================================

        [MenuItem("Googolplex/Unit 6/Generate Complete Scene Hierarchy", false, 1)]
        [MenuItem("Unit 6/Generate and Assign All Assets & Hierarchy", false, 1)]
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

            CanvasScaler scaler = GetOrAddComponent<CanvasScaler>(canvasObj);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // Load Sprite Dictionary
            Dictionary<string, Sprite> spriteDict = LoadAllSprites();

            // Root Background Image (Bleeds full screen 0,0 to 1,1)
            SetupRootBackground(canvasObj.transform, spriteDict);

            // 3. Create or find GameManager & AudioManager
            GameObject gmObj = GameObject.Find("[GameManager]");
            if (gmObj == null)
            {
                gmObj = new GameObject("[GameManager]");
                Undo.RegisterCreatedObjectUndo(gmObj, "Create GameManager");
            }
            var gameMgr = GetOrAddComponent<U6_GameManager_Masters_Activity>(gmObj);
            var audioMgr = GetOrAddComponent<U6_AudioManager_Masters_Activity>(gmObj);
            AutoWireAudioLibrary(audioMgr, gmObj);

            // 4. Create Screen Container Panels under Canvas (Translucent so restaurant background shows)
            GameObject liveTablePanel = CreateOrGetPanel(canvasObj.transform, "LiveTableScreen", new Color(0f, 0f, 0f, 0f));
            GameObject menuPanel = CreateOrGetPanel(canvasObj.transform, "MenuScreen", new Color(0.1f, 0.08f, 0.06f, 0.45f));
            GameObject choicePanel = CreateOrGetPanel(canvasObj.transform, "ChoiceScreen", new Color(0.06f, 0.09f, 0.14f, 0.45f));
            GameObject sliderPanel = CreateOrGetPanel(canvasObj.transform, "SliderScreen", new Color(0.08f, 0.06f, 0.14f, 0.45f));
            GameObject endingPanel = CreateOrGetPanel(canvasObj.transform, "EndingScreen", new Color(0.06f, 0.08f, 0.14f, 0.55f));

            // Setup Screen 1: LiveTableScreen (Waiting)
            SetupLiveTableScreen(liveTablePanel, spriteDict);

            // Setup Screen 2: MenuScreen (Menu & Ordering)
            SetupMenuScreen(menuPanel, spriteDict);

            // Setup Screen 3: ChoiceScreen (Waiter Interaction)
            SetupChoiceScreen(choicePanel, spriteDict);

            // Setup Screen 4: SliderScreen (Voice Volume & Leaving)
            SetupSliderScreen(sliderPanel, spriteDict);

            // Setup Screen 5: EndingScreen (Celebration)
            SetupEndingScreen(endingPanel, spriteDict);

            // 5. Connect screens to U6_GameManager
            SerializedObject gmSO = new SerializedObject(gameMgr);
            gmSO.FindProperty("liveTableScreen").objectReferenceValue = liveTablePanel;
            gmSO.FindProperty("menuScreen").objectReferenceValue = menuPanel;
            gmSO.FindProperty("choiceScreen").objectReferenceValue = choicePanel;
            gmSO.FindProperty("sliderScreen").objectReferenceValue = sliderPanel;
            gmSO.FindProperty("endingScreen").objectReferenceValue = endingPanel;
            gmSO.ApplyModifiedProperties();

            // Initial visibility state: Show Part 1, hide others
            liveTablePanel.SetActive(true);
            menuPanel.SetActive(false);
            choicePanel.SetActive(false);
            sliderPanel.SetActive(false);
            endingPanel.SetActive(false);

            EditorUtility.SetDirty(canvasObj);
            EditorUtility.SetDirty(gmObj);

            Debug.Log("<color=#4CAF50><b>[Unit 6] Complete UI Hierarchy successfully generated with Deliberate Pivots, Deliberate Anchors, and Safe-Area Constraints!</b></color>");
        }

        [MenuItem("Googolplex/Unit 6/Non-Destructive/Update Only Part 3 (ChoiceScreen & Waiter Panel)", false, 20)]
        [MenuItem("Unit 6/Update Only Part 3 (ChoiceScreen & Waiter Panel)", false, 20)]
        public static void UpdateOnlyPart3()
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[U6_SceneSetupTool] No Canvas found in scene! Please ensure your Canvas is active.");
                return;
            }

            Dictionary<string, Sprite> spriteDict = LoadAllSprites();
            GameObject choicePanel = CreateOrGetPanel(canvas.transform, "ChoiceScreen", new Color(0.06f, 0.09f, 0.14f, 0.45f));
            SetupChoiceScreen(choicePanel, spriteDict);

            GameObject gmObj = GameObject.Find("[GameManager]");
            if (gmObj != null)
            {
                var gameMgr = gmObj.GetComponent<U6_GameManager_Masters_Activity>();
                if (gameMgr != null)
                {
                    SerializedObject gmSO = new SerializedObject(gameMgr);
                    var choiceProp = gmSO.FindProperty("choiceScreen");
                    if (choiceProp != null)
                    {
                        choiceProp.objectReferenceValue = choicePanel;
                        gmSO.ApplyModifiedProperties();
                    }
                }
            }

            Selection.activeGameObject = choicePanel;
            choicePanel.SetActive(true);
            Debug.Log("<color=green>[U6_SceneSetupTool] Successfully updated Part 3 (ChoiceScreen & Waiter Panel) with deliberate pivots, anchors, and safe area!</color>");
        }

        [MenuItem("Googolplex/Unit 6/Non-Destructive/Update Only EndingScreen (Ending Panel & Stars)", false, 21)]
        [MenuItem("Unit 6/Update Only EndingScreen (Ending Panel & Stars)", false, 21)]
        public static void UpdateOnlyEndingScreen()
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[U6_SceneSetupTool] No Canvas found in scene! Please ensure your Canvas is active.");
                return;
            }

            Dictionary<string, Sprite> spriteDict = LoadAllSprites();
            GameObject endingPanel = CreateOrGetPanel(canvas.transform, "EndingScreen", new Color(0.06f, 0.08f, 0.14f, 0.55f));
            SetupEndingScreen(endingPanel, spriteDict);

            GameObject gmObj = GameObject.Find("[GameManager]");
            if (gmObj != null)
            {
                var gameMgr = gmObj.GetComponent<U6_GameManager_Masters_Activity>();
                if (gameMgr != null)
                {
                    SerializedObject gmSO = new SerializedObject(gameMgr);
                    var endingProp = gmSO.FindProperty("endingScreen");
                    if (endingProp != null)
                    {
                        endingProp.objectReferenceValue = endingPanel;
                        gmSO.ApplyModifiedProperties();
                    }
                }
            }

            Selection.activeGameObject = endingPanel;
            endingPanel.SetActive(true);
            Debug.Log("<color=green>[U6_SceneSetupTool] Successfully updated EndingScreen with deliberate pivots, anchors, and safe area!</color>");
        }

        // =========================================================================
        // Safe Area & Panel Helpers
        // =========================================================================

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
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = GetOrAddComponent<Image>(panelObj);
            img.color = bgColor;
            img.raycastTarget = false;

            return panelObj;
        }

        private static GameObject CreateSafeAreaContainer(Transform screenParent)
        {
            Transform existing = screenParent.Find("SafeArea");
            GameObject safeObj;
            if (existing != null)
            {
                safeObj = existing.gameObject;
            }
            else
            {
                safeObj = new GameObject("SafeArea", typeof(RectTransform), typeof(U6_SafeAreaConstraint));
                safeObj.transform.SetParent(screenParent, false);
            }

            RectTransform rt = GetOrAddComponent<RectTransform>(safeObj);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(24f, 20f);
            rt.offsetMax = new Vector2(-24f, -20f);

            var constraint = GetOrAddComponent<U6_SafeAreaConstraint>(safeObj);
            constraint.ApplySafeArea();

            return safeObj;
        }

        private static void SetupRootBackground(Transform parent, Dictionary<string, Sprite> sprites)
        {
            Transform existing = parent.Find("RestaurantBackground");
            GameObject bgObj;
            if (existing != null)
            {
                bgObj = existing.gameObject;
            }
            else
            {
                bgObj = new GameObject("RestaurantBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                bgObj.transform.SetParent(parent, false);
                bgObj.transform.SetAsFirstSibling();
            }

            RectTransform rt = GetOrAddComponent<RectTransform>(bgObj);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = GetOrAddComponent<Image>(bgObj);
            if (sprites.TryGetValue("U6 MA Main Restaurant Interior", out Sprite bgSprite))
            {
                img.sprite = bgSprite;
                img.color = Color.white;
            }
            else
            {
                img.color = new Color(0.92f, 0.94f, 0.96f, 1f);
            }
            img.raycastTarget = false;
        }

        // =========================================================================
        // Screen Setups with Deliberate Pivots & Anchors
        // =========================================================================

        private static void SetupLiveTableScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_LiveTableScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            // Safe Area Constraint Root (Interactive and core UI lives here)
            GameObject safeArea = CreateSafeAreaContainer(screenObj.transform);

            // 1. Top Header Banner: Pinned to Top-Center, Pivot (0.5, 1)
            CreateUIBox(safeArea.transform, "TitleBanner", "PART 1: WAITING FOR FOOD", 44,
                new Vector2(0, -10), new Vector2(920, 75),
                new Color(0.15f, 0.2f, 0.3f, 0.92f), Color.white,
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));

            // 2. Situation Prompt Card: Pinned below header, Pivot (0.5, 1)
            GameObject promptBox = CreateUIBox(safeArea.transform, "PromptBox", "The food is not here yet. Watch Anu.", 38,
                new Vector2(0, -95), new Vector2(1380, 85),
                new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f),
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));
            var promptText = promptBox.GetComponentInChildren<TextMeshProUGUI>();

            // 3. Wall Prop (Fish Tank): Anchored to Upper-Right wall, Pivot (0.5, 1)
            Sprite fishTankSp = GetSprite(sprites, "SPR_Prop_FishTank");
            if (fishTankSp != null)
            {
                CreateUISpriteBox(safeArea.transform, "FishTankProp", "",
                    Vector2.zero, new Vector2(240, 160), fishTankSp,
                    anchorMin: new Vector2(0.78f, 0.82f), anchorMax: new Vector2(0.78f, 0.82f), pivot: new Vector2(0.5f, 1f));
            }

            // 4. Background Diners: Anchored to background floor line, Pivot (0.5, 0)
            GameObject normalDiners1 = CreateUISpriteBox(safeArea.transform, "Diners_Normal_1", "Table 1 Normal",
                Vector2.zero, new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_EatingNormal 1"),
                anchorMin: new Vector2(0.24f, 0.44f), anchorMax: new Vector2(0.24f, 0.44f), pivot: new Vector2(0.5f, 0f));

            GameObject normalDiners2 = CreateUISpriteBox(safeArea.transform, "Diners_Normal_2", "Table 2 Normal",
                Vector2.zero, new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_EatingNormal 2"),
                anchorMin: new Vector2(0.76f, 0.44f), anchorMax: new Vector2(0.76f, 0.44f), pivot: new Vector2(0.5f, 0f));

            GameObject lookDiners1 = CreateUISpriteBox(safeArea.transform, "Diners_Look_1", "Table 1 Looking",
                Vector2.zero, new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_HeadsTurned 1"),
                anchorMin: new Vector2(0.24f, 0.44f), anchorMax: new Vector2(0.24f, 0.44f), pivot: new Vector2(0.5f, 0f));

            GameObject lookDiners2 = CreateUISpriteBox(safeArea.transform, "Diners_Look_2", "Table 2 Looking",
                Vector2.zero, new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_HeadsTurned 2"),
                anchorMin: new Vector2(0.76f, 0.44f), anchorMax: new Vector2(0.76f, 0.44f), pivot: new Vector2(0.5f, 0f));
            lookDiners1.SetActive(false);
            lookDiners2.SetActive(false);

            // 5. Main Child & Family Visual: Deliberate seated baseline Pivot (0.5, 0)
            // Bottom pivot ensures changing between straight/sliding/kneeling sprites preserves seated height!
            Sprite familyStraight = GetSprite(sprites, "U6_MAct_Family_SittingStraight");
            Sprite anuStraight = familyStraight ?? GetSprite(sprites, "SPR_Anu_SittingStraight");
            bool isFamilyComposite = familyStraight != null;
            Vector2 tableSize = isFamilyComposite ? new Vector2(1040, 540) : new Vector2(300, 420);
            Vector2 tableAnchor = isFamilyComposite ? new Vector2(0.5f, 0.08f) : new Vector2(0.5f, 0.28f);

            GameObject anuObj = CreateUISpriteBox(safeArea.transform, "AnuCharacterVisual", "",
                Vector2.zero, tableSize, anuStraight,
                anchorMin: tableAnchor, anchorMax: tableAnchor, pivot: new Vector2(0.5f, 0f));
            var anuImg = anuObj.GetComponent<Image>();
            if (anuImg != null) anuImg.preserveAspect = true;

            // 6. Interactive Action Button: Pinned to Bottom-Center of Safe Area, Pivot (0.5, 0)
            // Positioned at Y = +115 above bottom safe edge, well clear of host shell back button
            GameObject btnObj = CreateUIButton(safeArea.transform, "ActionButton", "STAY SEATED", 38,
                new Vector2(0, 115), new Vector2(480, 95), new Color(0.18f, 0.65f, 0.32f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var actionBtn = btnObj.GetComponent<Button>();
            var actionBtnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            // 7. Feedback Panel: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject feedbackPanel = CreateUIBox(safeArea.transform, "FeedbackPanel", "Nice job! Anu stays seated.", 36,
                new Vector2(0, 15), new Vector2(1300, 85), new Color(1f, 0.95f, 0.7f, 0.96f), new Color(0.2f, 0.15f, 0.05f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
            feedbackPanel.SetActive(false);

            // Wire Serialized Properties
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("promptText").objectReferenceValue = promptText;
            so.FindProperty("actionButton").objectReferenceValue = actionBtn;
            so.FindProperty("actionButtonText").objectReferenceValue = actionBtnText;
            so.FindProperty("feedbackPanel").objectReferenceValue = feedbackPanel;
            so.FindProperty("feedbackText").objectReferenceValue = feedbackText;
            so.FindProperty("anuCharacterImage").objectReferenceValue = anuImg;
            so.FindProperty("anuSittingStraightSprite").objectReferenceValue = anuStraight;

            SerializedProperty normArr = so.FindProperty("otherTablesNormal");
            normArr.arraySize = 2;
            normArr.GetArrayElementAtIndex(0).objectReferenceValue = normalDiners1;
            normArr.GetArrayElementAtIndex(1).objectReferenceValue = normalDiners2;

            SerializedProperty lookArr = so.FindProperty("otherTablesLooking");
            lookArr.arraySize = 2;
            lookArr.GetArrayElementAtIndex(0).objectReferenceValue = lookDiners1;
            lookArr.GetArrayElementAtIndex(1).objectReferenceValue = lookDiners2;

            // Populate Waiting Events
            Sprite fidgetFish = GetSprite(sprites, "U6_MAct_Family_SlidingChair") ?? GetSprite(sprites, "SPR_Anu_SlidingChair");
            Sprite fidgetGlass = GetSprite(sprites, "U6_MAct_Family_TappingGlass") ?? GetSprite(sprites, "SPR_Anu_TappingGlass");
            Sprite fidgetHungry = GetSprite(sprites, "U6_MAct_Family_ShoutingHungry") ?? GetSprite(sprites, "SPR_Anu_ShoutingHungry");
            Sprite fidgetKneel = GetSprite(sprites, "U6_MAct_Family_KneelingChair") ?? GetSprite(sprites, "SPR_Anu_KneelingChair");

            SerializedProperty eventsProp = so.FindProperty("waitingEvents");
            eventsProp.ClearArray();
            AddWaitingEvent(eventsProp, "Fish Tank", "Anu spots a fish tank and starts sliding off her chair!", "STAY SEATED", "Good job! Anu stays safely in her seat.", "Anu ran across! The waiter had to swerve around her.", "", "", fidgetFish);
            AddWaitingEvent(eventsProp, "Glass Tapping", "Anu picks up a spoon and starts tapping the glass!", "PUT SPOON DOWN", "Nice! The table remains quiet and polite.", "Ting ting ting! The other tables turn and stare.", "SFX_GlassTing", "", fidgetGlass);
            AddWaitingEvent(eventsProp, "Hungry Shout", "Anu is about to shout how hungry she is!", "WAIT QUIETLY", "Great patience! Food is being prepared.", "\"I am SO hungry! Where is my food?\" Mother looks embarrassed.", "", "VO_U6_ANU_7", fidgetHungry);
            AddWaitingEvent(eventsProp, "Kneeling on Chair", "Anu kneels up on the chair with feet underneath!", "FEET ON FLOOR", "Both feet on the floor! Sitting straight.", "The chair wobbled and nearly tipped over!", "SFX_ChairWobble", "", fidgetKneel);

            so.ApplyModifiedProperties();
        }

        private static void SetupMenuScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_MenuScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            GameObject safeArea = CreateSafeAreaContainer(screenObj.transform);

            // 1. Top Header Banner: Pinned to Top-Center, Pivot (0.5, 1)
            CreateUIBox(safeArea.transform, "MenuHeaderBanner", "O U R   M E N U", 46,
                new Vector2(0, -10), new Vector2(720, 75),
                new Color(0.35f, 0.2f, 0.1f, 0.92f), Color.white,
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));

            // 2. Responsive Card Grid Viewport: Relative proportional bounds (8%..92% X, 32%..85% Y)
            GameObject gridObj = new GameObject("CardsContainer", typeof(RectTransform), typeof(GridLayoutGroup));
            gridObj.transform.SetParent(safeArea.transform, false);
            RectTransform gridRT = gridObj.GetComponent<RectTransform>();
            gridRT.anchorMin = new Vector2(0.08f, 0.32f);
            gridRT.anchorMax = new Vector2(0.92f, 0.85f);
            gridRT.pivot = new Vector2(0.5f, 0.5f);
            gridRT.offsetMin = Vector2.zero;
            gridRT.offsetMax = Vector2.zero;

            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(320, 230);
            glg.spacing = new Vector2(50, 30);
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = 3;
            glg.childAlignment = TextAnchor.MiddleCenter;

            // Generate DishCard UI Template
            GameObject cardTemplate = CreateDishCardTemplate(safeArea.transform);
            cardTemplate.SetActive(false);

            // 3. Standing Waiter: Anchored to right floor plane, Pivot (0.5, 0)
            Sprite raviStanding = GetSprite(sprites, "SPR_Ravi_StandingPad");
            GameObject waiterStandingObj = CreateUISpriteBox(safeArea.transform, "WaiterStandingVisual", "",
                Vector2.zero, new Vector2(280, 540), raviStanding,
                anchorMin: new Vector2(0.85f, 0.12f), anchorMax: new Vector2(0.85f, 0.12f), pivot: new Vector2(0.5f, 0f));
            waiterStandingObj.SetActive(false);

            // 4. Interactive Call Waiter Button: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject callWaiterBtn = CreateUIButton(safeArea.transform, "CallWaiterButton", "CALL WAITER", 36,
                new Vector2(0, 115), new Vector2(440, 90), new Color(0.9f, 0.45f, 0.2f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));

            // 5. Read First Prompt Banner: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject readFirstPrompt = CreateUIBox(safeArea.transform, "ReadFirstPrompt", "READ FIRST", 42,
                new Vector2(0, 15), new Vector2(650, 85), new Color(0.9f, 0.25f, 0.25f, 0.96f), Color.white,
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var readFirstText = readFirstPrompt.GetComponentInChildren<TextMeshProUGUI>();
            readFirstPrompt.SetActive(false);

            // 6. Awkward Waiter Stammer Overlay: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject awkwardOverlay = CreateUIBox(safeArea.transform, "AwkwardOverlay", "Ravi waits politely...\nAnu: \"Ummm... ummm...\"", 36,
                new Vector2(0, 15), new Vector2(900, 100), new Color(1f, 0.9f, 0.7f, 0.96f), new Color(0.2f, 0.15f, 0.05f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var stammerText = awkwardOverlay.GetComponentInChildren<TextMeshProUGUI>();
            awkwardOverlay.SetActive(false);

            // 7. Order Choice Panel: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject choicePanel = new GameObject("OrderChoicePanel", typeof(RectTransform));
            choicePanel.transform.SetParent(safeArea.transform, false);
            RectTransform cpRT = choicePanel.GetComponent<RectTransform>();
            cpRT.anchorMin = new Vector2(0.5f, 0f);
            cpRT.anchorMax = new Vector2(0.5f, 0f);
            cpRT.pivot = new Vector2(0.5f, 0f);
            cpRT.anchoredPosition = new Vector2(0, 15);
            cpRT.sizeDelta = new Vector2(1480, 110);

            // Relative split for buttons inside choice container
            GameObject btnPolite = CreateUIButton(choicePanel.transform, "PoliteButton", "\"Could I have the dosa, please?\"", 32,
                Vector2.zero, Vector2.zero, new Color(0.25f, 0.68f, 0.38f),
                anchorMin: new Vector2(0.02f, 0.05f), anchorMax: new Vector2(0.48f, 0.95f), pivot: new Vector2(0.5f, 0.5f));

            GameObject btnImpolite = CreateUIButton(choicePanel.transform, "ImpoliteButton", "\"I want dosa.\"", 32,
                Vector2.zero, Vector2.zero, new Color(0.85f, 0.55f, 0.25f),
                anchorMin: new Vector2(0.52f, 0.05f), anchorMax: new Vector2(0.98f, 0.95f), pivot: new Vector2(0.5f, 0.5f));
            choicePanel.SetActive(false);

            // 8. Waiter Feedback: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject waiterFeedback = CreateUIBox(safeArea.transform, "WaiterFeedback", "Ravi: \"Certainly!\"", 34,
                new Vector2(0, 15), new Vector2(950, 80), new Color(0.85f, 0.95f, 1f, 0.96f), new Color(0.1f, 0.2f, 0.35f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var waiterDialogText = waiterFeedback.GetComponentInChildren<TextMeshProUGUI>();
            waiterFeedback.SetActive(false);

            // Wire Serialized Properties
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("cardsContainer").objectReferenceValue = gridObj.transform;
            so.FindProperty("dishCardPrefab").objectReferenceValue = cardTemplate;
            so.FindProperty("callWaiterButton").objectReferenceValue = callWaiterBtn.GetComponent<Button>();
            so.FindProperty("readFirstPrompt").objectReferenceValue = readFirstPrompt;
            so.FindProperty("readFirstText").objectReferenceValue = readFirstText;
            so.FindProperty("awkwardOverlay").objectReferenceValue = awkwardOverlay;
            so.FindProperty("awkwardStammerText").objectReferenceValue = stammerText;
            so.FindProperty("orderChoicePanel").objectReferenceValue = choicePanel;
            so.FindProperty("politeOptionButton").objectReferenceValue = btnPolite.GetComponent<Button>();
            so.FindProperty("impoliteOptionButton").objectReferenceValue = btnImpolite.GetComponent<Button>();
            so.FindProperty("waiterFeedbackBox").objectReferenceValue = waiterFeedback;
            so.FindProperty("waiterDialogText").objectReferenceValue = waiterDialogText;
            so.FindProperty("waiterStandingVisual").objectReferenceValue = waiterStandingObj.GetComponent<Image>();

            // Populate Default Dishes
            SerializedProperty dishesProp = so.FindProperty("dishes");
            dishesProp.ClearArray();
            AddDishItem(dishesProp, "Dosa", 60, GetSprite(sprites, "SPR_Dish_Dosa"));
            AddDishItem(dishesProp, "Idli", 40, GetSprite(sprites, "SPR_Dish_Idli"));
            AddDishItem(dishesProp, "Noodles", 80, GetSprite(sprites, "SPR_Dish_Noodles"));
            AddDishItem(dishesProp, "Sandwich", 70, GetSprite(sprites, "SPR_Dish_Sandwich"));
            AddDishItem(dishesProp, "Juice", 50, GetSprite(sprites, "SPR_Dish_Juice"));
            AddDishItem(dishesProp, "Ice Cream", 45, GetSprite(sprites, "SPR_Dish_IceCream"));

            so.ApplyModifiedProperties();
        }

        private static void SetupChoiceScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_WaiterInteractionScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            GameObject safeArea = CreateSafeAreaContainer(screenObj.transform);

            // 1. Top Header: Pinned to Top-Center, Pivot (0.5, 1)
            CreateUIBox(safeArea.transform, "TitleBanner", "PART 3: THE WAITER (RAVI)", 44,
                new Vector2(0, -10), new Vector2(920, 75),
                new Color(0.15f, 0.2f, 0.3f, 0.92f), Color.white,
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));

            // 2. Situation Prompt Card: Pinned below header, Pivot (0.5, 1)
            GameObject promptBox = CreateUIBox(safeArea.transform, "PromptBox", "Ravi brings a fresh jug of water.", 38,
                new Vector2(0, -95), new Vector2(1380, 85),
                new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f),
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));
            var promptText = promptBox.GetComponentInChildren<TextMeshProUGUI>();

            // Load Characters & Props
            Sprite warmSmile = GetSprite(sprites, "SPR_Ravi_WarmSmile");
            Sprite neutralBlank = GetSprite(sprites, "SPR_Ravi_NeutralBlank");
            Sprite stiffPolite = GetSprite(sprites, "SPR_Ravi_StiffPolite");
            Sprite raviWater = GetSprite(sprites, "SPR_Ravi_WaterJug");
            Sprite raviPlate = GetSprite(sprites, "SPR_Ravi_ServingPlate");
            Sprite raviPad = GetSprite(sprites, "SPR_Ravi_StandingPad");

            Sprite familyStraight = GetSprite(sprites, "U6_MAct_Family_SittingStraight");
            Sprite anuStraight = familyStraight ?? GetSprite(sprites, "SPR_Anu_SittingStraight");
            Sprite anuHandRaise = GetSprite(sprites, "U6_MAct_Family_HandRaise") ?? GetSprite(sprites, "SPR_Anu_HandRaise");
            Sprite emptyGlass = GetSprite(sprites, "SPR_EmptyGlass");
            Sprite spoonFork = GetSprite(sprites, "SPR_Spoon_and_Fork");
            Sprite dishDosa = GetSprite(sprites, "SPR_Dish_Dosa");

            // 3. Stage Characters & Table Plane:
            // Anu / Family Table: Seated at table baseline, Pivot (0.5, 0)
            bool isFamily = familyStraight != null;
            Vector2 anuSize = isFamily ? new Vector2(760, 420) : new Vector2(320, 480);
            Vector2 anuAnchor = isFamily ? new Vector2(0.32f, 0.16f) : new Vector2(0.26f, 0.28f);

            GameObject anuObj = CreateUISpriteBox(safeArea.transform, "AnuAvatar", "",
                Vector2.zero, anuSize, anuStraight,
                anchorMin: anuAnchor, anchorMax: anuAnchor, pivot: new Vector2(0.5f, 0f));

            // Prop Item: Resting on center table surface, Pivot (0.5, 0)
            GameObject propObj = CreateUISpriteBox(safeArea.transform, "PropItem", "",
                Vector2.zero, new Vector2(220, 180), emptyGlass,
                anchorMin: new Vector2(0.48f, 0.28f), anchorMax: new Vector2(0.48f, 0.28f), pivot: new Vector2(0.5f, 0f));

            // Waiter Ravi: Standing on floor baseline, Pivot (0.5, 0)
            GameObject waiterFullBodyObj = CreateUISpriteBox(safeArea.transform, "WaiterFullBody", "",
                Vector2.zero, new Vector2(360, 580), raviWater,
                anchorMin: new Vector2(0.74f, 0.18f), anchorMax: new Vector2(0.74f, 0.18f), pivot: new Vector2(0.5f, 0f));

            // 4. Interactive Choice Container: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject choiceContainer = new GameObject("ChoiceContainer", typeof(RectTransform));
            choiceContainer.transform.SetParent(safeArea.transform, false);
            RectTransform ccRT = choiceContainer.GetComponent<RectTransform>();
            ccRT.anchorMin = new Vector2(0.5f, 0f);
            ccRT.anchorMax = new Vector2(0.5f, 0f);
            ccRT.pivot = new Vector2(0.5f, 0f);
            ccRT.anchoredPosition = new Vector2(0, 115);
            ccRT.sizeDelta = new Vector2(1460, 100);

            // Option A and B: Deliberate relative horizontal distribution
            GameObject btnA = CreateUIButton(choiceContainer.transform, "OptionA_Button", "\"Thank you!\"", 32,
                Vector2.zero, Vector2.zero, new Color(0.25f, 0.68f, 0.38f),
                anchorMin: new Vector2(0.02f, 0.05f), anchorMax: new Vector2(0.48f, 0.95f), pivot: new Vector2(0.5f, 0.5f));

            GameObject btnB = CreateUIButton(choiceContainer.transform, "OptionB_Button", "(Say nothing)", 32,
                Vector2.zero, Vector2.zero, new Color(0.85f, 0.55f, 0.25f),
                anchorMin: new Vector2(0.52f, 0.05f), anchorMax: new Vector2(0.98f, 0.95f), pivot: new Vector2(0.5f, 0.5f));

            // 5. Outcome Feedback Panel: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject outcomePanel = CreateUIBox(safeArea.transform, "OutcomePanel", "Ravi smiles warmly and nods.", 36,
                new Vector2(0, 15), new Vector2(1300, 85), new Color(1f, 0.95f, 0.75f, 0.96f), new Color(0.2f, 0.15f, 0.05f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var outcomeText = outcomePanel.GetComponentInChildren<TextMeshProUGUI>();
            outcomePanel.SetActive(false);

            // Wire Serialized Properties
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("promptText").objectReferenceValue = promptText;
            so.FindProperty("waiterFullBodyAvatar").objectReferenceValue = waiterFullBodyObj.GetComponent<Image>();
            so.FindProperty("anuCharacterAvatar").objectReferenceValue = anuObj.GetComponent<Image>();
            so.FindProperty("propItemImage").objectReferenceValue = propObj.GetComponent<Image>();
            so.FindProperty("waiterExpressionBadge").objectReferenceValue = null;
            so.FindProperty("waiterSmileSprite").objectReferenceValue = warmSmile;
            so.FindProperty("waiterNeutralSprite").objectReferenceValue = neutralBlank;
            so.FindProperty("waiterStiffSprite").objectReferenceValue = stiffPolite;
            so.FindProperty("waiterNameBadge").objectReferenceValue = null;
            so.FindProperty("choiceContainer").objectReferenceValue = choiceContainer;
            so.FindProperty("optionA_Button").objectReferenceValue = btnA.GetComponent<Button>();
            so.FindProperty("optionA_Label").objectReferenceValue = btnA.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("optionB_Button").objectReferenceValue = btnB.GetComponent<Button>();
            so.FindProperty("optionB_Label").objectReferenceValue = btnB.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("outcomePanel").objectReferenceValue = outcomePanel;
            so.FindProperty("outcomeText").objectReferenceValue = outcomeText;

            // Populate all 5 Moments
            SerializedProperty momentsProp = so.FindProperty("moments");
            momentsProp.ClearArray();

            AddWaiterMoment(momentsProp, "Water Served", "Ravi brings a fresh jug of water to the table.", raviWater, emptyGlass, anuStraight,
                "\"Thank you!\"", "Ravi smiles warmly and nods.", "VO_U6_ANU_4", U6_WaiterFaceState.WarmSmile,
                "(Say nothing)", "Ravi is polite, but neutral.", "", U6_WaiterFaceState.BlankNeutral);

            AddWaiterMoment(momentsProp, "Calling Ravi", "Anu needs something and Ravi is across the room.", raviPad, null, anuHandRaise,
                "Raise a hand gently and make eye contact", "Ravi notices and comes over calmly.", "", U6_WaiterFaceState.WarmSmile,
                "Wave both arms and shout across the room", "Ravi hurries over while other tables look over.", "", U6_WaiterFaceState.BlankNeutral);

            AddWaiterMoment(momentsProp, "Wrong Dish", "Ravi brings the wrong dish by mistake!", raviPlate, null, anuStraight,
                "\"Sorry, I think this is the wrong dish.\"", "Ravi apologises warmly: \"I will fix that right away!\"", "VO_U6_WAIT_3", U6_WaiterFaceState.WarmSmile,
                "\"This is WRONG!\" (loudly)", "Ravi apologises stiffly and fixes it quietly.", "VO_U6_ANU_6", U6_WaiterFaceState.StiffPolite);

            AddWaiterMoment(momentsProp, "Food Served", "Ravi sets the hot dosa down in front of Anu.", raviPlate, dishDosa, anuStraight,
                "\"Thank you, Ravi!\"", "Ravi nods happily: \"Enjoy your meal!\"", "VO_U6_ANU_4", U6_WaiterFaceState.WarmSmile,
                "(Grab fork and eat immediately without looking)", "Ravi steps away quietly.", "", U6_WaiterFaceState.BlankNeutral);

            AddWaiterMoment(momentsProp, "Dropped Fork", "Anu accidentally drops her fork on the floor.", raviPad, spoonFork, anuStraight,
                "\"Excuse me, could I have another fork please?\"", "Ravi brings a clean fork right away with a smile.", "VO_U6_ANU_8", U6_WaiterFaceState.WarmSmile,
                "\"I dropped my fork!\" (shouted)", "Ravi brings one while the nearby diners look up.", "VO_U6_ANU_9", U6_WaiterFaceState.StiffPolite);

            so.ApplyModifiedProperties();
        }

        private static void SetupSliderScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_SliderScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            Sprite roundedSp = GetOrCreateRoundedBoxSprite();
            Sprite circleKnobSp = GetOrCreateCircleSprite();

            GameObject safeArea = CreateSafeAreaContainer(screenObj.transform);

            // 1. Top Header: Pinned to Top-Center, Pivot (0.5, 1)
            CreateUIBox(safeArea.transform, "TitleBanner", "PART 4: VOICES & GOODBYES", 44,
                new Vector2(0, -10), new Vector2(920, 75),
                new Color(0.15f, 0.2f, 0.3f, 0.92f), Color.white,
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));

            // ============================================
            // Phase 1: Volume Phase Container
            // ============================================
            GameObject volPhase = new GameObject("VolumePhaseContainer", typeof(RectTransform));
            volPhase.transform.SetParent(safeArea.transform, false);
            RectTransform vpRT = volPhase.GetComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.offsetMin = Vector2.zero;
            vpRT.offsetMax = Vector2.zero;

            // Prompt: Pinned below header, Pivot (0.5, 1)
            CreateUIBox(volPhase.transform, "VolPromptBox", "How loud should Anu speak in the restaurant?", 38,
                new Vector2(0, -95), new Vector2(1300, 85),
                new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f),
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));

            // 3 Zone Cards Row: Anchored to middle zone (Y = 0.62f)
            Sprite icWhisper = GetSprite(sprites, "SPR_Icon_VolWhisper");
            Sprite icJustRight = GetSprite(sprites, "SPR_Icon_VolJustRight");
            Sprite icBigVoice = GetSprite(sprites, "SPR_Icon_VolBigVoice");

            // Card Container: Deliberate Anchors (0.5, 0.62)
            GameObject cardsRow = new GameObject("CardsRow", typeof(RectTransform));
            cardsRow.transform.SetParent(volPhase.transform, false);
            RectTransform crRT = cardsRow.GetComponent<RectTransform>();
            crRT.anchorMin = new Vector2(0.5f, 0.62f);
            crRT.anchorMax = new Vector2(0.5f, 0.62f);
            crRT.pivot = new Vector2(0.5f, 0.5f);
            crRT.anchoredPosition = Vector2.zero;
            crRT.sizeDelta = new Vector2(1040, 140);

            // Whisper Card
            GameObject whisperCard = CreateVolumeCard(cardsRow.transform, "WhisperCard", "WHISPER", "Too Soft",
                new Vector2(-360, 0), new Color(0.92f, 0.96f, 1f, 0.95f), new Color(0.2f, 0.5f, 0.85f), icWhisper, roundedSp, out GameObject whisperHl);

            // Just Right Card
            GameObject justRightCard = CreateVolumeCard(cardsRow.transform, "JustRightCard", "JUST RIGHT", "Polite & Clear",
                new Vector2(0, 0), new Color(0.92f, 0.98f, 0.94f, 0.95f), new Color(0.15f, 0.65f, 0.3f), icJustRight, roundedSp, out GameObject justRightHl);
            justRightHl.SetActive(true);

            // Big Voice Card
            GameObject bigVoiceCard = CreateVolumeCard(cardsRow.transform, "BigVoiceCard", "BIG VOICE", "Too Loud!",
                new Vector2(360, 0), new Color(1f, 0.93f, 0.93f, 0.95f), new Color(0.85f, 0.3f, 0.2f), icBigVoice, roundedSp, out GameObject bigVoiceHl);

            // Slider: Anchored to middle zone (Y = 0.44f), Pivot (0.5, 0.5)
            GameObject sliderObj = new GameObject("VolumeSlider", typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(volPhase.transform, false);
            RectTransform sRT = sliderObj.GetComponent<RectTransform>();
            sRT.anchorMin = new Vector2(0.5f, 0.44f);
            sRT.anchorMax = new Vector2(0.5f, 0.44f);
            sRT.pivot = new Vector2(0.5f, 0.5f);
            sRT.anchoredPosition = Vector2.zero;
            sRT.sizeDelta = new Vector2(960, 56);

            Slider slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 2f;
            slider.wholeNumbers = true;
            slider.value = 1f;

            // Track Background
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(sliderObj.transform, false);
            RectTransform bgRT = bg.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            Image bgImg = bg.GetComponent<Image>();
            bgImg.sprite = roundedSp;
            bgImg.type = Image.Type.Sliced;
            bgImg.color = new Color(0.86f, 0.89f, 0.93f);
            bgImg.raycastTarget = false;

            // Colored Zone Strips inside Track Background
            CreateTrackZone(bg.transform, "Zone1_WhisperStrip", new Vector2(0f, 0f), new Vector2(0.333f, 1f), new Color(0.3f, 0.6f, 0.95f, 0.35f), roundedSp);
            CreateTrackZone(bg.transform, "Zone2_JustRightStrip", new Vector2(0.333f, 0f), new Vector2(0.666f, 1f), new Color(0.2f, 0.8f, 0.4f, 0.35f), roundedSp);
            CreateTrackZone(bg.transform, "Zone3_BigVoiceStrip", new Vector2(0.666f, 0f), new Vector2(1f, 1f), new Color(0.95f, 0.35f, 0.3f, 0.35f), roundedSp);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform faRT = fillArea.GetComponent<RectTransform>();
            faRT.anchorMin = Vector2.zero;
            faRT.anchorMax = Vector2.one;
            faRT.offsetMin = new Vector2(10, 6);
            faRT.offsetMax = new Vector2(-10, -6);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.sprite = roundedSp;
            fillImg.type = Image.Type.Sliced;
            fillImg.color = new Color(0.15f, 0.72f, 0.32f);
            fillImg.raycastTarget = false;
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Handle Slide Area & Circular Knob Handle
            GameObject handleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleSlideArea.transform.SetParent(sliderObj.transform, false);
            RectTransform hsaRT = handleSlideArea.GetComponent<RectTransform>();
            hsaRT.anchorMin = Vector2.zero;
            hsaRT.anchorMax = Vector2.one;
            hsaRT.offsetMin = new Vector2(36, 0);
            hsaRT.offsetMax = new Vector2(-36, 0);

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleSlideArea.transform, false);
            RectTransform hRT = handle.GetComponent<RectTransform>();
            hRT.sizeDelta = new Vector2(74, 74);
            hRT.pivot = new Vector2(0.5f, 0.5f);
            Image handleImg = handle.GetComponent<Image>();
            handleImg.sprite = circleKnobSp;
            handleImg.color = new Color(0.15f, 0.72f, 0.32f);

            // Inner White Knob Cap
            GameObject knobCore = new GameObject("KnobCore", typeof(RectTransform), typeof(Image));
            knobCore.transform.SetParent(handle.transform, false);
            RectTransform kcRT = knobCore.GetComponent<RectTransform>();
            kcRT.anchorMin = new Vector2(0.5f, 0.5f);
            kcRT.anchorMax = new Vector2(0.5f, 0.5f);
            kcRT.pivot = new Vector2(0.5f, 0.5f);
            kcRT.sizeDelta = new Vector2(56, 56);
            Image kcImg = knobCore.GetComponent<Image>();
            kcImg.sprite = circleKnobSp;
            kcImg.color = Color.white;
            kcImg.raycastTarget = false;

            // Inner Grip Dot
            GameObject knobDot = new GameObject("KnobDot", typeof(RectTransform), typeof(Image));
            knobDot.transform.SetParent(knobCore.transform, false);
            RectTransform kdRT = knobDot.GetComponent<RectTransform>();
            kdRT.anchorMin = new Vector2(0.5f, 0.5f);
            kdRT.anchorMax = new Vector2(0.5f, 0.5f);
            kdRT.pivot = new Vector2(0.5f, 0.5f);
            kdRT.sizeDelta = new Vector2(20, 20);
            Image kdImg = knobDot.GetComponent<Image>();
            kdImg.sprite = circleKnobSp;
            kdImg.color = new Color(0.15f, 0.72f, 0.32f);
            kdImg.raycastTarget = false;

            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImg;

            // Zone Badge Box: Anchored to lower-middle (Y = 0.30f), Pivot (0.5, 0.5)
            GameObject badgeBox = CreateUIBox(volPhase.transform, "ZoneBadgeBox", "", 34,
                Vector2.zero, new Vector2(640, 68),
                new Color(1f, 1f, 1f, 0.95f), Color.white,
                anchorMin: new Vector2(0.5f, 0.30f), anchorMax: new Vector2(0.5f, 0.30f), pivot: new Vector2(0.5f, 0.5f));
            var zoneLabel = badgeBox.GetComponentInChildren<TextMeshProUGUI>();
            zoneLabel.text = "JUST RIGHT  (Polite & Clear)";
            zoneLabel.color = new Color(0.15f, 0.72f, 0.32f);

            // Say It Button: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject sayBtn = CreateUIButton(volPhase.transform, "SayItButton", "SAY IT", 38,
                new Vector2(0, 110), new Vector2(380, 85), new Color(0.2f, 0.72f, 0.35f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));

            // ============================================
            // Phase 2: Leaving Container
            // ============================================
            GameObject leavingPhase = new GameObject("LeavingPhaseContainer", typeof(RectTransform));
            leavingPhase.transform.SetParent(safeArea.transform, false);
            RectTransform lpRT = leavingPhase.GetComponent<RectTransform>();
            lpRT.anchorMin = Vector2.zero;
            lpRT.anchorMax = Vector2.one;
            lpRT.offsetMin = Vector2.zero;
            lpRT.offsetMax = Vector2.zero;

            // Leaving Prompt: Pinned below header, Pivot (0.5, 1)
            CreateUIBox(leavingPhase.transform, "LeavePromptBox", "The meal is finished. Another family is waiting by the door.", 38,
                new Vector2(0, -95), new Vector2(1400, 85),
                new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f),
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));

            // Waiting Family Character Visual: Anchored to doorway baseline, Pivot (0.5, 0)
            Sprite waitingFamilySp = GetSprite(sprites, "Waiting Family");
            GameObject waitFamily = CreateUISpriteBox(leavingPhase.transform, "WaitingFamilyVisual", "Waiting Family",
                Vector2.zero, new Vector2(480, 340), waitingFamilySp,
                anchorMin: new Vector2(0.5f, 0.32f), anchorMax: new Vector2(0.5f, 0.32f), pivot: new Vector2(0.5f, 0f));

            // Decision Buttons: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject btnLeave = CreateUIButton(leavingPhase.transform, "LeavePolitelyButton", "Say thank you and leave", 34,
                new Vector2(-360, 110), new Vector2(640, 95), new Color(0.25f, 0.68f, 0.38f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));

            GameObject btnStay = CreateUIButton(leavingPhase.transform, "StayAndPlayButton", "Stay & play with spoons", 34,
                new Vector2(360, 110), new Vector2(640, 95), new Color(0.85f, 0.55f, 0.25f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            leavingPhase.SetActive(false);

            // 5. Shared Feedback Panel: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject feedback = CreateUIBox(safeArea.transform, "FeedbackPanel", "Feedback text", 36,
                new Vector2(0, 15), new Vector2(1300, 85), new Color(1f, 0.95f, 0.7f, 0.96f), new Color(0.2f, 0.15f, 0.05f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var feedbackText = feedback.GetComponentInChildren<TextMeshProUGUI>();
            feedback.SetActive(false);

            // Wire Serialized Properties
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("volumePhaseContainer").objectReferenceValue = volPhase;
            so.FindProperty("volumeSlider").objectReferenceValue = slider;
            so.FindProperty("sliderFillImage").objectReferenceValue = fillImg;
            so.FindProperty("handleKnobImage").objectReferenceValue = kdImg;
            so.FindProperty("currentZoneLabel").objectReferenceValue = zoneLabel;
            so.FindProperty("whisperHighlight").objectReferenceValue = whisperHl;
            so.FindProperty("justRightHighlight").objectReferenceValue = justRightHl;
            so.FindProperty("bigVoiceHighlight").objectReferenceValue = bigVoiceHl;
            so.FindProperty("sayItButton").objectReferenceValue = sayBtn.GetComponent<Button>();
            so.FindProperty("leavingPhaseContainer").objectReferenceValue = leavingPhase;
            so.FindProperty("waitingFamilyVisual").objectReferenceValue = waitFamily;
            so.FindProperty("leavePolitelyButton").objectReferenceValue = btnLeave.GetComponent<Button>();
            so.FindProperty("stayAndPlayButton").objectReferenceValue = btnStay.GetComponent<Button>();
            so.FindProperty("feedbackPanel").objectReferenceValue = feedback;
            so.FindProperty("feedbackText").objectReferenceValue = feedbackText;
            so.ApplyModifiedProperties();
        }

        private static void SetupEndingScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_EndingScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            GameObject safeArea = CreateSafeAreaContainer(screenObj.transform);

            // 1. Confetti Banner: Top stretch overlay, Pivot (0.5, 1)
            Sprite confettiSp = GetSprite(sprites, "SPR_FX_Confetti");
            if (confettiSp != null)
            {
                CreateUISpriteBox(safeArea.transform, "ConfettiBanner", "",
                    new Vector2(0, 0), new Vector2(1400, 420), confettiSp,
                    anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));
            }

            // 2. Stars Row: Pinned to Top-Center, Pivot (0.5, 1)
            GameObject starsRow = new GameObject("StarsContainer", typeof(RectTransform));
            starsRow.transform.SetParent(safeArea.transform, false);
            RectTransform srRT = starsRow.GetComponent<RectTransform>();
            srRT.anchorMin = new Vector2(0.5f, 1f);
            srRT.anchorMax = new Vector2(0.5f, 1f);
            srRT.pivot = new Vector2(0.5f, 1f);
            srRT.anchoredPosition = new Vector2(0, -40);
            srRT.sizeDelta = new Vector2(620, 130);

            Sprite goldStar = GetSprite(sprites, "SPR_Icon_GoldStar");
            GameObject s1 = CreateUISpriteBox(starsRow.transform, "Star1", "", new Vector2(-190, 0), new Vector2(130, 130), goldStar);
            GameObject s2 = CreateUISpriteBox(starsRow.transform, "Star2", "", new Vector2(0, 0), new Vector2(150, 150), goldStar);
            GameObject s3 = CreateUISpriteBox(starsRow.transform, "Star3", "", new Vector2(190, 0), new Vector2(130, 130), goldStar);

            // 3. Characters Celebrating: Anchored to floor baseline, Pivot (0.5, 0)
            Sprite anuWave = GetSprite(sprites, "SPR_Anu_GoodbyeWave");
            Sprite raviSmile = GetSprite(sprites, "SPR_Ravi_WarmSmile");
            CreateUISpriteBox(safeArea.transform, "AnuWaveCelebration", "",
                Vector2.zero, new Vector2(280, 420), anuWave,
                anchorMin: new Vector2(0.24f, 0.28f), anchorMax: new Vector2(0.24f, 0.28f), pivot: new Vector2(0.5f, 0f));

            CreateUISpriteBox(safeArea.transform, "RaviSmileCelebration", "",
                Vector2.zero, new Vector2(280, 420), raviSmile,
                anchorMin: new Vector2(0.76f, 0.28f), anchorMax: new Vector2(0.76f, 0.28f), pivot: new Vector2(0.5f, 0f));

            // 4. Celebration Card: Pinned below stars, Pivot (0.5, 1)
            GameObject bannerBox = CreateUIBox(safeArea.transform, "BannerCard", "THANK YOU, DO COME AGAIN!", 44,
                new Vector2(0, -185), new Vector2(1050, 80),
                new Color(0.12f, 0.18f, 0.32f, 0.95f), new Color(1f, 0.88f, 0.3f),
                anchorMin: new Vector2(0.5f, 1f), anchorMax: new Vector2(0.5f, 1f), pivot: new Vector2(0.5f, 1f));
            var bannerText = bannerBox.GetComponentInChildren<TextMeshProUGUI>();

            // 5. Reflection Panel: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject refPanel = CreateUIBox(safeArea.transform, "ReflectionPanel", "Teacher Reflection:\nWhat will you say to the waiter next time?", 36,
                new Vector2(0, 125), new Vector2(1150, 115),
                new Color(1f, 1f, 1f, 0.96f), new Color(0.15f, 0.18f, 0.25f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));
            var refText = refPanel.GetComponentInChildren<TextMeshProUGUI>();

            // 6. Interactive Restart Button: Pinned to Bottom-Center, Pivot (0.5, 0)
            GameObject restartBtn = CreateUIButton(safeArea.transform, "RestartButton", "Play Again", 38,
                new Vector2(0, 25), new Vector2(360, 80), new Color(0.2f, 0.72f, 0.35f),
                anchorMin: new Vector2(0.5f, 0f), anchorMax: new Vector2(0.5f, 0f), pivot: new Vector2(0.5f, 0f));

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

        // =========================================================================
        // UI Creation Helpers with Deliberate Pivots & Anchors
        // =========================================================================

        private static GameObject CreateUIBox(
            Transform parent, 
            string name, 
            string textContent, 
            float fontSize, 
            Vector2 anchoredPos, 
            Vector2 sizeDelta, 
            Color bgColor, 
            Color textColor,
            Vector2? anchorMin = null,
            Vector2? anchorMax = null,
            Vector2? pivot = null)
        {
            GameObject box = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            box.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(box);
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(box);
            img.sprite = GetOrCreateRoundedBoxSprite();
            img.type = Image.Type.Sliced;
            img.color = bgColor;
            img.raycastTarget = false;

            if (!string.IsNullOrEmpty(textContent))
            {
                CreateTMPText(box.transform, "Label", textContent, fontSize, Vector2.zero, Vector2.zero, TextAlignmentOptions.Center, textColor,
                    anchorMin: Vector2.zero, anchorMax: Vector2.one, padding: new Vector4(16, 8, 16, 8));
            }
            return box;
        }

        private static GameObject CreateUIButton(
            Transform parent, 
            string name, 
            string labelText, 
            float fontSize, 
            Vector2 anchoredPos, 
            Vector2 sizeDelta, 
            Color btnColor,
            Vector2? anchorMin = null,
            Vector2? anchorMax = null,
            Vector2? pivot = null)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(btnObj);
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(btnObj);
            img.sprite = GetOrCreateRoundedBoxSprite();
            img.type = Image.Type.Sliced;
            img.color = btnColor;
            img.raycastTarget = true;

            Button btn = btnObj.GetComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(Mathf.Min(1f, btnColor.r * 1.15f), Mathf.Min(1f, btnColor.g * 1.15f), Mathf.Min(1f, btnColor.b * 1.15f), 1f);
            colors.pressedColor = new Color(btnColor.r * 0.85f, btnColor.g * 0.85f, btnColor.b * 0.85f, 1f);
            btn.colors = colors;

            CreateTMPText(btnObj.transform, "Label", labelText, fontSize, Vector2.zero, Vector2.zero, TextAlignmentOptions.Center, Color.white,
                anchorMin: Vector2.zero, anchorMax: Vector2.one, padding: new Vector4(16, 8, 16, 8));
            return btnObj;
        }

        private static GameObject CreateUISpriteBox(
            Transform parent, 
            string name, 
            string fallbackText, 
            Vector2 anchoredPos, 
            Vector2 sizeDelta, 
            Sprite sprite,
            Vector2? anchorMin = null,
            Vector2? anchorMax = null,
            Vector2? pivot = null)
        {
            GameObject box = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            box.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(box);
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(box);
            if (sprite != null)
            {
                img.sprite = sprite;
                img.color = Color.white;
                img.preserveAspect = true;
            }
            else
            {
                img.sprite = GetOrCreateRoundedBoxSprite();
                img.type = Image.Type.Sliced;
                img.color = new Color(0.85f, 0.85f, 0.85f, 1f);
                if (!string.IsNullOrEmpty(fallbackText))
                    CreateTMPText(box.transform, "Label", fallbackText, 26, Vector2.zero, Vector2.zero, TextAlignmentOptions.Center, Color.black,
                        anchorMin: Vector2.zero, anchorMax: Vector2.one);
            }
            img.raycastTarget = false;
            return box;
        }

        private static TextMeshProUGUI CreateTMPText(
            Transform parent, 
            string name, 
            string content, 
            float fontSize, 
            Vector2 anchoredPos, 
            Vector2 sizeDelta, 
            TextAlignmentOptions alignment, 
            Color color,
            Vector2? anchorMin = null,
            Vector2? anchorMax = null,
            Vector2? pivot = null,
            Vector4? padding = null)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);

            if (anchorMin.HasValue && anchorMax.HasValue && anchorMin.Value == Vector2.zero && anchorMax.Value == Vector2.one)
            {
                Vector4 pad = padding ?? Vector4.zero;
                rt.offsetMin = new Vector2(pad.x, pad.w); // Left, Bottom
                rt.offsetMax = new Vector2(-pad.z, -pad.y); // Right, Top
            }
            else
            {
                rt.anchoredPosition = anchoredPos;
                rt.sizeDelta = sizeDelta;
            }

            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = alignment;
            tmp.color = color;
            tmp.enableWordWrapping = true;
            tmp.raycastTarget = false;

            return tmp;
        }

        private static GameObject CreateDishCardTemplate(Transform parent)
        {
            GameObject cardObj = new GameObject("DishCard_Template", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(U6_DishCardUI_Masters_Activity));
            cardObj.transform.SetParent(parent, false);

            RectTransform rt = cardObj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(320, 230);
            rt.pivot = new Vector2(0.5f, 0.5f);

            Sprite roundedSp = GetOrCreateRoundedBoxSprite();
            Image cardImg = cardObj.GetComponent<Image>();
            cardImg.sprite = roundedSp;
            cardImg.type = Image.Type.Sliced;
            cardImg.color = new Color(1f, 1f, 1f, 0.96f);

            // Icon: Anchored to upper half
            GameObject iconObj = new GameObject("DishIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(cardObj.transform, false);
            RectTransform iconRT = iconObj.GetComponent<RectTransform>();
            iconRT.anchorMin = new Vector2(0.5f, 0.65f);
            iconRT.anchorMax = new Vector2(0.5f, 0.65f);
            iconRT.pivot = new Vector2(0.5f, 0.5f);
            iconRT.sizeDelta = new Vector2(160, 115);
            iconRT.anchoredPosition = Vector2.zero;
            Image iconImg = iconObj.GetComponent<Image>();
            iconImg.preserveAspect = true;
            iconImg.raycastTarget = false;

            // Name: Anchored to lower middle
            var nameText = CreateTMPText(cardObj.transform, "DishName", "Dosa", 28,
                Vector2.zero, new Vector2(280, 42), TextAlignmentOptions.Center, new Color(0.1f, 0.1f, 0.1f),
                anchorMin: new Vector2(0.5f, 0.28f), anchorMax: new Vector2(0.5f, 0.28f), pivot: new Vector2(0.5f, 0.5f));

            // Price: Anchored to bottom
            var priceText = CreateTMPText(cardObj.transform, "PriceText", "Rs. 60", 26,
                Vector2.zero, new Vector2(280, 36), TextAlignmentOptions.Center, new Color(0.85f, 0.45f, 0.1f),
                anchorMin: new Vector2(0.5f, 0.12f), anchorMax: new Vector2(0.5f, 0.12f), pivot: new Vector2(0.5f, 0.5f));

            // Selection Highlight
            GameObject highlight = new GameObject("SelectionHighlight", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            highlight.transform.SetParent(cardObj.transform, false);
            RectTransform hlRT = highlight.GetComponent<RectTransform>();
            hlRT.anchorMin = Vector2.zero;
            hlRT.anchorMax = Vector2.one;
            hlRT.pivot = new Vector2(0.5f, 0.5f);
            hlRT.offsetMin = new Vector2(-6, -6);
            hlRT.offsetMax = new Vector2(6, 6);
            Image hlImg = highlight.GetComponent<Image>();
            hlImg.sprite = roundedSp;
            hlImg.type = Image.Type.Sliced;
            hlImg.color = new Color(0.15f, 0.75f, 1f, 0.45f);
            hlImg.raycastTarget = false;
            highlight.SetActive(false);

            var cardUI = cardObj.GetComponent<U6_DishCardUI_Masters_Activity>();
            SerializedObject so = new SerializedObject(cardUI);
            so.FindProperty("dishIcon").objectReferenceValue = iconImg;
            so.FindProperty("dishNameText").objectReferenceValue = nameText;
            so.FindProperty("priceText").objectReferenceValue = priceText;
            so.FindProperty("selectionHighlight").objectReferenceValue = highlight;
            so.FindProperty("cardButton").objectReferenceValue = cardObj.GetComponent<Button>();
            so.ApplyModifiedProperties();

            return cardObj;
        }

        private static GameObject CreateVolumeCard(
            Transform parent, 
            string name, 
            string title, 
            string sub, 
            Vector2 anchoredPos, 
            Color cardColor, 
            Color titleColor, 
            Sprite iconSprite, 
            Sprite roundedSp,
            out GameObject highlightObj)
        {
            GameObject card = new GameObject(name, typeof(RectTransform), typeof(Image));
            card.transform.SetParent(parent, false);

            RectTransform rt = card.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(320, 140);

            Image cImg = card.GetComponent<Image>();
            cImg.sprite = roundedSp;
            cImg.type = Image.Type.Sliced;
            cImg.color = cardColor;
            cImg.raycastTarget = false;

            CreateUISpriteBox(card.transform, "Icon", title,
                new Vector2(-80, 0), new Vector2(80, 80), iconSprite,
                anchorMin: new Vector2(0.5f, 0.5f), anchorMax: new Vector2(0.5f, 0.5f), pivot: new Vector2(0.5f, 0.5f));

            CreateTMPText(card.transform, "Title", title, 24,
                new Vector2(50, 20), new Vector2(180, 35), TextAlignmentOptions.Left, titleColor,
                anchorMin: new Vector2(0.5f, 0.5f), anchorMax: new Vector2(0.5f, 0.5f), pivot: new Vector2(0.5f, 0.5f));

            CreateTMPText(card.transform, "Sub", sub, 20,
                new Vector2(50, -18), new Vector2(180, 30), TextAlignmentOptions.Left, new Color(0.4f, 0.45f, 0.55f),
                anchorMin: new Vector2(0.5f, 0.5f), anchorMax: new Vector2(0.5f, 0.5f), pivot: new Vector2(0.5f, 0.5f));

            highlightObj = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
            highlightObj.transform.SetParent(card.transform, false);
            RectTransform hlRT = highlightObj.GetComponent<RectTransform>();
            hlRT.anchorMin = Vector2.zero;
            hlRT.anchorMax = Vector2.one;
            hlRT.pivot = new Vector2(0.5f, 0.5f);
            hlRT.offsetMin = new Vector2(-4, -4);
            hlRT.offsetMax = new Vector2(4, 4);
            Image hlImg = highlightObj.GetComponent<Image>();
            hlImg.sprite = roundedSp;
            hlImg.type = Image.Type.Sliced;
            hlImg.color = new Color(titleColor.r, titleColor.g, titleColor.b, 0.65f);
            hlImg.raycastTarget = false;
            highlightObj.SetActive(false);

            return card;
        }

        private static void CreateTrackZone(Transform parent, string name, Vector2 aMin, Vector2 aMax, Color col, Sprite roundedSp)
        {
            GameObject strip = new GameObject(name, typeof(RectTransform), typeof(Image));
            strip.transform.SetParent(parent, false);
            RectTransform rt = strip.GetComponent<RectTransform>();
            rt.anchorMin = aMin;
            rt.anchorMax = aMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(2, 4);
            rt.offsetMax = new Vector2(-2, -4);
            Image img = strip.GetComponent<Image>();
            img.sprite = roundedSp;
            img.type = Image.Type.Sliced;
            img.color = col;
            img.raycastTarget = false;
        }

        // =========================================================================
        // Data & Serialized Population
        // =========================================================================

        private static void AddWaitingEvent(
            SerializedProperty listProp, 
            string name, 
            string prompt, 
            string btnLabel, 
            string success, 
            string fail, 
            string sfx, 
            string vo, 
            Sprite sprite)
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            SerializedProperty elem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            elem.FindPropertyRelative("eventName").stringValue = name;
            elem.FindPropertyRelative("situationPrompt").stringValue = prompt;
            elem.FindPropertyRelative("actionButtonLabel").stringValue = btnLabel;
            elem.FindPropertyRelative("successFeedback").stringValue = success;
            elem.FindPropertyRelative("failFeedback").stringValue = fail;
            elem.FindPropertyRelative("sfxOnFail").stringValue = sfx;
            elem.FindPropertyRelative("voOnFail").stringValue = vo;
            elem.FindPropertyRelative("fidgetSprite").objectReferenceValue = sprite;
        }

        private static void AddDishItem(SerializedProperty listProp, string name, int price, Sprite sprite)
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            SerializedProperty elem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            elem.FindPropertyRelative("dishName").stringValue = name;
            elem.FindPropertyRelative("price").intValue = price;
            elem.FindPropertyRelative("dishSprite").objectReferenceValue = sprite;
        }

        private static void AddWaiterMoment(
            SerializedProperty listProp,
            string title,
            string prompt,
            Sprite fullBodyRavi,
            Sprite prop,
            Sprite anuSp,
            string optAText,
            string optAOutcome,
            string optAVO,
            U6_WaiterFaceState optAFace,
            string optBText,
            string optBOutcome,
            string optBVO,
            U6_WaiterFaceState optBFace)
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            SerializedProperty elem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            elem.FindPropertyRelative("situationTitle").stringValue = title;
            elem.FindPropertyRelative("situationPrompt").stringValue = prompt;
            elem.FindPropertyRelative("fullBodyPoseSprite").objectReferenceValue = fullBodyRavi;
            elem.FindPropertyRelative("propSprite").objectReferenceValue = prop;
            elem.FindPropertyRelative("anuReactionSprite").objectReferenceValue = anuSp;
            elem.FindPropertyRelative("optionA_Text").stringValue = optAText;
            elem.FindPropertyRelative("optionA_Outcome").stringValue = optAOutcome;
            elem.FindPropertyRelative("optionA_VO").stringValue = optAVO;
            elem.FindPropertyRelative("optionA_Face").enumValueIndex = (int)optAFace;
            elem.FindPropertyRelative("optionB_Text").stringValue = optBText;
            elem.FindPropertyRelative("optionB_Outcome").stringValue = optBOutcome;
            elem.FindPropertyRelative("optionB_VO").stringValue = optBVO;
            elem.FindPropertyRelative("optionB_Face").enumValueIndex = (int)optBFace;
        }

        // =========================================================================
        // Texture & Sprite Generation & Loading
        // =========================================================================

        private static Dictionary<string, Sprite> LoadAllSprites()
        {
            Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { SPRITES_PATH });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object obj in objects)
                {
                    if (obj is Sprite sp)
                    {
                        if (!dict.ContainsKey(sp.name))
                        {
                            dict.Add(sp.name, sp);
                        }
                    }
                }
            }
            return dict;
        }

        private static Sprite GetSprite(Dictionary<string, Sprite> dict, string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            if (dict.TryGetValue(key, out Sprite sp))
                return sp;

            string cleanKey = key.Replace(".png", "").Trim().ToLower();
            foreach (var kvp in dict)
            {
                string dictKey = kvp.Key.Replace(".png", "").Trim().ToLower();
                if (dictKey == cleanKey || dictKey.Contains(cleanKey) || cleanKey.Contains(dictKey))
                    return kvp.Value;
            }
            return null;
        }

        private static Sprite GetOrCreateRoundedBoxSprite()
        {
            string dir = "Assets/SeniorsActivityUnit6/Art";
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            string path = dir + "/UI_RoundedBox_9Slice.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            int size = 128;
            int radius = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int dx = Mathf.Max(0, Mathf.Max(radius - x, x - (size - 1 - radius)));
                    int dy = Mathf.Max(0, Mathf.Max(radius - y, y - (size - 1 - radius)));
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(radius + 0.5f - dist);
                    colors[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spriteBorder = new Vector4(radius, radius, radius, radius);
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite GetOrCreateCircleSprite()
        {
            string dir = "Assets/SeniorsActivityUnit6/Art";
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            string path = dir + "/UI_Circle_Knob.png";
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            int size = 128;
            float radius = size * 0.5f - 2f;
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float alpha = Mathf.Clamp01(radius + 0.5f - dist);
                    if (alpha <= 0)
                    {
                        colors[y * size + x] = Color.clear;
                    }
                    else
                    {
                        if (dist > radius - 12f)
                        {
                            colors[y * size + x] = new Color(1f, 1f, 1f, alpha);
                        }
                        else
                        {
                            colors[y * size + x] = new Color(0.96f, 0.96f, 0.98f, alpha);
                        }
                    }
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // =========================================================================
        // Audio Library Wiring
        // =========================================================================

        private static void AutoWireAudioLibrary(U6_AudioManager_Masters_Activity audioMgr, GameObject gmObj)
        {
            AudioSource[] sources = gmObj.GetComponents<AudioSource>();
            while (sources.Length < 4)
            {
                gmObj.AddComponent<AudioSource>();
                sources = gmObj.GetComponents<AudioSource>();
            }

            for (int s = 0; s < sources.Length; s++)
            {
                sources[s].playOnAwake = false;
                sources[s].clip = null;
            }

            SerializedObject so = new SerializedObject(audioMgr);
            so.FindProperty("bgmSource").objectReferenceValue = sources[0];
            so.FindProperty("ambSource").objectReferenceValue = sources[1];
            so.FindProperty("sfxSource").objectReferenceValue = sources[2];
            so.FindProperty("voSource").objectReferenceValue = sources[3];

            var soundListProp = so.FindProperty("sounds");
            soundListProp.ClearArray();
            HashSet<string> registeredIds = new HashSet<string>();

            if (System.IO.Directory.Exists(SFX_PATH))
            {
                string[] sfxGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { SFX_PATH });
                foreach (string guid in sfxGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string soundId = clip.name;
                    if (!registeredIds.Contains(soundId))
                    {
                        registeredIds.Add(soundId);
                        soundListProp.InsertArrayElementAtIndex(soundListProp.arraySize);
                        SerializedProperty elem = soundListProp.GetArrayElementAtIndex(soundListProp.arraySize - 1);
                        elem.FindPropertyRelative("id").stringValue = soundId;
                        elem.FindPropertyRelative("clip").objectReferenceValue = clip;
                        elem.FindPropertyRelative("volume").floatValue = 1f;
                    }
                }
            }

            if (System.IO.Directory.Exists(AUDIOS_PATH))
            {
                string[] voGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { AUDIOS_PATH });
                foreach (string guid in voGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string soundId = ResolveSoundId(clip.name);
                    if (!string.IsNullOrEmpty(soundId) && !registeredIds.Contains(soundId))
                    {
                        registeredIds.Add(soundId);
                        soundListProp.InsertArrayElementAtIndex(soundListProp.arraySize);
                        SerializedProperty elem = soundListProp.GetArrayElementAtIndex(soundListProp.arraySize - 1);
                        elem.FindPropertyRelative("id").stringValue = soundId;
                        elem.FindPropertyRelative("clip").objectReferenceValue = clip;
                        elem.FindPropertyRelative("volume").floatValue = 1f;
                    }
                }
            }

            so.ApplyModifiedProperties();
        }

        private static string ResolveSoundId(string fileName)
        {
            if (fileName.StartsWith("SFX_") || fileName.StartsWith("AMB_") || fileName.StartsWith("MUS_") || fileName.StartsWith("VO_"))
            {
                return fileName;
            }

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

            return null;
        }

        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            return comp;
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
