using System.Linq;
using System.Reflection;
using System.Text;
using CitadelFix.Fixes;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace CitadelFix;
public class CitadelFixModSystem : ModSystem
{

    internal static CitadelFixModSystem modSystem;
    internal static ICoreServerAPI sapi;
    internal static ICoreAPI api;
    internal Harmony harmony;

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        CitadelFixModSystem.api = api;
        this.ApplyHarmonyPatches();
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        modSystem = this;
        sapi = api;
        api.Event.DidBreakBlock += PlumbAndSquareHack.HandleBlockBreak;
        api.Event.DidPlaceBlock += PlumbAndSquareHack.HandleBlockPlace;
    }

    public override void Dispose()
    {
        base.Dispose();
        harmony?.UnpatchAll();
        harmony = null;

        if (sapi == null) return;
        sapi.Event.DidBreakBlock -= PlumbAndSquareHack.HandleBlockBreak;
        sapi.Event.DidPlaceBlock -= PlumbAndSquareHack.HandleBlockPlace;
    }

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return true;
    }

    private void ApplyHarmonyPatches()
    {
        if(harmony != null){
            return;
        }
        harmony = new Harmony($"CitadelFix.Patches");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        var builder = new StringBuilder();
        var patchedMethods = harmony.GetPatchedMethods();
        builder.Append($"Applied {patchedMethods.Count()} patches for {api.Side} side:\n");
        foreach(var patchedMethod in patchedMethods){
            builder.Append($"    {patchedMethod}\n");
        }
        builder.Remove(builder.Length-1, 1);
        api.Logger.Notification(builder.ToString());
    }

}
