# MSX AI Development Stack

Guide for developing, testing, and debugging MSX software with Cursor, MSXgl, and openMSX.

## Recommended greenfield stack

| Layer | Recommended choice |
| --- | --- |
| Language | C, with selective Z80 assembly |
| Framework / library | MSXgl |
| C compiler | SDCC |
| Assembler | MSXgl’s supported assembler workflow; SjASMPlus for assembly-heavy projects |
| Editor | Cursor |
| Build system | MSXgl’s included build tooling; Make as the public build interface |
| Emulator | openMSX 21 |
| Debugger | openMSX’s integrated debugger |
| AI / emulator bridge | [mcp-openmsx](https://github.com/nataliapc/mcp-openmsx#readme) |
| Version control | Git |
| Hardware testing | Real MSX or FPGA implementation after emulator testing |

Use Z80 assembly only where it provides a concrete size, speed, timing, or hardware-access benefit. Prefer openMSX 21 as the main development emulator; never treat emulator success as proof of compatibility with all physical MSX machines.

## Architecture

```
Cursor Agent
    │
    │ Model Context Protocol
    ▼
mcp-openmsx
    │
    │ openMSX control protocol / Tcl commands
    ▼
openMSX
```

### Tool layers

**Cursor**

- MSX-aware project rules
- Agent terminal access
- MCP: mcp-openmsx
- Git

**Build**

- MSXgl
- Pinned SDCC version
- Assembler
- Asset converters
- Make or project build script

**Runtime / debug**

- openMSX
- mcp-openmsx typed tools
- Screenshots, savestates, and replay-based regression tests

## Expected repository structure

```
my-msx-game/
├── .cursor/
│   ├── mcp.json
│   └── rules/
│       └── msx-development.mdc
├── src/                 # C and assembly source
├── include/             # project headers, when present
├── assets/              # original graphics, maps, fonts, music, sound
├── generated/           # generated C, assembly, binary, or include files
├── tools/               # project-specific host-side utilities
├── tests/
│   ├── smoke-test.md
│   └── expected/
├── build/               # generated build output
├── screenshots/         # emulator test evidence
├── replays/             # openMSX replay files
├── docs/                # technical notes and specifications
├── MSXgl/
├── Makefile
└── README.md
```

Keep generated files out of hand-written source directories. Never hand-edit generated assets unless their generator explicitly requires it.

## Development workflow

1. Cursor edits C or assembly source.
2. Cursor runs the project build command.
3. The build produces `game.rom`.
4. Cursor invokes mcp-openmsx to launch openMSX.
5. MCP inserts `game.rom`.
6. MCP resets or powers on the emulated machine.
7. Cursor waits for the title screen.
8. MCP captures the screen and reads machine state.
9. Cursor compares the result with the expected behaviour.
10. Cursor diagnoses and edits the code.

## Automated testing patterns

**Smoke test**

Build ROM → launch emulator → insert ROM → reset → wait 120 frames → capture screenshot → check screen mode → inspect selected memory locations → report pass or failure

**Crash investigation**

Run game → detect unexpected stop or visual failure → pause CPU → retrieve PC, SP and registers → inspect stack → disassemble around PC → inspect RAM → suggest likely source location

**Gameplay test**

Launch game → send SPACE → wait → send RIGHT for several frames → capture screenshot → verify player coordinates in RAM

**Regression test**

Load known savestate → perform exact keyboard sequence → advance a fixed number of frames → compare RAM values and screenshot

## Tool responsibilities

**Use Cursor terminal commands for**

- Compiling, linking, and ROM construction
- Asset conversion
- Host-side unit tests and linting
- File manipulation and Git operations

**Use mcp-openmsx for**

- Starting and resetting the emulated machine
- Loading ROM, disk, and tape images
- Reading registers and memory
- Breakpoints and stepping
- Keyboard automation
- Screenshots and screen text
- Savestates and replays
- Emulator-specific validation

## Cursor agent rules

Use these as core project conventions (for example in `.cursor/rules/msx-development.mdc`).

```yaml
---
description: Core project conventions for this MSX software project
alwaysApply: true
---
```

### Source of truth

Before making changes:

1. Read `README.md`.
2. Inspect `Makefile` and relevant build configuration.
3. Inspect the installed MSXgl version and its local headers.
4. Prefer APIs proven to exist in the installed framework over APIs recalled from memory.
5. Do not change the compiler, framework, ROM mapper, target machine, or output format unless the task explicitly requires it.

### Working principles

- Make the smallest coherent change that solves the task.
- Preserve existing naming, formatting, directory structure, and architectural conventions.
- Do not rewrite unrelated code.
- Do not add host-side dependencies unless they provide a clear benefit.
- Do not claim that a change works until the relevant build or test has run successfully.
- Report build errors and emulator-test failures accurately.

### Documentation

When behavior, memory layout, controls, mapper configuration, build requirements, or test procedures change, update the relevant documentation in the same change.

Document important hardware assumptions, including:

- Target MSX generation
- Required RAM or VRAM
- Required VDP
- ROM size and mapper
- Required extensions
- PAL or NTSC timing assumptions
- BIOS or MSX-DOS dependencies

## MSX development knowledge

- [Programming](https://www.msx.org/wiki/Category:Programming)
- [Graphics](https://www.msx.org/wiki/Category:Graphics)
- [Music](https://www.msx.org/wiki/Category:Music)
- [Game Design](https://www.msx.org/wiki/Game_Design)
- [Hardware Design](https://www.msx.org/wiki/Hardware_Design)
