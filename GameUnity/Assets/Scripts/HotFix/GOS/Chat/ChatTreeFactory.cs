using Fantasy;
using GOS.Chat;

namespace GOS.Chat
{
    /// <summary>
    /// 创建聊天树的总入口
    /// </summary>
    public static class ChatTreeFactory
    {
        public static ChatInfoTree World(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.World,
            };
        }

        public static ChatInfoTree Private(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.Private,
            };
        }

        public static ChatInfoTree System(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.System,
            };
        }

        public static ChatInfoTree Broadcast(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.Broadcast,
            };
        }

        public static ChatInfoTree Notice(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.Notice,
            };
        }

        public static ChatInfoTree Team(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.Team,
            };
        }

        public static ChatInfoTree Group(Scene scene, long groupId)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.Group,
                ChatChannelId = groupId,
            };
        }

        public static ChatInfoTree Near(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.Near,
            };
        }

        public static ChatInfoTree CurrentMap(Scene scene)
        {
            return new ChatInfoTree
            {
                Scene = scene,
                ChatChannelType = (int)ChatChannelType.CurrentMap,
            };
        }
    }
}
