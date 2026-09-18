import os

ART_DIR = os.path.dirname(os.path.abspath(__file__))

def save_svg(filename, content):
    path = os.path.join(ART_DIR, filename)
    with open(path, "w", encoding="utf-8") as f:
        f.write(content.strip())
    print(f"  -> Generated SVG: {filename}")

def main():
    print(f"Creating Unit 8 Vector SVGs in: {ART_DIR}")

    # -------------------------------------------------------------
    # 1. FRIENDLY GERM BLOBS (Soft, cute, non-scary vector characters)
    # -------------------------------------------------------------
    # Germ 1: Rosy Pink Blob with cute eyes and warm smile
    save_svg("SPR_Germ_1.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200" width="200" height="200">
  <defs>
    <radialGradient id="pinkGlow" cx="40%" cy="40%" r="60%">
      <stop offset="0%" stop-color="#FFB6C1"/>
      <stop offset="100%" stop-color="#FF69B4"/>
    </radialGradient>
  </defs>
  <!-- Blob Body -->
  <path d="M 50,100 C 30,50 80,20 120,30 C 160,40 180,80 170,120 C 160,160 120,180 80,170 C 40,160 30,130 50,100 Z" fill="url(#pinkGlow)" stroke="#FF1493" stroke-width="4"/>
  <!-- Rosy Cheeks -->
  <circle cx="65" cy="115" r="12" fill="#FF1493" opacity="0.3"/>
  <circle cx="135" cy="115" r="12" fill="#FF1493" opacity="0.3"/>
  <!-- Eyes -->
  <circle cx="75" cy="95" r="10" fill="#222"/>
  <circle cx="78" cy="92" r="3" fill="#FFF"/>
  <circle cx="125" cy="95" r="10" fill="#222"/>
  <circle cx="128" cy="92" r="3" fill="#FFF"/>
  <!-- Happy Mouth -->
  <path d="M 90,115 Q 100,130 110,115" fill="none" stroke="#222" stroke-width="4" stroke-linecap="round"/>
</svg>
""")

    # Germ 2: Turquoise / Cyan Joyful Blob with small bubbles
    save_svg("SPR_Germ_2.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200" width="200" height="200">
  <defs>
    <radialGradient id="cyanGlow" cx="40%" cy="40%" r="60%">
      <stop offset="0%" stop-color="#A7F3D0"/>
      <stop offset="100%" stop-color="#06B6D4"/>
    </radialGradient>
  </defs>
  <path d="M 60,70 C 50,30 100,20 140,40 C 180,60 170,110 160,140 C 150,170 90,180 60,150 C 30,120 40,90 60,70 Z" fill="url(#cyanGlow)" stroke="#0891B2" stroke-width="4"/>
  <!-- Little antennae buds -->
  <circle cx="80" cy="30" r="8" fill="#06B6D4" stroke="#0891B2" stroke-width="2"/>
  <circle cx="120" cy="35" r="8" fill="#06B6D4" stroke="#0891B2" stroke-width="2"/>
  <!-- Eyes -->
  <circle cx="80" cy="95" r="11" fill="#1E293B"/>
  <circle cx="83" cy="91" r="3.5" fill="#FFF"/>
  <circle cx="120" cy="95" r="11" fill="#1E293B"/>
  <circle cx="123" cy="91" r="3.5" fill="#FFF"/>
  <!-- Big Smile -->
  <path d="M 88,118 Q 100,138 112,118 Z" fill="#EF4444" stroke="#1E293B" stroke-width="3"/>
</svg>
""")

    # Germ 3: Sunny Yellow/Orange blob waving
    save_svg("SPR_Germ_3.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200" width="200" height="200">
  <defs>
    <radialGradient id="yellowGlow" cx="40%" cy="40%" r="60%">
      <stop offset="0%" stop-color="#FEF08A"/>
      <stop offset="100%" stop-color="#F59E0B"/>
    </radialGradient>
  </defs>
  <path d="M 50,90 C 30,40 100,30 130,50 C 170,70 180,120 150,160 C 120,180 60,170 40,130 C 30,110 40,100 50,90 Z" fill="url(#yellowGlow)" stroke="#D97706" stroke-width="4"/>
  <circle cx="60" cy="115" r="10" fill="#F97316" opacity="0.3"/>
  <circle cx="130" cy="115" r="10" fill="#F97316" opacity="0.3"/>
  <circle cx="75" cy="90" r="9" fill="#1F2937"/>
  <circle cx="77" cy="87" r="3" fill="#FFF"/>
  <circle cx="115" cy="90" r="9" fill="#1F2937"/>
  <circle cx="117" cy="87" r="3" fill="#FFF"/>
  <!-- Surprised cute mouth -->
  <ellipse cx="95" cy="118" rx="7" ry="10" fill="#1F2937"/>
</svg>
""")

    # Germ 4: Soft Purple / Lavender gentle blob
    save_svg("SPR_Germ_4.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200" width="200" height="200">
  <defs>
    <radialGradient id="purpleGlow" cx="40%" cy="40%" r="60%">
      <stop offset="0%" stop-color="#DDD6FE"/>
      <stop offset="100%" stop-color="#8B5CF6"/>
    </radialGradient>
  </defs>
  <path d="M 70,50 C 110,30 160,60 150,110 C 140,160 80,180 50,140 C 20,100 40,60 70,50 Z" fill="url(#purpleGlow)" stroke="#6D28D9" stroke-width="4"/>
  <!-- Eyes -->
  <circle cx="85" cy="95" r="10" fill="#1E1B4B"/>
  <circle cx="88" cy="92" r="3" fill="#FFF"/>
  <circle cx="120" cy="95" r="10" fill="#1E1B4B"/>
  <circle cx="123" cy="92" r="3" fill="#FFF"/>
  <path d="M 95,115 Q 102,125 110,115" fill="none" stroke="#1E1B4B" stroke-width="3" stroke-linecap="round"/>
</svg>
""")

    # -------------------------------------------------------------
    # 2. HANDWASHING SCREEN ASSETS
    # -------------------------------------------------------------
    save_svg("SPR_Hands_Normal.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 300" width="400" height="300">
  <defs>
    <linearGradient id="skinGrad" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#FDE68A"/>
      <stop offset="100%" stop-color="#F59E0B"/>
    </linearGradient>
  </defs>
  <!-- Left Hand -->
  <g transform="translate(60, 40) rotate(-15)">
    <path d="M 60,180 L 60,110 C 60,90 90,90 90,110 L 90,80 C 90,60 115,60 115,80 L 115,70 C 115,50 140,50 140,70 L 140,90 C 140,75 160,75 160,95 L 160,180 Z" fill="url(#skinGrad)" stroke="#B45309" stroke-width="4"/>
  </g>
  <!-- Right Hand -->
  <g transform="translate(190, 40) rotate(15)">
    <path d="M 60,180 L 60,95 C 60,75 80,75 80,90 L 80,70 C 80,50 105,50 105,70 L 105,80 C 105,60 130,60 130,80 L 130,110 C 130,90 160,90 160,110 L 160,180 Z" fill="url(#skinGrad)" stroke="#B45309" stroke-width="4"/>
  </g>
</svg>
""")

    save_svg("SPR_Hands_Soapy.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 300" width="400" height="300">
  <defs>
    <linearGradient id="skinGrad" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#FDE68A"/>
      <stop offset="100%" stop-color="#F59E0B"/>
    </linearGradient>
  </defs>
  <!-- Left Hand -->
  <g transform="translate(60, 40) rotate(-15)">
    <path d="M 60,180 L 60,110 C 60,90 90,90 90,110 L 90,80 C 90,60 115,60 115,80 L 115,70 C 115,50 140,50 140,70 L 140,90 C 140,75 160,75 160,95 L 160,180 Z" fill="url(#skinGrad)" stroke="#B45309" stroke-width="4"/>
  </g>
  <!-- Right Hand -->
  <g transform="translate(190, 40) rotate(15)">
    <path d="M 60,180 L 60,95 C 60,75 80,75 80,90 L 80,70 C 80,50 105,50 105,70 L 105,80 C 105,60 130,60 130,80 L 130,110 C 130,90 160,90 160,110 L 160,180 Z" fill="url(#skinGrad)" stroke="#B45309" stroke-width="4"/>
  </g>
  <!-- Soap Bubbles Overlay -->
  <circle cx="160" cy="140" r="25" fill="#FFFFFF" opacity="0.85" stroke="#93C5FD" stroke-width="3"/>
  <circle cx="200" cy="120" r="30" fill="#FFFFFF" opacity="0.9" stroke="#93C5FD" stroke-width="3"/>
  <circle cx="235" cy="150" r="22" fill="#FFFFFF" opacity="0.8" stroke="#93C5FD" stroke-width="3"/>
  <circle cx="180" cy="170" r="28" fill="#FFFFFF" opacity="0.9" stroke="#93C5FD" stroke-width="3"/>
</svg>
""")

    save_svg("SPR_Bubbles.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 200" width="200" height="200">
  <circle cx="80" cy="100" r="40" fill="#E0F2FE" opacity="0.75" stroke="#38BDF8" stroke-width="3"/>
  <circle cx="65" cy="85" r="10" fill="#FFFFFF" opacity="0.8"/>
  <circle cx="130" cy="70" r="30" fill="#E0F2FE" opacity="0.75" stroke="#38BDF8" stroke-width="3"/>
  <circle cx="120" cy="60" r="8" fill="#FFFFFF" opacity="0.8"/>
  <circle cx="140" cy="130" r="35" fill="#E0F2FE" opacity="0.75" stroke="#38BDF8" stroke-width="3"/>
  <circle cx="130" cy="115" r="9" fill="#FFFFFF" opacity="0.8"/>
</svg>
""")

    # -------------------------------------------------------------
    # 3. WASHROOM FIXTURES
    # -------------------------------------------------------------
    # Clean Porcelain Sink
    save_svg("SPR_Sink_Clean.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 300 200" width="300" height="200">
  <defs>
    <linearGradient id="basinGrad" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#FFFFFF"/>
      <stop offset="100%" stop-color="#E2E8F0"/>
    </linearGradient>
  </defs>
  <!-- Basin Base -->
  <path d="M 40,60 L 260,60 C 250,140 210,160 150,160 C 90,160 50,140 40,60 Z" fill="url(#basinGrad)" stroke="#64748B" stroke-width="4"/>
  <!-- Inner Rim -->
  <ellipse cx="150" cy="65" rx="100" ry="18" fill="#CBD5E1"/>
  <!-- Chrome Faucet -->
  <path d="M 140,65 L 140,25 C 140,10 160,10 160,25 L 160,40" fill="none" stroke="#94A3B8" stroke-width="12" stroke-linecap="round"/>
  <!-- Clean Sparkle -->
  <path d="M 230,40 L 235,55 L 250,60 L 235,65 L 230,80 L 225,65 L 210,60 L 225,55 Z" fill="#FBBF24"/>
</svg>
""")

    # Splashed Wet Sink
    save_svg("SPR_Sink_Splashed.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 300 200" width="300" height="200">
  <defs>
    <linearGradient id="basinGrad" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#FFFFFF"/>
      <stop offset="100%" stop-color="#E2E8F0"/>
    </linearGradient>
  </defs>
  <path d="M 40,60 L 260,60 C 250,140 210,160 150,160 C 90,160 50,140 40,60 Z" fill="url(#basinGrad)" stroke="#64748B" stroke-width="4"/>
  <ellipse cx="150" cy="65" rx="100" ry="18" fill="#93C5FD"/>
  <path d="M 140,65 L 140,25 C 140,10 160,10 160,25 L 160,40" fill="none" stroke="#94A3B8" stroke-width="12" stroke-linecap="round"/>
  <!-- Splashes on rim & counter -->
  <ellipse cx="70" cy="55" rx="18" ry="6" fill="#60A5FA" opacity="0.8"/>
  <ellipse cx="230" cy="55" rx="22" ry="7" fill="#60A5FA" opacity="0.8"/>
  <circle cx="50" cy="40" r="5" fill="#3B82F6"/>
  <circle cx="255" cy="45" r="6" fill="#3B82F6"/>
</svg>
""")

    # Water Stream
    save_svg("SPR_WaterStream.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 60 120" width="60" height="120">
  <path d="M 25,0 L 22,120 C 25,125 35,125 38,120 L 35,0 Z" fill="#60A5FA" opacity="0.85"/>
  <path d="M 28,0 L 26,115 C 28,118 32,118 34,115 L 32,0 Z" fill="#BFDBFE" opacity="0.9"/>
</svg>
""")

    # Closed Cubicle Door with Lock
    save_svg("SPR_Cubicle_Closed.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 240 380" width="240" height="380">
  <rect x="10" y="10" width="220" height="360" rx="8" fill="#F8FAFC" stroke="#0284C7" stroke-width="6"/>
  <!-- Door Handle & Bolt Lock -->
  <rect x="25" y="180" width="35" height="15" rx="3" fill="#64748B" stroke="#334155" stroke-width="2"/>
  <circle cx="32" cy="187" r="4" fill="#EF4444"/>
  <!-- SHUT / CLOSED SIGN -->
  <rect x="55" y="60" width="130" height="50" rx="6" fill="#DC2626" stroke="#FFFFFF" stroke-width="3"/>
  <text x="120" y="93" font-family="Arial, sans-serif" font-size="20" font-weight="bold" fill="#FFFFFF" text-anchor="middle">SHUT</text>
</svg>
""")

    # Open Cubicle Door
    save_svg("SPR_Cubicle_Open.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 240 380" width="240" height="380">
  <!-- Stall interior shadow -->
  <rect x="10" y="10" width="220" height="360" rx="8" fill="#CBD5E1" stroke="#0284C7" stroke-width="6"/>
  <!-- Clean white toilet bowl partially visible -->
  <ellipse cx="120" cy="260" rx="45" ry="30" fill="#FFFFFF" stroke="#64748B" stroke-width="3"/>
  <!-- Swung door perspective -->
  <polygon points="10,10 70,30 70,350 10,370" fill="#F8FAFC" stroke="#0284C7" stroke-width="4"/>
</svg>
""")

    # Soap Dispensers
    save_svg("SPR_Soap_Full.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 160" width="100" height="160">
  <!-- Bottle body -->
  <rect x="20" y="50" width="60" height="100" rx="12" fill="#E0F2FE" stroke="#0284C7" stroke-width="3"/>
  <!-- Green soap liquid inside -->
  <rect x="23" y="75" width="54" height="72" rx="8" fill="#34D399" opacity="0.85"/>
  <!-- Pump top -->
  <rect x="42" y="30" width="16" height="20" fill="#94A3B8"/>
  <path d="M 30,30 L 70,30 C 70,20 60,15 50,15 L 30,15 Z" fill="#64748B"/>
</svg>
""")

    save_svg("SPR_Soap_Empty.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 160" width="100" height="160">
  <rect x="20" y="50" width="60" height="100" rx="12" fill="#E0F2FE" stroke="#0284C7" stroke-width="3"/>
  <!-- Low soap / empty line -->
  <rect x="23" y="140" width="54" height="7" rx="3" fill="#EF4444" opacity="0.6"/>
  <!-- Pump top -->
  <rect x="42" y="30" width="16" height="20" fill="#94A3B8"/>
  <path d="M 30,30 L 70,30 C 70,20 60,15 50,15 L 30,15 Z" fill="#64748B"/>
  <!-- Empty Warning X -->
  <text x="50" y="105" font-family="Arial, sans-serif" font-size="28" font-weight="bold" fill="#DC2626" text-anchor="middle">!</text>
</svg>
""")

    # Paper Towel Dispenser & Soggy Floor Towel
    save_svg("SPR_Towel_Holder.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 140 180" width="140" height="180">
  <rect x="15" y="15" width="110" height="130" rx="8" fill="#F1F5F9" stroke="#475569" stroke-width="4"/>
  <rect x="40" y="135" width="60" height="30" fill="#FDE68A" stroke="#D97706" stroke-width="2"/>
  <text x="70" y="60" font-family="Arial, sans-serif" font-size="14" font-weight="bold" fill="#64748B" text-anchor="middle">TOWELS</text>
</svg>
""")

    save_svg("SPR_Towel_SoggyFloor.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 160 90" width="160" height="90">
  <!-- Wet crumpled lump -->
  <path d="M 20,60 C 30,30 60,25 80,40 C 100,20 130,35 140,65 C 130,80 40,85 20,60 Z" fill="#D97706" stroke="#92400E" stroke-width="3"/>
  <ellipse cx="80" cy="72" rx="70" ry="12" fill="#60A5FA" opacity="0.4"/>
</svg>
""")

    # Trash Bin & Wet Floor Puddle
    save_svg("SPR_TrashBin.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 140 200" width="140" height="200">
  <path d="M 20,40 L 120,40 L 110,190 L 30,190 Z" fill="#94A3B8" stroke="#334155" stroke-width="4"/>
  <ellipse cx="70" cy="40" rx="50" ry="15" fill="#CBD5E1" stroke="#334155" stroke-width="4"/>
</svg>
""")

    save_svg("SPR_WetFloorPuddle.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 280 100" width="280" height="100">
  <!-- Slippery blue water patch -->
  <path d="M 20,50 C 40,15 120,10 180,25 C 240,15 270,45 260,70 C 230,95 80,95 20,50 Z" fill="#60A5FA" opacity="0.75" stroke="#2563EB" stroke-width="3"/>
  <ellipse cx="140" cy="50" rx="90" ry="20" fill="#93C5FD" opacity="0.8"/>
  <circle cx="50" cy="35" r="4" fill="#1D4ED8"/>
  <circle cx="230" cy="65" r="5" fill="#1D4ED8"/>
</svg>
""")

    # -------------------------------------------------------------
    # 4. CHARACTER POSES (Clean vector avatars)
    # -------------------------------------------------------------
    # Anu Normal
    save_svg("SPR_Anu_Normal.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 180 300" width="180" height="300">
  <defs>
    <linearGradient id="anuDress" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#F43F5E"/>
      <stop offset="100%" stop-color="#BE123C"/>
    </linearGradient>
  </defs>
  <!-- Head & Hair -->
  <circle cx="90" cy="70" r="40" fill="#292524"/>
  <circle cx="90" cy="75" r="32" fill="#FED7AA"/>
  <!-- Pigtails -->
  <circle cx="45" cy="65" r="16" fill="#292524"/>
  <circle cx="135" cy="65" r="16" fill="#292524"/>
  <!-- Eyes & Smile -->
  <circle cx="80" cy="75" r="4" fill="#1C1917"/>
  <circle cx="100" cy="75" r="4" fill="#1C1917"/>
  <path d="M 84,88 Q 90,96 96,88" fill="none" stroke="#1C1917" stroke-width="2.5" stroke-linecap="round"/>
  <!-- Dress -->
  <polygon points="90,115 40,240 140,240" fill="url(#anuDress)" stroke="#881337" stroke-width="3"/>
  <!-- Legs -->
  <rect x="65" y="240" width="14" height="45" rx="4" fill="#FED7AA"/>
  <rect x="101" y="240" width="14" height="45" rx="4" fill="#FED7AA"/>
  <ellipse cx="72" cy="288" rx="12" ry="6" fill="#1C1917"/>
  <ellipse cx="108" cy="288" rx="12" ry="6" fill="#1C1917"/>
</svg>
""")

    # Meera Slipping (Flailing arms, sliding foot)
    save_svg("SPR_Meera_Slip.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 260 300" width="260" height="300">
  <defs>
    <linearGradient id="meeraDress" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#3B82F6"/>
      <stop offset="100%" stop-color="#1D4ED8"/>
    </linearGradient>
  </defs>
  <!-- Tilted Body / Slip Pose -->
  <g transform="translate(130,150) rotate(25) translate(-130,-150)">
    <!-- Head & Hair -->
    <circle cx="130" cy="70" r="38" fill="#18181B"/>
    <circle cx="130" cy="75" r="30" fill="#FDE68A"/>
    <!-- Startled Eyes & Open Mouth -->
    <circle cx="120" cy="72" r="5" fill="#000"/>
    <circle cx="140" cy="72" r="5" fill="#000"/>
    <ellipse cx="130" cy="88" rx="7" ry="9" fill="#991B1B"/>
    <!-- Dress -->
    <polygon points="130,115 80,230 180,230" fill="url(#meeraDress)" stroke="#1E3A8A" stroke-width="3"/>
    <!-- Flailing Arms -->
    <line x1="100" y1="130" x2="30" y2="80" stroke="#FDE68A" stroke-width="12" stroke-linecap="round"/>
    <line x1="160" y1="130" x2="230" y2="90" stroke="#FDE68A" stroke-width="12" stroke-linecap="round"/>
    <!-- Slipping Legs -->
    <line x1="105" y1="230" x2="60" y2="280" stroke="#FDE68A" stroke-width="12" stroke-linecap="round"/>
    <line x1="155" y1="230" x2="190" y2="275" stroke="#FDE68A" stroke-width="12" stroke-linecap="round"/>
  </g>
</svg>
""")

    # Meera Happy & Waving
    save_svg("SPR_Meera_Happy.svg", """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 200 300" width="200" height="300">
  <defs>
    <linearGradient id="meeraDress" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#3B82F6"/>
      <stop offset="100%" stop-color="#1D4ED8"/>
    </linearGradient>
  </defs>
  <circle cx="100" cy="70" r="38" fill="#18181B"/>
  <circle cx="100" cy="75" r="30" fill="#FDE68A"/>
  <circle cx="90" cy="75" r="4" fill="#000"/>
  <circle cx="110" cy="75" r="4" fill="#000"/>
  <path d="M 94,88 Q 100,96 106,88" fill="none" stroke="#000" stroke-width="2.5" stroke-linecap="round"/>
  <!-- Dress -->
  <polygon points="100,115 50,240 150,240" fill="url(#meeraDress)" stroke="#1E3A8A" stroke-width="3"/>
  <!-- Waving Hand -->
  <line x1="130" y1="130" x2="175" y2="85" stroke="#FDE68A" stroke-width="12" stroke-linecap="round"/>
  <!-- Legs -->
  <rect x="75" y="240" width="14" height="45" rx="4" fill="#FDE68A"/>
  <rect x="111" y="240" width="14" height="45" rx="4" fill="#FDE68A"/>
  <ellipse cx="82" cy="288" rx="12" ry="6" fill="#1C1917"/>
  <ellipse cx="118" cy="288" rx="12" ry="6" fill="#1C1917"/>
</svg>
""")

    print(f"All Unit 8 SVGs generated successfully in {ART_DIR}!")

if __name__ == "__main__":
    main()
