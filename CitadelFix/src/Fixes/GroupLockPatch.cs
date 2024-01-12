using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CitadelFix.Fixes;

[HarmonyPatch]
public class GroupLockPatch
{


    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemBlockReinforcement), "TryLock")]
    public static bool TryLock(ModSystemBlockReinforcement __instance, BlockPos pos, IPlayer byPlayer, string itemCode, ref bool __result)
    {
        Dictionary<int, BlockReinforcement> reinforcmentsAt = getOrCreateReinforcementsAt(__instance, pos);
        if (reinforcmentsAt == null)
            return false;
        int localIndex = toLocalIndex(__instance, pos);
        BlockReinforcement blockReinforcement;
        if (reinforcmentsAt.TryGetValue(localIndex, out blockReinforcement))
        {
            if (!(byPlayer.GetGroups().Where(g => g.GroupUid == blockReinforcement.GroupUid).Count() > 0) || blockReinforcement.Locked){
                return false;
            }
            blockReinforcement.Locked = true;
            blockReinforcement.LockedByItemCode = itemCode;
            saveReinforcements(__instance, reinforcmentsAt, pos);
            return true;
        }
        reinforcmentsAt[localIndex] = new BlockReinforcement()
        {
            PlayerUID = byPlayer.PlayerUID,
            LastPlayername = byPlayer.PlayerName,
            Strength = 0,
            Locked = true,
            LockedByItemCode = itemCode
        };
        saveReinforcements(__instance, reinforcmentsAt, pos);
        return true;
    }

    public static Dictionary<int, BlockReinforcement> getOrCreateReinforcementsAt(ModSystemBlockReinforcement _instance, BlockPos pos)
    {
        return _instance.GetType().GetMethod("getOrCreateReinforcmentsAt",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
        ).Invoke(_instance, new object[] { pos }) as Dictionary<int, BlockReinforcement>;
    }

    public static int toLocalIndex(ModSystemBlockReinforcement _instance, BlockPos pos)
    {
        return (int)_instance.GetType().GetMethod("toLocalIndex", 
            BindingFlags.NonPublic | BindingFlags.Instance, 
            null, 
            new Type[] { typeof(BlockPos) }, 
            null
        ).Invoke(_instance, new object[]{pos});
    }

    public static void saveReinforcements(ModSystemBlockReinforcement _instance, Dictionary<int, BlockReinforcement> reif, BlockPos pos)
    {
        _instance.GetType().GetMethod("saveReinforcments",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
        ).Invoke(_instance, new object[] { reif, pos });
    }


}