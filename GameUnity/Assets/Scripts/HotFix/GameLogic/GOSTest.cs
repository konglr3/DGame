using System.Collections.Generic;
using System.Reflection;
using Fantasy;
using Fantasy.Async;
using Fantasy.Helper;
using GOS;
using GOS.Chat;
using GOS.Friend;
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
            await TestFriend(client);
            await TestPresence(client);
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
    }
}
