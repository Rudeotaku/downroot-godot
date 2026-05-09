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
| Content definitions | `src/Downroot.Core/Definitions/` | Abstract `ContentDef` record hierarchy |
| Save/load | `src/Downroot.Core/Save/` + `game/.../Infrastructure/` | JSON file store, repositories |
| UI presentation | `src/Downroot.UI/Presentation/` | ViewData records, `GamePresentationBuilder` |
| Godot rendering | `game/Downroot.Game/Runtime/WorldRenderer.cs` | Largest file (706 lines) |
| Content packs | `packs/basegame/` + `docs/PACKS.md` | Asset/ddef/scene conventions |

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

## NOTES
- Uses **Jolt Physics** for 3D (configured in `project.godot`)
- Rendering: D3D12 on Windows, pixel-snap enabled for 2D
- Window size: 1600×900
- Content root relative path: `../../` (project looks up two dirs from `game/Downroot.Game/`)
- `packs/basegame/assets/_inbox/` is temporary — confirmed assets should migrate to permanent dirs
