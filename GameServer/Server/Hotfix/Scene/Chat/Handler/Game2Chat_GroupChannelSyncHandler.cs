using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

/// <summary>
/// Game → Chat：同步群组聊天频道成员。
/// </summary>
public sealed class Game2Chat_GroupChannelSyncHandler : Address<Scene, Game2Chat_GroupChannelSync>
{
    protected override async FTask Run(Scene scene, Game2Chat_GroupChannelSync message)
    {
        if (scene.SceneType != SceneType.Chat)
        {
            return;
        }

        switch (message.Op)
        {
            case GroupChannelSyncOp.Join:
            {
                var channel = ChatChannelCenterHelper.Apply(scene, message.GroupId);
                channel.JoinChannel(message.RoleId);
                break;
            }
            case GroupChannelSyncOp.Leave:
            {
                if (ChatChannelCenterHelper.TryGet(scene, message.GroupId, out var channel))
                {
                    channel.ExitChannel(message.RoleId);
                    if (channel.IsDisposed)
                    {
                        scene.GetComponent<ChatChannelCenterComponent>().Channels.Remove(message.GroupId);
                    }
                }

                break;
            }
            case GroupChannelSyncOp.Disband:
            {
                ChatChannelCenterHelper.Disband(scene, message.GroupId);
                break;
            }
        }

        await FTask.CompletedTask;
    }
}
