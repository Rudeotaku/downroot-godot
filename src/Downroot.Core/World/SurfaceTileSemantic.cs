namespace Downroot.Core.World;

public readonly record struct SurfaceTileSemantic(
    TerrainVisualKind Visual,
    SurfaceGameplayKind Surface,
    HeightKind Height,
    ShoreProfileKind ShoreProfile,
    bool Buildable,
    bool Diggable,
    bool SupportsTrees);
