using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace GestaltEngine
{
    [StaticConstructorOnStartup]
    internal static class Utils
    {
        static Utils()
        {
            var harmony = new Harmony("GestaltEngine.Mod");
            harmony.PatchAll();
            var hooks = new List<MethodInfo>
            {
                AccessTools.Method(typeof(Game), "InitNewGame"),
                AccessTools.Method(typeof(Game), "LoadGame"),
                AccessTools.Method(typeof(SavedGameLoaderNow), "LoadGameFromSaveFileNow")
            };

            foreach (var hook in hooks)
            {
                harmony.Patch(hook, new HarmonyMethod(typeof(Utils), nameof(Utils.ResetStaticData)));
            }
        }

        public static void ResetStaticData()
        {
            CompGestaltEngine.compGestaltEngines.Clear();
        }
        public static bool TryGetGestaltEngineInstead(this Pawn overseer, out CompGestaltEngine result)
        {
            foreach (var comp in CompGestaltEngine.compGestaltEngines)
            {
                if (comp.dummyPawn == overseer)
                {
                    result = comp;
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}
