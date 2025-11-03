using HarmonyLib;
using System.Collections.Generic;
using static Chore;

namespace BetterRepair
{
    internal class PreconditionData(BuildingHP buildingHP, BuildingRepairTracker tracker)
    {
        public readonly BuildingHP buildingHP = buildingHP;
        public readonly BuildingRepairTracker tracker = tracker;
    }

    internal class BetterRepairTools
    {
        public static float ConditionThreshold;
        public static float TimeThreshold;
        public static bool RestoreTemperature;
        public static float OverallSpeedMultiplier;
        public static float ConstructionSpeedMultiplier;
        public static float MachinerySpeedMultiplier;
        public static float StrengthSpeedMultiplier;
        public static bool RepairIsTidyingChore;
        public static bool RepairIsBuildingChore;
        public static bool RepairIsOperatingChore;

        public static Precondition AboveThreshold;

        public static void UpdateChores()
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

        public static void UpdateChoreGroupTypes(ChoreGroup choreGroup, bool enableRepair)
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

        public static void InitRepairPrecondition()
        {
            Precondition precondition = new Precondition();
            precondition.id = nameof(AboveThreshold);
            precondition.description = STRINGS.CHORES.PRECONDITIONS.ABOVE_THRESHOLD;
            precondition.fn = (ref Precondition.Context context, object data) =>
            {
                BuildingHP buildingHp = (data as PreconditionData).buildingHP;
                BuildingRepairTracker tracker = (data as PreconditionData).tracker;

                bool conditionThresholdPass = buildingHp.HitPoints < buildingHp.MaxHitPoints * ConditionThreshold;
                bool timeThresholdPass = GameClock.Instance.GetTime() > tracker.GetDamageTimeThreshold();
                return conditionThresholdPass || // allow repair if condition below threshold level
                (timeThresholdPass && !conditionThresholdPass); // complete repair if didn't take damage for threshold time
            };
            precondition.canExecuteOnAnyThread = true;
            AboveThreshold = precondition;
        }
    }
}
