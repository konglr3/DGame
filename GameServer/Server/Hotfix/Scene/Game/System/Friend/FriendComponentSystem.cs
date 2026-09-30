using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Helper;
using System;
using System.Collections.Generic;

namespace Fantasy;

/// <summary>
/// Game Scene 好友关系逻辑。
/// </summary>
public static class FriendComponentSystem
{
    public static async FTask<(uint errorCode, CSFriendInfo? friend)> AddFriend(this FriendComponent self, PlayerData owner, ulong targetRoleId, string targetRoleName)
    {
        var target = await self.ResolveTarget(targetRoleId, targetRoleName);
        if (target == null)
        {
            return (ErrorCode.FRIEND_TARGET_NOT_FOUND, null);
        }

        if (target.Id == owner.Id)
        {
            return (ErrorCode.FRIEND_CANNOT_SELF, null);
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.FriendOperateLock, GetLockKey(owner.Id, target.Id), "Friend.Add", 10000))
        {
            var ownerEdge = await self.LoadEdge(owner.Id, target.Id);
            var targetEdge = await self.LoadEdge(target.Id, owner.Id);
            if (ownerEdge?.State == FriendState.Blocked || targetEdge?.State == FriendState.Blocked)
            {
                return (ErrorCode.FRIEND_BLOCKED, null);
            }

            if (ownerEdge?.State == FriendState.Mutual)
            {
                return (ErrorCode.FRIEND_ALREADY_FRIEND, await self.ToFriendInfo(owner.Id, ownerEdge));
            }

            if (ownerEdge?.State == FriendState.Outgoing)
            {
                return (ErrorCode.FRIEND_ALREADY_PENDING, await self.ToFriendInfo(owner.Id, ownerEdge));
            }

            if (ownerEdge?.State == FriendState.Incoming)
            {
                var mutualCount = await self.CountState(owner.Id, FriendState.Mutual);
                var targetMutualCount = await self.CountState(target.Id, FriendState.Mutual);
                if (mutualCount >= FriendLimit.MutualMax || targetMutualCount >= FriendLimit.MutualMax)
                {
                    return (ErrorCode.FRIEND_LIMIT, null);
                }

                await self.UpsertEdge(owner.Id, target.Id, FriendState.Mutual, ownerEdge);
                await self.UpsertEdge(target.Id, owner.Id, FriendState.Mutual, targetEdge);
                var info = CreateFriendInfo(FriendState.Mutual, target);
                self.Notify(owner.Id, FriendChangedOp.Upsert, info);
                self.Notify(target.Id, FriendChangedOp.Upsert, CreateFriendInfo(FriendState.Mutual, owner));
                return (ErrorCode.SUCCESS, info);
            }

            var outgoingCount = await self.CountState(owner.Id, FriendState.Outgoing);
            var incomingCount = await self.CountState(target.Id, FriendState.Incoming);
            if (outgoingCount >= FriendLimit.ApplyMax || incomingCount >= FriendLimit.ApplyMax)
            {
                return (ErrorCode.FRIEND_LIMIT, null);
            }

            await self.UpsertEdge(owner.Id, target.Id, FriendState.Outgoing, ownerEdge);
            await self.UpsertEdge(target.Id, owner.Id, FriendState.Incoming, targetEdge);
            var applyInfo = CreateFriendInfo(FriendState.Outgoing, target);
            self.Notify(owner.Id, FriendChangedOp.Upsert, applyInfo);
            self.Notify(target.Id, FriendChangedOp.Upsert, CreateFriendInfo(FriendState.Incoming, owner));
            return (ErrorCode.SUCCESS, applyInfo);
        }
    }

    public static async FTask<uint> DeleteFriend(this FriendComponent self, PlayerData owner, ulong targetRoleId, string targetRoleName)
    {
        var target = await self.ResolveTarget(targetRoleId, targetRoleName);
        if (target == null)
        {
            return ErrorCode.FRIEND_TARGET_NOT_FOUND;
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.FriendOperateLock, GetLockKey(owner.Id, target.Id), "Friend.Delete", 10000))
        {
            var ownerEdge = await self.LoadEdge(owner.Id, target.Id);
            if (ownerEdge == null)
            {
                return ErrorCode.FRIEND_NOT_FOUND;
            }

            await self.RemoveEdge(owner.Id, target.Id);
            await self.RemoveEdge(target.Id, owner.Id);
            self.Notify(owner.Id, FriendChangedOp.Remove, new CSFriendInfo { RoleId = (ulong)target.Id, State = ownerEdge.State });
            self.Notify(target.Id, FriendChangedOp.Remove, new CSFriendInfo { RoleId = (ulong)owner.Id, State = ownerEdge.State });
            return ErrorCode.SUCCESS;
        }
    }

    public static async FTask<(uint errorCode, CSFriendInfo? friend)> BlockFriend(this FriendComponent self, PlayerData owner, ulong targetRoleId, string targetRoleName)
    {
        var target = await self.ResolveTarget(targetRoleId, targetRoleName);
        if (target == null)
        {
            return (ErrorCode.FRIEND_TARGET_NOT_FOUND, null);
        }

        if (target.Id == owner.Id)
        {
            return (ErrorCode.FRIEND_CANNOT_SELF, null);
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.FriendOperateLock, GetLockKey(owner.Id, target.Id), "Friend.Block", 10000))
        {
            var ownerEdge = await self.LoadEdge(owner.Id, target.Id);
            await self.UpsertEdge(owner.Id, target.Id, FriendState.Blocked, ownerEdge);
            await self.RemoveEdge(target.Id, owner.Id);
            var info = CreateFriendInfo(FriendState.Blocked, target);
            self.Notify(owner.Id, FriendChangedOp.Upsert, info);
            self.Notify(target.Id, FriendChangedOp.Remove, new CSFriendInfo { RoleId = (ulong)owner.Id });
            return (ErrorCode.SUCCESS, info);
        }
    }

    public static async FTask<(List<CSFriendInfo> friends, string cursor)> ListFriends(this FriendComponent self, long ownerRoleId, int state, int limit, string cursor)
    {
        if (limit <= 0)
        {
            limit = 20;
        }

        if (limit > FriendLimit.ListMax)
        {
            limit = FriendLimit.ListMax;
        }

        var pageIndex = 1;
        if (!string.IsNullOrEmpty(cursor) && int.TryParse(cursor, out var parsed) && parsed > 0)
        {
            pageIndex = parsed;
        }

        var database = self.Scene.World.Database;
        var edges = await database.QueryByPageOrderBy<FriendEdge>(
            d => d.OwnerRoleId == ownerRoleId && d.State == state,
            pageIndex,
            limit,
            d => d.UpdateTime,
            false,
            true);

        var friends = new List<CSFriendInfo>();
        foreach (var edge in edges)
        {
            friends.Add(await self.ToFriendInfo(ownerRoleId, edge));
        }

        var nextCursor = edges.Count >= limit ? (pageIndex + 1).ToString() : string.Empty;
        return (friends, nextCursor);
    }

    public static async FTask<List<CSFriendInfo>> ListRecommend(this FriendComponent self, PlayerData owner, int limit)
    {
        if (limit <= 0 || limit > FriendLimit.RecommendMax)
        {
            limit = FriendLimit.RecommendMax;
        }

        var result = new List<CSFriendInfo>();
        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (playerManage == null)
        {
            return result;
        }

        foreach (var player in playerManage.Players.Values)
        {
            if (result.Count >= limit)
            {
                break;
            }

            if (player == null || player.IsDisposed || player.Id == owner.Id)
            {
                continue;
            }

            var edge = await self.LoadEdge(owner.Id, player.Id);
            if (edge != null)
            {
                continue;
            }

            result.Add(CreateFriendInfo(-1, player));
        }

        return result;
    }

    private static async FTask<PlayerData?> ResolveTarget(this FriendComponent self, ulong targetRoleId, string targetRoleName)
    {
        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (targetRoleId > 0)
        {
            if (playerManage != null && playerManage.TryGet((long)targetRoleId, out var online))
            {
                return online;
            }

            return await self.Scene.World.Database.Query<PlayerData>((long)targetRoleId, true);
        }

        if (string.IsNullOrWhiteSpace(targetRoleName))
        {
            return null;
        }

        if (playerManage != null)
        {
            foreach (var player in playerManage.Players.Values)
            {
                if (player.RoleName == targetRoleName)
                {
                    return player;
                }
            }
        }

        return await self.Scene.World.Database.First<PlayerData>(d => d.RoleName == targetRoleName, true);
    }

    private static async FTask<FriendEdge?> LoadEdge(this FriendComponent self, long ownerRoleId, long targetRoleId)
        => await self.Scene.World.Database.First<FriendEdge>(d => d.OwnerRoleId == ownerRoleId && d.TargetRoleId == targetRoleId, true);

    private static async FTask<long> CountState(this FriendComponent self, long ownerRoleId, int state)
        => await self.Scene.World.Database.Count<FriendEdge>(d => d.OwnerRoleId == ownerRoleId && d.State == state);

    private static async FTask UpsertEdge(this FriendComponent self, long ownerRoleId, long targetRoleId, int state, FriendEdge? exist)
    {
        var database = self.Scene.World.Database;
        var now = TimeHelper.Now;
        if (exist == null)
        {
            exist = Entity.Create<FriendEdge>(self.Scene, true, true);
            exist.OwnerRoleId = ownerRoleId;
            exist.TargetRoleId = targetRoleId;
            exist.State = state;
            exist.UpdateTime = now;
            await database.Insert(exist);
            return;
        }

        exist.State = state;
        exist.UpdateTime = now;
        await database.Save(exist);
    }

    private static async FTask RemoveEdge(this FriendComponent self, long ownerRoleId, long targetRoleId)
    {
        var edge = await self.LoadEdge(ownerRoleId, targetRoleId);
        if (edge == null)
        {
            return;
        }

        await self.Scene.World.Database.Remove<FriendEdge>(edge.Id);
    }

    private static async FTask<CSFriendInfo> ToFriendInfo(this FriendComponent self, long ownerRoleId, FriendEdge edge)
    {
        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (playerManage != null && playerManage.TryGet(edge.TargetRoleId, out var online))
        {
            return CreateFriendInfo(edge.State, online);
        }

        var player = await self.Scene.World.Database.Query<PlayerData>(edge.TargetRoleId, true);
        return CreateFriendInfo(edge.State, player);
    }

    private static CSFriendInfo CreateFriendInfo(int state, PlayerData? player)
    {
        var info = new CSFriendInfo
        {
            State = state
        };

        if (player != null)
        {
            info.RoleId = (ulong)player.Id;
            info.RoleName = player.RoleName ?? string.Empty;
            info.HeadID = player.HeadID;
            info.Level = player.Level;
        }

        return info;
    }

    private static void Notify(this FriendComponent self, long roleId, int op, CSFriendInfo friend)
    {
        var presence = self.Scene.GetComponent<PresenceComponent>();
        var notify = G2C_FriendChangedNotify.Create(false);
        notify.Op = op;
        notify.Friend = friend;
        presence?.SendToRole(roleId, notify);
    }

    private static long GetLockKey(long a, long b)
    {
        var min = Math.Min(a, b);
        var max = Math.Max(a, b);
        return HashCodeHelper.ComputeHash64($"{min}_{max}");
    }
}
