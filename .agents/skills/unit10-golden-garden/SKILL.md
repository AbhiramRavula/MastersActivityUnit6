---
name: unit10-golden-garden
description: Guidelines, asset pipelines, scene generation tools, and design standards for Googolplex Senior Etiquette Unit 10 ("The Golden Garden").
---

# Unit 10: The Golden Garden (Senior Etiquette)

This skill provides comprehensive instructions for maintaining, building, and extending **Unit 10: Etiquette for Golden Life ("The Golden Garden")** in Unity 2022.3.

---

## 1. Scene Generation & Non-Destructive Menu Actions

Whenever UI layouts, audio files, sprites, or script references are updated:

1. **Full Generation**:
   `Googolplex > Unit 10 > Generate Complete Scene Hierarchy`
   - Re-imports and configures all sprites and audio assets.
   - Builds Canvas screens (`SetupScreen`, `GardenScreen`, `WeeklyCheckScreen`, `EndTermScreen`, `GoldenLineModal`).
   - Wires all serialized references to `[GameManager]` and `U10_AudioManager`.
   - Saves to `Assets/SeniorsActivityUnit10/Scenes/SeniorsActivity_unit10.unity`.

2. **Non-Destructive Scene Updates (Preserves Manual Transforms)**:
   - **`Apply Game-Like Card and Plaque Sprites`**: Replaces plain white backgrounds with 9-sliced storybook plaques, wooden containers, and botanical sub-cards from `U10 borad card Bg sprites.png`.
   - **`Fix Pot Labels and Badges`**: Formats the dual pot labels (`HabitBadge` = Habit Title, `StageBadge` = `Stage 1 of 11`).
   - **`Skin All Buttons In Active Scene`**: Applies kid-friendly 3D pill button sprites and attention pulses.
   - **`Assign All Inspector Fields In Active Scene`**: Connects VO clips, sprites, and screen references without rebuilding game objects.

---

## 2. Child-Friendly & Accessible Design Rules

* **Zero Emojis**: Never use emoji glyphs in labels, UI text, or script strings.
* **Zero Angle Brackets**: Avoid `<` or `>` characters (e.g. do NOT use `<<` or `>>`).
* **Zero Textbook Citations in UI**: Never include `(Book Pg 34)` or curriculum metadata inside student-facing cards or modals.
* **Game-Like Tactile Aesthetics**:
  - Avoid stark, flat white card backgrounds. Use warm storybook parchment, caramel wood borders, and botanical leaf-trimmed sub-cards.
  - Sliced sprites must have exact 9-slice borders configured in `.meta` files (e.g. `{175, 40, 175, 40}` on plaques, `{75, 75, 75, 75}` on boards) to prevent corner leaf stretching.
* **Mobile-First High Contrast**:
  - Text sizes must be between **26pt and 46pt Bold**.
  - Always use high-contrast dark forest greens (`#0a220f`, `#16331a`) or deep charcoal/espresso (`#1f242e`) on light cards, and pure white on colored buttons.
  - Ensure `raycastTarget = false` on text components.
* **Positive Reinforcement**: Zero punishment or death states; neglected habits remain in starter green sprout stages.

---

## 3. Plant Growth Pipeline (11 Stages)

Plants evolve through 11 rich, leaf-first developmental stages across Weeks 1 to 11 (reaching full Golden Bloom at Week 12 Harvest):
* Generated via `Assets/SeniorsActivityUnit10/Art/generate_improved_plants.py` (88 PNG sprites total: 8 habits $\times$ 11 stages).
* **75–80% Leaf Density**: Plants focus on organic botanical foliage attached directly to stem nodes (zero floating leaves).
* Flowers remain proportionate botanical crowns at maturity without overwhelming foliage.

---

## 4. Audio & Music Pipeline

Audio files are procedurally generated via Python in `Assets/SeniorsActivityUnit10/SFX/generate_sfx.py`:

* **`MUS_Garden.wav`**: 16.0-second seamless wrap-around loop in C Major ($C \to F \to Am \to G$) using warm Rhodes/marimba harmonics with zero seam clicks or spikes.
* **`AMB_Garden.wav`**: 16.0-second seamless warm ambient harmonic pad without random noise hiss.
* **SFX Envelopes**: All sound effects use smooth raised-cosine attacks ($\ge 45\text{ms}$) to prevent transient clicks on mobile speakers.

To regenerate audio assets:
```bash
python Assets/SeniorsActivityUnit10/SFX/generate_sfx.py
```

---

## 5. Screen Architecture & Core Mechanics

* **SetupScreen (`U10_SetupScreen_Masters_Activity.cs`)**: Allows selecting 4 of 8 golden habits at term start with game-like botanical cards.
* **GardenScreen (`U10_GardenScreen_Masters_Activity.cs`)**:
  - 4 growing terracotta pots with dual badges: `HabitBadge` (Habit Name) and `StageBadge` (`Stage X of 11` $\to$ `Golden Bloom`).
  - Kindness Jar (0 to 50+ marbles) with fill states.
  - Teacher Menu with **"Advance to Next Week"** that automatically auto-checks in all students for all 4 habits (+1 growth, +3 marbles, totalCheckins++) and animates growth.
  - `customClassName` field with live `OnValidate()` inspector preview.
* **WeeklyCheckScreen (`U10_WeeklyCheckScreen_Masters_Activity.cs`)**:
  - Two-column card container: Left `PlantSubCard` (mint-cream botanical card) and Right `ContentSubCard` (warm ivory paper card with golden corner filigree).
  - 2-minute weekly check-in routine stepping through the 4 chosen habits with show of hands.
* **GoldenLineModal (`U10_GoldenLineModal_Masters_Activity.cs`)**:
  - Clean popup modal displaying 12-week inspiring quotes.
  - Audio-gated action button: displays **`Listen`** in disabled muted slate while audio plays, transitioning to **`Continue`** with spring pop-in animation once audio ends.
* **EndTermScreen (`U10_EndTermScreen_Masters_Activity.cs`)**:
  - Week 12 Harvest Celebration certificate honoring **The Three Es: Energy, Empathy, and Excellence**.
  - Live `customClassName` support.

---

## 6. Host Game Navigation Clearance & Art Standards

* **Top-Left and Bottom-Left Clearance**:
  - Always leave Top-Left ($(x \in [0, 340], y \in [900, 1080])$) and Bottom-Left ($(x \in [0, 340], y \in [0, 180])$) empty on all screens so the parent game shell can place universal Back/Next navigation buttons.
  - Contextual buttons and badges (e.g. `WeekBadge`, `BackToGardenBtn`) are placed at **Top-Right** (`anchor (1f, 1f)`).
* **3D Ceramic Terracotta Pots**:
  - Pots feature a 3D rounded ceramic rim with rich dark potting soil, smooth curved terracotta body, and a rounded base with drop shadow.
  - Set `sizeDelta = (230, 190)` and `preserveAspect = true` in uGUI Image components.
