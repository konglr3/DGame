using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Game → Gate：推送状态显示变更到指定 Session。
/// </summary>
public sealed class Game2G_StatusPresenceNotifyHandler : Address<Scene, Game2G_StatusPresenceNotify>
{
    protected override async FTask Run(Scene scene, Game2G_StatusPresenceNotify message)
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

        session.Send(new G2C_StatusPresenceNotify
        {
            Joins = message.Joins,
            Leaves = message.Leaves
        });

        await FTask.CompletedTask;
    }
}
