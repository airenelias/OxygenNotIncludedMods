using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using System.Linq;

namespace RefinedMetalsAreStillMetals
{
    public class RefinedMetalsAreStillMetalsPatch : UserMod2
    {
        private static readonly SimHashes[] forcedElementsList = [
            SimHashes.EnrichedUranium
        ];

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
                foreach (Element e in ElementLoader.elements)
                {
                    // make all listed elements usable as refined metals and ore
                    if (forcedElementsList.Contains(e.id))
                    {
                        if (!e.HasTag(GameTags.RefinedMetal))
                            e.oreTags = e.oreTags.Append(GameTags.RefinedMetal);
                        if (!e.HasTag(GameTags.Metal))
                            e.oreTags = e.oreTags.Append(GameTags.Metal);
                        continue;
                    }

                    // make all refined metals usable as ore
                    if (e.IsSolid && e.HasTag(GameTags.RefinedMetal) && !e.HasTag(GameTags.Metal))
                    {
                        e.oreTags = e.oreTags.Append(GameTags.Metal);
                    }
                }
            }
        }
    }
}
