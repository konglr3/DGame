using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// 其他服务器代发聊天消息到 ChatUnit。
/// </summary>
public sealed class Other2Chat_ChatMessageHandler : Address<ChatUnit, Other2Chat_ChatMessage>
{
    protected override async FTask Run(ChatUnit chatUnit, Other2Chat_ChatMessage message)
    {
        if (chatUnit == null || chatUnit.IsDisposed)
        {
            return;
        }

        var result = ChatSceneHelper.Distribution(chatUnit, message.ChatInfoTree, false);
        if (result != 0)
        {
            Log.Warning($"Other2Chat_ChatMessageHandler: Distribution failed, result: {result}");
        }

        await FTask.CompletedTask;
    }
}
