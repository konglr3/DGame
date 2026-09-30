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
