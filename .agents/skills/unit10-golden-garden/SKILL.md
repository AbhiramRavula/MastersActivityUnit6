---
name: unit10-golden-garden
description: Guidelines, asset pipelines, scene generation tools, and design standards for Googolplex Senior Etiquette Unit 10 ("The Golden Garden").
---

# Unit 10: The Golden Garden (Senior Etiquette)

This skill provides comprehensive instructions for maintaining, building, and extending **Unit 10: Etiquette for Golden Life ("The Golden Garden")** in Unity 2022.3.

---

## 1. Quick Scene Hierarchy Generation

Whenever UI layouts, audio files, sprites, or script references are updated, rebuild the scene hierarchy:

1. Open the Unity project.
2. In the top Unity menu bar, click:
   **`Googolplex` > `Unit 10` > `Generate Complete Scene Hierarchy`**
3. The automated editor script (`U10_SceneSetupTool_Masters_Activity.cs`) will:
   - Re-import and configure all sprites (Single, Sliced with 32px borders).
   - Build and anchor all Canvas screens (`SetupScreen`, `GardenScreen`, `WeeklyCheckScreen`, `EndTermScreen`, `GoldenLineModal`).
   - Apply bold, mobile-optimized typography (**26pt to 46pt Bold**).
   - Wire all UI and Audio clips to `[GameManager]` and `U10_AudioManager`.
   - Save the scene to `Assets/SeniorsActivityUnit10/Scenes/SeniorsActivity_unit10.unity`.

---

## 2. Child-Friendly & Accessible Design Rules

* **Zero Emojis**: Never use emoji glyphs in labels, UI text, or script strings.
* **Zero Angle Brackets**: Avoid `<` or `>` characters (e.g. do NOT use `<<` or `>>`).
* **Zero Textbook Citations in UI**: Never include `(Book Pg 34)` or curriculum metadata inside student-facing cards or modals.
* **Mobile-First High Contrast**:
  - Text sizes must be between **26pt and 46pt Bold**.
  - Always use high-contrast dark forest greens (`#0a220f`, `#16331a`) or deep amber (`#2b1802`) on light cards, and pure white on colored buttons.
  - Ensure `raycastTarget = false` on text components.
* **Positive Reinforcement**: Zero punishment or death states; neglected habits remain in starter green sprout stages.

---

## 3. Audio & Music Pipeline

Audio files are procedurally generated via Python in `Assets/SeniorsActivityUnit10/SFX/generate_sfx.py`:

* **`MUS_Garden.wav`**: 16.0-second seamless wrap-around loop in C Major ($C \to F \to Am \to G$) using warm Rhodes/marimba harmonics with zero seam clicks or spikes.
* **`AMB_Garden.wav`**: 16.0-second seamless warm ambient harmonic pad without random noise hiss.
* **SFX Envelopes**: All sound effects use smooth raised-cosine attacks ($\ge 45\text{ms}$) to prevent transient clicks on mobile speakers.

To regenerate audio assets:
```bash
python Assets/SeniorsActivityUnit10/SFX/generate_sfx.py
```

---

## 4. Screen Architecture

* **SetupScreen (`U10_SetupScreen_Masters_Activity.cs`)**: Allows selecting 4 of 8 golden habits at term start with white cards turning green on selection.
* **GardenScreen (`U10_GardenScreen_Masters_Activity.cs`)**: Living garden board with 4 growing terracotta pots, Kindness Jar (0 to 50+ marbles), and dynamic weekly pulse.
* **WeeklyCheckScreen (`U10_WeeklyCheckScreen_Masters_Activity.cs`)**: 2-minute weekly check-in routine stepping through the 4 chosen habits with show of hands (`SFX_Tally`).
* **EndTermScreen (`U10_EndTermScreen_Masters_Activity.cs`)**: Week 12 Harvest Celebration certificate honoring **The Three Es: Energy, Enthusiasm, and Empathy**.
* **GoldenLineModal (`U10_GoldenLineModal_Masters_Activity.cs`)**: Clean popup modal displaying the 12-week inspirational quotes from Section 4.

---

## 5. Host Game Navigation Clearance & Art Standards

* **Top-Left and Bottom-Left Clearance**:
  - Always leave Top-Left ($(x \in [0, 340], y \in [900, 1080])$) and Bottom-Left ($(x \in [0, 340], y \in [0, 180])$) empty on all screens so the parent game shell can place universal Back/Next navigation buttons.
  - Contextual buttons and badges (e.g. `WeekBadge`, `BackToGardenBtn`) are placed at **Top-Right** (`anchor (1f, 1f)`).
* **3D Ceramic Terracotta Pots**:
  - Pots must feature a 3D rounded ceramic rim with rich dark potting soil, smooth curved terracotta body, and a rounded base with drop shadow to prevent flat horizontal cuts on top or bottom.
  - Set `sizeDelta = (230, 190)` and `preserveAspect = true` in uGUI Image components.
