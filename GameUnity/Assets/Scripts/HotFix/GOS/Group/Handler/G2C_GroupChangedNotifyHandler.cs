using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace GOS.Group
{
    /// <summary>
    /// 接收群组变更推送并落地到 <see cref="GroupComponent"/>。
    /// </summary>
    public sealed class G2C_GroupChangedNotifyHandler : Message<G2C_GroupChangedNotify>
    {
        protected override async FTask Run(Session session, G2C_GroupChangedNotify message)
        {
            var group = session.Scene.GetComponent<GroupComponent>();
            group?.ApplyNotify(message);
            await FTask.CompletedTask;
        }
    }
}
