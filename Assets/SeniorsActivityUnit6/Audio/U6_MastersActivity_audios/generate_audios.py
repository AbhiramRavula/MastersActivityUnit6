import asyncio
import edge_tts
import os

# Voice mappings supported by Edge-TTS
VOICE_TEACHER = "en-IN-NeerjaNeural"
VOICE_ANU = "en-IN-NeerjaNeural"
VOICE_RAVI = "en-IN-PrabhatNeural"
VOICE_DAD = "en-IN-PrabhatNeural"
VOICE_MUM = "en-IN-NeerjaNeural"

audio_items = [
    # 1. Teacher / Narrator
    {"file": "VO_U6_01.mp3", "text": "Today, Anu's family is eating out!", "voice": VOICE_TEACHER},
    {"file": "VO_U6_02.mp3", "text": "The food is not here yet. Watch Anu.", "voice": VOICE_TEACHER},
    {"file": "VO_U6_03.mp3", "text": "Quick! Tap the button.", "voice": VOICE_TEACHER},
    {"file": "VO_U6_04.mp3", "text": "Everybody is happy. One star!", "voice": VOICE_TEACHER},
    {"file": "VO_U6_05.mp3", "text": "Now choose. What will Anu eat?", "voice": VOICE_TEACHER},
    {"file": "VO_U6_06.mp3", "text": "Read first.", "voice": VOICE_TEACHER},
    {"file": "VO_U6_07.mp3", "text": "How should Anu ask?", "voice": VOICE_TEACHER},
    {"file": "VO_U6_08.mp3", "text": "This is Ravi. He is looking after their table.", "voice": VOICE_TEACHER},
    {"file": "VO_U6_09.mp3", "text": "Oh! That is the wrong dish.", "voice": VOICE_TEACHER},
    {"file": "VO_U6_10.mp3", "text": "How loud should Anu talk here?", "voice": VOICE_TEACHER},
    {"file": "VO_U6_11.mp3", "text": "The restaurant is full now.", "voice": VOICE_TEACHER},
    {"file": "VO_U6_12.mp3", "text": "Three stars! Thank you, come again!", "voice": VOICE_TEACHER},
    {"file": "VO_U6_13.mp3", "text": "What will you say to the waiter next time?", "voice": VOICE_TEACHER},

    # 2. Anu (Child)
    {"file": "VO_U6_ANU_1.mp3", "text": "Could I have the dosa, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_2.mp3", "text": "I want dosa.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_3.mp3", "text": "Ummm... ummm...", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_4.mp3", "text": "Thank you!", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_5.mp3", "text": "Sorry, I think I ordered dosa.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_6.mp3", "text": "This is WRONG!", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_7.mp3", "text": "I am SO hungry! Where is my food?", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_8.mp3", "text": "Excuse me, could I have another fork please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_9.mp3", "text": "I dropped my fork!", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_10.mp3", "text": "Daddy, guess what happened at school!", "voice": VOICE_ANU},

    # 3. Waiter Ravi
    {"file": "VO_U6_WAIT_1.mp3", "text": "Certainly.", "voice": VOICE_RAVI},
    {"file": "VO_U6_WAIT_2.mp3", "text": "Of course, one moment.", "voice": VOICE_RAVI},
    {"file": "VO_U6_WAIT_3.mp3", "text": "I am so sorry, I will fix that right away.", "voice": VOICE_RAVI},
    {"file": "VO_U6_WAIT_4.mp3", "text": "Thank you, do come again.", "voice": VOICE_RAVI},

    # 4. Parents
    {"file": "VO_U6_DAD_1.mp3", "text": "Sorry? I cannot hear you at all.", "voice": VOICE_DAD},
    {"file": "VO_U6_MUM_1.mp3", "text": "Anu.", "voice": VOICE_MUM},
]

async def generate():
    total = len(audio_items)
    for index, item in enumerate(audio_items, 1):
        filename = item["file"]
        text = item["text"]
        voice = item["voice"]

        # Guardrail 1: Check if file already exists with valid content (> 2000 bytes)
        if os.path.exists(filename):
            file_size = os.path.getsize(filename)
            if file_size > 2000:
                print(f"[{index}/{total}] SKIP (Already exists, size={file_size} bytes): {filename}")
                continue
            else:
                # Remove corrupted 0-byte file before re-generating
                print(f"[{index}/{total}] Removing invalid 0-byte file: {filename}")
                try:
                    os.remove(filename)
                except Exception:
                    pass

        # Generate fresh file
        print(f"[{index}/{total}] Generating: {filename} ({voice})...")
        try:
            communicate = edge_tts.Communicate(text, voice)
            await communicate.save(filename)
            new_size = os.path.getsize(filename)
            print(f"  -> SUCCESS: {filename} ({new_size} bytes)")
        except Exception as e:
            print(f"  -> ERROR generating {filename}: {e}")

if __name__ == "__main__":
    asyncio.run(generate())
