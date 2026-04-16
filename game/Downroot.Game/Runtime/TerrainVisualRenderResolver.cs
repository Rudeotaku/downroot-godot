using Downroot.Core.Ids;
using Downroot.Core.World;
using Godot;

namespace Downroot.Game.Runtime;

public static class TerrainVisualRenderResolver
{
    private static readonly ContentId MountainTerrainId = new("basegame:mountain");
    private static readonly ContentId RiverWaterTerrainId = new("basegame:river_water");
    private static readonly Color DefaultTint = Colors.White;
    private static readonly Color RaisedMountainTint = new(0.78f, 0.80f, 0.84f, 1f);
    private static readonly Color CliffMountainTint = new(0.58f, 0.62f, 0.68f, 1f);

    // Base dual-grid visuals are mutually exclusive with ordinary base terrain sprites.
    // When this returns Dirt, the renderer should not also draw a second flat dirt base tile.
    public static IReadOnlyList<DualGridLayerDef> BaseDualGridLayers => [DualGridLayerCatalog.Dirt];

    public static IReadOnlyList<DualGridLayerDef> OverlayDualGridLayers => [
        DualGridLayerCatalog.DeepWater,
        DualGridLayerCatalog.Beach,
        DualGridLayerCatalog.Grass
    ];

    public static TerrainVisualRenderProfile Resolve(
        WorldSpaceKind worldSpaceKind,
        SurfaceTileSemantic semantic,
        ContentId fallbackBaseTerrainId,
        ContentId? legacyCoverTerrainId)
    {
        if (worldSpaceKind != WorldSpaceKind.Overworld)
        {
            return new TerrainVisualRenderProfile(
                fallbackBaseTerrainId,
                legacyCoverTerrainId is not null,
                DefaultTint,
                null);
        }

        return semantic.Visual switch
        {
            TerrainVisualKind.DeepWater => new TerrainVisualRenderProfile(RiverWaterTerrainId, false, DefaultTint, null),
            TerrainVisualKind.ShallowWater => new TerrainVisualRenderProfile(RiverWaterTerrainId, false, DefaultTint, null),
            // Beach and grass sit on top of dirt conceptually, so they reuse dirt as the
            // base visual family rather than requesting a separate flat dirt sprite.
            TerrainVisualKind.Beach => new TerrainVisualRenderProfile(null, false, DefaultTint, TerrainVisualKind.Dirt),
            TerrainVisualKind.Grass => new TerrainVisualRenderProfile(null, false, DefaultTint, TerrainVisualKind.Dirt),
            TerrainVisualKind.Mountain => new TerrainVisualRenderProfile(MountainTerrainId, false, ResolveMountainTint(semantic.Height), null),
            _ => new TerrainVisualRenderProfile(null, false, DefaultTint, TerrainVisualKind.Dirt)
        };
    }

    private static Color ResolveMountainTint(HeightKind heightKind)
    {
        return heightKind switch
        {
            HeightKind.Cliff => CliffMountainTint,
            HeightKind.Raised => RaisedMountainTint,
            _ => DefaultTint
        };
    }
}

public sealed record TerrainVisualRenderProfile(
    ContentId? BaseTerrainId,
    bool RenderLegacyCover,
    Color BaseTint,
    TerrainVisualKind? BaseDualGridVisualKind);
