# Project requirements (Windows 11)

Overview of software, tools, and dependencies to install on a **Windows 11** development PC for **TestGame**. Aligns with [msx-dev-ai.md](../msx-dev-ai.md).

Product decisions are in §2. ROM mapper and video/audio pipeline are still open.

---

## 1. Goals of this environment

| Goal | How this stack supports it |
| --- | --- |
| Develop **TestGame** (MSX2, PAL) in C (+ selective ASM) | MSXgl + SDCC |
| Build ROMs reproducibly | MSXgl `build.bat` + Make wrapper |
| Edit and automate with AI | Cursor + project rules |
| Emulate, debug, and regression-test | openMSX + mcp-openmsx |
| Version and share | Git |

---

## 2. Product decisions

| Topic | Decision |
| --- | --- |
| Project name | **TestGame** |
| Software type | **Game** |
| Minimum MSX generation | **MSX2** (`Machine = "2"`) |
| Timing | **PAL** (`Emul60Hz = false`) |
| MSXgl location | **Cloned into** `MSXgl/` (alternative layout) |
| Hardware validation | **openMSX only** for now |
| Delivery format / mapper | **TBD** — provisional build target `ROM_32K` |
| Video | **TBD** — starter uses Screen 5 |
| Audio | **TBD** |

Do not change compiler, framework, machine, or provisional `ROM_32K` unless the task explicitly requires it (or after a mapper decision is recorded here).

---

## 3. Required installs

### 3.1 Cursor (editor + agent)

| Item | Detail |
| --- | --- |
| Product | [Cursor](https://cursor.com/) |
| Why | Editor, agent terminal, MCP host, project rules |
| Notes | Open this folder as the workspace. Enable mcp-openmsx from `.cursor/mcp.json`. |

### 3.2 Git

| Item | Detail |
| --- | --- |
| Product | Git for Windows |
| Why | Version control; cloning MSXgl |
| Install | [https://git-scm.com/download/win](https://git-scm.com/download/win) |
| Verify | `git --version` |

### 3.3 Node.js (LTS)

| Item | Detail |
| --- | --- |
| Product | Node.js **18.12.1+** (LTS recommended) |
| Why | Required by **mcp-openmsx** (`npx`) |
| Install | [https://nodejs.org/](https://nodejs.org/) |
| Verify | `node -v` and `npx -v` |

> MSXgl for Windows ships a bundled Node for its build tool; system Node is still needed for the MCP package.

### 3.4 MSXgl (+ bundled SDCC toolchain)

| Item | Detail |
| --- | --- |
| Product | [MSXgl](https://github.com/aoineko-fr/MSXgl) |
| Why | Framework and build tooling; Windows package embeds SDCC |
| Install | Path **without spaces**. This repo uses `D:\Projects\MSX-Dev-Cursor\MSXgl` |
| Steps | Already cloned for this workspace. Update with `git -C MSXgl pull` when needed. Build from repo root via `build.bat`. |
| Docs | [Install](https://aoineko.org/msxgl/index.php?title=Install), [Alternative layout](https://aoineko.org/msxgl/index.php?title=Alternative_file_layout) |
| Note | No separate system-wide SDCC required on Windows with MSXgl’s package. |

> **Windows PATH trap:** Chocolatey (and some GCC installs) provide `C:\ProgramData\chocolatey\bin\cc1.exe`, which can override MSXgl’s SDCC `cc1` and break compiles. This repo’s `build.bat` prepends MSXgl’s SDCC `bin` to `PATH`. After recloning MSXgl, ensure `MSXgl\tools\sdcc\bin\cc1.exe` exists (copy from the extensionless `cc1` file if needed).

### 3.5 openMSX

| Item | Detail |
| --- | --- |
| Product | openMSX (**21** preferred) |
| Why | Primary emulator / debugger |
| Status on this PC | `C:\Program Files\openMSX\openmsx.exe` present (v21.0) |
| C-BIOS | **Installed** into `%USERPROFILE%\Documents\openMSX\share\systemroms\` (C-BIOS 0.29a). Program Files pool lacked current C-BIOS ROMs, which caused immediate exit when launching C-BIOS machines. |
| Share for MCP | `.cursor/mcp.json` → `OPENMSX_SHARE_DIR` = `%USERPROFILE%\Documents\openMSX\share` (junctions to install `machines` / `extensions`) |
| System ROMs | Optional authentic machine dumps; C-BIOS is enough for TestGame smoke tests |
| Verify | Start Menu / Explorer launch should show **openMSX 21.0 - C-BIOS …**. Prefer **C-BIOS_MSX2_EU** (PAL/50 Hz) for TestGame |

### 3.6 mcp-openmsx

| Item | Detail |
| --- | --- |
| Product | [@nataliapc/mcp-openmsx](https://github.com/nataliapc/mcp-openmsx) |
| Why | Cursor control of openMSX (ROM load, keys, memory, screenshots, savestates) |
| Install | **Installed:** `npm install -g @nataliapc/mcp-openmsx` → **v1.2.13**<br>Binary: `%APPDATA%\npm\mcp-openmsx.cmd` |
| Config on this PC | `.cursor/mcp.json` uses `npx @nataliapc/mcp-openmsx`<br>`OPENMSX_EXECUTABLE` → `tools\openmsx-mcp-launcher.exe` (Windows workaround: CreateProcess cannot start openMSX here; launcher uses explorer + socket PID mirror)<br>`OPENMSX_REAL_EXECUTABLE` → `C:\Program Files\openMSX\openmsx.exe`<br>`OPENMSX_SHARE_DIR` → `%USERPROFILE%\Documents\openMSX\share` |
| Verify | Reload MCP in Cursor; panel shows connected; can launch/reset and capture a screenshot |

---

## 4. Optional / situational

| Software | When needed |
| --- | --- |
| **SjASMPlus** | Assembly-heavy work beyond MSXgl’s default assembler |
| **Make for Windows** | Optional; `build.bat` is enough. Make wraps the same entry |
| **openMSX Debugger UI** | Desktop debugger alongside MCP |
| **Image / map editors** | Authoring `assets/` |
| **Music trackers** | After audio pipeline is chosen |
| **Python** | Only if `tools/` needs it |
| **FPGA / real MSX** | After emulator phase |

---

## 5. Suggested remaining steps (this PC)

1. Confirm Node.js LTS (`node -v`).
2. Enable mcp-openmsx in Cursor (reload MCP if needed).
3. Run `build.bat` → get `emul\rom\TestGame.rom`.
4. Smoke test: [tests/smoke-test.md](../tests/smoke-test.md).
5. Decide ROM mapper/format and screen/audio when design is ready; update this file.

---

## 6. Disk / path conventions

| Path | Purpose |
| --- | --- |
| `D:\Projects\MSX-Dev-Cursor\` | This repository |
| `...\MSXgl\` | Framework clone (gitignored in parent; update locally) |
| `emul\rom\TestGame.rom` | Build output ROM |
| `C:\Program Files\openMSX\` | Emulator install + share |

---

## 7. Verify checklist

- [x] MSXgl cloned into `MSXgl/`
- [x] `project_config.js` / `build.bat` / `src/main.c` for TestGame
- [x] `build.bat` produces `emul\rom\TestGame.rom`
- [x] `node -v` (≥ 18.12.1) for mcp-openmsx
- [x] `@nataliapc/mcp-openmsx` installed globally (v1.2.13)
- [x] openMSX launches MSX2 / PAL (`C-BIOS_MSX2_EU` via mcp-openmsx)
- [x] Cursor lists mcp-openmsx as connected
- [x] Agent can insert ROM and capture a screenshot (smoke test PASS)
- [ ] Record final mapper + video/audio decisions in §2

---

## 8. References

- Stack overview: [msx-dev-ai.md](../msx-dev-ai.md)
- MSXgl: [https://aoineko.org/msxgl/](https://aoineko.org/msxgl/)
- openMSX setup: [https://openmsx.org/manual/setup.html](https://openmsx.org/manual/setup.html)
- mcp-openmsx: [https://github.com/nataliapc/mcp-openmsx](https://github.com/nataliapc/mcp-openmsx)
- MSX wiki: [Programming](https://www.msx.org/wiki/Category:Programming), [Graphics](https://www.msx.org/wiki/Category:Graphics), [Music](https://www.msx.org/wiki/Category:Music)
