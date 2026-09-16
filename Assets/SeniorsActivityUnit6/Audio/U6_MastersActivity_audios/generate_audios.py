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
    {"file": "Today Anus family is eating out.mp3", "text": "Today, Anu's family is eating out.", "voice": VOICE_TEACHER},
    {"file": "The food is not here yet Watch Anu.mp3", "text": "The food is not here yet... Watch Anu.", "voice": VOICE_TEACHER},
    {"file": "Quick Tap the button.mp3", "text": "Quick! Tap the button.", "voice": VOICE_TEACHER},
    {"file": "Everybody is happy One star.mp3", "text": "Everybody is happy! One star.", "voice": VOICE_TEACHER},
    {"file": "Now choose What will Anu eat.mp3", "text": "Now choose. What will Anu eat?", "voice": VOICE_TEACHER},
    {"file": "Read first.mp3", "text": "Read first.", "voice": VOICE_TEACHER},
    {"file": "How should Anu ask.mp3", "text": "How should Anu ask?", "voice": VOICE_TEACHER},
    {"file": "This is Ravi He is looking after their.mp3", "text": "This is Ravi. He is looking after their table.", "voice": VOICE_TEACHER},
    {"file": "Oh That is the wrong dish.mp3", "text": "Oh! That is the wrong dish.", "voice": VOICE_TEACHER},
    {"file": "How loud should Anu talk here.mp3", "text": "How loud should Anu talk here?", "voice": VOICE_TEACHER},
    {"file": "The restaurant is full now.mp3", "text": "The restaurant is full now.", "voice": VOICE_TEACHER},
    {"file": "Three stars Thank you come again.mp3", "text": "Three stars! Thank you, come again.", "voice": VOICE_TEACHER},
    {"file": "What will you say to the waiter next.mp3", "text": "What will you say to the waiter next time?", "voice": VOICE_TEACHER},

    # 2. Anu (Child)
    {"file": "Ummm ummm.mp3", "text": "Uhh... uhh...", "voice": VOICE_ANU},
    {"file": "Could I have the dosa please.mp3", "text": "Could I have the dosa, please?", "voice": VOICE_ANU},
    {"file": "I want dosa.mp3", "text": "I want dosa!", "voice": VOICE_ANU},
    {"file": "Thank you.mp3", "text": "Thank you!", "voice": VOICE_ANU},
    {"file": "Sorry I think I ordered dosa.mp3", "text": "Sorry... I think I ordered dosa.", "voice": VOICE_ANU},
    {"file": "This is WRONG.mp3", "text": "This is wrong!", "voice": VOICE_ANU},
    {"file": "I am SO hungry Where is my food.mp3", "text": "I am so hungry! Where is my food?!", "voice": VOICE_ANU},
    {"file": "Excuse me could I have another fork please.mp3", "text": "Excuse me, could I have another fork, please?", "voice": VOICE_ANU},
    {"file": "I dropped my fork shouted.mp3", "text": "I dropped my fork!!", "voice": VOICE_ANU},
    {"file": "Daddy guess what happened at school.mp3", "text": "Daddy, guess what happened at school!", "voice": VOICE_ANU},

    # 3. Waiter Ravi
    {"file": "Certainly.mp3", "text": "Certainly!", "voice": VOICE_RAVI},
    {"file": "Of course one moment.mp3", "text": "Of course, one moment.", "voice": VOICE_RAVI},
    {"file": "I am so sorry I will fix that.mp3", "text": "I am so sorry, I will fix that right away.", "voice": VOICE_RAVI},
    {"file": "Thank you do come again.mp3", "text": "Thank you, do come again!", "voice": VOICE_RAVI},

    # 4. Parents
    {"file": "Sorry I cannot hear you at all.mp3", "text": "Sorry? I cannot hear you at all.", "voice": VOICE_DAD},
    {"file": "Anu quiet just a name gently warning.mp3", "text": "Anu... shh, quiet.", "voice": VOICE_MUM},
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
