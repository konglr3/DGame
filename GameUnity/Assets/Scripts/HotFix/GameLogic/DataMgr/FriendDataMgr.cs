using System.Collections.Generic;
using DGame;
using Fantasy;
using GameProto;

namespace GameLogic
{
    /// <summary>
    /// 好友数据缓存。
    /// </summary>
    public sealed class FriendDataMgr : DataCenterModule<FriendDataMgr>
    {
        public readonly List<CSFriendInfo> MutualFriends = new List<CSFriendInfo>();
        public readonly List<CSFriendInfo> IncomingRequests = new List<CSFriendInfo>();
        public readonly List<CSFriendInfo> OutgoingRequests = new List<CSFriendInfo>();
        public readonly List<CSFriendInfo> BlockedUsers = new List<CSFriendInfo>();
        public readonly List<CSFriendInfo> RecommendFriends = new List<CSFriendInfo>();

        public override void OnRoleLogout()
        {
            Clear();
        }

        public void Clear()
        {
            MutualFriends.Clear();
            IncomingRequests.Clear();
            OutgoingRequests.Clear();
            BlockedUsers.Clear();
            RecommendFriends.Clear();
            RefreshRedDot();
            GameEvent.Get<IFriendLogicEvent>().OnFriendListChange();
        }

        public void SyncList(int state, List<CSFriendInfo> friends)
        {
            var target = GetListByState(state);
            if (target == null)
            {
                return;
            }

            target.Clear();
            if (friends != null)
            {
                target.AddRange(friends);
            }

            RefreshRedDot();
            GameEvent.Get<IFriendLogicEvent>().OnFriendListChange();
        }

        public void SyncRecommend(List<CSFriendInfo> friends)
        {
            RecommendFriends.Clear();
            if (friends != null)
            {
                RecommendFriends.AddRange(friends);
            }

            RefreshRedDot();
            GameEvent.Get<IFriendLogicEvent>().OnFriendListChange();
        }

        public void Upsert(CSFriendInfo friend)
        {
            if (friend == null || friend.RoleId == 0)
            {
                return;
            }

            RemoveByRoleId(friend.RoleId);
            GetListByState(friend.State)?.Add(Clone(friend));
            RefreshRedDot();
            GameEvent.Get<IFriendLogicEvent>().OnFriendListChange();
        }

        public void Remove(ulong roleId)
        {
            if (roleId == 0)
            {
                return;
            }

            RemoveByRoleId(roleId);
            RefreshRedDot();
            GameEvent.Get<IFriendLogicEvent>().OnFriendListChange();
        }

        private void RemoveByRoleId(ulong roleId)
        {
            RemoveFrom(MutualFriends, roleId);
            RemoveFrom(IncomingRequests, roleId);
            RemoveFrom(OutgoingRequests, roleId);
            RemoveFrom(BlockedUsers, roleId);
            RemoveFrom(RecommendFriends, roleId);
        }

        private static void RemoveFrom(List<CSFriendInfo> list, ulong roleId)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] != null && list[i].RoleId == roleId)
                {
                    list.RemoveAt(i);
                }
            }
        }

        private List<CSFriendInfo> GetListByState(int state)
        {
            switch (state)
            {
                case 0:
                    return MutualFriends;
                case 1:
                    return OutgoingRequests;
                case 2:
                    return IncomingRequests;
                case 3:
                    return BlockedUsers;
                default:
                    return null;
            }
        }

        private static CSFriendInfo Clone(CSFriendInfo source)
        {
            return new CSFriendInfo
            {
                RoleId = source.RoleId,
                RoleName = source.RoleName,
                HeadID = source.HeadID,
                Level = source.Level,
                State = source.State
            };
        }

        private void RefreshRedDot()
        {
            GameModule.RedDotModule.SetValue(RedDotPathDefine.Social.Friends.Request, IncomingRequests.Count);
            GameModule.RedDotModule.SetValue(RedDotPathDefine.Social.Friends.Recommend, RecommendFriends.Count > 0 ? 1 : 0);
        }
    }
}
