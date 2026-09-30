using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_AddGroupUsersRequestHandler
    : RoamingRPC<PlayerData, C2Game_AddGroupUsersRequest, Game2C_AddGroupUsersResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_AddGroupUsersRequest request, Game2C_AddGroupUsersResponse response, Action reply)
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

        response.ErrorCode = await groupComponent.AddGroupUsers(playerData, request.GroupId, request.RoleIds);
    }
}
