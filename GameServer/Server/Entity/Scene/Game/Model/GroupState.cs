namespace Fantasy;

/// <summary>
/// 群组成员状态，对齐 Nakama Groups state。
/// </summary>
public static class GroupMemberState
{
    public const int Superadmin = 0;
    public const int Admin = 1;
    public const int Member = 2;
    public const int JoinRequest = 3;
}

/// <summary>
/// 群组变更推送操作。
/// </summary>
public static class GroupChangedOp
{
    public const int UpsertGroup = 1;
    public const int RemoveGroup = 2;
    public const int UpsertMember = 3;
    public const int RemoveMember = 4;
}

/// <summary>
/// 群聊频道同步操作。
/// </summary>
public static class GroupChannelSyncOp
{
    public const int Join = 1;
    public const int Leave = 2;
    public const int Disband = 3;
}

/// <summary>
/// 群组数量限制。
/// </summary>
public static class GroupLimit
{
    public const int JoinMax = 10;
    public const int MemberMax = 100;
    public const int ApplyMax = 50;
    public const int ListMax = 50;
    public const int DefaultMaxCount = 100;
}
