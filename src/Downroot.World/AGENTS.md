# Downroot.World

Procedural world generation: chunk models and generation passes.

## STRUCTURE
```
Downroot.World/
├── Generation/           # WorldGenerator, pass factory, autotile resolvers
│   └── Passes/           # Individual IWorldGenPass implementations
└── Models/               # ChunkData, GeneratedChunk, SurfaceCell, WorldModel
```

## WHERE TO LOOK
| Task | Location |
|------|----------|
| Chunk generator | `Generation/WorldGenerator.cs` |
| Pass implementations | `Generation/Passes/` (Grass, River, Rock, Ore, etc.) |
| Chunk models | `Models/GeneratedChunk.cs`, `Models/ChunkData.cs` |
| World model | `Models/WorldModel.cs` |
| Pass factory | `Generation/WorldGenPassFactory.cs` |

## CONVENTIONS
- **Pass-based generation** — each terrain/feature is a separate `IWorldGenPass`
- **Immutable models** — `GeneratedChunk`, `WorldModel` use primary constructors (C# 12)
- **ContentId injection** — passes receive `ContentId` for the terrain/feature they place
- **Autotile resolution** — `RaisedFeatureAutotileResolver` computes tile variants post-generation

## ANTI-PATTERNS
- Passes should not depend on Godot types (use pure math/struct models)
