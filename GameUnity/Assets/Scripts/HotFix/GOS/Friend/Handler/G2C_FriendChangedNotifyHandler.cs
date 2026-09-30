using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;

namespace GOS.Friend
{
    /// <summary>
    /// 接收 Game→Gate→Client 的好友关系变更推送，并交给 <see cref="FriendComponent"/> 落地缓存。
    /// </summary>
    public sealed class G2C_FriendChangedNotifyHandler : Message<G2C_FriendChangedNotify>
    {
        /// <summary>
        /// 处理推送：若 Scene 上尚未 EnsureFriend，则忽略本次通知。
        /// </summary>
        protected override async FTask Run(Session session, G2C_FriendChangedNotify message)
        {
            var friend = session.Scene.GetComponent<FriendComponent>();
            friend?.ApplyNotify(message);
            await FTask.CompletedTask;
        }
    }
}
