using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// 单向好友关系边，按 OwnerRoleId 分边持久化。
/// </summary>
public sealed class FriendEdge : Entity
{
    public long OwnerRoleId { get; set; }

    public long TargetRoleId { get; set; }

    public int State { get; set; }

    public long UpdateTime { get; set; }
}
