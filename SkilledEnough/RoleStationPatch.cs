using HarmonyLib;

namespace SkilledEnough
{
    internal class RoleStationPatch
    {
        [HarmonyPatch(typeof(RoleStation), "OnSpawn")]
        public static class RoleStationOnSpawnPatch
        {
            public static void Postfix(RoleStation __instance)
            {
                SkilledEnoughSaveData.Instance.LoadData();
                SkilledEnoughTools.RoleStationInstance = __instance;
                SkilledEnoughTools.UpdateSkillPointAvailableStatusItem();
            }
        }

        [HarmonyPatch(typeof(RoleStation), "UpdateSkillPointAvailableStatusItem")]
        public static class RoleStationUpdateSkillPointAvailableStatusItemPatch
        {
            public static void Postfix(RoleStation __instance)
            {
                SkilledEnoughTools.UpdateSkillPointAvailableStatusItem(__instance);
            }
        }
    }
}
