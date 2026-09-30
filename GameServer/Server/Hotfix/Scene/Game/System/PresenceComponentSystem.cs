using System.Collections.Generic;
using Fantasy.Entitas.Interface;
using Fantasy.Network.Roaming;

namespace Fantasy;

public sealed class PresenceComponentDestroySystem : DestroySystem<PresenceComponent>
{
    protected override void Destroy(PresenceComponent self)
    {
        self.StatusByRoleId.Clear();
        self.FollowerMap.Clear();
        self.FollowingMap.Clear();
    }
}

/// <summary>
/// Game Scene 状态与显示逻辑。
/// </summary>
public static class PresenceComponentSystem
{
    public static void OnPlayerOnline(this PresenceComponent self, PlayerData playerData)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            return;
        }

        self.StatusByRoleId.TryAdd(playerData.Id, string.Empty);
        self.BroadcastToFollowers(playerData.Id, true);
    }

    public static void OnPlayerOffline(this PresenceComponent self, PlayerData playerData)
    {
        if (playerData == null)
        {
            return;
        }

        var roleId = playerData.Id;
        self.BroadcastToFollowers(roleId, false);
        self.StatusByRoleId.Remove(roleId);

        if (self.FollowingMap.Remove(roleId, out var following))
        {
            foreach (var targetId in following)
            {
                if (self.FollowerMap.TryGetValue(targetId, out var followers))
                {
                    followers.Remove(roleId);
                }
            }
        }

        self.FollowerMap.Remove(roleId);
    }

    public static List<CSStatusPresence> Follow(this PresenceComponent self, long followerRoleId, IList<ulong> roleIds)
    {
        var result = new List<CSStatusPresence>();
        if (roleIds == null || roleIds.Count == 0)
        {
            return result;
        }

        if (!self.FollowingMap.TryGetValue(followerRoleId, out var following))
        {
            following = new HashSet<long>();
            self.FollowingMap[followerRoleId] = following;
        }

        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        foreach (var rawId in roleIds)
        {
            var targetId = (long)rawId;
            if (targetId == 0 || targetId == followerRoleId)
            {
                continue;
            }

            following.Add(targetId);
            if (!self.FollowerMap.TryGetValue(targetId, out var followers))
            {
                followers = new HashSet<long>();
                self.FollowerMap[targetId] = followers;
            }

            followers.Add(followerRoleId);

            if (playerManage != null && playerManage.TryGet(targetId, out var target) && !target.IsDisposed)
            {
                result.Add(self.CreatePresence(target, true));
            }
        }

        return result;
    }

    public static void Unfollow(this PresenceComponent self, long followerRoleId, IList<ulong> roleIds)
    {
        if (roleIds == null || roleIds.Count == 0)
        {
            return;
        }

        if (!self.FollowingMap.TryGetValue(followerRoleId, out var following))
        {
            return;
        }

        foreach (var rawId in roleIds)
        {
            var targetId = (long)rawId;
            following.Remove(targetId);
            if (self.FollowerMap.TryGetValue(targetId, out var followers))
            {
                followers.Remove(followerRoleId);
            }
        }
    }

    public static void UpdateStatus(this PresenceComponent self, PlayerData playerData, string statusText)
    {
        statusText ??= string.Empty;
        self.StatusByRoleId[playerData.Id] = statusText;
        self.BroadcastToFollowers(playerData.Id, true);
    }

    private static void BroadcastToFollowers(this PresenceComponent self, long targetRoleId, bool online)
    {
        if (!self.FollowerMap.TryGetValue(targetRoleId, out var followers) || followers.Count == 0)
        {
            return;
        }

        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        PlayerData? target = null;
        playerManage?.TryGet(targetRoleId, out target);

        var presence = online && target != null
            ? self.CreatePresence(target, true)
            : new CSStatusPresence
            {
                RoleId = (ulong)targetRoleId,
                RoleName = target?.RoleName ?? string.Empty,
                StatusText = string.Empty
            };

        foreach (var followerId in followers)
        {
            var notify = G2C_StatusPresenceNotify.Create(false);
            if (online)
            {
                notify.Joins = new List<CSStatusPresence>
                {
                    new CSStatusPresence
                    {
                        RoleId = presence.RoleId,
                        RoleName = presence.RoleName,
                        StatusText = presence.StatusText
                    }
                };
                notify.Leaves = new List<CSStatusPresence>();
            }
            else
            {
                notify.Joins = new List<CSStatusPresence>();
                notify.Leaves = new List<CSStatusPresence>
                {
                    new CSStatusPresence
                    {
                        RoleId = presence.RoleId,
                        RoleName = presence.RoleName,
                        StatusText = string.Empty
                    }
                };
            }

            self.SendToRole(followerId, notify);
        }
    }

    private static CSStatusPresence CreatePresence(this PresenceComponent self, PlayerData playerData, bool online)
    {
        self.StatusByRoleId.TryGetValue(playerData.Id, out var status);
        return new CSStatusPresence
        {
            RoleId = (ulong)playerData.Id,
            RoleName = playerData.RoleName ?? string.Empty,
            StatusText = online ? status ?? string.Empty : string.Empty
        };
    }

    public static void SendToRole(this PresenceComponent self, long roleId, G2C_StatusPresenceNotify message)
    {
        PushToGate(self, roleId, message);
    }

    public static void SendToRole(this PresenceComponent self, long roleId, G2C_FriendChangedNotify message)
    {
        PushToGate(self, roleId, message);
    }

    public static void SendToRole(this PresenceComponent self, long roleId, G2C_GroupChangedNotify message)
    {
        PushToGate(self, roleId, message);
    }

    private static void PushToGate(PresenceComponent self, long roleId, G2C_StatusPresenceNotify message)
    {
        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (playerManage == null || !playerManage.TryGet(roleId, out var playerData) || playerData.IsDisposed)
        {
            return;
        }

        if (!playerData.TryGetLinkTerminus(out var terminus) ||
            terminus == null ||
            terminus.ForwardSceneAddress == 0 ||
            terminus.ForwardSessionAddress == 0)
        {
            return;
        }

        self.Scene.Send(terminus.ForwardSceneAddress, new Game2G_StatusPresenceNotify
        {
            SessionRuntimeId = terminus.ForwardSessionAddress,
            Joins = message.Joins,
            Leaves = message.Leaves
        });
    }

    private static void PushToGate(PresenceComponent self, long roleId, G2C_FriendChangedNotify message)
    {
        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (playerManage == null || !playerManage.TryGet(roleId, out var playerData) || playerData.IsDisposed)
        {
            return;
        }

        if (!playerData.TryGetLinkTerminus(out var terminus) ||
            terminus == null ||
            terminus.ForwardSceneAddress == 0 ||
            terminus.ForwardSessionAddress == 0)
        {
            return;
        }

        self.Scene.Send(terminus.ForwardSceneAddress, new Game2G_FriendChangedNotify
        {
            SessionRuntimeId = terminus.ForwardSessionAddress,
            Op = message.Op,
            Friend = message.Friend
        });
    }

    private static void PushToGate(PresenceComponent self, long roleId, G2C_GroupChangedNotify message)
    {
        var playerManage = self.Scene.GetComponent<GamePlayerManageComponent>();
        if (playerManage == null || !playerManage.TryGet(roleId, out var playerData) || playerData.IsDisposed)
        {
            return;
        }

        if (!playerData.TryGetLinkTerminus(out var terminus) ||
            terminus == null ||
            terminus.ForwardSceneAddress == 0 ||
            terminus.ForwardSessionAddress == 0)
        {
            return;
        }

        self.Scene.Send(terminus.ForwardSceneAddress, new Game2G_GroupChangedNotify
        {
            SessionRuntimeId = terminus.ForwardSessionAddress,
            Op = message.Op,
            GroupId = message.GroupId,
            Group = message.Group,
            User = message.User
        });
    }
}
