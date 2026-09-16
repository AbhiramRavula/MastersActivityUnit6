import os
import sys
import json
import asyncio
import tempfile
import edge_tts

async def main():
    if len(sys.argv) < 2:
        print("Error: No data file provided.", flush=True)
        sys.exit(1)
        
    data_file = sys.argv[1]
    if not os.path.exists(data_file):
        print(f"Error: Data file {data_file} not found.", flush=True)
        sys.exit(1)
        
    with open(data_file, 'r', encoding='utf-8') as f:
        data = json.load(f)
        
    sentences = data.get('sentences', [])
    speed = data.get('speed', '+0%')
    pitch = data.get('pitch', '+0Hz')
    voice = data.get('voice', 'en-IN-NeerjaNeural')
    output_dir = data.get('output_dir', '.')
    
    os.makedirs(output_dir, exist_ok=True)
    total = len(sentences)
    
    for idx, text in enumerate(sentences):
        text = text.strip()
        if not text:
            continue
            
        clean_name = "".join(c for c in text if c.isalnum() or c.isspace()).strip()
        words = clean_name.split()
        short_name_words = []
        current_len = 0
        
        for w in words:
            if current_len + len(w) > 60 or len(short_name_words) >= 8:
                break
            short_name_words.append(w)
            current_len += len(w) + 1
            
        short_name = " ".join(short_name_words)
        if not short_name:
            short_name = "audio_" + str(hash(text))[-8:]
            
        filename = short_name + ".mp3"
        save_path = os.path.join(output_dir, filename)
        
        print(f"PROGRESS:{idx + 1}/{total}:{filename}", flush=True)
        
        # Write to temporary file first, then atomically replace to avoid Unity file lock errors
        temp_fd, temp_path = tempfile.mkstemp(suffix=".mp3")
        os.close(temp_fd)
        
        try:
            communicate = edge_tts.Communicate(text=text, voice=voice, rate=speed, pitch=pitch, volume="+0%")
            await communicate.save(temp_path)
            if os.path.exists(save_path):
                try:
                    os.remove(save_path)
                except Exception:
                    pass
            os.replace(temp_path, save_path)
        except Exception as e:
            print(f"Error generating {filename}: {e}", flush=True)
            if os.path.exists(temp_path):
                os.remove(temp_path)
                
    print("ALL DONE", flush=True)

if __name__ == "__main__":
    asyncio.run(main())
