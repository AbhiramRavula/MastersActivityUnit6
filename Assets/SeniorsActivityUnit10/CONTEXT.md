# Context: Unit 10 "The Golden Garden"

## 1. Project Directory Structure
```
Assets/SeniorsActivityUnit10/
├── Art/
│   ├── BG_Garden_Smartboard.png (1920x1080 Storybook Garden Background)
│   ├── U10 borad card Bg sprites.png (Sliced multiple-sprite sheet: Header plaque, Main board, Sub boards 7 & 8)
│   ├── UI_Title_Plaque_Wood.png & .meta (Carved wood & gold title board plaque)
│   ├── UI_Card_Container_Main.png & .meta (Caramel wood framed warm parchment container)
│   ├── UI_SubCard_Plant.png & .meta (Botanical mint-cream card with pedestal)
│   ├── UI_SubCard_Content.png & .meta (Ivory paper card with golden corner filigree)
│   ├── UI_Habit_Card_Normal.png & .meta (Warm recipe card for habit picker)
│   ├── UI_Bar_Footer_Parchment.png & .meta (Warm parchment summary bar)
│   ├── UI_RoundedBox_9Slice.png (128x128 9-slice card with 32px borders)
│   ├── UI_Button_[Green,Gold,Blue,Orange,Purple,Red,Grey,Disabled].png (9-Slice Tactile UI Buttons)
│   ├── Icon_Close_Circle.png (Circular Flat X Close Button)
│   ├── Pot_Terracotta.png (3D Ceramic Rounded Pot)
│   ├── Plant_[Habit]_[1-11].png (88 Sprites - 8 habits x 11 stages, leaf-first botanical growth)
│   ├── Jar_Glass_[Empty,Stage1-3,Full].png (5 Mason Jar Sprites)
│   ├── Marble_Golden.png (Golden Sphere Marble)
│   ├── Habit_Icons_Spritesheet.png (8 Circular Habit Icons: Water, Sleep, Outside, Read, Quiet, Walk, Give, Family)
│   ├── Certificate_Gold_Border.png
│   ├── Icon_Ribbon_Gold.png
│   ├── generate_improved_plants.py (88-sprite botanical generator with 75-80% leaf volume)
│   ├── generate_card_sprites.py
│   ├── generate_button_sprites.py
│   └── generate_spritesheets.py
├── Audio/ & Audio voices/ & SFX/
│   ├── VO_U10_01.mp3 to VO_U10_14.mp3 (25 EngSnap Neural TTS Voice Tracks: en-IN-NeerjaNeural)
│   ├── AMB_Garden.wav (16.0s Seamless Warm Harmonic Ambient Pad)
│   ├── MUS_Garden.wav (16.0s Seamless Calming Rhodes/Strings Music)
│   ├── MUS_EndTerm.wav (6.0s Celebratory Harvest Fanfare)
│   ├── SFX_PlantGrow.wav (1.0s Smooth Arpeggio Chime)
│   ├── SFX_Flower.wav (1.0s Gentle Bell Shimmer)
│   ├── SFX_Marble.wav (0.6s Warm Glass/Wood Marble Droplet)
│   ├── SFX_JarFull.wav (2.0s Resonant Singing Bowl Chord)
│   ├── SFX_GoldenLine.wav (1.8s Celestial Chime Melody)
│   ├── SFX_Tally.wav (0.12s Warm Woodblock Tap)
│   ├── SFX_Tap.wav (0.10s Soft Bubble UI Tap)
│   ├── generate_sfx.py (Procedural Audio Synthesizer)
│   └── generate_unit10_vo.py (EngSnap Neural Voiceover Generator)
├── Doc/
│   └── Unit_10_Golden_Life_Class3-4.docx
├── Scenes/
│   └── SeniorsActivity_unit10.unity
├── Scripts/
│   ├── Core/
│   │   ├── U10_SaveData.cs (JSON/PlayerPrefs class-level persistence, 8 habits, 12 quotes)
│   │   ├── U10_AudioManager_Masters_Activity.cs (Audio manager with Inspector & key playback)
│   │   └── U10_GameManager_Masters_Activity.cs (State machine coordinator + auto-growth handler)
│   ├── Screens/
│   │   ├── U10_SetupScreen_Masters_Activity.cs (Start of term 4-habit selector with game-like cards)
│   │   ├── U10_GardenScreen_Masters_Activity.cs (Main living garden board + Kindness Jar + dynamic weekly pulse + customClassName)
│   │   ├── U10_WeeklyCheckScreen_Masters_Activity.cs (2-minute check-in with dedicated left plant & right content sub-cards)
│   │   ├── U10_GoldenLineModal_Masters_Activity.cs (12-week quote popup modal with audio-gated "Listen" -> "Continue")
│   │   └── U10_EndTermScreen_Masters_Activity.cs (Week 12 Harvest Celebration & The Three Es Class Certificate + customClassName)
│   ├── UI/
│   │   ├── U10_PlantDisplayUI_Masters_Activity.cs (Pot presenter with separated HabitBadge and StageBadge)
│   │   └── U10_ButtonAttentionPulse_Masters_Activity.cs (Attention pop-in, pulse breathing & tactile click)
│   └── Editor/
│       └── U10_SceneSetupTool_Masters_Activity.cs (Non-destructive menu tools: card skinner, pot badge fixer, field assigner)
├── Unit_10_Specification.md
├── Audio_Manifest_Unit_10.md
├── CONTEXT.md
└── README.md
```

## 2. Key Architecture & Design Decisions
* **Data Persistence**: `U10_SaveData.cs` automatically stores chosen habits, positive growth stages (1-11), marble counts, and week counters in `PlayerPrefs` (`U10_GoldenGarden_SaveData_v1`). Zero individual child data is recorded.
* **11-Week Plant Growth Progression**:
  - Plants evolve through 11 rich botanical stages (Weeks 1 to 11), with 75–80% leaf volume and mathematically attached foliage nodes (zero floating leaves), reaching full Golden Bloom at harvest.
* **Game-Like Tactile Card UI**:
  - Replaces plain stark white backgrounds with 9-sliced storybook assets:
    - **Title Plaques**: Carved wood with golden trim and side leaves (`border: {175, 40, 175, 40}`).
    - **Main Containers**: Caramel wood frame with round corner studs (`border: {75, 75, 75, 75}`).
    - **Sub-Cards**: Botanical mint-cream cards for plants and warm ivory parchment cards for dialogue/prompts.
* **Teacher Menu Auto-Growth**:
  - Clicking "Advance to Next Week" automatically assumes all students completed all 4 chosen habits, increments `growthStage` (+1 up to 11), adds +3 kindness marbles, advances the week, and triggers immediate visual plant growth animations.
* **Dual Badge Pot Labels**:
  - **`HabitBadge`**: Displays the chosen habit name (e.g. *Drink Water*).
  - **`StageBadge`**: Displays the live growth progress (*Stage X of 11*, transitioning to *Golden Bloom* at Stage 11).
* **Audio-Gated Listening Routine**:
  - In `U10_GoldenLineModal`, the action button displays **`Listen`** in disabled muted slate while voiceover is active, then springs to **`Continue`** with pop-in attention animation once audio completes.
* **Custom Class Name Support**:
  - Both `U10_GardenScreen` and `U10_EndTermScreen` feature a serialized `customClassName` field with live `OnValidate()` editor preview.
* **The Three Es Certificate**:
  - Celebrates mastery of **Energy, Empathy, and Excellence** as a single shared class certificate.
