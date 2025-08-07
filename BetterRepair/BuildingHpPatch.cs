using HarmonyLib;

namespace BetterRepair
{
    internal class BuildingHpPatch
    {
        [HarmonyPatch(typeof(BuildingHP), "OnPrefabInit")]
        public class BuildingHPOnPrefabInitPatch
        {
            public static void Postfix(BuildingHP __instance)
            {
                __instance.gameObject.AddComponent<BuildingRepairTracker>();
            }
        }

        [HarmonyPatch(typeof(BuildingHP), "OnDoBuildingDamage")]
        public class BuildingHPOnDoBuildingDamagePatch
        {
            public static void Postfix(BuildingHP __instance)
            {
                BuildingRepairTracker tracker = __instance.gameObject.AddOrGet<BuildingRepairTracker>();
                tracker.SetDamageTime();
            }
        }

        [HarmonyPatch(typeof(BuildingHP), "Repair")]
        public class BuildingHPRepairPatch
        {
            public static void Postfix(BuildingHP __instance, Building ___building, int repair_amount)
            {
                if (!BetterRepairTools.RestoreTemperature)
                    return;

                PrimaryElement component = ___building.GetComponent<PrimaryElement>();
                float temperatureDifference = component.Element.defaultValues.temperature - component.Temperature;
                float temperaturePerHp = temperatureDifference / __instance.MaxHitPoints;
                component.Temperature += temperaturePerHp * repair_amount;
            }
        }
    }
}
