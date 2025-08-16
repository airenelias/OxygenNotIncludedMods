using HarmonyLib;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SkilledEnough
{
    internal class SkilledEnoughTools
    {
        public static readonly Tag SkilledEnough = TagManager.Create(nameof(SkilledEnough));

        public static RoleStation RoleStationInstance = null;
        public static SkillsScreen SkillsScreenInstance = null;
        public static GameObject SuppressButton = null;

        public static LocText headerText, levelIndicatorText;
        public static MinionResume CurrentlySelectedMinionResume = null;

        public static void Suppress(GameObject _)
        {
            if (!CurrentlySelectedMinionResume.HasTag(SkilledEnough))
            {
                CurrentlySelectedMinionResume.AddTag(SkilledEnough);
                UpdateTooltip();
                ColorizeMasteryPoints(CurrentlySelectedMinionResume.GetComponent<KPrefabID>()?.InstanceID, false);
            }
            else
            {
                CurrentlySelectedMinionResume.RemoveTag(SkilledEnough);
                UpdateTooltip();
                ColorizeMasteryPoints(CurrentlySelectedMinionResume.GetComponent<KPrefabID>()?.InstanceID, true);
            }
            ColorizeLevel();
            UpdateSkillPointAvailableStatusItem();
        }

        public static void UpdateTooltip()
        {
            if (CurrentlySelectedMinionResume == null || SuppressButton == null)
                return;

            string name = CurrentlySelectedMinionResume.GetIdentity.GetProperName();
            bool suppressed = CurrentlySelectedMinionResume.HasTag(SkilledEnough);
            ToolTip tooltip = SuppressButton.GetComponent<ToolTip>();
            TextStyleSetting style = tooltip.GetStyleSetting(0);
            if (suppressed)
            {
                tooltip.ClearMultiStringTooltip();
                tooltip.AddMultiStringTooltip(StringFormatter.Replace(STRINGS.UI.SUPPRESS_BUTTON.DISABLE_TOOLTIP, "{Duplicant}", name), style);
            }
            else
            {
                tooltip.ClearMultiStringTooltip();
                tooltip.AddMultiStringTooltip(StringFormatter.Replace(STRINGS.UI.SUPPRESS_BUTTON.ENABLE_TOOLTIP, "{Duplicant}", name), style);
            }
        }

        public static void ColorizeLevel()
        {
            if (CurrentlySelectedMinionResume == null || headerText == null || levelIndicatorText == null)
                return;

            if (CurrentlySelectedMinionResume.HasTag(SkilledEnough))
            {
                headerText.color = levelIndicatorText.color = Color.gray;
            }
            else
            {
                headerText.color = levelIndicatorText.color = Color.white;
            }
        }

        public static void ColorizeMasteryPoints()
        {
            ColorizeMasteryPoints(null);
        }

        public static void ColorizeMasteryPoints(SkillsScreen __instance)
        {
            ColorizeMasteryPoints(__instance, null, false);
        }

        public static void ColorizeMasteryPoints(int? prefabId, bool colorize)
        {
            ColorizeMasteryPoints(null, prefabId, colorize);
        }

        public static void ColorizeMasteryPoints(SkillsScreen __instance, int? instanceId, bool colorize)
        {
            if (__instance == null)
            {
                if (SkillsScreenInstance == null)
                    return;
                __instance = SkillsScreenInstance;
            }

            Transform content = __instance.gameObject.transform.Find("Minions/Contents/Scroll View/Viewport/Content");
            for (int childIdx = 0; childIdx < content.gameObject.transform.childCount; childIdx++)
            {
                // iterating over every duplicant row
                Transform child = content.gameObject.transform.GetChild(childIdx);
                if (!child.name.Equals("MinionPrefab2"))
                    continue;

                SkillMinionWidget skillMinionWidget = child.GetComponent<SkillMinionWidget>();
                __instance.GetMinionIdentity(skillMinionWidget.assignableIdentity, out MinionIdentity minionIdentity, out _);
                if (minionIdentity == null)
                    continue;

                // if looking for a specific duplicant
                if (instanceId != null)
                {
                    KPrefabID prefab = minionIdentity.GetComponent<KPrefabID>();
                    if (prefab == null)
                        continue;
                    // skipping not matching ids
                    if (prefab.InstanceID != instanceId)
                        continue;
                }

                Transform label = child.gameObject.transform.Find("MasteryPoints/Label");
                LocText locText = label.GetComponent<LocText>();
                if (instanceId != null)
                {
                    // if looking for a duplicant, then might need to return greens
                    if (colorize)
                    {
                        // if has mastery points, colorize
                        if (int.Parse(Regex.Replace(locText.text, "<.*?>", string.Empty)) > 0)
                        {
                            locText.SetText(ColorizeMasteryPoint(locText.text));
                        }
                    }
                    else
                    {
                        locText.SetText(DecolorizeMasteryPoint(locText.text));
                    }
                }
                else
                {
                    // else it's just refresh, keep greens, decolorize others
                    if (minionIdentity.HasTag(SkilledEnough))
                    {
                        locText.SetText(DecolorizeMasteryPoint(locText.text));
                    }
                }
            }
        }

        private static string ColorizeMasteryPoint(string masteryPointText)
        {
            if (masteryPointText.Contains("color"))
                return masteryPointText;
            return "<color=#80FF80FF>" + masteryPointText + "</color>";
        }

        private static string DecolorizeMasteryPoint(string masteryPointText)
        {
            if (!masteryPointText.Contains("color"))
                return masteryPointText;
            return Regex.Replace(masteryPointText, "<color=.*?>|<\\/color>", string.Empty);
        }

        public static void UpdateSkillPointAvailableStatusItem()
        {
            UpdateSkillPointAvailableStatusItem(null);
        }

        public static void UpdateSkillPointAvailableStatusItem(RoleStation __instance)
        {
            if (__instance == null)
            {
                if (RoleStationInstance == null)
                    return;
                __instance = RoleStationInstance;
            }

            Guid skillPointAvailableStatusItem = Traverse.Create(__instance).Field("skillPointAvailableStatusItem").GetValue<Guid>();
            foreach (MinionResume minionResume in Components.MinionResumes)
            {
                if (!minionResume.HasTag(GameTags.Dead) && !minionResume.HasTag(SkilledEnough) && minionResume.AvailableSkillpoints > 0)
                {
                    if (!(skillPointAvailableStatusItem == Guid.Empty))
                        return;
                    skillPointAvailableStatusItem = __instance.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable);
                    return;
                }
            }
            __instance.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SkillPointsAvailable);
            Traverse.Create(__instance).Field("skillPointAvailableStatusItem").SetValue(Guid.Empty);
            return;
        }
    }
}
