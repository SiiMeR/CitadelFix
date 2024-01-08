using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
using Vintagestory.API.Util;
using System;
using Vintagestory.Common;

namespace CitadelFix;

[HarmonyPatch]
internal class HarmonyPatches : CitadelFixThing
{

    public static EnumAppSide Side => EnumAppSide.Universal;

    public static ModRunPhase RunPhase => ModRunPhase.Pre;

    private const string HarmonyPatchName = "CitadelFix.HarmonyPatches";

    private static Harmony harmony;

    public static void Apply(CitadelFixModSystem modSystem, ICoreAPI api)
    {
        if(harmony != null){
            return;
        }
        harmony = new Harmony(HarmonyPatchName);
        harmony.PatchAll();
        api.Logger.Notification("Applied Harmony Patches");
    }

    public static void Dispose()
    {
        harmony?.UnpatchAll();
    }

}