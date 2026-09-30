using System.Collections.Generic;
using System.Reflection;
using Fantasy;
using Fantasy.Async;
using Fantasy.Helper;
using GOS;
using GOS.Chat;
using GOS.Friend;
using GOS.Group;
using GOS.Presence;

namespace GameLogic
{
    public static class GOSTest
    {
        public static async FTask Initialize(List<Assembly> assemblies)
        {
            var apiUrl = $"http://127.0.0.1:20001/api/";
            await GOS.GOSGame.Initialize(assemblies, new GOSEngineModule(), apiUrl);
            var client = await GOS.GOSGame.CreateClient();

            // var response = await client.Login.Register("testUsername", "testPassword");
            // UnityEngine.Debug.LogWarning(response?.ToJson());

            var errorCode = await client.Auth.Login("testUsername", "testPassword");
            if (errorCode != 0)
            {
                return;
            }

            // await TestAutoReconnect(client);
            // await TestGameRoamingRpc(client);
            // await TestChat(client);
            // await TestFriend(client);
            // await TestPresence(client);
            await TestGroup(client);
        }

        private static async FTask TestAutoReconnect(GOSClient client)
        {
            await client.Scene.TimerComponent.Unity.WaitAsync(1500);
            client.Scene.GetComponent<HostComponent>().Disconnect(true);
        }

        private static async FTask TestGameRoamingRpc(GOSClient client)
        {
            var session = client.Scene.Session;
            if (session == null || session.IsDisposed)
            {
                UnityEngine.Debug.LogError("TestGameRoamingRpc fail: session disposed");
                return;
            }

            var response = await session.C2Game_GetPlayerDataRequest();
            UnityEngine.Debug.LogWarning(
                $"TestGameRoamingRpc ErrorCode={response.ErrorCode} PlayerData={response.PlayerData?.ToJson()}");

            await TestChat(client);
        }

        private static async FTask TestChat(GOSClient client)
        {
            var chat = client.Scene.EnsureChat();
            chat.OnMessageReceived = (tree, text) =>
            {
                UnityEngine.Debug.LogWarning($"[Chat] from={tree?.UserName} channel={tree?.ChatChannelType} text={text}");
            };

            var broadcastResponse = await chat.SendBroadcast("hello chat");
            UnityEngine.Debug.LogWarning($"TestChat Broadcast ErrorCode={broadcastResponse?.ErrorCode}");

            var teamResponse = await chat.SendTeam("team hello");
            UnityEngine.Debug.LogWarning($"TestChat Team ErrorCode={teamResponse?.ErrorCode}");
        }

        /// <summary>
        /// 好友 ECS：拉列表 / 推荐 / 按角色名申请（目标不存在时也会打出错误码）。
        /// </summary>
        private static async FTask TestFriend(GOSClient client)
        {
            var friend = client.Scene.EnsureFriend();
            friend.OnFriendChanged = () =>
            {
                UnityEngine.Debug.LogWarning(
                    $"[Friend] changed mutual={friend.MutualFriends.Count} incoming={friend.IncomingRequests.Count} " +
                    $"outgoing={friend.OutgoingRequests.Count} recommend={friend.RecommendFriends.Count}");
            };

            await friend.RefreshAll();
            UnityEngine.Debug.LogWarning(
                $"TestFriend RefreshAll mutual={friend.MutualFriends.Count} incoming={friend.IncomingRequests.Count} " +
                $"outgoing={friend.OutgoingRequests.Count} recommend={friend.RecommendFriends.Count}");

            foreach (var item in friend.MutualFriends)
            {
                UnityEngine.Debug.LogWarning($"[Friend][Mutual] {item.RoleName} Id={item.RoleId} Lv={item.Level}");
            }

            foreach (var item in friend.RecommendFriends)
            {
                UnityEngine.Debug.LogWarning($"[Friend][Recommend] {item.RoleName} Id={item.RoleId} Lv={item.Level}");
            }

            // 按角色名发申请：请改成实际存在的第二个角色名做双向联调
            var addByName = await friend.AddFriend(0, "NotExistFriendName");
            UnityEngine.Debug.LogWarning($"TestFriend AddByName ErrorCode={addByName?.ErrorCode}");

            if (friend.RecommendFriends.Count > 0)
            {
                var target = friend.RecommendFriends[0];
                var addById = await friend.AddFriend(target.RoleId);
                UnityEngine.Debug.LogWarning(
                    $"TestFriend AddById RoleId={target.RoleId} Name={target.RoleName} ErrorCode={addById?.ErrorCode} " +
                    $"FriendState={addById?.Friend?.State}");
            }

            var listIncoming = await friend.ListFriends(FriendState.Incoming);
            UnityEngine.Debug.LogWarning(
                $"TestFriend ListIncoming ErrorCode={listIncoming?.ErrorCode} Count={friend.IncomingRequests.Count}");
        }

        /// <summary>
        /// Presence ECS：对共同好友 Follow，并更新自身状态文案。
        /// </summary>
        private static async FTask TestPresence(GOSClient client)
        {
            var friend = client.Scene.EnsureFriend();
            var presence = client.Scene.EnsurePresence();
            presence.OnPresenceChanged = () =>
            {
                UnityEngine.Debug.LogWarning($"[Presence] cacheCount={presence.Cache.Count} following={presence.Following.Count}");
            };

            var mutualIds = new List<ulong>();
            foreach (var item in friend.MutualFriends)
            {
                if (item != null && item.RoleId > 0)
                {
                    mutualIds.Add(item.RoleId);
                }
            }

            await presence.Follow(mutualIds);
            UnityEngine.Debug.LogWarning($"TestPresence Follow mutualCount={mutualIds.Count} onlineCache={presence.Cache.Count}");

            foreach (var kv in presence.Cache)
            {
                var cache = kv.Value;
                UnityEngine.Debug.LogWarning(
                    $"[Presence] RoleId={cache.RoleId} Name={cache.RoleName} Online={cache.Online} Status={cache.StatusText}");
            }

            await presence.UpdateStatus("GOSTest Viewing Friend Panel");
            UnityEngine.Debug.LogWarning($"TestPresence UpdateStatus MyStatus={presence.MyStatusText}");
        }

        /// <summary>
        /// 群组 ECS：创建 / 列表 / 元数据 / 成员列表 / 群聊；Join/审批需第二角色联调。
        /// </summary>
        private static async FTask TestGroup(GOSClient client)
        {
            var chat = client.Scene.EnsureChat();
            chat.OnMessageReceived = (tree, text) =>
            {
                UnityEngine.Debug.LogWarning(
                    $"[GroupChat] from={tree?.UserName} channelType={tree?.ChatChannelType} " +
                    $"channelId={tree?.ChatChannelId} text={text}");
            };

            var group = client.Scene.EnsureGroup();
            group.OnGroupChanged = () =>
            {
                UnityEngine.Debug.LogWarning(
                    $"[Group] changed myGroups={group.MyGroups.Count} search={group.SearchGroups.Count} " +
                    $"members={group.CurrentMembers.Count} currentGroupId={group.CurrentGroupId}");
            };

            await group.RefreshMyGroups();
            UnityEngine.Debug.LogWarning($"TestGroup RefreshMyGroups count={group.MyGroups.Count}");
            foreach (var item in group.MyGroups)
            {
                UnityEngine.Debug.LogWarning(
                    $"[Group][Mine] {item.Group?.Name} Id={item.Group?.GroupId} State={item.State} " +
                    $"Open={item.Group?.Open} Members={item.Group?.MemberCount}");
            }

            var groupName = $"GOSTest_{TimeHelper.Now % 100000}";
            var createOpen = await group.CreateGroup(groupName, "GOSTest open group", open: true, maxCount: 50);
            UnityEngine.Debug.LogWarning(
                $"TestGroup CreateOpen ErrorCode={createOpen?.ErrorCode} " +
                $"GroupId={createOpen?.Group?.GroupId} Name={createOpen?.Group?.Name}");

            ulong groupId = createOpen?.Group?.GroupId ?? 0;
            if (groupId == 0 && group.MyGroups.Count > 0 && group.MyGroups[0].Group != null)
            {
                groupId = group.MyGroups[0].Group.GroupId;
            }

            if (groupId == 0)
            {
                UnityEngine.Debug.LogError("TestGroup abort: no groupId");
                return;
            }

            var listSearch = await group.ListGroups("GOSTest", limit: 20);
            UnityEngine.Debug.LogWarning(
                $"TestGroup ListGroups ErrorCode={listSearch?.ErrorCode} Count={group.SearchGroups.Count} Cursor={listSearch?.Cursor}");
            foreach (var item in group.SearchGroups)
            {
                UnityEngine.Debug.LogWarning($"[Group][Search] {item.Name} Id={item.GroupId} Open={item.Open}");
            }

            var meta = new Dictionary<string, string>
            {
                ["Interests"] = "Deception,Sabotage",
                ["ActiveTimes"] = "9am-10pm",
                ["Lang"] = "zh-CN"
            };
            var updateMeta = await group.UpdateGroupMetadata(groupId, meta);
            UnityEngine.Debug.LogWarning(
                $"TestGroup UpdateMetadata ErrorCode={updateMeta?.ErrorCode} MetaCount={updateMeta?.Group?.Metadata?.Count}");
            if (updateMeta?.Group?.Metadata != null)
            {
                foreach (var kv in updateMeta.Group.Metadata)
                {
                    UnityEngine.Debug.LogWarning($"[Group][Meta] {kv.Key}={kv.Value}");
                }
            }

            var updateGroup = await group.UpdateGroup(groupId, description: "updated by GOSTest", open: true);
            UnityEngine.Debug.LogWarning(
                $"TestGroup UpdateGroup ErrorCode={updateGroup?.ErrorCode} Desc={updateGroup?.Group?.Description}");

            var listUsers = await group.ListGroupUsers(groupId, state: -1);
            UnityEngine.Debug.LogWarning(
                $"TestGroup ListUsers ErrorCode={listUsers?.ErrorCode} Count={group.CurrentMembers.Count}");
            foreach (var user in group.CurrentMembers)
            {
                UnityEngine.Debug.LogWarning(
                    $"[Group][Member] {user.RoleName} Id={user.RoleId} State={user.State} Lv={user.Level}");
            }

            var chatResponse = await group.SendGroupChat(groupId, "hello group chat from GOSTest");
            UnityEngine.Debug.LogWarning($"TestGroup SendGroupChat ErrorCode={chatResponse?.ErrorCode}");

            // 私密群：创建后第二客户端 Join 会变申请(State=3)，本端再 AddGroupUsers 批准
            var privateName = $"GOSTest_Private_{TimeHelper.Now % 100000}";
            var createPrivate = await group.CreateGroup(privateName, "GOSTest private group", open: false, maxCount: 20);
            UnityEngine.Debug.LogWarning(
                $"TestGroup CreatePrivate ErrorCode={createPrivate?.ErrorCode} GroupId={createPrivate?.Group?.GroupId}");

            // 单端烟雾：Join 不存在的群应返回 GROUP_NOT_FOUND
            var joinMissing = await group.JoinGroup(999999999UL);
            UnityEngine.Debug.LogWarning($"TestGroup JoinMissing ErrorCode={joinMissing?.ErrorCode}");

            // 联调提示：第二角色执行 JoinGroup(groupId) 后，本端可：
            // await group.ListGroupUsers(groupId, GroupMemberState.JoinRequest);
            // await group.AddGroupUsers(groupId, targetRoleId);
            // await group.PromoteGroupUsers(groupId, targetRoleId);
            // await group.KickGroupUsers(groupId, targetRoleId);
            await group.RefreshMyGroups();
            UnityEngine.Debug.LogWarning($"TestGroup Done myGroups={group.MyGroups.Count}");
        }
    }
}
