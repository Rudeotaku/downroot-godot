using Downroot.Core.Ids;

namespace Downroot.Core.Definitions;

public sealed record ItemDef(
    ContentId Id,
    string DisplayName,
    string SourcePackId,
    string IconPath,
    int IconWidth,
    int IconHeight,
    int MaxStack,
    ContentId? PlaceableId = null,
    int HungerRestore = 0,
    int HealthRestore = 0,
    ItemUseBehaviorKind UseBehavior = ItemUseBehaviorKind.None,
    HarvestToolDef? HarvestTool = null,
    MeleeWeaponDef? MeleeWeapon = null,
    ThrowableWeaponDef? ThrowableWeapon = null) : ContentDef(Id, DisplayName, SourcePackId);
