import os
import uuid
from PIL import Image, ImageDraw, ImageFilter

ART_DIR = os.path.dirname(os.path.abspath(__file__))

def create_meta_file(png_path, border_x=24, border_y=24, border_z=24, border_w=24):
    meta_path = png_path + ".meta"
    # Generate unique 32-char hex GUID if file doesn't already exist
    existing_guid = None
    if os.path.exists(meta_path):
        with open(meta_path, "r", encoding="utf-8") as f:
            for line in f:
                if line.startswith("guid:"):
                    existing_guid = line.split(":", 1)[1].strip()
                    break
    
    guid = existing_guid if existing_guid else uuid.uuid4().hex

    meta_content = f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: {border_x}, y: {border_y}, z: {border_z}, w: {border_w}}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  cookieLightType: 0
  platformSettings: []
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
    with open(meta_path, "w", encoding="utf-8") as f:
        f.write(meta_content)

def draw_rounded_rect(draw, xy, radius, fill=None, outline=None, width=1):
    draw.rounded_rectangle(xy, radius=radius, fill=fill, outline=outline, width=width)

def generate_title_plaque_wood():
    """Generates a warm, polished storybook wooden signboard for title banners."""
    w, h = 256, 96
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # 1. Soft Drop Shadow
    shadow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    sdraw = ImageDraw.Draw(shadow)
    sdraw.rounded_rectangle([4, 6, w - 4, h - 2], radius=24, fill=(20, 15, 10, 80))
    shadow = shadow.filter(ImageFilter.GaussianBlur(3))
    img.alpha_composite(shadow)
    draw = ImageDraw.Draw(img)

    # 2. Outer Carved Wood Rim (Dark Honey Oak)
    draw_rounded_rect(draw, [4, 2, w - 4, h - 8], radius=24, fill=(115, 68, 36, 255), outline=(75, 42, 20, 255), width=2)
    # Subtle Wood Highlight along top bevel
    draw.line([(24, 4), (w - 24, 4)], fill=(160, 105, 58, 220), width=2)

    # 3. Inner Gold Accent Inlay
    draw_rounded_rect(draw, [10, 8, w - 10, h - 14], radius=18, fill=(195, 145, 55, 255), outline=(145, 100, 30, 255), width=1)
    draw.line([(22, 10), (w - 22, 10)], fill=(245, 215, 120, 255), width=1)

    # 4. Center Parchment Field (Warm cream for text contrast)
    draw_rounded_rect(draw, [14, 12, w - 14, h - 18], radius=14, fill=(255, 252, 244, 255), outline=(215, 185, 130, 200), width=1)
    
    # 5. Corner Brass Rivets
    rivets = [(12, 12), (w - 12, 12), (12, h - 18), (w - 12, h - 18)]
    for rx, ry in rivets:
        draw.ellipse([rx - 2, ry - 2, rx + 2, ry + 2], fill=(225, 175, 65, 255), outline=(100, 65, 20, 255))

    out_path = os.path.join(ART_DIR, "UI_Title_Plaque_Wood.png")
    img.save(out_path, "PNG")
    create_meta_file(out_path, border_x=28, border_y=28, border_z=28, border_w=28)
    print("Generated UI_Title_Plaque_Wood.png")

def generate_card_container_main():
    """Generates a large warm parchment container card with caramel wooden frame."""
    w, h = 256, 256
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))

    # Soft Drop Shadow
    shadow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    sdraw = ImageDraw.Draw(shadow)
    sdraw.rounded_rectangle([4, 6, w - 4, h - 2], radius=28, fill=(20, 18, 15, 75))
    shadow = shadow.filter(ImageFilter.GaussianBlur(4))
    img.alpha_composite(shadow)
    draw = ImageDraw.Draw(img)

    # Outer Caramel Wood Frame
    draw_rounded_rect(draw, [4, 2, w - 4, h - 8], radius=28, fill=(145, 95, 52, 255), outline=(95, 58, 28, 255), width=2)
    draw.line([(28, 4), (w - 28, 4)], fill=(190, 135, 80, 230), width=2)

    # Gold Inset Line
    draw_rounded_rect(draw, [10, 8, w - 10, h - 14], radius=22, fill=(215, 170, 75, 255), outline=(160, 120, 45, 255), width=1)

    # Main Warm Parchment Center
    draw_rounded_rect(draw, [14, 12, w - 14, h - 18], radius=18, fill=(253, 250, 242, 255), outline=(230, 212, 185, 255), width=1)

    out_path = os.path.join(ART_DIR, "UI_Card_Container_Main.png")
    img.save(out_path, "PNG")
    create_meta_file(out_path, border_x=32, border_y=32, border_z=32, border_w=32)
    print("Generated UI_Card_Container_Main.png")

def generate_subcard_plant():
    """Generates the left child card for the plant pot with a soft mint tint and pedestal base."""
    w, h = 256, 256
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Subtle inset shadow
    draw_rounded_rect(draw, [2, 2, w - 2, h - 2], radius=22, fill=(235, 245, 238, 255), outline=(110, 170, 125, 255), width=2)
    # Inner border
    draw_rounded_rect(draw, [6, 6, w - 6, h - 6], radius=18, fill=(245, 252, 247, 255), outline=(195, 225, 205, 255), width=1)
    
    # Wooden display shelf/pedestal line at the bottom
    draw.rounded_rectangle([12, h - 28, w - 12, h - 12], radius=6, fill=(165, 115, 75, 255), outline=(115, 75, 40, 255), width=1)
    draw.line([(16, h - 26), (w - 16, h - 26)], fill=(205, 155, 105, 255), width=1)

    out_path = os.path.join(ART_DIR, "UI_SubCard_Plant.png")
    img.save(out_path, "PNG")
    create_meta_file(out_path, border_x=24, border_y=24, border_z=24, border_w=24)
    print("Generated UI_SubCard_Plant.png")

def generate_subcard_content():
    """Generates the right child card for the dialogue/questions with clean warm ivory paper."""
    w, h = 256, 256
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Soft outer border
    draw_rounded_rect(draw, [2, 2, w - 2, h - 2], radius=22, fill=(245, 240, 230, 255), outline=(175, 160, 140, 255), width=2)
    # Crisp ivory reading surface
    draw_rounded_rect(draw, [6, 6, w - 6, h - 6], radius=18, fill=(255, 255, 252, 255), outline=(225, 218, 205, 255), width=1)
    # Very subtle top paper highlight
    draw.line([(20, 8), (w - 20, 8)], fill=(255, 255, 255, 255), width=1)

    out_path = os.path.join(ART_DIR, "UI_SubCard_Content.png")
    img.save(out_path, "PNG")
    create_meta_file(out_path, border_x=24, border_y=24, border_z=24, border_w=24)
    print("Generated UI_SubCard_Content.png")

def generate_habit_card_normal():
    """Generates warm, tactile recipe cards for the 8 habit cards in Setup screen."""
    w, h = 192, 192
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))

    # Drop shadow
    shadow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    sdraw = ImageDraw.Draw(shadow)
    sdraw.rounded_rectangle([3, 5, w - 3, h - 2], radius=20, fill=(30, 25, 20, 50))
    shadow = shadow.filter(ImageFilter.GaussianBlur(3))
    img.alpha_composite(shadow)
    draw = ImageDraw.Draw(img)

    # Outer Embossed Bevel
    draw_rounded_rect(draw, [3, 2, w - 3, h - 6], radius=20, fill=(215, 200, 175, 255), outline=(170, 150, 125, 255), width=1)
    draw.line([(18, 4), (w - 18, 4)], fill=(245, 235, 215, 240), width=1)

    # Warm Cream Card Center
    draw_rounded_rect(draw, [7, 6, w - 7, h - 10], radius=16, fill=(254, 252, 247, 255), outline=(232, 222, 205, 255), width=1)

    out_path = os.path.join(ART_DIR, "UI_Habit_Card_Normal.png")
    img.save(out_path, "PNG")
    create_meta_file(out_path, border_x=22, border_y=22, border_z=22, border_w=22)
    print("Generated UI_Habit_Card_Normal.png")

def generate_bar_footer_parchment():
    """Generates the bottom selection summary bar on Setup screen."""
    w, h = 256, 80
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Outer gold/wood border
    draw_rounded_rect(draw, [3, 2, w - 3, h - 4], radius=20, fill=(155, 115, 65, 255), outline=(105, 75, 35, 255), width=2)
    draw.line([(20, 4), (w - 20, 4)], fill=(215, 175, 105, 230), width=1)

    # Warm parchment surface
    draw_rounded_rect(draw, [7, 6, w - 7, h - 8], radius=16, fill=(255, 252, 244, 255), outline=(225, 205, 165, 255), width=1)

    out_path = os.path.join(ART_DIR, "UI_Bar_Footer_Parchment.png")
    img.save(out_path, "PNG")
    create_meta_file(out_path, border_x=24, border_y=24, border_z=24, border_w=24)
    print("Generated UI_Bar_Footer_Parchment.png")

if __name__ == "__main__":
    generate_title_plaque_wood()
    generate_card_container_main()
    generate_subcard_plant()
    generate_subcard_content()
    generate_habit_card_normal()
    generate_bar_footer_parchment()
    print("All UI background sprites generated successfully!")
