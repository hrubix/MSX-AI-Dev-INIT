/*
 * helloworld — MSX2 (PAL) demo for the AI development stack
 */

#include "msxgl.h"

#include "font/font_mgl_sample6.h"

static void WaitFrames(u8 frames)
{
	u8 i;
	for (i = 0; i < frames; ++i)
		Halt();
}

/* Require `frames` consecutive samples of pressed or released. */
static void WaitKeyState(u8 key, bool wantPressed, u8 frames)
{
	u8 stable = 0;
	while (stable < frames)
	{
		Halt();
		if (wantPressed)
		{
			if (Keyboard_IsKeyPressed(key))
				stable++;
			else
				stable = 0;
		}
		else
		{
			if (!Keyboard_IsKeyPressed(key))
				stable++;
			else
				stable = 0;
		}
	}
}

void main(void)
{
	VDP_SetMode(VDP_MODE_SCREEN5);
	VDP_SetColor(COLOR_BLACK);
	VDP_EnableVBlank(TRUE);
	VDP_ClearVRAM();

	Print_SetBitmapFont(g_Font_MGL_Sample6);
	Print_SetColor(COLOR_WHITE, COLOR_BLACK);

	Print_SetPosition(0, 0);
	Print_DrawText("Hi, I'm your MSX AI Development stack");

	Print_SetPosition(0, 16);
	Print_DrawText("Press SPACE to exit");

	/* Ignore keyboard during boot / cart init (≈1.5 s @ 50 Hz). */
	WaitFrames(75);
	WaitKeyState(KEY_SPACE, FALSE, 15);
	WaitKeyState(KEY_SPACE, TRUE, 8);

	Print_SetPosition(0, 32);
	Print_DrawText("Bye!");

	/* Soft-reset would relaunch a ROM cart; halt the machine instead.
	   Closing the openMSX window is done by the host (Alt+F4 / MCP close). */
	DisableInterrupt();
	while (1)
		Halt();
}
