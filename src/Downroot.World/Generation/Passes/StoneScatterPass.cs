using Downroot.Core.Ids;
using Downroot.Core.World;

namespace Downroot.World.Generation.Passes;

public sealed class StoneScatterPass(
    ContentId targetId,
    int count,
    int startColumn,
    int startRow,
    int width,
    int height,
    int minSpacing,
    float candidateDensity,
    int? maxCountOverride) : IWorldGenPass
{
    private const int FineNoiseSalt = 8101;
    private const int ClusterNoiseSalt = 8117;

    public string Name => WorldGenPassTypes.StoneScatter;

    public void Execute(IWorldGenContext context)
    {
        var usableWidth = width > 0 ? Math.Min(width, context.Width) : context.Width;
        var usableHeight = height > 0 ? Math.Min(height, context.Height) : context.Height;
        var originX = Math.Clamp(startColumn, 0, Math.Max(0, context.Width - 1));
        var originY = Math.Clamp(startRow, 0, Math.Max(0, context.Height - 1));

        var candidates = new List<ScoredCandidate>();
        for (var y = originY; y < originY + usableHeight; y++)
        {
            for (var x = originX; x < originX + usableWidth; x++)
            {
                var coord = new LocalTileCoord(x, y);
                if (!IsEligible(context, coord))
                {
                    continue;
                }

                var score = ScoreCandidate(context, coord);
                if (score < 0.68f)
                {
                    continue;
                }

                candidates.Add(new ScoredCandidate(coord, score));
            }
        }

        if (candidates.Count == 0)
        {
            return;
        }

        var desiredCount = ComputeDesiredCount(candidates.Count);
        if (desiredCount <= 0)
        {
            return;
        }

        var chosen = new List<LocalTileCoord>(desiredCount);
        foreach (var candidate in candidates.OrderByDescending(static candidate => candidate.Score))
        {
            if (chosen.Count >= desiredCount)
            {
                break;
            }

            if (context.IsSpawnOccupied(candidate.Coord))
            {
                continue;
            }

            if (minSpacing > 0 && chosen.Any(existing => DistanceSquared(existing, candidate.Coord) < minSpacing * minSpacing))
            {
                continue;
            }

            context.AddSpawn(candidate.Coord, targetId);
            chosen.Add(candidate.Coord);
        }
    }

    private static bool IsEligible(IWorldGenContext context, LocalTileCoord coord)
    {
        if (context.HasRaisedFeature(coord) || context.IsSpawnOccupied(coord))
        {
            return false;
        }

        var semantic = context.GetSurfaceSemantic(coord);
        if (!semantic.Buildable || semantic.Surface != SurfaceGameplayKind.Ground)
        {
            return false;
        }

        return semantic.Visual is TerrainVisualKind.Dirt or TerrainVisualKind.Grass;
    }

    private static float ScoreCandidate(IWorldGenContext context, LocalTileCoord coord)
    {
        var world = context.GetWorldTileCoord(coord);
        var fineNoise = context.GetStableUnitValue(world, FineNoiseSalt);
        var clusterCoord = new WorldTileCoord(world.X / 5, world.Y / 5);
        var clusterNoise = context.GetStableUnitValue(clusterCoord, ClusterNoiseSalt);
        return (clusterNoise * 0.72f) + (fineNoise * 0.28f);
    }

    private int ComputeDesiredCount(int viableCandidateCount)
    {
        if (viableCandidateCount <= 0)
        {
            return 0;
        }

        var desired = count > 0 ? Math.Min(count, viableCandidateCount) : 0;
        if (candidateDensity > 0f)
        {
            desired = Math.Max(desired, (int)MathF.Ceiling(viableCandidateCount * candidateDensity));
        }

        if (maxCountOverride.HasValue)
        {
            desired = Math.Min(desired, maxCountOverride.Value);
        }

        return Math.Clamp(desired, 0, viableCandidateCount);
    }

    private static int DistanceSquared(LocalTileCoord a, LocalTileCoord b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return (dx * dx) + (dy * dy);
    }

    private readonly record struct ScoredCandidate(LocalTileCoord Coord, float Score);
}
