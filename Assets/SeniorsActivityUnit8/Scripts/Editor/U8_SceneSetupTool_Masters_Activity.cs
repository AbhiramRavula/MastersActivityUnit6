#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Googolplex.Unit8
{
    public static class U8_SceneSetupTool_Masters_Activity
    {
        public const string ScenePath = "Assets/SeniorsActivityUnit8/Scenes/SeniorsActivity_unit8.unity";
        private const string ArtPath = "Assets/SeniorsActivityUnit8/Art";

        [MenuItem("Googolplex/Unit 8/Setup Complete Unit 8 Scene", false, 10)]
        public static void SetupCompleteUnit8Scene()
        {
            // Ensure target scene directory exists
            string sceneDir = "Assets/SeniorsActivityUnit8/Scenes";
            if (!Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
                AssetDatabase.Refresh();
            }

            EnsureEventSystem();
            EnsureCamera();

            GameObject canvasGo = EnsureCanvas();
            RectTransform canvasRect = canvasGo.GetComponent<RectTransform>();

            // Setup Managers
            GameObject managersGo = GameObject.Find("--- MANAGERS ---");
            if (managersGo == null) managersGo = new GameObject("--- MANAGERS ---");

            var gm = managersGo.GetComponent<U8_GameManager_Masters_Activity>();
            if (gm == null) gm = managersGo.AddComponent<U8_GameManager_Masters_Activity>();

            var am = managersGo.GetComponent<U8_AudioManager_Masters_Activity>();
            if (am == null) am = managersGo.AddComponent<U8_AudioManager_Masters_Activity>();

            // Setup Screens
            GameObject p1 = CreateOrGetScreen(canvasRect, "U8_Screen_01_Part1Door", typeof(U8_Part1_DoorController_Masters_Activity));
            GameObject p2 = CreateOrGetScreen(canvasRect, "U8_Screen_02_Part2Inside", typeof(U8_Part2_WashroomInsideController_Masters_Activity));
            GameObject hw = CreateOrGetScreen(canvasRect, "U8_Screen_03_Handwash", typeof(U8_HandwashScreenController_Masters_Activity));
            GameObject p3 = CreateOrGetScreen(canvasRect, "U8_Screen_04_Part3AfterYou", typeof(U8_Part3_AfterYouController_Masters_Activity));
            GameObject p4 = CreateOrGetScreen(canvasRect, "U8_Screen_05_Part4EmptySoap", typeof(U8_Part4_EmptySoapController_Masters_Activity));
            GameObject end = CreateOrGetScreen(canvasRect, "U8_Screen_06_Ending", typeof(U8_EndingScreen_Masters_Activity));

            SetupPart1UI(p1);
            SetupPart2UI(p2);
            SetupHandwashUI(hw);
            SetupPart3UI(p3);
            SetupPart4UI(p4);
            SetupEndingUI(end);

            p1.SetActive(true);
            p2.SetActive(false);
            hw.SetActive(false);
            p3.SetActive(false);
            p4.SetActive(false);
            end.SetActive(false);

            // Mark active scene dirty & save
            Scene currentScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);
            if (string.IsNullOrEmpty(currentScene.path))
            {
                EditorSceneManager.SaveScene(currentScene, ScenePath);
            }

            EditorUtility.DisplayDialog("Unit 8 Scene Setup", 
                $"Unit 8: Washroom Etiquette scene hierarchy created successfully with all GameObjects and UI elements!\n\nTarget scene location:\n{ScenePath}", "OK");
            Debug.Log($"[U8_SceneSetupTool] Completed full Unit 8 scene generation. Target: {ScenePath}");
        }

        private static Sprite LoadSprite(string filename)
        {
            string path = $"{ArtPath}/{filename}";
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            }
        }

        private static void EnsureCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                cam.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }
            cam.orthographic = true;
            cam.backgroundColor = new Color(0.12f, 0.16f, 0.22f);
        }

        private static GameObject EnsureCanvas()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGo = new GameObject("Unit8_Canvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                var scaler = canvasGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                canvasGo.AddComponent<GraphicRaycaster>();
                return canvasGo;
            }
            return canvas.gameObject;
        }

        private static GameObject CreateOrGetScreen(RectTransform parent, string screenName, System.Type controllerType)
        {
            Transform t = parent.Find(screenName);
            GameObject screenGo;
            if (t == null)
            {
                screenGo = new GameObject(screenName, typeof(RectTransform));
                screenGo.transform.SetParent(parent, false);
                RectTransform rt = screenGo.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            else
            {
                screenGo = t.gameObject;
            }

            if (controllerType != null && screenGo.GetComponent(controllerType) == null)
            {
                screenGo.AddComponent(controllerType);
            }

            return screenGo;
        }

        private static void SetupPart1UI(GameObject screen)
        {
            CreateHeader(screen, "UNIT 8: WASHROOM ETIQUETTE", "Part 1: The Door");

            // Door Image
            CreateImage(screen, "WashroomDoor", new Vector2(-150, 0), new Vector2(260, 420), LoadSprite("SPR_Cubicle_Closed.svg"));
            // Anu Avatar
            CreateImage(screen, "AnuAvatar", new Vector2(200, -50), new Vector2(200, 320), LoadSprite("SPR_Anu_Normal.svg"));

            // Prompts
            CreateText(screen, "PromptText", "The door is closed. What should Anu do?", new Vector2(0, 320), new Vector2(900, 70), 32);
            CreateText(screen, "FeedbackText", "", new Vector2(0, -320), new Vector2(900, 60), 26, Color.yellow);

            // Choice Buttons
            GameObject container = CreateContainer(screen, "ChoiceContainer", new Vector2(0, -200), new Vector2(800, 120));
            CreateButton(container, "Btn_Knock", "Knock and Wait", new Vector2(-260, 0), new Vector2(240, 70), new Color(0.18f, 0.65f, 0.35f));
            CreateButton(container, "Btn_Bang", "Bang Loudly", new Vector2(0, 0), new Vector2(240, 70), new Color(0.85f, 0.35f, 0.25f));
            CreateButton(container, "Btn_Push", "Push Open", new Vector2(260, 0), new Vector2(240, 70), new Color(0.75f, 0.55f, 0.2f));
        }

        private static void SetupPart2UI(GameObject screen)
        {
            CreateHeader(screen, "UNIT 8: WASHROOM ETIQUETTE", "Part 2: Inside the Washroom");

            // 1. Cubicle Section (Left)
            GameObject cubicle = CreateContainer(screen, "Cubicle", new Vector2(-550, 0), new Vector2(300, 500));
            CreateImage(cubicle, "CubicleDoorImage", new Vector2(0, 40), new Vector2(240, 380), LoadSprite("SPR_Cubicle_Open.svg"));
            CreateButton(cubicle, "Btn_CloseDoor", "1. Close Door", new Vector2(-70, -180), new Vector2(130, 50), new Color(0.2f, 0.6f, 0.9f));
            CreateButton(cubicle, "Btn_Flush", "2. Flush", new Vector2(70, -180), new Vector2(130, 50), new Color(0.2f, 0.7f, 0.4f));
            CreateButton(cubicle, "Btn_ComeOut", "3. Step Out", new Vector2(0, -240), new Vector2(200, 45), new Color(0.5f, 0.4f, 0.8f));

            // 2. Sink Section (Center)
            GameObject sink = CreateContainer(screen, "Sink", new Vector2(0, 0), new Vector2(360, 500));
            CreateImage(sink, "SinkImage", new Vector2(0, 0), new Vector2(320, 220), LoadSprite("SPR_Sink_Splashed.svg"));
            CreateImage(sink, "TapWaterStream", new Vector2(0, -30), new Vector2(50, 100), LoadSprite("SPR_WaterStream.svg"));
            CreateButton(sink, "Btn_WashHands", "4. Wash Hands", new Vector2(0, -160), new Vector2(200, 50), new Color(0.2f, 0.7f, 0.8f));
            CreateButton(sink, "Btn_TurnTapOff", "5. Turn Tap Off", new Vector2(0, -220), new Vector2(200, 50), new Color(0.8f, 0.5f, 0.2f));
            CreateButton(sink, "Btn_WipeSink", "7. Wipe Sink", new Vector2(0, -280), new Vector2(200, 50), new Color(0.2f, 0.8f, 0.4f));

            // 3. Towel & Bin (Right)
            GameObject towelHolder = CreateContainer(screen, "TowelHolder", new Vector2(520, 100), new Vector2(200, 250));
            CreateImage(towelHolder, "TowelHolderImage", Vector2.zero, new Vector2(140, 180), LoadSprite("SPR_Towel_Holder.svg"));
            CreateButton(towelHolder, "Btn_Towel", "6. Paper Towel", new Vector2(0, -110), new Vector2(160, 45), new Color(0.9f, 0.65f, 0.2f));

            CreateImage(screen, "TrashBinImage", new Vector2(520, -180), new Vector2(130, 180), LoadSprite("SPR_TrashBin.svg"));
            CreateImage(screen, "TowelInAir", new Vector2(400, -50), new Vector2(60, 40), null);
            CreateImage(screen, "SoggyTowelOnFloor", new Vector2(450, -240), new Vector2(120, 70), LoadSprite("SPR_Towel_SoggyFloor.svg"));

            // Anu Avatar
            CreateImage(screen, "AnuInsideAvatar", new Vector2(-220, -60), new Vector2(180, 280), LoadSprite("SPR_Anu_Normal.svg"));

            // Prompts
            CreateText(screen, "PromptText", "Step 1: Close the cubicle door first!", new Vector2(0, 320), new Vector2(900, 60), 30);
            CreateText(screen, "FeedbackText", "", new Vector2(0, -350), new Vector2(900, 50), 24, Color.yellow);

            CreateButton(screen, "Btn_ProceedToPart3", "Next: See What Meera Finds ➔", new Vector2(0, -320), new Vector2(340, 60), new Color(0.18f, 0.65f, 0.35f));
        }

        private static void SetupHandwashUI(GameObject screen)
        {
            CreateHeader(screen, "UNIT 8: HANDWASHING", "20-Second Song & Scrub");

            // Hands Image
            CreateImage(screen, "HandsImage", new Vector2(0, 0), new Vector2(420, 320), LoadSprite("SPR_Hands_Soapy.svg"));

            // Large Touch Scrub Target
            CreateButton(screen, "ScrubAreaButton", "TAP / RUB TO SCRUB!", new Vector2(0, -220), new Vector2(320, 65), new Color(0.2f, 0.7f, 0.9f));

            // Bubbles Container
            GameObject bubbles = CreateContainer(screen, "Bubbles", Vector2.zero, new Vector2(400, 300));
            CreateImage(bubbles, "BubbleCluster", Vector2.zero, new Vector2(220, 220), LoadSprite("SPR_Bubbles.svg"));

            // 4 Germ Blobs
            GameObject germs = CreateContainer(screen, "Germs", Vector2.zero, new Vector2(400, 300));
            CreateImage(germs, "Germ_1", new Vector2(-120, 60), new Vector2(90, 90), LoadSprite("SPR_Germ_1.svg"));
            CreateImage(germs, "Germ_2", new Vector2(120, 60), new Vector2(90, 90), LoadSprite("SPR_Germ_2.svg"));
            CreateImage(germs, "Germ_3", new Vector2(-80, -70), new Vector2(90, 90), LoadSprite("SPR_Germ_3.svg"));
            CreateImage(germs, "Germ_4", new Vector2(80, -70), new Vector2(90, 90), LoadSprite("SPR_Germ_4.svg"));

            // Timer & Prompt
            CreateText(screen, "PromptText", "Keep tapping to scrub hands with soap!", new Vector2(0, 310), new Vector2(900, 60), 30);
            CreateText(screen, "TimerText", "20s", new Vector2(0, 240), new Vector2(200, 50), 36, Color.cyan);
        }

        private static void SetupPart3UI(GameObject screen)
        {
            CreateHeader(screen, "UNIT 8: AFTER YOU", "Part 3: What Meera Finds");

            CreateImage(screen, "WetFloorPuddleObject", new Vector2(0, -180), new Vector2(320, 120), LoadSprite("SPR_WetFloorPuddle.svg"));
            CreateImage(screen, "SplashedSinkOverlay", new Vector2(-200, 20), new Vector2(280, 180), LoadSprite("SPR_Sink_Splashed.svg"));
            CreateImage(screen, "MeeraAvatar", new Vector2(100, -30), new Vector2(260, 320), LoadSprite("SPR_Meera_Slip.svg"));

            CreateText(screen, "PromptText", "Here comes Meera!", new Vector2(0, 310), new Vector2(900, 60), 32);
            CreateText(screen, "FeedbackText", "", new Vector2(0, -280), new Vector2(900, 50), 24, Color.yellow);

            CreateButton(screen, "Btn_TryAgain", "↺ TRY AGAIN", new Vector2(-160, -340), new Vector2(240, 65), new Color(0.85f, 0.4f, 0.2f));
            CreateButton(screen, "Btn_ProceedToPart4", "Next: The Soap! ➔", new Vector2(160, -340), new Vector2(240, 65), new Color(0.18f, 0.65f, 0.35f));
        }

        private static void SetupPart4UI(GameObject screen)
        {
            CreateHeader(screen, "UNIT 8: EMPTY SOAP!", "Part 4: What Should Meera Do?");

            CreateImage(screen, "SoapDispenserImage", new Vector2(-150, 0), new Vector2(160, 240), LoadSprite("SPR_Soap_Empty.svg"));
            CreateImage(screen, "MeeraAvatar", new Vector2(160, -40), new Vector2(200, 300), LoadSprite("SPR_Meera_Happy.svg"));

            CreateText(screen, "PromptText", "The soap is finished. What should Meera do?", new Vector2(0, 310), new Vector2(900, 60), 30);
            CreateText(screen, "FeedbackText", "", new Vector2(0, -280), new Vector2(900, 50), 24, Color.yellow);

            GameObject container = CreateContainer(screen, "ChoiceContainer", new Vector2(0, -180), new Vector2(700, 100));
            CreateButton(container, "Btn_TellTeacher", "Go and Tell a Teacher", new Vector2(-180, 0), new Vector2(280, 65), new Color(0.18f, 0.65f, 0.35f));
            CreateButton(container, "Btn_JustLeave", "Just Leave", new Vector2(180, 0), new Vector2(280, 65), new Color(0.75f, 0.35f, 0.25f));

            CreateButton(screen, "Btn_Finish", "Finish Unit ➔", new Vector2(0, -340), new Vector2(240, 60), new Color(0.2f, 0.6f, 0.9f));
        }

        private static void SetupEndingUI(GameObject screen)
        {
            CreateHeader(screen, "UNIT 8: SUMMARY", "Ready for the Next Person!");

            CreateText(screen, "TitleText", "READY FOR THE NEXT PERSON!", new Vector2(0, 260), new Vector2(900, 70), 40, Color.yellow);
            CreateText(screen, "DiscussionQuestionText", "Would the next person be happy?", new Vector2(0, 10), new Vector2(1100, 100), 46, new Color(0.4f, 0.9f, 1f));

            // Stars Container
            GameObject stars = CreateContainer(screen, "StarsContainer", new Vector2(0, 140), new Vector2(500, 120));
            CreateText(stars, "Star_1", "★", new Vector2(-120, 0), new Vector2(90, 90), 72, Color.yellow);
            CreateText(stars, "Star_2", "★", new Vector2(0, 0), new Vector2(90, 90), 72, Color.yellow);
            CreateText(stars, "Star_3", "★", new Vector2(120, 0), new Vector2(90, 90), 72, Color.yellow);

            CreateButton(screen, "Btn_Restart", "Play Again ↺", new Vector2(0, -280), new Vector2(260, 65), new Color(0.2f, 0.7f, 0.4f));
        }

        private static GameObject CreateContainer(GameObject parent, string name, Vector2 pos, Vector2 size)
        {
            Transform existing = parent.transform.Find(name);
            if (existing != null) return existing.gameObject;

            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent.transform, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            return go;
        }

        private static Image CreateImage(GameObject parent, string name, Vector2 pos, Vector2 size, Sprite sprite)
        {
            Transform existing = parent.transform.Find(name);
            GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent.transform, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.GetComponent<Image>();
            if (sprite != null)
            {
                img.sprite = sprite;
                img.color = Color.white;
            }
            else
            {
                img.color = new Color(1f, 1f, 1f, 0.2f);
            }
            return img;
        }

        private static Button CreateButton(GameObject parent, string name, string label, Vector2 pos, Vector2 size, Color btnColor)
        {
            Transform existing = parent.transform.Find(name);
            GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent.transform, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.GetComponent<Image>();
            img.color = btnColor;

            Button btn = go.GetComponent<Button>();

            // Button label
            Transform textTrans = go.transform.Find("Label");
            GameObject textGo = textTrans != null ? textTrans.gameObject : new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(go.transform, false);
            RectTransform textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 22;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return btn;
        }

        private static TextMeshProUGUI CreateText(GameObject parent, string name, string content, Vector2 pos, Vector2 size, float fontSize, Color? color = null)
        {
            Transform existing = parent.transform.Find(name);
            GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent.transform, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = color ?? Color.white;
            tmp.enableWordWrapping = true;
            return tmp;
        }

        private static void CreateHeader(GameObject screen, string title, string sub)
        {
            if (screen.transform.Find("HeaderBar") != null) return;

            GameObject header = new GameObject("HeaderBar", typeof(RectTransform));
            header.transform.SetParent(screen.transform, false);
            RectTransform rt = header.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.9f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            GameObject textGo = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(header.transform, false);
            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = $"{title}  •  {sub}";
            tmp.fontSize = 28;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }
    }
}
#endif
