using System;
using Fantasy;
using Fantasy.Async;
using Fantasy.Network.Interface;

namespace Hotfix;

public sealed class C2Game_UpdateGroupMetadataRequestHandler
    : RoamingRPC<PlayerData, C2Game_UpdateGroupMetadataRequest, Game2C_UpdateGroupMetadataResponse>
{
    protected override async FTask Run(PlayerData playerData, C2Game_UpdateGroupMetadataRequest request, Game2C_UpdateGroupMetadataResponse response, Action reply)
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

        var (errorCode, group) = await groupComponent.UpdateGroupMetadata(playerData, request.GroupId, request.Metadata);
        response.ErrorCode = errorCode;
        response.Group = group;
    }
}
