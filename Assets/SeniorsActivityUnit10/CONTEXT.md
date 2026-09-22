# Context: Unit 10 "The Golden Garden"

## 1. Project Directory Structure
```
Assets/SeniorsActivityUnit10/
├── Art/
│   ├── BG_Garden_Smartboard.png (1920x1080 Storybook Garden Background)
│   ├── UI_RoundedBox_9Slice.png (128x128 9-slice card with 32px borders)
│   ├── UI_Button_[Green,Gold,Blue,Orange,Purple,Red,Grey,Disabled].png (9-Slice Tactile UI Buttons)
│   ├── Icon_Close_Circle.png (Circular Flat X Close Button)
│   ├── Pot_Terracotta.png (3D Ceramic Rounded Pot)
│   ├── Plant_[Habit]_[1-6].png (48 Sprites - 8 habits x 6 stages)
│   ├── Jar_Glass_[Empty,Stage1-3,Full].png (5 Mason Jar Sprites)
│   ├── Marble_Golden.png (Golden Sphere Marble)
│   ├── Icon_[Water,Sleep,Outside,Read,Quiet,Walk,Give,Family].png (8 Circular Habit Icons)
│   ├── Certificate_Gold_Border.png
│   ├── Icon_Ribbon_Gold.png
│   ├── generate_unit10_art.py
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
│   │   └── U10_GameManager_Masters_Activity.cs (State machine coordinator)
│   ├── Screens/
│   │   ├── U10_SetupScreen_Masters_Activity.cs (Start of term 4-habit selector with white/green cards)
│   │   ├── U10_GardenScreen_Masters_Activity.cs (Main living garden board + Kindness Jar + dynamic weekly pulse)
│   │   ├── U10_WeeklyCheckScreen_Masters_Activity.cs (2-minute weekly check-in with audio-gating & show-of-hands tally)
│   │   ├── U10_GoldenLineModal_Masters_Activity.cs (12-week inspiring Golden Quote popup modal)
│   │   └── U10_EndTermScreen_Masters_Activity.cs (Week 12 Harvest Celebration & The Three Es Class Certificate)
│   ├── UI/
│   │   ├── U10_PlantDisplayUI_Masters_Activity.cs (Pot growth stage presenter)
│   │   └── U10_ButtonAttentionPulse_Masters_Activity.cs (Attention pop-in, pulse breathing & tactile click)
│   └── Editor/
│       └── U10_SceneSetupTool_Masters_Activity.cs (Non-destructive Inspector assigner, button skinner & scene builder)
├── Unit_10_Specification.md
├── Audio_Manifest_Unit_10.md
├── CONTEXT.md
└── README.md
```

## 2. Key Architecture & Design Decisions
* **Data Persistence**: `U10_SaveData.cs` automatically stores chosen habits, positive growth stages (1-6), marble counts, and week counters in `PlayerPrefs` (`U10_GoldenGarden_SaveData_v1`). Zero individual child data is recorded.
* **Child-Centric Intuitive Visual Guidance**:
  - **Dynamic Attention Guidance**: `U10_ButtonAttentionPulse_Masters_Activity` gives primary action buttons an elastic pop-in bounce and continuous breathing pulse ($1.0 \leftrightarrow 1.045\times$).
  - **Dynamic Action Labels**: Weekly check button dynamically displays `Start Week X Check-In!`.
  - **Tactile 9-Slice Buttons**: Vibrant, kid-friendly 9-sliced button assets (`UI_Button_Green`, `UI_Button_Gold`, etc.) with drop shadows and top gloss highlights.
* **Audio-Gated Listening Routine**:
  - In `U10_WeeklyCheckScreen` and `U10_GoldenLineModal`, proceed/skip buttons are locked during spoken narration and smoothly unlock and pop-in once the voice track finishes.
* **Non-Destructive Scene Integration**:
  - Tools under `Googolplex > Unit 10 > Non-Destructive > ...` allow the developer to skin buttons, attach close icons, and populate all Inspector serialized fields without overwriting manual scene hierarchy adjustments.
* **Audio Standard**:
  - All 25 voice tracks are generated using the project's standard **EngSnap Neural TTS** (`en-IN-NeerjaNeural`, calm `-5%` rate).
* **The Three Es Certificate**:
  - Celebrates mastery of **Energy, Enthusiasm, and Empathy** as a single shared class certificate.
```
