import os
import win32com.client
import shutil

AUDIO_DIR = os.path.dirname(os.path.abspath(__file__))
VOICES_DIR = os.path.join(AUDIO_DIR, "Audio voices")
SFX_DIR = os.path.join(os.path.dirname(AUDIO_DIR), "SFX")

# Exactly 25 VO files from Section 7.1 of Unit 10 curriculum specification
VOICE_LINES = [
    ("VO_U10_01.wav", "This is our golden garden."),
    ("VO_U10_02.wav", "Choose four things to look after this term."),
    ("VO_U10_03.wav", "Time for our weekly check!"),
    ("VO_U10_04.wav", "Who drank their water most days this week?"),
    ("VO_U10_05.wav", "Who went to bed early most days?"),
    ("VO_U10_06.wav", "Who played outside this week?"),
    ("VO_U10_07.wav", "Who read a book this week?"),
    ("VO_U10_08.wav", "Who sat quietly for ten minutes?"),
    ("VO_U10_09.wav", "Who went for a walk?"),
    ("VO_U10_10.wav", "Who gave something to somebody?"),
    ("VO_U10_11.wav", "Who spent time with their family?"),
    ("VO_U10_12.wav", "Look how it is growing!"),
    # 12 Golden Lines (read slowly and clearly)
    ("VO_U10_13_01.wav", "The Golden Life begins with the simple things you do every day."),
    ("VO_U10_13_02.wav", "Water gives life to plants, and water gives energy to you."),
    ("VO_U10_13_03.wav", "Early to bed brings bright thoughts in the morning."),
    ("VO_U10_13_04.wav", "Sunshine and fresh air help young minds grow strong."),
    ("VO_U10_13_05.wav", "A book is a garden carried in your pocket."),
    ("VO_U10_13_06.wav", "In quiet moments, we find our greatest peace."),
    ("VO_U10_13_07.wav", "A smiling face brings sunshine to everyone you meet."),
    ("VO_U10_13_08.wav", "Giving with a happy heart makes the whole world brighter."),
    ("VO_U10_13_09.wav", "Kindness is a seed that blooms in every heart."),
    ("VO_U10_13_10.wav", "Family love is the deepest root in our garden."),
    ("VO_U10_13_11.wav", "Good habits today become great character tomorrow."),
    ("VO_U10_13_12.wav", "Excellence is not an act, but a habit. Energy, Empathy, and Excellence."),
    ("VO_U10_14.wav", "Look at our garden. You grew all of that.")
]

def generate_all_vo():
    speaker = win32com.client.Dispatch("SAPI.SpVoice")
    # Rate -1 for clear educational articulation
    speaker.Rate = -1
    speaker.Volume = 100

    os.makedirs(VOICES_DIR, exist_ok=True)
    os.makedirs(AUDIO_DIR, exist_ok=True)
    os.makedirs(SFX_DIR, exist_ok=True)

    for fname, text in VOICE_LINES:
        out_path = os.path.join(VOICES_DIR, fname)
        stream = win32com.client.Dispatch("SAPI.SpFileStream")
        stream.Open(out_path, 3) # 3 = SSFMCreateForWrite
        speaker.AudioOutputStream = stream
        speaker.Speak(text)
        stream.Close()
        print(f"Generated VO: {fname} -> '{text}'")

        # Also copy to root Audio and SFX folder for backward compatibility
        shutil.copy2(out_path, os.path.join(AUDIO_DIR, fname))
        shutil.copy2(out_path, os.path.join(SFX_DIR, fname))

    print(f"All {len(VOICE_LINES)} Unit 10 Voiceover Lines Generated Successfully into '{VOICES_DIR}'!")

if __name__ == "__main__":
    generate_all_vo()

