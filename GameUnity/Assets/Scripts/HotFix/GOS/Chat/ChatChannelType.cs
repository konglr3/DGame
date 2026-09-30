using System;

namespace GOS.Chat
{
    /// <summary>
    /// 聊天频道类型
    /// </summary>
    [Flags]
    public enum ChatChannelType
    {
        None                   = 0,
        World                  = 1 << 1,
        Private                = 1 << 2,
        System                 = 1 << 3,
        Broadcast              = 1 << 4,
        Notice                 = 1 << 5,
        Team                   = 1 << 6,
        Near                   = 1 << 7,
        CurrentMap             = 1 << 8,
        Group                  = 1 << 9,

        All                    = World | Private | System | Broadcast | Notice | Team | Near | Group,
        Display                = World | Private | System | Broadcast | Notice | Team | Near | CurrentMap | Group
    }

    /// <summary>
    /// 聊天节点类型
    /// </summary>
    public enum ChatNodeType
    {
        None                   = 0,
        Position               = 1,
        OpenUI                 = 2,
        Link                   = 3,
        Item                   = 4,
        Text                   = 5,
        Image                  = 6,
    }

    /// <summary>
    /// 聊天节点事件类型（与服端保持一致）
    /// </summary>
    public enum ChatNodeEvent
    {
        None = 0,
        OpenUI = 1,
        OpenLink = 2,
        UseItem = 3,
        Position = 4,
    }
}
