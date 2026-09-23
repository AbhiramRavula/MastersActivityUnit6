# Unit 10: Etiquette For Golden Life ("The Golden Garden")

**Senior Etiquette (Class 3 & 4 | Ages 8–9)**

---

## 1. Quick Setup in Unity Editor

1. Open the Unity Project (`Unity 2022.3.62f3`).
2. In the top Unity menu, click:
   **`Googolplex` > `Unit 10` > `Generate Complete Scene Hierarchy`**
   *(or use **`Googolplex` > `Unit 10` > `Non-Destructive` > `Apply Game-Like Card and Plaque Sprites`** to update an existing scene)*.
3. The automated scene tools will:
   - Configure all Sprite textures (Single, Sliced with 9-slice borders) and Audio assets.
   - Apply game-like carved wood plaques, warm parchment containers, and botanical leaf-accented sub-cards (`U10 borad card Bg sprites.png`).
   - Build the complete Canvas, UI panels, buttons, icons, Kindness Jar, and plant displays.
   - Apply bold, high-contrast, mobile-optimized typography (**26pt to 46pt Bold**).
   - Wire all references to `[GameManager]` and `U10_AudioManager`.
   - Save the ready-to-run scene at `Assets/SeniorsActivityUnit10/Scenes/SeniorsActivity_unit10.unity`.
4. Press **Play** in Unity to run the activity!

---

## 2. Who Plays This Game?

* **Teacher-Operated**: The teacher displays the activity on a classroom Smartboard (or mobile tablet), facilitates the 2-minute weekly check-in routine, advances the weeks, and drops golden marbles for observed kindness.
* **Collective Student Participation**: Students actively participate as a class by a **show of hands** to report their weekly practice of the 4 chosen habits and reciting the weekly **Golden Line**.
* **Auto-Growth on Teacher Advance**: When teachers use the Teacher Menu to "Advance to Next Week", all 4 plants automatically advance (+1 stage up to 11), marble counts increase (+3), and the garden view animates growth immediately.

---

## 3. Four Core Activity Screens

1. **Setup Screen (Term Habit Selection)**:
   - Choose 4 Golden Habits for the 12-week term out of 8 available options (Drink Water, Sleep Early, Play Outside, Read a Book, Sit Quietly, Walk and Smile, Give with Heart, Family Time).
   - Game-like botanical cards (`sub board 7`) with leaf corners, cushioned text padding, bold quotes, and gold checkmark ribbons.
   - Warm parchment selection bar (`sub board 8`) tracking chosen habits.
2. **Garden Screen (Living Etiquette Board)**:
   - 4 terracotta pots with plants growing across **11 developmental stages** (Sprout $\to$ Stems $\to$ Bushy Foliage $\to$ Buds $\to$ Partial Flowers $\to$ Golden Bloom).
   - **Dual Badge Pot Labels**:
     - **Top Badge (`HabitBadge`)**: Displays the habit name (e.g. *Drink Water*, *Sleep Early*).
     - **Bottom Badge (`StageBadge`)**: Displays the live growth progress (*Stage 1 of 11* $\to$ *Golden Bloom*).
   - **Kindness Jar**: Interactive marble counter with 5 fill stages and gentle chime audio.
   - **Custom Class Name**: Configurable via `customClassName` in the Inspector with live `OnValidate()` preview.
   - Navigation bar with quick access to Weekly Check, Golden Quote, Teacher Menu, and Harvest Celebration.
3. **Weekly Check Screen (2-Minute Class Check-in)**:
   - Two-column card container:
     - **Left Sub-Card**: Dedicated botanical mint-cream card displaying the plant and pot.
     - **Right Sub-Card**: Dedicated ivory parchment card holding the habit badge, question prompt, quote, and action buttons with high-contrast text.
   - Large bold prompt text with `"Yes, We Practiced!"` and `"Next Habit"` buttons.
4. **End of Term Screen (Harvest Celebration)**:
   - Week 12 celebration awarding the **Certificate of Etiquette Excellence**.
   - Displays all 4 harvested golden blooms, the filled kindness jar, and summary statistics.
   - Commemorates the mastery of **The Three Es: Energy, Empathy, and Excellence**.

---

## 4. Child-Friendly & Accessible Design Standards

- **Zero Emojis & Zero Angle Brackets (`<<` or `>>`)**: Clean typography and custom artwork only.
- **No Textbook Citations in Game UI**: Removed all page numbers and textbook titles from student-facing cards and modals.
- **Game-Like Tactile Art**: No plain stark white cards; all boards, plaques, and cards use warm storybook parchment, carved honey oak frames, and leafy flourishes with 9-slice borders to prevent stretching.
- **Large High-Contrast Typography**: 26pt to 46pt bold font sizing ensuring crystal-clear readability on phones, tablets, and smartboards.
- **Audio-Gated Listening Routine**: Golden Line modal button displays `"Listen"` while quote audio plays, smoothly transitioning to `"Continue"` once finished.
- **Positive Reinforcement**: Zero punishment mechanics; plants never wilt or die.
- **Audio Comfort**: Smooth 16.0-second seamless looping background music in C Major with zero sharp attack spikes or noise hiss.
