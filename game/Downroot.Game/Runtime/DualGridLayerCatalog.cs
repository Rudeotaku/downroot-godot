using Downroot.Core.World;

namespace Downroot.Game.Runtime;

public static class DualGridLayerCatalog
{
    public static readonly DualGridLayerDef Dirt = new(
        TerrainVisualKind.Dirt,
        1,
        "basegame:dirt_dualgrid",
        "packs/basegame/assets/world/terrain/ground/dirt_dualgrid.png");

    public static readonly DualGridLayerDef DeepWater = new(
        TerrainVisualKind.DeepWater,
        2,
        "basegame:deepwater_dualgrid",
        "packs/basegame/assets/world/terrain/ground/deepwater_dualgrid.png");

    public static readonly DualGridLayerDef Beach = new(
        TerrainVisualKind.Beach,
        3,
        "basegame:sand_dualgrid",
        "packs/basegame/assets/world/terrain/ground/sand_dualgrid.png");

    public static readonly DualGridLayerDef Grass = new(
        TerrainVisualKind.Grass,
        4,
        "basegame:grass_dualgrid",
        "packs/basegame/assets/world/terrain/ground/grass_dualgrid.png");

    public static readonly IReadOnlyList<DualGridLayerDef> All = [Dirt, DeepWater, Beach, Grass];
}
