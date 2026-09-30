using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

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
