# PowerShell script to run edge-tts audio generator for Unit 8
Write-Host "Installing edge-tts if missing..." -ForegroundColor Cyan
pip install edge-tts

Write-Host "Generating Unit 8 voiceover audio files..." -ForegroundColor Green
python generate_audios.py

Write-Host "Done generating Unit 8 audio files!" -ForegroundColor Yellow
