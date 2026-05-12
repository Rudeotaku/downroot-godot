# PROJECT KNOWLEDGE BASE

**Generated:** 2025-05-09
**Commit:** cc52f88
**Branch:** main

## OVERVIEW
Downroot — Godot 4.6 + C# (.NET 8) 2D farming/survival game with dimensional shard mechanics. Content-pack driven architecture with procedural world generation.

## STRUCTURE
```
.
├── src/                          # Pure C# game libraries
│   ├── Downroot.Core/            # Save system, definitions, IDs, input, registries
│   ├── Downroot.Content/         # Content pack loading, registries
│   ├── Downroot.World/           # World generation passes & models
│   ├── Downroot.Gameplay/        # Game runtime, systems, persistence, bootstrap
│   └── Downroot.UI/              # ViewData DTOs, presentation builders
├── game/Downroot.Game/           # Godot project (scenes, runtime controllers/views)
├── packs/basegame/               # Default content pack (assets, defs, scenes)
└── docs/                         # Architecture docs, pack conventions
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Entry point | `game/Downroot.Game/Runtime/AppRoot.cs` | Main scene controller, page navigation |
| Game bootstrap | `src/Downroot.Gameplay/Bootstrap/GameBootstrapper.cs` | DI setup, world creation, save loading |
| World generation | `src/Downroot.World/Generation/Passes/` | Per-pass terrain/feature generation |
| Game simulation | `src/Downroot.Gameplay/Runtime/GameSimulation.cs` | Main tick loop |
| Day-night cycle | `src/Downroot.Gameplay/Runtime/GameSimulation.cs` → `UpdateWorldTime()` | `DayLengthSeconds` controls cycle duration |
| Lighting | `src/Downroot.Gameplay/Runtime/Systems/LightingFieldSystem.cs` | Skylight + emitter field |
| Content definitions | `src/Downroot.Core/Definitions/` | Abstract `ContentDef` record hierarchy |
| Save/load | `src/Downroot.Core/Save/` + `game/.../Infrastructure/` | JSON file store, repositories |
| UI presentation | `src/Downroot.UI/Presentation/` | ViewData records, `GamePresentationBuilder` |
| Godot rendering | `game/Downroot.Game/Runtime/WorldRenderer.cs` | Largest file (706 lines) |
| Content packs | `packs/basegame/` + `docs/PACKS.md` | Asset/ddef/scene conventions |
| Basegame assets | `packs/basegame/assets/README.md` | Asset layout, naming rules, atlas notes |

## CODE MAP

| Symbol | Type | Location | Role |
|--------|------|----------|------|
| AppRoot | class | game/Runtime/AppRoot.cs | Main scene, page host, session lifecycle |
| GameBootstrapper | class | src/Gameplay/Bootstrap/ | DI composition, world init, save restore |
| GameRuntime | class | src/Gameplay/Runtime/ | Central runtime state holder |
| GameSimulation | class | src/Gameplay/Runtime/ | Tick loop, system orchestration |
| WorldGenerator | class | src/World/Generation/ | Chunk generation coordinator |
| ContentRegistrySet | class | src/Content/Registries/ | Aggregated content registries |
| ContentDef | record | src/Core/Definitions/ | Base definition type |
| SaveGameData | class | src/Core/Save/ | Root save DTO |
| WorldStreamingSystem | class | src/Gameplay/Runtime/Systems/ | Chunk load/unload around player |
| GamePresentationBuilder | class | src/UI/Presentation/ | Binds runtime → ViewData |

## CONVENTIONS
- **C# 12 primary constructors** used extensively (parameters auto-become fields)
- **`sealed` by default** — classes are sealed unless inheritance is required
- **Records for defs/DTOs** — `ContentDef` and derivatives are `abstract record`
- **Nullable reference types enabled** — `?` annotations required
- **Manual DI** — no container; `AppRoot` and `GameBootstrapper` wire dependencies by hand
- **Godot/C# split** — pure logic in `src/`, Godot nodes and views in `game/`
- **Namespaces match folders** — `Downroot.Gameplay.Runtime.Systems` etc.

## ANTI-PATTERNS (THIS PROJECT)
- No test project or CI pipeline
- No generic `as any` / `@ts-ignore` equivalents (C# nullable strict)
- `RuntimeProfiler.Measure` calls should stay lightweight; avoid nesting deeply

## UNIQUE STYLES
- **Content pack system** — runtime scans `packs/*`, each pack registers via `IContentPack`
- **World space kinds** — Overworld + DimShardPocket with portal links
- **Entity projection** — `GameRuntime` maintains a flattened entity view for rendering
- **Chunk-based streaming** — radius-based load/unload with `WorldStreamingSystem`

## COMMANDS
```bash
# Build solution
dotnet build Downroot.sln

# Run Godot project
godot --path game/Downroot.Game
```

## TIME SYSTEM

The game uses **dual time tracking** with no time-scale multiplier:

| Counter | Purpose | Unit |
|---------|---------|------|
| `WorldState.TotalElapsedSeconds` | Survival mechanics (hunger drain, poison, hit flash, fuel) | Real seconds |
| `WorldState.TimeOfDaySeconds` | Day-night cycle, lighting, skylight | Real seconds (displayed as "game minutes") |

- **Day length** is configured per-content-pack via `BootstrapConfig.DayLengthSeconds`
- Base game sets this to `1440` → one full day-night cycle = **24 real minutes**
- Both counters increment by raw `deltaSeconds`; pause is handled via Godot's `ProcessMode.Pausable`
- If you need faster/slower days, change `DayLengthSeconds` and re-tune survival intervals (hunger drain uses `TotalElapsedSeconds % 3f`)

## NOTES
- Uses **Jolt Physics** for 3D (configured in `project.godot`)
- Rendering: D3D12 on Windows, pixel-snap enabled for 2D
- Window size: 1600×900
- Content root relative path: `../../` (project looks up two dirs from `game/Downroot.Game/`)
- `packs/basegame/assets/_inbox/` is temporary — confirmed assets should migrate to permanent dirs

## ASSET CHANGES
- **`packs/basegame/assets/world/nature/rocks/rock_outcrop.png`** — Extracted from `stone.png` (variant 2, medium-sized rock). See `packs/basegame/assets/README.md` for atlas layout details.

## COLLISION SYSTEM

### Collision Center Alignment (2025-05-12)
**Problem**: Resource nodes and placeables with large sprites (e.g., 32×32 `rock_outcrop`) had their collision centered on the sprite's top-left corner (`entity.Position`), causing a perceptible offset between the visual sprite and the collision area.

**Solution**: `LoadedWorldState` now computes a `GetCollisionCenter()` based on the entity's sprite dimensions:
- `ResourceNode`: `Position + (SpriteWidth/2, SpriteHeight/2)`
- `Placeable`: `Position + (SpriteWidth/2, SpriteHeight/2)`

**Files changed**:
- `src/Downroot.Gameplay/Runtime/LoadedWorldState.cs`
  - `GetCollisionCenter()` — new method
  - `GetResourceCollisionCenter()` — new method
  - `GetPlaceableCollisionCenter()` — new method
  - `UpdateBlockerIndex()` — uses collision center for tile indexing
  - `IsBlocked()` — uses collision center for distance checks
