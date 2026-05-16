using Downroot.Core.World;

namespace Downroot.Game.Runtime;

public enum DualGridRenderRole : byte
{
    // Base visuals replace the old flat base-tile draw for that terrain family.
    // Dirt is the current example: it is not an extra overlay on top of another dirt tile.
    BaseVisual = 0,
    // Overlay visuals sit on top of the resolved base visual and are used for
    // materials like deep water, beach, and grass.
    OverlayVisual = 1
}

public sealed record DualGridLayerDef(
    TerrainVisualKind VisualKind,
    DualGridRenderRole RenderRole,
    int RenderOrder,
    string TextureId,
    string TexturePath);
