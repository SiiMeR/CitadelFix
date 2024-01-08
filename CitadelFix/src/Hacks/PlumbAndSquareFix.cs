using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace CitadelFix.Hacks;

public class PlumbAndSquareFix : CitadelFixThing
{
    public static EnumAppSide Side => EnumAppSide.Server;

    public static ModRunPhase RunPhase => ModRunPhase.Start;

    private static ICoreServerAPI sapi;

    public static void Apply(CitadelFixModSystem modSystem, ICoreAPI api)
    {
        sapi = api as ICoreServerAPI;
        sapi.Event.DidBreakBlock += HandleBlockBreak;
        sapi.Event.DidPlaceBlock += HandleBlockPlace;
    }

    public static void Dispose()
    {
        sapi.Event.DidBreakBlock -= HandleBlockBreak;
        sapi.Event.DidPlaceBlock -= HandleBlockPlace;
    }

    public static void HandleBlockBreak(IServerPlayer byPlayer, int oldBlockId, BlockSelection selection)
    {
        ItemSlot slot = byPlayer?.InventoryManager?.GetHotbarInventory()?[10];
        EnumHandHandling handling = EnumHandHandling.Handled;
        BlockPos position = selection.Position;
        if (slot?.Itemstack?.Item is ItemPlumbAndSquare itemPlumbAndSquare)
        {
            itemPlumbAndSquare.OnHeldAttackStart(slot, byPlayer.Entity, selection, null, ref handling);
        }
        if (handling == EnumHandHandling.Handled)
        {
            return;
        }
        byPlayer.Entity.World.BlockAccessor.BreakBlock(position, byPlayer);
    }

    public static void HandleBlockPlace(IServerPlayer byPlayer, int oldBlockId, BlockSelection selection, ItemStack withItemStack)
    {
        ItemSlot slot = byPlayer?.InventoryManager?.GetHotbarInventory()?[10];
        EnumHandHandling handling = EnumHandHandling.Handled;
        BlockPos position = selection.Position;
        if (!(slot?.Itemstack?.Item is ItemPlumbAndSquare itemPlumbAndSquare))
        {
            return;
        }
        itemPlumbAndSquare.OnHeldInteractStart(slot, byPlayer.Entity, selection, null, true, ref handling);
    }
}