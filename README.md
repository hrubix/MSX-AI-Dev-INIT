# MSX-AI-Dev-INIT

Starter project for building **MSX** software with **Cursor** (AI-assisted), **MSXgl**, **SDCC**, **openMSX**, and **mcp-openmsx**.

Repo: [hrubix/MSX-AI-Dev-INIT](https://github.com/hrubix/MSX-AI-Dev-INIT)

## What you get

- MSX2 / PAL project layout (provisional `ROM_32K`)
- Cursor rules + MCP config template for openMSX automation
- `helloworld` demo ROM source (`src/main.c`)
- Windows helper to launch openMSX from mcp-openmsx (`tools/openmsx-mcp-launcher.*`)

## Quick start (Windows 11)

1. **Clone** this repo (path without spaces recommended).
2. **Clone MSXgl** into the project root:
   ```bat
   git clone https://github.com/aoineko-fr/MSXgl.git MSXgl
   ```
3. If SDCC fails with a strange `cc1` error, copy:
   `MSXgl\tools\sdcc\bin\cc1` → `MSXgl\tools\sdcc\bin\cc1.exe`
4. **Install** [openMSX](https://openmsx.org/), [Node.js LTS](https://nodejs.org/), and C-BIOS ROMs into your openMSX `systemroms` pool (see `docs/project-requirements.md`).
5. Copy MCP config and edit paths:
   ```bat
   copy .cursor\mcp.json.example .cursor\mcp.json
   ```
   Set `OPENMSX_SHARE_DIR`, screenshot/replay dirs, and keep `OPENMSX_EXECUTABLE` pointed at `tools\openmsx-mcp-launcher.exe`.
6. Enable **mcp-openmsx** in Cursor: `Ctrl+Shift+J` → **Tools & MCP**.
7. **Build**:
   ```bat
   build.bat
   ```
   Output: `emul\rom\helloworld.rom`
8. Run the smoke flow in Agent chat (or follow `tests/smoke-test.md`).

### helloworld demo

- Prints `Hi, I'm your MSX AI Development stack`
- Then `Press SPACE to exit`
- SPACE → `Bye!` and CPU halt (a cart ROM cannot quit the openMSX process; close the window on the host)

Sample binary (optional): `samples/helloworld.rom`

## Stack

| Layer | Choice |
| --- | --- |
| Editor / AI | Cursor + project rules |
| Framework | MSXgl |
| Compiler | SDCC (bundled with MSXgl on Windows) |
| Emulator | openMSX 21 |
| MCP | [@nataliapc/mcp-openmsx](https://github.com/nataliapc/mcp-openmsx) |

## Docs

- [docs/project-requirements.md](docs/project-requirements.md) — install checklist
- [msx-dev-ai.md](msx-dev-ai.md) — AI/dev workflow
- [tests/smoke-test.md](tests/smoke-test.md) — emulator smoke procedure

## License notes

- This template: use freely; attribute appreciated.
- **MSXgl**, **openMSX**, **C-BIOS**, and **mcp-openmsx** keep their own licenses — clone/install them separately.
