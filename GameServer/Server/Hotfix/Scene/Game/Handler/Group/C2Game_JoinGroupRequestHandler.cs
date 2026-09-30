using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_JoinGroupRequestHandler
    : RoamingRPC<PlayerData, C2Game_JoinGroupRequest, Game2C_JoinGroupResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_JoinGroupRequest request, Game2C_JoinGroupResponse response, Action reply)
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

        var (errorCode, group, state) = await groupComponent.JoinGroup(playerData, request.GroupId);
        response.ErrorCode = errorCode;
        response.Group = group;
        response.State = state;
    }
}
