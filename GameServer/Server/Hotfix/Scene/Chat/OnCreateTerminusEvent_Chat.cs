using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Event;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// Chat 场景 Terminus 创建/重连事件：关联 ChatUnit 并加入演示频道。
/// </summary>
public sealed class OnCreateTerminusEvent_Chat : AsyncEventSystem<OnCreateTerminus>
{
    protected override async FTask Handler(OnCreateTerminus self)
    {
        var scene = self.Scene;
        var terminus = self.Terminus;

        if (scene.SceneType != SceneType.Chat)
        {
            return;
        }

        if (terminus.RoamingType != RoamingType.ChatRoamingType)
        {
            self.Args?.Dispose();
            return;
        }

        switch (self.Type)
        {
            case CreateTerminusType.Link:
            {
                if (self.Args is not PlayerRoamingArgs args)
                {
                    Log.Error($"[OnCreateTerminusEvent_Chat][Link] Args 类型不匹配 SceneId:{scene.Id}");
                    self.Args?.Dispose();
                    return;
                }

                var chatUnit = await CreateOrReuseChatUnit(scene, args);
                await terminus.LinkTerminusEntity(chatUnit, autoDispose: true);
                JoinDemoChannel(scene, chatUnit);
                args.Dispose();
                Log.Debug($"[OnCreateTerminusEvent_Chat][Link] SceneId:{scene.Id} RoleId:{chatUnit.Id}");
                break;
            }
            case CreateTerminusType.ReLink:
            {
                var chatUnit = terminus.TerminusEntity as ChatUnit;
                if (chatUnit == null)
                {
                    var roleId = self.Args is PlayerRoamingArgs relinkArgs ? relinkArgs.RoleId : terminus.Id;
                    var displayName = self.Args is PlayerRoamingArgs nameArgs ? nameArgs.DisplayName : string.Empty;
                    using var tempArgs = Entity.Create<PlayerRoamingArgs>(scene);
                    tempArgs.RoleId = roleId;
                    tempArgs.DisplayName = displayName ?? string.Empty;
                    chatUnit = await CreateOrReuseChatUnit(scene, tempArgs);
                    await terminus.LinkTerminusEntity(chatUnit, autoDispose: true);
                    JoinDemoChannel(scene, chatUnit);
                    Log.Debug($"[OnCreateTerminusEvent_Chat][ReLink] 重建 ChatUnit SceneId:{scene.Id} RoleId:{chatUnit.Id}");
                }
                else
                {
                    scene.GetComponent<ChatUnitManageComponent>().Add(chatUnit);
                    JoinDemoChannel(scene, chatUnit);
                    Log.Debug($"[OnCreateTerminusEvent_Chat][ReLink] 恢复在线 SceneId:{scene.Id} RoleId:{chatUnit.Id}");
                }

                self.Args?.Dispose();
                break;
            }
            default:
            {
                self.Args?.Dispose();
                break;
            }
        }

        await FTask.CompletedTask;
    }

    private static async FTask<ChatUnit> CreateOrReuseChatUnit(Scene scene, PlayerRoamingArgs args)
    {
        var manage = scene.GetComponent<ChatUnitManageComponent>();
        if (manage.TryGet(args.RoleId, out var exist) && !exist.IsDisposed)
        {
            exist.UserName = args.DisplayName;
            return exist;
        }

        var chatUnit = Entity.Create<ChatUnit>(scene, args.RoleId, true, true);
        chatUnit.UserName = args.DisplayName;
        manage.Add(chatUnit);
        await FTask.CompletedTask;
        return chatUnit;
    }

    private static void JoinDemoChannel(Scene scene, ChatUnit chatUnit)
    {
        var channel = scene.GetComponent<ChatChannelCenterComponent>().Apply(1);
        channel.JoinChannel(chatUnit.Id);
    }
}
