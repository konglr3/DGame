using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_PromoteGroupUsersRequestHandler
    : RoamingRPC<PlayerData, C2Game_PromoteGroupUsersRequest, Game2C_PromoteGroupUsersResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_PromoteGroupUsersRequest request, Game2C_PromoteGroupUsersResponse response, Action reply)
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

        response.ErrorCode = await groupComponent.PromoteGroupUsers(playerData, request.GroupId, request.RoleIds);
    }
}
