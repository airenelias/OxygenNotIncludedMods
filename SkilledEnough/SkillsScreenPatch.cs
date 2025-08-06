using HarmonyLib;
using PeterHan.PLib.UI;
using System.Collections.Generic;
using UnityEngine;

namespace SkilledEnough
{
    public class SkillsScreenPatch
    {
        [HarmonyPatch(typeof(SkillsScreen), "OnSpawn")]
        public static class SkillsScreenOnSpawnPatch
        {
            public static void Postfix(SkillsScreen __instance)
            {
                SkilledEnoughTools.SkillsScreenInstance = __instance;

                Transform currentLevel = __instance.gameObject.transform.Find("Minions/SelectedDuplicant/Top/BoxButtons/CurrentLevel");
                Transform header = currentLevel.gameObject.transform.Find("Header");
                SkilledEnoughTools.headerText = header.GetComponent<LocText>();
                Transform levelIndicator = currentLevel.gameObject.transform.Find("LevelIndicator");
                SkilledEnoughTools.levelIndicatorText = levelIndicator.GetComponent<LocText>();

                ColorStyleSetting transparent = ScriptableObject.CreateInstance<ColorStyleSetting>();
                transparent.Init(Color.clear);
                transparent.hoverColor = new Color(1f, 1f, 1f, 0.1f);

                PButton suppressButton = new PButton("SuppressButton");
                suppressButton.OnClick = SkilledEnoughTools.Suppress;
                suppressButton.ToolTip = StringFormatter.Replace(STRINGS.UI.SUPPRESS_BUTTON.ENABLE_TOOLTIP, "{Duplicant}", "Duplicant");
                suppressButton.DynamicSize = true;
                suppressButton.Color = transparent;

                SkilledEnoughTools.SuppressButton = suppressButton.AddTo(currentLevel.gameObject);
                SkilledEnoughTools.SuppressButton.transform.SetSiblingIndex(3);

                SkilledEnoughTools.UpdateTooltip();
                SkilledEnoughTools.ColorizeLevel();
                SkilledEnoughTools.ColorizeMasteryPoints();
            }
        }

        [HarmonyPatch(typeof(SkillsScreen), "RefreshSelectedMinion")]
        public static class SkillsScreenRefreshSelectedMinionPatch
        {
            public static void Postfix(SkillsScreen __instance, IAssignableIdentity ___currentlySelectedMinion)
            {
                __instance.GetMinionIdentity(___currentlySelectedMinion, out MinionIdentity minionIdentity, out _);
                if (minionIdentity == null)
                    return;

                SkilledEnoughTools.CurrentlySelectedMinionResume = minionIdentity.GetComponent<MinionResume>();
                SkilledEnoughTools.UpdateTooltip();
                SkilledEnoughTools.ColorizeLevel();
            }
        }

        [HarmonyPatch(typeof(SkillsScreen), "RefreshSkillWidgets")]
        public static class SkillsScreenRefreshSkillWidgetsPatch
        {
            public static void Postfix(SkillsScreen __instance)
            {
                SkilledEnoughTools.ColorizeMasteryPoints(__instance);
            }
        }
    }
}
