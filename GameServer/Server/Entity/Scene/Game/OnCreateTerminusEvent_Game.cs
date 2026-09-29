using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Event;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// Game 场景 Terminus 创建/重连事件：关联并初始化玩家数据。
/// </summary>
public sealed class OnCreateTerminusEvent_Game : AsyncEventSystem<OnCreateTerminus>
{
    protected override async FTask Handler(OnCreateTerminus self)
    {
        var scene = self.Scene;
        var terminus = self.Terminus;

        if (scene.SceneType != SceneType.Game)
        {
            return;
        }

        if (terminus.RoamingType != RoamingType.GameRoamingType)
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
                    Log.Error($"[OnCreateTerminusEvent_Game][Link] Args 类型不匹配 SceneId:{scene.Id}");
                    self.Args?.Dispose();
                    return;
                }

                var playerData = await CreateOrLoadPlayerData(scene, args.RoleId, args);
                await terminus.LinkTerminusEntity(playerData, autoDispose: true);
                args.Dispose();
                Log.Debug($"[OnCreateTerminusEvent_Game][Link] SceneId:{scene.Id} RoleId:{playerData.Id}");
                break;
            }
            case CreateTerminusType.ReLink:
            {
                var playerData = terminus.TerminusEntity as PlayerData;
                if (playerData == null)
                {
                    // 延迟移除窗口内实体已被清理时，重新加载并关联
                    var roleId = self.Args is PlayerRoamingArgs relinkArgs ? relinkArgs.RoleId : terminus.Id;
                    playerData = await CreateOrLoadPlayerData(scene, roleId, self.Args as PlayerRoamingArgs);
                    await terminus.LinkTerminusEntity(playerData, autoDispose: true);
                    Log.Debug($"[OnCreateTerminusEvent_Game][ReLink] 重建 PlayerData SceneId:{scene.Id} RoleId:{playerData.Id}");
                }
                else
                {
                    Log.Debug($"[OnCreateTerminusEvent_Game][ReLink] 恢复在线 SceneId:{scene.Id} RoleId:{playerData.Id}");
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

    private static async FTask<PlayerData> CreateOrLoadPlayerData(Scene scene, long roleId, PlayerRoamingArgs? args)
    {
        var playerData = await scene.World.Database.First<PlayerData>(d => d.Id == roleId, true);
        if (playerData == null)
        {
            playerData = Entity.Create<PlayerData>(scene, roleId, true, true);
        }

        if (args != null)
        {
            if (!string.IsNullOrEmpty(args.DisplayName))
            {
                playerData.RoleName = args.DisplayName;
            }

            if (args.Level > 0)
            {
                playerData.Level = args.Level;
            }
        }

        return playerData;
    }
}
