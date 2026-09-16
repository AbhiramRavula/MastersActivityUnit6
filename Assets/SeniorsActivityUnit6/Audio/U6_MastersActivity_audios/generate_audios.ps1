# Script to generate all Unit 6 Voice-Over audio files using edge-tts

Write-Host "Checking for edge-tts..." -ForegroundColor Cyan
if (-not (Get-Command "edge-tts" -ErrorAction SilentlyContinue)) {
    Write-Host "Installing edge-tts..." -ForegroundColor Yellow
    pip install edge-tts
}

Write-Host "Generating Teacher / Narrator audios..." -ForegroundColor Green
edge-tts --voice en-IN-NeerjaNeural --text "Today, Anu's family is eating out." --write-media "Today Anus family is eating out.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "The food is not here yet... Watch Anu." --write-media "The food is not here yet Watch Anu.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "Quick! Tap the button." --write-media "Quick Tap the button.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "Everybody is happy! One star." --write-media "Everybody is happy One star.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "Now choose. What will Anu eat?" --write-media "Now choose What will Anu eat.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "Read first." --write-media "Read first.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "How should Anu ask?" --write-media "How should Anu ask.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "This is Ravi. He is looking after their table." --write-media "This is Ravi He is looking after their.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "Oh! That is the wrong dish." --write-media "Oh That is the wrong dish.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "How loud should Anu talk here?" --write-media "How loud should Anu talk here.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "The restaurant is full now." --write-media "The restaurant is full now.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "Three stars! Thank you, come again." --write-media "Three stars Thank you come again.mp3"
edge-tts --voice en-IN-NeerjaNeural --text "What will you say to the waiter next time?" --write-media "What will you say to the waiter next.mp3"

Write-Host "Generating Anu (Child) audios..." -ForegroundColor Green
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "Uhh... uhh..." --write-media "Ummm ummm.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "Could I have the dosa, please?" --write-media "Could I have the dosa please.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "I want dosa!" --write-media "I want dosa.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "Thank you!" --write-media "Thank you.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "Sorry... I think I ordered dosa." --write-media "Sorry I think I ordered dosa.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+20Hz --rate=+10% --text "This is wrong!" --write-media "This is WRONG.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+20Hz --rate=+10% --text "I am so hungry! Where is my food?!" --write-media "I am SO hungry Where is my food.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "Excuse me, could I have another fork, please?" --write-media "Excuse me could I have another fork please.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+20Hz --rate=+15% --text "I dropped my fork!!" --write-media "I dropped my fork shouted.mp3"
edge-tts --voice en-IN-KavyaNeural --pitch=+15Hz --text "Daddy, guess what happened at school!" --write-media "Daddy guess what happened at school.mp3"

Write-Host "Generating Waiter Ravi audios..." -ForegroundColor Green
edge-tts --voice en-IN-PrabhatNeural --rate=-5% --text "Certainly!" --write-media "Certainly.mp3"
edge-tts --voice en-IN-PrabhatNeural --rate=-5% --text "Of course, one moment." --write-media "Of course one moment.mp3"
edge-tts --voice en-IN-PrabhatNeural --rate=-5% --text "I am so sorry, I will fix that right away." --write-media "I am so sorry I will fix that.mp3"
edge-tts --voice en-IN-PrabhatNeural --text "Thank you, do come again!" --write-media "Thank you do come again.mp3"

Write-Host "Generating Parents audios..." -ForegroundColor Green
edge-tts --voice en-IN-PrabhatNeural --pitch=-5Hz --rate=-10% --text "Sorry? I cannot hear you at all." --write-media "Sorry I cannot hear you at all.mp3"
edge-tts --voice en-IN-NeerjaNeural --pitch=-5Hz --rate=-15% --text "Anu... shh, quiet." --write-media "Anu quiet just a name gently warning.mp3"

Write-Host "All Voice-Over audios generated successfully!" -ForegroundColor Cyan
