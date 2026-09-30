using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_ListGroupUsersRequestHandler
    : RoamingRPC<PlayerData, C2Game_ListGroupUsersRequest, Game2C_ListGroupUsersResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_ListGroupUsersRequest request, Game2C_ListGroupUsersResponse response, Action reply)
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

        var (users, cursor) = await groupComponent.ListGroupUsers(request.GroupId, request.State, request.Limit, request.Cursor);
        response.ErrorCode = ErrorCode.SUCCESS;
        response.Users = users;
        response.Cursor = cursor;
    }
}
