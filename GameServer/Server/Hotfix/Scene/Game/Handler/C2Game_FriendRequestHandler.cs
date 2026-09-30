using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_AddFriendRequestHandler
    : RoamingRPC<PlayerData, C2Game_AddFriendRequest, Game2C_AddFriendResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_AddFriendRequest request, Game2C_AddFriendResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        if (request.TargetRoleId == 0 && string.IsNullOrWhiteSpace(request.TargetRoleName))
        {
            response.ErrorCode = ErrorCode.FRIEND_INVALID_PARAMETER;
            return;
        }

        var friendComponent = playerData.Scene.GetComponent<FriendComponent>();
        if (friendComponent == null)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        var (errorCode, friend) = await friendComponent.AddFriend(playerData, request.TargetRoleId, request.TargetRoleName);
        response.ErrorCode = errorCode;
        response.Friend = friend;
    }
}

public sealed class C2Game_DeleteFriendRequestHandler
    : RoamingRPC<PlayerData, C2Game_DeleteFriendRequest, Game2C_DeleteFriendResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_DeleteFriendRequest request, Game2C_DeleteFriendResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        if (request.TargetRoleId == 0 && string.IsNullOrWhiteSpace(request.TargetRoleName))
        {
            response.ErrorCode = ErrorCode.FRIEND_INVALID_PARAMETER;
            return;
        }

        var friendComponent = playerData.Scene.GetComponent<FriendComponent>();
        if (friendComponent == null)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        response.ErrorCode = await friendComponent.DeleteFriend(playerData, request.TargetRoleId, request.TargetRoleName);
    }
}

public sealed class C2Game_BlockFriendRequestHandler
    : RoamingRPC<PlayerData, C2Game_BlockFriendRequest, Game2C_BlockFriendResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_BlockFriendRequest request, Game2C_BlockFriendResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        if (request.TargetRoleId == 0 && string.IsNullOrWhiteSpace(request.TargetRoleName))
        {
            response.ErrorCode = ErrorCode.FRIEND_INVALID_PARAMETER;
            return;
        }

        var friendComponent = playerData.Scene.GetComponent<FriendComponent>();
        if (friendComponent == null)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        var (errorCode, friend) = await friendComponent.BlockFriend(playerData, request.TargetRoleId, request.TargetRoleName);
        response.ErrorCode = errorCode;
        response.Friend = friend;
    }
}

public sealed class C2Game_ListFriendsRequestHandler
    : RoamingRPC<PlayerData, C2Game_ListFriendsRequest, Game2C_ListFriendsResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_ListFriendsRequest request, Game2C_ListFriendsResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        var friendComponent = playerData.Scene.GetComponent<FriendComponent>();
        if (friendComponent == null)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        var (friends, cursor) = await friendComponent.ListFriends(playerData.Id, request.State, request.Limit, request.Cursor);
        response.Friends = friends;
        response.Cursor = cursor;
        response.ErrorCode = ErrorCode.SUCCESS;
    }
}

public sealed class C2Game_ListRecommendFriendsRequestHandler
    : RoamingRPC<PlayerData, C2Game_ListRecommendFriendsRequest, Game2C_ListRecommendFriendsResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_ListRecommendFriendsRequest request, Game2C_ListRecommendFriendsResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        var friendComponent = playerData.Scene.GetComponent<FriendComponent>();
        if (friendComponent == null)
        {
            response.ErrorCode = ErrorCode.FRIEND_INTERNAL_ERROR;
            return;
        }

        response.Friends = await friendComponent.ListRecommend(playerData, request.Limit);
        response.ErrorCode = ErrorCode.SUCCESS;
    }
}
