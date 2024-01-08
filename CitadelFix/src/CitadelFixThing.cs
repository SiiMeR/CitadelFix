using Vintagestory.API.Common;
using Vintagestory.Common;

namespace CitadelFix;

public interface CitadelFixThing {
    public static EnumAppSide Side { get; }
    public static ModRunPhase RunPhase { get; }
    public static abstract void Apply(CitadelFixModSystem modSystem, ICoreAPI api);
    public static abstract void Dispose();
}