using CitadelFix.Fixes;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace CitadelFix;
public class CitadelFixModSystem : ModSystem
{

    internal static CitadelFixModSystem modSystem;
    internal ICoreServerAPI sapi;
    internal Harmony harmony;

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
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

        if(sapi == null) return;
        sapi.Event.DidBreakBlock -= PlumbAndSquareHack.HandleBlockBreak;
        sapi.Event.DidPlaceBlock -= PlumbAndSquareHack.HandleBlockPlace;
    }

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return true;
    }

    private void ApplyHarmonyPatches(){
        harmony = new Harmony("CitadelFix.Patches");
        harmony.PatchAll();
    }

}
