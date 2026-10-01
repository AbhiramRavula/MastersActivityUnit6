import asyncio
import edge_tts
import os
import uuid

VOICE_ANU = "en-IN-NeerjaNeural"

wrong_dish_audios = [
    {"file": "VO_U6_ANU_WRONG_DOSA.mp3", "text": "Sorry, I think I ordered dosa.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_WRONG_IDLI.mp3", "text": "Sorry, I think I ordered idli.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_WRONG_NOODLES.mp3", "text": "Sorry, I think I ordered noodles.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_WRONG_RICE.mp3", "text": "Sorry, I think I ordered rice.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_WRONG_ROTI.mp3", "text": "Sorry, I think I ordered roti.", "voice": VOICE_ANU},
    {"file": "VO_U6_ANU_WRONG_ICECREAM.mp3", "text": "Sorry, I think I ordered ice cream.", "voice": VOICE_ANU},
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
    print(f"Generating wrong dish voiceovers in: {target_dir}")
    
    for item in wrong_dish_audios:
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
