using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.IO;
using System.Reflection;

namespace GeotuneActivity
{
    public class GeotuneActivityPatch : UserMod2
    {
        private static float ActivityMultiplier;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            new POptions().RegisterOptions(this, typeof(GeotuneActivityConfig));
        }

        [HarmonyPatch(typeof(Game), "OnSpawn")]
        public static class GameOnSpawnPatch
        {
            public static void Prefix()
            {
                // read the config file each time the game is loaded - so we don't need to restart all the game
                GeotuneActivityConfig config = POptions.ReadSettings<GeotuneActivityConfig>() ?? new GeotuneActivityConfig();
                ActivityMultiplier = config.ActivityMultiplier;
            }
        }

        [HarmonyPatch(typeof(GeoTuner.Instance), "RefreshModification")]
        public class GeoTunerRefreshModificationPatch
        {
            public static void Prefix(GeoTuner.Instance __instance)
            {
                Debug.Log("[GeotuneActivity] this.currentGeyserModification = " + __instance.currentGeyserModification);

                Geyser assignedGeyser = __instance.GetAssignedGeyser();
                if (assignedGeyser == null)
                {
                    Debug.Log("[GeotuneActivity] got geyser " + assignedGeyser);
                    return;
                }
                GeoTunerConfig.GeotunedGeyserSettings settingsForGeyser = __instance.def.GetSettingsForGeyser(assignedGeyser);
                Debug.Log("[GeotuneActivity] got settings yearPercentageModifier " + settingsForGeyser.template.yearPercentageModifier);
                settingsForGeyser.template.yearPercentageModifier = 1;
                Debug.Log("[GeotuneActivity] set settings yearPercentageModifier " + settingsForGeyser.template.yearPercentageModifier);
            }
        }
/*
        [HarmonyPatch(typeof(GeoTuner), "ApplyTuning")]
        public class ApplyTuningPatch
        {
            public static void Prefix(GeoTuner.Instance smi)
            {
                Debug.Log("[GeotuneActivity] apply tuning");
                if (smi == null)
                {
                    Debug.Log("[GeotuneActivity] GeoTuner.Instance not found");
                    return;
                }
                Geyser geyser = smi.GetAssignedGeyser();
                Debug.Log("[GeotuneActivity] geyser = " + geyser);
                if (geyser != null)
                {
                    Debug.Log("[GeotuneActivity] before:");
                    Debug.Log("[GeotuneActivity] rateRoll " + geyser.configuration.rateRoll);
                    Debug.Log("[GeotuneActivity] iterationLengthRoll " + geyser.configuration.iterationLengthRoll);
                    Debug.Log("[GeotuneActivity] iterationPercentRoll " + geyser.configuration.iterationPercentRoll);
                    Debug.Log("[GeotuneActivity] yearLengthRoll " + geyser.configuration.yearLengthRoll);
                    Debug.Log("[GeotuneActivity] yearPercentRoll " + geyser.configuration.yearPercentRoll);
                    Debug.Log("[GeotuneActivity] scaledRate " + geyser.configuration.scaledRate);
                    Debug.Log("[GeotuneActivity] scaledIterationLength " + geyser.configuration.scaledIterationLength);
                    Debug.Log("[GeotuneActivity] scaledIterationPercent " + geyser.configuration.scaledIterationPercent);
                    Debug.Log("[GeotuneActivity] scaledYearLength " + geyser.configuration.scaledYearLength);
                    Debug.Log("[GeotuneActivity] scaledYearPercent " + geyser.configuration.scaledYearPercent);
                }

                smi.currentGeyserModification.yearPercentageModifier = 2;
                Debug.Log("[GeotuneActivity] set yearPercentageModifier 2");
            }
            public static void Postfix(GeoTuner.Instance smi)
            {
                Debug.Log("[GeotuneActivity] after tuning");
                if (smi == null)
                {
                    Debug.Log("[GeotuneActivity] GeoTuner.Instance not found");
                    return;
                }
                Geyser geyser = smi.GetAssignedGeyser();
                Debug.Log("[GeotuneActivity] geyser = " + geyser);
                if (geyser != null)
                {
                    Debug.Log("[GeotuneActivity] after:");
                    Debug.Log("[GeotuneActivity] rateRoll " + geyser.configuration.rateRoll);
                    Debug.Log("[GeotuneActivity] iterationLengthRoll " + geyser.configuration.iterationLengthRoll);
                    Debug.Log("[GeotuneActivity] iterationPercentRoll " + geyser.configuration.iterationPercentRoll);
                    Debug.Log("[GeotuneActivity] yearLengthRoll " + geyser.configuration.yearLengthRoll);
                    Debug.Log("[GeotuneActivity] yearPercentRoll " + geyser.configuration.yearPercentRoll);
                    Debug.Log("[GeotuneActivity] scaledRate " + geyser.configuration.scaledRate);
                    Debug.Log("[GeotuneActivity] scaledIterationLength " + geyser.configuration.scaledIterationLength);
                    Debug.Log("[GeotuneActivity] scaledIterationPercent " + geyser.configuration.scaledIterationPercent);
                    Debug.Log("[GeotuneActivity] scaledYearLength " + geyser.configuration.scaledYearLength);
                    Debug.Log("[GeotuneActivity] scaledYearPercent " + geyser.configuration.scaledYearPercent);
                }
            }
        }*/

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
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
