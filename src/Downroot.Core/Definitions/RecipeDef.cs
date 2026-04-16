using Downroot.Core.Ids;
using Downroot.Core.Gameplay;

namespace Downroot.Core.Definitions;

public sealed record RecipeDef(
    ContentId Id,
    string DisplayName,
    string SourcePackId,
    IReadOnlyList<ItemAmount> Ingredients,
    ItemAmount Result,
    CraftingStationKind RequiredStationKind = CraftingStationKind.Handcraft,
    RecipeExecutionKind ExecutionKind = RecipeExecutionKind.CreateItem,
    CraftingStationKind? UpgradeStationKind = null,
    float CraftDurationSeconds = 0f,
    IReadOnlyList<ItemAmount>? ExtraResults = null) : ContentDef(Id, DisplayName, SourcePackId);
