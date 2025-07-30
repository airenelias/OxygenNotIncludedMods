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

        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch("FinaliseElementsTable")]
        public static class ElementLoaderPatch
        {
            public static void Postfix()
            {
                foreach (Element element in ElementLoader.elements.FindAll((Predicate<Element>)
                    (e => e.IsSolid && e.HasTag(GameTags.RefinedMetal) && !e.HasTag(GameTags.Metal))))
                {
                    element.oreTags = element.oreTags.Append(GameTags.Metal);
                }
            }
        }
    }
}
