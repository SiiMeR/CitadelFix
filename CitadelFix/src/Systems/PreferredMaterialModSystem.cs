using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace CitadelFix;

public class PreferredMaterialHack : ModSystem
{
    private IChatCommand chatCommand;
    internal ICoreServerAPI sapi;

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        this.sapi = api;
        chatCommand = api.ChatCommands.Create("setpreferredmaterial")
            .WithDescription("Sets the preferred material for reinforcement to whatever is in your main hand.")
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(this.Handle);
        chatCommand.Validate();
    }

    internal OnCommandDelegate Handle => (cmd) =>
    {
        var player = cmd.Caller.Player;

        if (player == null)
        {
            return TextCommandResult.Error("You must be in-game to use this command.");
        }

        var activeSlot = player.InventoryManager.ActiveHotbarSlot;
        var data = sapi.PlayerData.GetPlayerDataByUid(player.PlayerUID);

        if (activeSlot == null || activeSlot.Empty)
        {
            if(data.CustomPlayerData.GetValueOrDefault("CitadelFixPreferredMaterial", null) == null){
                return TextCommandResult.Success("Preferred material was reset.");
            }
            return TextCommandResult.Error("You must have a reinforcement material in hand to set your preferred material.");
        }

        if (activeSlot.Itemstack.ItemAttributes["reinforcementStrength"].AsInt(0) == 0)
        {
            return TextCommandResult.Error("Your need a valid reinforcement material to set your preferred material.");
        }

        data.CustomPlayerData["CitadelFixPreferredMaterial"] = activeSlot.Itemstack.Collectible.Code.ToString();
        return TextCommandResult.Success("Set preferred material to " + activeSlot.Itemstack.Collectible.Code.ToString());
    };
}