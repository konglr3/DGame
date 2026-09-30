using System.Collections.Generic;
using DGame;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;
using GameProto;
using GOS.Presence;

namespace GameLogic
{
    /// <summary>
    /// 好友网络管理（Game Roaming）。
    /// </summary>
    public sealed class FriendNetMgr : DataCenterModule<FriendNetMgr>
    {
        public const int StateMutual = 0;
        public const int StateOutgoing = 1;
        public const int StateIncoming = 2;
        public const int StateBlocked = 3;

        private const int FriendChangedUpsert = 1;
        private const int FriendChangedRemove = 2;

        public override void OnInit()
        {
            GameClient.Instance.RegisterMsgHandler(OuterOpcode.G2C_FriendChangedNotify, OnFriendChangedNotify);
            GameEvent.AddEventListener(ILoginUI_Event.OnLoginGateSuccess, OnLoginGateSuccess);
        }

        public override void OnRoleLogout()
        {
            FriendDataMgr.Instance.Clear();
            var presence = GameClient.Instance.Scene?.GetComponent<PresenceComponent>();
            presence?.Clear();
        }

        private void OnLoginGateSuccess()
        {
            GameClient.Instance.Scene?.EnsurePresence();
            RefreshAll().Coroutine();
        }

        private void OnFriendChangedNotify(IMessage message)
        {
            if (message is not G2C_FriendChangedNotify notify || notify.Friend == null)
            {
                return;
            }

            if (notify.Op == FriendChangedRemove)
            {
                FriendDataMgr.Instance.Remove(notify.Friend.RoleId);
                UnfollowRoles(new[] { notify.Friend.RoleId }).Coroutine();
                return;
            }

            if (notify.Op == FriendChangedUpsert)
            {
                FriendDataMgr.Instance.Upsert(notify.Friend);
                if (notify.Friend.State == StateMutual)
                {
                    FollowRoles(new[] { notify.Friend.RoleId }).Coroutine();
                }
            }
        }

        public async FTask RefreshAll()
        {
            await ListFriends(StateMutual);
            await ListFriends(StateIncoming);
            await ListFriends(StateOutgoing);
            await ListRecommend();

            var mutualIds = new List<ulong>();
            foreach (var friend in FriendDataMgr.Instance.MutualFriends)
            {
                if (friend != null && friend.RoleId > 0)
                {
                    mutualIds.Add(friend.RoleId);
                }
            }

            await FollowRoles(mutualIds);
        }

        public async FTask<bool> AddFriend(ulong roleId = 0, string roleName = null)
        {
            var response = await GameClient.Instance.Call(new C2Game_AddFriendRequest
            {
                TargetRoleId = roleId,
                TargetRoleName = roleName ?? string.Empty
            });

            if (response is not Game2C_AddFriendResponse addResponse)
            {
                return false;
            }

            if (addResponse.ErrorCode != 0)
            {
                GameModule.UIModule.ShowTipsUI(addResponse.ErrorCode);
                return false;
            }

            if (addResponse.Friend != null)
            {
                FriendDataMgr.Instance.Upsert(addResponse.Friend);
                if (addResponse.Friend.State == StateMutual)
                {
                    await FollowRoles(new[] { addResponse.Friend.RoleId });
                }
            }

            return true;
        }

        public async FTask<bool> DeleteFriend(ulong roleId)
        {
            var response = await GameClient.Instance.Call(new C2Game_DeleteFriendRequest
            {
                TargetRoleId = roleId
            });

            if (response is not Game2C_DeleteFriendResponse deleteResponse)
            {
                return false;
            }

            if (deleteResponse.ErrorCode != 0)
            {
                GameModule.UIModule.ShowTipsUI(deleteResponse.ErrorCode);
                return false;
            }

            FriendDataMgr.Instance.Remove(roleId);
            await UnfollowRoles(new[] { roleId });
            return true;
        }

        public async FTask<bool> BlockFriend(ulong roleId)
        {
            var response = await GameClient.Instance.Call(new C2Game_BlockFriendRequest
            {
                TargetRoleId = roleId
            });

            if (response is not Game2C_BlockFriendResponse blockResponse)
            {
                return false;
            }

            if (blockResponse.ErrorCode != 0)
            {
                GameModule.UIModule.ShowTipsUI(blockResponse.ErrorCode);
                return false;
            }

            if (blockResponse.Friend != null)
            {
                FriendDataMgr.Instance.Upsert(blockResponse.Friend);
            }

            await UnfollowRoles(new[] { roleId });
            return true;
        }

        public async FTask ListFriends(int state, int limit = 100, string cursor = null)
        {
            var response = await GameClient.Instance.Call(new C2Game_ListFriendsRequest
            {
                State = state,
                Limit = limit,
                Cursor = cursor ?? string.Empty
            });

            if (response is not Game2C_ListFriendsResponse listResponse)
            {
                return;
            }

            if (listResponse.ErrorCode != 0)
            {
                GameModule.UIModule.ShowTipsUI(listResponse.ErrorCode);
                return;
            }

            FriendDataMgr.Instance.SyncList(state, listResponse.Friends);
        }

        public async FTask ListRecommend(int limit = 20)
        {
            var response = await GameClient.Instance.Call(new C2Game_ListRecommendFriendsRequest
            {
                Limit = limit
            });

            if (response is not Game2C_ListRecommendFriendsResponse recommendResponse)
            {
                return;
            }

            if (recommendResponse.ErrorCode != 0)
            {
                GameModule.UIModule.ShowTipsUI(recommendResponse.ErrorCode);
                return;
            }

            FriendDataMgr.Instance.SyncRecommend(recommendResponse.Friends);
        }

        private async FTask FollowRoles(IEnumerable<ulong> roleIds)
        {
            var presence = GameClient.Instance.Scene?.EnsurePresence();
            if (presence == null)
            {
                return;
            }

            await presence.Follow(roleIds);
        }

        private async FTask UnfollowRoles(IEnumerable<ulong> roleIds)
        {
            var presence = GameClient.Instance.Scene?.GetComponent<PresenceComponent>();
            if (presence == null)
            {
                return;
            }

            await presence.Unfollow(roleIds);
        }
    }
}
