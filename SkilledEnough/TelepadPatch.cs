using HarmonyLib;
using UnityEngine.UI;

namespace SkilledEnough
{
    internal class TelepadPatch
    {
        [HarmonyPatch(typeof(Telepad), "OnAcceptDelivery")]
        public static class TelepadOnAcceptDeliveryPatch
        {
            public static void Postfix()
            {
                SkilledEnoughTools.UpdateSkillPointAvailableStatusItem();
            }
        }

        [HarmonyPatch(typeof(TelepadSideScreen), "UpdateSkills")]
        public static class TelepadSideScreenUpdateSkillsPatch
        {
            public static void Postfix(Image ___skillPointsAvailable)
            {
                bool flag = false;
                foreach (MinionResume minionResume in Components.MinionResumes)
                {
                    if (!minionResume.HasTag(GameTags.Dead) && !minionResume.HasTag(SkilledEnoughTools.SkilledEnough) && minionResume.AvailableSkillpoints > 0)
                    {
                        flag = true;
                        break;
                    }
                }
                ___skillPointsAvailable.gameObject.SetActive(flag);
            }
        }
    }
}
