using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.IO;
using System.Reflection;

namespace BetterRepair
{
    public class BetterRepairPatch : UserMod2
    {
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
                BetterRepairTools.ConditionThreshold = config.ConditionThreshold / 100f;
                BetterRepairTools.TimeThreshold = config.TimeThreshold;
                BetterRepairTools.RestoreTemperature = config.RestoreTemperature;
                BetterRepairTools.OverallSpeedMultiplier = config.OverallSpeedMultiplier / 100f;
                BetterRepairTools.ConstructionSpeedMultiplier = config.ConstructionSpeedMultiplier / 100f;
                BetterRepairTools.MachinerySpeedMultiplier = config.MachinerySpeedMultiplier / 100f;
                BetterRepairTools.StrengthSpeedMultiplier = config.StrengthSpeedMultiplier / 100f;
                BetterRepairTools.RepairIsTidyingChore = config.RepairIsTidyingChore;
                BetterRepairTools.RepairIsBuildingChore = config.RepairIsBuildingChore;
                BetterRepairTools.RepairIsOperatingChore = config.RepairIsOperatingChore;
                BetterRepairTools.UpdateChores();
                BetterRepairTools.InitRepairPrecondition();
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
