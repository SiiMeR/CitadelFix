using System;
using System.Collections.Generic;
using System.Reflection;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CitadelFix;

public class ReinforcementSystemAccessor
{

    private ModSystemBlockReinforcement modSystem;
    private ReinforcementSystemAccessor(ModSystemBlockReinforcement instance){
        modSystem = instance;
    }

    public Dictionary<int, BlockReinforcement> GetOrCreateReinforcementsAt(BlockPos pos){
        return getOrCreateReinforcementsAt(modSystem, pos);
    }

    public int ToLocalIndex(BlockPos pos){
        return toLocalIndex(modSystem, pos);
    }

    public void SaveReinforcements(Dictionary<int, BlockReinforcement> reif, BlockPos pos){
        saveReinforcements(modSystem, reif, pos);
    }

    public static ReinforcementSystemAccessor With(ModSystemBlockReinforcement instance){
        return new ReinforcementSystemAccessor(instance);
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
        _instance.GetType().GetMethod("SaveReinforcments",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod
        ).Invoke(_instance, new object[] { reif, pos });
    }
}