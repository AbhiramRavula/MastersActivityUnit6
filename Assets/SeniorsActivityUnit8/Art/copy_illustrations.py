import shutil
import os

src_dir = r"C:\Users\abhir\.gemini\antigravity-ide\brain\3f4ef38e-52f8-4f63-ba2a-4010a0ca8cf6"
dest_dir = r"C:\Users\abhir\SR_InterProjects\Repos\MastersActivity-Unit-6\Assets\SeniorsActivityUnit8\Art"

copies = [
    ("u8_washroom_background_1789725022629.jpg", "BG_Washroom.jpg"),
    ("u8_handwash_screen_1789725046598.jpg", "BG_HandwashScreen.jpg"),
    ("u8_meera_slip_1789725072921.jpg", "ILLUST_MeeraSlip.jpg"),
    ("u8_corridor_door_1789725102458.jpg", "BG_CorridorDoor.jpg")
]

for src_name, dest_name in copies:
    src_path = os.path.join(src_dir, src_name)
    dest_path = os.path.join(dest_dir, dest_name)
    if os.path.exists(src_path):
        shutil.copy2(src_path, dest_path)
        print(f"Copied {src_name} -> {dest_name}")
    else:
        print(f"Not found: {src_path}")
