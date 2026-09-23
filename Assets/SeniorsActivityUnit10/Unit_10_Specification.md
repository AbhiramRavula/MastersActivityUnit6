# Unit 10 Specification: Senior Etiquette -- Etiquette for Golden Life ("The Golden Garden")

### 1. Overview & Pedagogical Objective
* **Unit Title**: Etiquette for Golden Life (The Golden Garden)
* **Target Audience**: Class 3 & 4 (Ages 8 to 9)
* **Type**: Teacher-Led Living Class Board & Continuous Etiquette Tracker
* **Session Length**: 2-minute weekly check-in routine + daily Kindness Jar check-in
* **Pedagogical Philosophy**:
  1. **Zero Individual Tracking**: Whole-class progress only; no student is ever singled out or put on the spot.
  2. **Positive Reinforcement**: Neglected habits stay as small green sprouts; plants only grow forward with practice and never wilt or die.
  3. **The Three Es**: Energy, Empathy, and Excellence.

---

### 2. The 8 Golden Habits (Select 4 per Term)
The class selects 4 focus habits at the beginning of each 12-week term:

| # | Habit | Key | Plant Visual | Class Quote / Guidance |
|---|---|---|---|---|
| 1 | **Drink Water** | `Water` | Dewdrop Lily | *"Drink your water -- inside and outside."* |
| 2 | **Sleep Early** | `Sleep` | Night Lavender | *"Sleep early, wake up fresh and ready."* |
| 3 | **Play Outside** | `Outside` | Sun Blossom | *"Play outside -- run under the open sky."* |
| 4 | **Read a Book** | `Read` | Wise Orchid | *"Read a book -- every page holds a wonder."* |
| 5 | **Sit Quietly** | `Quiet` | Peace Lotus | *"Sit quietly for ten minutes. Listen to the calm."* |
| 6 | **Walk and Smile** | `Walk` | Morning Glory | *"Walk with good posture, and greet others with a smile."* |
| 7 | **Give with Heart** | `Give` | Heart Bloom | *"Give something to somebody with a happy heart."* |
| 8 | **Family Time** | `Family` | Tree of Life | *"Spend time with your family every single day."* |

---

### 3. Screen Structure & Flow

```mermaid
graph TD
    A["Screen 0: Setup Screen"] -->|"Select 4 Habits & Plant"| B["Screen 1: The Golden Garden"]
    B -->|"Daily Good Deeds"| C["Kindness Jar +1 Marble"]
    B -->|"Weekly Check-in Button"| D["Screen 2: Weekly Check-in"]
    D -->|"Step through 4 Habits"| E["Golden Line Modal"]
    E -->|"Advance Week & Return"| B
    B -->|"Week 12 Harvest"| F["Screen 3: End of Term Celebration"]
    F -->|"Reset for New Term"| A
```

#### Screen 0: Setup Screen (`U10_SetupScreen_Masters_Activity.cs`)
* Displayed once at term beginning.
* 8 habit cards in a 2x4 grid with icons and bold guidance quotes.
* Select exactly 4 habits.
* Clicking **"Plant Our Garden"** initializes the 4 pots and persists save data to `U10_SaveData`.

#### Screen 1: The Golden Garden (`U10_GardenScreen_Masters_Activity.cs`)
* **Header**: "The Golden Garden", "Week X of 12", Class Name.
* **4 Terracotta Pots**: Displays the 4 chosen plants evolving progressively across 11 weeks (Seedling at Week 1 to Full Golden Bloom at Week 11).
* **Pot Labels**: Displays the habit name (e.g. `WATER`, `SLEEP`, `READ`, `GIVE`) with rounded container pill, displaying `Golden Bloom` at full growth (no artificial numerical "Stage X of 6" counters as per Section 2.2).
* **Kindness Jar**: Right-hand mason jar with dynamic marble count (0 to 50+). Tap **"Add Kindness Marble"** on observed good deeds.
* **Navigation Bar**:
  * `Weekly Check (2 Min)`: Launches the 2-minute class routine.
  * `Golden Quote`: Displays this week's quote modal.
  * `Teacher Menu`: Centered popup overlay modal with **"Advance to Next Week"**, **"Reset for New Term"**, and **"Close Menu"**.
  * `Harvest Celebration`: Activates at Week 12.

#### Screen 2: Weekly Check-In Screen (`U10_WeeklyCheckScreen_Masters_Activity.cs`)
* 4-step interactive routine:
  1. Teacher reads habit question (e.g. *"Who drank fresh water every day this week?"*).
  2. Ask for show of hands.
  3. Tap **"Yes, We Practiced!"** to advance growth stage and play chime.
  4. Tap **"Next Habit"** to move to the next habit.
  5. After the 4th habit, pops up the Golden Line Modal with the week's inspirational quote.

#### Screen 3: End of Term Harvest Screen (`U10_EndTermScreen_Masters_Activity.cs`)
* Week 12 harvest celebration:
  * Golden Class Certificate: **THE THREE Es: ENERGY · ENTHUSIASM · EMPATHY** (Awarded to CLASS ______ for growing a golden garden).
  * Summary tally of weeks completed, kindness marbles collected, and golden blooms harvested.

---

### 4. Golden Lines of the Week (12-Week Rotation)
1. **Week 1**: *"Every day is a new chance."*
2. **Week 2**: *"If today was not good, tomorrow can still be better."*
3. **Week 3**: *"You do not have to be like anybody else."*
4. **Week 4**: *"Say something kind about somebody today."*
5. **Week 5**: *"Give something to somebody. It can be very small."*
6. **Week 6**: *"Some days are not fair. It is still a good life."*
7. **Week 7**: *"Spend some time with your family today."*
8. **Week 8**: *"Nothing stays the same forever."*
9. **Week 9**: *"If somebody is sad, sit with them."*
10. **Week 10**: *"Do the thing you have been putting off."*
11. **Week 11**: *"Be excited about something small today."*
12. **Week 12**: *"Look at our garden. You grew all of that."*
