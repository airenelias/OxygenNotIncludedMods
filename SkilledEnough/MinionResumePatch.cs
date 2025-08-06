using HarmonyLib;

namespace SkilledEnough
{
    internal class MinionResumePatch
    {
        [HarmonyPatch(typeof(MinionResume), "ShowNewSkillPointNotification")]
        public static class ShowNewSkillPointNotificationPatch
        {
            public static bool Prefix(MinionResume __instance)
            {
                SkilledEnoughSaveData.Instance.LoadData();
                if (__instance.HasTag(SkilledEnoughTools.SkilledEnough))
                    return false;
                return true;
            }
        }
    }
}
