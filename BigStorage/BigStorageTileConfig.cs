using PeterHan.PLib.Options;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace BigStorage;

public class BigStorageTileConfig : IBuildingConfig
{
    public const string ID = "BigStorageTile";

    private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
    {
        Storage.StoredItemModifier.Insulate,
        Storage.StoredItemModifier.Seal,
        Storage.StoredItemModifier.Hide
    };

    public override BuildingDef CreateBuildingDef()
    {
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
            ID,
            1, 1,
            "bigstoragetile_kanim",
            30,
            45f, // increased construction time
            TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3.Concat(TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3), // increased price
            MATERIALS.REFINED_METALS.Concat(MATERIALS.GLASSES),
            800f,
            BuildLocationRule.Tile,
            TUNING.BUILDINGS.DECOR.NONE, // no decor penalty
            NOISE_POLLUTION.NONE);
        BuildingTemplates.CreateFoundationTileDef(buildingDef);
        buildingDef.Floodable = false;
        buildingDef.Entombable = false;
        buildingDef.Overheatable = false;
        buildingDef.UseStructureTemperature = false;
        buildingDef.AudioCategory = "Glass";
        buildingDef.AudioSize = "small";
        buildingDef.BaseTimeUntilRepair = -1f;
        buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
        buildingDef.AddSearchTerms(SEARCH_TERMS.TILE);
        buildingDef.AddSearchTerms(SEARCH_TERMS.STORAGE);
        buildingDef.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        GeneratedBuildings.MakeBuildingAlwaysOperational(go);
        BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
        SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
        simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT_MODIFIERS.PENALTY_2;
        simCellOccupier.notifyOnMelt = true;
        Storage storage = go.AddOrGet<Storage>();
        storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
        storage.capacityKg = SingletonOptions<BigStorage.BigStorageConfig>.Instance.BigStorageTileCapacity; // custom capacity
        storage.showInUI = true;
        storage.allowItemRemoval = true;
        storage.showDescriptor = true;
        storage.storageFilters = STORAGEFILTERS.STORAGE_LOCKERS_STANDARD;
        storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
        storage.showCapacityStatusItem = true;
        storage.showCapacityAsMainStatus = true;
        go.AddOrGet<StorageTileSwitchItemWorkable>();
        TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
        treeFilterable.copySettingsEnabled = false;
        treeFilterable.dropIncorrectOnFilterChange = false;
        treeFilterable.preventAutoAddOnDiscovery = true;
        StorageTile.Def def = go.AddOrGetDef<StorageTile.Def>();
        def.MaxCapacity = SingletonOptions<BigStorage.BigStorageConfig>.Instance.BigStorageTileCapacity; // custom capacity
        def.specialItemCases = new StorageTile.SpecificItemTagSizeInstruction[3]
        {
      new StorageTile.SpecificItemTagSizeInstruction(GameTags.AirtightSuit, 0.5f),
      new StorageTile.SpecificItemTagSizeInstruction(GameTags.Dehydrated, 0.6f),
      new StorageTile.SpecificItemTagSizeInstruction(GameTags.MoltShell, 0.5f)
        };
        go.AddOrGet<TileTemperature>();
        go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
        Prioritizable.AddRef(go);
        go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        GeneratedBuildings.RemoveLoopingSounds(go);
        go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
    }
}
