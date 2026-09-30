using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_FollowUsersRequestHandler
    : RoamingRPC<PlayerData, C2Game_FollowUsersRequest, Game2C_FollowUsersResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_FollowUsersRequest request, Game2C_FollowUsersResponse response, Action reply)
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

        response.Presences = presence.Follow(playerData.Id, request.RoleIds);
        response.ErrorCode = ErrorCode.SUCCESS;
        await FTask.CompletedTask;
    }
}
