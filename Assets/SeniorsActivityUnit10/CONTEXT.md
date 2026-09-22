# Context: Unit 10 "The Golden Garden"

## 1. Project Directory Structure
```
Assets/SeniorsActivityUnit10/
├── Art/
│   ├── BG_Garden_Smartboard.png (1920x1080 Storybook Garden Background)
│   ├── UI_RoundedBox_9Slice.png (128x128 9-slice card with 32px borders)
│   ├── Pot_Terracotta.png
│   ├── Plant_[Habit]_[1-6].png (48 Sprites - 8 habits x 6 stages)
│   ├── Jar_Glass_[Empty,Stage1-3,Full].png (5 Mason Jar Sprites)
│   ├── Marble_Golden.png
│   ├── Icon_[Water,Sleep,Outside,Read,Quiet,Walk,Give,Family].png (8 Circular Habit Icons)
│   ├── Certificate_Gold_Border.png
│   ├── Icon_Ribbon_Gold.png
│   ├── generate_unit10_art.py
│   └── generate_spritesheets.py
├── Audio/ & SFX/
│   ├── AMB_Garden.wav (16.0s Seamless Warm Harmonic Ambient Pad, Zero Noise Hiss)
│   ├── MUS_Garden.wav (16.0s Seamless C Major Rhodes/Marimba Music, Zero Spikes)
│   ├── MUS_EndTerm.wav (6.0s Celebratory Harvest Fanfare)
│   ├── SFX_PlantGrow.wav (1.0s Smooth Arpeggio Chime)
│   ├── SFX_Flower.wav (1.0s Gentle Bell Shimmer)
│   ├── SFX_Marble.wav (0.6s Warm Glass/Wood Marble Droplet)
│   ├── SFX_JarFull.wav (2.0s Resonant Singing Bowl Chord)
│   ├── SFX_GoldenLine.wav (1.8s Celestial Chime Melody)
│   ├── SFX_Tally.wav (0.12s Warm Woodblock Tap)
│   ├── SFX_Tap.wav (0.10s Soft Bubble UI Tap)
│   ├── generate_sfx.py (Procedural Audio Synthesizer)
│   └── generate_unit10_vo.py (Voiceover Generator)
├── Doc/
│   └── Unit_10_Golden_Life_Class3-4.docx
├── Scenes/
│   └── SeniorsActivity_unit10.unity
├── Scripts/
│   ├── Core/
│   │   ├── U10_SaveData.cs (JSON/PlayerPrefs persistence, habit definitions)
│   │   ├── U10_AudioManager_Masters_Activity.cs (Audio manager with balanced volumes)
│   │   └── U10_GameManager_Masters_Activity.cs (State machine coordinator)
│   ├── Screens/
│   │   ├── U10_SetupScreen_Masters_Activity.cs (Start of term 4-habit selector)
│   │   ├── U10_GardenScreen_Masters_Activity.cs (Main living garden board + Kindness Jar)
│   │   ├── U10_WeeklyCheckScreen_Masters_Activity.cs (2-minute weekly check-in routine)
│   │   ├── U10_GoldenLineModal_Masters_Activity.cs (Inspiring weekly quote popup)
│   │   └── U10_EndTermScreen_Masters_Activity.cs (Week 12 Harvest Celebration certificate)
│   ├── UI/
│   │   └── U10_PlantDisplayUI_Masters_Activity.cs (Pot growth stage presenter)
│   └── Editor/
│       └── U10_SceneSetupTool_Masters_Activity.cs (1-Click Scene & Canvas Generator)
├── Unit_10_Specification.md
├── Audio_Manifest_Unit_10.md
├── CONTEXT.md
└── README.md
```

## 2. Key Architecture & Design Decisions
* **Data Persistence**: `U10_SaveData.cs` automatically stores chosen habits, growth stages (1-6), marble counts, and week counters in `PlayerPrefs` (`U10_GoldenGarden_SaveData_v1`).
* **Mobile-First Readability**:
  - All text elements use **Bold typography** (`FontStyles.Bold`) with font sizes ranging from **26pt to 46pt**.
  - High-contrast color palette: Near-black forest green (`#0a220f`), rich pine (`#16331a`), and deep amber (`#2b1802`) on clean solid backgrounds.
  - Zero book page citations (e.g. `(Book Pg 34)`) or curriculum headers in student-facing cards/modals.
* **Child-Friendly & Non-Violent Design**:
  - Zero emojis and zero angle brackets (`<<` or `>>`).
  - No punishment states: Neglected habits stay in healthy green starter stages and never wilt or die.
* **Audio Comfort**:
  - `MUS_Garden.wav` and `AMB_Garden.wav` feature a **16.0-second seamless wrap-around loop** with zero audio seam pop or transient spike.
  - Smooth raised-cosine envelopes ($\ge 45\text{ms}$) across all interactive SFX to prevent ear fatigue in classroom environments.
* **Host Game Shell Navigation Clearance**:
  - **Top-Left** ($(x \in [0, 340], y \in [900, 1080])$) and **Bottom-Left** ($(x \in [0, 340], y \in [0, 180])$) screen areas are permanently reserved as empty space across ALL screens/modals.
  - This allows the parent shell game to place universal Back and Next navigation buttons without overlapping Unit 10 content.
  - Header titles are centered at Top-Center, and contextual controls (e.g. `WeekBadge`, `BackToGardenBtn`) are placed at **Top-Right**.
* **Complete 3D Terracotta Ceramic Pots**:
  - `Pot_Terracotta.png` and `Pot_Default.png` feature a full 3D rounded ceramic rim with rich dark potting soil, smooth curved shading, and a rounded base with soft contact drop shadow.
  - Rendered with `preserveAspect = true` at `(230, 190)` so pots display completely without awkward flat horizontal cuts at top or bottom.
* **1-Click Regeneration**:
  - Menu item: `Googolplex > Unit 10 > Generate Complete Scene Hierarchy`.
