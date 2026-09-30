using DGame;

namespace GameLogic
{
    /// <summary>
    /// 好友逻辑事件。
    /// </summary>
    [EventInterface(EEventGroup.GroupUI)]
    public interface IFriendLogicEvent
    {
        /// <summary>
        /// 好友列表数据变化。
        /// </summary>
        void OnFriendListChange();
    }
}
