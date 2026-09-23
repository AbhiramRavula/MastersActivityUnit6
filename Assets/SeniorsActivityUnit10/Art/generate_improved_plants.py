import os
import math
from PIL import Image, ImageDraw, ImageFilter

ART_DIR = os.path.dirname(os.path.abspath(__file__))

HABITS = [
    {
        "key": "Water",
        "name": "Dewdrop Lily",
        "flower_pri": (35, 145, 225),
        "flower_acc": (110, 210, 255),
        "flower_center": (255, 245, 140),
        "leaf_style": "long_arched",
        "stem_col": (40, 125, 45),
        "leaf_dark": (45, 145, 55),
        "leaf_light": (90, 195, 80),
        "vein_col": (140, 225, 125)
    },
    {
        "key": "Sleep",
        "name": "Night Lavender",
        "flower_pri": (115, 85, 215),
        "flower_acc": (175, 145, 255),
        "flower_center": (245, 230, 255),
        "leaf_style": "slender",
        "stem_col": (35, 115, 50),
        "leaf_dark": (40, 135, 60),
        "leaf_light": (80, 180, 85),
        "vein_col": (130, 215, 135)
    },
    {
        "key": "Outside",
        "name": "Sun Blossom",
        "flower_pri": (245, 175, 25),
        "flower_acc": (255, 220, 70),
        "flower_center": (150, 80, 20),
        "leaf_style": "broad",
        "stem_col": (50, 140, 40),
        "leaf_dark": (55, 160, 50),
        "leaf_light": (105, 210, 75),
        "vein_col": (160, 235, 120)
    },
    {
        "key": "Read",
        "name": "Wise Orchid",
        "flower_pri": (30, 165, 125),
        "flower_acc": (85, 220, 175),
        "flower_center": (255, 245, 160),
        "leaf_style": "elegant_oval",
        "stem_col": (38, 120, 48),
        "leaf_dark": (42, 140, 62),
        "leaf_light": (85, 190, 95),
        "vein_col": (145, 225, 145)
    },
    {
        "key": "Quiet",
        "name": "Peace Lotus",
        "flower_pri": (220, 225, 235),
        "flower_acc": (250, 252, 255),
        "flower_center": (255, 220, 90),
        "leaf_style": "rounded_broad",
        "stem_col": (35, 115, 55),
        "leaf_dark": (45, 135, 65),
        "leaf_light": (90, 185, 95),
        "vein_col": (140, 220, 140)
    },
    {
        "key": "Walk",
        "name": "Morning Glory",
        "flower_pri": (230, 70, 140),
        "flower_acc": (255, 140, 195),
        "flower_center": (255, 240, 160),
        "leaf_style": "heart_tipped",
        "stem_col": (42, 128, 48),
        "leaf_dark": (48, 150, 58),
        "leaf_light": (95, 200, 85),
        "vein_col": (150, 230, 130)
    },
    {
        "key": "Give",
        "name": "Heart Bloom",
        "flower_pri": (225, 50, 60),
        "flower_acc": (255, 115, 125),
        "flower_center": (255, 230, 130),
        "leaf_style": "lush_oval",
        "stem_col": (40, 125, 45),
        "leaf_dark": (46, 145, 55),
        "leaf_light": (92, 195, 82),
        "vein_col": (145, 225, 130)
    },
    {
        "key": "Family",
        "name": "Tree of Life",
        "flower_pri": (225, 150, 35),
        "flower_acc": (255, 205, 85),
        "flower_center": (160, 95, 25),
        "leaf_style": "canopy_clustered",
        "stem_col": (95, 70, 35),
        "leaf_dark": (50, 145, 50),
        "leaf_light": (100, 198, 78),
        "vein_col": (155, 230, 125)
    }
]

def draw_organic_leaf(draw, base_x, base_y, length, width, angle_deg, dark_col, light_col, vein_col, golden_trim=False):
    """Draws a pointed, organic leaf physically connected to base_x, base_y."""
    rad = math.radians(angle_deg)
    cos_a = math.cos(rad)
    sin_a = math.sin(rad)
    
    # Leaf tip
    tip_x = base_x + length * cos_a
    tip_y = base_y + length * sin_a
    
    # Mid-body bulge
    mid_dist = length * 0.45
    mid_x = base_x + mid_dist * cos_a
    mid_y = base_y + mid_dist * sin_a
    
    perp_x = -sin_a * (width * 0.5)
    perp_y = cos_a * (width * 0.5)
    
    p_base = (base_x, base_y)
    p_left = (mid_x + perp_x, mid_y + perp_y)
    p_tip = (tip_x, tip_y)
    p_right = (mid_x - perp_x, mid_y - perp_y)
    
    # Base dark side (shaded)
    draw.polygon([p_base, p_right, p_tip], fill=dark_col + (255,))
    # Light side (illuminated)
    draw.polygon([p_base, p_left, p_tip], fill=light_col + (255,))
    
    # Golden border on Stage 11
    if golden_trim:
        draw.line([p_base, p_left, p_tip, p_right, p_base], fill=(255, 230, 90, 240), width=2)
    
    # Central vein line connecting base directly to tip
    vein_c = (255, 245, 140, 255) if golden_trim else vein_col + (255,)
    draw.line([p_base, p_tip], fill=vein_c, width=max(1, int(width * 0.08)))

def draw_flower_bloom(draw, fx, fy, size, col_pri, col_acc, col_cent, is_golden=False):
    """Draws a balanced, tasteful flower bloom with petals and pistil."""
    num_petals = 5
    petal_len = size * 0.75
    petal_w = size * 0.42
    
    for i in range(num_petals):
        angle = (360 / num_petals) * i
        rad = math.radians(angle)
        px = fx + petal_len * math.cos(rad)
        py = fy + petal_len * math.sin(rad)
        # Petal outer
        draw.ellipse([px - petal_w, py - petal_w, px + petal_w, py + petal_w], fill=col_pri + (255,))
        # Petal highlight
        hw = petal_w * 0.6
        draw.ellipse([px - hw, py - hw, px + hw, py + hw], fill=col_acc + (255,))
        if is_golden:
            draw.ellipse([px - hw*0.4, py - hw*0.4, px + hw*0.4, py + hw*0.4], fill=(255, 250, 180, 255))
            
    # Central pistil
    cr = size * 0.35
    draw.ellipse([fx - cr, fy - cr, fx + cr, fy + cr], fill=col_cent + (255,))
    draw.ellipse([fx - cr*0.5, fy - cr*0.5, fx + cr*0.5, fy + cr*0.5], fill=(255, 255, 210, 255))

def draw_small_bud(draw, bx, by, size, col_pri, col_acc):
    """Draws a delicate swelling bud at a branch tip."""
    # Green calyx
    draw.ellipse([bx - size*0.5, by - size*0.3, bx + size*0.5, by + size*0.6], fill=(50, 140, 50, 255))
    # Colored petal bud
    draw.ellipse([bx - size*0.35, by - size*0.6, bx + size*0.35, by + size*0.2], fill=col_pri + (255,))
    draw.ellipse([bx - size*0.18, by - size*0.45, bx + size*0.18, by], fill=col_acc + (255,))

def generate_all_plants():
    w, h = 360, 420
    cx = w // 2
    base_y = 390  # Soil level in the pot

    for habit in HABITS:
        hkey = habit["key"]
        stem_col = habit["stem_col"]
        leaf_dark = habit["leaf_dark"]
        leaf_light = habit["leaf_light"]
        vein_col = habit["vein_col"]
        col_pri = habit["flower_pri"]
        col_acc = habit["flower_acc"]
        col_cent = habit["flower_center"]

        for stage in range(1, 12):
            img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
            draw = ImageDraw.Draw(img)

            # Golden aura on Stage 11
            if stage == 11:
                glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
                gdraw = ImageDraw.Draw(glow)
                gdraw.ellipse([cx - 140, 50, cx + 140, 330], fill=(255, 225, 90, 80))
                gdraw.ellipse([cx - 100, 80, cx + 100, 300], fill=(255, 240, 140, 110))
                glow = glow.filter(ImageFilter.GaussianBlur(14))
                img.alpha_composite(glow)

            # Growth metrics per stage
            # Stems gradually thicken and lengthen
            stem_h = int(50 + (stage - 1) * 27)  # from 50px (Stage 1) to 320px (Stage 11)
            stem_w = max(4, int(5 + (stage - 1) * 0.9))
            top_y = base_y - stem_h

            # Draw Main Stem (tapering slightly upward)
            draw.line([(cx, base_y), (cx, top_y)], fill=stem_col + (255,), width=stem_w)

            # Determine nodes and branches
            # Every leaf is rooted at an actual stem coordinate
            if stage == 1:
                # Stage 1: Seedling sprout with 2 curved baby cotyledons
                draw_organic_leaf(draw, cx, top_y + 8, 32, 16, -145, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, top_y + 8, 32, 16, -35, leaf_dark, leaf_light, vein_col)

            elif stage == 2:
                # Stage 2: Small young sprout (4 leaves: 2 lower, 2 at top)
                draw_organic_leaf(draw, cx, base_y - 25, 34, 17, -155, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, base_y - 25, 34, 17, -25, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, top_y + 6, 38, 18, -135, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, top_y + 6, 38, 18, -45, leaf_dark, leaf_light, vein_col)

            elif stage == 3:
                # Stage 3: Stalk with 6 leaves in 3 tiers
                nodes = [base_y - 30, base_y - 70, top_y + 6]
                for idx, ny in enumerate(nodes):
                    l_len = 36 + idx * 4
                    l_w = 18 + idx * 2
                    draw_organic_leaf(draw, cx, ny, l_len, l_w, -155 + idx * 8, leaf_dark, leaf_light, vein_col)
                    draw_organic_leaf(draw, cx, ny, l_len, l_w, -25 - idx * 8, leaf_dark, leaf_light, vein_col)

            elif stage == 4:
                # Stage 4: Sturdy plant with 8 leaves and first subtle side branches
                # Lower side petioles
                b1_end = (cx - 35, base_y - 65)
                b2_end = (cx + 35, base_y - 75)
                draw.line([(cx, base_y - 45), b1_end], fill=stem_col + (255,), width=max(3, stem_w - 2))
                draw.line([(cx, base_y - 55), b2_end], fill=stem_col + (255,), width=max(3, stem_w - 2))

                # Leaves attached to branches and main stem (8 total)
                draw_organic_leaf(draw, b1_end[0], b1_end[1], 44, 20, -160, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, b1_end[0], b1_end[1], 40, 18, -125, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, b2_end[0], b2_end[1], 44, 20, -20, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, b2_end[0], b2_end[1], 40, 18, -55, leaf_dark, leaf_light, vein_col)

                draw_organic_leaf(draw, cx, base_y - 110, 46, 21, -145, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, base_y - 120, 46, 21, -35, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, top_y + 6, 42, 19, -135, leaf_dark, leaf_light, vein_col)
                draw_organic_leaf(draw, cx, top_y + 6, 42, 19, -45, leaf_dark, leaf_light, vein_col)

            else:
                # Stages 5 through 11: Rich branching architecture
                # Left primary branch
                lb1_start = base_y - int(stem_h * 0.32)
                lb1_mid = (cx - 45, lb1_start - 35)
                lb1_end = (cx - 75, lb1_start - 55)
                draw.line([(cx, lb1_start), lb1_mid, lb1_end], fill=stem_col + (255,), width=max(3, stem_w - 3))

                # Right primary branch
                rb1_start = base_y - int(stem_h * 0.42)
                rb1_mid = (cx + 45, rb1_start - 35)
                rb1_end = (cx + 75, rb1_start - 55)
                draw.line([(cx, rb1_start), rb1_mid, rb1_end], fill=stem_col + (255,), width=max(3, stem_w - 3))

                # Upper left secondary branch (Stages 6+)
                if stage >= 6:
                    lb2_start = base_y - int(stem_h * 0.65)
                    lb2_end = (cx - 55, lb2_start - 35)
                    draw.line([(cx, lb2_start), lb2_end], fill=stem_col + (255,), width=max(2, stem_w - 4))
                else:
                    lb2_end = None

                # Upper right secondary branch (Stages 6+)
                if stage >= 6:
                    rb2_start = base_y - int(stem_h * 0.72)
                    rb2_end = (cx + 55, rb2_start - 35)
                    draw.line([(cx, rb2_start), rb2_end], fill=stem_col + (255,), width=max(2, stem_w - 4))
                else:
                    rb2_end = None

                is_gold = (stage == 11)

                # 1. Lower stem leaves (always lush and rooted)
                draw_organic_leaf(draw, cx, base_y - 25, 46, 21, -160, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, cx, base_y - 25, 46, 21, -20, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, cx, base_y - 55, 48, 22, -150, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, cx, base_y - 65, 48, 22, -30, leaf_dark, leaf_light, vein_col, is_gold)

                # 2. Left branch leaves (attached to lb1_mid & lb1_end)
                draw_organic_leaf(draw, lb1_mid[0], lb1_mid[1], 46, 21, -165, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, lb1_mid[0], lb1_mid[1], 44, 20, -120, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, lb1_end[0], lb1_end[1], 46, 21, -170, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, lb1_end[0], lb1_end[1], 42, 19, -110, leaf_dark, leaf_light, vein_col, is_gold)

                # 3. Right branch leaves (attached to rb1_mid & rb1_end)
                draw_organic_leaf(draw, rb1_mid[0], rb1_mid[1], 46, 21, -15, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, rb1_mid[0], rb1_mid[1], 44, 20, -60, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, rb1_end[0], rb1_end[1], 46, 21, -10, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, rb1_end[0], rb1_end[1], 42, 19, -70, leaf_dark, leaf_light, vein_col, is_gold)

                # 4. Upper foliage (mid stem & secondary branches)
                if lb2_end:
                    draw_organic_leaf(draw, lb2_end[0], lb2_end[1], 44, 20, -155, leaf_dark, leaf_light, vein_col, is_gold)
                    draw_organic_leaf(draw, lb2_end[0], lb2_end[1], 40, 18, -115, leaf_dark, leaf_light, vein_col, is_gold)
                if rb2_end:
                    draw_organic_leaf(draw, rb2_end[0], rb2_end[1], 44, 20, -25, leaf_dark, leaf_light, vein_col, is_gold)
                    draw_organic_leaf(draw, rb2_end[0], rb2_end[1], 40, 18, -65, leaf_dark, leaf_light, vein_col, is_gold)

                # 5. Top crown leaves (clustering below stem tip)
                draw_organic_leaf(draw, cx, top_y + 25, 45, 20, -145, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, cx, top_y + 25, 45, 20, -35, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, cx, top_y + 12, 42, 19, -135, leaf_dark, leaf_light, vein_col, is_gold)
                draw_organic_leaf(draw, cx, top_y + 12, 42, 19, -45, leaf_dark, leaf_light, vein_col, is_gold)

                # Extra dense foliage for high stages (8 to 11)
                if stage >= 8:
                    draw_organic_leaf(draw, cx, base_y - int(stem_h * 0.52), 44, 20, -150, leaf_dark, leaf_light, vein_col, is_gold)
                    draw_organic_leaf(draw, cx, base_y - int(stem_h * 0.52), 44, 20, -30, leaf_dark, leaf_light, vein_col, is_gold)
                if stage >= 10:
                    draw_organic_leaf(draw, cx - 25, top_y + 40, 42, 19, -160, leaf_dark, leaf_light, vein_col, is_gold)
                    draw_organic_leaf(draw, cx + 25, top_y + 40, 42, 19, -20, leaf_dark, leaf_light, vein_col, is_gold)

                # Balanced Flowers & Buds (Never overwhelming the greenery)
                if stage == 5:
                    # 1 small swelling bud at top
                    draw_small_bud(draw, cx, top_y - 4, 18, col_pri, col_acc)
                elif stage == 6:
                    # 2 buds: 1 central, 1 on left branch
                    draw_small_bud(draw, cx, top_y - 6, 20, col_pri, col_acc)
                    draw_small_bud(draw, lb1_end[0] - 4, lb1_end[1] - 4, 16, col_pri, col_acc)
                elif stage == 7:
                    # 1 graceful open flower at top + 1 side bud
                    draw_small_bud(draw, lb1_end[0] - 4, lb1_end[1] - 4, 16, col_pri, col_acc)
                    draw_flower_bloom(draw, cx, top_y - 10, 24, col_pri, col_acc, col_cent)
                elif stage == 8:
                    # 2 open blooms: top and right branch
                    draw_flower_bloom(draw, cx, top_y - 10, 25, col_pri, col_acc, col_cent)
                    draw_flower_bloom(draw, rb1_end[0] + 5, rb1_end[1] - 8, 21, col_pri, col_acc, col_cent)
                elif stage == 9:
                    # 2 blooms + 1 side bud
                    draw_flower_bloom(draw, cx, top_y - 12, 26, col_pri, col_acc, col_cent)
                    draw_flower_bloom(draw, rb1_end[0] + 5, rb1_end[1] - 8, 22, col_pri, col_acc, col_cent)
                    draw_flower_bloom(draw, lb1_end[0] - 5, lb1_end[1] - 8, 22, col_pri, col_acc, col_cent)
                elif stage == 10:
                    # 3 tasteful blooms nestled in rich greenery
                    draw_flower_bloom(draw, cx, top_y - 14, 27, col_pri, col_acc, col_cent)
                    draw_flower_bloom(draw, lb1_end[0] - 6, lb1_end[1] - 10, 23, col_pri, col_acc, col_cent)
                    draw_flower_bloom(draw, rb1_end[0] + 6, rb1_end[1] - 10, 23, col_pri, col_acc, col_cent)
                elif stage == 11:
                    # Stage 11: Golden Bloom Masterpiece
                    # 3 radiant crowning blooms with golden centers + golden sparkles
                    draw_flower_bloom(draw, cx, top_y - 15, 30, col_pri, col_acc, col_cent, is_golden=True)
                    draw_flower_bloom(draw, lb1_end[0] - 8, lb1_end[1] - 10, 25, col_pri, col_acc, col_cent, is_golden=True)
                    draw_flower_bloom(draw, rb1_end[0] + 8, rb1_end[1] - 10, 25, col_pri, col_acc, col_cent, is_golden=True)

                    # Golden star sparkles around the crowning flowers
                    sparkles = [
                        (cx - 50, top_y - 25), (cx + 50, top_y - 25),
                        (cx, top_y - 45), (lb1_end[0] - 25, lb1_end[1] - 25),
                        (rb1_end[0] + 25, rb1_end[1] - 25)
                    ]
                    for sx, sy in sparkles:
                        sp_poly = [
                            (sx, sy - 10), (sx + 3, sy - 3), (sx + 10, sy), (sx + 3, sy + 3),
                            (sx, sy + 10), (sx - 3, sy + 3), (sx - 10, sy), (sx - 3, sy - 3)
                        ]
                        draw.polygon(sp_poly, fill=(255, 245, 140, 255))

            filename = f"Plant_{hkey}_{stage}.png"
            fpath = os.path.join(ART_DIR, filename)
            img.save(fpath, "PNG")

    print("Successfully generated all 88 improved plant sprites (8 habits x 11 stages)!")

if __name__ == "__main__":
    generate_all_plants()
