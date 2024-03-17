using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace CitadelFix.Behaviours;

public class CropBlockBehaviourReinforcable : BlockBehavior
{
    public CropBlockBehaviourReinforcable(Block block) : base(block)
    {
    }

    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, ref EnumHandling handling)
    {
        if (byPlayer == null)
        {
            base.OnBlockBroken(world, pos, byPlayer, ref handling);
            return;
        }
        ModSystemBlockReinforcement modSystem = world.Api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
        var farmlandPos = pos.DownCopy();
        BlockReinforcement reinforcement = modSystem.GetReinforcment(farmlandPos);
        if (reinforcement == null)
        {
            base.OnBlockBroken(world, pos, byPlayer, ref handling);
            return;
        }
        
        var isBrokenByPlayer = byPlayer.PlayerUID == reinforcement.PlayerUID;
        var isBrokenByPlayerInGroup = reinforcement.GroupUid != 0 &&
                                      byPlayer.GetGroups().Any(g => g.GroupUid == reinforcement.GroupUid);

        if (reinforcement.Strength <= 0 || isBrokenByPlayer || isBrokenByPlayerInGroup)
        {
            base.OnBlockBroken(world, pos, byPlayer, ref handling);
            return;
        }
        
        world.PlaySoundAt(new AssetLocation("sounds/tool/breakreinforced"), farmlandPos.X, farmlandPos.Y, farmlandPos.Z, byPlayer);
        if (byPlayer.HasPrivilege("denybreakreinforced"))
        {
            base.OnBlockBroken(world, pos, byPlayer, ref handling);
            return;
        }
        handling = EnumHandling.PreventDefault;
        modSystem.ConsumeStrength(farmlandPos, 1);
        world.BlockAccessor.MarkBlockDirty(farmlandPos);
        world.BlockAccessor.MarkBlockDirty(pos);
    }

    public override void OnBlockExploded(IWorldAccessor world, BlockPos pos, BlockPos explosionCenter, EnumBlastType blastType, ref EnumHandling handling)
    {
        ModSystemBlockReinforcement modSystem = world.Api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
        var farmlandPos = pos.DownCopy();
        BlockReinforcement reinforcment = modSystem.GetReinforcment(farmlandPos);
        if (reinforcment != null && reinforcment.Strength > 0)
        {
            world.BlockAccessor.MarkBlockDirty(farmlandPos);
            world.BlockAccessor.MarkBlockDirty(pos);
            handling = EnumHandling.PreventDefault;
        }
        else
        {
            base.OnBlockExploded(world, pos, explosionCenter, blastType, ref handling);
        }
    }

    public override float GetMiningSpeedModifier(
      IWorldAccessor world,
      BlockPos pos,
      IPlayer byPlayer)
    {
        BlockReinforcement reinforcment = world.Api.ModLoader.GetModSystem<ModSystemBlockReinforcement>().GetReinforcment(pos.DownCopy());
        return reinforcment != null && reinforcment.Strength > 0 && byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative ? 0.6f : 1f;
    }

    public override string GetPlacedBlockInfo(IWorldAccessor world, BlockPos pos, IPlayer forPlayer)
    {
        ModSystemBlockReinforcement modSystem = world.Api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
        if (modSystem == null)
        {
            return null;
        }

        var farmlandPos = pos.Down();
        if (world.BlockAccessor.GetBlock(farmlandPos) is not BlockFarmland farmland)
        {
            return null;
        }

        if (!modSystem.IsReinforced(pos))
        {
            return null;
        }

        var reinforcement = modSystem.GetReinforcment(pos);
        StringBuilder builder = new StringBuilder();
        if (reinforcement.GroupUid != 0)
        {
            builder.AppendLine(Lang.Get("Has been reinforced by group {0}.", reinforcement.LastGroupname));
        }
        else
        {
            builder.AppendLine(Lang.Get("Has been reinforced by {0}.", reinforcement.LastPlayername));
        }
        builder.AppendLine(Lang.Get("Strength: {0}", reinforcement.Strength));

        return builder.ToString();
    }
}