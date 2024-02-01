using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace GestaltEngine
{
    [HarmonyPatch]
    public static class ITab_Bills_FillTab_NoMechanitorWarning_Patch
    {
        public static MethodBase TargetMethod()
        {
            foreach (var type in typeof(ITab_Bills).GetNestedTypes(AccessTools.all))
            {
                foreach (var method in type.GetMethods(AccessTools.all))
                {
                    if (method.Name.Contains("<FillTab>") && method.ReturnType == typeof(void) 
                        && method.GetParameters().Count() == 0)
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            bool patched = false; 
            var loadMapCodes = new List<CodeInstruction>();
            var mapInfo = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Map));
            for (var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (i > 6 && code.Calls(mapInfo))
                {
                    loadMapCodes = new List<CodeInstruction>
                    {
                        codes[i - 5],
                        codes[i - 4],
                        codes[i - 3],
                        codes[i - 2],
                        codes[i - 1],
                        codes[i]
                    };
                }
                if (i > 0 && !patched && codes[i].opcode == OpCodes.Brtrue_S && (codes[i - 1].operand?.ToString().Contains("Any") ?? false))
                {
                    patched = true;
                    foreach (var loadMapCode in loadMapCodes)
                    {
                        yield return loadMapCode;
                    }
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ITab_Bills_FillTab_NoMechanitorWarning_Patch),
                        nameof(GestaltEnginePresent)));
                    yield return new CodeInstruction(code.opcode, code.operand);
                }
            }
        }

        public static bool GestaltEnginePresent(Map map)
        {
            return CompGestaltEngine.compGestaltEngines.Any(x => x.parent.Map == map);
        }
    }
}
