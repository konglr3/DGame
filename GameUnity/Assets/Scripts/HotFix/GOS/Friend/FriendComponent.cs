using System;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Network;
using GOS.Presence;

namespace GOS.Friend
{
    /// <summary>
    /// 好友关系状态常量，与服务端 <c>FriendState</c> / Nakama Friends state 对齐。
    /// </summary>
    public static class FriendState
    {
        /// <summary>共同好友（双向已确认）。</summary>
        public const int Mutual = 0;

        /// <summary>我发出的好友申请，等待对方接受。</summary>
        public const int Outgoing = 1;

        /// <summary>对方发给我的好友申请，等待我接受。</summary>
        public const int Incoming = 2;

        /// <summary>我屏蔽了对方。</summary>
        public const int Blocked = 3;
    }

    /// <summary>
    /// 好友关系变更推送操作码，对应 <see cref="G2C_FriendChangedNotify.Op"/>。
    /// </summary>
    public static class FriendChangedOp
    {
        /// <summary>新增或更新一条好友边（申请 / 成为好友 / 屏蔽等）。</summary>
        public const int Upsert = 1;

        /// <summary>删除一条好友边（删好友 / 拒绝申请等）。</summary>
        public const int Remove = 2;
    }

    /// <summary>
    /// 客户端好友 ECS 组件：缓存各状态好友列表，并通过 Game Roaming 调用服务端好友接口。
    /// <para>与 <see cref="PresenceComponent"/> 解耦：本组件只维护关系图，在线态由 Presence 负责。</para>
    /// </summary>
    public sealed class FriendComponent : Entity
    {
        /// <summary>共同好友列表（State = Mutual）。</summary>
        public readonly List<CSFriendInfo> MutualFriends = new List<CSFriendInfo>();

        /// <summary>收到的好友申请列表（State = Incoming）。</summary>
        public readonly List<CSFriendInfo> IncomingRequests = new List<CSFriendInfo>();

        /// <summary>已发出、待对方处理的申请列表（State = Outgoing）。</summary>
        public readonly List<CSFriendInfo> OutgoingRequests = new List<CSFriendInfo>();

        /// <summary>我屏蔽的用户列表（State = Blocked）。</summary>
        public readonly List<CSFriendInfo> BlockedUsers = new List<CSFriendInfo>();

        /// <summary>服务端返回的推荐好友候选（通常不含已有关系边）。</summary>
        public readonly List<CSFriendInfo> RecommendFriends = new List<CSFriendInfo>();

        /// <summary>
        /// 本地好友缓存发生变化时回调（拉表成功、主动操作成功、收到推送后触发）。
        /// </summary>
        public Action OnFriendChanged;
    }

    /// <summary>
    /// <see cref="FriendComponent"/> 扩展逻辑：Ensure、CRUD/列表 RPC、推送落地与本地缓存维护。
    /// </summary>
    public static class FriendComponentSystem
    {
        /// <summary>
        /// 确保 Scene 上挂载好友组件；同时保证 Presence 组件已就绪（共同好友需 Follow）。
        /// </summary>
        /// <param name="scene">客户端 Fantasy Scene。</param>
        /// <returns>好友组件实例。</returns>
        public static FriendComponent EnsureFriend(this Scene scene)
        {
            scene.EnsurePresence();
            return scene.GetOrAddComponent<FriendComponent>();
        }

        /// <summary>
        /// 拉取共同好友、收/发申请、推荐列表，并对共同好友执行 Presence.Follow。
        /// </summary>
        /// <param name="self">好友组件。</param>
        public static async FTask RefreshAll(this FriendComponent self)
        {
            // 按状态分段拉取，写入本地缓存
            await self.ListFriends(FriendState.Mutual);
            await self.ListFriends(FriendState.Incoming);
            await self.ListFriends(FriendState.Outgoing);
            await self.ListRecommend();

            // 共同好友需要订阅在线态：收集 RoleId 后交给 Presence
            var mutualIds = new List<ulong>();
            foreach (var friend in self.MutualFriends)
            {
                if (friend != null && friend.RoleId > 0)
                {
                    mutualIds.Add(friend.RoleId);
                }
            }

            if (mutualIds.Count > 0)
            {
                var presence = self.Scene.EnsurePresence();
                await presence.Follow(mutualIds);
            }

            self.OnFriendChanged?.Invoke();
        }

        /// <summary>
        /// 添加好友：发申请，或对 Incoming 再添加一次以接受申请（Nakama 语义）。
        /// </summary>
        /// <param name="self">好友组件。</param>
        /// <param name="roleId">目标角色 Id；与 roleName 二选一，优先 Id。</param>
        /// <param name="roleName">目标角色名；Id 为 0 时按名查找。</param>
        /// <returns>服务端响应；Session 无效时返回 null。</returns>
        public static async FTask<Game2C_AddFriendResponse> AddFriend(this FriendComponent self, ulong roleId = 0, string roleName = null)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("FriendComponent.AddFriend fail: session disposed");
                return null;
            }

            var response = await session.C2Game_AddFriendRequest(roleId, roleName ?? string.Empty);
            if (response == null)
            {
                return null;
            }

            // 成功后同步本地缓存；若已变为共同好友则立即 Follow
            if (response.ErrorCode == 0 && response.Friend != null)
            {
                self.UpsertLocal(response.Friend);
                if (response.Friend.State == FriendState.Mutual)
                {
                    await self.Scene.EnsurePresence().Follow(new[] { response.Friend.RoleId });
                }

                self.OnFriendChanged?.Invoke();
            }

            return response;
        }

        /// <summary>
        /// 删除好友或拒绝申请（按服务端边状态删除双向关系）。
        /// </summary>
        /// <param name="self">好友组件。</param>
        /// <param name="roleId">目标角色 Id。</param>
        /// <returns>服务端响应；Session 无效时返回 null。</returns>
        public static async FTask<Game2C_DeleteFriendResponse> DeleteFriend(this FriendComponent self, ulong roleId)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("FriendComponent.DeleteFriend fail: session disposed");
                return null;
            }

            var response = await session.C2Game_DeleteFriendRequest(roleId, string.Empty);
            if (response == null)
            {
                return null;
            }

            if (response.ErrorCode == 0)
            {
                self.RemoveLocal(roleId);
                // 关系解除后不再关注其在线态
                await self.Scene.EnsurePresence().Unfollow(new[] { roleId });
                self.OnFriendChanged?.Invoke();
            }

            return response;
        }

        /// <summary>
        /// 屏蔽用户：写入 Blocked 边，并取消对其 Presence 关注。
        /// </summary>
        /// <param name="self">好友组件。</param>
        /// <param name="roleId">目标角色 Id。</param>
        /// <returns>服务端响应；Session 无效时返回 null。</returns>
        public static async FTask<Game2C_BlockFriendResponse> BlockFriend(this FriendComponent self, ulong roleId)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("FriendComponent.BlockFriend fail: session disposed");
                return null;
            }

            var response = await session.C2Game_BlockFriendRequest(roleId, string.Empty);
            if (response == null)
            {
                return null;
            }

            if (response.ErrorCode == 0)
            {
                if (response.Friend != null)
                {
                    self.UpsertLocal(response.Friend);
                }
                else
                {
                    self.RemoveLocal(roleId);
                }

                await self.Scene.EnsurePresence().Unfollow(new[] { roleId });
                self.OnFriendChanged?.Invoke();
            }

            return response;
        }

        /// <summary>
        /// 按关系状态分页列出好友，并覆盖对应本地列表。
        /// </summary>
        /// <param name="self">好友组件。</param>
        /// <param name="state">关系状态，见 <see cref="FriendState"/>。</param>
        /// <param name="limit">单页数量上限。</param>
        /// <param name="cursor">分页游标；首页传 null/空串。</param>
        /// <returns>服务端响应；Session 无效时返回 null。</returns>
        public static async FTask<Game2C_ListFriendsResponse> ListFriends(this FriendComponent self, int state, int limit = 100, string cursor = null)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("FriendComponent.ListFriends fail: session disposed");
                return null;
            }

            var response = await session.C2Game_ListFriendsRequest(state, limit, cursor ?? string.Empty);
            if (response == null)
            {
                return null;
            }

            if (response.ErrorCode == 0)
            {
                self.SyncList(state, response.Friends);
                self.OnFriendChanged?.Invoke();
            }

            return response;
        }

        /// <summary>
        /// 拉取推荐好友列表并写入 <see cref="FriendComponent.RecommendFriends"/>。
        /// </summary>
        /// <param name="self">好友组件。</param>
        /// <param name="limit">推荐数量上限。</param>
        /// <returns>服务端响应；Session 无效时返回 null。</returns>
        public static async FTask<Game2C_ListRecommendFriendsResponse> ListRecommend(this FriendComponent self, int limit = 20)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error("FriendComponent.ListRecommend fail: session disposed");
                return null;
            }

            var response = await session.C2Game_ListRecommendFriendsRequest(limit);
            if (response == null)
            {
                return null;
            }

            if (response.ErrorCode == 0)
            {
                self.RecommendFriends.Clear();
                if (response.Friends != null)
                {
                    self.RecommendFriends.AddRange(response.Friends);
                }

                self.OnFriendChanged?.Invoke();
            }

            return response;
        }

        /// <summary>
        /// 处理服务端好友变更推送：更新本地缓存，并联动 Presence Follow/Unfollow。
        /// </summary>
        /// <param name="self">好友组件。</param>
        /// <param name="notify">好友变更通知。</param>
        public static void ApplyNotify(this FriendComponent self, G2C_FriendChangedNotify notify)
        {
            if (notify?.Friend == null)
            {
                return;
            }

            if (notify.Op == FriendChangedOp.Remove)
            {
                self.RemoveLocal(notify.Friend.RoleId);
                self.Scene.EnsurePresence().Unfollow(new[] { notify.Friend.RoleId }).Coroutine();
            }
            else if (notify.Op == FriendChangedOp.Upsert)
            {
                self.UpsertLocal(notify.Friend);
                // 对方同意申请后，本端会收到 Mutual Upsert，需补 Follow
                if (notify.Friend.State == FriendState.Mutual)
                {
                    self.Scene.EnsurePresence().Follow(new[] { notify.Friend.RoleId }).Coroutine();
                }
            }

            self.OnFriendChanged?.Invoke();
        }

        /// <summary>
        /// 登出或 Scene 重置时清空本地好友缓存与回调。
        /// </summary>
        /// <param name="self">好友组件。</param>
        public static void Clear(this FriendComponent self)
        {
            self.MutualFriends.Clear();
            self.IncomingRequests.Clear();
            self.OutgoingRequests.Clear();
            self.BlockedUsers.Clear();
            self.RecommendFriends.Clear();
            self.OnFriendChanged = null;
        }

        /// <summary>
        /// 用服务端列表整表覆盖指定状态的本地缓存。
        /// </summary>
        private static void SyncList(this FriendComponent self, int state, List<CSFriendInfo> friends)
        {
            var target = self.GetListByState(state);
            if (target == null)
            {
                return;
            }

            target.Clear();
            if (friends != null)
            {
                target.AddRange(friends);
            }
        }

        /// <summary>
        /// 先按 RoleId 从各列表移除旧边，再插入到对应 State 列表（本地 Upsert）。
        /// </summary>
        private static void UpsertLocal(this FriendComponent self, CSFriendInfo friend)
        {
            if (friend == null || friend.RoleId == 0)
            {
                return;
            }

            self.RemoveLocal(friend.RoleId);
            self.GetListByState(friend.State)?.Add(Clone(friend));
        }

        /// <summary>
        /// 从所有本地列表中移除指定 RoleId（含推荐列表）。
        /// </summary>
        private static void RemoveLocal(this FriendComponent self, ulong roleId)
        {
            RemoveFrom(self.MutualFriends, roleId);
            RemoveFrom(self.IncomingRequests, roleId);
            RemoveFrom(self.OutgoingRequests, roleId);
            RemoveFrom(self.BlockedUsers, roleId);
            RemoveFrom(self.RecommendFriends, roleId);
        }

        /// <summary>
        /// 从单个列表按 RoleId 倒序删除匹配项。
        /// </summary>
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

        /// <summary>
        /// 按状态返回对应本地列表引用；未知状态返回 null。
        /// </summary>
        private static List<CSFriendInfo> GetListByState(this FriendComponent self, int state)
        {
            switch (state)
            {
                case FriendState.Mutual:
                    return self.MutualFriends;
                case FriendState.Outgoing:
                    return self.OutgoingRequests;
                case FriendState.Incoming:
                    return self.IncomingRequests;
                case FriendState.Blocked:
                    return self.BlockedUsers;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 浅拷贝好友条目，避免与协议对象池/后续复用互相污染。
        /// </summary>
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
    }
}
