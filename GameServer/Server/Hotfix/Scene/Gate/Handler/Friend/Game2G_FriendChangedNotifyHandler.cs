using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Game → Gate：推送好友关系变更到指定 Session。
/// </summary>
public sealed class Game2G_FriendChangedNotifyHandler : Address<Scene, Game2G_FriendChangedNotify>
{
    protected override async FTask Run(Scene scene, Game2G_FriendChangedNotify message)
    {
        if (scene.SceneType != SceneType.Gate)
        {
            return;
        }

        if (!scene.TryGetEntity<Session>(message.SessionRuntimeId, out var session) ||
            session == null || session.IsDisposed)
        {
            return;
        }

        session.Send(new G2C_FriendChangedNotify
        {
            Op = message.Op,
            Friend = message.Friend
        });

        await FTask.CompletedTask;
    }
}
