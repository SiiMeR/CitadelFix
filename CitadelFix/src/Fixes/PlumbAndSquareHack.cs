using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace CitadelFix.Fixes;

public class PlumbAndSquareHack
{

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