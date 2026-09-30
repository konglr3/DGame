using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// Gate 状态与显示组件（会话级关注图）。
/// </summary>
public sealed class PresenceComponent : Entity
{
    /// <summary>
    /// 在线玩家当前状态文案。
    /// </summary>
    public readonly Dictionary<long, string> StatusByRoleId = new Dictionary<long, string>();

    /// <summary>
    /// 目标 -> 关注者集合。
    /// </summary>
    public readonly Dictionary<long, HashSet<long>> FollowerMap = new Dictionary<long, HashSet<long>>();

    /// <summary>
    /// 关注者 -> 目标集合。
    /// </summary>
    public readonly Dictionary<long, HashSet<long>> FollowingMap = new Dictionary<long, HashSet<long>>();
}
