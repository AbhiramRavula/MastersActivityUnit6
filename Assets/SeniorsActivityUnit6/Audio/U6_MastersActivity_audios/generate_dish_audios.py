import asyncio
import edge_tts
import os
import uuid

VOICE_ANU = "en-IN-NeerjaNeural"

dish_audios = [
    {"file": "VO_U6_ORD_IDLI_POLITE.mp3", "text": "Could I have the idli, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_IDLI_BLUNT.mp3", "text": "I want idli.", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_NOODLES_POLITE.mp3", "text": "Could I have the noodles, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_NOODLES_BLUNT.mp3", "text": "I want noodles.", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_RICE_POLITE.mp3", "text": "Could I have the rice, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_RICE_BLUNT.mp3", "text": "I want rice.", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_ROTI_POLITE.mp3", "text": "Could I have the roti, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_ROTI_BLUNT.mp3", "text": "I want roti.", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_ICECREAM_POLITE.mp3", "text": "Could I have the ice cream, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_ICECREAM_BLUNT.mp3", "text": "I want ice cream.", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_SANDWICH_POLITE.mp3", "text": "Could I have the sandwich, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_SANDWICH_BLUNT.mp3", "text": "I want sandwich.", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_JUICE_POLITE.mp3", "text": "Could I have the juice, please?", "voice": VOICE_ANU},
    {"file": "VO_U6_ORD_JUICE_BLUNT.mp3", "text": "I want juice.", "voice": VOICE_ANU},
]

META_TEMPLATE = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 7
  defaultSettings:
    serializedVersion: 2
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 1
    conversionMode: 0
    preloadAudioData: 0
  platformSettingOverrides: {{}}
  forceToMono: 0
  normalize: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

async def generate():
    target_dir = os.path.dirname(os.path.abspath(__file__))
    print(f"Generating dish voiceovers in: {target_dir}")
    
    for item in dish_audios:
        filepath = os.path.join(target_dir, item["file"])
        meta_filepath = filepath + ".meta"
        
        print(f"Synthesizing: {item['file']} -> '{item['text']}'")
        communicate = edge_tts.Communicate(item["text"], item["voice"])
        await communicate.save(filepath)
        
        size = os.path.getsize(filepath)
        print(f"  Saved ({size} bytes)")
        
        if not os.path.exists(meta_filepath):
            file_guid = uuid.uuid4().hex
            with open(meta_filepath, "w", encoding="utf-8") as f:
                f.write(META_TEMPLATE.format(guid=file_guid))
            print(f"  Generated meta: {meta_filepath}")

if __name__ == "__main__":
    asyncio.run(generate())
