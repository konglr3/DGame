using System;
using System.Collections.Generic;
using Fantasy;
using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Network;
using GOS.Chat;

namespace GOS.Group
{
    /// <summary>
    /// 群组成员状态，对齐 Nakama Groups / 服务端 GroupMemberState。
    /// </summary>
    public static class GroupMemberState
    {
        public const int Superadmin = 0;
        public const int Admin = 1;
        public const int Member = 2;
        public const int JoinRequest = 3;
    }

    /// <summary>
    /// 群组变更推送操作码。
    /// </summary>
    public static class GroupChangedOp
    {
        public const int UpsertGroup = 1;
        public const int RemoveGroup = 2;
        public const int UpsertMember = 3;
        public const int RemoveMember = 4;
    }

    /// <summary>
    /// 客户端群组 ECS 组件：缓存我的群、搜索结果、成员列表。
    /// </summary>
    public sealed class GroupComponent : Entity
    {
        public readonly List<CSUserGroup> MyGroups = new List<CSUserGroup>();

        public readonly List<CSGroupInfo> SearchGroups = new List<CSGroupInfo>();

        public readonly List<CSGroupUser> CurrentMembers = new List<CSGroupUser>();

        public ulong CurrentGroupId;

        public Action OnGroupChanged;
    }

    /// <summary>
    /// <see cref="GroupComponent"/> 扩展：Ensure、群 CRUD、成员管理、群聊发送、推送落地。
    /// </summary>
    public static class GroupComponentSystem
    {
        public static GroupComponent EnsureGroup(this Scene scene)
        {
            return scene.GetOrAddComponent<GroupComponent>();
        }

        public static async FTask RefreshMyGroups(this GroupComponent self)
        {
            await self.ListUserGroups();
        }

        public static async FTask<Game2C_CreateGroupResponse> CreateGroup(
            this GroupComponent self,
            string name,
            string description = "",
            bool open = true,
            int maxCount = 100,
            string avatarUrl = "",
            string langTag = "")
        {
            var session = self.GetSession("CreateGroup");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_CreateGroupRequest(
                name ?? string.Empty,
                description ?? string.Empty,
                avatarUrl ?? string.Empty,
                langTag ?? string.Empty,
                open,
                maxCount);
            if (response != null && response.ErrorCode == 0 && response.Group != null)
            {
                self.UpsertMyGroup(response.Group, GroupMemberState.Superadmin);
                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_UpdateGroupResponse> UpdateGroup(
            this GroupComponent self,
            ulong groupId,
            string name = null,
            string description = null,
            string avatarUrl = null,
            string langTag = null,
            bool? open = null,
            int maxCount = 0)
        {
            var session = self.GetSession("UpdateGroup");
            if (session == null)
            {
                return null;
            }

            using var request = C2Game_UpdateGroupRequest.Create();
            request.GroupId = groupId;
            request.Name = name ?? string.Empty;
            request.Description = description ?? string.Empty;
            request.AvatarUrl = avatarUrl ?? string.Empty;
            request.LangTag = langTag ?? string.Empty;
            request.HasOpen = open.HasValue;
            request.Open = open ?? false;
            request.MaxCount = maxCount;
            var response = await session.C2Game_UpdateGroupRequest(request);
            if (response != null && response.ErrorCode == 0 && response.Group != null)
            {
                self.UpsertMyGroupInfo(response.Group);
                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_UpdateGroupMetadataResponse> UpdateGroupMetadata(
            this GroupComponent self,
            ulong groupId,
            Dictionary<string, string> metadata)
        {
            var session = self.GetSession("UpdateGroupMetadata");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_UpdateGroupMetadataRequest(groupId, metadata ?? new Dictionary<string, string>());
            if (response != null && response.ErrorCode == 0 && response.Group != null)
            {
                self.UpsertMyGroupInfo(response.Group);
                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_DeleteGroupResponse> DeleteGroup(this GroupComponent self, ulong groupId)
        {
            var session = self.GetSession("DeleteGroup");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_DeleteGroupRequest(groupId);
            if (response != null && response.ErrorCode == 0)
            {
                self.RemoveMyGroup(groupId);
                if (self.CurrentGroupId == groupId)
                {
                    self.CurrentGroupId = 0;
                    self.CurrentMembers.Clear();
                }

                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_ListGroupsResponse> ListGroups(
            this GroupComponent self,
            string nameFilter = "",
            int limit = 20,
            string cursor = null)
        {
            var session = self.GetSession("ListGroups");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_ListGroupsRequest(nameFilter ?? string.Empty, limit, cursor ?? string.Empty);
            if (response != null && response.ErrorCode == 0)
            {
                self.SearchGroups.Clear();
                if (response.Groups != null)
                {
                    self.SearchGroups.AddRange(response.Groups);
                }

                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_ListUserGroupsResponse> ListUserGroups(this GroupComponent self)
        {
            var session = self.GetSession("ListUserGroups");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_ListUserGroupsRequest();
            if (response != null && response.ErrorCode == 0)
            {
                self.MyGroups.Clear();
                if (response.UserGroups != null)
                {
                    self.MyGroups.AddRange(response.UserGroups);
                }

                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_JoinGroupResponse> JoinGroup(this GroupComponent self, ulong groupId)
        {
            var session = self.GetSession("JoinGroup");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_JoinGroupRequest(groupId);
            if (response != null && response.ErrorCode == 0 && response.Group != null)
            {
                self.UpsertMyGroup(response.Group, response.State);
                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_LeaveGroupResponse> LeaveGroup(this GroupComponent self, ulong groupId)
        {
            var session = self.GetSession("LeaveGroup");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_LeaveGroupRequest(groupId);
            if (response != null && response.ErrorCode == 0)
            {
                self.RemoveMyGroup(groupId);
                if (self.CurrentGroupId == groupId)
                {
                    self.CurrentGroupId = 0;
                    self.CurrentMembers.Clear();
                }

                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_ListGroupUsersResponse> ListGroupUsers(
            this GroupComponent self,
            ulong groupId,
            int state = -1,
            int limit = 50,
            string cursor = null)
        {
            var session = self.GetSession("ListGroupUsers");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_ListGroupUsersRequest(groupId, state, limit, cursor ?? string.Empty);
            if (response != null && response.ErrorCode == 0)
            {
                self.CurrentGroupId = groupId;
                self.CurrentMembers.Clear();
                if (response.Users != null)
                {
                    self.CurrentMembers.AddRange(response.Users);
                }

                self.OnGroupChanged?.Invoke();
            }

            return response;
        }

        public static async FTask<Game2C_AddGroupUsersResponse> AddGroupUsers(this GroupComponent self, ulong groupId, params ulong[] roleIds)
        {
            var session = self.GetSession("AddGroupUsers");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_AddGroupUsersRequest(groupId, new List<ulong>(roleIds));
            if (response != null && response.ErrorCode == 0)
            {
                await self.ListGroupUsers(groupId);
            }

            return response;
        }

        public static async FTask<Game2C_PromoteGroupUsersResponse> PromoteGroupUsers(this GroupComponent self, ulong groupId, params ulong[] roleIds)
        {
            var session = self.GetSession("PromoteGroupUsers");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_PromoteGroupUsersRequest(groupId, new List<ulong>(roleIds));
            if (response != null && response.ErrorCode == 0)
            {
                await self.ListGroupUsers(groupId);
            }

            return response;
        }

        public static async FTask<Game2C_DemoteGroupUsersResponse> DemoteGroupUsers(this GroupComponent self, ulong groupId, params ulong[] roleIds)
        {
            var session = self.GetSession("DemoteGroupUsers");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_DemoteGroupUsersRequest(groupId, new List<ulong>(roleIds));
            if (response != null && response.ErrorCode == 0)
            {
                await self.ListGroupUsers(groupId);
            }

            return response;
        }

        public static async FTask<Game2C_KickGroupUsersResponse> KickGroupUsers(this GroupComponent self, ulong groupId, params ulong[] roleIds)
        {
            var session = self.GetSession("KickGroupUsers");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_KickGroupUsersRequest(groupId, new List<ulong>(roleIds));
            if (response != null && response.ErrorCode == 0)
            {
                await self.ListGroupUsers(groupId);
            }

            return response;
        }

        public static async FTask<Game2C_BanGroupUsersResponse> BanGroupUsers(this GroupComponent self, ulong groupId, params ulong[] roleIds)
        {
            var session = self.GetSession("BanGroupUsers");
            if (session == null)
            {
                return null;
            }

            var response = await session.C2Game_BanGroupUsersRequest(groupId, new List<ulong>(roleIds));
            if (response != null && response.ErrorCode == 0)
            {
                await self.ListGroupUsers(groupId);
            }

            return response;
        }

        /// <summary>
        /// 发送群组聊天（复用 Chat Roaming）。
        /// </summary>
        public static async FTask<Chat2C_SendMessageResponse> SendGroupChat(this GroupComponent self, ulong groupId, string content)
        {
            var chat = self.Scene.EnsureChat();
            var tree = ChatTreeFactory.Group(self.Scene, (long)groupId).AddendTextNode(content ?? string.Empty);
            return await chat.Send(tree);
        }

        public static void ApplyNotify(this GroupComponent self, G2C_GroupChangedNotify message)
        {
            if (message == null)
            {
                return;
            }

            switch (message.Op)
            {
                case GroupChangedOp.UpsertGroup:
                    if (message.Group != null)
                    {
                        self.UpsertMyGroupInfo(message.Group);
                    }

                    break;
                case GroupChangedOp.RemoveGroup:
                    self.RemoveMyGroup(message.GroupId);
                    if (self.CurrentGroupId == message.GroupId)
                    {
                        self.CurrentGroupId = 0;
                        self.CurrentMembers.Clear();
                    }

                    break;
                case GroupChangedOp.UpsertMember:
                    if (self.CurrentGroupId == message.GroupId && message.User != null)
                    {
                        self.UpsertMemberLocal(message.User);
                    }

                    if (message.Group != null)
                    {
                        self.UpsertMyGroupInfo(message.Group);
                    }

                    break;
                case GroupChangedOp.RemoveMember:
                    if (self.CurrentGroupId == message.GroupId && message.User != null)
                    {
                        self.RemoveMemberLocal(message.User.RoleId);
                    }

                    break;
            }

            self.OnGroupChanged?.Invoke();
        }

        public static void Clear(this GroupComponent self)
        {
            self.MyGroups.Clear();
            self.SearchGroups.Clear();
            self.CurrentMembers.Clear();
            self.CurrentGroupId = 0;
            self.OnGroupChanged = null;
        }

        private static Session GetSession(this GroupComponent self, string api)
        {
            var session = self.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                Log.Error($"GroupComponent.{api} fail: session disposed");
                return null;
            }

            return session;
        }

        private static void UpsertMyGroup(this GroupComponent self, CSGroupInfo group, int state)
        {
            for (var i = 0; i < self.MyGroups.Count; i++)
            {
                if (self.MyGroups[i]?.Group != null && self.MyGroups[i].Group.GroupId == group.GroupId)
                {
                    self.MyGroups[i].Group = group;
                    self.MyGroups[i].State = state;
                    return;
                }
            }

            self.MyGroups.Add(new CSUserGroup { Group = group, State = state });
        }

        private static void UpsertMyGroupInfo(this GroupComponent self, CSGroupInfo group)
        {
            for (var i = 0; i < self.MyGroups.Count; i++)
            {
                if (self.MyGroups[i]?.Group != null && self.MyGroups[i].Group.GroupId == group.GroupId)
                {
                    var state = self.MyGroups[i].State;
                    self.MyGroups[i].Group = group;
                    self.MyGroups[i].State = state;
                    return;
                }
            }
        }

        private static void RemoveMyGroup(this GroupComponent self, ulong groupId)
        {
            for (var i = self.MyGroups.Count - 1; i >= 0; i--)
            {
                if (self.MyGroups[i]?.Group != null && self.MyGroups[i].Group.GroupId == groupId)
                {
                    self.MyGroups.RemoveAt(i);
                }
            }
        }

        private static void UpsertMemberLocal(this GroupComponent self, CSGroupUser user)
        {
            for (var i = 0; i < self.CurrentMembers.Count; i++)
            {
                if (self.CurrentMembers[i] != null && self.CurrentMembers[i].RoleId == user.RoleId)
                {
                    self.CurrentMembers[i] = user;
                    return;
                }
            }

            self.CurrentMembers.Add(user);
        }

        private static void RemoveMemberLocal(this GroupComponent self, ulong roleId)
        {
            for (var i = self.CurrentMembers.Count - 1; i >= 0; i--)
            {
                if (self.CurrentMembers[i] != null && self.CurrentMembers[i].RoleId == roleId)
                {
                    self.CurrentMembers.RemoveAt(i);
                }
            }
        }
    }
}
