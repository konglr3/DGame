using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Chat → Gate：推送聊天消息到指定 Session 或本 Gate 全部在线 Session。
/// </summary>
public sealed class Chat2G_ChatMessageHandler : Address<Scene, Chat2G_ChatMessage>
{
    protected override async FTask Run(Scene scene, Chat2G_ChatMessage message)
    {
        if (scene.SceneType != SceneType.Gate)
        {
            return;
        }

        var chatMessage = new Chat2C_Message
        {
            ChatInfoTree = message.ChatInfoTree
        };

        if (message.SessionRuntimeId != 0)
        {
            if (scene.TryGetEntity<Session>(message.SessionRuntimeId, out var session) &&
                session != null && !session.IsDisposed)
            {
                session.Send(chatMessage);
            }

            await FTask.CompletedTask;
            return;
        }

        var playerManager = scene.GetComponent<PlayerManagerComponent>();
        if (playerManager == null)
        {
            return;
        }

        foreach (var playerData in playerManager.PlayerDataDict.Values)
        {
            if (playerData == null || playerData.IsDisposed || playerData.SessionRuntimeId == 0)
            {
                continue;
            }

            if (!scene.TryGetEntity<Session>(playerData.SessionRuntimeId, out var session) ||
                session == null || session.IsDisposed)
            {
                continue;
            }

            session.Send(chatMessage);
        }

        await FTask.CompletedTask;
    }
}
