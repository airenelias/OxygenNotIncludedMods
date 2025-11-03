using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

namespace BigStorage
{
    public class BigStoragePatch : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
            new POptions().RegisterOptions(this, typeof(BigStorageConfig));

            // Increasing maximum allowed mass for non-stackable entities (water/gas)
            float maxMass = Math.Max(
                SingletonOptions<BigStorageConfig>.Instance.BigLiquidStorageCapacity,
                SingletonOptions<BigStorageConfig>.Instance.BigGasStorageCapacity);
            if (maxMass > PrimaryElement.MAX_MASS)
                PrimaryElement.MAX_MASS = maxMass;
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class BigStorageGeneratedBuildingsPatch
        {
            public static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen("Base", BigStorageLockerConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigBeautifulStorageLockerConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigSmartStorageLockerConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigLiquidStorageConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigLiquidReservoirConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigGasStorageConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigGasReservoirConfig.ID);
                ModUtil.AddBuildingToPlanScreen("Base", BigStorageTileConfig.ID);
                if (SingletonOptions<BigStorageConfig>.Instance.BigRefrigeratorEnabled)
                    ModUtil.AddBuildingToPlanScreen("Food", BigRefrigeratorConfig.ID);
            }

            [HarmonyPatch(typeof(Db), "Initialize")]
            public class BigStorageDbPatch
            {
                public static void Postfix()
                {
                    Db.Get().Techs.Get("RefinedObjects").unlockedItemIDs.Add(BigStorageLockerConfig.ID);
                    Db.Get().Techs.Get("Smelting").unlockedItemIDs.Add(BigBeautifulStorageLockerConfig.ID);
                    Db.Get().Techs.Get("SolidTransport").unlockedItemIDs.Add(BigSmartStorageLockerConfig.ID);
                    Db.Get().Techs.Get("LiquidTemperature").unlockedItemIDs.Add(BigLiquidStorageConfig.ID);
                    Db.Get().Techs.Get("Catalytics").unlockedItemIDs.Add(BigGasStorageConfig.ID);
                    Db.Get().Techs.Get("Catalytics").unlockedItemIDs.Add(BigGasReservoirConfig.ID);
                    Db.Get().Techs.Get("SolidManagement").unlockedItemIDs.Add(BigStorageTileConfig.ID);
                    if (SingletonOptions<BigStorageConfig>.Instance.BigRefrigeratorEnabled)
                        Db.Get().Techs.Get("FoodRepurposing").unlockedItemIDs.Add(BigRefrigeratorConfig.ID);
                }
            }

            [HarmonyPatch(typeof(Mod), "Load")]
            public class ActiveControllerInitializeStatesPatch
            {
                public static void Postfix(Mod __instance)
                {
                    Debug.Log("[BigStorage] mod loaded " + __instance.staticID);
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
                    Localization.GenerateStringsTemplate(root,
                        Path.Combine(Manager.GetDirectory(), "strings_templates"));
                    // Load user created translation files
                    LoadStrings();
                    // Register strings without namespace
                    // because we already loaded user transltions, custom languages will overwrite these
                    LocString.CreateLocStringKeys(root, null);
                }

                private static void LoadStrings()
                {
                    string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        "translations", Localization.GetLocale()?.Code + ".po");
                    if (File.Exists(path))
                        Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
                }
            }
        }
    }
}
