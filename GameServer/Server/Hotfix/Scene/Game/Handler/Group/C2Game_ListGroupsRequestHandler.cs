using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_ListGroupsRequestHandler
    : RoamingRPC<PlayerData, C2Game_ListGroupsRequest, Game2C_ListGroupsResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_ListGroupsRequest request, Game2C_ListGroupsResponse response, Action reply)
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

        var (groups, cursor) = await groupComponent.ListGroups(request.NameFilter, request.Limit, request.Cursor);
        response.ErrorCode = ErrorCode.SUCCESS;
        response.Groups = groups;
        response.Cursor = cursor;
    }
}
