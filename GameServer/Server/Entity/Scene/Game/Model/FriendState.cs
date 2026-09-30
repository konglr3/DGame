namespace Fantasy;

/// <summary>
/// 好友关系状态，对齐 Nakama Friends state。
/// </summary>
public static class FriendState
{
    public const int Mutual = 0;
    public const int Outgoing = 1;
    public const int Incoming = 2;
    public const int Blocked = 3;
}

/// <summary>
/// 好友变更推送操作。
/// </summary>
public static class FriendChangedOp
{
    public const int Upsert = 1;
    public const int Remove = 2;
}

/// <summary>
/// 好友数量限制。
/// </summary>
public static class FriendLimit
{
    public const int MutualMax = 100;
    public const int ApplyMax = 50;
    public const int RecommendMax = 20;
    public const int ListMax = 100;
}
