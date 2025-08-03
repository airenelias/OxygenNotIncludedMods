using HarmonyLib;
using UnityEngine.UI;

namespace SkilledEnough
{
    internal class TelepadSideScreenPatch
    {
        [HarmonyPatch(typeof(TelepadSideScreen), "UpdateSkills")]
        public static class TelepadSideScreenUpdateSkillsPatch
        {
            public static void Postfix(ref Image ___skillPointsAvailable)
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
