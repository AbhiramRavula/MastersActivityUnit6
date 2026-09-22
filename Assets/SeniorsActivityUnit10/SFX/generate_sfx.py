import os
import math
import struct
import random

SAMPLE_RATE = 44100
SFX_DIR = os.path.dirname(os.path.abspath(__file__))
AUDIO_DIR = os.path.join(os.path.dirname(SFX_DIR), "Audio")

def save_wav(filename, samples):
    # Clamp and convert float samples [-1.0, 1.0] to 16-bit PCM mono WAV
    int_samples = []
    for s in samples:
        s = max(-0.95, min(0.95, s))
        int_samples.append(int(s * 32767))
        
    num_samples = len(int_samples)
    data_size = num_samples * 2
    
    header = bytearray()
    header.extend(b'RIFF')
    header.extend(struct.pack('<I', data_size + 36))
    header.extend(b'WAVE')
    header.extend(b'fmt ')
    header.extend(struct.pack('<I', 16))          # Subchunk1Size (16 for PCM)
    header.extend(struct.pack('<H', 1))           # AudioFormat (1 for PCM)
    header.extend(struct.pack('<H', 1))           # NumChannels (1 = Mono)
    header.extend(struct.pack('<I', SAMPLE_RATE)) # SampleRate
    header.extend(struct.pack('<I', SAMPLE_RATE * 2)) # ByteRate
    header.extend(struct.pack('<H', 2))           # BlockAlign
    header.extend(struct.pack('<H', 16))          # BitsPerSample
    header.extend(b'data')
    header.extend(struct.pack('<I', data_size))
    
    raw_bytes = bytearray()
    for s in int_samples:
        raw_bytes.extend(struct.pack('<h', s))
        
    out_path_sfx = os.path.join(SFX_DIR, filename)
    with open(out_path_sfx, 'wb') as f:
        f.write(header + raw_bytes)
        
    out_path_audio = os.path.join(AUDIO_DIR, filename)
    with open(out_path_audio, 'wb') as f:
        f.write(header + raw_bytes)
        
    print(f"Generated: {filename} ({len(samples)/SAMPLE_RATE:.2f}s)")

def smooth_env(t, attack_dur, decay_rate):
    """Smooth attack (raised cosine) followed by exponential decay. Zero clicks."""
    if t < 0:
        return 0.0
    if t < attack_dur:
        # Raised cosine attack: 0.5 * (1 - cos(pi * t / attack_dur))
        attack = 0.5 * (1.0 - math.cos(math.pi * t / attack_dur))
    else:
        attack = 1.0
    decay = math.exp(-(t - attack_dur) * decay_rate) if t >= attack_dur else 1.0
    return attack * decay

def warm_tone(freq, t):
    """Pure warm fundamental tone with gentle warm 2nd harmonic, no harsh high frequencies."""
    return math.sin(2 * math.pi * freq * t) * 0.85 + math.sin(4 * math.pi * freq * t) * 0.15

def make_plant_grow():
    # Warm, rising major chord arpeggio (C4 -> E4 -> G4 -> C5) with smooth rounded envelopes
    duration = 1.0
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    notes = [261.63, 329.63, 392.00, 523.25]
    
    for idx, freq in enumerate(notes):
        start_time = idx * 0.18
        start_idx = int(start_time * SAMPLE_RATE)
        for i in range(start_idx, min(num_samples, start_idx + int(0.55 * SAMPLE_RATE))):
            t = (i - start_idx) / SAMPLE_RATE
            env = smooth_env(t, attack_dur=0.03, decay_rate=6.0)
            tone = warm_tone(freq, t)
            samples[i] += tone * env * 0.28
            
    return samples

def make_flower_bloom():
    # Gentle, soft bell sparkle shimmer
    duration = 1.0
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    freqs = [392.0, 493.88, 587.33, 659.25]
    
    for idx, freq in enumerate(freqs):
        start_idx = int(idx * 0.15 * SAMPLE_RATE)
        for i in range(start_idx, min(num_samples, start_idx + int(0.55 * SAMPLE_RATE))):
            t = (i - start_idx) / SAMPLE_RATE
            env = smooth_env(t, attack_dur=0.035, decay_rate=6.5)
            tone = warm_tone(freq, t)
            samples[i] += tone * env * 0.22
            
    return samples

def make_marble_drop():
    # Warm wooden/glass chime droplet (soft attack, zero ear-piercing spike)
    duration = 0.6
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    
    # Fundamental chime 659Hz (E5) and soft sub-harmonic 330Hz
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        env = smooth_env(t, attack_dur=0.015, decay_rate=12.0)
        chime = math.sin(2 * math.pi * 659.25 * t) * 0.7 + math.sin(2 * math.pi * 329.63 * t) * 0.3
        samples[i] = chime * env * 0.45
        
    # Gentle soft bounce at 0.12s
    start_b2 = int(0.12 * SAMPLE_RATE)
    for i in range(start_b2, num_samples):
        t = (i - start_b2) / SAMPLE_RATE
        env2 = smooth_env(t, attack_dur=0.015, decay_rate=18.0)
        samples[i] += math.sin(2 * math.pi * 783.99 * t) * env2 * 0.25
        
    return samples

def make_jar_full():
    # Resonant, warm glowing chord (C4, E4, G4, C5)
    duration = 2.0
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    chord = [261.63, 329.63, 392.00, 523.25]
    
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        env = smooth_env(t, attack_dur=0.08, decay_rate=2.0)
        tone = 0.0
        for idx, freq in enumerate(chord):
            tone += warm_tone(freq, t) * (0.28 / (idx + 1))
        samples[i] = tone * env * 0.55
        
    return samples

def make_golden_line():
    # Warm, inspiring chime melody (A4, C5, D5, E5, G5)
    duration = 1.8
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    chimes = [440.0, 523.25, 587.33, 659.25, 783.99]
    
    for idx, f in enumerate(chimes):
        start_i = int(idx * 0.16 * SAMPLE_RATE)
        for i in range(start_i, min(num_samples, start_i + int(0.9 * SAMPLE_RATE))):
            t = (i - start_i) / SAMPLE_RATE
            env = smooth_env(t, attack_dur=0.04, decay_rate=3.5)
            tone = warm_tone(f, t)
            samples[i] += tone * env * 0.22
            
    return samples

def make_tally():
    # Soft warm wooden marimba tap
    duration = 0.12
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        env = smooth_env(t, attack_dur=0.008, decay_rate=35.0)
        sine = math.sin(2 * math.pi * 392.0 * t) * 0.75 + math.sin(2 * math.pi * 523.25 * t) * 0.25
        samples[i] = sine * env * 0.45
    return samples

def make_tap():
    # Soft mellow UI button bubble tap
    duration = 0.1
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        env = smooth_env(t, attack_dur=0.008, decay_rate=40.0)
        samples[i] = math.sin(2 * math.pi * 349.23 * t) * env * 0.4
    return samples

def make_amb_garden():
    # Warm, soothing, quiet harmonic ambient pad in C Major (NO static hiss, NO random noise, seamlessly looped)
    duration = 16.0
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    
    # Soft gentle chord pad (C3: 130.81Hz, G3: 196.00Hz, C4: 261.63Hz)
    # Modulation frequencies are exact integer cycles over 16 seconds to guarantee seamless loop!
    for i in range(num_samples):
        t = i / SAMPLE_RATE
        # Exact integer cycle swells (1 cycle = 0.0625 Hz, 2 cycles = 0.125 Hz)
        mod1 = 0.5 + 0.5 * math.sin(2 * math.pi * (1.0 / 16.0) * t - math.pi / 2) # smooth 0 -> 1 -> 0
        mod2 = 0.5 + 0.5 * math.sin(2 * math.pi * (2.0 / 16.0) * t)
        
        c3 = math.sin(2 * math.pi * 130.81 * t) * 0.08
        g3 = math.sin(2 * math.pi * 196.00 * t) * 0.05
        e4 = math.sin(2 * math.pi * 329.63 * t) * 0.03
        
        samples[i] = (c3 + g3 + e4) * (0.6 + 0.4 * mod1) * 0.35
        
    return samples

def make_mus_garden():
    # Calming, warm, peaceful acoustic music in C Major (Soft Rhodes / Marimba / Kalimba style)
    # Zero sharp spikes, smooth 45ms attacks, warm frequency range (130Hz - 660Hz), and seamless wrap-around loop!
    duration = 16.0
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    
    # 4 beautiful chords (4.0s each = 16.0s total):
    # 1. C Major   (C3 bass,  G3, C4, E4, G4)
    # 2. F Major   (F2 bass,  A3, C4, F4, A4)
    # 3. A Minor   (A2 bass,  E3, A3, C4, E4)
    # 4. G Major   (G2 bass,  D3, G3, B3, D4)
    chords = [
        # chord_start, bass_freq, [(offset, freq)]
        (0.0, 130.81, [(0.0, 261.63), (0.75, 329.63), (1.5, 392.00), (2.25, 523.25), (3.0, 392.00)]),
        (4.0, 87.31,  [(0.0, 174.61), (0.75, 261.63), (1.5, 349.23), (2.25, 440.00), (3.0, 349.23)]),
        (8.0, 110.00, [(0.0, 220.00), (0.75, 261.63), (1.5, 329.63), (2.25, 440.00), (3.0, 329.63)]),
        (12.0, 98.00, [(0.0, 196.00), (0.75, 246.94), (1.5, 293.66), (2.25, 392.00), (3.0, 293.66)]),
    ]
    
    for chord_start, bass_freq, melody_notes in chords:
        # Warm bass note with soft rounded attack
        bass_note_samples = int(3.6 * SAMPLE_RATE)
        for s in range(bass_note_samples):
            t = s / SAMPLE_RATE
            env_bass = smooth_env(t, attack_dur=0.08, decay_rate=0.8)
            tone_bass = math.sin(2 * math.pi * bass_freq * t) * 0.8 + math.sin(4 * math.pi * bass_freq * t) * 0.2
            # Wrap around sample index to ensure seamless looping
            dest_idx = (int(chord_start * SAMPLE_RATE) + s) % num_samples
            samples[dest_idx] += tone_bass * env_bass * 0.16
            
        # Melody arpeggios
        for offset, freq in melody_notes:
            note_time = chord_start + offset
            note_samples = int(2.2 * SAMPLE_RATE)
            for s in range(note_samples):
                t = s / SAMPLE_RATE
                env_mel = smooth_env(t, attack_dur=0.045, decay_rate=2.2)
                tone_mel = warm_tone(freq, t)
                dest_idx = (int(note_time * SAMPLE_RATE) + s) % num_samples
                samples[dest_idx] += tone_mel * env_mel * 0.14
                
    return samples

def make_mus_endterm():
    # Warm, celebratory, gentle harvest fanfare
    duration = 6.0
    num_samples = int(SAMPLE_RATE * duration)
    samples = [0.0] * num_samples
    
    melody = [
        (0.0, 0.4, 392.00),
        (0.4, 0.4, 523.25),
        (0.8, 0.4, 659.25),
        (1.2, 0.8, 783.99),
        (2.0, 0.4, 659.25),
        (2.4, 1.2, 783.99),
        (3.6, 2.0, 523.25)
    ]
    
    for start_t, dur, freq in melody:
        start_i = int(start_t * SAMPLE_RATE)
        note_samples = int((dur + 1.0) * SAMPLE_RATE)
        for s in range(note_samples):
            t = s / SAMPLE_RATE
            env = smooth_env(t, attack_dur=0.04, decay_rate=1.8 / dur)
            tone = warm_tone(freq, t)
            idx = start_i + s
            if idx < num_samples:
                samples[idx] += tone * env * 0.22
            
    return samples

if __name__ == '__main__':
    print("Generating Unit 10 Procedural SFX & Music Assets (Smooth Warm Harmonic Edition)...")
    save_wav("SFX_PlantGrow.wav", make_plant_grow())
    save_wav("SFX_Flower.wav", make_flower_bloom())
    save_wav("SFX_Marble.wav", make_marble_drop())
    save_wav("SFX_JarFull.wav", make_jar_full())
    save_wav("SFX_GoldenLine.wav", make_golden_line())
    save_wav("SFX_Tally.wav", make_tally())
    save_wav("SFX_Tap.wav", make_tap())
    save_wav("AMB_Garden.wav", make_amb_garden())
    save_wav("MUS_Garden.wav", make_mus_garden())
    save_wav("MUS_EndTerm.wav", make_mus_endterm())
    print("All 10 Unit 10 Audio Assets Regenerated Successfully!")

