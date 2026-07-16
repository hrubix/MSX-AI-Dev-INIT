set renderer SDLGL-PP
carta insert "D:/Projects/MSX-Dev-Cursor/emul/rom/TestGame.rom"
reset
after time 3 {
  screenshot "D:/Projects/MSX-Dev-Cursor/screenshots/smoke-latest.png"
}
after time 4 {
  exit
}
