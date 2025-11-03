using BigStorage;
using PeterHan.PLib.Options;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BigGasReservoirConfig : IBuildingConfig
{
    public const string ID = "BigGasReservoir";

    public static readonly List<Storage.StoredItemModifier> ReservoirStoredItemModifiers = new List<Storage.StoredItemModifier>
        {
            Storage.StoredItemModifier.Hide,
            Storage.StoredItemModifier.Seal
        };

    public override BuildingDef CreateBuildingDef()
    {
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
            ID,
            10, 3,
            "biggasreservoir_kanim",
            100,
            180f,  // increased construction time
            TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, // increased price
            MATERIALS.REFINED_METALS,
            800f,
            BuildLocationRule.OnFloor,
            TUNING.BUILDINGS.DECOR.NONE, // no decor penalty
            NOISE_POLLUTION.NOISY.TIER0);
        buildingDef.InputConduitType = ConduitType.Gas;
        buildingDef.OutputConduitType = ConduitType.Gas;
        buildingDef.Floodable = false;
        buildingDef.ViewMode = OverlayModes.GasConduits.ID;
        buildingDef.AudioCategory = "HollowMetal";
        buildingDef.UtilityInputOffset = new CellOffset(1, 2);
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
        GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, ID);
        buildingDef.AddSearchTerms(SEARCH_TERMS.STORAGE);
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        go.AddOrGet<Reservoir>();
        Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
        storage.showDescriptor = true;
        storage.storageFilters = STORAGEFILTERS.GASES;
        storage.capacityKg = SingletonOptions<BigStorage.BigStorageConfig>.Instance.BigGasStorageCapacity; // custom capacity
        storage.SetDefaultStoredItemModifiers(ReservoirStoredItemModifiers);
        storage.showCapacityStatusItem = true;
        storage.showCapacityAsMainStatus = true;
        go.AddOrGet<SmartReservoir>();
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Gas;
        conduitConsumer.ignoreMinMassCheck = true;
        conduitConsumer.forceAlwaysSatisfied = true;
        conduitConsumer.alwaysConsume = true;
        conduitConsumer.capacityKG = storage.capacityKg;
        ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
        conduitDispenser.conduitType = ConduitType.Gas;
        conduitDispenser.elementFilter = null;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        go.AddOrGetDef<StorageController.Def>();
        go.AddOrGet<StorageBasedTint>();
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
                .EventTransition(GameHashes.OnStorageChange, on, HasGas);
            on
                .PlayAnim("on")
                .OnAnimQueueComplete(working_loop);
            working_loop
                .PlayAnim("working_loop", KAnim.PlayMode.Loop)
                .EventTransition(GameHashes.OnStorageChange, to_off, HasNoGas);
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

        private bool HasGas(Instance smi)
        {
            return smi.master.GetComponent<Storage>().MassStored() > ConduitFlow.MAX_GAS_MASS;
        }

        private bool HasNoGas(Instance smi)
        {
            return smi.master.GetComponent<Storage>().MassStored() == 0;
        }
    }
}
