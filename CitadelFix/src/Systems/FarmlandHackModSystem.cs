using System.Linq;
using CitadelFix.Behaviours;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace CitadelFix;

public class FarmlandHackModSystem : ModSystem
{

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide == EnumAppSide.Server;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        api.RegisterBlockBehaviorClass("CropReinforcement", typeof(CropBlockBehaviourReinforcable));
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        base.AssetsFinalize(api);
        this.AddCropReinforcementBehavior(api);
    }

    private void AddCropReinforcementBehavior(ICoreAPI api)
    {
        foreach (Block block in api.World.Blocks)
        {
            if (!(block.Code == null) && block.Id != 0 && IsCrop(block))
            {
                api.Logger.Notification($"Adding Crop Reinforcement to {block.Code}[{block.Id}]");
                block.BlockBehaviors = block.BlockBehaviors.Append(new CropBlockBehaviourReinforcable(block)).ToArray();
            }
        }
    }

    private bool IsCrop(Block block)
    {
        var hasCropProps = block.CropProps != null;
        var isBlockCrop = block is BlockCrop;
        return hasCropProps || isBlockCrop;
    }
}