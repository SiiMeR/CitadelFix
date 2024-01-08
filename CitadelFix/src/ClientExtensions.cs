using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace CitadelFix;

public static class ClientExtensions
{

    public static void AddHotkey(this ICoreClientAPI api, string key, string name, GlKeys glKey, HotkeyType hotkeyType, ActionConsumable<KeyCombination> handler){
        api.Input.RegisterHotKey(key, name, glKey, hotkeyType);
        api.Input.SetHotKeyHandler(key, handler);
    }

}