# Downroot.Gameplay

Game runtime: state machines, systems, persistence, bootstrap.

## STRUCTURE
```
Downroot.Gameplay/
├── Bootstrap/            # GameBootstrapper, GameStartOptions
├── Runtime/              # GameRuntime, GameSimulation, player/world state
│   └── Systems/          # Movement, Crafting, Placement, WorldStreaming, etc.
└── Persistence/          # Save/load adapters
```

## WHERE TO LOOK
| Task | Location |
|------|----------|
| Main game loop | `Runtime/GameSimulation.cs` |
| Runtime state holder | `Runtime/GameRuntime.cs` |
| World streaming | `Runtime/Systems/WorldStreamingSystem.cs` |
| Bootstrap / DI | `Bootstrap/GameBootstrapper.cs` |
| Save loading | `Persistence/GameSaveLoader.cs` |
| Snapshot builder | `Persistence/GameSaveSnapshotBuilder.cs` |

## CONVENTIONS
- **System pattern** — each system is a `sealed class` with primary constructor taking `GameRuntime`
- **Tick methods** — systems expose `Update()` or `Update(delta)` called from `GameSimulation`
- **Manual DI** — `GameBootstrapper` wires all systems by hand; no container
- **State mutation** — `GameRuntime` holds mutable state; systems mutate it

## ANTI-PATTERNS
- Do not let systems hold references to Godot nodes (use `WorldRuntimeFacade` abstractions)
- Keep `RuntimeProfiler.Measure` calls shallow
