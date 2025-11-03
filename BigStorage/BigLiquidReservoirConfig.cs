using BigStorage;
using PeterHan.PLib.Options;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BigLiquidReservoirConfig : IBuildingConfig
{
    public const string ID = "BigLiquidReservoir";

    public override BuildingDef CreateBuildingDef()
    {
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
            ID,
            2, 6,
            "bigliquidreservoir_kanim",
            100,
            180f,  // increased construction time
            TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, // increased price
            MATERIALS.REFINED_METALS,
            800f,
            BuildLocationRule.OnFloor,
            TUNING.BUILDINGS.DECOR.NONE, // no decor penalty
            NOISE_POLLUTION.NOISY.TIER0);
        buildingDef.InputConduitType = ConduitType.Liquid;
        buildingDef.OutputConduitType = ConduitType.Liquid;
        buildingDef.Floodable = false;
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.UtilityInputOffset = new CellOffset(1, 5);
        buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
        buildingDef.LogicOutputPorts = new List<LogicPorts.Port>
        {
            LogicPorts.Port.OutputPort(
                SmartReservoir.PORT_ID,
                new CellOffset(0, 0),
                STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT,
                STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE,
                STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE)
        };
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
        buildingDef.AddSearchTerms(SEARCH_TERMS.STORAGE);
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        go.AddOrGet<Reservoir>();
        Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
        storage.showDescriptor = true;
        storage.allowItemRemoval = false;
        storage.storageFilters = STORAGEFILTERS.LIQUIDS;
        storage.capacityKg = SingletonOptions<BigStorage.BigStorageConfig>.Instance.BigLiquidStorageCapacity; // custom capacity
        storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
        storage.showCapacityStatusItem = true;
        storage.showCapacityAsMainStatus = true;
        go.AddOrGet<SmartReservoir>();
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Liquid;
        conduitConsumer.ignoreMinMassCheck = true;
        conduitConsumer.forceAlwaysSatisfied = true;
        conduitConsumer.alwaysConsume = true;
        conduitConsumer.capacityKG = storage.capacityKg;
        ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
        conduitDispenser.conduitType = ConduitType.Liquid;
        conduitDispenser.elementFilter = null;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.AddOrGetDef<StorageController.Def>();
        //go.AddOrGetDef<StorageController.Def>();
        //go.AddOrGet<StorageBasedTint>();
        go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
    }

    private class StorageController : GameStateMachine<StorageController, StorageController.Instance>
    {
        #pragma warning disable 0649
        public State to_off, off, on, working, working_loop;
        #pragma warning restore 0649

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = off;
            root
                .EventTransition(GameHashes.OnStorageInteracted, working);
            to_off
                .PlayAnim("to_off")
                .OnAnimQueueComplete(off);
            off
                .PlayAnim("off")
                .EventTransition(GameHashes.OnStorageChange, on, HasLiquid);
            on
                .PlayAnim("on")
                .OnAnimQueueComplete(working_loop);
            working_loop
                .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                .EventTransition(GameHashes.OnStorageChange, to_off, HasNoLiquid);
            working
                .PlayAnim("working")
                .OnAnimQueueComplete(working_loop);
        }

        public class Def : BaseDef
        {
        }

        #pragma warning disable 9113
        public new class Instance(IStateMachineTarget master, Def def) : GameInstance(master)
        #pragma warning restore 9113
        {
        }

        private bool HasLiquid(Instance smi)
        {
            return smi.master.GetComponent<Storage>().MassStored() > ConduitFlow.MAX_LIQUID_MASS;
        }

        private bool HasNoLiquid(Instance smi)
        {
            return smi.master.GetComponent<Storage>().MassStored() == 0f;
        }
    }
}
