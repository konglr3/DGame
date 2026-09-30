using Fantasy.Async;
using Fantasy.Event;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// Game 场景 Terminus 销毁事件：区分真正下线与传送清理。
/// </summary>
public sealed class OnDisposeTerminusEvent_Game : AsyncEventSystem<OnDisposeTerminus>
{
    protected override async FTask Handler(OnDisposeTerminus self)
    {
        var scene = self.Scene;
        var terminus = self.Terminus;

        if (scene.SceneType != SceneType.Game)
        {
            return;
        }

        if (terminus.RoamingType != RoamingType.GameRoamingType)
        {
            return;
        }

        var playerData = terminus.TerminusEntity as PlayerData;
        if (playerData == null)
        {
            return;
        }

        switch (self.Type)
        {
            case DisposeTerminusType.UnLink:
            {
                // 真正断线：持久化 Game 侧玩家数据。autoDispose=true 时框架会随后销毁实体。
                scene.GetComponent<PresenceComponent>()?.OnPlayerOffline(playerData);
                scene.GetComponent<GamePlayerManageComponent>()?.Remove(playerData.Id);
                await scene.World.Database.Save(playerData);
                Log.Debug($"[OnDisposeTerminusEvent_Game][UnLink] RoleId:{playerData.Id} 已存档");
                return;
            }
            case DisposeTerminusType.Transfer:
            {
                // 传送离开当前 Scene：只做本地清理，不执行下线存档。
                scene.GetComponent<PresenceComponent>()?.OnPlayerOffline(playerData);
                scene.GetComponent<GamePlayerManageComponent>()?.Remove(playerData.Id);
                Log.Debug($"[OnDisposeTerminusEvent_Game][Transfer] RoleId:{playerData.Id}");
                return;
            }
        }

        await FTask.CompletedTask;
    }
}
