# Context: Unit 6 "Eating Out / Table Etiquette"

## 1. Project Directory Structure
```
Assets/SeniorsActivityUnit6/
├── Art/
│   └── U6_MastersActivitySprites/
│       ├── U6 MA Restaurant Interior.png (1920x1080 Restaurant Background)
│       ├── U6_seniorsActivityBook.png (Sliced table states: SittingStraight, HandRaise, Fidgets)
│       ├── U6 MA Anu (Main Child) Fidgets & Reaction Sprite Sheet.png (Solo portraits)
│       ├── U6 MA Picture Menu Dishes.png (Dosa, Idli, Noodles, Rice, Roti, Ice Cream)
│       ├── u6 MA more waiter ravi sprites.png (10 Waiter Ravi Poses)
│       ├── U6 MA Props, Icons & Stars Sprite Sheet.png (Gold Stars, Water Glass, Cutlery)
│       ├── UI_Circle_Knob.png & UI_RoundedBox_9Slice.png (9-slice UI styling)
│       └── U6 MA Waiting Family at the Door (Table Turnover).png
├── Audio/
│   └── U6_MastersActivity_audios/
│       ├── VO_U6_01.mp3 to VO_U6_13.mp3 (Narrator Lines)
│       ├── VO_U6_ANU_1.mp3 to VO_U6_ANU_10.mp3 (Anu Dialogue Lines)
│       ├── VO_U6_ORD_*_POLITE.mp3 & VO_U6_ORD_*_BLUNT.mp3 (12 Indian Dish Order Audio Clips)
│       ├── VO_U6_ANU_WRONG_*.mp3 (6 Dynamic Wrong Dish Responses)
│       ├── VO_U6_WAIT_1.mp3 to VO_U6_WAIT_4.mp3 (Waiter Ravi Voiceover)
│       ├── VO_U6_SLIDER_WHISPER, JUSTRIGHT, BIGVOICE.mp3 ("Excuse me, Ravi!" modulated)
│       ├── VO_U6_DAD_1.mp3 & VO_U6_MUM_2.mp3 (Parent Feedback Lines)
│       └── generate_dish_audios.py (Neural Voice Generator)
├── SFX/
│   ├── AMB_Restaurant.wav & AMB_RestaurantHush.wav
│   └── SFX_*.wav (14 16-bit PCM Audio Effects: Glasses, Plates, Cutlery, Chimes, Star Bursts)
├── Docs/
│   └── Unit_6_Dining_Manners_Class3-4.docx.md (Pedagogical Curriculum Spec)
├── Scenes/
│   └── SeniorsActivity_unit6.unity
├── Scripts/
│   ├── Core/
│   │   ├── U6_GameManager_Masters_Activity.cs (State machine, ordered dish tracking, star scoring)
│   │   ├── U6_AudioManager_Masters_Activity.cs (Multi-channel audio manager with APK registry)
│   │   └── U6_Enums_Masters_Activity.cs (Waiter expressions, sound types)
│   ├── Screens/
│   │   ├── U6_LiveTableScreen_Masters_Activity.cs (Screen 1: Waiting mini-game)
│   │   ├── U6_MenuScreen_Masters_Activity.cs (Screen 2: 6-dish picture menu & ordering)
│   │   ├── U6_WaiterInteractionScreen_Masters_Activity.cs (Screen 3: 5 moments with Ravi & hand raise)
│   │   ├── U6_SliderScreen_Masters_Activity.cs (Screen 4: 3-zone voice volume slider)
│   │   └── U6_EndingScreen_Masters_Activity.cs (Screen 5: Dynamic 2 or 3 star celebration)
│   ├── UI/
│   │   ├── U6_SafeAreaConstraint.cs (Smartboard & notch hardware safe area container)
│   │   └── U6_DishItemData_Masters_Activity.cs (Data container for dishes)
│   └── Editor/
│       └── U6_SceneSetupTool_Masters_Activity.cs (Menu automation & scene generator)
├── Audio_Manifest_Unit_6.md
├── STATUS.md
├── CONTEXT.md
└── README.md
```

---

## 2. Key Architecture & Design Decisions

### 1. The Core Etiquette Problem Solved
* Unit 5 teaches home dining manners (elbows off table, napkin on lap).
* Unit 6 teaches **dining in public**:
  * Strangers and other families are sitting nearby watching and listening.
  * Food is not ready immediately—waiting patiently is required.
  * A waiter serves you—he is a human being deserving polite respect, not a servant.
  * The restaurant is shared—you must modulate your voice and leave when finished so waiting families can sit.

### 2. Family Table Hand-Raising Etiquette
* In public restaurant etiquette, snapping fingers, shouting, or banging cutlery is impolite.
* Whenever Anu needs to address or call the waiter, she gently raises her hand at the table:
  * **Moment 1 (Calling Ravi):** Shows `U6_MAct_Family_HandRaise` while Anu speaks *"Excuse me, Ravi!"*.
  * **Moment 2 (Wrong Dish):** Shows `U6_MAct_Family_HandRaise` while Anu speaks *"Sorry, I think I ordered [Dish]!"*.
  * **Moment 4 (Dropped Fork):** Shows `U6_MAct_Family_HandRaise` while Anu asks *"Excuse me, could I have another fork please?"*.
* Transitions smoothly back to `familyTableCalmSprite` (`U6_MAct_Family_SittingStraight`) once Ravi responds.

### 3. Dynamic Dish Context Architecture
* The user's chosen dish from **Screen 2 (Menu)** is stored in `U6_GameManager.Instance.SetOrderedDish(name, sprite)`.
* In **Screen 3 (Waiter Interaction)**:
  * If the wrong dish is brought in Moment 2, Anu dynamically calls out the specific dish selected by the student (e.g. *"Sorry, I think I ordered Idli!"*).
  * Audio clip length is measured dynamically (`wrongClip.length + 0.45f`) so Ravi's apology never interrupts Anu's line.
  * In Moment 3, Ravi serves the exact dish chosen by the student with matching visual icon on the table.

### 4. Zero-AssetDatabase APK Standalone Guarantees
* Unity strips `AssetDatabase` from runtime builds (Android APK, WebGL, Standalone).
* All 12 dish order audio clips (`VO_U6_ORD_*_POLITE` and `VO_U6_ORD_*_BLUNT`) are serialized directly into:
  1. `defaultDishes` list on `U6_MenuScreen`.
  2. Component-level clip fields on `U6_MenuScreen`.
  3. `sounds` registry on `U6_AudioManager`.
* Guarantees 100% audio playback in APK builds without missing references.

### 5. Dynamic Star Scoring & Ending Screen
* Stars are awarded across gameplay:
  * **Star 1:** Patience while waiting at Live Table.
  * **Star 2:** Polite response to Waiter Ravi when wrong dish arrives.
  * **Star 3:** Choosing the "Just Right" voice volume on the slider.
* The Ending Screen banner dynamically evaluates stars earned:
  * 3 Stars $\rightarrow$ `"3 Stars Earned!"` + `VO_U6_12` celebration.
  * 2 Stars $\rightarrow$ `"2 Stars Earned!"` + `VO_U6_12_2STARS` encouragement.
