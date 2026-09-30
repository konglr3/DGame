using Fantasy.Async;
using Fantasy.Event;
using Fantasy.Network.Roaming;

namespace Fantasy;

/// <summary>
/// Chat 场景 Terminus 销毁事件：从在线表移除 ChatUnit。
/// </summary>
public sealed class OnDisposeTerminusEvent_Chat : AsyncEventSystem<OnDisposeTerminus>
{
    protected override async FTask Handler(OnDisposeTerminus self)
    {
        var scene = self.Scene;
        var terminus = self.Terminus;

        if (scene.SceneType != SceneType.Chat)
        {
            return;
        }

        if (terminus.RoamingType != RoamingType.ChatRoamingType)
        {
            return;
        }

        var chatUnit = terminus.TerminusEntity as ChatUnit;
        if (chatUnit == null)
        {
            return;
        }

        switch (self.Type)
        {
            case DisposeTerminusType.UnLink:
            {
                // autoDispose=true 时框架随后会 Dispose ChatUnit；这里先从管理表摘掉。
                scene.GetComponent<ChatUnitManageComponent>().Remove(chatUnit.Id, isDispose: false);
                Log.Debug($"[OnDisposeTerminusEvent_Chat][UnLink] RoleId:{chatUnit.Id}");
                break;
            }
            case DisposeTerminusType.Transfer:
            {
                scene.GetComponent<ChatUnitManageComponent>().Remove(chatUnit.Id, isDispose: false);
                Log.Debug($"[OnDisposeTerminusEvent_Chat][Transfer] RoleId:{chatUnit.Id}");
                break;
            }
        }

        await FTask.CompletedTask;
    }
}
