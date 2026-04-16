namespace Downroot.Core.World;

public static class SurfaceSemanticDefaults
{
    public static SurfaceTileSemantic CreateForRegion(WorldSpaceKind worldSpaceKind, string surfaceRegion)
    {
        if (worldSpaceKind == WorldSpaceKind.DimShardPocket || surfaceRegion == SurfaceRegions.DimShardField)
        {
            return new SurfaceTileSemantic(
                TerrainVisualKind.Dirt,
                SurfaceGameplayKind.Ground,
                HeightKind.Low,
                ShoreProfileKind.None,
                true,
                true,
                false);
        }

        return surfaceRegion switch
        {
            SurfaceRegions.GrassField => new SurfaceTileSemantic(
                TerrainVisualKind.Grass,
                SurfaceGameplayKind.Ground,
                HeightKind.Low,
                ShoreProfileKind.None,
                true,
                true,
                true),
            SurfaceRegions.River => new SurfaceTileSemantic(
                TerrainVisualKind.ShallowWater,
                SurfaceGameplayKind.Wadeable,
                HeightKind.Low,
                ShoreProfileKind.Gentle,
                false,
                false,
                false),
            SurfaceRegions.RockyOutcrop => new SurfaceTileSemantic(
                TerrainVisualKind.Mountain,
                SurfaceGameplayKind.SolidRock,
                HeightKind.Raised,
                ShoreProfileKind.None,
                false,
                true,
                false),
            _ => new SurfaceTileSemantic(
                TerrainVisualKind.Dirt,
                SurfaceGameplayKind.Ground,
                HeightKind.Low,
                ShoreProfileKind.None,
                true,
                true,
                false)
        };
    }
}
