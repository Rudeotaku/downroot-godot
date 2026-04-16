using Downroot.Core.World;

namespace Downroot.Game.Runtime;

public sealed record DualGridLayerDef(
    TerrainVisualKind VisualKind,
    int RenderOrder,
    string TextureId,
    string TexturePath);
