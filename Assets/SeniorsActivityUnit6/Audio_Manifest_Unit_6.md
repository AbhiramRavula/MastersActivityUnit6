# Audio Manifest — Unit 6: Eating Out / Table Etiquette

**Curriculum Unit:** Senior Etiquette — Unit 6 (Class 3 & 4)  
**Standard Format:** 16-bit 44.1kHz Stereo/Mono, Normalized, Zero-Drop 2D Playback (`spatialBlend = 0f`).  
**Asset Folder:** [`Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios/`](file:///c:/Users/abhir/SR_InterProjects/Repos/MastersActivity-Unit-6/Assets/SeniorsActivityUnit6/Audio/U6_MastersActivity_audios/) & [`Assets/SeniorsActivityUnit6/SFX/`](file:///c:/Users/abhir/SR_InterProjects/Repos/MastersActivity-Unit-6/Assets/SeniorsActivityUnit6/SFX/)  

---

## 1. Background Music & Ambient Soundscapes

| Filename | Type | Format | Duration | Description & Usage |
| :--- | :--- | :--- | :--- | :--- |
| `AMB_Restaurant.wav` | Ambience | WAV (Loop) | 16.0s | Ambient restaurant soundscape: faint chatter, subtle cutlery clatter. |
| `AMB_RestaurantHush.wav` | Ambience | WAV (Loop) | 16.0s | Quieted room tone when tables turn to look or when focusing on Waiter Ravi. |
| `MUS_Restaurant.mp3` | Music | MP3 (Loop) | 24.0s | Light, cheerful acoustic guitar/Rhodes restaurant BGM (alias: `MUS_Loop`). |
| `MUS_Win.mp3` | Fanfare | MP3 (One-shot) | 4.5s | Celebratory victory brass/xylophone fanfare on Ending Screen. |

---

## 2. Interactive Sound Effects (SFX)

All sound effects are located in [`Assets/SeniorsActivityUnit6/SFX/`](file:///c:/Users/abhir/SR_InterProjects/Repos/MastersActivity-Unit-6/Assets/SeniorsActivityUnit6/SFX/):

| Filename | Trigger Event | Description |
| :--- | :--- | :--- |
| `SFX_MenuOpen.wav` | Tap on Menu / Open Screen 2 | Paper/laminate rustle of restaurant menu opening. |
| `SFX_PadWrite.wav` | Waiter takes note | Pencil scribbling quickly on waiter's notepad. |
| `SFX_PlateDown.wav` | Ravi serves plate on table | Ceramic dish clink on wooden table surface. |
| `SFX_ForkDrop.wav` | Moment 4: Fork dropped | Metal fork falling onto restaurant tiled floor. |
| `SFX_GlassTing.wav` | Live Table: Spoon taps glass | Clear metallic chime of cutlery hitting water glass. |
| `SFX_ChairWobble.wav` | Live Table: Chair fidgeting | Wooden chair squeak when kneeling or sliding. |
| `SFX_BabyCry.wav` | Volume Slider: Big Voice | Soft baby cry from nearby table after loud shout. |
| `SFX_DoorChime.wav` | Screen transitions | Gentle shop/restaurant entrance chime bell. |
| `SFX_Bubble.wav` | Slider / Water Glasses | Playful fluid pop/bubble bounce. |
| `SFX_Sparkle.wav` | Polite choice selected | Bright magical twinkle for polite etiquette success. |
| `SFX_Star.wav` | Star awarded (Screens 1, 3, 4, 5) | Resonant golden star chime. |
| `SFX_Confetti.wav` | Ending Screen 3-star burst | Party popper sound effect. |
| `SFX_Clap.wav` | Ending Screen celebration | Warm applause from restaurant patrons. |

---

## 3. Spoken Dialogue Lines

### Narrator Voice Lines
| ID | Filename | Spoken Content |
| :--- | :--- | :--- |
| `VO_U6_01` | `VO_U6_01.mp3` | *"Today Anu’s family is eating out!"* |
| `VO_U6_02` | `VO_U6_02.mp3` | *"The food is not here yet. Watch Anu."* |
| `VO_U6_03` | `VO_U6_03.mp3` | *"Quick! Tap the button."* |
| `VO_U6_04` | `VO_U6_04.mp3` | *"Everybody is happy. One star!"* |
| `VO_U6_05` | `VO_U6_05.mp3` | *"Now choose. What will Anu eat?"* |
| `VO_U6_06` | `VO_U6_06.mp3` | *"Read first."* |
| `VO_U6_07` | `VO_U6_07.mp3` | *"How should Anu ask?"* |
| `VO_U6_08` | `VO_U6_08.mp3` | *"This is Ravi. He is looking after their table."* |
| `VO_U6_09` | `VO_U6_09.mp3` | *"Oh! That is the wrong dish."* |
| `VO_U6_10` | `VO_U6_10.mp3` | *"How loud should Anu talk here?"* |
| `VO_U6_11` | `VO_U6_11.mp3` | *"The restaurant is full now."* |
| `VO_U6_12` | `VO_U6_12.mp3` | *"Three stars! Thank you, come again!"* |
| `VO_U6_12_2STARS`| `VO_U6_12_2STARS.mp3` | *"Two stars! Well tried, thank you, come again!"* |
| `VO_U6_13` | `VO_U6_13.mp3` | *"What will you say to the waiter next time?"* |

---

### Menu Ordering Dialogue (Screen 2)
Dedicated polite and blunt order variations for all 6 dishes:

| Dish | Polite Audio ID (`politeOrderClip`) | Blunt Audio ID (`impoliteOrderClip`) | Spoken Line |
| :--- | :--- | :--- | :--- |
| **Dosa** | `VO_U6_ORD_DOSA_POLITE` | `VO_U6_ORD_DOSA_BLUNT` | *"Could I have the dosa, please?"* / *"I want dosa!"* |
| **Idli** | `VO_U6_ORD_IDLI_POLITE` | `VO_U6_ORD_IDLI_BLUNT` | *"Could I have the idli, please?"* / *"I want idli!"* |
| **Noodles** | `VO_U6_ORD_NOODLES_POLITE`| `VO_U6_ORD_NOODLES_BLUNT` | *"Could I have the noodles, please?"* / *"I want noodles!"* |
| **Rice** | `VO_U6_ORD_RICE_POLITE` | `VO_U6_ORD_RICE_BLUNT` | *"Could I have the rice, please?"* / *"I want rice!"* |
| **Roti** | `VO_U6_ORD_ROTI_POLITE` | `VO_U6_ORD_ROTI_BLUNT` | *"Could I have the roti, please?"* / *"I want roti!"* |
| **Ice Cream**| `VO_U6_ORD_ICECREAM_POLITE`| `VO_U6_ORD_ICECREAM_BLUNT`| *"Could I have the ice cream, please?"* / *"I want ice cream!"* |

---

### Waiter Interaction Dialogue (Screen 3)

| ID | Filename | Speaker | Spoken Content | Usage |
| :--- | :--- | :--- | :--- | :--- |
| `VO_U6_ANU_3` | `VO_U6_ANU_3.mp3` | Anu | *"Ummm... ummm..."* | Menu Screen when calling waiter before choosing dish |
| `VO_U6_ANU_4` | `VO_U6_ANU_4.mp3` | Anu | *"Thank you!"* | Moment 0 (Water) & Moment 3 (Food Served) |
| `VO_U6_ANU_5` | `VO_U6_ANU_5.mp3` | Anu | *"Sorry, I think I ordered..."* | Moment 2 Wrong Dish (General fallback) |
| `VO_U6_ANU_WRONG_DOSA` | `VO_U6_ANU_WRONG_DOSA.mp3` | Anu | *"Sorry, I think I ordered dosa!"* | Moment 2 when player ordered Dosa |
| `VO_U6_ANU_WRONG_IDLI` | `VO_U6_ANU_WRONG_IDLI.mp3` | Anu | *"Sorry, I think I ordered idli!"* | Moment 2 when player ordered Idli |
| `VO_U6_ANU_WRONG_NOODLES`| `VO_U6_ANU_WRONG_NOODLES.mp3`| Anu | *"Sorry, I think I ordered noodles!"* | Moment 2 when player ordered Noodles |
| `VO_U6_ANU_WRONG_RICE` | `VO_U6_ANU_WRONG_RICE.mp3` | Anu | *"Sorry, I think I ordered rice!"* | Moment 2 when player ordered Rice |
| `VO_U6_ANU_WRONG_ROTI` | `VO_U6_ANU_WRONG_ROTI.mp3` | Anu | *"Sorry, I think I ordered roti!"* | Moment 2 when player ordered Roti |
| `VO_U6_ANU_WRONG_ICECREAM`| `VO_U6_ANU_WRONG_ICECREAM.mp3`| Anu | *"Sorry, I think I ordered ice cream!"*| Moment 2 when player ordered Ice Cream |
| `VO_U6_ANU_6` | `VO_U6_ANU_6.mp3` | Anu | *"This is WRONG!"* | Moment 2 blunt shout |
| `VO_U6_ANU_8` | `VO_U6_ANU_8.mp3` | Anu | *"Excuse me, could I have another fork please?"* | Moment 4 polite fork request |
| `VO_U6_ANU_9` | `VO_U6_ANU_9.mp3` | Anu | *"I dropped my fork!"* | Moment 4 blunt fork demand |
| `VO_U6_ANU_CALL_POLITE` | `VO_U6_ANU_CALL_POLITE.mp3` | Anu | *"Excuse me, Ravi!"* | Moment 1 gentle hand raise |
| `VO_U6_ANU_CALL_SHOUT` | `VO_U6_ANU_CALL_SHOUT.mp3` | Anu | *"Hey! Over here!"* | Moment 1 shouting across room |
| `VO_U6_ANU_EAT_FAST` | `VO_U6_ANU_EAT_FAST.mp3` | Anu | *"Yum! Give me that!"* | Moment 3 grabbing food hastily |
| `VO_U6_WAIT_1` | `VO_U6_WAIT_1.mp3` | Ravi | *"Certainly! One moment please."* | Waiter accepts order |
| `VO_U6_WAIT_2` | `VO_U6_WAIT_2.mp3` | Ravi | *"Of course, I am coming right over!"* | Waiter responds to polite call |
| `VO_U6_WAIT_3` | `VO_U6_WAIT_3.mp3` | Ravi | *"Oh! My apologies, I will bring your food right away!"* | Waiter warms up & fixes wrong dish |
| `VO_U6_WAIT_4` | `VO_U6_WAIT_4.mp3` | Ravi | *"I will get a fresh one for you right now."* | Waiter replaces fork |

---

### Volume Slider Dialogue (Screen 4)

| ID | Filename | Speaker | Spoken Content | Audio Tone |
| :--- | :--- | :--- | :--- | :--- |
| `VO_U6_SLIDER_WHISPER` | `VO_U6_SLIDER_WHISPER.mp3` | Anu | *"Excuse me, Ravi!"* | Whispered (-14dB, high frequency breath) |
| `VO_U6_SLIDER_JUSTRIGHT` | `VO_U6_SLIDER_JUSTRIGHT.mp3` | Anu | *"Excuse me, Ravi!"* | Warm, clear indoor dining volume (0dB) |
| `VO_U6_SLIDER_BIGVOICE` | `VO_U6_SLIDER_BIGVOICE.mp3` | Anu | *"Excuse me, Ravi!"* | Loud resonant shout (+6dB with room reverb) |
| `VO_U6_DAD_1` | `VO_U6_DAD_1.mp3` | Dad | *"Sorry? I cannot hear you at all."* | Confused father whispering back |
| `VO_U6_MUM_2` | `VO_U6_MUM_2.mp3` | Mum | *"Anu, indoor voice, please."* | Gentle mother guidance after loud voice |
