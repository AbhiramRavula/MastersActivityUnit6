#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Googolplex.Unit6
{
    public class U6_SceneSetupTool_Masters_Activity : EditorWindow
    {
        private const string SPRITES_PATH = "Assets/SeniorsActivityUnit6/Art/U6_MastersActivitySprites";
        private const string AUDIOS_PATH = "Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios";
        private const string SFX_PATH = "Assets/SeniorsActivityUnit6/SFX";

        [MenuItem("Unit 6/Update Only Part 3 (ChoiceScreen & Waiter Panel)")]
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
            Debug.Log("<color=green>[U6_SceneSetupTool] Successfully updated Part 3 (ChoiceScreen & Waiter Panel)! Other panels were NOT touched.</color>");
        }

        [MenuItem("Unit 6/Update Only EndingScreen (Ending Panel & Stars)")]
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
            Debug.Log("<color=green>[U6_SceneSetupTool] Successfully updated EndingScreen! Other panels were NOT touched.</color>");
        }

        [MenuItem("Unit 6/Generate and Assign All Assets & Hierarchy")]
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

            // Root Background Image
            SetupRootBackground(canvasObj.transform, spriteDict);

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

            // 4. Create Screens Container Panels under Canvas (Translucent so restaurant background shows)
            GameObject liveTablePanel = CreateOrGetPanel(canvasObj.transform, "LiveTableScreen", new Color(0f, 0f, 0f, 0f));
            GameObject menuPanel = CreateOrGetPanel(canvasObj.transform, "MenuScreen", new Color(0.1f, 0.08f, 0.06f, 0.45f));
            GameObject choicePanel = CreateOrGetPanel(canvasObj.transform, "ChoiceScreen", new Color(0.06f, 0.09f, 0.14f, 0.45f));
            GameObject sliderPanel = CreateOrGetPanel(canvasObj.transform, "SliderScreen", new Color(0.08f, 0.06f, 0.14f, 0.45f));
            GameObject endingPanel = CreateOrGetPanel(canvasObj.transform, "EndingScreen", new Color(0.06f, 0.08f, 0.14f, 0.55f));

            // Setup Screen 1: LiveTableScreen (Part 1 - Waiting)
            SetupLiveTableScreen(liveTablePanel, spriteDict);

            // Setup Screen 2: MenuScreen (Part 2 - Menu)
            SetupMenuScreen(menuPanel, spriteDict);

            // Setup Screen 3: ChoiceScreen (Part 3 - Waiter)
            SetupChoiceScreen(choicePanel, spriteDict);

            // Setup Screen 4: SliderScreen (Part 4 - Volume & Leaving)
            SetupSliderScreen(sliderPanel, spriteDict);

            // Setup Screen 5: EndingScreen
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

            Debug.Log("<color=#4CAF50><b>[Unit 6] Complete UI Hierarchy, Large Mobile Text & Assets updated!</b></color>");
        }

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

        private static void SetupLiveTableScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_LiveTableScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            // High-Contrast Title Header Card (Large & Bold)
            CreateUIBox(screenObj.transform, "TitleBanner", "PART 1: WAITING FOR FOOD", 44, new Vector2(0, 440), new Vector2(900, 80), new Color(0.15f, 0.2f, 0.3f, 0.92f), Color.white);

            // High-Contrast Situation Prompt Card (Large & Bold)
            GameObject promptBox = CreateUIBox(screenObj.transform, "PromptBox", "The food is not here yet. Watch Anu.", 38, new Vector2(0, 270), new Vector2(1400, 110), new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f));
            var promptText = promptBox.GetComponentInChildren<TextMeshProUGUI>();

            // Fish Tank Prop in background
            Sprite fishTankSp = GetSprite(sprites, "SPR_Prop_FishTank");
            if (fishTankSp != null)
            {
                CreateUISpriteBox(screenObj.transform, "FishTankProp", "", new Vector2(490, 270), new Vector2(230, 160), fishTankSp);
            }

            // Background Diner Table Rows with rounded borders
            GameObject normalDiners1 = CreateUISpriteBox(screenObj.transform, "Diners_Normal_1", "Table 1 Normal", new Vector2(-480, 110), new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_EatingNormal 1"));
            GameObject normalDiners2 = CreateUISpriteBox(screenObj.transform, "Diners_Normal_2", "Table 2 Normal", new Vector2(480, 110), new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_EatingNormal 2"));

            GameObject lookDiners1 = CreateUISpriteBox(screenObj.transform, "Diners_Look_1", "Table 1 Looking", new Vector2(-480, 110), new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_HeadsTurned 1"));
            GameObject lookDiners2 = CreateUISpriteBox(screenObj.transform, "Diners_Look_2", "Table 2 Looking", new Vector2(480, 110), new Vector2(440, 240), GetSprite(sprites, "SPR_Diners_HeadsTurned 2"));
            lookDiners1.SetActive(false);
            lookDiners2.SetActive(false);

            // Anu Foreground Character Visual
            Sprite anuStraight = GetSprite(sprites, "SPR_Anu_SittingStraight");
            GameObject anuObj = CreateUISpriteBox(screenObj.transform, "AnuCharacterVisual", "", new Vector2(0, 50), new Vector2(260, 360), anuStraight);
            var anuImg = anuObj.GetComponent<Image>();

            // Action Button (Large & Bold)
            GameObject btnObj = CreateUIButton(screenObj.transform, "ActionButton", "STAY SEATED", 38, new Vector2(0, -220), new Vector2(480, 100), new Color(0.18f, 0.65f, 0.32f));
            var actionBtn = btnObj.GetComponent<Button>();
            var actionBtnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();

            // Feedback Panel (Large & Bold)
            GameObject feedbackPanel = CreateUIBox(screenObj.transform, "FeedbackPanel", "Nice job! Anu stays seated.", 36, new Vector2(0, -370), new Vector2(1300, 100), new Color(1f, 0.95f, 0.7f, 0.96f), new Color(0.2f, 0.15f, 0.05f));
            var feedbackText = feedbackPanel.GetComponentInChildren<TextMeshProUGUI>();
            feedbackPanel.SetActive(false);

            // Wire SerializedObject
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

            // Populate Waiting Events with their specific Sliced Sprites
            SerializedProperty eventsProp = so.FindProperty("waitingEvents");
            eventsProp.ClearArray();
            AddWaitingEvent(eventsProp, "Fish Tank", "Anu spots a fish tank and starts sliding off her chair!", "STAY SEATED", "Good job! Anu stays safely in her seat.", "Anu ran across! The waiter had to swerve around her.", "", "", GetSprite(sprites, "SPR_Anu_SlidingChair"));
            AddWaitingEvent(eventsProp, "Glass Tapping", "Anu picks up a spoon and starts tapping the glass!", "PUT SPOON DOWN", "Nice! The table remains quiet and polite.", "Ting ting ting! The other tables turn and stare.", "SFX_GlassTing", "", GetSprite(sprites, "SPR_Anu_TappingGlass"));
            AddWaitingEvent(eventsProp, "Hungry Shout", "Anu is about to shout how hungry she is!", "WAIT QUIETLY", "Great patience! Food is being prepared.", "\"I am SO hungry! Where is my food?\" Mother looks embarrassed.", "", "VO_U6_ANU_7", GetSprite(sprites, "SPR_Anu_ShoutingHungry"));
            AddWaitingEvent(eventsProp, "Kneeling on Chair", "Anu kneels up on the chair with feet underneath!", "FEET ON FLOOR", "Both feet on the floor! Sitting straight.", "The chair wobbled and nearly tipped over!", "SFX_ChairWobble", "", GetSprite(sprites, "SPR_Anu_KneelingChair"));

            so.ApplyModifiedProperties();
        }

        private static void SetupMenuScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_MenuScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            // Header Banner
            CreateUIBox(screenObj.transform, "MenuHeaderBanner", "O U R   M E N U", 46, new Vector2(0, 440), new Vector2(700, 80), new Color(0.35f, 0.2f, 0.1f, 0.92f), Color.white);

            // Grid Container for 6 cards
            GameObject gridObj = new GameObject("CardsContainer", typeof(RectTransform), typeof(GridLayoutGroup));
            gridObj.transform.SetParent(screenObj.transform, false);
            RectTransform gridRT = gridObj.GetComponent<RectTransform>();
            gridRT.sizeDelta = new Vector2(1150, 520);
            gridRT.anchoredPosition = new Vector2(0, 80);

            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(320, 230);
            glg.spacing = new Vector2(60, 40);
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = 3;
            glg.childAlignment = TextAnchor.MiddleCenter;

            // Generate DishCard UI Template (Rounded & Large fonts)
            GameObject cardTemplate = CreateDishCardTemplate(screenObj.transform);
            cardTemplate.SetActive(false);

            // Call Waiter Button (Large & Bold)
            GameObject callWaiterBtn = CreateUIButton(screenObj.transform, "CallWaiterButton", "CALL WAITER", 36, new Vector2(0, -250), new Vector2(440, 90), new Color(0.9f, 0.45f, 0.2f));

            // Read First Prompt Banner
            GameObject readFirstPrompt = CreateUIBox(screenObj.transform, "ReadFirstPrompt", "READ FIRST", 42, new Vector2(0, -370), new Vector2(650, 90), new Color(0.9f, 0.25f, 0.25f, 0.96f), Color.white);
            var readFirstText = readFirstPrompt.GetComponentInChildren<TextMeshProUGUI>();
            readFirstPrompt.SetActive(false);

            // Awkward Waiter Stammer Overlay
            GameObject awkwardOverlay = CreateUIBox(screenObj.transform, "AwkwardOverlay", "Ravi waits politely...\nAnu: \"Ummm... ummm...\"", 36, new Vector2(0, -370), new Vector2(900, 110), new Color(1f, 0.9f, 0.7f, 0.96f), new Color(0.2f, 0.15f, 0.05f));
            var stammerText = awkwardOverlay.GetComponentInChildren<TextMeshProUGUI>();
            awkwardOverlay.SetActive(false);

            // Order Choice Panel (Rounded large buttons)
            GameObject choicePanel = new GameObject("OrderChoicePanel", typeof(RectTransform));
            choicePanel.transform.SetParent(screenObj.transform, false);
            RectTransform cpRT = choicePanel.GetComponent<RectTransform>();
            cpRT.anchoredPosition = new Vector2(0, -380);
            cpRT.sizeDelta = new Vector2(1500, 130);

            GameObject btnPolite = CreateUIButton(choicePanel.transform, "PoliteButton", "\"Could I have the dosa, please?\"", 32, new Vector2(-380, 0), new Vector2(700, 95), new Color(0.25f, 0.68f, 0.38f));
            GameObject btnImpolite = CreateUIButton(choicePanel.transform, "ImpoliteButton", "\"I want dosa.\"", 32, new Vector2(380, 0), new Vector2(700, 95), new Color(0.85f, 0.55f, 0.25f));
            choicePanel.SetActive(false);

            // Waiter Feedback
            GameObject waiterFeedback = CreateUIBox(screenObj.transform, "WaiterFeedback", "Ravi: \"Certainly!\"", 34, new Vector2(0, -450), new Vector2(950, 80), new Color(0.85f, 0.95f, 1f, 0.96f), new Color(0.1f, 0.2f, 0.35f));
            var waiterDialogText = waiterFeedback.GetComponentInChildren<TextMeshProUGUI>();
            waiterFeedback.SetActive(false);

            // Full Body Waiter on the right
            Sprite raviStanding = GetSprite(sprites, "SPR_Ravi_StandingPad");
            GameObject waiterStandingObj = CreateUISpriteBox(screenObj.transform, "WaiterStandingVisual", "", new Vector2(620, -40), new Vector2(280, 540), raviStanding);
            waiterStandingObj.SetActive(false);

            // Wire SerializedObject with default dishes containing Sprites
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
            so.FindProperty("waiterStandingVisual").objectReferenceValue = waiterStandingObj.GetComponent<Image>();
            so.FindProperty("waiterFeedbackPopup").objectReferenceValue = waiterFeedback;
            so.FindProperty("waiterDialogText").objectReferenceValue = waiterDialogText;

            SerializedProperty dishesProp = so.FindProperty("defaultDishes");
            dishesProp.ClearArray();
            AddDishEntry(dishesProp, "dosa", "Dosa", 60, GetSprite(sprites, "SPR_Dish_Dosa"));
            AddDishEntry(dishesProp, "idli", "Idli", 40, GetSprite(sprites, "SPR_Dish_Idli"));
            AddDishEntry(dishesProp, "noodles", "Noodles", 90, GetSprite(sprites, "SPR_Dish_Noodles"));
            AddDishEntry(dishesProp, "rice", "Rice", 80, GetSprite(sprites, "SPR_Dish_Rice"));
            AddDishEntry(dishesProp, "roti", "Roti", 30, GetSprite(sprites, "SPR_Dish_Roti"));
            AddDishEntry(dishesProp, "icecream", "Ice Cream", 50, GetSprite(sprites, "SPR_Dish_IceCream"));

            so.ApplyModifiedProperties();
        }

        private static void AddDishEntry(SerializedProperty listProp, string id, string name, int price, Sprite sprite)
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            SerializedProperty elem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            elem.FindPropertyRelative("dishId").stringValue = id;
            elem.FindPropertyRelative("dishName").stringValue = name;
            elem.FindPropertyRelative("price").intValue = price;
            elem.FindPropertyRelative("dishSprite").objectReferenceValue = sprite;
        }

        private static void AddWaitingEvent(SerializedProperty listProp, string name, string prompt, string btnLabel, string success, string fail, string sfxFail, string voFail, Sprite fidgetSp)
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            SerializedProperty elem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            elem.FindPropertyRelative("eventName").stringValue = name;
            elem.FindPropertyRelative("situationPrompt").stringValue = prompt;
            elem.FindPropertyRelative("actionButtonLabel").stringValue = btnLabel;
            elem.FindPropertyRelative("successFeedback").stringValue = success;
            elem.FindPropertyRelative("failFeedback").stringValue = fail;
            elem.FindPropertyRelative("sfxOnFail").stringValue = sfxFail;
            elem.FindPropertyRelative("voOnFail").stringValue = voFail;
            elem.FindPropertyRelative("fidgetSprite").objectReferenceValue = fidgetSp;
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

        private static void SetupChoiceScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_WaiterInteractionScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            CreateUIBox(screenObj.transform, "TitleBanner", "PART 3: THE WAITER (RAVI)", 44, new Vector2(0, 440), new Vector2(900, 80), new Color(0.15f, 0.2f, 0.3f, 0.92f), Color.white);

            Sprite warmSmile = GetSprite(sprites, "SPR_Ravi_WarmSmile");
            Sprite neutralBlank = GetSprite(sprites, "SPR_Ravi_NeutralBlank");
            Sprite stiffPolite = GetSprite(sprites, "SPR_Ravi_StiffPolite");
            Sprite raviWater = GetSprite(sprites, "SPR_Ravi_WaterJug");
            Sprite raviPlate = GetSprite(sprites, "SPR_Ravi_ServingPlate");
            Sprite raviPad = GetSprite(sprites, "SPR_Ravi_StandingPad");

            Sprite anuStraight = GetSprite(sprites, "SPR_Anu_SittingStraight");
            Sprite anuHandRaise = GetSprite(sprites, "SPR_Anu_HandRaise");
            Sprite emptyGlass = GetSprite(sprites, "SPR_EmptyGlass");
            Sprite spoonFork = GetSprite(sprites, "SPR_Spoon_and_Fork");
            Sprite dishDosa = GetSprite(sprites, "SPR_Dish_Dosa");
            Sprite dishNoodles = GetSprite(sprites, "SPR_Dish_Noodles");

            // Character Stage Visuals: Anu (Left), Prop (Center Table), Waiter Ravi (Right)
            GameObject anuObj = CreateUISpriteBox(screenObj.transform, "AnuAvatar", "", new Vector2(-430, 40), new Vector2(320, 480), anuStraight);
            GameObject propObj = CreateUISpriteBox(screenObj.transform, "PropItem", "", new Vector2(-30, -30), new Vector2(200, 160), emptyGlass);
            GameObject waiterFullBodyObj = CreateUISpriteBox(screenObj.transform, "WaiterFullBody", "", new Vector2(380, 70), new Vector2(360, 560), raviWater);

            // Prompt Text Card
            GameObject promptBox = CreateUIBox(screenObj.transform, "PromptBox", "Ravi brings a fresh jug of water.", 38, new Vector2(0, -90), new Vector2(1350, 90), new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f));
            var promptText = promptBox.GetComponentInChildren<TextMeshProUGUI>();

            // Choice Container with 2 large rounded buttons
            GameObject choiceContainer = new GameObject("ChoiceContainer", typeof(RectTransform));
            choiceContainer.transform.SetParent(screenObj.transform, false);
            RectTransform ccRT = choiceContainer.GetComponent<RectTransform>();
            ccRT.anchoredPosition = new Vector2(0, -220);
            ccRT.sizeDelta = new Vector2(1500, 120);

            GameObject btnA = CreateUIButton(choiceContainer.transform, "OptionA_Button", "\"Thank you!\"", 32, new Vector2(-370, 0), new Vector2(680, 95), new Color(0.25f, 0.68f, 0.38f));
            GameObject btnB = CreateUIButton(choiceContainer.transform, "OptionB_Button", "(Say nothing)", 32, new Vector2(370, 0), new Vector2(680, 95), new Color(0.85f, 0.55f, 0.25f));

            // Outcome Feedback
            GameObject outcomePanel = CreateUIBox(screenObj.transform, "OutcomePanel", "Ravi smiles warmly and nods.", 36, new Vector2(0, -340), new Vector2(1300, 90), new Color(1f, 0.95f, 0.75f, 0.96f), new Color(0.2f, 0.15f, 0.05f));
            var outcomeText = outcomePanel.GetComponentInChildren<TextMeshProUGUI>();
            outcomePanel.SetActive(false);

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

            // Populate all 5 Moments with their exact full-body poses and props!
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

        private static void SetupSliderScreen(GameObject screenObj, Dictionary<string, Sprite> sprites)
        {
            var comp = GetOrAddComponent<U6_SliderScreen_Masters_Activity>(screenObj);
            ClearChildren(screenObj.transform);

            Sprite roundedSp = GetOrCreateRoundedBoxSprite();
            Sprite circleKnobSp = GetOrCreateCircleSprite();

            CreateUIBox(screenObj.transform, "TitleBanner", "PART 4: VOICES & GOODBYES", 44, new Vector2(0, 440), new Vector2(900, 80), new Color(0.15f, 0.2f, 0.3f, 0.92f), Color.white);

            // Phase 1: Volume Container
            GameObject volPhase = new GameObject("VolumePhaseContainer", typeof(RectTransform));
            volPhase.transform.SetParent(screenObj.transform, false);
            RectTransform vpRT = volPhase.GetComponent<RectTransform>();
            vpRT.anchorMin = Vector2.zero;
            vpRT.anchorMax = Vector2.one;
            vpRT.offsetMin = Vector2.zero;
            vpRT.offsetMax = Vector2.zero;

            CreateUIBox(volPhase.transform, "VolPromptBox", "How loud should Anu speak in the restaurant?", 38, new Vector2(0, 315), new Vector2(1300, 90), new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f));

            // Zone Icons & Cards (3 Cards above the slider)
            Sprite icWhisper = GetSprite(sprites, "SPR_Icon_VolWhisper");
            Sprite icJustRight = GetSprite(sprites, "SPR_Icon_VolJustRight");
            Sprite icBigVoice = GetSprite(sprites, "SPR_Icon_VolBigVoice");

            // 1. Whisper Card (-340, 150)
            GameObject whisperCard = new GameObject("WhisperCard", typeof(RectTransform), typeof(Image));
            whisperCard.transform.SetParent(volPhase.transform, false);
            whisperCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350, 150);
            whisperCard.GetComponent<RectTransform>().sizeDelta = new Vector2(310, 150);
            Image wCardImg = whisperCard.GetComponent<Image>();
            wCardImg.sprite = roundedSp;
            wCardImg.type = Image.Type.Sliced;
            wCardImg.color = new Color(0.92f, 0.96f, 1f, 0.95f);

            CreateUISpriteBox(whisperCard.transform, "Icon", "Whisper", new Vector2(-80, 0), new Vector2(80, 80), icWhisper);
            CreateTMPText(whisperCard.transform, "Title", "WHISPER", 24, new Vector2(50, 22), new Vector2(180, 35), TextAlignmentOptions.Left, new Color(0.2f, 0.5f, 0.85f));
            CreateTMPText(whisperCard.transform, "Sub", "Too Soft", 20, new Vector2(50, -18), new Vector2(180, 30), TextAlignmentOptions.Left, new Color(0.4f, 0.5f, 0.6f));

            GameObject whisperHl = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
            whisperHl.transform.SetParent(whisperCard.transform, false);
            RectTransform whlRT = whisperHl.GetComponent<RectTransform>();
            whlRT.anchorMin = Vector2.zero;
            whlRT.anchorMax = Vector2.one;
            whlRT.offsetMin = new Vector2(-4, -4);
            whlRT.offsetMax = new Vector2(4, 4);
            Image whlImg = whisperHl.GetComponent<Image>();
            whlImg.sprite = roundedSp;
            whlImg.type = Image.Type.Sliced;
            whlImg.color = new Color(0.2f, 0.55f, 0.95f, 0.65f);
            whisperHl.SetActive(false);

            // 2. Just Right Card (0, 150)
            GameObject justRightCard = new GameObject("JustRightCard", typeof(RectTransform), typeof(Image));
            justRightCard.transform.SetParent(volPhase.transform, false);
            justRightCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            justRightCard.GetComponent<RectTransform>().sizeDelta = new Vector2(310, 150);
            Image jrCardImg = justRightCard.GetComponent<Image>();
            jrCardImg.sprite = roundedSp;
            jrCardImg.type = Image.Type.Sliced;
            jrCardImg.color = new Color(0.92f, 0.98f, 0.94f, 0.95f);

            CreateUISpriteBox(justRightCard.transform, "Icon", "Just Right", new Vector2(-80, 0), new Vector2(80, 80), icJustRight);
            CreateTMPText(justRightCard.transform, "Title", "JUST RIGHT", 24, new Vector2(50, 22), new Vector2(180, 35), TextAlignmentOptions.Left, new Color(0.15f, 0.65f, 0.3f));
            CreateTMPText(justRightCard.transform, "Sub", "Polite & Clear", 20, new Vector2(50, -18), new Vector2(180, 30), TextAlignmentOptions.Left, new Color(0.3f, 0.55f, 0.35f));

            GameObject justRightHl = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
            justRightHl.transform.SetParent(justRightCard.transform, false);
            RectTransform jrhlRT = justRightHl.GetComponent<RectTransform>();
            jrhlRT.anchorMin = Vector2.zero;
            jrhlRT.anchorMax = Vector2.one;
            jrhlRT.offsetMin = new Vector2(-4, -4);
            jrhlRT.offsetMax = new Vector2(4, 4);
            Image jrhlImg = justRightHl.GetComponent<Image>();
            jrhlImg.sprite = roundedSp;
            jrhlImg.type = Image.Type.Sliced;
            jrhlImg.color = new Color(0.15f, 0.72f, 0.32f, 0.75f);
            justRightHl.SetActive(true);

            // 3. Big Voice Card (350, 150)
            GameObject bigVoiceCard = new GameObject("BigVoiceCard", typeof(RectTransform), typeof(Image));
            bigVoiceCard.transform.SetParent(volPhase.transform, false);
            bigVoiceCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(350, 150);
            bigVoiceCard.GetComponent<RectTransform>().sizeDelta = new Vector2(310, 150);
            Image bvCardImg = bigVoiceCard.GetComponent<Image>();
            bvCardImg.sprite = roundedSp;
            bvCardImg.type = Image.Type.Sliced;
            bvCardImg.color = new Color(1f, 0.93f, 0.93f, 0.95f);

            CreateUISpriteBox(bigVoiceCard.transform, "Icon", "Big Voice", new Vector2(-80, 0), new Vector2(80, 80), icBigVoice);
            CreateTMPText(bigVoiceCard.transform, "Title", "BIG VOICE", 24, new Vector2(50, 22), new Vector2(180, 35), TextAlignmentOptions.Left, new Color(0.85f, 0.3f, 0.2f));
            CreateTMPText(bigVoiceCard.transform, "Sub", "Too Loud!", 20, new Vector2(50, -18), new Vector2(180, 30), TextAlignmentOptions.Left, new Color(0.6f, 0.35f, 0.35f));

            GameObject bigVoiceHl = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
            bigVoiceHl.transform.SetParent(bigVoiceCard.transform, false);
            RectTransform bvhlRT = bigVoiceHl.GetComponent<RectTransform>();
            bvhlRT.anchorMin = Vector2.zero;
            bvhlRT.anchorMax = Vector2.one;
            bvhlRT.offsetMin = new Vector2(-4, -4);
            bvhlRT.offsetMax = new Vector2(4, 4);
            Image bvhlImg = bigVoiceHl.GetComponent<Image>();
            bvhlImg.sprite = roundedSp;
            bvhlImg.type = Image.Type.Sliced;
            bvhlImg.color = new Color(0.92f, 0.3f, 0.22f, 0.65f);
            bigVoiceHl.SetActive(false);

            // ============================================
            // Modern UI Slider Hierarchy
            // ============================================
            GameObject sliderObj = new GameObject("VolumeSlider", typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(volPhase.transform, false);
            RectTransform sRT = sliderObj.GetComponent<RectTransform>();
            sRT.sizeDelta = new Vector2(960, 54);
            sRT.anchoredPosition = new Vector2(0, -20);

            Slider slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 2f;
            slider.wholeNumbers = true;
            slider.value = 1f; // Center: Just Right

            // Background Track Container
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

            // Colored Zone Strips inside Track Background
            GameObject zone1Strip = new GameObject("Zone1_WhisperStrip", typeof(RectTransform), typeof(Image));
            zone1Strip.transform.SetParent(bg.transform, false);
            RectTransform z1RT = zone1Strip.GetComponent<RectTransform>();
            z1RT.anchorMin = new Vector2(0f, 0f);
            z1RT.anchorMax = new Vector2(0.333f, 1f);
            z1RT.offsetMin = new Vector2(4, 4);
            z1RT.offsetMax = new Vector2(-2, -4);
            Image z1Img = zone1Strip.GetComponent<Image>();
            z1Img.sprite = roundedSp;
            z1Img.type = Image.Type.Sliced;
            z1Img.color = new Color(0.3f, 0.6f, 0.95f, 0.3f);

            GameObject zone2Strip = new GameObject("Zone2_JustRightStrip", typeof(RectTransform), typeof(Image));
            zone2Strip.transform.SetParent(bg.transform, false);
            RectTransform z2RT = zone2Strip.GetComponent<RectTransform>();
            z2RT.anchorMin = new Vector2(0.333f, 0f);
            z2RT.anchorMax = new Vector2(0.666f, 1f);
            z2RT.offsetMin = new Vector2(2, 4);
            z2RT.offsetMax = new Vector2(-2, -4);
            Image z2Img = zone2Strip.GetComponent<Image>();
            z2Img.sprite = roundedSp;
            z2Img.type = Image.Type.Sliced;
            z2Img.color = new Color(0.2f, 0.8f, 0.4f, 0.3f);

            GameObject zone3Strip = new GameObject("Zone3_BigVoiceStrip", typeof(RectTransform), typeof(Image));
            zone3Strip.transform.SetParent(bg.transform, false);
            RectTransform z3RT = zone3Strip.GetComponent<RectTransform>();
            z3RT.anchorMin = new Vector2(0.666f, 0f);
            z3RT.anchorMax = new Vector2(1f, 1f);
            z3RT.offsetMin = new Vector2(2, 4);
            z3RT.offsetMax = new Vector2(-4, -4);
            Image z3Img = zone3Strip.GetComponent<Image>();
            z3Img.sprite = roundedSp;
            z3Img.type = Image.Type.Sliced;
            z3Img.color = new Color(0.95f, 0.35f, 0.3f, 0.3f);

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
            Image handleImg = handle.GetComponent<Image>();
            handleImg.sprite = circleKnobSp;
            handleImg.color = new Color(0.15f, 0.72f, 0.32f);

            // Inner White Knob Cap
            GameObject knobCore = new GameObject("KnobCore", typeof(RectTransform), typeof(Image));
            knobCore.transform.SetParent(handle.transform, false);
            RectTransform kcRT = knobCore.GetComponent<RectTransform>();
            kcRT.sizeDelta = new Vector2(56, 56);
            Image kcImg = knobCore.GetComponent<Image>();
            kcImg.sprite = circleKnobSp;
            kcImg.color = Color.white;

            // Inner Grip Dot
            GameObject knobDot = new GameObject("KnobDot", typeof(RectTransform), typeof(Image));
            knobDot.transform.SetParent(knobCore.transform, false);
            RectTransform kdRT = knobDot.GetComponent<RectTransform>();
            kdRT.sizeDelta = new Vector2(20, 20);
            Image kdImg = knobDot.GetComponent<Image>();
            kdImg.sprite = circleKnobSp;
            kdImg.color = new Color(0.15f, 0.72f, 0.32f);

            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImg;

            // Zone Badge Box (Dynamic color feedback)
            GameObject badgeBox = CreateUIBox(volPhase.transform, "ZoneBadgeBox", "", 34, new Vector2(0, -125), new Vector2(620, 72), new Color(1f, 1f, 1f, 0.95f), Color.white);
            var zoneLabel = badgeBox.GetComponentInChildren<TextMeshProUGUI>();
            zoneLabel.text = "JUST RIGHT  (Polite & Clear)";
            zoneLabel.color = new Color(0.15f, 0.72f, 0.32f);

            // Say It Button (Large Emerald Green)
            GameObject sayBtn = CreateUIButton(volPhase.transform, "SayItButton", "SAY IT", 38, new Vector2(0, -230), new Vector2(380, 90), new Color(0.2f, 0.72f, 0.35f));

            // Phase 2: Leaving Container
            GameObject leavingPhase = new GameObject("LeavingPhaseContainer", typeof(RectTransform));
            leavingPhase.transform.SetParent(screenObj.transform, false);
            RectTransform lpRT = leavingPhase.GetComponent<RectTransform>();
            lpRT.anchorMin = Vector2.zero;
            lpRT.anchorMax = Vector2.one;
            lpRT.offsetMin = Vector2.zero;
            lpRT.offsetMax = Vector2.zero;

            CreateUIBox(leavingPhase.transform, "LeavePromptBox", "The meal is finished. Another family is waiting by the door.", 38, new Vector2(0, 360), new Vector2(1400, 95), new Color(1f, 1f, 1f, 0.95f), new Color(0.1f, 0.1f, 0.1f));

            Sprite waitingFamilySp = GetSprite(sprites, "Waiting Family");
            GameObject waitFamily = CreateUISpriteBox(leavingPhase.transform, "WaitingFamilyVisual", "Waiting Family", new Vector2(0, 120), new Vector2(480, 340), waitingFamilySp);

            GameObject btnLeave = CreateUIButton(leavingPhase.transform, "LeavePolitelyButton", "Say thank you and leave", 34, new Vector2(-360, -140), new Vector2(640, 100), new Color(0.25f, 0.68f, 0.38f));
            GameObject btnStay = CreateUIButton(leavingPhase.transform, "StayAndPlayButton", "Stay & play with spoons", 34, new Vector2(360, -140), new Vector2(640, 100), new Color(0.85f, 0.55f, 0.25f));
            leavingPhase.SetActive(false);

            // Feedback
            GameObject feedback = CreateUIBox(screenObj.transform, "FeedbackPanel", "Feedback text", 36, new Vector2(0, -360), new Vector2(1300, 100), new Color(1f, 0.95f, 0.7f, 0.96f), new Color(0.2f, 0.15f, 0.05f));
            var feedbackText = feedback.GetComponentInChildren<TextMeshProUGUI>();
            feedback.SetActive(false);

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

            // Confetti Overlay Banner across top
            Sprite confettiSp = GetSprite(sprites, "SPR_FX_Confetti");
            if (confettiSp != null)
            {
                CreateUISpriteBox(screenObj.transform, "ConfettiBanner", "", new Vector2(0, 220), new Vector2(1400, 480), confettiSp);
            }

            // Stars Row (Large Glowing Gold Stars)
            GameObject starsRow = new GameObject("StarsContainer", typeof(RectTransform));
            starsRow.transform.SetParent(screenObj.transform, false);
            RectTransform srRT = starsRow.GetComponent<RectTransform>();
            srRT.anchoredPosition = new Vector2(0, 310);
            srRT.sizeDelta = new Vector2(620, 140);

            Sprite goldStar = GetSprite(sprites, "SPR_Icon_GoldStar");
            GameObject s1 = CreateUISpriteBox(starsRow.transform, "Star1", "", new Vector2(-190, 0), new Vector2(140, 140), goldStar);
            GameObject s2 = CreateUISpriteBox(starsRow.transform, "Star2", "", new Vector2(0, 0), new Vector2(155, 155), goldStar);
            GameObject s3 = CreateUISpriteBox(starsRow.transform, "Star3", "", new Vector2(190, 0), new Vector2(140, 140), goldStar);

            // Characters Celebrating
            Sprite anuWave = GetSprite(sprites, "SPR_Anu_GoodbyeWave");
            Sprite raviSmile = GetSprite(sprites, "SPR_Ravi_WarmSmile");
            CreateUISpriteBox(screenObj.transform, "AnuWaveCelebration", "", new Vector2(-460, -20), new Vector2(280, 420), anuWave);
            CreateUISpriteBox(screenObj.transform, "RaviSmileCelebration", "", new Vector2(460, -20), new Vector2(280, 420), raviSmile);

            // High-Contrast Celebration Card (Deep Navy Card + Gold Text)
            GameObject bannerBox = CreateUIBox(screenObj.transform, "BannerCard", "THANK YOU, DO COME AGAIN!", 44, new Vector2(0, 120), new Vector2(1050, 85), new Color(0.12f, 0.18f, 0.32f, 0.95f), new Color(1f, 0.88f, 0.3f));
            var bannerText = bannerBox.GetComponentInChildren<TextMeshProUGUI>();

            // Reflection Panel (Clean White Rounded Card + Charcoal Text)
            GameObject refPanel = CreateUIBox(screenObj.transform, "ReflectionPanel", "Teacher Reflection:\nWhat will you say to the waiter next time?", 36, new Vector2(0, -130), new Vector2(1150, 130), new Color(1f, 1f, 1f, 0.96f), new Color(0.15f, 0.18f, 0.25f));
            var refText = refPanel.GetComponentInChildren<TextMeshProUGUI>();

            // Restart Button (Emerald Green Rounded Button)
            GameObject restartBtn = CreateUIButton(screenObj.transform, "RestartButton", "Play Again", 38, new Vector2(0, -320), new Vector2(360, 85), new Color(0.2f, 0.72f, 0.35f));

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
            rt.sizeDelta = new Vector2(320, 230);

            Sprite roundedSp = GetOrCreateRoundedBoxSprite();
            Image cardImg = cardObj.GetComponent<Image>();
            cardImg.sprite = roundedSp;
            cardImg.type = Image.Type.Sliced;
            cardImg.color = new Color(1f, 1f, 1f, 0.96f);

            // Icon Placeholder
            GameObject iconObj = new GameObject("DishIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(cardObj.transform, false);
            RectTransform iconRT = iconObj.GetComponent<RectTransform>();
            iconRT.sizeDelta = new Vector2(160, 115);
            iconRT.anchoredPosition = new Vector2(0, 35);
            iconObj.GetComponent<Image>().preserveAspect = true;

            // Name (Large Bold)
            var nameText = CreateTMPText(cardObj.transform, "DishName", "Dosa", 28, new Vector2(0, -40), new Vector2(280, 45), TextAlignmentOptions.Center, new Color(0.1f, 0.1f, 0.1f));

            // Price (Large Bold)
            var priceText = CreateTMPText(cardObj.transform, "PriceText", "Rs. 60", 26, new Vector2(0, -80), new Vector2(280, 40), TextAlignmentOptions.Center, new Color(0.85f, 0.45f, 0.1f));

            // Selection Highlight (Rounded)
            GameObject highlight = new GameObject("SelectionHighlight", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            highlight.transform.SetParent(cardObj.transform, false);
            RectTransform hlRT = highlight.GetComponent<RectTransform>();
            hlRT.anchorMin = Vector2.zero;
            hlRT.anchorMax = Vector2.one;
            hlRT.offsetMin = new Vector2(-6, -6);
            hlRT.offsetMax = new Vector2(6, 6);
            Image hlImg = highlight.GetComponent<Image>();
            hlImg.sprite = roundedSp;
            hlImg.type = Image.Type.Sliced;
            hlImg.color = new Color(0.15f, 0.75f, 1f, 0.45f);
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

        private static GameObject CreateUISpriteBox(Transform parent, string name, string fallbackText, Vector2 anchoredPos, Vector2 sizeDelta, Sprite sprite)
        {
            GameObject box = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            box.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(box);
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
                    CreateTMPText(box.transform, "Label", fallbackText, 26, Vector2.zero, sizeDelta, TextAlignmentOptions.Center, Color.black);
            }

            return box;
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
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = alignment;
            tmp.color = color;

            return tmp;
        }

        private static GameObject CreateUIBox(Transform parent, string name, string textContent, float fontSize, Vector2 anchoredPos, Vector2 sizeDelta, Color bgColor, Color textColor)
        {
            GameObject box = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            box.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(box);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(box);
            img.sprite = GetOrCreateRoundedBoxSprite();
            img.type = Image.Type.Sliced;
            img.color = bgColor;

            CreateTMPText(box.transform, "Label", textContent, fontSize, Vector2.zero, sizeDelta, TextAlignmentOptions.Center, textColor);
            return box;
        }

        private static GameObject CreateUIButton(Transform parent, string name, string labelText, float fontSize, Vector2 anchoredPos, Vector2 sizeDelta, Color btnColor)
        {
            GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(parent, false);

            RectTransform rt = GetOrAddComponent<RectTransform>(btnObj);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Image img = GetOrAddComponent<Image>(btnObj);
            img.sprite = GetOrCreateRoundedBoxSprite();
            img.type = Image.Type.Sliced;
            img.color = btnColor;

            CreateTMPText(btnObj.transform, "Label", labelText, fontSize, Vector2.zero, sizeDelta, TextAlignmentOptions.Center, Color.white);
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

            // 1. First register all genuine Sound Effects and Ambience from SFX folder
            if (System.IO.Directory.Exists(SFX_PATH))
            {
                string[] sfxGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { SFX_PATH });
                foreach (string guid in sfxGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string soundId = clip.name; // Exact ID e.g. SFX_Chirp, AMB_Restaurant, etc.
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

            // 2. Then register all Voice-Over character dialogue files from Audio folder
            if (System.IO.Directory.Exists(AUDIOS_PATH))
            {
                string[] voGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { AUDIOS_PATH });
                foreach (string guid in voGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string soundId = ResolveSoundId(clip.name);
                    // Only register if it's a VO or not already registered by genuine SFX
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

            return null; // Don't allow old speech prompts to be mapped as SFX
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
