import os
import math
from PIL import Image, ImageDraw, ImageFilter

ART_DIR = os.path.dirname(os.path.abspath(__file__))

def create_9slice_box():
    # 128x128 rounded box with 32px corner radius for 9-slice
    size = 128
    radius = 32
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    draw.rounded_rectangle([0, 0, size - 1, size - 1], radius=radius, fill=(255, 255, 255, 255))
    img.save(os.path.join(ART_DIR, "UI_RoundedBox_9Slice.png"))
    print("Created UI_RoundedBox_9Slice.png")

def create_garden_background():
    # If high quality illustration already exists, preserve it!
    bg_path = os.path.join(ART_DIR, "BG_Garden_Smartboard.png")
    if os.path.exists(bg_path) and os.path.getsize(bg_path) > 300000:
        print("Preserved high-quality BG_Garden_Smartboard.png")
        return

def create_pot_sprite():
    # 320x240 Classic Sturdy Terracotta Flower Pot
    w, h = 320, 240
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    cx = w // 2

    # 1. Subtle soft shadow underneath
    draw.ellipse([cx - 85, 212, cx + 85, 230], fill=(0, 0, 0, 40))

    # 2. Pot Body (tapering downwards with classic sturdy proportions)
    top_w, btm_w = 236, 168
    top_y, btm_y = 62, 215

    pot_poly = [
        (cx - top_w // 2, top_y),
        (cx + top_w // 2, top_y),
        (cx + btm_w // 2, btm_y),
        (cx - btm_w // 2, btm_y)
    ]
    draw.polygon(pot_poly, fill=(215, 115, 70, 255))

    # Shading: Warm highlight on left side, deep warm shadow on right side
    draw.polygon([
        (cx - top_w // 2, top_y),
        (cx - top_w // 2 + 32, top_y),
        (cx - btm_w // 2 + 22, btm_y),
        (cx - btm_w // 2, btm_y)
    ], fill=(245, 145, 100, 255))

    draw.polygon([
        (cx + top_w // 2 - 32, top_y),
        (cx + top_w // 2, top_y),
        (cx + btm_w // 2, btm_y),
        (cx + btm_w // 2 - 22, btm_y)
    ], fill=(160, 75, 40, 255))

    # Rounded base curve
    draw.rounded_rectangle([cx - btm_w // 2, btm_y - 8, cx + btm_w // 2, btm_y + 4], radius=6, fill=(185, 90, 50, 255))

    # 3. Pot Rim (Lip)
    rim_w, rim_h = 268, 42
    rim_y = 28
    draw.rounded_rectangle([cx - rim_w // 2, rim_y, cx + rim_w // 2, rim_y + rim_h], radius=10, fill=(225, 125, 78, 255))
    # Rim top highlight strip
    draw.rounded_rectangle([cx - rim_w // 2 + 8, rim_y + 3, cx + rim_w // 2 - 8, rim_y + 14], radius=5, fill=(248, 160, 115, 255))
    # Rim right shadow
    draw.rounded_rectangle([cx + rim_w // 2 - 24, rim_y + 2, cx + rim_w // 2 - 4, rim_y + rim_h - 2], radius=6, fill=(170, 80, 45, 255))

    # 4. Rich Potting Soil Ellipse inside the rim
    soil_w, soil_h = 226, 28
    soil_y = 24
    draw.ellipse([cx - soil_w // 2, soil_y, cx + soil_w // 2, soil_y + soil_h], fill=(75, 45, 25, 255))
    draw.ellipse([cx - soil_w // 2 + 12, soil_y + 4, cx + soil_w // 2 - 12, soil_y + soil_h - 4], fill=(55, 32, 18, 255))

    img.save(os.path.join(ART_DIR, "Pot_Terracotta.png"))
    img.save(os.path.join(ART_DIR, "Pot_Default.png"))
    print("Created classic sturdy Pot_Terracotta.png")

def create_plant_stages():
    # 8 Habit plant types, 6 stages each: 360x420 sprite with plant centered at bottom (anchor 0.5, 0.0)
    w, h = 360, 420
    habits = [
        ("Water", (40, 160, 220), (80, 200, 255), "Dewdrop Lily"),
        ("Sleep", (110, 90, 210), (170, 150, 255), "Night Lavender"),
        ("Outside", (240, 180, 30), (255, 220, 80), "Sun Blossom"),
        ("Read", (46, 175, 125), (100, 230, 180), "Wise Orchid"),
        ("Quiet", (230, 230, 240), (255, 255, 255), "Peace Lotus"),
        ("Walk", (235, 80, 150), (255, 140, 200), "Morning Glory"),
        ("Give", (225, 60, 60), (255, 120, 120), "Heart Bloom"),
        ("Family", (215, 160, 40), (255, 210, 90), "Tree of Life")
    ]

    for habit_key, col_primary, col_accent, name in habits:
        for stage in range(1, 7):
            img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
            draw = ImageDraw.Draw(img)

            cx = w // 2
            base_y = h - 20

            # Stem green colors
            stem_col = (45, 135, 45, 255)
            leaf_col = (70, 175, 65, 255)
            leaf_light = (110, 210, 95, 255)

            if stage == 1:
                # Stage 1: Seedling sprout (tiny 2 leaves)
                stem_h = 45
                draw.line([(cx, base_y), (cx, base_y - stem_h)], fill=stem_col, width=6)
                # Left tiny leaf
                draw.ellipse([cx - 24, base_y - stem_h - 12, cx, base_y - stem_h + 4], fill=leaf_col)
                # Right tiny leaf
                draw.ellipse([cx, base_y - stem_h - 12, cx + 24, base_y - stem_h + 4], fill=leaf_light)

            elif stage == 2:
                # Stage 2: Small young sprout (4 leaves)
                stem_h = 90
                draw.line([(cx, base_y), (cx, base_y - stem_h)], fill=stem_col, width=8)
                # Lower leaves
                draw.ellipse([cx - 38, base_y - 45, cx - 4, base_y - 25], fill=leaf_col)
                draw.ellipse([cx + 4, base_y - 45, cx + 38, base_y - 25], fill=leaf_light)
                # Upper leaves
                draw.ellipse([cx - 44, base_y - stem_h - 10, cx - 2, base_y - stem_h + 12], fill=leaf_col)
                draw.ellipse([cx + 2, base_y - stem_h - 10, cx + 44, base_y - stem_h + 12], fill=leaf_light)

            elif stage == 3:
                # Stage 3: Growing bush / stem with multiple foliage tiers
                stem_h = 160
                draw.line([(cx, base_y), (cx, base_y - stem_h)], fill=stem_col, width=10)
                # Branch left & right
                draw.line([(cx, base_y - 60), (cx - 40, base_y - 90)], fill=stem_col, width=6)
                draw.line([(cx, base_y - 90), (cx + 40, base_y - 120)], fill=stem_col, width=6)
                # Foliage clusters
                draw.ellipse([cx - 65, base_y - 110, cx - 15, base_y - 70], fill=leaf_col)
                draw.ellipse([cx + 15, base_y - 140, cx + 65, base_y - 100], fill=leaf_light)
                draw.ellipse([cx - 50, base_y - 175, cx, base_y - 135], fill=leaf_col)
                draw.ellipse([cx, base_y - 175, cx + 50, base_y - 135], fill=leaf_light)
                # Tiny top bud
                draw.ellipse([cx - 10, base_y - stem_h - 15, cx + 10, base_y - stem_h + 5], fill=col_primary + (255,))

            elif stage == 4:
                # Stage 4: Tall healthy leafy plant with swelling flower buds
                stem_h = 240
                draw.line([(cx, base_y), (cx, base_y - stem_h)], fill=stem_col, width=12)
                # Side branches
                draw.line([(cx, base_y - 80), (cx - 60, base_y - 120)], fill=stem_col, width=8)
                draw.line([(cx, base_y - 120), (cx + 60, base_y - 160)], fill=stem_col, width=8)
                draw.line([(cx, base_y - 170), (cx - 45, base_y - 210)], fill=stem_col, width=7)
                draw.line([(cx, base_y - 190), (cx + 45, base_y - 230)], fill=stem_col, width=7)

                # Rich leaves
                for lx, ly in [(cx - 80, base_y - 135), (cx + 40, base_y - 175), (cx - 65, base_y - 225), (cx + 25, base_y - 245)]:
                    draw.ellipse([lx, ly, lx + 55, ly + 32], fill=leaf_col)
                    draw.ellipse([lx + 10, ly + 4, lx + 45, ly + 26], fill=leaf_light)

                # 3 Buds
                for bx, by in [(cx - 60, base_y - 130), (cx + 60, base_y - 170), (cx, base_y - stem_h - 10)]:
                    draw.ellipse([bx - 16, by - 16, bx + 16, by + 16], fill=col_primary + (255,))
                    draw.ellipse([bx - 8, by - 8, bx + 8, by + 8], fill=col_accent + (255,))

            elif stage == 5:
                # Stage 5: Fully blooming flowers with lush leaves
                stem_h = 290
                draw.line([(cx, base_y), (cx, base_y - stem_h)], fill=stem_col, width=14)
                draw.line([(cx, base_y - 90), (cx - 80, base_y - 150)], fill=stem_col, width=9)
                draw.line([(cx, base_y - 140), (cx + 80, base_y - 200)], fill=stem_col, width=9)

                # Leaves
                for lx, ly in [(cx - 100, base_y - 160), (cx + 50, base_y - 210), (cx - 70, base_y - 240), (cx + 30, base_y - 270)]:
                    draw.ellipse([lx, ly, lx + 65, ly + 36], fill=leaf_col)
                    draw.ellipse([lx + 12, ly + 5, lx + 52, ly + 30], fill=leaf_light)

                # Bloom flowers at 3 heads
                flower_centers = [(cx - 85, base_y - 160), (cx + 85, base_y - 210), (cx, base_y - stem_h - 15)]
                for fx, fy in flower_centers:
                    # 5 petals
                    for angle in range(0, 360, 72):
                        rad = math.radians(angle)
                        px = fx + int(24 * math.cos(rad))
                        py = fy + int(24 * math.sin(rad))
                        draw.ellipse([px - 18, py - 18, px + 18, py + 18], fill=col_primary + (255,))
                        draw.ellipse([px - 10, py - 10, px + 10, py + 10], fill=col_accent + (255,))
                    # Center golden pistil
                    draw.ellipse([fx - 14, fy - 14, fx + 14, fy + 14], fill=(255, 215, 0, 255))
                    draw.ellipse([fx - 7, fy - 7, fx + 7, fy + 7], fill=(255, 240, 150, 255))

            elif stage == 6:
                # Stage 6: Golden Garden Masterpiece (Radiant Golden Glow + Full Bloom + Golden Leaves/Sparkles)
                # Glow background aura
                glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
                gdraw = ImageDraw.Draw(glow)
                gdraw.ellipse([cx - 150, 30, cx + 150, 330], fill=(255, 220, 80, 110))
                gdraw.ellipse([cx - 110, 60, cx + 110, 300], fill=(255, 240, 140, 150))
                glow = glow.filter(ImageFilter.GaussianBlur(15))
                img.alpha_composite(glow)

                stem_h = 310
                draw.line([(cx, base_y), (cx, base_y - stem_h)], fill=(60, 160, 50, 255), width=16)
                draw.line([(cx, base_y - 90), (cx - 90, base_y - 160)], fill=(60, 160, 50, 255), width=11)
                draw.line([(cx, base_y - 140), (cx + 90, base_y - 210)], fill=(60, 160, 50, 255), width=11)

                # Shimmering golden leaves
                for lx, ly in [(cx - 115, base_y - 170), (cx + 60, base_y - 220), (cx - 80, base_y - 255), (cx + 35, base_y - 285)]:
                    draw.ellipse([lx, ly, lx + 75, ly + 40], fill=(80, 190, 70, 255))
                    draw.ellipse([lx + 10, ly + 6, lx + 65, ly + 34], fill=(255, 225, 90, 230))

                # Large Majestic Blooms
                flower_centers = [
                    (cx - 95, base_y - 170),
                    (cx + 95, base_y - 220),
                    (cx, base_y - stem_h - 20),
                    (cx - 40, base_y - 260),
                    (cx + 40, base_y - 280)
                ]
                for fx, fy in flower_centers:
                    for angle in range(0, 360, 60):
                        rad = math.radians(angle)
                        px = fx + int(28 * math.cos(rad))
                        py = fy + int(28 * math.sin(rad))
                        draw.ellipse([px - 22, py - 22, px + 22, py + 22], fill=col_primary + (255,))
                        draw.ellipse([px - 14, py - 14, px + 14, py + 14], fill=(255, 235, 120, 255))
                    draw.ellipse([fx - 18, fy - 18, fx + 18, fy + 18], fill=(255, 200, 0, 255))
                    draw.ellipse([fx - 9, fy - 9, fx + 9, fy + 9], fill=(255, 255, 200, 255))

                # Golden Star Sparkles around
                sparkles = [(cx - 120, 80), (cx + 120, 100), (cx - 70, 40), (cx + 80, 50), (cx, 30)]
                for sx, sy in sparkles:
                    draw.polygon([(sx, sy - 14), (sx + 4, sy - 4), (sx + 14, sy), (sx + 4, sy + 4),
                                  (sx, sy + 14), (sx - 4, sy + 4), (sx - 14, sy), (sx - 4, sy - 4)], fill=(255, 245, 150, 255))

            filename = f"Plant_{habit_key}_{stage}.png"
            img.save(os.path.join(ART_DIR, filename))

    print("Created 48 plant stage sprites (8 types x 6 stages).")

def create_kindness_jar_sprites():
    w, h = 260, 360
    # Create clean marble sprite
    m_size = 128
    m_img = Image.new("RGBA", (m_size, m_size), (0, 0, 0, 0))
    mdraw = ImageDraw.Draw(m_img)
    # Spherical golden glass marble
    cx, cy, radius = 64, 64, 52
    mdraw.ellipse([cx - radius, cy - radius, cx + radius, cy + radius], fill=(215, 145, 15, 255), outline=(160, 95, 10, 255), width=1)
    mdraw.ellipse([cx - radius + 2, cy - radius + 2, cx + radius - 2, cy + radius - 2], fill=(250, 195, 30, 255))
    mdraw.ellipse([cx - radius + 4, cy - radius + 3, cx + radius - 6, cy + radius - 7], fill=(255, 225, 75, 255))
    mdraw.ellipse([cx - radius//2, cy - radius//2 - 2, cx - radius//6, cy - radius//6 - 2], fill=(255, 255, 235, 255))
    m_img.save(os.path.join(ART_DIR, "Marble_Golden.png"))
    print("Created clean Marble_Golden.png")

    jar_stages = [
        ("Jar_Glass_Empty.png", 0),
        ("Jar_Glass_Stage1.png", 6),
        ("Jar_Glass_Stage2.png", 18),
        ("Jar_Glass_Stage3.png", 36),
        ("Jar_Glass_Full.png", 60)
    ]

    for fname, count in jar_stages:
        img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
        draw = ImageDraw.Draw(img)

        # Wooden Cork / Lid at top
        draw.rounded_rectangle([70, 20, 190, 55], radius=8, fill=(195, 140, 85, 255))
        draw.rounded_rectangle([80, 55, 180, 70], radius=4, fill=(165, 115, 65, 255))

        # Glass Jar Body
        jar_rect = [35, 68, 225, 320]
        # Glass fill / soft reflection
        draw.rounded_rectangle(jar_rect, radius=36, fill=(210, 235, 250, 60), outline=(180, 220, 245, 200), width=4)

        # Draw marbles inside if count > 0
        if count > 0:
            import random
            rng = random.Random(42 + count)
            # Marbles fill from bottom up
            max_rows = min(6, (count // 5) + 1)
            drawn = 0
            for row in range(max_rows):
                marbles_in_row = 5 if row % 2 == 0 else 4
                y_pos = 285 - row * 32
                for col in range(marbles_in_row):
                    if drawn >= count:
                        break
                    x_pos = 65 + col * 32 + (16 if row % 2 == 1 else 0) + rng.randint(-3, 3)
                    # draw marble
                    draw.ellipse([x_pos - 14, y_pos - 14, x_pos + 14, y_pos + 14], fill=(245, 190, 35, 255))
                    draw.ellipse([x_pos - 11, y_pos - 11, x_pos + 11, y_pos + 11], fill=(255, 215, 60, 255))
                    draw.ellipse([x_pos - 6, y_pos - 8, x_pos + 2, y_pos], fill=(255, 255, 210, 255))
                    drawn += 1

        # Golden glow if full
        if "Full" in fname:
            glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
            gdraw = ImageDraw.Draw(glow)
            gdraw.rounded_rectangle(jar_rect, radius=36, fill=(255, 215, 60, 100))
            glow = glow.filter(ImageFilter.GaussianBlur(10))
            img.alpha_composite(glow)

        # Glass reflection shine line on left
        draw.arc([48, 85, 120, 300], start=100, end=260, fill=(255, 255, 255, 160), width=6)
        # Warm golden ribbon / jute twine collar around neck (harmonious with garden theme, no red!)
        draw.rounded_rectangle([55, 74, 205, 88], radius=4, fill=(215, 175, 55, 240), outline=(180, 135, 35, 255), width=2)
        draw.rounded_rectangle([80, 77, 180, 85], radius=3, fill=(245, 215, 100, 255))

        img.save(os.path.join(ART_DIR, fname))

    print("Created 5 Kindness Jar stage sprites.")

def create_habit_icons():
    # If high quality 256x256 icons exist, preserve them!
    water_path = os.path.join(ART_DIR, "Icon_Water.png")
    if os.path.exists(water_path) and os.path.getsize(water_path) > 15000:
        print("Preserved high-quality 3D Habit Icons and Spritesheet.")
        return

    # Fallback generator
    habits = ["Water", "Sleep", "Outside", "Read", "Quiet", "Walk", "Give", "Family"]
    for habit in habits:
        img = Image.new("RGBA", (128, 128), (0, 0, 0, 0))
        draw = ImageDraw.Draw(img)
        draw.ellipse([4, 4, 124, 124], fill=(255, 255, 255, 240), outline=(220, 220, 220, 255), width=3)
        img.save(os.path.join(ART_DIR, f"Icon_{habit}.png"))
    print("Created 8 Habit Icon sprites.")

def create_certificate_and_rosette():
    # Certificate Golden Border (960x640)
    w, h = 960, 640
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Cream Parchment background
    draw.rounded_rectangle([10, 10, w - 10, h - 10], radius=24, fill=(255, 252, 240, 255), outline=(215, 175, 65, 255), width=8)
    draw.rounded_rectangle([26, 26, w - 26, h - 26], radius=16, outline=(235, 205, 110, 255), width=3)
    # Ornate corner knots
    corners = [(40, 40), (w - 40, 40), (40, h - 40), (w - 40, h - 40)]
    for cx, cy in corners:
        draw.ellipse([cx - 14, cy - 14, cx + 14, cy + 14], fill=(215, 175, 65, 255))
        draw.ellipse([cx - 8, cy - 8, cx + 8, cy + 8], fill=(255, 235, 140, 255))

    img.save(os.path.join(ART_DIR, "Certificate_Gold_Border.png"))

    # Golden Star Rosette Badge (160x160)
    rw, rh = 160, 160
    rimg = Image.new("RGBA", (rw, rh), (0, 0, 0, 0))
    rdraw = ImageDraw.Draw(rimg)

    # Rosette ribbons hanging down
    rdraw.polygon([(60, 90), (40, 150), (65, 135), (75, 150), (80, 90)], fill=(210, 40, 40, 255))
    rdraw.polygon([(100, 90), (120, 150), (95, 135), (85, 150), (80, 90)], fill=(185, 30, 30, 255))

    # Rosette wheel (circular pleated badge)
    rcx, rcy = 80, 65
    for a in range(0, 360, 30):
        rad = math.radians(a)
        px = rcx + int(45 * math.cos(rad))
        py = rcy + int(45 * math.sin(rad))
        rdraw.ellipse([px - 14, py - 14, px + 14, py + 14], fill=(255, 200, 40, 255))

    rdraw.ellipse([rcx - 38, rcy - 38, rcx + 38, rcy + 38], fill=(245, 175, 25, 255))
    rdraw.ellipse([rcx - 30, rcy - 30, rcx + 30, rcy + 30], fill=(255, 230, 100, 255))
    # Center star
    star_poly = []
    for i in range(10):
        r = 22 if i % 2 == 0 else 10
        ang = math.radians(i * 36 - 90)
        star_poly.append((rcx + int(r * math.cos(ang)), rcy + int(r * math.sin(ang))))
    rdraw.polygon(star_poly, fill=(215, 140, 15, 255))

    rimg.save(os.path.join(ART_DIR, "Icon_Ribbon_Gold.png"))
    print("Created Certificate_Gold_Border.png and Icon_Ribbon_Gold.png")

if __name__ == "__main__":
    print("Generating Unit 10 2D Art Assets...")
    create_9slice_box()
    create_garden_background()
    create_pot_sprite()
    create_plant_stages()
    create_kindness_jar_sprites()
    create_habit_icons()
    create_certificate_and_rosette()
    print("All Unit 10 Art Assets Generated Successfully!")
