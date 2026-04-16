using Downroot.Core.Ids;
using Downroot.Core.World;

namespace Downroot.World.Models;

public sealed class ChunkData
{
    private readonly SurfaceCell[,] _cells;

    public ChunkData(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new SurfaceCell[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                _cells[x, y] = new SurfaceCell();
            }
        }
    }

    public int Width { get; }
    public int Height { get; }

    public ContentId? GetBaseTerrainId(int x, int y) => _cells[x, y].BaseTerrainId;

    public ContentId? GetCoverTerrainId(int x, int y) => _cells[x, y].CoverTerrainId;

    public ContentId? GetRaisedFeatureId(int x, int y) => _cells[x, y].RaisedFeatureId;

    public byte GetRaisedFeatureVariantIndex(int x, int y) => _cells[x, y].RaisedFeatureVariantIndex;

    public string GetSurfaceRegion(int x, int y) => _cells[x, y].SurfaceRegion;

    public TerrainVisualKind GetVisualKind(int x, int y) => _cells[x, y].VisualKind;

    public SurfaceGameplayKind GetSurfaceGameplayKind(int x, int y) => _cells[x, y].SurfaceKind;

    public HeightKind GetHeightKind(int x, int y) => _cells[x, y].HeightKind;

    public ShoreProfileKind GetShoreProfileKind(int x, int y) => _cells[x, y].ShoreProfileKind;

    public bool IsBuildable(int x, int y) => _cells[x, y].Buildable;

    public bool IsDiggable(int x, int y) => _cells[x, y].Diggable;

    public bool SupportsTrees(int x, int y) => _cells[x, y].SupportsTrees;

    public SurfaceTileSemantic GetSurfaceSemantic(int x, int y)
    {
        var cell = _cells[x, y];
        return new SurfaceTileSemantic(
            cell.VisualKind,
            cell.SurfaceKind,
            cell.HeightKind,
            cell.ShoreProfileKind,
            cell.Buildable,
            cell.Diggable,
            cell.SupportsTrees);
    }

    public bool HasRaisedFeature(int x, int y) => _cells[x, y].RaisedFeatureId is not null;

    public bool HasSurfaceRegion(int x, int y, string regionKey) => _cells[x, y].SurfaceRegion == regionKey;

    public void SetBaseTerrain(int x, int y, ContentId terrainId) => _cells[x, y].BaseTerrainId = terrainId;

    public void SetCoverTerrain(int x, int y, ContentId? terrainId) => _cells[x, y].CoverTerrainId = terrainId;

    public void SetRaisedFeature(int x, int y, ContentId featureId)
    {
        _cells[x, y].RaisedFeatureId = featureId;
    }

    public void ClearRaisedFeature(int x, int y)
    {
        _cells[x, y].RaisedFeatureId = null;
        _cells[x, y].RaisedFeatureVariantIndex = 0;
    }

    public void SetRaisedFeatureVariantIndex(int x, int y, byte index) => _cells[x, y].RaisedFeatureVariantIndex = index;

    public void SetSurfaceRegion(int x, int y, string regionKey) => _cells[x, y].SurfaceRegion = regionKey;

    public void SetSurfaceSemantic(int x, int y, SurfaceTileSemantic semantic)
    {
        var cell = _cells[x, y];
        cell.VisualKind = semantic.Visual;
        cell.SurfaceKind = semantic.Surface;
        cell.HeightKind = semantic.Height;
        cell.ShoreProfileKind = semantic.ShoreProfile;
        cell.Buildable = semantic.Buildable;
        cell.Diggable = semantic.Diggable;
        cell.SupportsTrees = semantic.SupportsTrees;
    }

    public IDictionary<string, int> CountSurfaceRegions()
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var region = _cells[x, y].SurfaceRegion;
                if (!counts.TryAdd(region, 1))
                {
                    counts[region]++;
                }
            }
        }

        return counts;
    }
}
