using System.Text;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace GOS.Chat
{
    /// <summary>
    /// 接收服务器推送的聊天消息。
    /// </summary>
    public sealed class Chat2C_MessageHandler : Message<Chat2C_Message>
    {
        protected override async FTask Run(Session session, Chat2C_Message message)
        {
            var text = ChatTreeParser.ParseToText(session.Scene, message.ChatInfoTree);
            Log.Info($"收到聊天信息：{text}");

            var chatComponent = session.Scene.GetComponent<ChatComponent>();
            chatComponent?.OnMessageReceived?.Invoke(message.ChatInfoTree, text);

            await FTask.CompletedTask;
        }
    }

    public static class ChatTreeParser
    {
        public static string ParseToText(Scene scene, ChatInfoTree tree)
        {
            if (tree?.Node == null || tree.Node.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var chatInfoNode in tree.Node)
            {
                if (!string.IsNullOrEmpty(chatInfoNode.Content))
                {
                    sb.Append(chatInfoNode.Content);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 触发节点事件（供 UI 点击回调使用）
        /// </summary>
        public static void HandleNodeEvent(Scene scene, ChatInfoNode node)
        {
            ChatNodeEventHelper.Handler(scene, node);
        }
    }
}
