using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Game 场景在线玩家管理。
/// </summary>
public sealed class GamePlayerManageComponent : Entity
{
    public readonly Dictionary<long, PlayerData> Players = new Dictionary<long, PlayerData>();
}
