using Downroot.Content.Packs;
using Downroot.Core.Ids;
using Downroot.Core.World;
using Xunit;

namespace Downroot.World.Tests;

public sealed class WorldGenerationIntegrationTests
{
    private static readonly ContentId PortalId = new("portalmod:portal");
    private static readonly ContentId StoneNodeId = new("basegame:stone_node");

    [Fact]
    public void OverworldGeneration_ProducesOnePortalPerSampledSector()
    {
        var registries = WorldGenTestHarness.BuildRegistries(new BaseGameContentPack(), new PortalModContentPack());
        var sectorWidthChunks = (int)MathF.Round(PortalPlacementRules.AveragePortalSpacingTiles / (float)WorldGenTestHarness.ChunkWidth);
        var sectorHeightChunks = (int)MathF.Round(PortalPlacementRules.AveragePortalSpacingTiles / (float)WorldGenTestHarness.ChunkHeight);
        var sampledSectors = new[]
        {
            new ChunkCoord(0, 0),
            new ChunkCoord(sectorWidthChunks, 0),
            new ChunkCoord(0, sectorHeightChunks),
            new ChunkCoord(-sectorWidthChunks, -sectorHeightChunks)
        };

        foreach (var sectorAnchor in sampledSectors)
        {
            var portalChunk = PortalPlacementRules.ResolveNearestPortalChunk(
                WorldSpaceKind.Overworld,
                WorldGenTestHarness.DefaultWorldSeed,
                WorldGenTestHarness.ChunkWidth,
                WorldGenTestHarness.ChunkHeight,
                sectorAnchor);
            var nonPortalChunk = new ChunkCoord(portalChunk.X + 1, portalChunk.Y);
            if (PortalPlacementRules.IsGeneratedPortalChunk(
                WorldSpaceKind.Overworld,
                WorldGenTestHarness.DefaultWorldSeed,
                WorldGenTestHarness.ChunkWidth,
                WorldGenTestHarness.ChunkHeight,
                nonPortalChunk))
            {
                nonPortalChunk = new ChunkCoord(portalChunk.X, portalChunk.Y + 1);
            }

            var portalGenerated = WorldGenTestHarness.GenerateChunk(registries, WorldSpaceKind.Overworld, portalChunk);
            var nonPortalGenerated = WorldGenTestHarness.GenerateChunk(registries, WorldSpaceKind.Overworld, nonPortalChunk);

            Assert.Single(WorldGenTestHarness.FilterSpawns(portalGenerated, PortalId));
            Assert.Empty(WorldGenTestHarness.FilterSpawns(nonPortalGenerated, PortalId));
        }
    }

    [Fact]
    public void OverworldGeneration_ProducesStoneNodesAcrossSampleArea()
    {
        var registries = WorldGenTestHarness.BuildRegistries(new BaseGameContentPack());
        var chunksWithStone = 0;
        var totalStoneNodes = 0;

        for (var y = -4; y <= 4; y++)
        {
            for (var x = -4; x <= 4; x++)
            {
                var chunk = WorldGenTestHarness.GenerateChunk(registries, WorldSpaceKind.Overworld, new ChunkCoord(x, y));
                var stones = WorldGenTestHarness.FilterSpawns(chunk, StoneNodeId);
                if (stones.Count > 0)
                {
                    chunksWithStone++;
                    totalStoneNodes += stones.Count;
                }
            }
        }

        Assert.InRange(chunksWithStone, 5, 81);
        Assert.InRange(totalStoneNodes, 8, 400);
    }
}
