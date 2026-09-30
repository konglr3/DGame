using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_CreateGroupRequestHandler
    : RoamingRPC<PlayerData, C2Game_CreateGroupRequest, Game2C_CreateGroupResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_CreateGroupRequest request, Game2C_CreateGroupResponse response, Action reply)
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

        var (errorCode, group) = await groupComponent.CreateGroup(
            playerData,
            request.Name,
            request.Description,
            request.AvatarUrl,
            request.LangTag,
            request.Open,
            request.MaxCount);
        response.ErrorCode = errorCode;
        response.Group = group;
    }
}
