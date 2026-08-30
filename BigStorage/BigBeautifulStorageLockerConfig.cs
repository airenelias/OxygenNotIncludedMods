using PeterHan.PLib.Options;
using STRINGS;
using System.Linq;
using TUNING;
using UnityEngine;

namespace BigStorage;

public class BigBeautifulStorageLockerConfig : IBuildingConfig
{
    public const string ID = "BigBeautifulStorageLocker";

    public override BuildingDef CreateBuildingDef()
    {
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
            ID,
            1, 2,
            "bigbeautifulstoragelocker_kanim",
            30,
            20f,  // increased construction time
            TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4.Concat(new float[1] { 1f }), // increased price
            MATERIALS.REFINED_METALS.Concat(MATERIALS.BUILDING_FIBER),
            1600f,
            BuildLocationRule.OnFloor,
            TUNING.BUILDINGS.DECOR.BONUS.TIER1, // increased decor
            NOISE_POLLUTION.NONE);
        buildingDef.Floodable = false;
        buildingDef.AudioCategory = "Metal";
        buildingDef.Overheatable = false;
        buildingDef.AddSearchTerms(SEARCH_TERMS.STORAGE);
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
        Prioritizable.AddRef(go);
        Storage storage = go.AddOrGet<Storage>();
        storage.showInUI = true;
        storage.allowItemRemoval = true;
        storage.showDescriptor = true;
        storage.capacityKg = SingletonOptions<BigStorage.BigStorageConfig>.Instance.BigBeautifulStorageLockerCapacity; // custom capacity
        storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
        storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
        storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
        storage.showCapacityStatusItem = true;
        storage.showCapacityAsMainStatus = true;
        go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
        go.AddOrGet<StorageLocker>();
        go.AddOrGet<UserNameable>();
        go.AddOrGetDef<RocketUsageRestriction.Def>();
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.AddOrGetDef<StorageController.Def>();
    }
}
