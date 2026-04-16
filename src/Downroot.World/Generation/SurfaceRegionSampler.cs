using Downroot.Core.World;
using Downroot.World.Generation.Passes;

namespace Downroot.World.Generation;

public static class SurfaceRegionSampler
{
    public static string SampleSurfaceRegion(IWorldGenContext context, WorldTileCoord worldTile)
    {
        return context.WorldSpaceKind switch
        {
            WorldSpaceKind.Overworld => SampleOverworldSurfaceRegion(context, worldTile),
            WorldSpaceKind.DimShardPocket => SurfaceRegions.DimShardField,
            _ => SurfaceRegions.DirtField
        };
    }

    public static string SampleOverworldSurfaceRegion(IWorldGenContext context, WorldTileCoord worldTile)
    {
        if (RiverPass.IsRiverTile(context, worldTile))
        {
            return SurfaceRegions.River;
        }

        var grass = GrassRegionPass.SampleLayeredNoise(context, worldTile);
        if (grass >= GrassRegionPass.GrassThreshold)
        {
            return SurfaceRegions.GrassField;
        }

        return SurfaceRegions.DirtField;
    }
}
