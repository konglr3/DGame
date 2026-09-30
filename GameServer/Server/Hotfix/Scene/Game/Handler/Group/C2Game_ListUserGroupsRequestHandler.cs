using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_ListUserGroupsRequestHandler
    : RoamingRPC<PlayerData, C2Game_ListUserGroupsRequest, Game2C_ListUserGroupsResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_ListUserGroupsRequest request, Game2C_ListUserGroupsResponse response, Action reply)
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

        response.ErrorCode = ErrorCode.SUCCESS;
        response.UserGroups = await groupComponent.ListUserGroups(playerData.Id);
    }
}
