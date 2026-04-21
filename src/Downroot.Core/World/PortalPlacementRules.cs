namespace Downroot.Core.World;

public static class PortalPlacementRules
{
    public const int AveragePortalSpacingTiles = 1000;
    private const int PocketWorldSeedMask = unchecked((int)0x05A17D13);
    private const int SectorOffsetSaltX = 81021;
    private const int SectorOffsetSaltY = 81037;

    public static bool SupportsGeneratedPortals(WorldSpaceKind worldSpaceKind)
    {
        return worldSpaceKind is WorldSpaceKind.Overworld or WorldSpaceKind.DimShardPocket;
    }

    public static int CreatePocketWorldSeed(int overworldSeed) => overworldSeed ^ PocketWorldSeedMask;

    public static string CreatePocketWorldId(int overworldSeed) => $"dimshard:{overworldSeed}";

    public static int ResolvePortalBaseSeed(WorldSpaceKind worldSpaceKind, int worldSeed)
    {
        return worldSpaceKind == WorldSpaceKind.DimShardPocket
            ? worldSeed ^ PocketWorldSeedMask
            : worldSeed;
    }

    public static bool IsGeneratedPortalChunk(WorldSpaceKind worldSpaceKind, int worldSeed, int chunkWidth, int chunkHeight, ChunkCoord chunkCoord)
    {
        if (!SupportsGeneratedPortals(worldSpaceKind))
        {
            return false;
        }

        var (sectorWidthChunks, sectorHeightChunks) = GetPortalSectorSizeChunks(chunkWidth, chunkHeight);
        var sectorX = FloorDiv(chunkCoord.X, sectorWidthChunks);
        var sectorY = FloorDiv(chunkCoord.Y, sectorHeightChunks);
        return ResolvePortalChunkForSector(worldSpaceKind, worldSeed, chunkWidth, chunkHeight, sectorX, sectorY) == chunkCoord;
    }

    public static ChunkCoord ResolveNearestPortalChunk(WorldSpaceKind worldSpaceKind, int worldSeed, int chunkWidth, int chunkHeight, ChunkCoord referenceChunk)
    {
        var (sectorWidthChunks, sectorHeightChunks) = GetPortalSectorSizeChunks(chunkWidth, chunkHeight);
        var sectorX = FloorDiv(referenceChunk.X, sectorWidthChunks);
        var sectorY = FloorDiv(referenceChunk.Y, sectorHeightChunks);

        var best = ResolvePortalChunkForSector(worldSpaceKind, worldSeed, chunkWidth, chunkHeight, sectorX, sectorY);
        var bestDistance = DistanceSquared(best, referenceChunk);
        for (var dy = -1; dy <= 1; dy++)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                var candidate = ResolvePortalChunkForSector(worldSpaceKind, worldSeed, chunkWidth, chunkHeight, sectorX + dx, sectorY + dy);
                var distance = DistanceSquared(candidate, referenceChunk);
                if (distance >= bestDistance)
                {
                    continue;
                }

                best = candidate;
                bestDistance = distance;
            }
        }

        return best;
    }

    public static PortalWorldLinkDef CreateGeneratedLink(WorldSpaceKind worldSpaceKind, int worldSeed, int chunkWidth, int chunkHeight, ChunkCoord portalChunk)
    {
        if (!IsGeneratedPortalChunk(worldSpaceKind, worldSeed, chunkWidth, chunkHeight, portalChunk))
        {
            throw new InvalidOperationException($"Chunk {portalChunk.X},{portalChunk.Y} is not a generated portal chunk for {worldSpaceKind}.");
        }

        var baseSeed = ResolvePortalBaseSeed(worldSpaceKind, worldSeed);
        return new PortalWorldLinkDef(
            WorldSpaceKind.Overworld,
            WorldSpaceKind.DimShardPocket,
            portalChunk,
            portalChunk,
            $"generated-dimshard:{baseSeed}:{portalChunk.X},{portalChunk.Y}");
    }

    private static ChunkCoord ResolvePortalChunkForSector(WorldSpaceKind worldSpaceKind, int worldSeed, int chunkWidth, int chunkHeight, int sectorX, int sectorY)
    {
        var (sectorWidthChunks, sectorHeightChunks) = GetPortalSectorSizeChunks(chunkWidth, chunkHeight);
        var baseSeed = ResolvePortalBaseSeed(worldSpaceKind, worldSeed);
        var chunkOffsetX = StableHash(baseSeed, sectorX, sectorY, SectorOffsetSaltX) % sectorWidthChunks;
        var chunkOffsetY = StableHash(baseSeed, sectorX, sectorY, SectorOffsetSaltY) % sectorHeightChunks;
        return new ChunkCoord(
            sectorX * sectorWidthChunks + chunkOffsetX,
            sectorY * sectorHeightChunks + chunkOffsetY);
    }

    private static (int WidthChunks, int HeightChunks) GetPortalSectorSizeChunks(int chunkWidth, int chunkHeight)
    {
        return (
            Math.Max(1, (int)MathF.Round(AveragePortalSpacingTiles / (float)Math.Max(1, chunkWidth))),
            Math.Max(1, (int)MathF.Round(AveragePortalSpacingTiles / (float)Math.Max(1, chunkHeight))));
    }

    private static int StableHash(int baseSeed, int x, int y, int salt)
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 31) + baseSeed;
            hash = (hash * 31) + x;
            hash = (hash * 31) + y;
            hash = (hash * 31) + salt;
            hash ^= hash >> 16;
            hash *= unchecked((int)0x7feb352d);
            hash ^= hash >> 15;
            hash *= unchecked((int)0x846ca68b);
            hash ^= hash >> 16;
            return hash & int.MaxValue;
        }
    }

    private static int FloorDiv(int value, int divisor)
    {
        var quotient = value / divisor;
        var remainder = value % divisor;
        return remainder != 0 && value < 0 ? quotient - 1 : quotient;
    }

    private static int DistanceSquared(ChunkCoord a, ChunkCoord b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return (dx * dx) + (dy * dy);
    }
}
