using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Game → Gate：推送群组变更到指定 Session。
/// </summary>
public sealed class Game2G_GroupChangedNotifyHandler : Address<Scene, Game2G_GroupChangedNotify>
{
    protected override async FTask Run(Scene scene, Game2G_GroupChangedNotify message)
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

        session.Send(new G2C_GroupChangedNotify
        {
            Op = message.Op,
            GroupId = message.GroupId,
            Group = message.Group,
            User = message.User
        });

        await FTask.CompletedTask;
    }
}
