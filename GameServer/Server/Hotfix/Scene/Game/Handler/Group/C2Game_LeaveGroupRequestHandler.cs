using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_LeaveGroupRequestHandler
    : RoamingRPC<PlayerData, C2Game_LeaveGroupRequest, Game2C_LeaveGroupResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_LeaveGroupRequest request, Game2C_LeaveGroupResponse response, Action reply)
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

        response.ErrorCode = await groupComponent.LeaveGroup(playerData, request.GroupId);
    }
}
