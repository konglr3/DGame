using System;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Network;

namespace GOS.Presence
{
    /// <summary>
    /// 单个被关注用户的本地在线/状态快照。
    /// </summary>
    public sealed class PresenceCache
    {
        /// <summary>目标角色 Id。</summary>
        public ulong RoleId;

        /// <summary>目标角色名（来自服务端 Presence 快照）。</summary>
        public string RoleName = string.Empty;

        /// <summary>目标当前状态文案（如“在主界面”）；离线时清空。</summary>
        public string StatusText = string.Empty;

        /// <summary>是否在线；Joins 置 true，Leaves 置 false。</summary>
        public bool Online;
    }

    /// <summary>
    /// 客户端状态与显示 ECS 组件（对齐 Nakama Status Presence）。
    /// <para>关注关系为会话级：重连后需业务侧重新 <see cref="PresenceComponentSystem.Follow"/>。</para>
    /// <para>不依赖好友关系；好友系统仅作为常见调用方。</para>
    /// </summary>
    public sealed class PresenceComponent : Entity
    {
        /// <summary>
        /// 本端已向服务端声明关注的 RoleId 集合（用于去重 Follow / 本地 Unfollow）。
        /// </summary>
        public readonly HashSet<ulong> Following = new HashSet<ulong>();

        /// <summary>
        /// 已关注用户的在线态缓存：Key = RoleId。
        /// </summary>
        public readonly Dictionary<ulong, PresenceCache> Cache = new Dictionary<ulong, PresenceCache>();

        /// <summary>
        /// 关注集合或在线缓存变化时回调（Follow 回包、推送 Joins/Leaves、Unfollow 后触发）。
        /// </summary>
        public Action OnPresenceChanged;

        /// <summary>
        /// 本端最近一次提交的状态文案（仅本地镜像，权威在服务端内存）。
        /// </summary>
        public string MyStatusText = string.Empty;
    }

    /// <summary>
    /// <see cref="PresenceComponent"/> 扩展逻辑：关注、取消关注、更新状态、处理推送。
    /// </summary>
    public static class PresenceComponentSystem
    {
        /// <summary>
        /// 确保 Scene 上挂载 Presence 组件。
        /// </summary>
        /// <param name="scene">客户端 Fantasy Scene。</param>
        /// <returns>Presence 组件实例。</returns>
        public static PresenceComponent EnsurePresence(this Scene scene)
        {
            return scene.GetOrAddComponent<PresenceComponent>();
        }

        /// <summary>
        /// 关注一批用户：过滤无效/已关注 Id，向服务端订阅，并用当前在线快照填充 <see cref="PresenceComponent.Cache"/>。
        /// </summary>
        /// <param name="self">Presence 组件。</param>
        /// <param name="roleIds">要关注的角色 Id 列表。</param>
        public static async FTask Follow(this PresenceComponent self, IEnumerable<ulong> roleIds)
        {
            // 仅提交尚未在 Following 中的有效 Id，避免重复 RPC
            var ids = new List<ulong>();
            foreach (var roleId in roleIds)
            {
                if (roleId == 0 || !self.Following.Add(roleId))
                {
                    continue;
                }

                ids.Add(roleId);
            }

            if (ids.Count == 0)
            {
                return;
            }

            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                return;
            }

            var response = (Game2C_FollowUsersResponse)await session.Call(new C2Game_FollowUsersRequest
            {
                RoleIds = ids
            });

            if (response == null || response.ErrorCode != 0)
            {
                return;
            }

            // Response.Presences：关注瞬间已在线用户的初始快照
            if (response.Presences != null)
            {
                foreach (var presence in response.Presences)
                {
                    self.ApplyJoin(presence);
                }
            }

            self.OnPresenceChanged?.Invoke();
        }

        /// <summary>
        /// 取消关注：本地移除 Following/Cache，并通知服务端停止推送。
        /// </summary>
        /// <param name="self">Presence 组件。</param>
        /// <param name="roleIds">要取消关注的角色 Id 列表。</param>
        public static async FTask Unfollow(this PresenceComponent self, IEnumerable<ulong> roleIds)
        {
            var ids = new List<ulong>();
            foreach (var roleId in roleIds)
            {
                if (self.Following.Remove(roleId))
                {
                    ids.Add(roleId);
                    self.Cache.Remove(roleId);
                }
            }

            if (ids.Count == 0)
            {
                return;
            }

            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                return;
            }

            await session.Call(new C2Game_UnfollowUsersRequest { RoleIds = ids });
            self.OnPresenceChanged?.Invoke();
        }

        /// <summary>
        /// 更新自身状态文案，并同步到服务端；服务端会向所有关注我的人推送 Joins。
        /// </summary>
        /// <param name="self">Presence 组件。</param>
        /// <param name="statusText">新的状态文案，允许空串。</param>
        public static async FTask UpdateStatus(this PresenceComponent self, string statusText)
        {
            self.MyStatusText = statusText ?? string.Empty;
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                return;
            }

            await session.Call(new C2Game_UpdateStatusRequest { StatusText = self.MyStatusText });
        }

        /// <summary>
        /// 处理服务端状态显示推送：Joins 表示上线或状态刷新，Leaves 表示下线。
        /// </summary>
        /// <param name="self">Presence 组件。</param>
        /// <param name="notify">状态显示通知。</param>
        public static void ApplyNotify(this PresenceComponent self, G2C_StatusPresenceNotify notify)
        {
            if (notify?.Joins != null)
            {
                foreach (var join in notify.Joins)
                {
                    self.ApplyJoin(join);
                }
            }

            if (notify?.Leaves != null)
            {
                foreach (var leave in notify.Leaves)
                {
                    // 保留缓存条目但标记离线，便于 UI 显示“离线”；Unfollow 才会删掉 Key
                    if (self.Cache.TryGetValue(leave.RoleId, out var cache))
                    {
                        cache.Online = false;
                        cache.StatusText = string.Empty;
                    }
                }
            }

            self.OnPresenceChanged?.Invoke();
        }

        /// <summary>
        /// 查询本地是否已有某角色的 Presence 缓存。
        /// </summary>
        /// <param name="self">Presence 组件。</param>
        /// <param name="roleId">角色 Id。</param>
        /// <param name="cache">输出缓存；不存在时为 null。</param>
        /// <returns>找到缓存返回 true。</returns>
        public static bool TryGet(this PresenceComponent self, ulong roleId, out PresenceCache cache)
            => self.Cache.TryGetValue(roleId, out cache);

        /// <summary>
        /// 登出或 Session 重置时清空关注集合、在线缓存与回调。
        /// </summary>
        /// <param name="self">Presence 组件。</param>
        public static void Clear(this PresenceComponent self)
        {
            self.Following.Clear();
            self.Cache.Clear();
            self.MyStatusText = string.Empty;
            self.OnPresenceChanged = null;
        }

        /// <summary>
        /// 将一条 Joins Presence 写入或刷新本地缓存（标记在线）。
        /// </summary>
        private static void ApplyJoin(this PresenceComponent self, CSStatusPresence presence)
        {
            if (presence == null || presence.RoleId == 0)
            {
                return;
            }

            if (!self.Cache.TryGetValue(presence.RoleId, out var cache))
            {
                cache = new PresenceCache { RoleId = presence.RoleId };
                self.Cache[presence.RoleId] = cache;
            }

            cache.RoleName = presence.RoleName ?? string.Empty;
            cache.StatusText = presence.StatusText ?? string.Empty;
            cache.Online = true;
        }
    }
}
