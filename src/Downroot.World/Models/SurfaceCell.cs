using Downroot.Core.Ids;
using Downroot.Core.World;

namespace Downroot.World.Models;

public sealed class SurfaceCell
{
    public ContentId? BaseTerrainId { get; set; }
    public ContentId? CoverTerrainId { get; set; }
    public ContentId? RaisedFeatureId { get; set; }
    public byte RaisedFeatureVariantIndex { get; set; }
    public string SurfaceRegion { get; set; } = string.Empty;
    public TerrainVisualKind VisualKind { get; set; } = TerrainVisualKind.Dirt;
    public SurfaceGameplayKind SurfaceKind { get; set; } = SurfaceGameplayKind.Ground;
    public HeightKind HeightKind { get; set; } = HeightKind.Low;
    public ShoreProfileKind ShoreProfileKind { get; set; } = ShoreProfileKind.None;
    public bool Buildable { get; set; } = true;
    public bool Diggable { get; set; } = true;
    public bool SupportsTrees { get; set; }
}
