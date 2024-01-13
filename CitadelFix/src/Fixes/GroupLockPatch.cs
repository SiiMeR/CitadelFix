using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace CitadelFix.Fixes;

[HarmonyPatch]
public class GroupLockPatch
{


    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModSystemBlockReinforcement), "TryLock")]
    public static bool TryLock(ModSystemBlockReinforcement __instance, BlockPos pos, IPlayer byPlayer, string itemCode, ref bool __result)
    {
        var accessor = ReinforcementSystemAccessor.With(__instance);
        Dictionary<int, BlockReinforcement> reinforcmentsAt = accessor.GetOrCreateReinforcementsAt(pos);
        if (reinforcmentsAt == null)
            return false;
        int localIndex = accessor.ToLocalIndex(pos);
        BlockReinforcement blockReinforcement;
        if (reinforcmentsAt.TryGetValue(localIndex, out blockReinforcement))
        {
            if(blockReinforcement.Locked){
                __result = false;
                return false;
            }

            //CitadelFixModSystem.api.Logger.Notification($"Block PlayerUID: {blockReinforcement.PlayerUID} | Block GroupUID: {blockReinforcement.GroupUid} | Real PlayerUID: {byPlayer.PlayerUID}");
            //CitadelFixModSystem.api.Logger.Notification($"Reinforcement players equal: {byPlayer.PlayerUID.EqualsFast(blockReinforcement.PlayerUID)}");
            if(blockReinforcement.PlayerUID != null && !blockReinforcement.PlayerUID.EqualsFast(byPlayer.PlayerUID)){
                __result = false;
                return false;
            }else if(blockReinforcement.GroupUid != 0 && !(byPlayer.GetGroups().Where(g => g.GroupUid == blockReinforcement.GroupUid).Count() > 0)){
                __result = false;
                return false;
            }

            blockReinforcement.Locked = true;
            blockReinforcement.LockedByItemCode = itemCode;
            accessor.SaveReinforcements(reinforcmentsAt, pos);
            __result = true;
            return false;
        }
        reinforcmentsAt[localIndex] = new BlockReinforcement()
        {
            PlayerUID = byPlayer.PlayerUID,
            LastPlayername = byPlayer.PlayerName,
            Strength = 0,
            Locked = true,
            LockedByItemCode = itemCode
        };
        accessor.SaveReinforcements(reinforcmentsAt, pos);
        return true;
    }
}