using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_UnfollowUsersRequestHandler
    : RoamingRPC<PlayerData, C2Game_UnfollowUsersRequest, Game2C_UnfollowUsersResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_UnfollowUsersRequest request, Game2C_UnfollowUsersResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.PRESENCE_INTERNAL_ERROR;
            return;
        }

        var presence = playerData.Scene.GetComponent<PresenceComponent>();
        if (presence == null)
        {
            response.ErrorCode = ErrorCode.PRESENCE_INTERNAL_ERROR;
            return;
        }

        presence.Unfollow(playerData.Id, request.RoleIds);
        response.ErrorCode = ErrorCode.SUCCESS;
        await FTask.CompletedTask;
    }
}
