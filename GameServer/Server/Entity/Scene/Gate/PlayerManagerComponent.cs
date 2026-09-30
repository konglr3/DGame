using Fantasy.Entitas;

namespace Fantasy;

public class PlayerManagerComponent : Entity
{
    /// <summary>
    /// 玩家数据缓存字典 (AccountID, ServerID) => PlayerData
    /// </summary>
    public readonly Dictionary<(long AccountID, int ServerID), PlayerData> PlayerDataDict = new Dictionary<(long AccountID, int ServerID), PlayerData>();

    /// <summary>
    /// 在线玩家 RoleId => PlayerData
    /// </summary>
    public readonly Dictionary<long, PlayerData> PlayerDataByRoleId = new Dictionary<long, PlayerData>();
}