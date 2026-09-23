#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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

        [MenuItem("Googolplex/Unit 10/Non-Destructive/Skin All Buttons In Active Scene")]
        public static void SkinAllButtonsInActiveScene()
        {
            ReimportTexturesAsSprites();
            Dictionary<string, Sprite> spriteDict = LoadAllSprites();

            Sprite greenBtn = GetSprite(spriteDict, "UI_Button_Green");
            Sprite goldBtn = GetSprite(spriteDict, "UI_Button_Gold");
            Sprite blueBtn = GetSprite(spriteDict, "UI_Button_Blue");
            Sprite orangeBtn = GetSprite(spriteDict, "UI_Button_Orange");
            Sprite purpleBtn = GetSprite(spriteDict, "UI_Button_Purple");
            Sprite redBtn = GetSprite(spriteDict, "UI_Button_Red");
            Sprite greyBtn = GetSprite(spriteDict, "UI_Button_Grey");
            Sprite closeIcon = GetSprite(spriteDict, "Icon_Close_Circle");
            Sprite roundedBox = GetSprite(spriteDict, "UI_RoundedBox_9Slice");

            var allButtons = Object.FindObjectsOfType<Button>(true);
            int skinnedCount = 0;

            foreach (var btn in allButtons)
            {
                if (btn == null) continue;
                string bName = btn.gameObject.name.ToLower();

                // Skip close icon buttons so we don't overwrite circular X icon
                if (bName.Contains("close") && bName.Contains("card"))
                {
                    var cImg = btn.GetComponent<Image>();
                    if (cImg != null && closeIcon != null)
                    {
                        Undo.RecordObject(cImg, "Skin Card Close Button");
                        cImg.sprite = closeIcon;
                        cImg.color = Color.white;
                        cImg.preserveAspect = true;
                    }
                    continue;
                }

                Sprite targetSprite = null;

                if (bName.Contains("weekly") || bName.Contains("plant") || bName.Contains("handsraised") || bName.Contains("startnewterm") || bName.Contains("resetnewterm"))
                {
                    targetSprite = greenBtn;
                }
                else if (bName.Contains("quote") || bName.Contains("golden") || bName.Contains("endterm") || bName.Contains("harvest"))
                {
                    targetSprite = goldBtn;
                }
                else if (bName.Contains("marble"))
                {
                    targetSprite = orangeBtn;
                }
                else if (bName.Contains("teacher") && !bName.Contains("close"))
                {
                    targetSprite = purpleBtn;
                }
                else if (bName.Contains("return") || bName.Contains("nextweek") || bName.Contains("garden"))
                {
                    targetSprite = blueBtn;
                }
                else if (bName.Contains("reset") || bName.Contains("delete") || bName.Contains("danger"))
                {
                    targetSprite = redBtn;
                }
                else if (bName.Contains("skip") || bName.Contains("next") || bName.Contains("close") || bName.Contains("cancel") || bName.Contains("modalclose"))
                {
                    targetSprite = greyBtn;
                }
                else if (bName.StartsWith("habitoptionbtn") || bName.StartsWith("card_"))
                {
                    targetSprite = roundedBox;
                }
                else
                {
                    targetSprite = greenBtn;
                }

                if (targetSprite != null)
                {
                    var img = btn.GetComponent<Image>();
                    if (img != null)
                    {
                        Undo.RecordObject(img, "Skin Button Sprite");
                        img.sprite = targetSprite;
                        img.type = Image.Type.Sliced;
                        img.color = Color.white;
                    }

                    if (btn.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>() == null)
                    {
                        btn.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                    }
                    skinnedCount++;
                }
            }

            // Also skin all StageBadge backgrounds in the scene to smooth rounded 9-slice pills
            var allStageBadges = Object.FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (var tmp in allStageBadges)
            {
                if (tmp != null && (tmp.gameObject.name.Contains("Stage") || tmp.transform.parent.name.Contains("StageBadge")))
                {
                    var pImg = tmp.transform.parent.GetComponent<Image>() ?? tmp.GetComponent<Image>();
                    if (pImg != null && roundedBox != null)
                    {
                        Undo.RecordObject(pImg, "Skin Stage Badge Rounded Box");
                        pImg.sprite = roundedBox;
                        pImg.type = Image.Type.Sliced;
                        pImg.color = new Color(0.12f, 0.45f, 0.22f, 0.95f);
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"<color=green>[U10_SceneSetupTool] Successfully skinned {skinnedCount} buttons and all Stage Badges with smooth rounded 9-slice sprites!</color>");
        }

        [MenuItem("Googolplex/Unit 10/Non-Destructive/Update Weekly Check Close Button & Sprites")]
        public static void UpdateWeeklyCheckCloseButtonAndSprites()
        {
            ReimportTexturesAsSprites();
            var checkScreen = Object.FindObjectOfType<U10_WeeklyCheckScreen_Masters_Activity>(true);
            if (checkScreen == null)
            {
                Debug.LogWarning("[U10_SceneSetupTool] WeeklyCheckScreen component not found in active scene.");
                return;
            }

            Dictionary<string, Sprite> spriteDict = LoadAllSprites();
            Sprite closeIconSprite = GetSprite(spriteDict, "Icon_Close_Circle");
            Sprite greenBtnSprite = GetSprite(spriteDict, "UI_Button_Green");
            Sprite greyBtnSprite = GetSprite(spriteDict, "UI_Button_Grey");
            Sprite roundedBox = GetSprite(spriteDict, "UI_Card_White_Soft");

            // Locate HabitStepCard
            Transform cardT = checkScreen.transform.Find("HabitStepCard");
            if (cardT == null)
            {
                foreach (Transform child in checkScreen.transform)
                {
                    if (child.name.Contains("Card") || child.name.Contains("Step") || child.name.Contains("Habit"))
                    {
                        cardT = child;
                        break;
                    }
                }
            }

            if (cardT != null)
            {
                // Resize HabitStepCard to spacious 1200x640 2-column layout
                RectTransform cardRT = cardT.GetComponent<RectTransform>();
                if (cardRT != null)
                {
                    cardRT.sizeDelta = new Vector2(1200, 640);
                    cardRT.anchoredPosition = new Vector2(0, -20);
                }

                // Create or find CloseCardBtn on top right corner of the card
                Transform existingClose = cardT.Find("CloseCardBtn");
                GameObject closeBtnObj;
                if (existingClose == null)
                {
                    closeBtnObj = new GameObject("CloseCardBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                    closeBtnObj.transform.SetParent(cardT, false);
                    Undo.RegisterCreatedObjectUndo(closeBtnObj, "Create Card Close Button");
                }
                else
                {
                    closeBtnObj = existingClose.gameObject;
                }

                RectTransform rt = closeBtnObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(-38, -38);
                rt.sizeDelta = new Vector2(58, 58);

                Image img = closeBtnObj.GetComponent<Image>();
                img.sprite = closeIconSprite;
                img.color = Color.white;
                img.preserveAspect = true;

                Button closeBtn = closeBtnObj.GetComponent<Button>();
                if (closeBtnObj.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>() == null)
                {
                    closeBtnObj.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                }

                // Hide old top-bar ReturnToGardenBtn
                Transform oldBackBtn = checkScreen.transform.Find("BackToGardenBtn");
                if (oldBackBtn != null)
                {
                    oldBackBtn.gameObject.SetActive(false);
                }

                // Reposition Step Indicator Badge (Top Center)
                Transform stepBadgeT = cardT.Find("StepIndicatorBadge");
                if (stepBadgeT != null)
                {
                    RectTransform srt = stepBadgeT.GetComponent<RectTransform>();
                    srt.anchoredPosition = new Vector2(0, 275);
                    srt.sizeDelta = new Vector2(260, 46);
                }

                // Reposition Habit Icon (Right Column top)
                Transform iconT = cardT.Find("HabitIcon") ?? cardT.Find("Icon");
                if (iconT != null)
                {
                    RectTransform irt = iconT.GetComponent<RectTransform>();
                    irt.anchoredPosition = new Vector2(180, 200);
                    irt.sizeDelta = new Vector2(95, 95);
                }

                // Reposition Habit Title Box (Right Column)
                Transform titleT = cardT.Find("HabitTitleBox") ?? cardT.Find("TitleText") ?? cardT.Find("Title");
                if (titleT != null)
                {
                    RectTransform trt = titleT.GetComponent<RectTransform>();
                    trt.anchoredPosition = new Vector2(180, 125);
                    trt.sizeDelta = new Vector2(700, 48);
                }

                // Reposition Habit Prompt / Question Box (Right Column)
                Transform promptT = cardT.Find("HabitPromptBox") ?? cardT.Find("QuestionText") ?? cardT.Find("Question");
                if (promptT != null)
                {
                    RectTransform prt = promptT.GetComponent<RectTransform>();
                    prt.anchoredPosition = new Vector2(180, 55);
                    prt.sizeDelta = new Vector2(700, 70);
                }

                // Reposition Habit Quote Box (Right Column)
                Transform quoteT = cardT.Find("HabitQuoteBox") ?? cardT.Find("QuoteText") ?? cardT.Find("Quote");
                if (quoteT != null)
                {
                    RectTransform qrt = quoteT.GetComponent<RectTransform>();
                    qrt.anchoredPosition = new Vector2(180, -15);
                    qrt.sizeDelta = new Vector2(700, 55);
                }

                // Reposition Status Feedback Box (Right Column)
                Transform statusT = cardT.Find("StatusFeedbackBox") ?? cardT.Find("StatusFeedbackText") ?? cardT.Find("PromptText");
                if (statusT != null)
                {
                    RectTransform srt = statusT.GetComponent<RectTransform>();
                    srt.anchoredPosition = new Vector2(180, -75);
                    srt.sizeDelta = new Vector2(700, 48);
                }

                // Wire to WeeklyCheckScreen component
                SerializedObject so = new SerializedObject(checkScreen);
                so.FindProperty("closeCardButton").objectReferenceValue = closeBtn;

                // Reposition & update HandsRaised button (Right Column bottom-left)
                var handsBtnProp = so.FindProperty("handsRaisedButton");
                if (handsBtnProp != null && handsBtnProp.objectReferenceValue != null)
                {
                    Button handsBtn = handsBtnProp.objectReferenceValue as Button;
                    if (handsBtn != null)
                    {
                        RectTransform hrt = handsBtn.GetComponent<RectTransform>();
                        hrt.anchoredPosition = new Vector2(30, -170);
                        hrt.sizeDelta = new Vector2(300, 74);

                        if (greenBtnSprite != null)
                        {
                            var bImg = handsBtn.GetComponent<Image>();
                            if (bImg != null)
                            {
                                bImg.sprite = greenBtnSprite;
                                bImg.type = Image.Type.Sliced;
                                bImg.color = Color.white;
                            }
                        }
                        if (handsBtn.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>() == null)
                        {
                            handsBtn.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                        }
                    }
                }

                // Reposition & update Skip button (Right Column bottom-right)
                var skipBtnProp = so.FindProperty("skipHabitButton");
                if (skipBtnProp != null && skipBtnProp.objectReferenceValue != null)
                {
                    Button skipBtn = skipBtnProp.objectReferenceValue as Button;
                    if (skipBtn != null)
                    {
                        RectTransform srt = skipBtn.GetComponent<RectTransform>();
                        srt.anchoredPosition = new Vector2(360, -170);
                        srt.sizeDelta = new Vector2(240, 74);

                        if (greyBtnSprite != null)
                        {
                            var bImg = skipBtn.GetComponent<Image>();
                            if (bImg != null)
                            {
                                bImg.sprite = greyBtnSprite;
                                bImg.type = Image.Type.Sliced;
                                bImg.color = Color.white;
                            }
                        }
                        if (skipBtn.GetComponent<U10_ButtonAttentionPulse_Masters_Activity>() == null)
                        {
                            skipBtn.gameObject.AddComponent<U10_ButtonAttentionPulse_Masters_Activity>();
                        }
                    }
                }

                // Ensure Live Plant Display is on the Left Column of the Card (x = -380)
                Transform existingPlantDisplay = cardT.Find("CardPlantDisplay");
                GameObject potObj;
                U10_PlantDisplayUI_Masters_Activity plantUI;
                Image plantImg;
                Image potImg;
                TextMeshProUGUI stageTMP;

                if (existingPlantDisplay == null)
                {
                    potObj = new GameObject("CardPlantDisplay", typeof(RectTransform));
                    potObj.transform.SetParent(cardT, false);
                    Undo.RegisterCreatedObjectUndo(potObj, "Create Card Plant Display");
                    plantUI = potObj.AddComponent<U10_PlantDisplayUI_Masters_Activity>();

                    // Plant Sprite (Tall and centered above the pot rim)
                    GameObject plantSpObj = new GameObject("PlantSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    plantSpObj.transform.SetParent(potObj.transform, false);
                    RectTransform plantSpRT = plantSpObj.GetComponent<RectTransform>();
                    plantSpRT.anchoredPosition = new Vector2(0, 75);
                    plantSpRT.sizeDelta = new Vector2(250, 260);
                    plantImg = plantSpObj.GetComponent<Image>();
                    plantImg.sprite = GetSprite(spriteDict, "Plant_Water_1");
                    plantImg.preserveAspect = true;
                    plantImg.raycastTarget = false;

                    // Pot Sprite (Terracotta pot)
                    GameObject potImgObj = new GameObject("PotSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    potImgObj.transform.SetParent(potObj.transform, false);
                    RectTransform potImgRT = potImgObj.GetComponent<RectTransform>();
                    potImgRT.anchoredPosition = new Vector2(0, -65);
                    potImgRT.sizeDelta = new Vector2(240, 175);
                    potImg = potImgObj.GetComponent<Image>();
                    potImg.sprite = GetSprite(spriteDict, "Pot_Terracotta");
                    potImg.preserveAspect = true;
                    potImg.raycastTarget = false;

                    // Pot Label Badge (Pill beneath pot displaying habit name)
                    GameObject stageCard = CreateUIBox(potObj.transform, "StageBadge", "Habit Name", 24, new Vector2(0, -175), new Vector2(220, 44), new Color(0.12f, 0.45f, 0.22f), Color.white, roundedBox);
                    stageTMP = stageCard.GetComponentInChildren<TextMeshProUGUI>();
                }
                else
                {
                    potObj = existingPlantDisplay.gameObject;
                    plantUI = potObj.GetComponent<U10_PlantDisplayUI_Masters_Activity>() ?? potObj.AddComponent<U10_PlantDisplayUI_Masters_Activity>();

                    // Reposition existing PlantSprite
                    Transform pst = potObj.transform.Find("PlantSprite");
                    if (pst != null)
                    {
                        RectTransform prt = pst.GetComponent<RectTransform>();
                        prt.anchoredPosition = new Vector2(0, 75);
                        prt.sizeDelta = new Vector2(250, 260);
                        plantImg = pst.GetComponent<Image>();
                    }
                    else
                    {
                        GameObject plantSpObj = new GameObject("PlantSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                        plantSpObj.transform.SetParent(potObj.transform, false);
                        RectTransform plantSpRT = plantSpObj.GetComponent<RectTransform>();
                        plantSpRT.anchoredPosition = new Vector2(0, 75);
                        plantSpRT.sizeDelta = new Vector2(250, 260);
                        plantImg = plantSpObj.GetComponent<Image>();
                        plantImg.sprite = GetSprite(spriteDict, "Plant_Water_1");
                        plantImg.preserveAspect = true;
                    }

                    // Reposition existing PotSprite
                    Transform potSt = potObj.transform.Find("PotSprite");
                    if (potSt != null)
                    {
                        RectTransform port = potSt.GetComponent<RectTransform>();
                        port.anchoredPosition = new Vector2(0, -65);
                        port.sizeDelta = new Vector2(240, 175);
                        potImg = potSt.GetComponent<Image>();
                    }
                    else
                    {
                        GameObject potImgObj = new GameObject("PotSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                        potImgObj.transform.SetParent(potObj.transform, false);
                        RectTransform potImgRT = potImgObj.GetComponent<RectTransform>();
                        potImgRT.anchoredPosition = new Vector2(0, -65);
                        potImgRT.sizeDelta = new Vector2(240, 175);
                        potImg = potImgObj.GetComponent<Image>();
                        potImg.sprite = GetSprite(spriteDict, "Pot_Terracotta");
                        potImg.preserveAspect = true;
                    }

                    // Reposition StageBadge
                    Transform sbt = potObj.transform.Find("StageBadge");
                    if (sbt != null)
                    {
                        RectTransform sbrt = sbt.GetComponent<RectTransform>();
                        sbrt.anchoredPosition = new Vector2(0, -175);
                        sbrt.sizeDelta = new Vector2(220, 44);
                        stageTMP = sbt.GetComponentInChildren<TextMeshProUGUI>();

                        var sImg = sbt.GetComponent<Image>();
                        if (sImg != null && roundedBox != null)
                        {
                            sImg.sprite = roundedBox;
                            sImg.type = Image.Type.Sliced;
                            sImg.color = new Color(0.12f, 0.45f, 0.22f, 0.95f);
                        }
                    }
                    else
                    {
                        GameObject stageCard = CreateUIBox(potObj.transform, "StageBadge", "Habit Name", 24, new Vector2(0, -175), new Vector2(220, 44), new Color(0.12f, 0.45f, 0.22f), Color.white, roundedBox);
                        stageTMP = stageCard.GetComponentInChildren<TextMeshProUGUI>();
                    }

                    // Disable any old blank square glow object
                    Transform glowT = potObj.transform.Find("Glow");
                    if (glowT != null) glowT.gameObject.SetActive(false);
                }

                RectTransform potContainerRT = potObj.GetComponent<RectTransform>();
                potContainerRT.anchorMin = new Vector2(0.5f, 0.5f);
                potContainerRT.anchorMax = new Vector2(0.5f, 0.5f);
                potContainerRT.pivot = new Vector2(0.5f, 0.5f);
                potContainerRT.anchoredPosition = new Vector2(-380, -10);
                potContainerRT.sizeDelta = new Vector2(320, 500);

                // Wire PlantDisplayUI
                SerializedObject pso = new SerializedObject(plantUI);
                pso.FindProperty("potImage").objectReferenceValue = potImg;
                pso.FindProperty("plantImage").objectReferenceValue = plantImg;
                pso.FindProperty("stageBadgeText").objectReferenceValue = stageTMP;
                pso.ApplyModifiedProperties();

                so.FindProperty("cardPlantDisplay").objectReferenceValue = plantUI;
                so.FindProperty("potSprite").objectReferenceValue = GetSprite(spriteDict, "Pot_Terracotta");

                so.ApplyModifiedProperties();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                Debug.Log("<color=green>[U10_SceneSetupTool] Successfully updated WeeklyCheckScreen into spacious 2-column layout with Left Potted Plant and Right Content!</color>");
            }
        }

        [MenuItem("Googolplex/Unit 10/Non-Destructive/Assign All Inspector Fields In Active Scene")]
        public static void AssignAllInspectorFieldsInActiveScene()
        {
            ReimportTexturesAsSprites();
            Dictionary<string, Sprite> spriteDict = LoadAllSprites();
            Dictionary<string, AudioClip> audioDict = LoadAllAudio();

            int populatedComponents = 0;

            // 1. Weekly Check Screen
            var weeklyScreen = Object.FindObjectOfType<U10_WeeklyCheckScreen_Masters_Activity>(true);
            if (weeklyScreen != null)
            {
                SerializedObject so = new SerializedObject(weeklyScreen);
                SetClipProp(so, "voWeeklyCheckStart", audioDict, "VO_U10_03");
                SetClipProp(so, "voWater", audioDict, "VO_U10_04");
                SetClipProp(so, "voSleep", audioDict, "VO_U10_05");
                SetClipProp(so, "voOutside", audioDict, "VO_U10_06");
                SetClipProp(so, "voRead", audioDict, "VO_U10_07");
                SetClipProp(so, "voQuiet", audioDict, "VO_U10_08");
                SetClipProp(so, "voWalk", audioDict, "VO_U10_09");
                SetClipProp(so, "voGive", audioDict, "VO_U10_10");
                SetClipProp(so, "voFamily", audioDict, "VO_U10_11");
                SetClipProp(so, "voPlantGrowing", audioDict, "VO_U10_12");

                so.FindProperty("potSprite").objectReferenceValue = GetSprite(spriteDict, "Pot_Terracotta");
                var cardPlant = weeklyScreen.GetComponentInChildren<U10_PlantDisplayUI_Masters_Activity>(true);
                if (cardPlant != null)
                {
                    so.FindProperty("cardPlantDisplay").objectReferenceValue = cardPlant;
                }

                List<Sprite> habitIcons = new List<Sprite>();
                foreach (var h in U10_SaveSystem.ALL_AVAILABLE_HABITS)
                {
                    Sprite s = GetSprite(spriteDict, $"Icon_{h.key}");
                    if (s != null) habitIcons.Add(s);
                }
                SetObjectList(so.FindProperty("allHabitIcons"), habitIcons);

                so.ApplyModifiedProperties();
                populatedComponents++;
            }

            // 2. Golden Line Modal
            var modal = Object.FindObjectOfType<U10_GoldenLineModal_Masters_Activity>(true);
            if (modal != null)
            {
                SerializedObject so = new SerializedObject(modal);
                List<AudioClip> goldenClips = new List<AudioClip>();
                for (int w = 1; w <= 12; w++)
                {
                    string key = $"VO_U10_13_{w:D2}";
                    if (audioDict.TryGetValue(key, out AudioClip clip))
                    {
                        goldenClips.Add(clip);
                    }
                }
                SetObjectList(so.FindProperty("goldenLineVoiceovers"), goldenClips);
                so.ApplyModifiedProperties();
                populatedComponents++;
            }

            // 3. Setup Screen
            var setupScreen = Object.FindObjectOfType<U10_SetupScreen_Masters_Activity>(true);
            if (setupScreen != null)
            {
                SerializedObject so = new SerializedObject(setupScreen);
                SetClipProp(so, "voSetupIntro", audioDict, "VO_U10_02");
                so.ApplyModifiedProperties();
                populatedComponents++;
            }

            // 4. Garden Screen
            var gardenScreen = Object.FindObjectOfType<U10_GardenScreen_Masters_Activity>(true);
            if (gardenScreen != null)
            {
                SerializedObject so = new SerializedObject(gardenScreen);
                SetClipProp(so, "voGardenWelcome", audioDict, "VO_U10_01");

                Sprite[] jarSprites = new Sprite[]
                {
                    GetSprite(spriteDict, "Jar_Glass_Empty"),
                    GetSprite(spriteDict, "Jar_Glass_Stage1"),
                    GetSprite(spriteDict, "Jar_Glass_Stage2"),
                    GetSprite(spriteDict, "Jar_Glass_Stage3"),
                    GetSprite(spriteDict, "Jar_Glass_Full")
                };
                SerializedProperty jarArr = so.FindProperty("jarStageSprites");
                if (jarArr != null)
                {
                    jarArr.arraySize = jarSprites.Length;
                    for (int j = 0; j < jarSprites.Length; j++)
                    {
                        jarArr.GetArrayElementAtIndex(j).objectReferenceValue = jarSprites[j];
                    }
                }

                so.ApplyModifiedProperties();
                populatedComponents++;
            }

            // 5. End Term Screen
            var endTermScreen = Object.FindObjectOfType<U10_EndTermScreen_Masters_Activity>(true);
            if (endTermScreen != null)
            {
                SerializedObject so = new SerializedObject(endTermScreen);
                SetClipProp(so, "voHarvestCelebration", audioDict, "VO_U10_14");
                so.ApplyModifiedProperties();
                populatedComponents++;
            }

            // 6. Audio Manager
            var am = Object.FindObjectOfType<U10_AudioManager_Masters_Activity>(true);
            if (am != null)
            {
                WireAudioManager(am, audioDict);
                populatedComponents++;
            }

            // 7. Game Manager
            var gm = Object.FindObjectOfType<U10_GameManager_Masters_Activity>(true);
            if (gm != null)
            {
                SerializedObject so = new SerializedObject(gm);
                if (modal != null)
                {
                    so.FindProperty("goldenLineModal").objectReferenceValue = modal.gameObject;
                }
                var setup = Object.FindObjectOfType<U10_SetupScreen_Masters_Activity>(true);
                if (setup != null) so.FindProperty("setupScreen").objectReferenceValue = setup.gameObject;

                var garden = Object.FindObjectOfType<U10_GardenScreen_Masters_Activity>(true);
                if (garden != null) so.FindProperty("gardenScreen").objectReferenceValue = garden.gameObject;

                var check = Object.FindObjectOfType<U10_WeeklyCheckScreen_Masters_Activity>(true);
                if (check != null) so.FindProperty("weeklyCheckScreen").objectReferenceValue = check.gameObject;

                var endTerm = Object.FindObjectOfType<U10_EndTermScreen_Masters_Activity>(true);
                if (endTerm != null) so.FindProperty("endTermScreen").objectReferenceValue = endTerm.gameObject;

                so.FindProperty("potSprite").objectReferenceValue = GetSprite(spriteDict, "Pot_Terracotta");
                List<Sprite> allPlants = new List<Sprite>();
                foreach (var kv in spriteDict)
                {
                    if (kv.Key.StartsWith("Plant_"))
                    {
                        allPlants.Add(kv.Value);
                    }
                }
                SetObjectList(so.FindProperty("allPlantSprites"), allPlants);
                so.ApplyModifiedProperties();
                populatedComponents++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"<color=green>[U10_SceneSetupTool] Successfully assigned all Inspector serialized fields (VO Clips, Sprites, Audio) across {populatedComponents} components in active scene!</color>");
        }

        [MenuItem("Googolplex/Unit 10/Non-Destructive/Fix Pot Labels and Badges")]
        public static void FixPotLabelsAndBadges()
        {
            var spriteDict = LoadAllSprites();
            Sprite roundedBox = GetSprite(spriteDict, "UI_RoundedBox_9Slice") ?? GetSprite(spriteDict, "Card_Parchment");

            var plantDisplays = Object.FindObjectsOfType<U10_PlantDisplayUI_Masters_Activity>(true);
            int fixedCount = 0;
            for (int i = 0; i < plantDisplays.Length; i++)
            {
                var pd = plantDisplays[i];
                Transform habitBadgeT = pd.transform.Find("HabitBadge");
                if (habitBadgeT != null)
                {
                    var htmp = habitBadgeT.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (htmp != null)
                    {
                        Undo.RecordObject(htmp, "Set HabitBadge Text");
                        htmp.text = $"Habit {i + 1}";
                    }
                }

                Transform stageBadgeT = pd.transform.Find("StageBadge");
                if (stageBadgeT != null)
                {
                    var img = stageBadgeT.GetComponent<Image>();
                    if (img != null && roundedBox != null)
                    {
                        Undo.RecordObject(img, "Set StageBadge Sprite");
                        img.sprite = roundedBox;
                        img.type = Image.Type.Sliced;
                        img.color = new Color(0.12f, 0.45f, 0.22f, 0.95f);
                    }
                    var btmp = stageBadgeT.GetComponentInChildren<TextMeshProUGUI>(true);
                    if (btmp != null)
                    {
                        Undo.RecordObject(btmp, "Set StageBadge Text");
                        btmp.text = "Stage 1 of 11";
                    }
                }

                // Ensure serialized references are linked properly
                SerializedObject pso = new SerializedObject(pd);
                if (habitBadgeT != null)
                {
                    pso.FindProperty("titleText").objectReferenceValue = habitBadgeT.GetComponentInChildren<TextMeshProUGUI>(true);
                }
                if (stageBadgeT != null)
                {
                    pso.FindProperty("stageBadgeText").objectReferenceValue = stageBadgeT.GetComponentInChildren<TextMeshProUGUI>(true);
                }
                pso.ApplyModifiedProperties();

                fixedCount++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"<color=green>[U10_SceneSetupTool] Fixed pot labels and badges on {fixedCount} plant display objects (Habit Badge = Habit Name, Stage Badge = Stage Progress)!</color>");
        }

        [MenuItem("Googolplex/Unit 10/Non-Destructive/Apply Game-Like Card and Plaque Sprites")]
        public static void ApplyGameLikeCardAndPlaqueSprites()
        {
            ReimportTexturesAsSprites();
            var spriteDict = LoadAllSprites();

            Sprite plaqueWood = FindBestSprite(spriteDict, "Header sprite U10 borad card Bg sprites_0", "Header sprite", "UI_Title_Plaque_Wood");
            Sprite cardContainer = FindBestSprite(spriteDict, "Main board U10 borad card Bg sprites_1", "Main board", "UI_Card_Container_Main");
            Sprite subcardPlant = FindBestSprite(spriteDict, "sub board U10 borad card Bg sprites_7", "sub board", "UI_SubCard_Plant");
            Sprite subcardContent = FindBestSprite(spriteDict, "sub boardU10 borad card Bg sprites_8", "sub boardU10", "UI_SubCard_Content");
            Sprite habitCardNormal = FindBestSprite(spriteDict, "sub board U10 borad card Bg sprites_7", "UI_Habit_Card_Normal", "UI_SubCard_Plant");
            Sprite footerParchment = FindBestSprite(spriteDict, "sub boardU10 borad card Bg sprites_8", "UI_Bar_Footer_Parchment", "UI_SubCard_Content");

            void SetSpriteToImage(Image img, Sprite sp)
            {
                if (img == null || sp == null) return;
                img.sprite = sp;
                img.type = (sp.border != Vector4.zero) ? Image.Type.Sliced : Image.Type.Simple;
                img.preserveAspect = false;
                img.color = Color.white;
            }

            int appliedCount = 0;

            // 1. Title Banners across SetupScreen, GardenScreen, and WeeklyCheckScreen
            var allTMPs = Object.FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (var tmp in allTMPs)
            {
                string pName = tmp.transform.parent != null ? tmp.transform.parent.name : "";
                if (pName.Equals("HeaderBanner", StringComparison.OrdinalIgnoreCase) ||
                    pName.Equals("WeeklyHeaderBox", StringComparison.OrdinalIgnoreCase) ||
                    pName.Equals("EndTermHeaderBox", StringComparison.OrdinalIgnoreCase) ||
                    pName.Equals("HeaderBox", StringComparison.OrdinalIgnoreCase))
                {
                    var parentT = tmp.transform.parent;
                    var img = parentT.GetComponent<Image>();
                    if (img != null && plaqueWood != null)
                    {
                        Undo.RecordObject(img, "Apply Title Plaque");
                        SetSpriteToImage(img, plaqueWood);

                        // Give plaque adequate height and text side margins so leaves never squeeze text
                        RectTransform pRT = parentT.GetComponent<RectTransform>();
                        if (pRT != null)
                        {
                            Undo.RecordObject(pRT, "Adjust Plaque Height");
                            float targetWidth = pRT.sizeDelta.x;
                            if (pName.Equals("HeaderBox", StringComparison.OrdinalIgnoreCase) || pName.Equals("EndTermHeaderBox", StringComparison.OrdinalIgnoreCase))
                            {
                                targetWidth = Mathf.Min(targetWidth, 1150f);
                            }
                            else
                            {
                                targetWidth = Mathf.Min(targetWidth, 880f);
                            }
                            pRT.sizeDelta = new Vector2(targetWidth, 122f);
                        }

                        // Add horizontal padding inside the plaque so text stays inside the central wooden board
                        Undo.RecordObject(tmp, "Adjust Plaque Text Margins");
                        tmp.margin = new Vector4(125f, 6f, 125f, 6f);
                        tmp.alignment = TextAlignmentOptions.Center;

                        appliedCount++;
                    }
                }
                else if (pName.Equals("SelectionSummaryBar", StringComparison.OrdinalIgnoreCase) ||
                         pName.Equals("BottomBar", StringComparison.OrdinalIgnoreCase))
                {
                    var parentT = tmp.transform.parent;
                    var img = parentT.GetComponent<Image>();
                    if (img != null && footerParchment != null)
                    {
                        Undo.RecordObject(img, "Apply Footer Bar");
                        SetSpriteToImage(img, footerParchment);

                        RectTransform bBarRT = parentT.GetComponent<RectTransform>();
                        if (bBarRT != null)
                        {
                            Undo.RecordObject(bBarRT, "Adjust Footer Bar Size");
                            bBarRT.sizeDelta = new Vector2(1240f, 110f);
                        }

                        appliedCount++;
                    }
                }
            }

            // 2. Setup Screen: 8 Habit Cards (Keep text & icons safely inside central area)
            var setup = Object.FindObjectOfType<U10_SetupScreen_Masters_Activity>(true);
            if (setup != null && habitCardNormal != null)
            {
                var buttons = setup.GetComponentsInChildren<Button>(true);
                foreach (var b in buttons)
                {
                    if (b.gameObject.name.StartsWith("HabitOptionBtn_") || b.gameObject.name.StartsWith("Card_"))
                    {
                        var img = b.GetComponent<Image>();
                        if (img != null)
                        {
                            Undo.RecordObject(img, "Apply Habit Card Normal Sprite");
                            SetSpriteToImage(img, habitCardNormal);
                            appliedCount++;
                        }

                        // Adjust Icon padding away from top leaves
                        Transform iconT = b.transform.Find("Icon");
                        if (iconT != null)
                        {
                            RectTransform irt = iconT.GetComponent<RectTransform>();
                            Undo.RecordObject(irt, "Adjust Icon Size & Pos");
                            irt.sizeDelta = new Vector2(88f, 88f);
                            irt.anchoredPosition = new Vector2(0f, -18f);
                        }

                        // Adjust Title padding away from side leaf accents
                        Transform titleT = b.transform.Find("Title");
                        if (titleT != null)
                        {
                            RectTransform trt = titleT.GetComponent<RectTransform>();
                            Undo.RecordObject(trt, "Adjust Title Size & Pos");
                            trt.sizeDelta = new Vector2(290f, 36f);
                            trt.anchoredPosition = new Vector2(0f, -118f);
                            var ttmp = titleT.GetComponent<TextMeshProUGUI>();
                            if (ttmp != null) ttmp.fontSize = 28f;
                        }

                        // Adjust Quote padding away from bottom leaf accents
                        Transform quoteT = b.transform.Find("Quote");
                        if (quoteT != null)
                        {
                            RectTransform qrt = quoteT.GetComponent<RectTransform>();
                            Undo.RecordObject(qrt, "Adjust Quote Size & Pos");
                            qrt.sizeDelta = new Vector2(290f, 68f);
                            qrt.anchoredPosition = new Vector2(0f, -176f);
                            var qtmp = quoteT.GetComponent<TextMeshProUGUI>();
                            if (qtmp != null) qtmp.fontSize = 19f;
                        }
                    }
                }
            }

            // 3. Weekly Check Screen: HabitStepCard + PlantSubCard + ContentSubCard
            var checkScreen = Object.FindObjectOfType<U10_WeeklyCheckScreen_Masters_Activity>(true);
            if (checkScreen != null)
            {
                Transform habitCardT = checkScreen.transform.Find("HabitStepCard");
                if (habitCardT != null)
                {
                    // Apply main container parchment sprite
                    var mainImg = habitCardT.GetComponent<Image>();
                    if (mainImg != null && cardContainer != null)
                    {
                        Undo.RecordObject(mainImg, "Apply Main Card Container Sprite");
                        SetSpriteToImage(mainImg, cardContainer);
                        appliedCount++;
                    }

                    // A. Left Child Sub-Card for Plant
                    Transform plantSubCardT = habitCardT.Find("PlantSubCard");
                    if (plantSubCardT == null)
                    {
                        GameObject pscObj = new GameObject("PlantSubCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                        pscObj.transform.SetParent(habitCardT, false);
                        Undo.RegisterCreatedObjectUndo(pscObj, "Create PlantSubCard");

                        RectTransform prt = pscObj.GetComponent<RectTransform>();
                        prt.anchorMin = new Vector2(0.5f, 0.5f);
                        prt.anchorMax = new Vector2(0.5f, 0.5f);
                        prt.pivot = new Vector2(0.5f, 0.5f);
                        prt.anchoredPosition = new Vector2(-365, -10);
                        prt.sizeDelta = new Vector2(400, 560);

                        Image pImg = pscObj.GetComponent<Image>();
                        SetSpriteToImage(pImg, subcardPlant);
                        plantSubCardT = pscObj.transform;
                        appliedCount++;
                    }
                    else
                    {
                        RectTransform prt = plantSubCardT.GetComponent<RectTransform>();
                        if (prt != null)
                        {
                            prt.anchoredPosition = new Vector2(-365, -10);
                            prt.sizeDelta = new Vector2(400, 560);
                        }
                        Image pImg = plantSubCardT.GetComponent<Image>();
                        if (pImg != null && subcardPlant != null)
                        {
                            Undo.RecordObject(pImg, "Update PlantSubCard Sprite");
                            SetSpriteToImage(pImg, subcardPlant);
                            appliedCount++;
                        }
                    }

                    // Reparent CardPlantDisplay into PlantSubCard
                    Transform cpdT = habitCardT.Find("CardPlantDisplay") ?? plantSubCardT.Find("CardPlantDisplay");
                    if (cpdT != null)
                    {
                        Undo.SetTransformParent(cpdT, plantSubCardT, "Parent CardPlantDisplay to PlantSubCard");
                        RectTransform cpdRT = cpdT.GetComponent<RectTransform>();
                        cpdRT.anchorMin = new Vector2(0.5f, 0.5f);
                        cpdRT.anchorMax = new Vector2(0.5f, 0.5f);
                        cpdRT.pivot = new Vector2(0.5f, 0.5f);
                        cpdRT.anchoredPosition = Vector2.zero;
                    }

                    // B. Right Child Sub-Card for Content
                    Transform contentSubCardT = habitCardT.Find("ContentSubCard");
                    if (contentSubCardT == null)
                    {
                        GameObject cscObj = new GameObject("ContentSubCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                        cscObj.transform.SetParent(habitCardT, false);
                        Undo.RegisterCreatedObjectUndo(cscObj, "Create ContentSubCard");

                        RectTransform crt = cscObj.GetComponent<RectTransform>();
                        crt.anchorMin = new Vector2(0.5f, 0.5f);
                        crt.anchorMax = new Vector2(0.5f, 0.5f);
                        crt.pivot = new Vector2(0.5f, 0.5f);
                        crt.anchoredPosition = new Vector2(215, -10);
                        crt.sizeDelta = new Vector2(700, 560);

                        Image cImg = cscObj.GetComponent<Image>();
                        SetSpriteToImage(cImg, subcardContent);
                        contentSubCardT = cscObj.transform;
                        appliedCount++;
                    }
                    else
                    {
                        RectTransform crt = contentSubCardT.GetComponent<RectTransform>();
                        if (crt != null)
                        {
                            crt.anchoredPosition = new Vector2(215, -10);
                            crt.sizeDelta = new Vector2(700, 560);
                        }
                        Image cImg = contentSubCardT.GetComponent<Image>();
                        if (cImg != null && subcardContent != null)
                        {
                            Undo.RecordObject(cImg, "Update ContentSubCard Sprite");
                            SetSpriteToImage(cImg, subcardContent);
                            appliedCount++;
                        }
                    }

                    // Reparent all right content elements into ContentSubCard with balanced offsets & width constrained to 560
                    string[] contentElementNames = { "HabitIcon", "HabitTitleBox", "HabitPromptBox", "HabitQuoteBox", "StatusFeedbackBox", "HandsRaisedBtn", "SkipBtn" };
                    Vector2[] targetPositions = {
                        new Vector2(0, 190),
                        new Vector2(0, 120),
                        new Vector2(0, 50),
                        new Vector2(0, -20),
                        new Vector2(0, -80),
                        new Vector2(-150, -175),
                        new Vector2(165, -175)
                    };

                    for (int ce = 0; ce < contentElementNames.Length; ce++)
                    {
                        Transform ceT = habitCardT.Find(contentElementNames[ce]) ?? contentSubCardT.Find(contentElementNames[ce]);
                        if (ceT != null)
                        {
                            Undo.SetTransformParent(ceT, contentSubCardT, $"Parent {contentElementNames[ce]} to ContentSubCard");
                            RectTransform ceRT = ceT.GetComponent<RectTransform>();
                            ceRT.anchorMin = new Vector2(0.5f, 0.5f);
                            ceRT.anchorMax = new Vector2(0.5f, 0.5f);
                            ceRT.pivot = new Vector2(0.5f, 0.5f);
                            ceRT.anchoredPosition = targetPositions[ce];

                            // Constrain text boxes to 560 width so they never collide with the golden corner ornaments
                            if (contentElementNames[ce].Contains("Box"))
                            {
                                ceRT.sizeDelta = new Vector2(560f, ceRT.sizeDelta.y);
                                var tmpComp = ceT.GetComponentInChildren<TextMeshProUGUI>();
                                if (tmpComp != null)
                                {
                                    tmpComp.color = new Color(0.12f, 0.14f, 0.18f); // High contrast dark charcoal
                                }
                            }
                        }
                    }

                    // Adjust close button so it doesn't overlap the top-right corner stud
                    Transform closeBtnT = habitCardT.Find("CloseBtn") ?? habitCardT.Find("WeeklyCheckCloseBtn");
                    if (closeBtnT != null)
                    {
                        RectTransform cbrt = closeBtnT.GetComponent<RectTransform>();
                        Undo.RecordObject(cbrt, "Adjust Close Button Offset");
                        cbrt.anchoredPosition = new Vector2(540f, 275f);
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"<color=green>[U10_SceneSetupTool] Successfully replaced plain white backgrounds with game-like card & plaque sprites across {appliedCount} elements!</color>");
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
                // CRITICAL: NEVER touch or modify multiple sprite sheets or manually sliced icon sheets
                if (assetPath.Contains("Spritesheet") || assetPath.Contains("sprites") || assetPath.Contains("Icons"))
                {
                    continue;
                }

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
                    else if (assetPath.Contains("UI_Button_"))
                    {
                        importer.spriteBorder = new Vector4(24, 24, 24, 24);
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
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { ART_PATH });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var obj in subAssets)
                {
                    if (obj is Sprite s && s != null && !dict.ContainsKey(s.name))
                    {
                        dict[s.name] = s;
                    }
                }
            }
            return dict;
        }

        private static Sprite FindBestSprite(Dictionary<string, Sprite> dict, params string[] candidates)
        {
            foreach (string c in candidates)
            {
                if (dict.TryGetValue(c, out Sprite exact) && exact != null) return exact;
            }
            foreach (string c in candidates)
            {
                foreach (var kvp in dict)
                {
                    if (kvp.Key.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return kvp.Value;
                    }
                }
            }
            return null;
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

                // Pot Label Badge (Displays habit title e.g. Water, Sleep)
                GameObject stageCard = CreateUIBox(potObj.transform, "StageBadge", "Habit Name", 24, new Vector2(0, -215), new Vector2(210, 40), new Color(0.12f, 0.45f, 0.22f), Color.white, roundedBox);
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

            // Habit Step Card (Center Anchor - 1200x640 2-column layout)
            GameObject habitCard = CreateUIBox(screenObj.transform, "HabitStepCard", "", 32, new Vector2(0, -20), new Vector2(1200, 640), new Color(0.98f, 0.99f, 0.98f, 0.98f), Color.black, roundedBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            // Step Indicator Badge (Top Center)
            GameObject stepBadge = CreateUIBox(habitCard.transform, "StepIndicatorBadge", "Habit 1 of 4", 28, new Vector2(0, 275), new Vector2(260, 46), new Color(0.16f, 0.48f, 0.3f), Color.white, roundedBox);
            TextMeshProUGUI stepTMP = stepBadge.GetComponentInChildren<TextMeshProUGUI>();

            // Habit Icon (Right Column top)
            GameObject iconObj = new GameObject("HabitIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObj.transform.SetParent(habitCard.transform, false);
            RectTransform iconRT = iconObj.GetComponent<RectTransform>();
            iconRT.anchoredPosition = new Vector2(180, 200);
            iconRT.sizeDelta = new Vector2(95, 95);
            Image habitIconImg = iconObj.GetComponent<Image>();
            habitIconImg.sprite = GetSprite(sprites, "Icon_Water");
            habitIconImg.preserveAspect = true;
            habitIconImg.raycastTarget = false;

            // Habit Title (Right Column)
            GameObject titleObj = CreateUIBox(habitCard.transform, "HabitTitleBox", "Drink Water", 42, new Vector2(180, 125), new Vector2(700, 48), Color.clear, new Color(0.05f, 0.18f, 0.08f));
            TextMeshProUGUI habitTitleTMP = titleObj.GetComponentInChildren<TextMeshProUGUI>();
            habitTitleTMP.fontStyle = FontStyles.Bold;

            // Habit Question Prompt (Right Column)
            GameObject promptObj = CreateUIBox(habitCard.transform, "HabitPromptBox", "Who drank fresh water every day this week?", 28, new Vector2(180, 55), new Vector2(700, 70), Color.clear, new Color(0.08f, 0.25f, 0.12f));
            TextMeshProUGUI habitPromptTMP = promptObj.GetComponentInChildren<TextMeshProUGUI>();
            habitPromptTMP.fontStyle = FontStyles.Bold;

            // Habit Book Quote (Right Column)
            GameObject quoteObj = CreateUIBox(habitCard.transform, "HabitQuoteBox", "\"Drink your water -- inside and outside.\"", 24, new Vector2(180, -15), new Vector2(700, 55), Color.clear, new Color(0.16f, 0.36f, 0.2f));
            TextMeshProUGUI habitQuoteTMP = quoteObj.GetComponentInChildren<TextMeshProUGUI>();
            habitQuoteTMP.fontStyle = FontStyles.Bold;

            // Status Feedback Text (Right Column)
            GameObject statusObj = CreateUIBox(habitCard.transform, "StatusFeedbackBox", "Raise your hand if you practiced this week!", 26, new Vector2(180, -75), new Vector2(700, 48), Color.clear, new Color(0.06f, 0.45f, 0.18f));
            TextMeshProUGUI statusTMP = statusObj.GetComponentInChildren<TextMeshProUGUI>();
            statusTMP.fontStyle = FontStyles.Bold;

            // Action Buttons (Right Column bottom)
            GameObject handsBtnObj = CreateUIButton(habitCard.transform, "HandsRaisedBtn", "Yes, We Practiced!", 34, new Vector2(30, -170), new Vector2(300, 74), new Color(0.16f, 0.62f, 0.28f), roundedBox);
            Button handsBtn = handsBtnObj.GetComponent<Button>();

            GameObject skipBtnObj = CreateUIButton(habitCard.transform, "SkipBtn", "Next Habit", 30, new Vector2(360, -170), new Vector2(240, 74), new Color(0.45f, 0.5f, 0.56f), roundedBox);
            Button skipBtn = skipBtnObj.GetComponent<Button>();

            // Card Plant Display (Left Column: x = -380)
            GameObject cardPlantObj = new GameObject("CardPlantDisplay", typeof(RectTransform));
            cardPlantObj.transform.SetParent(habitCard.transform, false);
            RectTransform cardPlantRT = cardPlantObj.GetComponent<RectTransform>();
            cardPlantRT.anchoredPosition = new Vector2(-380, -10);
            cardPlantRT.sizeDelta = new Vector2(320, 500);

            var plantUI = cardPlantObj.AddComponent<U10_PlantDisplayUI_Masters_Activity>();

            // Plant Sprite (Tall & prominent above the pot)
            GameObject plantSpObj = new GameObject("PlantSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            plantSpObj.transform.SetParent(cardPlantObj.transform, false);
            RectTransform plantSpRT = plantSpObj.GetComponent<RectTransform>();
            plantSpRT.anchoredPosition = new Vector2(0, 75);
            plantSpRT.sizeDelta = new Vector2(250, 260);
            Image plantImg = plantSpObj.GetComponent<Image>();
            plantImg.sprite = GetSprite(sprites, "Plant_Water_1");
            plantImg.preserveAspect = true;
            plantImg.raycastTarget = false;

            // Pot Sprite (Terracotta pot)
            GameObject potImgObj = new GameObject("PotSprite", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            potImgObj.transform.SetParent(cardPlantObj.transform, false);
            RectTransform potImgRT = potImgObj.GetComponent<RectTransform>();
            potImgRT.anchoredPosition = new Vector2(0, -65);
            potImgRT.sizeDelta = new Vector2(240, 175);
            Image potImg = potImgObj.GetComponent<Image>();
            potImg.sprite = GetSprite(sprites, "Pot_Terracotta");
            potImg.preserveAspect = true;
            potImg.raycastTarget = false;

            // Pot Label Badge (Displays habit title e.g. Water, Sleep)
            GameObject stageCard = CreateUIBox(cardPlantObj.transform, "StageBadge", "Habit Name", 24, new Vector2(0, -175), new Vector2(220, 44), new Color(0.12f, 0.45f, 0.22f), Color.white, roundedBox);
            TextMeshProUGUI stageTMP = stageCard.GetComponentInChildren<TextMeshProUGUI>();

            // Wire PlantDisplayUI
            SerializedObject pso = new SerializedObject(plantUI);
            pso.FindProperty("potImage").objectReferenceValue = potImg;
            pso.FindProperty("plantImage").objectReferenceValue = plantImg;
            pso.FindProperty("stageBadgeText").objectReferenceValue = stageTMP;
            pso.ApplyModifiedProperties();

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
            so.FindProperty("cardPlantDisplay").objectReferenceValue = plantUI;
            so.FindProperty("potSprite").objectReferenceValue = GetSprite(sprites, "Pot_Terracotta");

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
