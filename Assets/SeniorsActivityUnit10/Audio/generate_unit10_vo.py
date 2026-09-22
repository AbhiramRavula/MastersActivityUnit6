import asyncio
import edge_tts
import os
import shutil

AUDIO_DIR = os.path.dirname(os.path.abspath(__file__))
VOICES_DIR = os.path.join(AUDIO_DIR, "Audio voices")
SFX_DIR = os.path.join(os.path.dirname(AUDIO_DIR), "SFX")

# EngSnap standard educator neural voice
VOICE_NEURAL = "en-IN-NeerjaNeural"

VOICE_LINES = [
    # 7.1 Spoken lines (Narrator / Teacher)
    ("VO_U10_01.mp3", "This is our golden garden."),
    ("VO_U10_02.mp3", "Choose four things to look after this term."),
    ("VO_U10_03.mp3", "Time for our weekly check!"),
    ("VO_U10_04.mp3", "Who drank their water most days this week?"),
    ("VO_U10_05.mp3", "Who went to bed early most days?"),
    ("VO_U10_06.mp3", "Who played outside this week?"),
    ("VO_U10_07.mp3", "Who read a book this week?"),
    ("VO_U10_08.mp3", "Who sat quietly for ten minutes?"),
    ("VO_U10_09.mp3", "Who went for a walk?"),
    ("VO_U10_10.mp3", "Who gave something to somebody?"),
    ("VO_U10_11.mp3", "Who spent time with their family?"),
    ("VO_U10_12.mp3", "Look how it is growing!"),
    # 12 Golden Lines (Section 4: The Twelve Lines, in order)
    ("VO_U10_13_01.mp3", "Every day is a new chance."),
    ("VO_U10_13_02.mp3", "If today was not good, tomorrow can still be better."),
    ("VO_U10_13_03.mp3", "You do not have to be like anybody else."),
    ("VO_U10_13_04.mp3", "Say something kind about somebody today."),
    ("VO_U10_13_05.mp3", "Give something to somebody. It can be very small."),
    ("VO_U10_13_06.mp3", "Some days are not fair. It is still a good life."),
    ("VO_U10_13_07.mp3", "Spend some time with your family today."),
    ("VO_U10_13_08.mp3", "Nothing stays the same forever."),
    ("VO_U10_13_09.mp3", "If somebody is sad, sit with them."),
    ("VO_U10_13_10.mp3", "Do the thing you have been putting off."),
    ("VO_U10_13_11.mp3", "Be excited about something small today."),
    ("VO_U10_13_12.mp3", "Look at our garden. You grew all of that."),
    ("VO_U10_14.mp3", "Look at our garden. You grew all of that.")
]

def clean_old_wav_files():
    # Delete old SAPI wav files
    for folder in [VOICES_DIR, AUDIO_DIR, SFX_DIR]:
        if not os.path.exists(folder):
            continue
        for f in os.listdir(folder):
            if f.startswith("VO_U10_") and (f.endswith(".wav") or f.endswith(".wav.meta")):
                try:
                    os.remove(os.path.join(folder, f))
                    print(f"Removed old voice file: {f}")
                except Exception as e:
                    print(f"Error removing {f}: {e}")

async def generate_all_neural():
    os.makedirs(VOICES_DIR, exist_ok=True)
    os.makedirs(AUDIO_DIR, exist_ok=True)
    os.makedirs(SFX_DIR, exist_ok=True)

    clean_old_wav_files()

    total = len(VOICE_LINES)
    print(f"Starting EngSnap Neural TTS generation for {total} files using {VOICE_NEURAL}...")

    for i, (filename, text) in enumerate(VOICE_LINES, 1):
        target_path = os.path.join(VOICES_DIR, filename)

        # Rate -5% for calm, slow educational cadence
        communicate = edge_tts.Communicate(text, VOICE_NEURAL, rate="-5%", pitch="+0Hz")
        await communicate.save(target_path)
        
        file_size = os.path.getsize(target_path)
        print(f"[{i}/{total}] Generated EngSnap Neural VO: {filename} ({file_size} bytes) -> '{text}'")

        # Copy to root Audio and SFX folder for backward compatibility
        shutil.copy2(target_path, os.path.join(AUDIO_DIR, filename))
        shutil.copy2(target_path, os.path.join(SFX_DIR, filename))

    print(f"\nAll {total} EngSnap Neural Voiceover Lines generated successfully!")

if __name__ == "__main__":
    asyncio.run(generate_all_neural())
