using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Network;
using GOS.Common;

namespace GOS.Chat
{
    /// <summary>
    /// 客户端聊天组件：发送消息、接收回调。
    /// </summary>
    public sealed class ChatComponent : Entity
    {
        /// <summary>
        /// 收到聊天推送时回调（tree, displayText）
        /// </summary>
        public Action<ChatInfoTree, string> OnMessageReceived;
    }

    public static class ChatComponentSystem
    {
        public static ChatComponent EnsureChat(this Scene scene)
        {
            var serializer = scene.GetComponent<SerializerComponent>();
            if (serializer == null)
            {
                scene.AddComponent<SerializerComponent>().Initialize();
            }

            return scene.GetOrAddComponent<ChatComponent>();
        }

        /// <summary>
        /// 发送广播聊天
        /// </summary>
        public static async FTask<Chat2C_SendMessageResponse> SendBroadcast(this ChatComponent self, string content)
        {
            var tree = ChatTreeFactory.Broadcast(self.Scene).AddendTextNode(content);
            return await self.Send(tree);
        }

        /// <summary>
        /// 发送队伍频道聊天（演示频道 Id=1，与服端登录默认加入一致）
        /// </summary>
        public static async FTask<Chat2C_SendMessageResponse> SendTeam(this ChatComponent self, string content, long channelId = 1)
        {
            var tree = ChatTreeFactory.Team(self.Scene).AddendTextNode(content);
            tree.ChatChannelId = channelId;
            return await self.Send(tree);
        }

        /// <summary>
        /// 发送私聊
        /// </summary>
        public static async FTask<Chat2C_SendMessageResponse> SendPrivate(this ChatComponent self, long targetUnitId, string content)
        {
            var tree = ChatTreeFactory.Private(self.Scene).AddendTextNode(content);
            tree.Target.Add(targetUnitId);
            return await self.Send(tree);
        }

        public static async FTask<Chat2C_SendMessageResponse> Send(this ChatComponent self, ChatInfoTree tree)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("ChatComponent.Send fail: session disposed");
                return null;
            }

            return (Chat2C_SendMessageResponse)await session.C2Chat_SendMessageRequest(tree);
        }
    }
}
