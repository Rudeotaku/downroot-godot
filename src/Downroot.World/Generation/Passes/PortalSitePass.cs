using Downroot.Core.Ids;
using Downroot.Core.World;

namespace Downroot.World.Generation.Passes;

public sealed class PortalSitePass(ContentId portalId, IReadOnlyList<PortalWorldLinkDef> links) : IWorldGenPass
{
    public string Name => WorldGenPassTypes.PortalSite;

    public void Execute(IWorldGenContext context)
    {
        if (!HasPortalInChunk(context))
        {
            return;
        }

        var center = new LocalTileCoord(context.Width / 2, context.Height / 2);
        var best = FindNearestUsableTile(context, center);
        if (best is null)
        {
            return;
        }

        context.AddSpawn(best.Value, portalId);
    }

    private bool HasPortalInChunk(IWorldGenContext context)
    {
        if (links.Any(link =>
            (link.SourceWorldSpaceKind == context.WorldSpaceKind && link.SourcePortalChunk == context.ChunkCoord)
            || (link.TargetWorldSpaceKind == context.WorldSpaceKind && link.TargetPortalChunk == context.ChunkCoord)))
        {
            return true;
        }

        return PortalPlacementRules.IsGeneratedPortalChunk(
            context.WorldSpaceKind,
            context.WorldSeed,
            context.Width,
            context.Height,
            context.ChunkCoord);
    }

    private static LocalTileCoord? FindNearestUsableTile(IWorldGenContext context, LocalTileCoord origin)
    {
        LocalTileCoord? best = null;
        var bestDistance = int.MaxValue;
        for (var y = 0; y < context.Height; y++)
        {
            for (var x = 0; x < context.Width; x++)
            {
                var local = new LocalTileCoord(x, y);
                if (context.IsSpawnOccupied(local) || context.HasRaisedFeature(local) || context.HasSurfaceRegion(local, SurfaceRegions.River))
                {
                    continue;
                }

                var semantic = context.GetSurfaceSemantic(local);
                if (!semantic.Buildable || semantic.Surface != SurfaceGameplayKind.Ground || semantic.Visual == TerrainVisualKind.Mountain)
                {
                    continue;
                }

                var distance = DistanceSquared(origin, local);
                if (distance >= bestDistance)
                {
                    continue;
                }

                best = local;
                bestDistance = distance;
            }
        }

        return best;
    }

    private static int DistanceSquared(LocalTileCoord a, LocalTileCoord b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return (dx * dx) + (dy * dy);
    }
}
