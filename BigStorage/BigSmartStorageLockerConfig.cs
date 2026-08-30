using PeterHan.PLib.Options;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace BigStorage;

internal class BigSmartStorageLockerConfig : IBuildingConfig
{
    public const string ID = "BigSmartStorageLocker";

    public override BuildingDef CreateBuildingDef()
    {
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
            ID,
            1, 2,
            "bigsmartstoragelocker_kanim",
            30,
            90f, // increased construction time
            TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, // increased price
            MATERIALS.REFINED_METALS,
            1600f,
            BuildLocationRule.OnFloor,
            TUNING.BUILDINGS.DECOR.NONE, // no decor penalty
            NOISE_POLLUTION.NONE);
        buildingDef.Floodable = false;
        buildingDef.AudioCategory = "Metal";
        buildingDef.Overheatable = false;
        buildingDef.ViewMode = OverlayModes.Logic.ID;
        buildingDef.RequiresPowerInput = true;
        buildingDef.AddLogicPowerPort = false;
        buildingDef.EnergyConsumptionWhenActive = 60f;
        buildingDef.ExhaustKilowattsWhenActive = 0.125f;
        buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
        {
            LogicPorts.Port.OutputPort(
                FilteredStorage.FULL_PORT_ID,
                new CellOffset(0, 1),
                global::STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT,
                global::STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_ACTIVE,
                global::STRINGS.BUILDINGS.PREFABS.STORAGELOCKERSMART.LOGIC_PORT_INACTIVE,
                show_wire_missing_icon: true)
        };
        buildingDef.AddSearchTerms(SEARCH_TERMS.STORAGE);
        return buildingDef;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
        Prioritizable.AddRef(go);
        Storage storage = go.AddOrGet<Storage>();
        storage.showInUI = true;
        storage.allowItemRemoval = true;
        storage.showDescriptor = true;
        storage.capacityKg = SingletonOptions<BigStorage.BigStorageConfig>.Instance.BigSmartStorageLockerCapacity; // custom capacity
        storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
        storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
        storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
        storage.showCapacityStatusItem = true;
        storage.showCapacityAsMainStatus = true;
        go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
        go.AddOrGet<StorageLockerSmart>();
        go.AddOrGet<UserNameable>();
        go.AddOrGetDef<StorageController.Def>();
        go.AddOrGetDef<RocketUsageRestriction.Def>();
    }
}
