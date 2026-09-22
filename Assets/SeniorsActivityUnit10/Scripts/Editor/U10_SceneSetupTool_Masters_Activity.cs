#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Googolplex.Unit10
{
    public class U10_SceneSetupTool_Masters_Activity : EditorWindow
    {
        private const string ART_PATH = "Assets/SeniorsActivityUnit10/Art";
        private const string AUDIO_PATH = "Assets/SeniorsActivityUnit10/Audio";
        private const string SFX_PATH = "Assets/SeniorsActivityUnit10/SFX";
        private const string SCENE_PATH = "Assets/SeniorsActivityUnit10/Scenes/SeniorsActivity_unit10.unity";

        [MenuItem("Googolplex/Unit 10/Configure Sprites and Audio")]
        public static void ConfigureAssets()
        {
            ReimportTexturesAsSprites();
            Debug.Log("<color=green>[U10_SceneSetupTool] Sprites and Audio configured successfully!</color>");
        }

        [MenuItem("Googolplex/Unit 10/Clear Save Data (Reset to Week 1)")]
        public static void ClearSaveData()
        {
            U10_SaveSystem.ClearSave();
            Debug.Log("<color=yellow>[U10_SaveSystem] Save data cleared successfully! Next time you press Play, the game will start fresh from Week 1 (Setup Screen).</color>");
        }

        [MenuItem("Googolplex/Unit 10/Refresh Setup Screen Only")]
        public static void RefreshSetupScreenOnly()
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[U10_SceneSetupTool] No Canvas found in active scene. Please open SeniorsActivity_unit10 scene.");
                return;
            }

            Transform setupScreenT = canvas.transform.Find("SetupScreen");
            if (setupScreenT == null)
            {
                Debug.LogError("[U10_SceneSetupTool] SetupScreen GameObject not found under Canvas.");
                return;
            }

            Dictionary<string, Sprite> spriteDict = LoadAllSprites();
            Sprite roundedBoxSprite = GetSprite(spriteDict, "UI_RoundedBox_9Slice");

            // Destroy all children of SetupScreen
            for (int i = setupScreenT.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(setupScreenT.GetChild(i).gameObject);
            }

            // Remove existing component if present
            var oldComp = setupScreenT.GetComponent<U10_SetupScreen_Masters_Activity>();
            if (oldComp != null)
            {
                Undo.DestroyObjectImmediate(oldComp);
            }

            // Rebuild SetupScreen in place
            SetupScreen0(setupScreenT.gameObject, spriteDict, roundedBoxSprite);

            // Re-wire to GameManager (setupScreen is a GameObject field)
            var gm = Object.FindObjectOfType<U10_GameManager_Masters_Activity>();
            if (gm != null)
            {
                SerializedObject so = new SerializedObject(gm);
                so.FindProperty("setupScreen").objectReferenceValue = setupScreenT.gameObject;
                so.ApplyModifiedProperties();
            }

            // Ensure SetupScreen GameObject is active
            setupScreenT.gameObject.SetActive(true);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[U10_SceneSetupTool] Setup Screen successfully refreshed in place with large 3D icons & bold text!</color>");
        }

        [MenuItem("Googolplex/Unit 10/Generate Complete Scene Hierarchy")]
        public static void GenerateCompleteScene()
        {
            // 1. Reimport textures
            ReimportTexturesAsSprites();

            // 2. Setup or create scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 3. Camera
            GameObject camObj = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            Camera cam = camObj.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.85f, 0.93f, 0.96f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;

            // 4. EventSystem
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            // 5. Canvas
            GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            Dictionary<string, Sprite> spriteDict = LoadAllSprites();
            Dictionary<string, AudioClip> audioDict = LoadAllAudio();
            Sprite roundedBoxSprite = GetSprite(spriteDict, "UI_RoundedBox_9Slice");

            // 6. Background (Full Stretch Anchor)
            SetupGardenBackground(canvasObj.transform, spriteDict);

            // 7. Screen 0: SetupScreen
            GameObject setupScreenObj = CreatePanel(canvasObj.transform, "SetupScreen", new Color(0.1f, 0.2f, 0.15f, 0.65f));
            SetupScreen0(setupScreenObj, spriteDict, roundedBoxSprite);

            // 8. Screen 1: GardenScreen
            GameObject gardenScreenObj = CreatePanel(canvasObj.transform, "GardenScreen", Color.clear);
            SetupScreen1(gardenScreenObj, spriteDict, roundedBoxSprite);

            // 9. Screen 2: WeeklyCheckScreen
            GameObject weeklyCheckScreenObj = CreatePanel(canvasObj.transform, "WeeklyCheckScreen", new Color(0.08f, 0.15f, 0.12f, 0.75f));
            SetupScreen2(weeklyCheckScreenObj, spriteDict, roundedBoxSprite);

            // 10. Screen 3: EndTermScreen
            GameObject endTermScreenObj = CreatePanel(canvasObj.transform, "EndTermScreen", new Color(0.06f, 0.1f, 0.14f, 0.85f));
            SetupScreen3(endTermScreenObj, spriteDict, roundedBoxSprite);

            // 11. Modal: GoldenLineModal
            GameObject goldenLineModalObj = CreatePanel(canvasObj.transform, "GoldenLineModal", new Color(0f, 0f, 0f, 0.75f));
            SetupGoldenLineModal(goldenLineModalObj, spriteDict, roundedBoxSprite);

            // 12. Root Game Manager & Audio Manager
            GameObject gmObj = new GameObject("[GameManager]");
            var gameMgr = gmObj.AddComponent<U10_GameManager_Masters_Activity>();
            var audioMgr = gmObj.AddComponent<U10_AudioManager_Masters_Activity>();

            WireGameManager(gameMgr, setupScreenObj, gardenScreenObj, weeklyCheckScreenObj, endTermScreenObj, goldenLineModalObj, spriteDict);
            WireAudioManager(audioMgr, audioDict);

            // Default visibility
            setupScreenObj.SetActive(true);
            gardenScreenObj.SetActive(false);
            weeklyCheckScreenObj.SetActive(false);
            endTermScreenObj.SetActive(false);
            goldenLineModalObj.SetActive(false);

            // Save Scene
            string sceneDir = Path.GetDirectoryName(SCENE_PATH);
            if (!Directory.Exists(sceneDir)) Directory.CreateDirectory(sceneDir);

            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            Debug.Log($"<color=green>[U10_SceneSetupTool] Successfully built and saved complete Unit 10 scene to: {SCENE_PATH}!</color>");
        }

        private static void ReimportTexturesAsSprites()
        {
            if (!Directory.Exists(ART_PATH)) return;

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            string[] pngFiles = Directory.GetFiles(ART_PATH, "*.png", SearchOption.AllDirectories);
            foreach (string file in pngFiles)
            {
                string assetPath = file.Replace('\\', '/');
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    bool changed = false;
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        changed = true;
                    }
                    if (assetPath.Contains("UI_RoundedBox_9Slice"))
                    {
                        importer.spriteBorder = new Vector4(32, 32, 32, 32);
                        changed = true;
                    }
                    if (changed)
                    {
                        importer.SaveAndReimport();
                    }
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static Dictionary<string, Sprite> LoadAllSprites()
        {
            Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { ART_PATH });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null && !dict.ContainsKey(s.name))
                {
                    dict[s.name] = s;
                }
            }
            return dict;
        }

        private static Dictionary<string, AudioClip> LoadAllAudio()
        {
            Dictionary<string, AudioClip> dict = new Dictionary<string, AudioClip>();
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { AUDIO_PATH, SFX_PATH });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null && !dict.ContainsKey(clip.name))
                {
                    dict[clip.name] = clip;
                }
            }
            return dict;
        }

        private static Sprite GetSprite(Dictionary<string, Sprite> dict, string name)
        {
            if (dict.TryGetValue(name, out Sprite s)) return s;
            return null;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color bgColor)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = panel.GetComponent<Image>();
            img.color = bgColor;
            return panel;
        }

        private static void SetupGardenBackground(Transform parent, Dictionary<string, Sprite> sprites)
        {
            GameObject bgObj = new GameObject("GardenBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bgObj.transform.SetParent(parent, false);
            bgObj.transform.SetAsFirstSibling();

            RectTransform rt = bgObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = bgObj.GetComponent<Image>();
            Sprite bgSprite = GetSprite(sprites, "BG_Garden_Smartboard");
            if (bgSprite != null)
            {
                img.sprite = bgSprite;
                img.color = Color.white;
            }
            else
            {
                img.color = new Color(0.85f, 0.94f, 0.88f);
            }
        }

        private static void SetupScreen0(GameObject screenObj, Dictionary<string, Sprite> sprites, Sprite roundedBox)
        {
            var comp = screenObj.AddComponent<U10_SetupScreen_Masters_Activity>();

            // Header Banner (Top-Center Anchor)
            GameObject headerBox = CreateUIBox(screenObj.transform, "HeaderBox", "Plant Your Class Garden\n<size=26><color=#0a220f>Class 3 and 4  |  Choose 4 Golden Habits for this term</color></size>", 40, new Vector2(0, -35), new Vector2(1240, 105), new Color(0.96f, 0.99f, 0.96f, 0.98f), new Color(0.05f, 0.18f, 0.08f), roundedBox, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            // Cards Grid Container (Center Anchor)
            GameObject gridObj = new GameObject("CardsGrid", typeof(RectTransform), typeof(GridLayoutGroup));
            gridObj.transform.SetParent(screenObj.transform, false);
            RectTransform gridRT = gridObj.GetComponent<RectTransform>();
            gridRT.anchorMin = new Vector2(0.5f, 0.5f);
            gridRT.anchorMax = new Vector2(0.5f, 0.5f);
            gridRT.pivot = new Vector2(0.5f, 0.5f);
            gridRT.sizeDelta = new Vector2(1600, 580);
            gridRT.anchoredPosition = new Vector2(0, 30);

            GridLayoutGroup glg = gridObj.GetComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(370, 265);
            glg.spacing = new Vector2(24, 20);
            glg.childAlignment = TextAnchor.MiddleCenter;

            List<Button> buttons = new List<Button>();
            List<Image> checkmarks = new List<Image>();
            List<TextMeshProUGUI> titles = new List<TextMeshProUGUI>();
            List<TextMeshProUGUI> quotes = new List<TextMeshProUGUI>();
            List<Image> icons = new List<Image>();

            var allHabits = U10_SaveSystem.ALL_AVAILABLE_HABITS;
            for (int i = 0; i < allHabits.Length; i++)
            {
                var h = allHabits[i];
                GameObject cardObj = new GameObject($"Card_{h.key}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                cardObj.transform.SetParent(gridObj.transform, false);

                Image cardImg = cardObj.GetComponent<Image>();
                cardImg.sprite = roundedBox;
                cardImg.type = Image.Type.Sliced;
                cardImg.color = Color.white;

                Button btn = cardObj.GetComponent<Button>();
                buttons.Add(btn);

                // Habit Icon (Top-Center of Card - Large Prominent 110x110 3D Badge)
                GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                iconObj.transform.SetParent(cardObj.transform, false);
                RectTransform iconRT = iconObj.GetComponent<RectTransform>();
                iconRT.anchorMin = new Vector2(0.5f, 1f);
                iconRT.anchorMax = new Vector2(0.5f, 1f);
                iconRT.pivot = new Vector2(0.5f, 1f);
                iconRT.anchoredPosition = new Vector2(0, -12);
                iconRT.sizeDelta = new Vector2(110, 110);
                Image iconImg = iconObj.GetComponent<Image>();
                Sprite hIcon = GetSprite(sprites, $"Icon_{h.key}");
                if (hIcon != null) iconImg.sprite = hIcon;
                iconImg.preserveAspect = true;
                iconImg.raycastTarget = false;
                icons.Add(iconImg);

                // Title Text (Below Icon - Bold & High Contrast 30pt)
                GameObject titleObj = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
                titleObj.transform.SetParent(cardObj.transform, false);
                RectTransform titleRT = titleObj.GetComponent<RectTransform>();
                titleRT.anchorMin = new Vector2(0.5f, 1f);
                titleRT.anchorMax = new Vector2(0.5f, 1f);
                titleRT.pivot = new Vector2(0.5f, 1f);
                titleRT.anchoredPosition = new Vector2(0, -128);
                titleRT.sizeDelta = new Vector2(350, 38);
                TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
                titleTMP.text = h.title;
                titleTMP.fontSize = 30;
                titleTMP.fontStyle = FontStyles.Bold;
                titleTMP.color = new Color(0.05f, 0.18f, 0.08f);
                titleTMP.alignment = TextAlignmentOptions.Center;
                titleTMP.raycastTarget = false;
                titles.Add(titleTMP);

                // Quote Text (Below Title - Crisp, Bold, Clean contrast 24pt)
                GameObject quoteObj = new GameObject("QuoteText", typeof(RectTransform), typeof(TextMeshProUGUI));
                quoteObj.transform.SetParent(cardObj.transform, false);
                RectTransform quoteRT = quoteObj.GetComponent<RectTransform>();
                quoteRT.anchorMin = new Vector2(0.5f, 1f);
                quoteRT.anchorMax = new Vector2(0.5f, 1f);
                quoteRT.pivot = new Vector2(0.5f, 1f);
                quoteRT.anchoredPosition = new Vector2(0, -170);
                quoteRT.sizeDelta = new Vector2(350, 85);
                TextMeshProUGUI quoteTMP = quoteObj.GetComponent<TextMeshProUGUI>();
                quoteTMP.text = h.quote;
                quoteTMP.fontSize = 24;
                quoteTMP.fontStyle = FontStyles.Bold;
                quoteTMP.color = new Color(0.12f, 0.26f, 0.15f);
                quoteTMP.alignment = TextAlignmentOptions.Center;
                quoteTMP.enableWordWrapping = true;
                quoteTMP.raycastTarget = false;
                quotes.Add(quoteTMP);

                // Checkmark Badge (Top-Right of Card)
                GameObject checkObj = new GameObject("CheckmarkBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                checkObj.transform.SetParent(cardObj.transform, false);
                RectTransform checkRT = checkObj.GetComponent<RectTransform>();
                checkRT.anchorMin = new Vector2(1f, 1f);
                checkRT.anchorMax = new Vector2(1f, 1f);
                checkRT.pivot = new Vector2(1f, 1f);
                checkRT.anchoredPosition = new Vector2(-10, -10);
                checkRT.sizeDelta = new Vector2(48, 48);
                Image checkImg = checkObj.GetComponent<Image>();
                checkImg.sprite = GetSprite(sprites, "Icon_Ribbon_Gold");
                checkImg.raycastTarget = false;
                checkObj.SetActive(false);
                checkmarks.Add(checkImg);
            }

            // Bottom Bar (Bottom-Center Anchor)
            GameObject bottomBar = CreateUIBox(screenObj.transform, "BottomBar", "", 32, new Vector2(0, 30), new Vector2(1240, 105), new Color(0.96f, 0.99f, 0.96f, 0.98f), Color.black, roundedBox, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));

            GameObject countObj = new GameObject("SelectionCountText", typeof(RectTransform), typeof(TextMeshProUGUI));
            countObj.transform.SetParent(bottomBar.transform, false);
            RectTransform countRT = countObj.GetComponent<RectTransform>();
            countRT.anchorMin = new Vector2(0f, 0.5f);
            countRT.anchorMax = new Vector2(0f, 0.5f);
            countRT.pivot = new Vector2(0f, 0.5f);
            countRT.anchoredPosition = new Vector2(40, 0);
            countRT.sizeDelta = new Vector2(500, 60);
            TextMeshProUGUI countTMP = countObj.GetComponent<TextMeshProUGUI>();
            countTMP.text = "Selected: 0 of 4 Habits";
            countTMP.fontSize = 36;
            countTMP.fontStyle = FontStyles.Bold;
            countTMP.color = new Color(0.05f, 0.18f, 0.08f);
            countTMP.alignment = TextAlignmentOptions.Left;
            countTMP.raycastTarget = false;

            GameObject plantBtnObj = CreateUIButton(bottomBar.transform, "PlantGardenBtn", "Plant Our Garden", 34, new Vector2(-40, 0), new Vector2(420, 78), new Color(0.16f, 0.62f, 0.28f), roundedBox, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
            Button plantBtn = plantBtnObj.GetComponent<Button>();

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("headerTitleText").objectReferenceValue = headerBox.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("selectionCountText").objectReferenceValue = countTMP;
            so.FindProperty("plantGardenButton").objectReferenceValue = plantBtn;
            so.FindProperty("cardsContainer").objectReferenceValue = gridObj.transform;

            SetObjectList(so.FindProperty("habitOptionButtons"), buttons);
            SetObjectList(so.FindProperty("habitCheckmarkIcons"), checkmarks);
            SetObjectList(so.FindProperty("habitTitleTexts"), titles);
            SetObjectList(so.FindProperty("habitQuoteTexts"), quotes);
            SetObjectList(so.FindProperty("habitIconImages"), icons);

            so.ApplyModifiedProperties();
        }

        private static void SetupScreen1(GameObject screenObj, Dictionary<string, Sprite> sprites, Sprite roundedBox)
        {
            var comp = screenObj.AddComponent<U10_GardenScreen_Masters_Activity>();

            // Header Banner (Top-Center Anchor)
            GameObject headerBox = CreateUIBox(screenObj.transform, "HeaderBanner", "The Golden Garden\n<size=26><color=#0a220f>Class 3 and 4  |  Living Etiquette Board</color></size>", 42, new Vector2(0, -30), new Vector2(860, 100), new Color(0.96f, 0.99f, 0.96f, 0.98f), new Color(0.05f, 0.18f, 0.08f), roundedBox, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
            TextMeshProUGUI titleTMP = headerBox.GetComponentInChildren<TextMeshProUGUI>();

            // Week Status Badge (Top-Right Anchor - Leaves Top-Left 100% CLEAR for host shell navigation)
            GameObject weekBadge = CreateUIBox(screenObj.transform, "WeekBadge", "Week 1 of 12", 34, new Vector2(-60, -30), new Vector2(280, 88), new Color(0.12f, 0.38f, 0.2f), Color.white, roundedBox, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            TextMeshProUGUI weekTMP = weekBadge.GetComponentInChildren<TextMeshProUGUI>();

            // 4 Pots Container (Center Anchor - 1180px wide for 4 full pots)
            GameObject potsRow = new GameObject("PotsRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            potsRow.transform.SetParent(screenObj.transform, false);
            RectTransform potsRT = potsRow.GetComponent<RectTransform>();
            potsRT.anchorMin = new Vector2(0.5f, 0.5f);
            potsRT.anchorMax = new Vector2(0.5f, 0.5f);
            potsRT.pivot = new Vector2(0.5f, 0.5f);
            potsRT.anchoredPosition = new Vector2(-170, -35);
            potsRT.sizeDelta = new Vector2(1180, 540);

            HorizontalLayoutGroup hlg = potsRow.GetComponent<HorizontalLayoutGroup>();
            hlg.spacing = 25;
            hlg.childAlignment = TextAnchor.LowerCenter;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            List<U10_PlantDisplayUI_Masters_Activity> plantDisplays = new List<U10_PlantDisplayUI_Masters_Activity>();
            Sprite potSp = GetSprite(sprites, "Pot_Terracotta");

            for (int i = 1; i <= 4; i++)
            {
                GameObject potObj = new GameObject($"PlantPot_{i}", typeof(RectTransform));
                potObj.transform.SetParent(potsRow.transform, false);
                RectTransform potRT = potObj.GetComponent<RectTransform>();
                potRT.sizeDelta = new Vector2(265, 520);

                var plantUI = potObj.AddComponent<U10_PlantDisplayUI_Masters_Activity>();
                plantDisplays.Add(plantUI);

                // Glow Aura
                GameObject glowObj = new GameObject("GlowAura", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                glowObj.transform.SetParent(potObj.transform, false);
                RectTransform glowRT = glowObj.GetComponent<RectTransform>();
                glowRT.anchoredPosition = new Vector2(0, 140);
                glowRT.sizeDelta = new Vector2(280, 280);
                Image glowImg = glowObj.GetComponent<Image>();
                glowImg.sprite = GetSprite(sprites, "UI_RoundedBox_9Slice");
                glowImg.color = new Color(1f, 0.9f, 0.3f, 0.4f);
                glowImg.raycastTarget = false;
                glowObj.SetActive(false);

                // Plant Sprite
                GameObject plantSpObj = new GameObject("PlantSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                plantSpObj.transform.SetParent(potObj.transform, false);
                RectTransform plantSpRT = plantSpObj.GetComponent<RectTransform>();
                plantSpRT.anchoredPosition = new Vector2(0, 110);
                plantSpRT.sizeDelta = new Vector2(240, 260);
                Image plantImg = plantSpObj.GetComponent<Image>();
                plantImg.sprite = GetSprite(sprites, "Plant_Water_1");
                plantImg.preserveAspect = true;
                plantImg.raycastTarget = false;

                // Pot Sprite (Classic Sturdy Terracotta Flower Pot)
                GameObject potImgObj = new GameObject("PotSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                potImgObj.transform.SetParent(potObj.transform, false);
                RectTransform potImgRT = potImgObj.GetComponent<RectTransform>();
                potImgRT.anchoredPosition = new Vector2(0, -60);
                potImgRT.sizeDelta = new Vector2(235, 175);
                Image potImg = potImgObj.GetComponent<Image>();
                potImg.sprite = potSp;
                potImg.preserveAspect = true;
                potImg.raycastTarget = false;

                // Title Badge
                GameObject titleCard = CreateUIBox(potObj.transform, "HabitBadge", $"Habit {i}", 28, new Vector2(0, -165), new Vector2(240, 48), new Color(0.96f, 0.99f, 0.96f, 0.98f), new Color(0.05f, 0.18f, 0.08f), roundedBox);
                TextMeshProUGUI plantTitleTMP = titleCard.GetComponentInChildren<TextMeshProUGUI>();

                // Stage Badge
                GameObject stageCard = CreateUIBox(potObj.transform, "StageBadge", "Stage 1 of 6", 24, new Vector2(0, -215), new Vector2(210, 40), new Color(0.12f, 0.45f, 0.22f), Color.white, roundedBox);
                TextMeshProUGUI stageTMP = stageCard.GetComponentInChildren<TextMeshProUGUI>();

                // Wire PlantDisplayUI
                SerializedObject pso = new SerializedObject(plantUI);
                pso.FindProperty("potImage").objectReferenceValue = potImg;
                pso.FindProperty("plantImage").objectReferenceValue = plantImg;
                pso.FindProperty("glowImage").objectReferenceValue = glowImg;
                pso.FindProperty("titleText").objectReferenceValue = plantTitleTMP;
                pso.FindProperty("stageBadgeText").objectReferenceValue = stageTMP;
                pso.ApplyModifiedProperties();
            }

            // Kindness Jar Display (Right-Center Anchor - Completely separate from 4 pots)
            GameObject jarBox = CreateUIBox(screenObj.transform, "KindnessJarCard", "", 24, new Vector2(-50, -35), new Vector2(300, 540), new Color(0.96f, 0.99f, 0.96f, 0.98f), Color.black, roundedBox, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));

            GameObject jarTitle = CreateUIBox(jarBox.transform, "JarTitle", "Kindness Jar\n<size=22><color=#0e3015>Daily Good Deeds</color></size>", 30, new Vector2(0, 215), new Vector2(280, 70), Color.clear, new Color(0.05f, 0.2f, 0.08f));

            GameObject jarImgObj = new GameObject("JarImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            jarImgObj.transform.SetParent(jarBox.transform, false);
            RectTransform jarRT = jarImgObj.GetComponent<RectTransform>();
            jarRT.anchoredPosition = new Vector2(0, 35);
            jarRT.sizeDelta = new Vector2(210, 260);
            Image jarImg = jarImgObj.GetComponent<Image>();
            jarImg.sprite = GetSprite(sprites, "Jar_Glass_Empty");
            jarImg.preserveAspect = true;
            jarImg.raycastTarget = false;

            GameObject marbleCountObj = CreateUIBox(jarBox.transform, "MarbleCountBox", "0 Marbles", 30, new Vector2(0, -135), new Vector2(250, 52), new Color(0.96f, 0.82f, 0.22f), new Color(0.25f, 0.15f, 0.02f), roundedBox);
            TextMeshProUGUI marbleTMP = marbleCountObj.GetComponentInChildren<TextMeshProUGUI>();

            GameObject addMarbleBtnObj = CreateUIButton(jarBox.transform, "AddMarbleBtn", "Add Kindness Marble", 26, new Vector2(0, -205), new Vector2(260, 68), new Color(0.85f, 0.52f, 0.12f), roundedBox);
            Button addMarbleBtn = addMarbleBtnObj.GetComponent<Button>();

            // Bottom Navigation Bar (Bottom-Center Anchor - 1240px wide leaves >340px margin on left for host game Back/Next navigation)
            GameObject navBar = CreateUIBox(screenObj.transform, "GardenNavBar", "", 24, new Vector2(0, 25), new Vector2(1240, 95), new Color(0.96f, 0.99f, 0.96f, 0.98f), Color.black, roundedBox, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));

            GameObject weeklyBtnObj = CreateUIButton(navBar.transform, "WeeklyCheckBtn", "Weekly Check (2 Min)", 32, new Vector2(-360, 0), new Vector2(420, 75), new Color(0.16f, 0.62f, 0.28f), roundedBox);
            Button weeklyBtn = weeklyBtnObj.GetComponent<Button>();

            GameObject quoteBtnObj = CreateUIButton(navBar.transform, "ShowQuoteBtn", "Golden Quote", 28, new Vector2(-40, 0), new Vector2(200, 75), new Color(0.18f, 0.48f, 0.72f), roundedBox);
            Button quoteBtn = quoteBtnObj.GetComponent<Button>();

            GameObject teacherBtnObj = CreateUIButton(navBar.transform, "TeacherMenuBtn", "Teacher Menu", 28, new Vector2(180, 0), new Vector2(220, 75), new Color(0.42f, 0.32f, 0.52f), roundedBox);
            Button teacherBtn = teacherBtnObj.GetComponent<Button>();

            GameObject endTermBtnObj = CreateUIButton(navBar.transform, "EndTermBtn", "Harvest Celebration", 28, new Vector2(450, 0), new Vector2(300, 75), new Color(0.85f, 0.62f, 0.12f), roundedBox);
            Button endTermBtn = endTermBtnObj.GetComponent<Button>();
            endTermBtnObj.SetActive(false);

            // Teacher Options Full-Screen Overlay Modal (Proper centered popup dialog with Close button)
            GameObject teacherOverlay = CreatePanel(screenObj.transform, "TeacherMenuPanel", new Color(0f, 0f, 0f, 0.65f));
            GameObject teacherCard = CreateUIBox(teacherOverlay.transform, "TeacherCard", "Teacher Options", 36, Vector2.zero, new Vector2(500, 360), new Color(0.98f, 0.99f, 0.98f, 0.98f), new Color(0.08f, 0.18f, 0.1f), roundedBox);
            
            GameObject nextWeekBtnObj = CreateUIButton(teacherCard.transform, "NextWeekBtn", "Advance to Next Week", 28, new Vector2(0, 50), new Vector2(400, 64), new Color(0.18f, 0.48f, 0.72f), roundedBox);
            Button nextWeekBtn = nextWeekBtnObj.GetComponent<Button>();

            GameObject resetTermBtnObj = CreateUIButton(teacherCard.transform, "ResetTermBtn", "Reset for New Term", 28, new Vector2(0, -25), new Vector2(400, 64), new Color(0.72f, 0.22f, 0.22f), roundedBox);
            Button resetTermBtn = resetTermBtnObj.GetComponent<Button>();

            GameObject closeTeacherBtnObj = CreateUIButton(teacherCard.transform, "CloseTeacherBtn", "Close Menu", 28, new Vector2(0, -100), new Vector2(400, 60), new Color(0.4f, 0.46f, 0.52f), roundedBox);
            Button closeTeacherBtn = closeTeacherBtnObj.GetComponent<Button>();

            teacherOverlay.SetActive(false);

            // Populate Jar Stages
            Sprite[] jarSprites = new Sprite[]
            {
                GetSprite(sprites, "Jar_Glass_Empty"),
                GetSprite(sprites, "Jar_Glass_Stage1"),
                GetSprite(sprites, "Jar_Glass_Stage2"),
                GetSprite(sprites, "Jar_Glass_Stage3"),
                GetSprite(sprites, "Jar_Glass_Full")
            };

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("titleText").objectReferenceValue = titleTMP;
            so.FindProperty("weekStatusText").objectReferenceValue = weekTMP;
            so.FindProperty("classNameText").objectReferenceValue = null;
            so.FindProperty("kindnessJarImage").objectReferenceValue = jarImg;
            so.FindProperty("marbleCountText").objectReferenceValue = marbleTMP;
            so.FindProperty("addMarbleButton").objectReferenceValue = addMarbleBtn;
            so.FindProperty("weeklyCheckButton").objectReferenceValue = weeklyBtn;
            so.FindProperty("endTermButton").objectReferenceValue = endTermBtn;
            so.FindProperty("showQuoteButton").objectReferenceValue = quoteBtn;
            so.FindProperty("teacherMenuButton").objectReferenceValue = teacherBtn;
            so.FindProperty("teacherMenuPanel").objectReferenceValue = teacherOverlay;
            so.FindProperty("nextWeekButton").objectReferenceValue = nextWeekBtn;
            so.FindProperty("resetTermButton").objectReferenceValue = resetTermBtn;
            so.FindProperty("closeTeacherMenuButton").objectReferenceValue = closeTeacherBtn;

            SetObjectList(so.FindProperty("plantDisplays"), plantDisplays);

            SerializedProperty jarArr = so.FindProperty("jarStageSprites");
            jarArr.arraySize = jarSprites.Length;
            for (int j = 0; j < jarSprites.Length; j++)
            {
                jarArr.GetArrayElementAtIndex(j).objectReferenceValue = jarSprites[j];
            }

            so.ApplyModifiedProperties();
        }

        private static void SetupScreen2(GameObject screenObj, Dictionary<string, Sprite> sprites, Sprite roundedBox)
        {
            var comp = screenObj.AddComponent<U10_WeeklyCheckScreen_Masters_Activity>();

            // Back to Garden Button (Top-Right Anchor - Leaves Top-Left 100% CLEAR for host shell navigation)
            GameObject backBtnObj = CreateUIButton(screenObj.transform, "BackToGardenBtn", "Return to Garden", 30, new Vector2(-50, -35), new Vector2(280, 80), new Color(0.32f, 0.42f, 0.48f), roundedBox, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));
            Button backBtn = backBtnObj.GetComponent<Button>();

            // Header Banner (Top-Center Anchor)
            GameObject headerBox = CreateUIBox(screenObj.transform, "WeeklyHeaderBox", "Weekly Check-in\n<size=26><color=#0e3015>Show of hands for our class habits</color></size>", 38, new Vector2(0, -35), new Vector2(860, 90), new Color(0.96f, 0.99f, 0.96f, 0.98f), new Color(0.05f, 0.18f, 0.08f), roundedBox, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
            TextMeshProUGUI weekTitleTMP = headerBox.GetComponentInChildren<TextMeshProUGUI>();

            // Habit Step Card (Center Anchor - 1120x670 nicely spaced)
            GameObject habitCard = CreateUIBox(screenObj.transform, "HabitStepCard", "", 32, new Vector2(0, -20), new Vector2(1120, 670), new Color(0.98f, 0.99f, 0.98f, 0.98f), Color.black, roundedBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            // Step Indicator Badge
            GameObject stepBadge = CreateUIBox(habitCard.transform, "StepIndicatorBadge", "Habit 1 of 4", 28, new Vector2(0, 280), new Vector2(280, 52), new Color(0.16f, 0.48f, 0.3f), Color.white, roundedBox);
            TextMeshProUGUI stepTMP = stepBadge.GetComponentInChildren<TextMeshProUGUI>();

            // Habit Icon
            GameObject iconObj = new GameObject("HabitIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(habitCard.transform, false);
            RectTransform iconRT = iconObj.GetComponent<RectTransform>();
            iconRT.anchoredPosition = new Vector2(0, 185);
            iconRT.sizeDelta = new Vector2(120, 120);
            Image habitIconImg = iconObj.GetComponent<Image>();
            habitIconImg.sprite = GetSprite(sprites, "Icon_Water");
            habitIconImg.preserveAspect = true;
            habitIconImg.raycastTarget = false;

            // Habit Title
            GameObject titleObj = CreateUIBox(habitCard.transform, "HabitTitleBox", "Drink Water", 46, new Vector2(0, 95), new Vector2(900, 58), Color.clear, new Color(0.05f, 0.18f, 0.08f));
            TextMeshProUGUI habitTitleTMP = titleObj.GetComponentInChildren<TextMeshProUGUI>();
            habitTitleTMP.fontStyle = FontStyles.Bold;

            // Habit Question Prompt
            GameObject promptObj = CreateUIBox(habitCard.transform, "HabitPromptBox", "Who drank fresh water every day this week?", 36, new Vector2(0, 25), new Vector2(1000, 75), Color.clear, new Color(0.08f, 0.25f, 0.12f));
            TextMeshProUGUI habitPromptTMP = promptObj.GetComponentInChildren<TextMeshProUGUI>();
            habitPromptTMP.fontStyle = FontStyles.Bold;

            // Habit Book Quote
            GameObject quoteObj = CreateUIBox(habitCard.transform, "HabitQuoteBox", "\"Drink your water -- inside and outside.\"", 30, new Vector2(0, -48), new Vector2(1000, 55), Color.clear, new Color(0.16f, 0.36f, 0.2f));
            TextMeshProUGUI habitQuoteTMP = quoteObj.GetComponentInChildren<TextMeshProUGUI>();
            habitQuoteTMP.fontStyle = FontStyles.Bold;

            // Status Feedback Text
            GameObject statusObj = CreateUIBox(habitCard.transform, "StatusFeedbackBox", "Raise your hand if you practiced this week!", 34, new Vector2(0, -118), new Vector2(980, 55), Color.clear, new Color(0.06f, 0.45f, 0.18f));
            TextMeshProUGUI statusTMP = statusObj.GetComponentInChildren<TextMeshProUGUI>();
            statusTMP.fontStyle = FontStyles.Bold;

            // Action Buttons Container
            GameObject handsBtnObj = CreateUIButton(habitCard.transform, "HandsRaisedBtn", "Yes, We Practiced!", 36, new Vector2(-225, -230), new Vector2(440, 86), new Color(0.16f, 0.62f, 0.28f), roundedBox);
            Button handsBtn = handsBtnObj.GetComponent<Button>();

            GameObject skipBtnObj = CreateUIButton(habitCard.transform, "SkipBtn", "Next Habit", 32, new Vector2(245, -230), new Vector2(320, 86), new Color(0.45f, 0.5f, 0.56f), roundedBox);
            Button skipBtn = skipBtnObj.GetComponent<Button>();

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("weekHeaderTitle").objectReferenceValue = weekTitleTMP;
            so.FindProperty("habitStepIndicatorText").objectReferenceValue = stepTMP;
            so.FindProperty("habitIconImage").objectReferenceValue = habitIconImg;
            so.FindProperty("habitTitleText").objectReferenceValue = habitTitleTMP;
            so.FindProperty("habitPromptText").objectReferenceValue = habitPromptTMP;
            so.FindProperty("habitBookQuoteText").objectReferenceValue = habitQuoteTMP;
            so.FindProperty("handsRaisedButton").objectReferenceValue = handsBtn;
            so.FindProperty("skipHabitButton").objectReferenceValue = skipBtn;
            so.FindProperty("statusFeedbackText").objectReferenceValue = statusTMP;
            so.FindProperty("backToGardenButton").objectReferenceValue = backBtn;

            List<Sprite> allIcons = new List<Sprite>();
            foreach (var kv in sprites)
            {
                if (kv.Key.StartsWith("Icon_") && !kv.Key.Contains("Ribbon"))
                {
                    allIcons.Add(kv.Value);
                }
            }
            SetObjectList(so.FindProperty("allHabitIcons"), allIcons);

            so.ApplyModifiedProperties();
        }

        private static void SetupScreen3(GameObject screenObj, Dictionary<string, Sprite> sprites, Sprite roundedBox)
        {
            var comp = screenObj.AddComponent<U10_EndTermScreen_Masters_Activity>();

            // Celebration Banner (Top-Center Anchor)
            GameObject headerBox = CreateUIBox(screenObj.transform, "EndTermHeaderBox", "Golden Garden Harvest Celebration!", 44, new Vector2(0, -35), new Vector2(1150, 92), new Color(0.96f, 0.99f, 0.96f, 0.98f), new Color(0.55f, 0.35f, 0.05f), roundedBox, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
            TextMeshProUGUI headerTMP = headerBox.GetComponentInChildren<TextMeshProUGUI>();

            // Certificate Parchment Box (Center Anchor)
            GameObject certBox = CreateUIBox(screenObj.transform, "CertificateBox", "", 24, new Vector2(0, 20), new Vector2(1220, 630), new Color(1f, 0.99f, 0.94f, 0.98f), Color.black, GetSprite(sprites, "Certificate_Gold_Border"), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            Image certImg = certBox.GetComponent<Image>();

            // Rosette Badge
            GameObject rosetteObj = new GameObject("RosetteBadge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            rosetteObj.transform.SetParent(certBox.transform, false);
            RectTransform rosRT = rosetteObj.GetComponent<RectTransform>();
            rosRT.anchoredPosition = new Vector2(0, 215);
            rosRT.sizeDelta = new Vector2(115, 115);
            Image rosImg = rosetteObj.GetComponent<Image>();
            rosImg.sprite = GetSprite(sprites, "Icon_Ribbon_Gold");
            rosImg.preserveAspect = true;
            rosImg.raycastTarget = false;

            // Certificate Title
            GameObject certTitleObj = CreateUIBox(certBox.transform, "CertTitle", "Certificate of Etiquette Excellence", 40, new Vector2(0, 135), new Vector2(1050, 60), Color.clear, new Color(0.55f, 0.35f, 0.05f));
            TextMeshProUGUI certTitleTMP = certTitleObj.GetComponentInChildren<TextMeshProUGUI>();
            certTitleTMP.fontStyle = FontStyles.Bold;

            // Class Name
            GameObject classObj = CreateUIBox(certBox.transform, "ClassNameText", "Awarded to Class 3 and 4", 36, new Vector2(0, 75), new Vector2(850, 50), Color.clear, new Color(0.08f, 0.25f, 0.12f));
            TextMeshProUGUI classTMP = classObj.GetComponentInChildren<TextMeshProUGUI>();
            classTMP.fontStyle = FontStyles.Bold;

            // The Three Es Quote
            GameObject quoteObj = CreateUIBox(certBox.transform, "ThreeEsQuoteText", "\"Excellence is not an act, but a habit. You have mastered The Three Es: Energy, Empathy, and Excellence.\"", 30, new Vector2(0, 5), new Vector2(1050, 75), Color.clear, new Color(0.12f, 0.28f, 0.15f));
            TextMeshProUGUI quoteTMP = quoteObj.GetComponentInChildren<TextMeshProUGUI>();
            quoteTMP.fontStyle = FontStyles.Bold;

            // Stats Summary
            GameObject statsObj = CreateUIBox(certBox.transform, "StatsSummaryText", "12 Weeks Completed   |   50+ Kindness Marbles Collected   |   4 Golden Blooms Harvested", 30, new Vector2(0, -75), new Vector2(1100, 58), new Color(0.96f, 0.88f, 0.45f, 0.95f), new Color(0.3f, 0.18f, 0.02f), roundedBox);
            TextMeshProUGUI statsTMP = statsObj.GetComponentInChildren<TextMeshProUGUI>();
            statsTMP.fontStyle = FontStyles.Bold;

            // Bottom Action Buttons (Bottom-Center Anchor)
            GameObject returnBtnObj = CreateUIButton(screenObj.transform, "ReturnToGardenBtn", "Return to Garden", 32, new Vector2(-260, 35), new Vector2(380, 80), new Color(0.18f, 0.48f, 0.72f), roundedBox, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            Button returnBtn = returnBtnObj.GetComponent<Button>();

            GameObject resetBtnObj = CreateUIButton(screenObj.transform, "ResetNewTermBtn", "Start New Term", 32, new Vector2(260, 35), new Vector2(380, 80), new Color(0.16f, 0.62f, 0.28f), roundedBox, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            Button resetBtn = resetBtnObj.GetComponent<Button>();

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("celebrationHeader").objectReferenceValue = headerTMP;
            so.FindProperty("certificateTitle").objectReferenceValue = certTitleTMP;
            so.FindProperty("classNameText").objectReferenceValue = classTMP;
            so.FindProperty("threeEsQuoteText").objectReferenceValue = quoteTMP;
            so.FindProperty("statsSummaryText").objectReferenceValue = statsTMP;
            so.FindProperty("returnToGardenButton").objectReferenceValue = returnBtn;
            so.FindProperty("resetForNewTermButton").objectReferenceValue = resetBtn;
            so.FindProperty("certificateBackground").objectReferenceValue = certImg;
            so.ApplyModifiedProperties();
        }

        private static void SetupGoldenLineModal(GameObject screenObj, Dictionary<string, Sprite> sprites, Sprite roundedBox)
        {
            var comp = screenObj.AddComponent<U10_GoldenLineModal_Masters_Activity>();

            // Center Parchment Popup Card (Center Anchor)
            GameObject cardObj = CreateUIBox(screenObj.transform, "ModalCard", "", 24, Vector2.zero, new Vector2(1080, 580), new Color(1f, 0.99f, 0.94f, 0.98f), Color.black, GetSprite(sprites, "Certificate_Gold_Border"), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            // Rosette Icon
            GameObject rosObj = new GameObject("ModalRosette", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            rosObj.transform.SetParent(cardObj.transform, false);
            RectTransform rosRT = rosObj.GetComponent<RectTransform>();
            rosRT.anchoredPosition = new Vector2(0, 195);
            rosRT.sizeDelta = new Vector2(110, 110);
            Image rosImg = rosObj.GetComponent<Image>();
            rosImg.sprite = GetSprite(sprites, "Icon_Ribbon_Gold");
            rosImg.preserveAspect = true;
            rosImg.raycastTarget = false;

            // Header Text
            GameObject headerObj = CreateUIBox(cardObj.transform, "ModalHeader", "Golden Quote of the Week", 42, new Vector2(0, 110), new Vector2(900, 55), Color.clear, new Color(0.6f, 0.4f, 0.05f));
            TextMeshProUGUI headerTMP = headerObj.GetComponentInChildren<TextMeshProUGUI>();
            headerTMP.fontStyle = FontStyles.Bold;

            // Quote Text (Spacious, bold, and clear)
            GameObject quoteObj = CreateUIBox(cardObj.transform, "ModalQuote", "\"The Golden Life begins with the simple things you do every day.\"", 40, new Vector2(0, -10), new Vector2(960, 150), Color.clear, new Color(0.05f, 0.2f, 0.08f));
            TextMeshProUGUI quoteTMP = quoteObj.GetComponentInChildren<TextMeshProUGUI>();
            quoteTMP.fontStyle = FontStyles.Bold;

            // Close / Continue Button
            GameObject closeBtnObj = CreateUIButton(cardObj.transform, "ModalCloseBtn", "Continue", 36, new Vector2(0, -185), new Vector2(320, 78), new Color(0.16f, 0.62f, 0.28f), roundedBox);
            Button closeBtn = closeBtnObj.GetComponent<Button>();

            // Wire SerializedObject
            SerializedObject so = new SerializedObject(comp);
            so.FindProperty("modalPanel").objectReferenceValue = screenObj;
            so.FindProperty("headerText").objectReferenceValue = headerTMP;
            so.FindProperty("quoteText").objectReferenceValue = quoteTMP;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.ApplyModifiedProperties();
        }

        private static void WireGameManager(U10_GameManager_Masters_Activity gm, GameObject s0, GameObject s1, GameObject s2, GameObject s3, GameObject modal, Dictionary<string, Sprite> sprites)
        {
            SerializedObject so = new SerializedObject(gm);
            so.FindProperty("setupScreen").objectReferenceValue = s0;
            so.FindProperty("gardenScreen").objectReferenceValue = s1;
            so.FindProperty("weeklyCheckScreen").objectReferenceValue = s2;
            so.FindProperty("endTermScreen").objectReferenceValue = s3;
            so.FindProperty("goldenLineModal").objectReferenceValue = modal;
            so.FindProperty("potSprite").objectReferenceValue = GetSprite(sprites, "Pot_Terracotta");

            List<Sprite> allPlants = new List<Sprite>();
            foreach (var kv in sprites)
            {
                if (kv.Key.StartsWith("Plant_"))
                {
                    allPlants.Add(kv.Value);
                }
            }
            SetObjectList(so.FindProperty("allPlantSprites"), allPlants);

            so.ApplyModifiedProperties();
        }

        private static void WireAudioManager(U10_AudioManager_Masters_Activity am, Dictionary<string, AudioClip> audios)
        {
            SerializedObject so = new SerializedObject(am);
            SetClipProp(so, "sfxPlantGrow", audios, "SFX_PlantGrow");
            SetClipProp(so, "sfxFlower", audios, "SFX_Flower");
            SetClipProp(so, "sfxMarble", audios, "SFX_Marble");
            SetClipProp(so, "sfxJarFull", audios, "SFX_JarFull");
            SetClipProp(so, "sfxGoldenLine", audios, "SFX_GoldenLine");
            SetClipProp(so, "sfxTally", audios, "SFX_Tally");
            SetClipProp(so, "sfxTap", audios, "SFX_Tap");
            SetClipProp(so, "ambGarden", audios, "AMB_Garden");
            SetClipProp(so, "musGarden", audios, "MUS_Garden");
            SetClipProp(so, "musEndTerm", audios, "MUS_EndTerm");

            List<AudioClip> allClipsList = new List<AudioClip>(audios.Values);
            SetObjectList(so.FindProperty("allAudioClips"), allClipsList);

            so.ApplyModifiedProperties();
        }

        private static void SetClipProp(SerializedObject so, string propName, Dictionary<string, AudioClip> audios, string clipKey)
        {
            var prop = so.FindProperty(propName);
            if (prop != null && audios.TryGetValue(clipKey, out AudioClip clip))
            {
                prop.objectReferenceValue = clip;
            }
        }

        private static GameObject CreateUIBox(Transform parent, string name, string text, int fontSize, Vector2 pos, Vector2 size, Color bgColor, Color textColor, Sprite boxSprite = null, Vector2? anchorMin = null, Vector2? anchorMax = null, Vector2? pivot = null)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            obj.transform.SetParent(parent, false);

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = obj.GetComponent<Image>();
            if (boxSprite != null)
            {
                img.sprite = boxSprite;
                img.type = Image.Type.Sliced;
            }
            img.color = bgColor;
            img.raycastTarget = false;

            if (!string.IsNullOrEmpty(text))
            {
                GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
                txtObj.transform.SetParent(obj.transform, false);

                RectTransform txtRT = txtObj.GetComponent<RectTransform>();
                txtRT.anchorMin = Vector2.zero;
                txtRT.anchorMax = Vector2.one;
                txtRT.pivot = new Vector2(0.5f, 0.5f);
                txtRT.offsetMin = new Vector2(15, 8);
                txtRT.offsetMax = new Vector2(-15, -8);

                TextMeshProUGUI tmp = txtObj.GetComponent<TextMeshProUGUI>();
                tmp.text = text;
                tmp.fontSize = fontSize;
                tmp.fontStyle = FontStyles.Bold;
                tmp.color = textColor;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.enableWordWrapping = true;
                tmp.raycastTarget = false;
            }

            return obj;
        }

        private static GameObject CreateUIButton(Transform parent, string name, string text, int fontSize, Vector2 pos, Vector2 size, Color btnColor, Sprite boxSprite = null, Vector2? anchorMin = null, Vector2? anchorMax = null, Vector2? pivot = null)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            obj.transform.SetParent(parent, false);

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin ?? new Vector2(0.5f, 0.5f);
            rt.anchorMax = anchorMax ?? new Vector2(0.5f, 0.5f);
            rt.pivot = pivot ?? new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = obj.GetComponent<Image>();
            if (boxSprite != null)
            {
                img.sprite = boxSprite;
                img.type = Image.Type.Sliced;
            }
            img.color = btnColor;

            Button btn = obj.GetComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(btnColor.r * 1.15f, btnColor.g * 1.15f, btnColor.b * 1.15f, 1f);
            colors.pressedColor = new Color(btnColor.r * 0.85f, btnColor.g * 0.85f, btnColor.b * 0.85f, 1f);
            btn.colors = colors;

            GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            txtObj.transform.SetParent(obj.transform, false);

            RectTransform txtRT = txtObj.GetComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.pivot = new Vector2(0.5f, 0.5f);
            txtRT.offsetMin = Vector2.zero;
            txtRT.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = txtObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;

            return obj;
        }

        private static void SetObjectList<T>(SerializedProperty listProp, List<T> items) where T : UnityEngine.Object
        {
            if (listProp == null) return;
            listProp.ClearArray();
            listProp.arraySize = items.Count;
            for (int i = 0; i < items.Count; i++)
            {
                listProp.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
            }
        }
    }
}
#endif
