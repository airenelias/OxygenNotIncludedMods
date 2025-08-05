using HarmonyLib;
using Klei.AI;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BetterRepair
{
    public class BetterRepairPatch : UserMod2
    {
        private static float OverallSpeedMultiplier;
        private static float ConstructionSpeedMultiplier;
        private static float MachinerySpeedMultiplier;
        private static float StrengthSpeedMultiplier;
        private static bool RepairIsTidyingChore;
        private static bool RepairIsBuildingChore;
        private static bool RepairIsOperatingChore;

        private static Dictionary<Repairable, int> repairAmountList = new Dictionary<Repairable, int>();

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            new POptions().RegisterOptions(this, typeof(BetterRepairConfig));
        }

        [HarmonyPatch(typeof(Game), "OnSpawn")]
        public static class GameOnSpawnPatch
        {
            public static void Prefix()
            {
                // read the config file each time the game is loaded - so we don't need to restart all the game
                BetterRepairConfig config = POptions.ReadSettings<BetterRepairConfig>() ?? new BetterRepairConfig();
                OverallSpeedMultiplier = config.OverallSpeedMultiplier / 100f;
                ConstructionSpeedMultiplier = config.ConstructionSpeedMultiplier / 100f;
                MachinerySpeedMultiplier = config.MachinerySpeedMultiplier / 100f;
                StrengthSpeedMultiplier = config.StrengthSpeedMultiplier / 100f;
                RepairIsTidyingChore = config.RepairIsTidyingChore;
                RepairIsBuildingChore = config.RepairIsBuildingChore;
                RepairIsOperatingChore = config.RepairIsOperatingChore;
                UpdateChores();
            }
        }

        private static void UpdateChores()
        {
            List<ChoreGroup> repairChoreGroups = new List<ChoreGroup>();

            ChoreGroup tidyingChoreGroup = Db.Get().ChoreGroups.Get("Basekeeping");
            if (RepairIsTidyingChore)
            {
                UpdateChoreGroupTypes(tidyingChoreGroup, true);
                repairChoreGroups.Add(tidyingChoreGroup);
            }
            else
            {
                UpdateChoreGroupTypes(tidyingChoreGroup, false);
            }

            ChoreGroup buildingChoreGroup = Db.Get().ChoreGroups.Get("Build");
            if (RepairIsBuildingChore)
            {
                UpdateChoreGroupTypes(buildingChoreGroup, true);
                repairChoreGroups.Add(buildingChoreGroup);
            }
            else
            {
                UpdateChoreGroupTypes(buildingChoreGroup, false);
            }

            ChoreGroup operatingChoreGroup = Db.Get().ChoreGroups.Get("MachineOperating");
            if (RepairIsOperatingChore)
            {
                UpdateChoreGroupTypes(operatingChoreGroup, true);
                repairChoreGroups.Add(operatingChoreGroup);
            }
            else
            {
                UpdateChoreGroupTypes(operatingChoreGroup, false);
            }

            Traverse.Create(Db.Get().ChoreTypes.Repair).Property("groups").SetValue(repairChoreGroups.ToArray());
        }

        private static void UpdateChoreGroupTypes(ChoreGroup choreGroup, bool enableRepair)
        {
            if (enableRepair)
            {
                if (!choreGroup.choreTypes.Contains(Db.Get().ChoreTypes.Repair))
                    choreGroup.choreTypes.Add(Db.Get().ChoreTypes.Repair);
                if (!choreGroup.choreTypes.Contains(Db.Get().ChoreTypes.RepairFetch))
                    choreGroup.choreTypes.Add(Db.Get().ChoreTypes.RepairFetch);
            }
            else
            {
                choreGroup.choreTypes.Remove(Db.Get().ChoreTypes.Repair);
                choreGroup.choreTypes.Remove(Db.Get().ChoreTypes.RepairFetch);
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

        [HarmonyPatch(typeof(Repairable), "OnPrefabInit")]
        public class RepairableOnPrefabInitPatch
        {
            public static void Postfix(Repairable __instance, ref AttributeConverter ___attributeConverter)
            {
                // remove default construction skill modifier
                ___attributeConverter = null;
            }
        }

        [HarmonyPatch(typeof(Repairable), "OnStartWork")]
        public class RepairableOnStartWorkPatch
        {
            public static void Postfix(Repairable __instance, ref float ___expectedRepairTime)
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
                SetRepairAmount(__instance, hpPerRepairCycle);
            }
        }

        private static void SetRepairAmount(Repairable repairable, int repairAmount)
        {
            if (!repairAmountList.ContainsKey(repairable))
            {
                repairAmountList.Add(repairable, repairAmount);
            }
            else
            {
                repairAmountList[repairable] = repairAmount;
            }
        }

        [HarmonyPatch(typeof(Repairable), "OnWorkTick")]
        public class RepairableOnWorkTickPatch
        {
            public static bool Prefix(Repairable __instance, WorkerBase worker, ref float dt,
                BuildingHP ___hp, float ___expectedRepairTime, ref float ___timeSpentRepairing, ref bool __result)
            {
                // setting repair time multipliers
                float efficiencyMultiplier = 1f;
                efficiencyMultiplier += Db.Get().Attributes.Construction.Lookup(worker).GetTotalValue() * ConstructionSpeedMultiplier;
                efficiencyMultiplier += Db.Get().Attributes.Machinery.Lookup(worker).GetTotalValue() * MachinerySpeedMultiplier;
                efficiencyMultiplier += Db.Get().Attributes.Strength.Lookup(worker).GetTotalValue() * StrengthSpeedMultiplier;
                dt = dt * efficiencyMultiplier * OverallSpeedMultiplier;

                if (___timeSpentRepairing >= ___expectedRepairTime)
                {
                    ___timeSpentRepairing -= ___expectedRepairTime;
                    ___hp.Repair(repairAmountList[__instance]);
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
            public static void Postfix(Repairable __instance)
            {
                UnsetRepairAmount(__instance);
            }
        }

        private static void UnsetRepairAmount(Repairable repairable)
        {
            if (repairAmountList.ContainsKey(repairable))
            {
                repairAmountList.Remove(repairable);
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class LocalizationInitializePatch
        {
            public static void Postfix() => Translate(typeof(STRINGS));

            public static void Translate(Type root)
            {
                // Basic intended way to register strings, keeps namespace
                Localization.RegisterForTranslation(root);
                // Creates template for users to edit
                Localization.GenerateStringsTemplate(root, Path.Combine(Manager.GetDirectory(), "strings_templates"));
                // Load user created translation files
                LoadStrings();
                // Register strings without namespace
                // because we already loaded user transltions, custom languages will overwrite these
                LocString.CreateLocStringKeys(root, null);
            }

            private static void LoadStrings()
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "translations", Localization.GetLocale()?.Code + ".po");
                if (File.Exists(path))
                    Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
            }
        }
    }
}
