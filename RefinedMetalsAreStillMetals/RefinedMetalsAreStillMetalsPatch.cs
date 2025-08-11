using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using System;

namespace RefinedMetalsAreStillMetals
{
    public class RefinedMetalsAreStillMetalsPatch : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }

        [HarmonyPatch(typeof(ElementLoader), "FinaliseElementsTable")]
        public static class ElementLoaderFinaliseElementsTablePatch
        {
            public static void Postfix()
            {
                // make all refined metals usable as ore
                foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>)
                    (e => e.IsSolid && e.HasTag(GameTags.RefinedMetal) && !e.HasTag(GameTags.Metal))))
                {
                    element.oreTags = element.oreTags.Append(GameTags.Metal);
                }

                // make enriched uranium usable as refined and ore
                Element enrichedUranium = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium);
                if (enrichedUranium != null)
                {
                    enrichedUranium.oreTags = enrichedUranium.oreTags.Append(new[] { GameTags.RefinedMetal, GameTags.Metal });
                }
            }
        }
    }
}
