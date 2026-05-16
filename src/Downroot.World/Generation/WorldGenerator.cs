using Downroot.Content.Registries;
using Downroot.Core.Diagnostics;
using Downroot.Core.World;
using Downroot.World.Models;
using System.Text;

namespace Downroot.World.Generation;

public sealed class WorldGenerator(
    ContentRegistrySet registries,
    IReadOnlyList<IWorldGenPass> passes,
    IDiagnosticLogger? logger = null)
{
    private readonly IDiagnosticLogger _logger = logger ?? NullDiagnosticLogger.Instance;

    public GeneratedChunk GenerateChunk(WorldSpaceKind worldSpaceKind, int worldSeed, ChunkCoord chunkCoord, int width, int height)
    {
        _logger.Log($"[WorldGen][ChunkStart] world={worldSpaceKind} seed={worldSeed} chunk={chunkCoord.X},{chunkCoord.Y} size={width}x{height}");

        var spawns = new List<WorldSpawnDef>();
        var surface = new ChunkData(width, height);
        var context = new WorldGenContext(worldSpaceKind, worldSeed, chunkCoord, surface, registries, spawns, _logger);

        foreach (var pass in passes)
        {
            pass.Execute(context);
        }

        if (context.Logger is not NullDiagnosticLogger)
        {
            LogRegionDistribution(context, _logger);
        }

        _logger.Log($"[WorldGen][ChunkDone] world={worldSpaceKind} chunk={chunkCoord.X},{chunkCoord.Y} spawns={spawns.Count}");

        return new GeneratedChunk(worldSpaceKind, chunkCoord, surface, spawns.ToArray());
    }

    private static void LogRegionDistribution(IWorldGenContext context, IDiagnosticLogger logger)
    {
        if (context.WorldSpaceKind != WorldSpaceKind.Overworld)
        {
            return;
        }

        var counts = new Dictionary<TerrainRegionKind, int>();
        for (var y = 0; y < context.Height; y++)
        {
            for (var x = 0; x < context.Width; x++)
            {
                var region = context.SampleTerrainRegion(new LocalTileCoord(x, y));
                counts.TryGetValue(region, out var current);
                counts[region] = current + 1;
            }
        }

        var total = Math.Max(1, context.Width * context.Height);
        var summary = new StringBuilder();
        foreach (var pair in counts.OrderByDescending(pair => pair.Value))
        {
            if (summary.Length > 0)
            {
                summary.Append(", ");
            }

            var percent = (pair.Value * 100f) / total;
            summary.Append($"{pair.Key}:{pair.Value} ({percent:0.#}%)");
        }

        logger.Log($"[WorldGen][Regions] chunk {context.ChunkCoord.X},{context.ChunkCoord.Y} => {summary}");
    }
}
