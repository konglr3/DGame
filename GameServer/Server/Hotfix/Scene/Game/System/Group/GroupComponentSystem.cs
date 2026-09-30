using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Helper;
using Fantasy.Platform.Net;
using System;
using System.Collections.Generic;

namespace Fantasy;

/// <summary>
/// Game Scene 群组业务逻辑。
/// </summary>
public static class GroupComponentSystem
{
    public static async FTask<(uint errorCode, CSGroupInfo? group)> CreateGroup(
        this GroupComponent self,
        PlayerData owner,
        string name,
        string description,
        string avatarUrl,
        string langTag,
        bool open,
        int maxCount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (ErrorCode.GROUP_INVALID_PARAMETER, null);
        }

        maxCount = ClampMaxCount(maxCount);
        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, owner.Id, "Group.Create", 10000))
        {
            var joined = await self.CountJoined(owner.Id);
            if (joined >= GroupLimit.JoinMax)
            {
                return (ErrorCode.GROUP_LIMIT, null);
            }

            var group = Entity.Create<GroupData>(self.Scene, true, true);
            group.Name = name.Trim();
            group.Description = description ?? string.Empty;
            group.AvatarUrl = avatarUrl ?? string.Empty;
            group.LangTag = langTag ?? string.Empty;
            group.Open = open;
            group.MaxCount = maxCount;
            group.CreatorRoleId = owner.Id;
            group.CreateTime = TimeHelper.Now;
            group.Metadata = new Dictionary<string, string>();
            await self.Scene.World.Database.Insert(group);

            await self.UpsertMember(group.Id, owner.Id, GroupMemberState.Superadmin, null);
            self.SyncChatChannel(group.Id, owner.Id, GroupChannelSyncOp.Join);

            var info = await self.ToGroupInfo(group);
            self.NotifyGroup(owner.Id, GroupChangedOp.UpsertGroup, info, null);
            return (ErrorCode.SUCCESS, info);
        }
    }

    public static async FTask<(uint errorCode, CSGroupInfo? group)> UpdateGroup(
        this GroupComponent self,
        PlayerData owner,
        ulong groupId,
        string name,
        string description,
        string avatarUrl,
        string langTag,
        bool hasOpen,
        bool open,
        int maxCount)
    {
        if (groupId == 0)
        {
            return (ErrorCode.GROUP_INVALID_PARAMETER, null);
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, (long)groupId, "Group.Update", 10000))
        {
            var group = await self.LoadGroup((long)groupId);
            if (group == null)
            {
                return (ErrorCode.GROUP_NOT_FOUND, null);
            }

            var actor = await self.LoadMember(group.Id, owner.Id);
            if (actor == null || actor.State > GroupMemberState.Admin)
            {
                return (ErrorCode.GROUP_NO_PERMISSION, null);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                group.Name = name.Trim();
            }

            if (description != null)
            {
                group.Description = description;
            }

            if (avatarUrl != null)
            {
                group.AvatarUrl = avatarUrl;
            }

            if (langTag != null)
            {
                group.LangTag = langTag;
            }

            if (hasOpen)
            {
                group.Open = open;
            }

            if (maxCount > 0)
            {
                group.MaxCount = ClampMaxCount(maxCount);
            }

            await self.Scene.World.Database.Save(group);
            var info = await self.ToGroupInfo(group);
            await self.NotifyAllMembers(group.Id, GroupChangedOp.UpsertGroup, info, null, includePending: false);
            return (ErrorCode.SUCCESS, info);
        }
    }

    public static async FTask<(uint errorCode, CSGroupInfo? group)> UpdateGroupMetadata(
        this GroupComponent self,
        PlayerData owner,
        ulong groupId,
        Dictionary<string, string>? metadata)
    {
        if (groupId == 0)
        {
            return (ErrorCode.GROUP_INVALID_PARAMETER, null);
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, (long)groupId, "Group.Meta", 10000))
        {
            var group = await self.LoadGroup((long)groupId);
            if (group == null)
            {
                return (ErrorCode.GROUP_NOT_FOUND, null);
            }

            var actor = await self.LoadMember(group.Id, owner.Id);
            if (actor == null || actor.State != GroupMemberState.Superadmin)
            {
                return (ErrorCode.GROUP_NO_PERMISSION, null);
            }

            group.Metadata = metadata != null
                ? new Dictionary<string, string>(metadata)
                : new Dictionary<string, string>();
            await self.Scene.World.Database.Save(group);

            var info = await self.ToGroupInfo(group);
            await self.NotifyAllMembers(group.Id, GroupChangedOp.UpsertGroup, info, null, includePending: false);
            return (ErrorCode.SUCCESS, info);
        }
    }

    public static async FTask<uint> DeleteGroup(this GroupComponent self, PlayerData owner, ulong groupId)
    {
        if (groupId == 0)
        {
            return ErrorCode.GROUP_INVALID_PARAMETER;
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, (long)groupId, "Group.Delete", 10000))
        {
            var group = await self.LoadGroup((long)groupId);
            if (group == null)
            {
                return ErrorCode.GROUP_NOT_FOUND;
            }

            var actor = await self.LoadMember(group.Id, owner.Id);
            if (actor == null || actor.State != GroupMemberState.Superadmin)
            {
                return ErrorCode.GROUP_NO_PERMISSION;
            }

            var members = await self.QueryMembers(group.Id, stateFilter: null, 1, GroupLimit.MemberMax + GroupLimit.ApplyMax);
            foreach (var member in members)
            {
                await self.Scene.World.Database.Remove<GroupMember>(member.Id);
                self.NotifyGroup(member.RoleId, GroupChangedOp.RemoveGroup, null, null, group.Id);
            }

            var bans = await self.QueryBans(group.Id);
            foreach (var ban in bans)
            {
                await self.Scene.World.Database.Remove<GroupBan>(ban.Id);
            }

            await self.Scene.World.Database.Remove<GroupData>(group.Id);
            self.SyncChatChannel(group.Id, 0, GroupChannelSyncOp.Disband);
            return ErrorCode.SUCCESS;
        }
    }

    public static async FTask<(List<CSGroupInfo> groups, string cursor)> ListGroups(
        this GroupComponent self,
        string nameFilter,
        int limit,
        string cursor)
    {
        limit = ClampListLimit(limit);
        var pageIndex = ParseCursor(cursor);
        var database = self.Scene.World.Database;
        List<GroupData> rows;
        if (string.IsNullOrWhiteSpace(nameFilter))
        {
            rows = await database.QueryByPageOrderBy<GroupData>(
                d => true,
                pageIndex,
                limit,
                d => d.CreateTime,
                false,
                true);
        }
        else
        {
            var filter = nameFilter.Trim();
            rows = await database.QueryByPageOrderBy<GroupData>(
                d => d.Name.Contains(filter),
                pageIndex,
                limit,
                d => d.CreateTime,
                false,
                true);
        }

        var groups = new List<CSGroupInfo>();
        foreach (var row in rows)
        {
            groups.Add(await self.ToGroupInfo(row));
        }

        var next = rows.Count >= limit ? (pageIndex + 1).ToString() : string.Empty;
        return (groups, next);
    }

    public static async FTask<List<CSUserGroup>> ListUserGroups(this GroupComponent self, long roleId)
    {
        var members = await self.Scene.World.Database.QueryByPageOrderBy<GroupMember>(
            d => d.RoleId == roleId && d.State <= GroupMemberState.Member,
            1,
            GroupLimit.JoinMax,
            d => d.UpdateTime,
            false,
            true);

        var result = new List<CSUserGroup>();
        foreach (var member in members)
        {
            var group = await self.LoadGroup(member.GroupId);
            if (group == null)
            {
                continue;
            }

            result.Add(new CSUserGroup
            {
                Group = await self.ToGroupInfo(group),
                State = member.State
            });
        }

        return result;
    }

    public static async FTask<(uint errorCode, CSGroupInfo? group, int state)> JoinGroup(
        this GroupComponent self,
        PlayerData owner,
        ulong groupId)
    {
        if (groupId == 0)
        {
            return (ErrorCode.GROUP_INVALID_PARAMETER, null, 0);
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, (long)groupId, "Group.Join", 10000))
        {
            var group = await self.LoadGroup((long)groupId);
            if (group == null)
            {
                return (ErrorCode.GROUP_NOT_FOUND, null, 0);
            }

            if (await self.IsBanned(group.Id, owner.Id))
            {
                return (ErrorCode.GROUP_BANNED, null, 0);
            }

            var exist = await self.LoadMember(group.Id, owner.Id);
            if (exist != null)
            {
                if (exist.State == GroupMemberState.JoinRequest)
                {
                    return (ErrorCode.GROUP_ALREADY_PENDING, await self.ToGroupInfo(group), exist.State);
                }

                return (ErrorCode.GROUP_ALREADY_MEMBER, await self.ToGroupInfo(group), exist.State);
            }

            var joined = await self.CountJoined(owner.Id);
            if (joined >= GroupLimit.JoinMax)
            {
                return (ErrorCode.GROUP_LIMIT, null, 0);
            }

            if (group.Open)
            {
                var memberCount = await self.CountActiveMembers(group.Id);
                if (memberCount >= group.MaxCount)
                {
                    return (ErrorCode.GROUP_FULL, null, 0);
                }

                await self.UpsertMember(group.Id, owner.Id, GroupMemberState.Member, null);
                self.SyncChatChannel(group.Id, owner.Id, GroupChannelSyncOp.Join);
                var info = await self.ToGroupInfo(group);
                var user = await self.ToGroupUser(owner.Id, GroupMemberState.Member);
                await self.NotifyAllMembers(group.Id, GroupChangedOp.UpsertMember, info, user, includePending: false);
                self.NotifyGroup(owner.Id, GroupChangedOp.UpsertGroup, info, null);
                return (ErrorCode.SUCCESS, info, GroupMemberState.Member);
            }

            var pending = await self.CountState(group.Id, GroupMemberState.JoinRequest);
            if (pending >= GroupLimit.ApplyMax)
            {
                return (ErrorCode.GROUP_LIMIT, null, 0);
            }

            await self.UpsertMember(group.Id, owner.Id, GroupMemberState.JoinRequest, null);
            var applyInfo = await self.ToGroupInfo(group);
            var applyUser = await self.ToGroupUser(owner.Id, GroupMemberState.JoinRequest);
            await self.NotifyAdmins(group.Id, GroupChangedOp.UpsertMember, applyInfo, applyUser);
            self.NotifyGroup(owner.Id, GroupChangedOp.UpsertMember, applyInfo, applyUser);
            return (ErrorCode.SUCCESS, applyInfo, GroupMemberState.JoinRequest);
        }
    }

    public static async FTask<uint> LeaveGroup(this GroupComponent self, PlayerData owner, ulong groupId)
    {
        if (groupId == 0)
        {
            return ErrorCode.GROUP_INVALID_PARAMETER;
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, (long)groupId, "Group.Leave", 10000))
        {
            var group = await self.LoadGroup((long)groupId);
            if (group == null)
            {
                return ErrorCode.GROUP_NOT_FOUND;
            }

            var member = await self.LoadMember(group.Id, owner.Id);
            if (member == null)
            {
                return ErrorCode.GROUP_NOT_MEMBER;
            }

            if (member.State == GroupMemberState.Superadmin)
            {
                var superCount = await self.CountState(group.Id, GroupMemberState.Superadmin);
                if (superCount <= 1)
                {
                    return ErrorCode.GROUP_SOLE_SUPERADMIN;
                }
            }

            var wasActive = member.State <= GroupMemberState.Member;
            await self.Scene.World.Database.Remove<GroupMember>(member.Id);
            if (wasActive)
            {
                self.SyncChatChannel(group.Id, owner.Id, GroupChannelSyncOp.Leave);
            }

            var user = new CSGroupUser { RoleId = (ulong)owner.Id };
            await self.NotifyAllMembers(group.Id, GroupChangedOp.RemoveMember, null, user, includePending: false);
            self.NotifyGroup(owner.Id, GroupChangedOp.RemoveGroup, null, null, group.Id);
            return ErrorCode.SUCCESS;
        }
    }

    public static async FTask<(List<CSGroupUser> users, string cursor)> ListGroupUsers(
        this GroupComponent self,
        ulong groupId,
        int state,
        int limit,
        string cursor)
    {
        if (groupId == 0)
        {
            return (new List<CSGroupUser>(), string.Empty);
        }

        limit = ClampListLimit(limit);
        var pageIndex = ParseCursor(cursor);
        List<GroupMember> members;
        if (state < 0)
        {
            members = await self.Scene.World.Database.QueryByPageOrderBy<GroupMember>(
                d => d.GroupId == (long)groupId,
                pageIndex,
                limit,
                d => d.UpdateTime,
                false,
                true);
        }
        else
        {
            members = await self.Scene.World.Database.QueryByPageOrderBy<GroupMember>(
                d => d.GroupId == (long)groupId && d.State == state,
                pageIndex,
                limit,
                d => d.UpdateTime,
                false,
                true);
        }

        var users = new List<CSGroupUser>();
        foreach (var member in members)
        {
            users.Add(await self.ToGroupUser(member.RoleId, member.State));
        }

        var next = members.Count >= limit ? (pageIndex + 1).ToString() : string.Empty;
        return (users, next);
    }

    public static async FTask<uint> AddGroupUsers(this GroupComponent self, PlayerData owner, ulong groupId, List<ulong> roleIds)
    {
        return await self.MutateMembers(owner, groupId, roleIds, async (group, actor, targetId) =>
        {
            if (actor.State > GroupMemberState.Admin)
            {
                return ErrorCode.GROUP_NO_PERMISSION;
            }

            var target = await self.LoadMember(group.Id, targetId);
            if (target == null || target.State != GroupMemberState.JoinRequest)
            {
                return ErrorCode.GROUP_NOT_MEMBER;
            }

            var memberCount = await self.CountActiveMembers(group.Id);
            if (memberCount >= group.MaxCount)
            {
                return ErrorCode.GROUP_FULL;
            }

            await self.UpsertMember(group.Id, targetId, GroupMemberState.Member, target);
            self.SyncChatChannel(group.Id, targetId, GroupChannelSyncOp.Join);
            var info = await self.ToGroupInfo(group);
            var user = await self.ToGroupUser(targetId, GroupMemberState.Member);
            await self.NotifyAllMembers(group.Id, GroupChangedOp.UpsertMember, info, user, includePending: false);
            self.NotifyGroup(targetId, GroupChangedOp.UpsertGroup, info, null);
            return ErrorCode.SUCCESS;
        });
    }

    public static async FTask<uint> PromoteGroupUsers(this GroupComponent self, PlayerData owner, ulong groupId, List<ulong> roleIds)
    {
        return await self.MutateMembers(owner, groupId, roleIds, async (group, actor, targetId) =>
        {
            var target = await self.LoadMember(group.Id, targetId);
            if (target == null || target.State > GroupMemberState.Member)
            {
                return ErrorCode.GROUP_NOT_MEMBER;
            }

            if (target.State == GroupMemberState.Member)
            {
                if (actor.State > GroupMemberState.Admin)
                {
                    return ErrorCode.GROUP_NO_PERMISSION;
                }

                await self.UpsertMember(group.Id, targetId, GroupMemberState.Admin, target);
            }
            else if (target.State == GroupMemberState.Admin)
            {
                if (actor.State != GroupMemberState.Superadmin)
                {
                    return ErrorCode.GROUP_NO_PERMISSION;
                }

                await self.UpsertMember(group.Id, targetId, GroupMemberState.Superadmin, target);
            }
            else
            {
                return ErrorCode.GROUP_INVALID_PARAMETER;
            }

            var user = await self.ToGroupUser(targetId, target.State);
            await self.NotifyAllMembers(group.Id, GroupChangedOp.UpsertMember, null, user, includePending: false);
            return ErrorCode.SUCCESS;
        });
    }

    public static async FTask<uint> DemoteGroupUsers(this GroupComponent self, PlayerData owner, ulong groupId, List<ulong> roleIds)
    {
        return await self.MutateMembers(owner, groupId, roleIds, async (group, actor, targetId) =>
        {
            var target = await self.LoadMember(group.Id, targetId);
            if (target == null || target.State > GroupMemberState.Admin)
            {
                return ErrorCode.GROUP_NOT_MEMBER;
            }

            if (target.State == GroupMemberState.Admin)
            {
                if (actor.State > GroupMemberState.Admin)
                {
                    return ErrorCode.GROUP_NO_PERMISSION;
                }

                await self.UpsertMember(group.Id, targetId, GroupMemberState.Member, target);
            }
            else if (target.State == GroupMemberState.Superadmin)
            {
                if (actor.State != GroupMemberState.Superadmin || targetId == owner.Id)
                {
                    return ErrorCode.GROUP_NO_PERMISSION;
                }

                await self.UpsertMember(group.Id, targetId, GroupMemberState.Admin, target);
            }
            else
            {
                return ErrorCode.GROUP_INVALID_PARAMETER;
            }

            var user = await self.ToGroupUser(targetId, target.State);
            await self.NotifyAllMembers(group.Id, GroupChangedOp.UpsertMember, null, user, includePending: false);
            return ErrorCode.SUCCESS;
        });
    }

    public static async FTask<uint> KickGroupUsers(this GroupComponent self, PlayerData owner, ulong groupId, List<ulong> roleIds)
    {
        return await self.MutateMembers(owner, groupId, roleIds, async (group, actor, targetId) =>
        {
            if (actor.State > GroupMemberState.Admin)
            {
                return ErrorCode.GROUP_NO_PERMISSION;
            }

            if (targetId == owner.Id)
            {
                return ErrorCode.GROUP_INVALID_PARAMETER;
            }

            var target = await self.LoadMember(group.Id, targetId);
            if (target == null)
            {
                return ErrorCode.GROUP_NOT_MEMBER;
            }

            if (target.State <= actor.State)
            {
                return ErrorCode.GROUP_NO_PERMISSION;
            }

            var wasActive = target.State <= GroupMemberState.Member;
            await self.Scene.World.Database.Remove<GroupMember>(target.Id);
            if (wasActive)
            {
                self.SyncChatChannel(group.Id, targetId, GroupChannelSyncOp.Leave);
            }

            var user = new CSGroupUser { RoleId = (ulong)targetId };
            await self.NotifyAllMembers(group.Id, GroupChangedOp.RemoveMember, null, user, includePending: false);
            self.NotifyGroup(targetId, GroupChangedOp.RemoveGroup, null, null, group.Id);
            return ErrorCode.SUCCESS;
        });
    }

    public static async FTask<uint> BanGroupUsers(this GroupComponent self, PlayerData owner, ulong groupId, List<ulong> roleIds)
    {
        return await self.MutateMembers(owner, groupId, roleIds, async (group, actor, targetId) =>
        {
            if (actor.State > GroupMemberState.Admin)
            {
                return ErrorCode.GROUP_NO_PERMISSION;
            }

            if (targetId == owner.Id)
            {
                return ErrorCode.GROUP_INVALID_PARAMETER;
            }

            var target = await self.LoadMember(group.Id, targetId);
            if (target != null)
            {
                if (target.State <= actor.State)
                {
                    return ErrorCode.GROUP_NO_PERMISSION;
                }

                var wasActive = target.State <= GroupMemberState.Member;
                await self.Scene.World.Database.Remove<GroupMember>(target.Id);
                if (wasActive)
                {
                    self.SyncChatChannel(group.Id, targetId, GroupChannelSyncOp.Leave);
                }
            }

            var ban = await self.LoadBan(group.Id, targetId);
            if (ban == null)
            {
                ban = Entity.Create<GroupBan>(self.Scene, true, true);
                ban.GroupId = group.Id;
                ban.RoleId = targetId;
                ban.UpdateTime = TimeHelper.Now;
                await self.Scene.World.Database.Insert(ban);
            }

            var user = new CSGroupUser { RoleId = (ulong)targetId };
            await self.NotifyAllMembers(group.Id, GroupChangedOp.RemoveMember, null, user, includePending: false);
            self.NotifyGroup(targetId, GroupChangedOp.RemoveGroup, null, null, group.Id);
            return ErrorCode.SUCCESS;
        });
    }

    private static async FTask<uint> MutateMembers(
        this GroupComponent self,
        PlayerData owner,
        ulong groupId,
        List<ulong> roleIds,
        Func<GroupData, GroupMember, long, FTask<uint>> action)
    {
        if (groupId == 0 || roleIds == null || roleIds.Count == 0)
        {
            return ErrorCode.GROUP_INVALID_PARAMETER;
        }

        using (await self.Scene.CoroutineLockComponent.Wait(CoroutineLockType.GroupOperateLock, (long)groupId, "Group.Mutate", 10000))
        {
            var group = await self.LoadGroup((long)groupId);
            if (group == null)
            {
                return ErrorCode.GROUP_NOT_FOUND;
            }

            var actor = await self.LoadMember(group.Id, owner.Id);
            if (actor == null)
            {
                return ErrorCode.GROUP_NO_PERMISSION;
            }

            uint lastError = ErrorCode.SUCCESS;
            foreach (var roleId in roleIds)
            {
                if (roleId == 0)
                {
                    lastError = ErrorCode.GROUP_INVALID_PARAMETER;
                    continue;
                }

                var code = await action(group, actor, (long)roleId);
                if (code != ErrorCode.SUCCESS)
                {
                    lastError = code;
                }
            }

            return lastError;
        }
    }

    private static int ClampMaxCount(int maxCount)
    {
        if (maxCount <= 0)
        {
            return GroupLimit.DefaultMaxCount;
        }

        return Math.Clamp(maxCount, 1, GroupLimit.MemberMax);
    }

    private static int ClampListLimit(int limit)
    {
        if (limit <= 0)
        {
            return 20;
        }

        return Math.Min(limit, GroupLimit.ListMax);
    }

    private static int ParseCursor(string cursor)
    {
        if (!string.IsNullOrEmpty(cursor) && int.TryParse(cursor, out var parsed) && parsed > 0)
        {
            return parsed;
        }

        return 1;
    }

    private static async FTask<GroupData?> LoadGroup(this GroupComponent self, long groupId)
        => await self.Scene.World.Database.Query<GroupData>(groupId, true);

    private static async FTask<GroupMember?> LoadMember(this GroupComponent self, long groupId, long roleId)
        => await self.Scene.World.Database.First<GroupMember>(d => d.GroupId == groupId && d.RoleId == roleId, true);

    private static async FTask<GroupBan?> LoadBan(this GroupComponent self, long groupId, long roleId)
        => await self.Scene.World.Database.First<GroupBan>(d => d.GroupId == groupId && d.RoleId == roleId, true);

    private static async FTask<bool> IsBanned(this GroupComponent self, long groupId, long roleId)
        => await self.LoadBan(groupId, roleId) != null;

    private static async FTask<long> CountState(this GroupComponent self, long groupId, int state)
        => await self.Scene.World.Database.Count<GroupMember>(d => d.GroupId == groupId && d.State == state);

    private static async FTask<long> CountActiveMembers(this GroupComponent self, long groupId)
        => await self.Scene.World.Database.Count<GroupMember>(d => d.GroupId == groupId && d.State <= GroupMemberState.Member);

    private static async FTask<long> CountJoined(this GroupComponent self, long roleId)
        => await self.Scene.World.Database.Count<GroupMember>(d => d.RoleId == roleId && d.State <= GroupMemberState.Member);

    private static async FTask UpsertMember(this GroupComponent self, long groupId, long roleId, int state, GroupMember? exist)
    {
        var now = TimeHelper.Now;
        if (exist == null)
        {
            exist = Entity.Create<GroupMember>(self.Scene, true, true);
            exist.GroupId = groupId;
            exist.RoleId = roleId;
            exist.State = state;
            exist.UpdateTime = now;
            await self.Scene.World.Database.Insert(exist);
            return;
        }

        exist.State = state;
        exist.UpdateTime = now;
        await self.Scene.World.Database.Save(exist);
    }

    private static async FTask<List<GroupMember>> QueryMembers(this GroupComponent self, long groupId, int? stateFilter, int page, int limit)
    {
        if (stateFilter.HasValue)
        {
            return await self.Scene.World.Database.QueryByPageOrderBy<GroupMember>(
                d => d.GroupId == groupId && d.State == stateFilter.Value,
                page,
                limit,
                d => d.UpdateTime,
                false,
                true);
        }

        return await self.Scene.World.Database.QueryByPageOrderBy<GroupMember>(
            d => d.GroupId == groupId,
            page,
            limit,
            d => d.UpdateTime,
            false,
            true);
    }

    private static async FTask<List<GroupBan>> QueryBans(this GroupComponent self, long groupId)
        => await self.Scene.World.Database.QueryByPageOrderBy<GroupBan>(
            d => d.GroupId == groupId,
            1,
            GroupLimit.MemberMax,
            d => d.UpdateTime,
            false,
            true);

    private static async FTask<CSGroupInfo> ToGroupInfo(this GroupComponent self, GroupData group)
    {
        var memberCount = (int)await self.CountActiveMembers(group.Id);
        var info = new CSGroupInfo
        {
            GroupId = (ulong)group.Id,
            Name = group.Name ?? string.Empty,
            Description = group.Description ?? string.Empty,
            AvatarUrl = group.AvatarUrl ?? string.Empty,
            LangTag = group.LangTag ?? string.Empty,
            Open = group.Open,
            MaxCount = group.MaxCount,
            MemberCount = memberCount,
            CreatorRoleId = (ulong)group.CreatorRoleId,
            CreateTime = group.CreateTime
        };

        if (group.Metadata != null)
        {
            foreach (var kv in group.Metadata)
            {
                info.Metadata[kv.Key] = kv.Value;
            }
        }

        return info;
    }

    private static async FTask<CSGroupUser> ToGroupUser(this GroupComponent self, long roleId, int state)
    {
        var user = new CSGroupUser
        {
            RoleId = (ulong)roleId,
            State = state
        };

        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (playerManage != null && playerManage.TryGet(roleId, out var online))
        {
            user.RoleName = online.RoleName ?? string.Empty;
            user.HeadID = online.HeadID;
            user.Level = online.Level;
            return user;
        }

        var player = await self.Scene.World.Database.Query<PlayerData>(roleId, true);
        if (player != null)
        {
            user.RoleName = player.RoleName ?? string.Empty;
            user.HeadID = player.HeadID;
            user.Level = player.Level;
        }

        return user;
    }

    private static async FTask NotifyAllMembers(
        this GroupComponent self,
        long groupId,
        int op,
        CSGroupInfo? group,
        CSGroupUser? user,
        bool includePending)
    {
        var members = await self.QueryMembers(groupId, null, 1, GroupLimit.MemberMax + GroupLimit.ApplyMax);
        foreach (var member in members)
        {
            if (!includePending && member.State == GroupMemberState.JoinRequest)
            {
                continue;
            }

            self.NotifyGroup(member.RoleId, op, group, user, groupId);
        }
    }

    private static async FTask NotifyAdmins(this GroupComponent self, long groupId, int op, CSGroupInfo? group, CSGroupUser? user)
    {
        var members = await self.QueryMembers(groupId, null, 1, GroupLimit.MemberMax);
        foreach (var member in members)
        {
            if (member.State <= GroupMemberState.Admin)
            {
                self.NotifyGroup(member.RoleId, op, group, user, groupId);
            }
        }
    }

    private static void NotifyGroup(
        this GroupComponent self,
        long roleId,
        int op,
        CSGroupInfo? group,
        CSGroupUser? user,
        long groupId = 0)
    {
        var presence = self.Scene.GetComponent<PresenceComponent>();
        var notify = G2C_GroupChangedNotify.Create(false);
        notify.Op = op;
        notify.GroupId = group != null ? group.GroupId : (ulong)groupId;
        notify.Group = group;
        notify.User = user;
        presence?.SendToRole(roleId, notify);
    }

    private static void SyncChatChannel(this GroupComponent self, long groupId, long roleId, int op)
    {
        var chatConfigs = SceneConfigData.Instance.GetSceneBySceneType(SceneType.Chat);
        if (chatConfigs == null || chatConfigs.Count == 0)
        {
            return;
        }

        foreach (var config in chatConfigs)
        {
            self.Scene.Send(config.Address, new Game2Chat_GroupChannelSync
            {
                GroupId = groupId,
                RoleId = roleId,
                Op = op
            });
        }
    }
}
