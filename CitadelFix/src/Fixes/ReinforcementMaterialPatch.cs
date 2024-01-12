using System.Linq;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
using Vintagestory.API.Util;
using System.Collections.Generic;

namespace CitadelFix.Fixes;

[HarmonyPatch]
public class ReinforcmentMaterialPatch
{

    private static ItemSlot getPreferredReinforcementSlot(ItemSlot slotA, ItemSlot slotB){
        if (slotA == null){
            return slotB;
        }
        if (slotB == null){
            return slotA;
        }

        int strengthA = slotA.Itemstack.ItemAttributes["reinforcementStrength"].AsInt(0);
        int strengthB = slotB.Itemstack.ItemAttributes["reinforcementStrength"].AsInt(0);

        if(strengthA == 0 && strengthB != 0){
            return slotB;
        }

        if(strengthB == 0 && strengthA != 0){
            return slotA;
        }

        if(slotA.Inventory.ClassName.EqualsFast(GlobalConstants.hotBarInvClassName) && slotB.Inventory.ClassName.EqualsFast(GlobalConstants.hotBarInvClassName)){
            if(strengthA > strengthB){
                return slotA;
            }else{
                return slotB;
            }
        }

        if(slotA.Inventory.ClassName.EqualsFast(GlobalConstants.hotBarInvClassName)){
            return slotA;
        }

        if(slotB.Inventory.ClassName.EqualsFast(GlobalConstants.hotBarInvClassName)){
            return slotB;
        }

        if (strengthA > strengthB){
            return slotA;
        } else {
            return slotB;
        }

    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemBlockReinforcement), "FindResourceForReinforcing")]
    public static bool FindResourceForReinforcing(ModSystemBlockReinforcement __instance, IPlayer byPlayer, ref ItemSlot __result)
    {
        ItemSlot foundSlot = null;
        var sapi = CitadelFixModSystem.sapi;
        var data = sapi.PlayerData.GetPlayerDataByUid(byPlayer.PlayerUID);
        var preferredMaterial = data.CustomPlayerData.GetValueOrDefault("CitadelFixPreferredMaterial", null);

        foreach (var inv in byPlayer.InventoryManager.Inventories.Select(p => p.Value).ToList())
        {
            for (int i = 0; i < inv.Count; i++)
            {
                var slot = inv[i];
                if (slot == null ||
                slot.Empty ||
                slot.Itemstack.ItemAttributes == null ||
                slot.Itemstack.ItemAttributes["reinforcementStrength"].AsInt(0) == 0 ||
                slot is ItemSlotCreative || !(slot.Inventory is InventoryBasePlayer))
                {
                    continue;
                }

                if(preferredMaterial != null && slot.Itemstack.Collectible.Code.ToString().EqualsFast(preferredMaterial)){
                    foundSlot = slot;
                    break;
                }

                ItemSlot newSlot = getPreferredReinforcementSlot(slot, foundSlot);
                if(newSlot != null && !newSlot.Equals(foundSlot)){
                    foundSlot = newSlot;
                }
            }
        }

        __result = foundSlot;
        return false;
    }

}