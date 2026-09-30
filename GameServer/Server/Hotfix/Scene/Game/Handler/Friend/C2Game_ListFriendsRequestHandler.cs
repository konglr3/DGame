using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

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
