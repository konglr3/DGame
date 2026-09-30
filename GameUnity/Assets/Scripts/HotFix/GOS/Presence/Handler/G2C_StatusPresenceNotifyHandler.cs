using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace GOS.Presence
{
    /// <summary>
    /// 接收 Game→Gate→Client 的状态显示推送（Joins/Leaves），并交给 <see cref="PresenceComponent"/> 更新缓存。
    /// </summary>
    public sealed class G2C_StatusPresenceNotifyHandler : Message<G2C_StatusPresenceNotify>
    {
        /// <summary>
        /// 处理推送：若 Scene 上尚未 EnsurePresence，则忽略本次通知。
        /// </summary>
        protected override async FTask Run(Session session, G2C_StatusPresenceNotify message)
        {
            var presence = session.Scene.GetComponent<PresenceComponent>();
            presence?.ApplyNotify(message);
            await FTask.CompletedTask;
        }
    }
}
