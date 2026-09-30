using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_KickGroupUsersRequestHandler
    : RoamingRPC<PlayerData, C2Game_KickGroupUsersRequest, Game2C_KickGroupUsersResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_KickGroupUsersRequest request, Game2C_KickGroupUsersResponse response, Action reply)
    {
        if (playerData == null || playerData.IsDisposed)
        {
            response.ErrorCode = ErrorCode.GROUP_INTERNAL_ERROR;
            return;
        }

        var groupComponent = playerData.Scene.GetComponent<GroupComponent>();
        if (groupComponent == null)
        {
            response.ErrorCode = ErrorCode.GROUP_INTERNAL_ERROR;
            return;
        }

        response.ErrorCode = await groupComponent.KickGroupUsers(playerData, request.GroupId, request.RoleIds);
    }
}
