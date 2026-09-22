import os
import math
from PIL import Image, ImageDraw, ImageFilter

ART_DIR = os.path.dirname(os.path.abspath(__file__))

def create_9slice_button(filename, base_color_top, base_color_bottom, border_color, bevel_shadow_color, size=(128, 128), radius=28):
    w, h = size
    scale = 4
    sw, sh = w * scale, h * scale
    s_radius = radius * scale
    
    img = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # 1. Soft Bottom Drop Shadow
    draw.rounded_rectangle([2 * scale, 6 * scale, sw - 2 * scale, sh - 1], radius=s_radius, fill=(0, 0, 0, 60))
    
    # 2. Bottom 3D Bevel
    draw.rounded_rectangle([2 * scale, 2 * scale, sw - 2 * scale, sh - 4 * scale], radius=s_radius, fill=bevel_shadow_color)
    
    # 3. Main Button Face Gradient
    face_top = 2 * scale
    face_bottom = sh - 10 * scale
    face_img = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))
    face_draw = ImageDraw.Draw(face_img)
    
    for y in range(face_top, face_bottom):
        t = (y - face_top) / max(1, (face_bottom - face_top))
        t_smooth = t * t * (3 - 2 * t)
        r = int(base_color_top[0] * (1 - t_smooth) + base_color_bottom[0] * t_smooth)
        g = int(base_color_top[1] * (1 - t_smooth) + base_color_bottom[1] * t_smooth)
        b = int(base_color_top[2] * (1 - t_smooth) + base_color_bottom[2] * t_smooth)
        face_draw.line([(0, y), (sw, y)], fill=(r, g, b, 255))
        
    mask = Image.new("L", (sw, sh), 0)
    mask_draw = ImageDraw.Draw(mask)
    mask_draw.rounded_rectangle([2 * scale, face_top, sw - 2 * scale, face_bottom], radius=s_radius, fill=255)
    img.paste(face_img, (0, 0), mask)
    
    # 4. Top Gloss Highlight Arc
    gloss_h = int((face_bottom - face_top) * 0.45)
    gloss_mask = Image.new("L", (sw, sh), 0)
    gloss_draw = ImageDraw.Draw(gloss_mask)
    gloss_draw.rounded_rectangle([4 * scale, face_top + 2 * scale, sw - 4 * scale, face_top + gloss_h], radius=int(s_radius * 0.8), fill=90)
    
    gloss_layer = Image.new("RGBA", (sw, sh), (255, 255, 255, 255))
    img.paste(gloss_layer, (0, 0), gloss_mask)
    
    # 5. Crisp Outer Border
    border_draw = ImageDraw.Draw(img)
    border_draw.rounded_rectangle([2 * scale, face_top, sw - 2 * scale, face_bottom], radius=s_radius, outline=border_color, width=int(2.5 * scale))
    
    final_img = img.resize((w, h), Image.Resampling.LANCZOS)
    out_path = os.path.join(ART_DIR, filename)
    final_img.save(out_path, "PNG")
    print(f"Generated 9-slice button: {filename} -> {out_path}")

def create_close_button_icon(filename="Icon_Close_Circle.png", size=(128, 128)):
    w, h = size
    scale = 4
    sw, sh = w * scale, h * scale
    
    img = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    cx, cy = sw // 2, sh // 2
    r = int(54 * scale)
    
    # 1. Soft Shadow
    draw.ellipse([cx - r + 2*scale, cy - r + 5*scale, cx + r - 2*scale, cy + r + 5*scale], fill=(0, 0, 0, 70))
    
    # 2. Bottom 3D Bevel
    draw.ellipse([cx - r, cy - r + 3*scale, cx + r, cy + r + 3*scale], fill=(160, 40, 45, 255))
    
    # 3. Main Gradient Face (Coral / Ruby Red)
    top_color = (245, 85, 95)
    bot_color = (200, 45, 55)
    
    face_img = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))
    face_draw = ImageDraw.Draw(face_img)
    for y in range(cy - r, cy + r):
        t = (y - (cy - r)) / max(1, 2 * r)
        r_col = int(top_color[0] * (1 - t) + bot_color[0] * t)
        g_col = int(top_color[1] * (1 - t) + bot_color[1] * t)
        b_col = int(top_color[2] * (1 - t) + bot_color[2] * t)
        face_draw.line([(0, y), (sw, y)], fill=(r_col, g_col, b_col, 255))
        
    mask = Image.new("L", (sw, sh), 0)
    mask_draw = ImageDraw.Draw(mask)
    mask_draw.ellipse([cx - r, cy - r, cx + r, cy + r], fill=255)
    img.paste(face_img, (0, 0), mask)
    
    # 4. White Outer Ring Border
    draw.ellipse([cx - r, cy - r, cx + r, cy + r], outline=(255, 255, 255, 230), width=int(3.5 * scale))
    
    # 5. Bold Crisp White 'X' Cross
    x_len = int(20 * scale)
    x_width = int(7 * scale)
    
    draw.line([(cx - x_len, cy - x_len), (cx + x_len, cy + x_len)], fill=(255, 255, 255, 255), width=x_width)
    draw.line([(cx - x_len, cy + x_len), (cx + x_len, cy - x_len)], fill=(255, 255, 255, 255), width=x_width)
    
    cap_r = x_width // 2
    for px, py in [(cx - x_len, cy - x_len), (cx + x_len, cy + x_len), (cx - x_len, cy + x_len), (cx + x_len, cy - x_len)]:
        draw.ellipse([px - cap_r, py - cap_r, px + cap_r, py + cap_r], fill=(255, 255, 255, 255))
        
    final_img = img.resize((w, h), Image.Resampling.LANCZOS)
    out_path = os.path.join(ART_DIR, filename)
    final_img.save(out_path, "PNG")
    print(f"Generated Close Button Icon: {filename} -> {out_path}")

def generate_all():
    # 1. Emerald Green (Primary Action)
    create_9slice_button(
        "UI_Button_Green.png",
        base_color_top=(52, 199, 89),
        base_color_bottom=(34, 154, 62),
        border_color=(25, 120, 48, 220),
        bevel_shadow_color=(20, 95, 38, 255)
    )
    
    # 2. Golden Amber (Special / Weekly Check)
    create_9slice_button(
        "UI_Button_Gold.png",
        base_color_top=(255, 195, 45),
        base_color_bottom=(225, 145, 20),
        border_color=(185, 110, 10, 220),
        bevel_shadow_color=(150, 85, 8, 255)
    )
    
    # 3. Sky Blue (Navigation / Secondary)
    create_9slice_button(
        "UI_Button_Blue.png",
        base_color_top=(58, 160, 235),
        base_color_bottom=(32, 115, 185),
        border_color=(22, 85, 145, 220),
        bevel_shadow_color=(16, 68, 120, 255)
    )
    
    # 4. Slate Grey (Next / Skip Action)
    create_9slice_button(
        "UI_Button_Grey.png",
        base_color_top=(130, 142, 158),
        base_color_bottom=(95, 106, 122),
        border_color=(68, 78, 92, 220),
        bevel_shadow_color=(50, 58, 70, 255)
    )
    
    # 5. Soft Orange / Tangerine (Kindness Marble)
    create_9slice_button(
        "UI_Button_Orange.png",
        base_color_top=(255, 140, 40),
        base_color_bottom=(225, 95, 15),
        border_color=(185, 70, 10, 220),
        bevel_shadow_color=(145, 50, 8, 255)
    )

    # 6. Royal Purple (Teacher Menu)
    create_9slice_button(
        "UI_Button_Purple.png",
        base_color_top=(160, 110, 215),
        base_color_bottom=(120, 70, 180),
        border_color=(90, 45, 145, 220),
        bevel_shadow_color=(70, 32, 115, 255)
    )

    # 7. Warm Ruby Red (Reset / Danger)
    create_9slice_button(
        "UI_Button_Red.png",
        base_color_top=(245, 80, 85),
        base_color_bottom=(205, 45, 55),
        border_color=(165, 30, 40, 220),
        bevel_shadow_color=(130, 20, 30, 255)
    )
    
    # 8. Disabled State
    create_9slice_button(
        "UI_Button_Disabled.png",
        base_color_top=(195, 200, 205),
        base_color_bottom=(170, 175, 180),
        border_color=(145, 150, 155, 180),
        bevel_shadow_color=(125, 130, 135, 255)
    )
    
    # 9. Close Icon
    create_close_button_icon("Icon_Close_Circle.png")

if __name__ == "__main__":
    generate_all()
