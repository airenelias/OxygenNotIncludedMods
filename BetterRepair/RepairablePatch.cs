using HarmonyLib;
using Klei.AI;
using UnityEngine;

namespace BetterRepair
{
    internal class RepairablePatch
    {
        [HarmonyPatch(typeof(Repairable), "OnPrefabInit")]
        public class RepairableOnPrefabInitPatch
        {
            public static void Postfix(ref AttributeConverter ___attributeConverter)
            {
                // remove default construction skill modifier
                ___attributeConverter = null;
            }
        }

        [HarmonyPatch(typeof(Repairable.SMInstance), "HasRequiredMass")]
        public class RepairableHasRequiredMassPatch
        {
            public static void Postfix(ref bool __result)
            {
                // skip the materials requirement
                __result = true;
            }
        }

        [HarmonyPatch(typeof(Repairable.States), "CreateRepairChore")]
        public class RepairableCreateRepairChorePatch
        {
            public static void Postfix(Repairable.SMInstance smi, Chore __result)
            {
                BuildingHP buildingHp = smi.master.GetComponent<BuildingHP>();
                if (buildingHp == null)
                    return;
                __result.AddPrecondition(BetterRepairTools.AboveThreshold, buildingHp);
            }
        }

        [HarmonyPatch(typeof(Repairable), "OnStartWork")]
        public class RepairableOnStartWorkPatch
        {
            public static void Postfix(Repairable __instance, BuildingHP ___hp, ref float ___expectedRepairTime)
            {
                BuildingDef buildingDef = __instance.GetComponent<BuildingComplete>().Def;
                if (buildingDef == null)
                    return;

                float repairTimePerHp = buildingDef.ConstructionTime / buildingDef.HitPoints;

                // get repair tick timer from base game so it wouldn't go off too often
                float defaultExpectedRepairTime = Mathf.Sqrt(__instance.GetComponent<PrimaryElement>().Mass) * 0.1f;
                // adjust repair time and hp to ticks
                int hpPerRepairCycle = Mathf.CeilToInt(defaultExpectedRepairTime / repairTimePerHp);
                ___expectedRepairTime = repairTimePerHp * hpPerRepairCycle;
                BuildingRepairTracker tracker = ___hp.gameObject.AddOrGet<BuildingRepairTracker>();
                tracker.SetRepairAmount(hpPerRepairCycle);
            }
        }

        [HarmonyPatch(typeof(Repairable), "OnWorkTick")]
        public class RepairableOnWorkTickPatch
        {
            public static bool Prefix(WorkerBase worker, ref float dt,
                BuildingHP ___hp, float ___expectedRepairTime, ref float ___timeSpentRepairing, ref bool __result)
            {
                // setting repair time multipliers
                float efficiencyMultiplier = 1f;
                efficiencyMultiplier += Db.Get().Attributes.Construction.Lookup(worker).GetTotalValue() * BetterRepairTools.ConstructionSpeedMultiplier;
                efficiencyMultiplier += Db.Get().Attributes.Machinery.Lookup(worker).GetTotalValue() * BetterRepairTools.MachinerySpeedMultiplier;
                efficiencyMultiplier += Db.Get().Attributes.Strength.Lookup(worker).GetTotalValue() * BetterRepairTools.StrengthSpeedMultiplier;
                dt = dt * efficiencyMultiplier * BetterRepairTools.OverallSpeedMultiplier;

                if (___timeSpentRepairing >= ___expectedRepairTime)
                {
                    ___timeSpentRepairing -= ___expectedRepairTime;
                    BuildingRepairTracker tracker = ___hp.gameObject.AddOrGet<BuildingRepairTracker>();
                    ___hp.Repair(tracker.repairAmount);
                    if (___hp.HitPoints >= ___hp.MaxHitPoints)
                    {
                        __result = true;
                        return false;
                    }
                }
                ___timeSpentRepairing += dt;
                __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(Repairable), "OnStopWork")]
        public class RepairableOnStopWorkPatch
        {
            public static void Postfix(BuildingHP ___hp)
            {
                // unset repair amount in case other duplicant will continue repairs
                BuildingRepairTracker tracker = ___hp.gameObject.AddOrGet<BuildingRepairTracker>();
                tracker.UnsetRepairAmount();
            }
        }
    }
}
