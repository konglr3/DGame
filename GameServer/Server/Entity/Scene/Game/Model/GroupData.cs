using Fantasy.Entitas;
using System.Collections.Generic;

namespace Fantasy;

/// <summary>
/// 群组持久化实体。
/// </summary>
public sealed class GroupData : Entity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string AvatarUrl { get; set; } = string.Empty;

    public string LangTag { get; set; } = string.Empty;

    public bool Open { get; set; }

    public int MaxCount { get; set; }

    public long CreatorRoleId { get; set; }

    public long CreateTime { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}
