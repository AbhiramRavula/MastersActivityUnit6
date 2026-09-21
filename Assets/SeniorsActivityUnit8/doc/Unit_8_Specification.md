# Unit 8: Washroom Etiquette — Complete Design Specification

**Curriculum:** Googolplex Communicative English — Senior Etiquette  
**Book Pages:** 32–33  
**Target Group:** Class 3 & Class 4 (Ages 8–9)  
**Game Title:** "AFTER YOU"  

---

## 1. Pedagogical Scope & Tone Constraints

### 1.1 Inclusions from Book
- **Flush the toilet after use** (Pages 32, 33) $\rightarrow$ Part 2 & Part 3 consequence.
- **Wash hands to prevent spread of colds and flu** (Page 33) $\rightarrow$ Handwash Screen (20-second scrub).
- **Do not spill water on the floor** (Page 32) $\rightarrow$ Part 3 wet floor slip sequence.
- **Use the bin to discard waste / paper towels** (Pages 32, 33) $\rightarrow$ Part 2 & Part 3 towel discard.
- **Wipe off the sink for the next user** (Page 33) $\rightarrow$ The core premise of the entire activity.
- **Use water and paper towels conservatively / turn off tap** (Page 33) $\rightarrow$ Part 2 tap shutoff.
- **Notify management / tell teacher when soap is empty** (Page 33) $\rightarrow$ Part 4 choice.
- **Knocking before entering a closed door** (Page 29 callback) $\rightarrow$ Part 1 door interaction.

### 1.2 Deliberate Exclusions
- *Adult / sensitive topics (sanitary napkins, mobile phones, toilet seat squatting)* are excluded to keep the lesson age-appropriate, positive, and focused on cleanliness and communal consideration.
- *Gross-out humor prohibition:* No toilet humor or embarrassing depictions. The comedy and learning come exclusively from the wet floor slip and environmental cleanliness contrasts. Nobody is ever shown inside a cubicle.

---

## 2. Comprehensive Screen & Flow Breakdown

### Part 1 — The Door (1 Minute, 0 Stars)
- Closed corridor door with "SHUT" sign.
- **Choice 1: Knock and wait** (Correct) $\rightarrow$ Plays `SFX_Knock`, door opens, previous occupant exits, Anu walks in.
- **Choice 2: Bang on door loudly** (Incorrect) $\rightarrow$ Plays `SFX_BangDoor`, inside voice says `"Just a minute!"` (`VO_U8_VOICE_1`), Anu looks sheepish.
- **Choice 3: Push it open** (Incorrect) $\rightarrow$ Plays `SFX_BoltRattle`, door is bolted, shows patience is needed.

### Part 2 — Inside the Washroom (6 Minutes, 1 Star)
- Side-on 2D washroom view (Cubicle $\rightarrow$ Sink & Tap & Soap $\rightarrow$ Towels & Bin) with dedicated step button card.
- Sequence of 7 actions:
  1. **Lock Cubicle Door:** `SFX_BoltClick` (Mandatory before proceeding).
  2. **Flush Toilet:** `SFX_Flush` (Skipping causes unflushed toilet in Part 3).
  3. **Step Out of Cubicle:** Door swings open, Anu walks to sink.
  4. **Wash Hands:** Triggers **Handwash Screen** (20-second scrub, bubbles, germs slide off). Awards Star 1.
  5. **Turn Tap Off:** `SFX_TapOff` (Skipping leaves tap running in Part 3).
  6. **Throw Towel in Bin:** `SFX_TowelPull` $\rightarrow$ `SFX_BinDrop` (Skipping drops soggy towel on floor). Parabolic toss trajectory.
  7. **Wipe Sink:** `SFX_Wipe` (Skipping leaves splashed wet sink and floor puddle).
- **Intuitive Step Navigation:** Active button pops with gentle pulse animation (`alpha = 1.0`), while inactive and completed steps are non-interactable without dimming.

### Dedicated Handwash Screen
- Close-up of 2 hands under water stream with soap and bubbles.
- 20-second timer matching full duration of `MUS_HandwashSong`.
- **8 Friendly Germ Blobs:** Placed across real handwashing zones (fingertips, palms, thumbs, between fingers, wrists).
- Tapping/swiping rubs hands, builds bubbles, and wiggles active germs.
- Germs clear gradually and evenly across the 20 seconds (~1 germ every 2.5s).
- If tapping stops before 20s, shows `"Keep going! Scrub the germs away!"` with `VO_U8_07`.
- Finishes with sparkle bloom (`SFX_Sparkle`), awards Star 1, and returns automatically to Part 2.

### Part 3 — After You (3 Minutes, 1 Star)
- Anu walks out $\rightarrow$ 3-second pause $\rightarrow$ Meera walks in.
- **If all steps passed:** Meera enters dry, clean room, uses sink with a smile, leaves happy. Star 2 awarded.
- **If steps skipped:**
  - Wet floor: Shoe hits puddle, slides, flails arms, catches herself on sink (`SFX_Slip`, `VO_U8_MEE_2`).
  - Splashed sink: Wipes sink with unimpressed expression; splashed overlay persists.
  - Running tap: Turns off forgotten running tap.
  - Soggy towel: Visible on floor next to bin.
  - Unflushed toilet: Opens cubicle, stops, closes door with a sigh.
  - Classroom is given **"TRY AGAIN"** button to replay Part 2 and succeed.

### Part 4 — Empty Soap! (1 Minute, 1 Star)
- Meera pumps soap dispenser $\rightarrow$ `SFX_SoapPump` $\rightarrow$ nothing comes out $\rightarrow$ `SFX_SoapEmpty`.
- **Choice 1: Go and tell a teacher** (Correct) $\rightarrow$ Informs teacher (`VO_U8_MEE_1`), dispenser visibly refills (`SPR_Soap_Full`), sparkles (`SFX_Sparkle`), two other students use it. Awards Star 3.
- **Choice 2: Just leave** (Lesson outcome) $\rightarrow$ Shrugs and leaves. 3 students enter, try in frustration, and leave empty-handed.

### Ending Celebration & Discussion (1 Minute)
- 3 Gold Stars reveal from sprite sheet (`SPR_Icon_GoldStar`, `SFX_Star`), Confetti (`SFX_Confetti`), Win music (`MUS_Win`).
- **Core Reflection Question:**
  > **"Would the next person be happy?"**
