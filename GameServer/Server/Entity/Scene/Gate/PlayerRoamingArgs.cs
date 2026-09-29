using Fantasy.Entitas;
using MemoryPack;

namespace Fantasy;

[MemoryPackable]
public sealed partial class PlayerRoamingArgs : Entity
{
    public uint Level;
    public long RoleId;
    public string DisplayName;

    public override void Dispose()
    {
        Level = 0;
        RoleId = 0;
        DisplayName = null;
        base.Dispose();
    }
}