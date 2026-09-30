using Fantasy.Entitas;

namespace Fantasy;

/// <summary>
/// 群组封禁记录。
/// </summary>
public sealed class GroupBan : Entity
{
    public long GroupId { get; set; }

    public long RoleId { get; set; }

    public long UpdateTime { get; set; }
}
