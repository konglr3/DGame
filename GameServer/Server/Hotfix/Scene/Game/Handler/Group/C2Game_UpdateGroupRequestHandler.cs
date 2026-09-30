using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_UpdateGroupRequestHandler
    : RoamingRPC<PlayerData, C2Game_UpdateGroupRequest, Game2C_UpdateGroupResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_UpdateGroupRequest request, Game2C_UpdateGroupResponse response, Action reply)
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

        var (errorCode, group) = await groupComponent.UpdateGroup(
            playerData,
            request.GroupId,
            request.Name,
            request.Description,
            request.AvatarUrl,
            request.LangTag,
            request.HasOpen,
            request.Open,
            request.MaxCount);
        response.ErrorCode = errorCode;
        response.Group = group;
    }
}
