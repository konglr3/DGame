using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// 群组成员边。
/// </summary>
public sealed class GroupMember : Entity
{
    public long GroupId { get; set; }

    public long RoleId { get; set; }

    public int State { get; set; }

    public long UpdateTime { get; set; }
}
