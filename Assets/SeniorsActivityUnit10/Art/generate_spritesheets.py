import os
from PIL import Image

ART_DIR = os.path.dirname(os.path.abspath(__file__))

def create_plants_spritesheet():
    habits = ["Water", "Sleep", "Outside", "Read", "Quiet", "Walk", "Give", "Family"]
    cell_w, cell_h = 360, 420
    cols = 6
    rows = 8
    
    sheet = Image.new("RGBA", (cols * cell_w, rows * cell_h), (0, 0, 0, 0))
    
    for r, habit in enumerate(habits):
        for c in range(1, 7):
            fname = f"Plant_{habit}_{c}.png"
            fpath = os.path.join(ART_DIR, fname)
            if os.path.exists(fpath):
                img = Image.open(fpath)
                sheet.paste(img, ( (c - 1) * cell_w, r * cell_h ), img)
                
    sheet.save(os.path.join(ART_DIR, "Plants_Growth_Spritesheet.png"))
    print("Created Plants_Growth_Spritesheet.png (2160x3360)")

def create_icons_spritesheet():
    habits = ["Water", "Sleep", "Outside", "Read", "Quiet", "Walk", "Give", "Family"]
    cell_size = 128
    cols = 4
    rows = 2
    sheet = Image.new("RGBA", (cols * cell_size, rows * cell_size), (0, 0, 0, 0))
    
    for idx, habit in enumerate(habits):
        r = idx // cols
        c = idx % cols
        fname = f"Icon_{habit}.png"
        fpath = os.path.join(ART_DIR, fname)
        if os.path.exists(fpath):
            img = Image.open(fpath)
            sheet.paste(img, (c * cell_size, r * cell_size), img)
            
    sheet.save(os.path.join(ART_DIR, "Habit_Icons_Spritesheet.png"))
    print("Created Habit_Icons_Spritesheet.png (512x256)")

def create_jar_spritesheet():
    stages = ["Jar_Glass_Empty.png", "Jar_Glass_Stage1.png", "Jar_Glass_Stage2.png", "Jar_Glass_Stage3.png", "Jar_Glass_Full.png"]
    cell_w, cell_h = 260, 340
    cols = 5
    rows = 1
    sheet = Image.new("RGBA", (cols * cell_w, rows * cell_h), (0, 0, 0, 0))
    
    for c, fname in enumerate(stages):
        fpath = os.path.join(ART_DIR, fname)
        if os.path.exists(fpath):
            img = Image.open(fpath)
            sheet.paste(img, (c * cell_w, 0), img)
            
    sheet.save(os.path.join(ART_DIR, "Kindness_Jar_Spritesheet.png"))
    print("Created Kindness_Jar_Spritesheet.png (1300x340)")

if __name__ == "__main__":
    print("Generating Unit 10 Sprite Sheets...")
    create_plants_spritesheet()
    create_icons_spritesheet()
    create_jar_spritesheet()
    print("All Sprite Sheets Generated Successfully!")
